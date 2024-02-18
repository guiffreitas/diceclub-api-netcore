using diceclub_api_netcore.Domain.Entities;
using diceclub_api_netcore.Domain.Enums;
using diceclub_api_netcore.Domain.Interfaces.Services;
using diceclub_api_netcore.Domain.ValueObjects;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace diceclub_api_netcore.Domain.Services
{
    public class TokenService : ITokenService
    {
        private readonly TokenSettings tokenSettings;

        public TokenService(IOptions<TokenSettings> tokenSettings) 
        {
            this.tokenSettings = tokenSettings.Value;
        }

        public string GenerateUserToken(User user)
        {
            var claims = GenerateUserClaims(user);

            return GenerateToken(claims, Parameters.Token.AuthTokenExpiration);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public string? GetUsernameFromToken(string token)
        {
            var claims = RetrieveClaimsFromToken(token);

            if(claims?.Claims == null || !claims.Claims.Any())
            {
                return null;
            }

            var username = claims.Claims.FirstOrDefault(c => c.Subject!.NameClaimType.Equals(ClaimTypes.Name));

            if(username == default) 
            {
                return null;
            }

            return username.Value;
        }

        private ClaimsIdentity GenerateUserClaims(User user)
        {
            var claims = new ClaimsIdentity(new Claim[]
            {
                new(ClaimTypes.Name, user.UserName!.ToString())
            });

            return claims;
        }

        private string GenerateToken(ClaimsIdentity claims, int expirationMinutes)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(tokenSettings.TokenKey);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256),
                Issuer = tokenSettings.TokenIssuer
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            
            return tokenHandler.WriteToken(token);
        }

        private ClaimsIdentity RetrieveClaimsFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenClaims = tokenHandler.ReadJwtToken(token).Claims;

            return new ClaimsIdentity(tokenClaims);
        }
    }
}
