using Application.Models;
using Core.Application.Models;

namespace Core.Application.Interfaces.Services;

public interface IUserService
{
    Task<ResponseViewModel<UserModal>> GetUserDefaultInfoAsync(int userId);
}