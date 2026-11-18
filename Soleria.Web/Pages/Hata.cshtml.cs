using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Soleria.Web.Pages;

[IgnoreAntiforgeryToken]
public class HataModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public int? Kod { get; set; }

    public void OnGet() { }
    public void OnPost() { }
}
