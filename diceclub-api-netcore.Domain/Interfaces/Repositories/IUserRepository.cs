using diceclub_api_netcore.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace diceclub_api_netcore.Domain.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<IdentityResult> CreateUser(User user, string password);
        Task<string> GenerateEmailConfirmatioToken(User user);
        Task<User?> GetUserById(int userId);
        Task<User?> GetUserByEmail(string userEmail);
        Task<User?> GetUserByName(string userName);
        Task<(SignInResult SignIn, User? User)> UserSignIn(string userEmail, string password);
        Task<IdentityResult> SetAuthToken(User user, string userToken);
        Task<IdentityResult> ConfirmUserEmail(User user, string token);
        Task<int?> GetUserIdByUsername(string username);

    }
}
