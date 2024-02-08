using diceclub_api_netcore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace diceclub_api_netcore.Domain.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendEmailConfirmationAsync(User user, string confirmationLink);
    }
}
