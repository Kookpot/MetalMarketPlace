using Microsoft.AspNetCore.Identity;

namespace MetalMarketPlace.DataLayer.Entities
{
    public class CompanyUser : IdentityUser
    {
        [PersonalData]
        public string CompanyName { get; set; }
    }
}