using System.ComponentModel.DataAnnotations;

namespace MetalMarketPlace.Areas.Landingpage.Pages.Main.Models
{
    public class ContactInputModel
    {
        [Required(ErrorMessage = "The {0} field is required.")]
        [DataType(DataType.Text)]
        [StringLength(100, ErrorMessage = "The {0} must be at max {1} characters long.")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [EmailAddress]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(100, ErrorMessage = "The {0} must be at max {1} characters long.")]
        [DataType(DataType.Text)]
        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [DataType(DataType.Text)]
        [StringLength(10000, ErrorMessage = "The {0} must be at max {1} characters long.")]
        [Display(Name = "Message")]
        public string Message { get; set; }
    }
}