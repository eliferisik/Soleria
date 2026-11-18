namespace Soleria.Web.Models;

public class Favorite
{
    public int Id { get; set; }
    public string UserId { get; set; } = "";
    public AppUser? User { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;
}
