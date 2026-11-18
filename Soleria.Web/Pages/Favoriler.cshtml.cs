using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Soleria.Web.Data;
using Soleria.Web.Models;
using Soleria.Web.Services;

namespace Soleria.Web.Pages;

[Authorize]
public class FavorilerModel : PageModel
{
    private readonly AppDbContext _db;
    public FavorilerModel(AppDbContext db) => _db = db;
    public List<Product> Urunler { get; private set; } = new();

    public async Task OnGetAsync()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
        Urunler = await _db.Favorites.Where(f => f.UserId == userId).Select(f => f.Product!).AsNoTracking().ToListAsync();
        ProductImageOrder.SwapFirstTwo(Urunler);
        ProductImageOrder.SwapRequestedProducts(Urunler);
    }

    public async Task<IActionResult> OnPostToggleAsync(int productId, string? returnUrl)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
        if (!await _db.Products.AnyAsync(p => p.Id == productId)) return NotFound();
        var favorite = await _db.Favorites.FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == productId);
        if (favorite == null)
        {
            _db.Favorites.Add(new Favorite { UserId = userId, ProductId = productId });
            TempData["Mesaj"] = "Ürün favorilerinize eklendi.";
        }
        else
        {
            _db.Favorites.Remove(favorite);
            TempData["Mesaj"] = "Ürün favorilerinizden çıkarıldı.";
        }
        await _db.SaveChangesAsync();
        return !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl) ? LocalRedirect(returnUrl) : RedirectToPage();
    }
}
