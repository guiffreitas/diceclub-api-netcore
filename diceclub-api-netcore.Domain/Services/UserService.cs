using diceclub_api_netcore.Domain.Interfaces.Services;
using diceclub_api_netcore.Domain.ValueObjects;
using Microsoft.AspNetCore.WebUtilities;
using diceclub_api_netcore.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using diceclub_api_netcore.Domain.Models;

namespace diceclub_api_netcore.Domain.Services
{
    public class UserService : IUserService
    {
        private readonly IEmailService emailService;
        private readonly ITokenService tokenService;

        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        
        private readonly ApiUrls apiUrls;

        public UserService(
            IEmailService emailService, 
            ITokenService tokenService, 
            UserManager<User> userManager, 
            SignInManager<User> signInManager, 
            IOptions<ApiUrls> apiUrls)
        {
            this.emailService = emailService;
            this.tokenService = tokenService;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.apiUrls = apiUrls.Value;
        }

        public async Task<IdentityResult> RegisterUser(User user, string password)
        {
            try
            {
                var result = await userManager.CreateAsync(user, password);

                if(result.Succeeded)
                {
                    await SendConfirmationEmail(user);

                    return result;
                }

                return IdentityResult.Failed(new IdentityError()
                {
                    Description = "User could not be registered. Error: " + result.Errors.First().Description,
                    Code = result.Errors.First().Code
                });
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

            var confirmationLink = apiUrls.EmailConfirmationUrl + "userId=" + user.Id + "&confirmationTokem=" + confirmationToken;

            try
            {
                await emailService.SendEmailConfirmationAsync(user, confirmationLink);
            }
            catch (Exception ex)
            {
                throw new Exception("Error at confirmation email sender: " + ex.Message, ex);
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

            var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(confirmationToken));

            var result = await userManager.ConfirmEmailAsync(user, token);
            
            if (result.Succeeded)
            {
                return result;
            }

            return IdentityResult.Failed(new IdentityError() 
            { 
                Description = "User could not be confirmed. Error: " + result.Errors.First().Description, 
                Code = result.Errors.First().Code
            }) ;
        }

        public async Task<(IdentityResult Result, string UserToken)> LoginUser(string email, string password)
        {
            try
            {
                var user = await userManager.FindByEmailAsync(email);

                if (user == default)
                {
                    return (IdentityResult.Failed(new IdentityError()
                    {
                        Description = $"User {email} could not be found",
                        Code = "404"
                    }), string.Empty);
                }

                var result = await signInManager.PasswordSignInAsync(user, password, false, false);

                if(!result.Succeeded)
                {
                    return (IdentityResult.Failed(new IdentityError()
                    {
                        Description = $"Login not allowed",
                        Code = "400"
                    }), string.Empty);
                }

                var userToken = tokenService.GetUserToken(user);

                if (!string.IsNullOrWhiteSpace(userToken)) 
                {
                    await userManager.SetAuthenticationTokenAsync(user, string.Empty, "auth_token", userToken);

                    return (IdentityResult.Success, userToken);
                }

                return (IdentityResult.Failed(new IdentityError()
                {
                    Description = $"User token could not be created",
                    Code = "500"
                }), string.Empty);
     
            }
            catch (Exception ex)
            {
                throw new Exception("User could not be signed up. Error: " + ex.Message, ex);
            }
        }

        public async Task<ResultModel<bool>> UserIsNew(User user)
        {
            try
            {
                var previousUser = await userManager.FindByEmailAsync(user.Email!);

                if(previousUser != default)
                {
                    return new ResultModel<bool> { Result = false, Message = $"Email {user.UserName} is already registered" };
                }

                previousUser = await userManager.FindByNameAsync(user.UserName!);

                if (previousUser != default)
                {
                    return new ResultModel<bool> { Result = false, Message = $"Username {user.UserName} is already been using" };
                }

                return new ResultModel<bool> { Result = true };
            }
            catch(Exception ex)
            {
                throw new Exception("User information could not be verified. Error: " + ex.Message, ex);
            }
        }
    }
}
