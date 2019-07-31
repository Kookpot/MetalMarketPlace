using System.ComponentModel.DataAnnotations;

namespace MetalMarketPlace.Models
{
    public class ContactModel
    {
        [MaxLength(255)]
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Message { get; set; }
    }
}