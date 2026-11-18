using System.ComponentModel.DataAnnotations;

namespace Soleria.Web.Models;

public class Cart
{
    public int Id { get; set; }

    /// <summary>Üyeye bağlı sepet ise kullanıcı Id'si, ziyaretçi sepetinde null.</summary>
    public string? UserId { get; set; }
    public AppUser? User { get; set; }

    /// <summary>Ziyaretçi sepeti için çerezde tutulan rastgele anahtar.</summary>
    [MaxLength(64)]
    public string? SessionKey { get; set; }

    public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;
    public DateTime GuncellemeTarihi { get; set; } = DateTime.UtcNow;

    public List<CartItem> Items { get; set; } = new();

    public decimal Toplam => Items.Sum(i => (i.Product?.Fiyat ?? 0m) * i.Adet);
    public int ToplamAdet => Items.Sum(i => i.Adet);
}

public class CartItem
{
    public int Id { get; set; }

    public int CartId { get; set; }
    public Cart? Cart { get; set; }

    public int ProductId { get; set; }
    public Product? Product { get; set; }

    [MaxLength(10)]
    public string Beden { get; set; } = "";

    public int Adet { get; set; }
}
