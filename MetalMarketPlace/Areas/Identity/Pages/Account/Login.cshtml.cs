using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using MetalMarketPlace.DataLayer.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MetalMarketPlace.Areas.Identity.Pages.Account.Models;
using Microsoft.AspNetCore.Mvc.Localization;

namespace MetalMarketPlace.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<CompanyUser> _signInManager;
        private readonly UserManager<CompanyUser> _userManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly IHtmlLocalizer<LoginModel> _localizer;

        public LoginModel(SignInManager<CompanyUser> signInManager, ILogger<LoginModel> logger, UserManager<CompanyUser> userManager,
            IHtmlLocalizer<LoginModel> localizer)
        {
            _signInManager = signInManager;
            _logger = logger;
            _userManager = userManager;
            _localizer = localizer;
        }

        [BindProperty]
        public LoginInputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
                ModelState.AddModelError(string.Empty, _localizer.GetString(ErrorMessage));

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, Input.RememberMe });

                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    var user = await _userManager.FindByEmailAsync(Input.Email);
                    if (user != null && !await _userManager.IsEmailConfirmedAsync(user))
                    {
                        ModelState.AddModelError(string.Empty, _localizer.GetString("Emailadress hasn't been confirmed yet."));
                        return Page();
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, _localizer.GetString("Invalid login attempt."));
                        return Page();
                    }
                }
            }

            return Page();
        }
    }
}