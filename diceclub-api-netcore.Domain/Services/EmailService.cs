using diceclub_api_netcore.Domain.Entities;
using diceclub_api_netcore.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace diceclub_api_netcore.Domain.Services
{
    public class EmailService : IEmailService
    {
        public IEmailSender emailSender { get; set; }

        public EmailService(IEmailSender emailSender)
        {
            this.emailSender = emailSender;
        }

        public async Task SendEmailConfirmationAsync(User user, string confirmationLink)
        {
            try
            {
                var email = user.Email;

                if (email == null)
                {
                    throw new ArgumentNullException(nameof(email) + $" null for user {user.UserName}");
                }

                var html = await File.ReadAllTextAsync("../Templates/EmailConfirmation.html");

                html = html.Replace("ConfirmationLink", confirmationLink);

                await emailSender.SendEmailAsync(email, "Email Confirmation", html);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
