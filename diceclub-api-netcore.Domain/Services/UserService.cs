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
        private readonly IUserRepository userRepository;
        
        private readonly ApiUrls apiUrls;

        public UserService(
            IEmailService emailService, 
            ITokenService tokenService,
            ICacheRedisRepository cacheRepository,
            IUserRepository userRepository,
            IOptions<ApiUrls> apiUrls)
        {
            this.emailService = emailService;
            this.tokenService = tokenService;
            this.cacheRepository = cacheRepository;
            this.userRepository = userRepository;
            this.apiUrls = apiUrls.Value;
        }

        public async Task<IdentityResult> RegisterUser(User user, string password)
        {
            try
            {
                var result = await userRepository.CreateUser(user, password);

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
            var confirmationToken = await userRepository.GenerateEmailConfirmatioToken(user);

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

        public async Task<IdentityResult> ConfirmEmail(int userId, string confirmationToken)
        {
            if (confirmationToken == null)
            {
                return IdentityResult.Failed(new IdentityError() { Description = "Invalid token confirmation"});
            }

            var user = await userRepository.GetUserById(userId);
           
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError() { Description = "User not found" });
            }

            var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(confirmationToken));

            var result = await userRepository.ConfirmUserEmail(user, token);
            
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
                var result = await userRepository.UserSignIn(email, password);

                if(!result.SignIn.Succeeded)
                {
                    return new LoginModel()
                    {
                        Success = false,
                        Message = $"Login not allowed",
                    };
                }

                (var userToken, var refreshToken) = GenerateLoginTokens(result.User!);

                if (string.IsNullOrWhiteSpace(userToken) || string.IsNullOrWhiteSpace(refreshToken))
                {
                    return new LoginModel()
                    {
                        Success = false,
                        Message = $"User's tokens could not be created",
                    };
                }

                await PersistLoginToken(result.User!, userToken, refreshToken);

                return new LoginModel() 
                { 
                    Success = true,
                    Message = $"User {result.User!.UserName} logged",
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

                var user = await userRepository.GetUserByName(username);

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

        public Task<int?> GetUserIdByUsername(string username)
        {
            return userRepository.GetUserIdByUsername(username);
        }

        private (string UserToken, string RefreshToken) GenerateLoginTokens(User user)
        {
            var userToken = tokenService.GenerateUserToken(user);

            var refreshToken = tokenService.GenerateRefreshToken();

            return(userToken, refreshToken);
        }

        private async Task PersistLoginToken(User user,string userToken, string refreshToken)
        {
            var persistAuthToken = userRepository.SetAuthToken(user, userToken);

            var persistRefreshToken = cacheRepository.HashSetAsync(string.Empty, user.Id.ToString(), refreshToken, CacheType.RefreshToken, Parameters.Token.RefreshTokenExpiration);

            await Task.WhenAll(persistAuthToken, persistRefreshToken);
        }
    }
}
