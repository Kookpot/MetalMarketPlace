using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MetalMarketPlace.ConfigModels;
using MetalMarketPlace.DataLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MimeKit;
using reCAPTCHA.AspNetCore;

namespace MetalMarketPlace.Areas.Landingpage.Pages.Main
{
    [AllowAnonymous]
    public class ContactModel : PageModel
    {
        private readonly EmailConfiguration _emailConfiguration;
        private readonly IRecaptchaService _recaptcha;
        private readonly UserManager<CompanyUser> _userManager;

        public ContactModel(EmailConfiguration emailConfiguration, IRecaptchaService recaptcha, UserManager<CompanyUser> userManager)
        {
            _emailConfiguration = emailConfiguration;
            _recaptcha = recaptcha;
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Text)]
            [StringLength(100, ErrorMessage = "The {0} must be at max {1} characters long.")]
            [Display(Name = "Name")]
            public string Name { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email address")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at max {1} characters long.")]
            [DataType(DataType.Text)]
            [Display(Name = "Subject")]
            public string Subject { get; set; }

            [DataType(DataType.Text)]
            [StringLength(10000, ErrorMessage = "The {0} must be at max {1} characters long.")]
            [Display(Name = "Message")]
            public string Message { get; set; }
        }

        public async Task OnGet()
        {
            Input = new InputModel();
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
                Input.Email = await _userManager.GetEmailAsync(user);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var recaptcha = await _recaptcha.Validate(Request);
            if (!recaptcha.success)
            {
                ModelState.AddModelError(string.Empty, "There was an error validating recatpcha. Please try again!");
                return Page();
            }

            var message = new MimeMessage();
            var from = new MailboxAddress("Admin", "admin@metalmarketplace.com");
            message.From.Add(from);
            var to = new MailboxAddress("Koen", "k.plasmans@tiptec.be");
            message.To.Add(to);
            message.Subject = $"Contact form send : {Input.Subject}";
            var bodyBuilder = new BodyBuilder
            {
                TextBody = $"Message from {Input.Name} @ {Input.Email}{Environment.NewLine}{Input.Message}"
            };
            message.Body = bodyBuilder.ToMessageBody();
            using (var emailClient = new SmtpClient())
            {
                //The last parameter here is to use SSL (Which you should!)
                await emailClient.ConnectAsync(_emailConfiguration.SmtpServer, _emailConfiguration.SmtpPort, false);
                //Remove any OAuth functionality as we won't be using it. 
                emailClient.AuthenticationMechanisms.Remove("XOAUTH2");
                await emailClient.AuthenticateAsync(_emailConfiguration.SmtpUsername, _emailConfiguration.SmtpPassword);
                await emailClient.SendAsync(message);
                await emailClient.DisconnectAsync(true);
            }
            StatusMessage = "Your enquiry has been send to us. We will try to contact you with the needed assistance as soon as possible. Thank you!";
            Input.Message = string.Empty;
            Input.Subject = string.Empty;
            return Page();
        }
    }
}