using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Core.Application.Interfaces.Services;
using Core.Application.Models;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.ProjectServices.Implementations
{
    public static class AuthOptions
    {
        public const string ISSUER = "EventPlannerManager";
        public const string AUDIENCE = "EventPlannerClient";
        const string KEY = "mysupersecret_secretsecretsecretkey!123";
        public static SymmetricSecurityKey GetSymmetricSecurityKey() => new(Encoding.UTF8.GetBytes(KEY));
    }
    public class JwtTokenGenerator : IJWTTokenGenerator
    {
        public string GenerateJWTtoken(IEnumerable<Claim> claims)
        {
            var jwt = new JwtSecurityToken(
           issuer: AuthOptions.ISSUER,
           audience: AuthOptions.AUDIENCE,
           claims: claims,
           expires: DateTime.UtcNow.Add(TimeSpan.FromHours(2)),
           signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
           return $"Bearer {new JwtSecurityTokenHandler().WriteToken(jwt)}";
        }
        public UserModal GetClaimsFromToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentException("Token cannot be null or empty.");
            }

            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true, 
                    ValidateAudience = true, 
                    ValidateLifetime = true,
                    IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey()
                };

                var tokenHandler = new JwtSecurityTokenHandler();

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);


                return new UserModal
                {
                    Id = int.Parse(principal.FindFirst("userId")!.Value),
                    Email = principal.FindFirst("email")!.Value,
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to validate token.", ex);
            }
        }
    }
}
