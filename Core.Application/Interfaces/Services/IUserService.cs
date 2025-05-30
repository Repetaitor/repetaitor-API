using Application.Models;
using Core.Application.Models;

namespace Core.Application.Interfaces.Services;

public interface IUserService
{
    Task<ResponseView<UserModal>> GetUserDefaultInfoAsync(int userId);
}