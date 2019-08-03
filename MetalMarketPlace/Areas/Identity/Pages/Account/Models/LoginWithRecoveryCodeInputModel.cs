using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MetalMarketPlace.Areas.Identity.Pages.Account.Models
{
    public class LoginWithRecoveryCodeInputModel
    {
        [BindProperty]
        [Required(ErrorMessage = "The {0} field is required.")]
        [DataType(DataType.Text)]
        [Display(Name = "Recovery Code")]
        public string RecoveryCode { get; set; }
    }
}