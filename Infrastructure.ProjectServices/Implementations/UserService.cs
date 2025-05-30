using Core.Application.Interfaces.Repositories;
using Core.Application.Interfaces.Services;
using Core.Application.Models;

namespace Infrastructure.ProjectServices.Implementations;

public class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<ResponseView<UserModal>> GetUserDefaultInfoAsync(int userId)
    {
        return await Task.Run(() => userRepository.GetUserInfo(userId));
    }
}