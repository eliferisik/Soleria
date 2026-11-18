using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Soleria.Web.Models;

namespace Soleria.Web.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Favorite> Favorites => Set<Favorite>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        b.Entity<Category>(e =>
        {
            e.HasIndex(c => c.Slug).IsUnique();
        });

        b.Entity<Product>(e =>
        {
            e.Property(p => p.Fiyat).HasPrecision(18, 2);
            e.HasOne(p => p.Category).WithMany(c => c.Urunler)
                .HasForeignKey(p => p.CategoryId).OnDelete(DeleteBehavior.Restrict);
        });

        b.Entity<Cart>(e =>
        {
            e.HasIndex(c => c.SessionKey);
            e.HasIndex(c => c.UserId).IsUnique(); // bir üyenin tek sepeti; NULL değerler çakışmaz
            e.HasOne(c => c.User).WithMany().HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasMany(c => c.Items).WithOne(i => i.Cart).HasForeignKey(i => i.CartId).OnDelete(DeleteBehavior.Cascade);
        });

        b.Entity<CartItem>(e =>
        {
            e.HasIndex(i => new { i.CartId, i.ProductId, i.Beden }).IsUnique();
            e.HasOne(i => i.Product).WithMany().HasForeignKey(i => i.ProductId).OnDelete(DeleteBehavior.Restrict);
        });

        b.Entity<Favorite>(e =>
        {
            e.HasIndex(f => new { f.UserId, f.ProductId }).IsUnique();
            e.HasOne(f => f.User).WithMany().HasForeignKey(f => f.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(f => f.Product).WithMany().HasForeignKey(f => f.ProductId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}
