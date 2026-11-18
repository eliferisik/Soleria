using System.ComponentModel.DataAnnotations;

namespace Soleria.Web.Models;

public class Product
{
    public int Id { get; set; }

    [MaxLength(200)]
    public string Ad { get; set; } = "";

    [MaxLength(1000)]
    public string Aciklama { get; set; } = "";

    public decimal Fiyat { get; set; }

    /// <summary>Örn. /images/urun1.jpg (wwwroot/images altında)</summary>
    [MaxLength(200)]
    public string GorselYolu { get; set; } = "";

    [MaxLength(1000)]
    public string EkGorseller { get; set; } = "";

    public IReadOnlyList<string> GorselListesi =>
        (new[] { GorselYolu }).Concat(EkGorseller.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();

    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    /// <summary>Virgülle ayrılmış beden listesi: "36,37,38"</summary>
    [MaxLength(200)]
    public string Bedenler { get; set; } = "";

    /// <summary>Her satır "Başlık: Değer" biçiminde bir özellik.</summary>
    [MaxLength(1000)]
    public string Ozellikler { get; set; } = "";

    // Salt okunur yardımcılar (set'i olmadığı için veritabanına eşlenmez)
    public IReadOnlyList<string> BedenListesi =>
        Bedenler.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    public IReadOnlyList<(string Baslik, string Deger)> OzellikListesi =>
        Ozellikler.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(s =>
            {
                var i = s.IndexOf(':');
                return i < 0 ? ("", s) : (s[..i].Trim(), s[(i + 1)..].Trim());
            })
            .ToList();
}
