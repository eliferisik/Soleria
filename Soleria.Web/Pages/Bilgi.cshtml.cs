using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Soleria.Web.Pages;

/// <summary>Yardım, hakkımızda ve diğer bilgilendirme sayfaları.</summary>
public class BilgiModel : PageModel
{
    private static readonly Dictionary<string, string> Basliklar = new()
    {
        ["magaza-bul"] = "Mağaza Bul",
        ["yardim"] = "Yardım",
        ["hakkimizda"] = "Hakkımızda",
        ["satis-yap"] = "Soleria'da Satış Yap",
        ["favorilerim"] = "Favorilerim",
        ["siparislerim"] = "Siparişlerim",
    };

    public string Baslik { get; private set; } = "";
    public string Mesaj { get; private set; } = "";
    public bool HazirIcerik { get; private set; }

    public IActionResult OnGet(string slug)
    {
        if (!Basliklar.TryGetValue(slug, out var baslik)) return NotFound();
        Baslik = baslik;
        HazirIcerik = slug is "yardim" or "hakkimizda";
        Mesaj = slug == "siparislerim"
            ? "Gerçek ödeme entegrasyonu olmadığı için şu anda sipariş oluşturulmuyor; bu sayfa ileride hazırlanacak."
            : "Bu bölüm henüz hazır değil. Çok yakında burada olacak.";
        return Page();
    }
}
