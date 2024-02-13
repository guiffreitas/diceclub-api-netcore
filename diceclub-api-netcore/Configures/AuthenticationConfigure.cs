using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace diceclub_api_netcore.Configures
{
    public static class AuthenticationConfigure
    {
        public static IServiceCollection AddAuthenticationConfigure(this IServiceCollection service, IConfiguration configuration)
        {
            var tokenKeyString = configuration.GetSection("TokenSettings:TokenKey").Value!;

            var tokenKey = Encoding.ASCII.GetBytes(tokenKeyString);

            service.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(tokenKey),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            return service;
        }
    }
}
