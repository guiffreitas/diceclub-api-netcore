using diceclub_api_netcore.Domain;
using diceclub_api_netcore.Domain.Entities;
using diceclub_api_netcore.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace diceclub_api_netcore.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        public UserRepository(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public async Task<IdentityResult> CreateUser(User user, string password)
        {
            return await userManager.CreateAsync(user, password);
        }

        public async Task<string> GenerateEmailConfirmatioToken(User user)
        {
            return await userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<User?> GetUserById(int userId)
        {
            return await userManager.FindByIdAsync(userId.ToString());
        }

        public async Task<User?> GetUserByEmail(string userEmail)
        {
            return await userManager.FindByEmailAsync(userEmail);
        }
        public async Task<User?> GetUserByName(string username)
        {
            return await userManager.FindByNameAsync(username);
        }

        public async Task<int?> GetUserIdByUsername(string username)
        {
            var user = await GetUserByName(username);

            if(user == null)
            {
                return null;
            }

            return user.Id;
        }

        public async Task<(SignInResult SignIn, User? User)> UserSignIn(string userEmail, string password)
        {
            var user = await GetUserByEmail(userEmail);

            if (user == default)
            {
                return (SignInResult.Failed, null);
            }

            return (await signInManager.PasswordSignInAsync(user, password, false, false), user);
        }

        public async Task<IdentityResult> SetAuthToken(User user, string userToken)
        {
            return await userManager.SetAuthenticationTokenAsync(user, string.Empty, Parameters.Token.AuthToken, userToken);
        }

        public async Task<IdentityResult> ConfirmUserEmail(User user, string token)
        {
            return await userManager.ConfirmEmailAsync(user, token);
        }
    }
}
