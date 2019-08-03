using System.ComponentModel.DataAnnotations;

namespace MetalMarketPlace.Areas.Identity.Pages.Account.Models
{
    public class ExternalLoginInputModel
    {
        [Required(ErrorMessage = "The {0} field is required.")]
        [DataType(DataType.Text)]
        [StringLength(100, ErrorMessage = "The {0} must be at max {1} characters long.")]
        [Display(Name = "Company name")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
