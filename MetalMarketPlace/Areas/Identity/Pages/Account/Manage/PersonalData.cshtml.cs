using System.Threading.Tasks;
using MetalMarketPlace.DataLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace MetalMarketPlace.Areas.Identity.Pages.Account.Manage
{
    public class PersonalDataModel : PageModel
    {
        private readonly UserManager<CompanyUser> _userManager;
        private readonly ILogger<PersonalDataModel> _logger;
        private readonly IHtmlLocalizer<PersonalDataModel> _localizer;

        public PersonalDataModel(
            UserManager<CompanyUser> userManager,
            ILogger<PersonalDataModel> logger,
            IHtmlLocalizer<PersonalDataModel> localizer)
        {
            _userManager = userManager;
            _logger = logger;
            _localizer = localizer;
        }

        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound(_localizer.GetString("Unable to load user with ID '{0}'.", _userManager.GetUserId(User)));

            return Page();
        }
    }
}