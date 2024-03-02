using diceclub_api_netcore.Domain.Entities;
using diceclub_api_netcore.Domain.Interfaces.Repositories;
using diceclub_api_netcore.Infrastructure.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            await context.AddAsync(profile, cancellationToken);

            context.SaveChanges();
        }
    }
}
