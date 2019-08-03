using System.Threading.Tasks;
using MetalMarketPlace.Areas.Identity.Pages.Account.Manage.Models;
using MetalMarketPlace.DataLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MetalMarketPlace.Areas.Identity.Pages.Account.Manage
{
    public class SetPasswordModel : PageModel
    {
        private readonly UserManager<CompanyUser> _userManager;
        private readonly SignInManager<CompanyUser> _signInManager;
        private readonly IHtmlLocalizer<SetPasswordModel> _localizer;

        public SetPasswordModel(
            UserManager<CompanyUser> userManager,
            SignInManager<CompanyUser> signInManager,
            IHtmlLocalizer<SetPasswordModel> localizer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _localizer = localizer;
        }

        [BindProperty]
        public SetPasswordInputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }
        
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound(_localizer.GetString("Unable to load user with ID '{0}'.", _userManager.GetUserId(User)));

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (hasPassword)
                return RedirectToPage("./ChangePassword");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound(_localizer.GetString("Unable to load user with ID '{0}'.", _userManager.GetUserId(User)));

            var addPasswordResult = await _userManager.AddPasswordAsync(user, Input.NewPassword);
            if (!addPasswordResult.Succeeded)
            {
                foreach (var error in addPasswordResult.Errors)
                    ModelState.AddModelError(string.Empty, _localizer.GetString(error.Description));

                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = _localizer.GetString("Your password has been set.");

            return RedirectToPage();
        }
    }
}