namespace Core.Application.Models.RequestsDTO.Authorization;

public class UserSignInRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}