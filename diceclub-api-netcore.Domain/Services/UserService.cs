using diceclub_api_netcore.Domain.Entities;
using diceclub_api_netcore.Domain.Enums;
using diceclub_api_netcore.Domain.Interfaces.Repositories;
using diceclub_api_netcore.Domain.Interfaces.Services;
using diceclub_api_netcore.Domain.Models;
using diceclub_api_netcore.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System.Text;

namespace diceclub_api_netcore.Domain.Services
{
    public class UserService : IUserService
    {
        private readonly IEmailService emailService;
        private readonly ITokenService tokenService;
        private readonly ICacheRedisRepository cacheRepository;

        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        
        private readonly ApiUrls apiUrls;

        public UserService(
            IEmailService emailService, 
            ITokenService tokenService,
            ICacheRedisRepository cacheRepository,
            UserManager<User> userManager, 
            SignInManager<User> signInManager, 
            IOptions<ApiUrls> apiUrls)
        {
            this.emailService = emailService;
            this.tokenService = tokenService;
            this.cacheRepository = cacheRepository;
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

        public async Task<LoginModel> LoginUser(string email, string password)
        {
            try
            {
                var user = await userManager.FindByEmailAsync(email);

                if (user == default)
                {
                    return new LoginModel()
                    {
                        Success = false,
                        Message = $"User {email} could not be found",
                    };
                }

                var result = await signInManager.PasswordSignInAsync(user, password, false, false);

                if(!result.Succeeded)
                {
                    return new LoginModel()
                    {
                        Success = false,
                        Message = $"Login not allowed",
                    };
                }

                (var userToken, var refreshToken) = GenerateLoginTokens(user);

                if (string.IsNullOrWhiteSpace(userToken) || string.IsNullOrWhiteSpace(refreshToken))
                {
                    return new LoginModel()
                    {
                        Success = false,
                        Message = $"User's tokens could not be created",
                    };
                }

                await PersistLoginToken(user, userToken, refreshToken);

                return new LoginModel() 
                { 
                    Success = true,
                    Message = $"User {user.UserName} logged",
                    UserToken = userToken,
                    RefreshToken = refreshToken
                };
            }
            catch (Exception ex)
            {
                throw new Exception("User could not be signed up. Error: " + ex.Message, ex);
            }
        }

        public async Task<LoginModel> RefreshLogin(string userToken, string refreshToken)
        {
            try
            {
                var username = tokenService.GetUsernameFromToken(userToken);

                if (string.IsNullOrWhiteSpace(username))
                {
                    return new LoginModel()
                    {
                        Success = false,
                        Message = $"User's token is invalid",
                    };
                }

                var user = await userManager.FindByNameAsync(username);

                if(user == default)
                {
                    return new LoginModel()
                    {
                        Success = false,
                        Message = $"User not found",
                    };
                }

                var cacheRefreshToken = await cacheRepository.HashGetAsync<string>(string.Empty, user.Id.ToString(), CacheType.RefreshToken);

                if (cacheRefreshToken == default || !cacheRefreshToken!.Equals(refreshToken))
                {
                    return new LoginModel()
                    {
                        Success = false,
                        Message = $"Invalid refresh token",
                    };
                }

                (var newUserToken, var newRefreshToken) = GenerateLoginTokens(user);

                if (string.IsNullOrWhiteSpace(newUserToken) || string.IsNullOrWhiteSpace(newRefreshToken))
                {
                    return new LoginModel()
                    {
                        Success = false,
                        Message = $"User's tokens could not be created",
                    };
                }

                await PersistLoginToken(user, newUserToken, newRefreshToken);

                return new LoginModel()
                {
                    Success = true,
                    Message = $"User {user.UserName} logged",
                    UserToken = userToken,
                    RefreshToken = refreshToken
                };
            }
            catch(Exception ex)
            {
                throw new Exception("User could not be signed up. Error: " + ex.Message, ex);
            }
        }

        private (string UserToken, string RefreshToken) GenerateLoginTokens(User user)
        {
            var userToken = tokenService.GenerateUserToken(user);

            var refreshToken = tokenService.GenerateRefreshToken();

            return(userToken, refreshToken);
        }

        private async Task PersistLoginToken(User user,string userToken, string refreshToken)
        {
            var persistAuthToken = userManager.SetAuthenticationTokenAsync(user, string.Empty, Parameters.Token.AuthToken, userToken);

            var persistRefreshToken = cacheRepository.HashSetAsync(string.Empty, user.Id.ToString(), refreshToken, CacheType.RefreshToken, Parameters.Token.RefreshTokenExpiration);

            await Task.WhenAll(persistAuthToken, persistRefreshToken);
        }
    }
}
