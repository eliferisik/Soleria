using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Soleria.Web.Models;
using Soleria.Web.Services;
using Soleria.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace Soleria.Web.Pages.Hesap;

public class GirisModel : PageModel
{
    private readonly UserManager<AppUser> _users;
    private readonly SignInManager<AppUser> _signIn;
    private readonly ICartService _cart;
    private readonly AppDbContext _db;

    public GirisModel(UserManager<AppUser> users, SignInManager<AppUser> signIn, ICartService cart, AppDbContext db)
    {
        _users = users;
        _signIn = signIn;
        _cart = cart;
        _db = db;
    }

    [BindProperty]
    public GirdiModeli Girdi { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public class GirdiModeli
    {
        [Required(ErrorMessage = "E-posta zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Parola zorunludur.")]
        [DataType(DataType.Password)]
        public string Parola { get; set; } = "";

        public bool BeniHatirla { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        if (User.Identity?.IsAuthenticated == true) return LocalRedirect("/");
        if (!string.IsNullOrWhiteSpace(ReturnUrl) && ReturnUrl.StartsWith("/urun/", StringComparison.Ordinal))
        {
            var tail = ReturnUrl[6..].Split('?', '#')[0];
            if (int.TryParse(tail, out var productId))
            {
                var userId = _users.GetUserId(User);
                if (!string.IsNullOrEmpty(userId) && await _db.Favorites.AnyAsync(f => f.UserId == userId && f.ProductId == productId))
                    TempData["FavoriGiris"] = "Favorilerinizi kullanmak için giriş yapın.";
            }
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var email = Girdi.Email.Trim();
        var sonuc = await _signIn.PasswordSignInAsync(email, Girdi.Parola, Girdi.BeniHatirla, lockoutOnFailure: true);

        if (sonuc.Succeeded)
        {
            var kullanici = await _users.FindByEmailAsync(email);
            if (kullanici != null)
                await _cart.MergeGuestCartAsync(kullanici.Id);

            return LocalRedirect(Url.IsLocalUrl(ReturnUrl) ? ReturnUrl! : "/");
        }

        ModelState.AddModelError(string.Empty, sonuc.IsLockedOut
            ? "Çok fazla başarısız deneme yapıldı. Lütfen birkaç dakika sonra tekrar deneyin."
            : "E-posta veya parola hatalı.");
        return Page();
    }
}
