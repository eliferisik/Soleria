using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Soleria.Web.Data;
using Soleria.Web.Models;
using Soleria.Web.Services;
using System.Text;

namespace Soleria.Web.Pages;

public class AraModel : PageModel
{
    private readonly AppDbContext _db;
    public AraModel(AppDbContext db) => _db = db;

    [BindProperty(SupportsGet = true)]
    public string? Q { get; set; }

    public List<Product> Sonuclar { get; private set; } = new();

    public async Task OnGetAsync()
    {
        Q = Q?.Trim();
        if (string.IsNullOrEmpty(Q)) return;
        if (Q.Length > 100) Q = Q[..100];

        // SQLite büyük/küçük harf duyarsız karşılaştırmayı Türkçe karakterlerde (İ, ı, ş...)
        // yapamadığı için katalog küçük olduğundan filtre bellekte Türkçe kültürle yapılır.
        var tum = await _db.Products.AsNoTracking().OrderBy(p => p.Id).ToListAsync();
        var karsilastirma = Bicim.TrKultur.CompareInfo;
        var kategoriAdlari = await _db.Categories.AsNoTracking()
            .ToDictionaryAsync(c => c.Id, c => c.Ad);
        var sorgu = AramaIcinNormallestir(Q);
        Sonuclar = tum
            .Where(p => karsilastirma.IndexOf(p.Ad, Q, CompareOptions.IgnoreCase) >= 0
                || AramaIcinNormallestir(p.Ad).Contains(sorgu, StringComparison.Ordinal)
                || (kategoriAdlari.TryGetValue(p.CategoryId, out var kategori) &&
                    (karsilastirma.IndexOf(kategori, Q, CompareOptions.IgnoreCase) >= 0
                     || AramaIcinNormallestir(kategori).Contains(sorgu, StringComparison.Ordinal))))
            .ToList();
        ProductImageOrder.SwapFirstTwo(Sonuclar);
        ProductImageOrder.SwapRequestedProducts(Sonuclar);
    }

    private static string AramaIcinNormallestir(string metin)
    {
        var kaynak = metin.ToLower(Bicim.TrKultur).Normalize(NormalizationForm.FormD);
        var sonuc = new StringBuilder(kaynak.Length);
        foreach (var karakter in kaynak)
            if (CharUnicodeInfo.GetUnicodeCategory(karakter) != UnicodeCategory.NonSpacingMark)
                sonuc.Append(karakter == 'ı' ? 'i' : karakter);
        return sonuc.ToString().Normalize(NormalizationForm.FormC);
    }
}
