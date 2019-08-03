using System.Threading.Tasks;
using MetalMarketPlace.DataLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace MetalMarketPlace.Areas.Identity.Pages.Account.Manage
{
    public class ResetAuthenticatorModel : PageModel
    {
        private readonly UserManager<CompanyUser> _userManager;
        private readonly SignInManager<CompanyUser> _signInManager;
        private readonly ILogger<ResetAuthenticatorModel> _logger;
        private readonly IHtmlLocalizer<ResetAuthenticatorModel> _localizer;

        public ResetAuthenticatorModel(
            UserManager<CompanyUser> userManager,
            SignInManager<CompanyUser> signInManager,
            ILogger<ResetAuthenticatorModel> logger,
            IHtmlLocalizer<ResetAuthenticatorModel> localizer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _localizer = localizer;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound(_localizer.GetString("Unable to load user with ID '{0}'.", _userManager.GetUserId(User)));

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound(_localizer.GetString("Unable to load user with ID '{0}'.", _userManager.GetUserId(User)));

            await _userManager.SetTwoFactorEnabledAsync(user, false);
            await _userManager.ResetAuthenticatorKeyAsync(user);
            _logger.LogInformation("User with ID '{UserId}' has reset their authentication app key.", user.Id);

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = _localizer.GetString("Your authenticator app key has been reset, you will need to configure your authenticator app using the new key.");

            return RedirectToPage("./EnableAuthenticator");
        }
    }
}