namespace Core.Application.Models.DTO.Authorization;

public class UserSignInResponse
{
    public UserModal User {get; set;}
    public string JWTToken { get; set; }    
}