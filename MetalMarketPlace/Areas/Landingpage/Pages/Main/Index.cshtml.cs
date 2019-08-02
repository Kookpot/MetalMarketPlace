using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MetalMarketPlace.Areas.Landingpage.Pages.Main
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        public void OnGet() { }
    }
}