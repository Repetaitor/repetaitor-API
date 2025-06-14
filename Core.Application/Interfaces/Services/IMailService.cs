namespace Core.Application.Interfaces.Services;

public interface IMailService
{
    Task<(bool, string)> SendAuthMail(string to, string subject, string body);
}