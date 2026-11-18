using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Soleria.Web.Data;
using Soleria.Web.Models;
using Soleria.Web.Services;

namespace Soleria.Web.Pages;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    public IndexModel(AppDbContext db) => _db = db;

    public List<Product> KadinUrunler { get; private set; } = new();
    public List<Product> KizUrunler { get; private set; } = new();
    public List<Product> ErkekCocukUrunler { get; private set; } = new();
    public List<Product> ErkekUrunler { get; private set; } = new();

    public async Task OnGetAsync()
    {
        var urunler = await _db.Products.AsNoTracking().Include(p => p.Category).OrderBy(p => p.Id).ToListAsync();
        KadinUrunler = urunler.Where(p => p.Category?.Grup == "Kadın").ToList();
        KizUrunler = urunler.Where(p => p.Category?.Grup == "Kız Çocuk").ToList();
        ErkekCocukUrunler = urunler.Where(p => p.Category?.Grup == "Erkek Çocuk").ToList();
        ErkekUrunler = urunler.Where(p => p.Category?.Grup == "Erkek").ToList();

        // Tüm ürün gruplarında ilk iki fotoğrafı aynı sırayla göster.
        ProductImageOrder.SwapFirstTwo(KadinUrunler);
        ProductImageOrder.SwapFirstTwo(KizUrunler);
        ProductImageOrder.SwapFirstTwo(ErkekCocukUrunler);
        ProductImageOrder.SwapFirstTwo(ErkekUrunler);

        // Mevcut genel sıralamayı koru; yalnızca istenen altı ürünü ayrıca değiştir.
        ProductImageOrder.SwapRequestedProducts(KizUrunler);
    }
}
