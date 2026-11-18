using Microsoft.AspNetCore.Mvc;
using Soleria.Web.Services;

namespace Soleria.Web.ViewComponents;

public class SepetSayisiViewComponent : ViewComponent
{
    private readonly ICartService _cart;
    public SepetSayisiViewComponent(ICartService cart) => _cart = cart;

    public async Task<IViewComponentResult> InvokeAsync()
    {
        int adet = await _cart.CountAsync();
        return View(adet);
    }
}
