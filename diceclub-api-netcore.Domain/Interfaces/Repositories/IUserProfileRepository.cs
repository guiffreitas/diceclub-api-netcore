using diceclub_api_netcore.Domain.Entities;

namespace diceclub_api_netcore.Domain.Interfaces.Repositories
{
    public interface IUserProfileRepository
    {
        Task CreateUserProfile(UserProfile profile, CancellationToken cancellationToken);
        Task UpdateUserProfile(UserProfile profile, CancellationToken cancellationToken);
        Task<UserProfile?> GetUserProfileByUserId(int userId, CancellationToken cancellationToken);
    }
}
