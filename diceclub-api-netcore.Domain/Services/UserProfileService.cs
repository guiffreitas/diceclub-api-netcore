using diceclub_api_netcore.Domain.Entities;
using diceclub_api_netcore.Domain.Interfaces.Repositories;
using diceclub_api_netcore.Domain.Interfaces.Services;
using diceclub_api_netcore.Domain.Models;

namespace diceclub_api_netcore.Domain.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUserProfileRepository profileRepository;

        public UserProfileService(IUserProfileRepository profileRepository)
        {
            this.profileRepository = profileRepository;
        }

        public async Task<ResultModel<string>> CreateUserProfile(UserProfile profile, CancellationToken cancellationToken)
        {
            try
            {
                await profileRepository.CreateUserProfile(profile, cancellationToken);

                return new ResultModel<string> { Success = true, Result = "Profile Created" };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
