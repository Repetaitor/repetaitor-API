namespace Core.Application.Models.DTO.Authorization;

public class SignUpUserRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string AuthCodeGuid {get; set;}
    
    public bool IsTeacher { get; set; } = false;
    public string Password { get; set; }
}