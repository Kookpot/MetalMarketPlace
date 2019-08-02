using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using MetalMarketPlace.DataLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MetalMarketPlace.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<CompanyUser> _userManager;
        private readonly SignInManager<CompanyUser> _signInManager;

        public ConfirmEmailModel(UserManager<CompanyUser> userManager, SignInManager<CompanyUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> OnGetAsync(string userId, string code, bool isDone = false)
        {
            if (isDone)
                return Page();

            if (userId == null || code == null)
                return LocalRedirect("/Landingpage/Main/Index");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound($"Unable to load user with ID '{userId}'.");

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (!result.Succeeded)
                throw new InvalidOperationException($"Error confirming email for user with ID '{userId}':");           
            else
            {
                await _signInManager.SignInAsync(user, false);
                return RedirectToPage(new { isDone = true });
            }
        }
    }
}