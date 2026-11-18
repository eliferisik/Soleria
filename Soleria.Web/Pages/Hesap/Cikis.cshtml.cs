using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Soleria.Web.Models;

namespace Soleria.Web.Pages.Hesap;

public class CikisModel : PageModel
{
    private readonly SignInManager<AppUser> _signIn;
    public CikisModel(SignInManager<AppUser> signIn) => _signIn = signIn;

    // Çıkış yalnızca POST (antiforgery korumalı) ile yapılır; GET ile gelen anasayfaya yönlendirilir.
    public IActionResult OnGet() => RedirectToPage("/Index");

    public async Task<IActionResult> OnPostAsync()
    {
        await _signIn.SignOutAsync();
        TempData["Mesaj"] = "Çıkış yaptınız.";
        return RedirectToPage("/Index");
    }
}
