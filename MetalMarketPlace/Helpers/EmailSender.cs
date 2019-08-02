using MailKit.Net.Smtp;
using MetalMarketPlace.ConfigModels;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;
using System.Threading.Tasks;

namespace MetalMarketPlace.Helpers
{
    public class EmailSender : IEmailSender
    {
        private EmailConfiguration _emailConfiguration;

        public EmailSender(EmailConfiguration emailConfiguration)
        {
            _emailConfiguration = emailConfiguration;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var mimeMessage = new MimeMessage();
            var from = new MailboxAddress("Admin", "admin@metalmarketplace.com");
            mimeMessage.From.Add(from);
            var to = new MailboxAddress(email);
            mimeMessage.To.Add(to);
            mimeMessage.Subject = subject;
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = message,
                TextBody = message
            };
            mimeMessage.Body = bodyBuilder.ToMessageBody();
            using (var emailClient = new SmtpClient())
            {
                //The last parameter here is to use SSL (Which you should!)
                await emailClient.ConnectAsync(_emailConfiguration.SmtpServer, _emailConfiguration.SmtpPort, false);
                //Remove any OAuth functionality as we won't be using it. 
                emailClient.AuthenticationMechanisms.Remove("XOAUTH2");
                await emailClient.AuthenticateAsync(_emailConfiguration.SmtpUsername, _emailConfiguration.SmtpPassword);
                await emailClient.SendAsync(mimeMessage);
                await emailClient.DisconnectAsync(true);
            }
        }
    }
}