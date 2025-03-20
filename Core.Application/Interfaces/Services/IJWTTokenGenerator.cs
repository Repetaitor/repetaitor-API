using System.Security.Claims;
using Application.Models;
using Core.Application.Models;

namespace Core.Application.Interfaces.Services;

public interface IJWTTokenGenerator
{
    string GenerateJWTtoken(IEnumerable<Claim> claims);
    bool CheckUserIdWithTokenClaims(int userId, string token);
}