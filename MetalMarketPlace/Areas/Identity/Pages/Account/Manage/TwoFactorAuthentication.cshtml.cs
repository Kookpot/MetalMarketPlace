using System.Threading.Tasks;
using MetalMarketPlace.DataLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace MetalMarketPlace.Areas.Identity.Pages.Account.Manage
{
    public class TwoFactorAuthenticationModel : PageModel
    {
        private const string AuthenicatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}";

        private readonly UserManager<CompanyUser> _userManager;
        private readonly SignInManager<CompanyUser> _signInManager;
        private readonly ILogger<TwoFactorAuthenticationModel> _logger;
        private readonly IHtmlLocalizer<TwoFactorAuthenticationModel> _localizer;

        public TwoFactorAuthenticationModel(
            UserManager<CompanyUser> userManager,
            SignInManager<CompanyUser> signInManager,
            ILogger<TwoFactorAuthenticationModel> logger,
            IHtmlLocalizer<TwoFactorAuthenticationModel> localizer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _localizer = localizer;
        }

        public bool HasAuthenticator { get; set; }

        public int RecoveryCodesLeft { get; set; }

        [BindProperty]
        public bool Is2faEnabled { get; set; }

        public bool IsMachineRemembered { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound(_localizer.GetString("Unable to load user with ID '{0}'.", _userManager.GetUserId(User)));

            HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) != null;
            Is2faEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
            IsMachineRemembered = await _signInManager.IsTwoFactorClientRememberedAsync(user);
            RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user);

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound(_localizer.GetString("Unable to load user with ID '{0}'.", _userManager.GetUserId(User)));

            await _signInManager.ForgetTwoFactorClientAsync();
            StatusMessage = _localizer.GetString("The current browser has been forgotten. When you login again from this browser you will be prompted for your 2fa code.");
            return RedirectToPage();
        }
    }
}