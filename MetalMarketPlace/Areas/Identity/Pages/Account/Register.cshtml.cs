using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using MetalMarketPlace.Areas.Identity.Pages.Account.Models;
using MetalMarketPlace.DataLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace MetalMarketPlace.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<CompanyUser> _signInManager;
        private readonly UserManager<CompanyUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IHtmlLocalizer<RegisterModel> _localizer;

        public RegisterModel(
            UserManager<CompanyUser> userManager,
            SignInManager<CompanyUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IHtmlLocalizer<RegisterModel> localizer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _localizer = localizer;
        }

        [BindProperty]
        public RegisterInputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = new CompanyUser { CompanyName = Input.CompanyName, UserName = Input.Email, Email = Input.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId = user.Id, code },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, _localizer.GetString("Confirm your email"),
                            $"{_localizer.GetString("Please confirm your account by")} <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>{_localizer.GetString("clicking here")}</a>.");

                    return RedirectToPage("./RegistrationComplete");
                }
                foreach (var error in result.Errors.Where(x => x.Code != "DuplicateUserName"))
                    ModelState.AddModelError(string.Empty, _localizer.GetString(error.Description));
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
