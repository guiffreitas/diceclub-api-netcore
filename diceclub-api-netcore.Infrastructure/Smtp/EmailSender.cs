using diceclub_api_netcore.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace diceclub_api_netcore.Infrastructure.Smtp
{
    public class EmailSender : IEmailSender
    {
        private readonly MailSettings mailSettings;
        public EmailSender(IOptions<MailSettings> mailSettings) 
        {
            this.mailSettings = mailSettings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient(mailSettings.Server, mailSettings.Port)
            {
                Credentials = new NetworkCredential(mailSettings.UserName, mailSettings.Password),
                EnableSsl = true
            };

            var message = new MailMessage();
            message.From = new MailAddress(mailSettings.SenderEmail, mailSettings.SenderName);
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
