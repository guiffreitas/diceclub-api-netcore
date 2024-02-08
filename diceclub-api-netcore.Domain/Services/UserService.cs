using diceclub_api_netcore.Domain.Interfaces.Services;
using diceclub_api_netcore.Domain.ValueObjects;
using Microsoft.AspNetCore.WebUtilities;
using diceclub_api_netcore.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Text;

namespace diceclub_api_netcore.Domain.Services
{
    public class UserService : IUserService
    {
        private readonly IEmailService emailService;
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        public UserService(IEmailService emailService, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            this.emailService = emailService;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public async Task<IdentityResult> RegisterUser(User user, string password)
        {
            try
            {
                var result = await userManager.CreateAsync(user, password);

                _ = Task.Run(async () => await SendConfirmationEmail(user));

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task SendConfirmationEmail(User user)
        {
            var confirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);

            confirmationToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(confirmationToken));

            var confirmationLink = ApiUrls.EmailConfirmationUrl + "userId=" + user.Id + "&confirmationTokem=" + confirmationToken;

            try
            {
                await emailService.SendEmailConfirmationAsync(user, confirmationLink);
            }
            catch (Exception ex)
            {
                throw new Exception("Error at confirmation email sender", ex);
            }

        }

        public async Task<IdentityResult> ConfirmEmail(string userId, string confirmationToken)
        {
            if (confirmationToken == null)
            {
                return IdentityResult.Failed(new IdentityError() { Description = "Invalid token confirmation"});
            }

            var user = await userManager.FindByIdAsync(userId);
           
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError() { Description = "User not found" });

            }

            var result = await userManager.ConfirmEmailAsync(user, confirmationToken);
            
            if (result.Succeeded)
            {
                return result;
            }

            return IdentityResult.Failed(new IdentityError() { Description = "User could not be confirmed" });
        }
    }
}
