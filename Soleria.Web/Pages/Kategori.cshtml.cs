using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Soleria.Web.Data;
using Soleria.Web.Models;
using Soleria.Web.Services;

namespace Soleria.Web.Pages;

public class KategoriModel : PageModel
{
    private readonly AppDbContext _db;
    public KategoriModel(AppDbContext db) => _db = db;

    public Category? Kategori { get; private set; }
    public List<Product> Urunler { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync(string slug)
    {
        Kategori = await _db.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Slug == slug);
        if (Kategori == null) return NotFound();

        Urunler = await _db.Products.AsNoTracking()
            .Where(p => p.CategoryId == Kategori.Id)
            .OrderBy(p => p.Id)
            .ToListAsync();

        ProductImageOrder.SwapFirstTwo(Urunler);
        ProductImageOrder.SwapRequestedProducts(Urunler);

        return Page();
    }
}
