using diceclub_api_netcore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace diceclub_api_netcore.Domain.Interfaces.Repositories
{
    public interface IUserProfileRepository
    {
        Task CreateUserProfile(UserProfile profile, CancellationToken cancellationToken);
        Task UpdateUserProfile(UserProfile profile, CancellationToken cancellationToken);
    }
}
