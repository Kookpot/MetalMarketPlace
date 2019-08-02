using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace MetalMarketPlace.Areas.Landingpage.Pages.Main
{
    [AllowAnonymous]
    public class ErrorModel : PageModel
    {
        [BindProperty]
        public ErrorViewModel Error { get; set; }

        public class ErrorViewModel
        {
            public string RequestId { get; set; }

            public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        }

        public void OnGet()
        {
            Error.RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;          
        }
    }
}