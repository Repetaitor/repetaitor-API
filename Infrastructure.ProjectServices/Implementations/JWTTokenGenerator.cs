using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Core.Application.Interfaces.Services;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.ProjectServices.Implementations
{
    public static class AuthOptions
    {
        public const string ISSUER = "RepetaitorManager";
        public const string AUDIENCE = "RepetaitorClient";
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
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(),
                    SecurityAlgorithms.HmacSha256));
            return $"Bearer {new JwtSecurityTokenHandler().WriteToken(jwt)}";
        }

        public bool CheckUserIdWithTokenClaims(int userId, string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentException("Token cannot be null or empty.");
            }

            token = token.Split(" ").Last();
            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey()
                };

                var tokenHandler = new JwtSecurityTokenHandler();

                var principal =
                    tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);


                return userId == int.Parse(principal.FindFirst("userId")!.Value);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public int GetUserIdFromToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentException("Token cannot be null or empty.");
            }

            token = token.Split(" ").Last();
            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey()
                };

                var tokenHandler = new JwtSecurityTokenHandler();

                var principal =
                    tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);


                return int.Parse(principal.FindFirst("userId")!.Value);
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
    }
}