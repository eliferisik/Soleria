using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Soleria.Web.Data;
using Soleria.Web.Models;
using Soleria.Web.Services;
using Microsoft.AspNetCore.Identity;

namespace Soleria.Web.Pages;

public class UrunModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly ICartService _cart;
    private readonly UserManager<AppUser> _users;

    public UrunModel(AppDbContext db, ICartService cart, UserManager<AppUser> users)
    {
        _db = db;
        _cart = cart;
        _users = users;
    }

    public Product? Urun { get; private set; }
    public bool Favoride { get; private set; }

    [BindProperty]
    public SepetFormu Form { get; set; } = new();

    public class SepetFormu
    {
        [Required(ErrorMessage = "Lütfen bir beden seçin.")]
        public string? Beden { get; set; }

        [Range(1, CartService.MaxAdet, ErrorMessage = "Adet 1 ile 10 arasında olmalıdır.")]
        public int Adet { get; set; } = 1;
    }

    private Task<Product?> UrunuGetir(int id) =>
        _db.Products.AsNoTracking().Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);

    private async Task FavoriDurumunuAyarlaAsync(int id)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            var userId = _users.GetUserId(User);
            Favoride = await _db.Favorites.AnyAsync(f => f.UserId == userId && f.ProductId == id);
        }
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Urun = await UrunuGetir(id);
        if (Urun != null)
        {
            ProductImageOrder.SwapFirstTwo(new[] { Urun });
            ProductImageOrder.SwapRequestedProducts(new[] { Urun });
        }
        await FavoriDurumunuAyarlaAsync(id);
        return Urun == null ? NotFound() : Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        Urun = await UrunuGetir(id);
        if (Urun == null) return NotFound();
        ProductImageOrder.SwapFirstTwo(new[] { Urun });
        ProductImageOrder.SwapRequestedProducts(new[] { Urun });
        await FavoriDurumunuAyarlaAsync(id);

        if (!string.IsNullOrEmpty(Form.Beden) && !Urun.BedenListesi.Contains(Form.Beden))
            ModelState.AddModelError("Form.Beden", "Geçersiz beden seçimi.");

        if (!ModelState.IsValid) return Page();

        if (!await _cart.AddAsync(id, Form.Beden!, Form.Adet))
        {
            ModelState.AddModelError(string.Empty, "Ürün sepete eklenemedi. Lütfen tekrar deneyin.");
            return Page();
        }

        TempData["Mesaj"] = "Ürün sepetinize eklendi.";
        return RedirectToPage("/Sepet");
    }
}
