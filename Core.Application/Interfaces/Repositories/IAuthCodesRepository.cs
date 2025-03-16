namespace Core.Application.Interfaces.Repositories;

public interface IAuthCodesRepository
{
    Task<bool> CreateAuthCode(string code, string email);
    Task<bool> CheckAuthCode(string code, string email);
    Task<bool> EmailIsVerfied(string email);
}