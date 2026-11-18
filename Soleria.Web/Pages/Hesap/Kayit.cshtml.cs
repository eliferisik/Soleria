using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Soleria.Web.Models;
using Soleria.Web.Services;

namespace Soleria.Web.Pages.Hesap;

public class KayitModel : PageModel
{
    private readonly UserManager<AppUser> _users;
    private readonly SignInManager<AppUser> _signIn;
    private readonly ICartService _cart;

    public KayitModel(UserManager<AppUser> users, SignInManager<AppUser> signIn, ICartService cart)
    {
        _users = users;
        _signIn = signIn;
        _cart = cart;
    }

    [BindProperty]
    public GirdiModeli Girdi { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public class GirdiModeli
    {
        [Required(ErrorMessage = "Ad zorunludur.")]
        [StringLength(60, ErrorMessage = "Ad en fazla 60 karakter olabilir.")]
        public string Ad { get; set; } = "";

        [Required(ErrorMessage = "Soyad zorunludur.")]
        [StringLength(60, ErrorMessage = "Soyad en fazla 60 karakter olabilir.")]
        public string Soyad { get; set; } = "";

        [Phone(ErrorMessage = "Geçerli bir telefon numarası girin.")]
        [StringLength(20, ErrorMessage = "Telefon en fazla 20 karakter olabilir.")]
        public string? Telefon { get; set; }

        [Required(ErrorMessage = "E-posta zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
        [StringLength(256, ErrorMessage = "E-posta en fazla 256 karakter olabilir.")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Parola zorunludur.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Parola en az 8 karakter olmalıdır.")]
        public string Parola { get; set; } = "";

        [Required(ErrorMessage = "Parolayı tekrar girin.")]
        [DataType(DataType.Password)]
        [Compare(nameof(Parola), ErrorMessage = "Parolalar eşleşmiyor.")]
        public string ParolaTekrar { get; set; } = "";
    }

    private static bool BoslukIceriyor(string? value) =>
        !string.IsNullOrEmpty(value) && value.Any(char.IsWhiteSpace);

    public IActionResult OnGet()
    {
        if (User.Identity?.IsAuthenticated == true) return LocalRedirect("/");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (BoslukIceriyor(Girdi.Ad)) ModelState.AddModelError("Girdi.Ad", "Ad alanında boşluk kullanmayın.");
        if (BoslukIceriyor(Girdi.Soyad)) ModelState.AddModelError("Girdi.Soyad", "Soyad alanında boşluk kullanmayın.");
        if (BoslukIceriyor(Girdi.Email)) ModelState.AddModelError("Girdi.Email", "E-posta adresinde boşluk kullanmayın.");
        if (BoslukIceriyor(Girdi.Parola)) ModelState.AddModelError("Girdi.Parola", "Parolada boşluk kullanmayın.");
        if (BoslukIceriyor(Girdi.ParolaTekrar)) ModelState.AddModelError("Girdi.ParolaTekrar", "Parolada boşluk kullanmayın.");
        if (!ModelState.IsValid) return Page();

        var email = Girdi.Email.Trim();
        var kullanici = new AppUser
        {
            UserName = email,
            Email = email,
            Ad = Girdi.Ad.Trim(),
            Soyad = Girdi.Soyad.Trim(),
            PhoneNumber = string.IsNullOrWhiteSpace(Girdi.Telefon) ? null : Girdi.Telefon.Trim(),
        };

        var sonuc = await _users.CreateAsync(kullanici, Girdi.Parola);
        if (!sonuc.Succeeded)
        {
            foreach (var mesaj in sonuc.Errors.Select(e => e.Description).Distinct())
                ModelState.AddModelError(string.Empty, mesaj);
            return Page();
        }

        await _signIn.SignInAsync(kullanici, isPersistent: false);
        await _cart.MergeGuestCartAsync(kullanici.Id);

        return LocalRedirect(Url.IsLocalUrl(ReturnUrl) ? ReturnUrl! : "/");
    }
}
