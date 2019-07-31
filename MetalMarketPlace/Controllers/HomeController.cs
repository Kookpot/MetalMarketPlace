using MailKit.Net.Smtp;
using MetalMarketPlace.ConfigModels;
using MetalMarketPlace.Models;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using reCAPTCHA.AspNetCore;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MetalMarketPlace.Controllers
{
    public class HomeController : Controller
    {
        private IRecaptchaService _recaptcha;
        private EmailConfiguration _emailConfiguration;

        public HomeController(IRecaptchaService recaptcha, EmailConfiguration emailConfiguration)
        {
            _recaptcha = recaptcha;
            _emailConfiguration = emailConfiguration;
        }

        public IActionResult Index() => View();

        public IActionResult Privacy() => View();

        public IActionResult Contact() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> SendContact(ContactModel model)
        {
            var recaptcha = await _recaptcha.Validate(Request);
            if (!recaptcha.success)
                ModelState.AddModelError(string.Empty, "There was an error validating recatpcha. Please try again!");

            var message = new MimeMessage();
            var from = new MailboxAddress("Admin", "admin@metalmarketplace.com");
            message.From.Add(from);
            var to = new MailboxAddress("Koen", "k.plasmans@tiptec.be");
            message.To.Add(to);
            message.Subject = $"Contact form send : {model.Subject}";
            var bodyBuilder = new BodyBuilder
            {
                TextBody = $"Message from {model.Name} @ {model.Email}{Environment.NewLine}{model.Message}"
            };
            message.Body = bodyBuilder.ToMessageBody();
            using (var emailClient = new SmtpClient())
            {
                //The last parameter here is to use SSL (Which you should!)
                emailClient.Connect(_emailConfiguration.SmtpServer, _emailConfiguration.SmtpPort, false);
                //Remove any OAuth functionality as we won't be using it. 
                emailClient.AuthenticationMechanisms.Remove("XOAUTH2");
                emailClient.Authenticate(_emailConfiguration.SmtpUsername, _emailConfiguration.SmtpPassword);
                emailClient.Send(message);
                emailClient.Disconnect(true);
            }
            return View(model);
        }
    }
}