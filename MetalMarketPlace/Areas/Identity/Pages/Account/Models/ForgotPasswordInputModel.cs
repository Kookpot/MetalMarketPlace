using System.ComponentModel.DataAnnotations;

namespace MetalMarketPlace.Areas.Identity.Pages.Account.Models
{
    public class ForgotPasswordInputModel
    {
        [Required(ErrorMessage = "The {0} field is required.")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}