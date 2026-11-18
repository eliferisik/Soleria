using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Soleria.Web.Data;
using Soleria.Web.Models;
using Soleria.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Gizli/yerel ayarlar için isteğe bağlı dosya (git'e eklenmez).
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

// --- SQLite bağlantısı: göreli yol proje klasörüne göre çözülür ---
var csb = new SqliteConnectionStringBuilder(
    builder.Configuration.GetConnectionString("SoleriaDb") ?? "Data Source=App_Data/soleria.db");
if (!Path.IsPathRooted(csb.DataSource))
    csb.DataSource = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, csb.DataSource));
Directory.CreateDirectory(Path.GetDirectoryName(csb.DataSource)!);

builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlite(csb.ToString()));

// --- ASP.NET Core Identity (parolalar PBKDF2 ile hash'lenir, açık metin saklanmaz) ---
builder.Services.AddIdentity<AppUser, IdentityRole>(o =>
    {
        o.User.RequireUniqueEmail = true;
        o.Password.RequiredLength = 8;
        o.Password.RequireDigit = true;
        o.Password.RequireLowercase = true;
        o.Password.RequireUppercase = false;
        o.Password.RequireNonAlphanumeric = false;
        o.Lockout.MaxFailedAccessAttempts = 5;
        o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddErrorDescriber<TurkceIdentityErrorDescriber>();

builder.Services.ConfigureApplicationCookie(o =>
{
    o.LoginPath = "/hesap/giris";
    o.AccessDeniedPath = "/hesap/giris";
    o.Cookie.Name = "soleria_oturum";
    o.Cookie.HttpOnly = true;
    o.Cookie.SameSite = SameSiteMode.Lax;
    o.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    o.ExpireTimeSpan = TimeSpan.FromDays(14);
    o.SlidingExpiration = true;
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICartService, CartService>();

// Razor Pages: POST istekleri için CSRF (antiforgery) doğrulaması varsayılan olarak açıktır.
builder.Services.AddRazorPages();

var app = builder.Build();

// --- Veritabanı: migration varsa uygula, yoksa tabloları oluştur; sonra başlangıç verisini ekle ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (db.Database.GetMigrations().Any())
        db.Database.Migrate();
    else
        db.Database.EnsureCreated();

    // Var olan SQLite veritabanını silmeden yeni ürün galerisi ve favori tablosunu ekle.
    var connection = db.Database.GetDbConnection();
    await connection.OpenAsync();
    await using (var check = connection.CreateCommand())
    {
        check.CommandText = "PRAGMA table_info('Products');";
        var hasGalleryColumn = false;
        await using var reader = await check.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            if (string.Equals(reader["name"]?.ToString(), "EkGorseller", StringComparison.OrdinalIgnoreCase)) hasGalleryColumn = true;
        await reader.CloseAsync();
        if (!hasGalleryColumn)
        {
            await using var alter = connection.CreateCommand();
            alter.CommandText = "ALTER TABLE Products ADD COLUMN EkGorseller TEXT NOT NULL DEFAULT '';";
            await alter.ExecuteNonQueryAsync();
        }
    }
    await using (var favorites = connection.CreateCommand())
    {
        favorites.CommandText = """
            CREATE TABLE IF NOT EXISTS Favorites (
                Id INTEGER NOT NULL CONSTRAINT PK_Favorites PRIMARY KEY AUTOINCREMENT,
                UserId TEXT NOT NULL,
                ProductId INTEGER NOT NULL,
                OlusturmaTarihi TEXT NOT NULL,
                CONSTRAINT FK_Favorites_AspNetUsers_UserId FOREIGN KEY (UserId) REFERENCES AspNetUsers (Id) ON DELETE CASCADE,
                CONSTRAINT FK_Favorites_Products_ProductId FOREIGN KEY (ProductId) REFERENCES Products (Id) ON DELETE CASCADE
            );
            CREATE UNIQUE INDEX IF NOT EXISTS IX_Favorites_UserId_ProductId ON Favorites (UserId, ProductId);
            """;
        await favorites.ExecuteNonQueryAsync();
    }
    await connection.CloseAsync();

    await DbSeeder.SeedAsync(db);
}

var tr = new CultureInfo("tr-TR");
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(tr),
    SupportedCultures = new List<CultureInfo> { tr },
    SupportedUICultures = new List<CultureInfo> { tr },
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/hata");
    app.UseHsts();
    app.UseHttpsRedirection();
}
app.UseStatusCodePagesWithReExecute("/hata", "?kod={0}");

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

app.Run();
