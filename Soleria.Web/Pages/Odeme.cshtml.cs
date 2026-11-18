using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Soleria.Web.Models;
using Soleria.Web.Services;

namespace Soleria.Web.Pages;

public class OdemeModel : PageModel
{
    private readonly ICartService _cart;
    public OdemeModel(ICartService cart) => _cart = cart;

    public Cart? Sepet { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            TempData["Hata"] = "Ödeme adımına geçmek için önce giriş yapmalısınız.";
            return RedirectToPage("/Hesap/Giris", new { returnUrl = "/Odeme" });
        }
        Sepet = await _cart.GetCartAsync();
        if (Sepet == null || Sepet.Items.Count == 0)
        {
            TempData["Hata"] = "Ödeme adımına geçmek için sepetinizde ürün olmalıdır.";
            return RedirectToPage("/Sepet");
        }
        return Page();
    }
}
