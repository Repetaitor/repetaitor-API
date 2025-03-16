namespace Core.Application.Interfaces.Services;

public interface IMailService
{
    Task<bool> SendAuthMail(string to, string subject, string body);
}