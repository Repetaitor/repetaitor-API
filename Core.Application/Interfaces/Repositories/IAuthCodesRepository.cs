namespace Core.Application.Interfaces.Repositories;

public interface IAuthCodesRepository
{
    Task<string> CreateAuthCode(string code, string email);
    Task<bool> CheckAuthCode(string guid, string code, string email);
    Task<bool> EmailIsVerfied(string guid, string email);
}