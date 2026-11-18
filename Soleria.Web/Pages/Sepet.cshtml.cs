using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Soleria.Web.Models;
using Soleria.Web.Services;

namespace Soleria.Web.Pages;

public class SepetModel : PageModel
{
    private readonly ICartService _cart;
    public SepetModel(ICartService cart) => _cart = cart;

    public Cart? Sepet { get; private set; }

    public async Task OnGetAsync()
    {
        Sepet = await _cart.GetCartAsync();
    }

    public async Task<IActionResult> OnPostGuncelleAsync(int itemId, int adet)
    {
        if (adet < 1 || adet > CartService.MaxAdet)
            TempData["Hata"] = $"Adet 1 ile {CartService.MaxAdet} arasında olmalıdır.";
        else if (!await _cart.UpdateAsync(itemId, adet))
            TempData["Hata"] = "Ürün sepetinizde bulunamadı.";
        else
            TempData["Mesaj"] = "Sepetiniz güncellendi.";

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostKaldirAsync(int itemId)
    {
        if (await _cart.RemoveAsync(itemId))
            TempData["Mesaj"] = "Ürün sepetten kaldırıldı.";
        else
            TempData["Hata"] = "Ürün sepetinizde bulunamadı.";

        return RedirectToPage();
    }
}
