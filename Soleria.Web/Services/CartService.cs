using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Soleria.Web.Data;
using Soleria.Web.Models;

namespace Soleria.Web.Services;

public interface ICartService
{
    /// <summary>Geçerli kullanıcı/ziyaretçinin sepetini getirir. olustur=true ise yoksa oluşturur.</summary>
    Task<Cart?> GetCartAsync(bool olustur = false);
    Task<int> CountAsync();
    Task<bool> AddAsync(int productId, string beden, int adet);
    Task<bool> UpdateAsync(int itemId, int adet);
    Task<bool> RemoveAsync(int itemId);
    /// <summary>Giriş/kayıt sonrası ziyaretçi sepetini üyenin sepetiyle birleştirir.</summary>
    Task MergeGuestCartAsync(string userId);
}

/// <summary>
/// Sepet veritabanında tutulur.
///  - Giriş yapmış kullanıcı: Cart.UserId ile eşleşir.
///  - Ziyaretçi: "soleria_sepet" çerezindeki rastgele anahtar (Cart.SessionKey) ile eşleşir (30 gün).
/// </summary>
public class CartService : ICartService
{
    public const string CerezAdi = "soleria_sepet";
    public const int MaxAdet = 10;

    private readonly AppDbContext _db;
    private readonly IHttpContextAccessor _accessor;

    public CartService(AppDbContext db, IHttpContextAccessor accessor)
    {
        _db = db;
        _accessor = accessor;
    }

    private HttpContext Http => _accessor.HttpContext ?? throw new InvalidOperationException("HttpContext yok.");

    private string? UserId =>
        Http.User.Identity?.IsAuthenticated == true ? Http.User.FindFirstValue(ClaimTypes.NameIdentifier) : null;

    private IQueryable<Cart> SepetSorgusu() =>
        _db.Carts.Include(c => c.Items).ThenInclude(i => i.Product);

    private string? CerezAnahtari()
    {
        var key = Http.Request.Cookies[CerezAdi];
        return string.IsNullOrWhiteSpace(key) || key.Length > 64 ? null : key;
    }

    public async Task<Cart?> GetCartAsync(bool olustur = false)
    {
        var uid = UserId;
        Cart? cart;

        if (uid != null)
        {
            cart = await SepetSorgusu().FirstOrDefaultAsync(c => c.UserId == uid);
            if (cart == null && olustur)
            {
                cart = new Cart { UserId = uid };
                _db.Carts.Add(cart);
                await _db.SaveChangesAsync();
            }
            return cart;
        }

        var anahtar = CerezAnahtari();
        cart = anahtar == null
            ? null
            : await SepetSorgusu().FirstOrDefaultAsync(c => c.UserId == null && c.SessionKey == anahtar);

        if (cart == null && olustur)
        {
            anahtar = Guid.NewGuid().ToString("N");
            cart = new Cart { SessionKey = anahtar };
            _db.Carts.Add(cart);
            await _db.SaveChangesAsync();

            Http.Response.Cookies.Append(CerezAdi, anahtar, new CookieOptions
            {
                HttpOnly = true,
                IsEssential = true,
                SameSite = SameSiteMode.Lax,
                Secure = Http.Request.IsHttps,
                Expires = DateTimeOffset.UtcNow.AddDays(30),
            });
        }
        return cart;
    }

    public async Task<int> CountAsync()
    {
        var uid = UserId;
        if (uid != null)
            return await _db.CartItems.Where(i => i.Cart!.UserId == uid).SumAsync(i => i.Adet);

        var anahtar = CerezAnahtari();
        if (anahtar == null) return 0;
        return await _db.CartItems
            .Where(i => i.Cart!.UserId == null && i.Cart.SessionKey == anahtar)
            .SumAsync(i => i.Adet);
    }

    public async Task<bool> AddAsync(int productId, string beden, int adet)
    {
        if (adet < 1 || adet > MaxAdet) return false;

        var urun = await _db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == productId);
        if (urun == null || !urun.BedenListesi.Contains(beden)) return false;

        var cart = await GetCartAsync(olustur: true);
        if (cart == null) return false;

        var satir = cart.Items.FirstOrDefault(i => i.ProductId == productId && i.Beden == beden);
        if (satir == null)
            cart.Items.Add(new CartItem { ProductId = productId, Beden = beden, Adet = adet });
        else
            satir.Adet = Math.Min(MaxAdet, satir.Adet + adet);

        cart.GuncellemeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateAsync(int itemId, int adet)
    {
        if (adet < 1 || adet > MaxAdet) return false;
        var cart = await GetCartAsync();
        var satir = cart?.Items.FirstOrDefault(i => i.Id == itemId);
        if (cart == null || satir == null) return false;

        satir.Adet = adet;
        cart.GuncellemeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveAsync(int itemId)
    {
        var cart = await GetCartAsync();
        var satir = cart?.Items.FirstOrDefault(i => i.Id == itemId);
        if (cart == null || satir == null) return false;

        cart.Items.Remove(satir);
        _db.CartItems.Remove(satir);
        cart.GuncellemeTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task MergeGuestCartAsync(string userId)
    {
        var anahtar = CerezAnahtari();
        if (anahtar == null) return;

        var misafir = await SepetSorgusu().FirstOrDefaultAsync(c => c.UserId == null && c.SessionKey == anahtar);
        if (misafir != null)
        {
            var uyeSepeti = await SepetSorgusu().FirstOrDefaultAsync(c => c.UserId == userId);
            if (uyeSepeti == null)
            {
                // Üyenin sepeti yok: ziyaretçi sepetini üyeye devret.
                misafir.UserId = userId;
                misafir.SessionKey = null;
                misafir.GuncellemeTarihi = DateTime.UtcNow;
            }
            else
            {
                foreach (var s in misafir.Items.ToList())
                {
                    var mevcut = uyeSepeti.Items.FirstOrDefault(i => i.ProductId == s.ProductId && i.Beden == s.Beden);
                    if (mevcut != null)
                        mevcut.Adet = Math.Min(MaxAdet, mevcut.Adet + s.Adet);
                    else
                        uyeSepeti.Items.Add(new CartItem { ProductId = s.ProductId, Beden = s.Beden, Adet = s.Adet });
                }
                uyeSepeti.GuncellemeTarihi = DateTime.UtcNow;
                _db.Carts.Remove(misafir);
            }
            await _db.SaveChangesAsync();
        }

        Http.Response.Cookies.Delete(CerezAdi);
    }
}
