namespace Core.Application.Models.DTO.Authorization;

public class UserSignInRequest
{
    public string Identifier { get; set; }
    public string Password { get; set; }
}