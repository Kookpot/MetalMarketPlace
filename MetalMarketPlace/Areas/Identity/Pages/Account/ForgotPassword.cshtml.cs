using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using MetalMarketPlace.DataLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MetalMarketPlace.Areas.Identity.Pages.Account.Models;
using Microsoft.AspNetCore.Mvc.Localization;

namespace MetalMarketPlace.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<CompanyUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IHtmlLocalizer<ForgotPasswordModel> _localizer;

        public ForgotPasswordModel(UserManager<CompanyUser> userManager, IEmailSender emailSender,
            IHtmlLocalizer<ForgotPasswordModel> localizer)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _localizer = localizer;
        }

        [BindProperty]
        public ForgotPasswordInputModel Input { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                    return RedirectToPage("./ForgotPasswordConfirmation");

                // For more information on how to enable account confirmation and password reset please 
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { code },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(
                    Input.Email,
                    _localizer.GetString("Reset Password"),
                    $"{_localizer.GetString("Please reset your password by")} <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>{_localizer.GetString("clicking here")}</a>.");

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}