using diceclub_api_netcore.Domain.Entities;
using diceclub_api_netcore.Domain.Interfaces.Repositories;
using diceclub_api_netcore.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Threading;

namespace diceclub_api_netcore.Infrastructure.Repositories
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly ApplicationDbContext context;

        public UserProfileRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task CreateUserProfile(UserProfile profile, CancellationToken cancellationToken)
        {
            await context.Profiles.AddAsync(profile, cancellationToken);

            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateUserProfile(UserProfile profile, CancellationToken cancellationToken)
        {
            var currentProfile = await context.Profiles.FirstOrDefaultAsync(p => p.UserId == profile.UserId, cancellationToken);

            if(currentProfile == null)
            {
                await CreateUserProfile(profile, cancellationToken);

                return;
            }

            profile.Id = currentProfile.Id;

            context.Entry(currentProfile).CurrentValues.SetValues(profile);

            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task<UserProfile?> GetUserProfileByUserId(int userId, CancellationToken cancellationToken)
        {
            return await context.Profiles.FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
        }
    }
}
