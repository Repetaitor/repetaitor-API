using System.Security.Claims;

namespace Core.Application.Interfaces.Services;

public interface IJWTTokenGenerator
{
    string GenerateJWTtoken(IEnumerable<Claim> claims);
    bool CheckUserIdWithTokenClaims(int userId, string token);
    int GetUserIdFromToken(string token);
}