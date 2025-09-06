namespace Core.Application.Models.RequestsDTO.Authorization;

public class VerifyEmailRequest
{
    public string Guid { get; set; }
    public string Email { get; set; }
    public string Code { get; set; }
}