using Core.Application.Models;

namespace Core.Application.Interfaces.Repositories;

public interface IAuthCodesRepository
{
    Task<ResponseView<string>> CreateAuthCode(string code, string email, int userId);
    Task<ResponseView<bool>> CheckAuthCode(string guid, string code, string email);
    Task<ResponseView<bool>> EmailIsVerified(string guid, string email);
}