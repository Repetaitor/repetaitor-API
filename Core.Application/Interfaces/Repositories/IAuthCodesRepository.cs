using Core.Application.Models;

namespace Core.Application.Interfaces.Repositories;

public interface IAuthCodesRepository
{
    Task<string> CreateAuthCode(string code, string email, int userId);
    Task<bool> CheckAuthCode(string guid, string code, string email);
    Task<bool> EmailIsVerified(string guid, string email);
}