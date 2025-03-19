namespace Core.Application.Models.DTO.Authorization;

public class UserSignInRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}