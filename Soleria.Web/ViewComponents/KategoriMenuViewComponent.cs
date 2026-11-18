using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soleria.Web.Data;

namespace Soleria.Web.ViewComponents;

public class KategoriMenuViewComponent : ViewComponent
{
    private readonly AppDbContext _db;
    public KategoriMenuViewComponent(AppDbContext db) => _db = db;

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var kategoriler = await _db.Categories.AsNoTracking()
            .Where(c => !c.Slug.EndsWith("-terlik")
                && c.Slug != "kiz-cocuk-klasik-ayakkabi"
                && c.Slug != "erkek-babet"
                && c.Slug != "erkek-cocuk-babet")
            .ToListAsync();
        var grupSirasi = new[] { "Kadın", "Erkek", "Kız Çocuk", "Erkek Çocuk" };
        kategoriler = kategoriler
            .OrderBy(c => Array.IndexOf(grupSirasi, c.Grup))
            .ThenBy(c => c.Sira)
            .ToList();
        return View(kategoriler);
    }
}
