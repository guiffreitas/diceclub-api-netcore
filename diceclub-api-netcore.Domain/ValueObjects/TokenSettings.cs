using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace diceclub_api_netcore.Domain.ValueObjects
{
    public class TokenSettings
    {
        public string TokenKey { get; set; } = string.Empty;
        public string TokenIssuer {  get; set; } = string.Empty;
    }
}
