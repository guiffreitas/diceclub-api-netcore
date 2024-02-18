using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace diceclub_api_netcore.Domain
{
    public static class Parameters
    {
        public static class Token
        {
            public static string AuthToken => "auth_token";
            public static int AuthTokenExpiration => 10; //minutes
            public static string RefreshToken => "refresh_token";
            public static int RefreshTokenExpiration => 60; //minutes
        }

        public static class Redis 
        {
            public static string Name => "dice-club-redis:backend";
        }
    }
}
