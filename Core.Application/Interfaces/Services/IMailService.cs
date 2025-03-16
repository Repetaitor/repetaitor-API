namespace Core.Application.Interfaces.Services;

public interface IMailService
{
    Task<string> SendAuthMail(string to, string subject, string body);
}