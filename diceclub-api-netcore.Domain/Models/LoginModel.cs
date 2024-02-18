using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace diceclub_api_netcore.Domain.Models
{
    public class LoginModel
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string UserToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
