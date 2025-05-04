using System.Security.Claims;

namespace Core.Application.Models.DTO.Authorization;

public class UserSignInResponse
{
    public UserModal User {get; set;}
    public ClaimsIdentity ClaimsIdentity { get; set; }    
}