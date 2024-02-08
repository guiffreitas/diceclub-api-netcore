using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;
using diceclub_api_netcore.Domain.Entities;
using System.Text;
using System.Net.Mime;
using diceclub_api_netcore.Domain.ValueObjects;

namespace diceclub_api_netcore.Infrastructure.Smtp
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient(MailSettings.Server, MailSettings.Port)
            {
                Credentials = new NetworkCredential(MailSettings.UserName, MailSettings.Password),
                EnableSsl = true
            };

            var message = new MailMessage();
            message.From = new MailAddress(MailSettings.SenderEmail, MailSettings.SenderName);
            message.To.Add(new MailAddress(email));
            message.SubjectEncoding = Encoding.UTF8;
            message.BodyEncoding = Encoding.UTF8;
            message.Subject = subject;
            var alternativeView = AlternateView.CreateAlternateViewFromString(htmlMessage, new ContentType(MediaTypeNames.Text.Html));
            message.AlternateViews.Add(alternativeView);

            await client.SendMailAsync(message);
        }
    }
}
