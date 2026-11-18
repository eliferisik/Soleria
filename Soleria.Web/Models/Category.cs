using System.ComponentModel.DataAnnotations;

namespace Soleria.Web.Models;

public class Category
{
    public int Id { get; set; }

    [MaxLength(100)]
    public string Ad { get; set; } = "";

    [MaxLength(100)]
    public string Slug { get; set; } = "";

    /// <summary>Kadın, Kız Çocuk, Erkek Çocuk, Erkek</summary>
    [MaxLength(40)]
    public string Grup { get; set; } = "";

    public int Sira { get; set; }

    public List<Product> Urunler { get; set; } = new();
}
