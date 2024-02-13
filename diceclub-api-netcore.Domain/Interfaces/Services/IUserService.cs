using diceclub_api_netcore.Domain.Entities;
using diceclub_api_netcore.Domain.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace diceclub_api_netcore.Domain.Interfaces.Services
{
    public interface IUserService
    {
        Task<IdentityResult> RegisterUser(User user, string password);
        Task<IdentityResult> ConfirmEmail(string userId, string confirmationToken);
        Task<(IdentityResult Result, string UserToken)> LoginUser(string email, string password);
        Task<ResultModel<bool>> UserIsNew(User user);

    }
}
