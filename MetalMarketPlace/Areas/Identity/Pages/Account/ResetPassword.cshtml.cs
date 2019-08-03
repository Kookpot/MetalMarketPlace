using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using MetalMarketPlace.DataLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MetalMarketPlace.Areas.Identity.Pages.Account.Models;
using Microsoft.AspNetCore.Mvc.Localization;

namespace MetalMarketPlace.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<CompanyUser> _userManager;
        private readonly IHtmlLocalizer<ResetPasswordModel> _localizer;

        public ResetPasswordModel(UserManager<CompanyUser> userManager,
             IHtmlLocalizer<ResetPasswordModel> localizer)
        {
            _userManager = userManager;
            _localizer = localizer;
        }

        [BindProperty]
        public ResetPasswordInputModel Input { get; set; }

        public IActionResult OnGet(string code = null)
        {
            if (code == null)
                return BadRequest(_localizer.GetString("A code must be supplied for password reset."));
            else
            {
                Input = new ResetPasswordInputModel
                {
                    Code = code
                };
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToPage("./ResetPasswordConfirmation");
            }

            var result = await _userManager.ResetPasswordAsync(user, Input.Code, Input.Password);
            if (result.Succeeded)
                return RedirectToPage("./ResetPasswordConfirmation");

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, _localizer.GetString(error.Description));

            return Page();
        }
    }
}