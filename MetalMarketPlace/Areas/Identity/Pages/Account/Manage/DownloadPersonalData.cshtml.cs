using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetalMarketPlace.DataLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MetalMarketPlace.Areas.Identity.Pages.Account.Manage
{
    public class DownloadPersonalDataModel : PageModel
    {
        private readonly UserManager<CompanyUser> _userManager;
        private readonly ILogger<DownloadPersonalDataModel> _logger;
        private readonly IHtmlLocalizer<Disable2faModel> _localizer;

        public DownloadPersonalDataModel(
            UserManager<CompanyUser> userManager,
            ILogger<DownloadPersonalDataModel> logger,
            IHtmlLocalizer<Disable2faModel> localizer)
        {
            _userManager = userManager;
            _logger = logger;
            _localizer = localizer;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound(_localizer.GetString("Unable to load user with ID '{0}'.", _userManager.GetUserId(User)));

            _logger.LogInformation("User with ID '{UserId}' asked for their personal data.", _userManager.GetUserId(User));

            // Only include personal data for download
            var personalData = new Dictionary<string, string>();
            var personalDataProps = typeof(CompanyUser).GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
            foreach (var p in personalDataProps.Where(x => x.Name != "UserName" && x.Name != "PhoneNumber" && x.Name != "PhoneNumberConfirmed"))
                personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");

            Response.Headers.Add("Content-Disposition", "attachment; filename=PersonalData.json");
            return new FileContentResult(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(personalData)), "text/json");
        }
    }
}