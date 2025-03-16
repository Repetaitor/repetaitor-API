namespace Core.Application.Models.DTO.Authorization;

public class VerifyEmailRequest
{
    public string Email { get; set; }
    public string Code { get; set; }
}