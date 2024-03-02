using diceclub_api_netcore.Domain.Entities;
using diceclub_api_netcore.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace diceclub_api_netcore.Domain.Interfaces.Services
{
    public interface IUserProfileService
    {
        Task<ResultModel<string>> CreateUserProfile(UserProfile profile, CancellationToken cancellationToken);
    }
}
