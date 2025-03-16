using Core.Application.Interfaces.Repositories;
using Core.Application.Interfaces.Services;
using Core.Application.Models;

namespace Infrastructure.ProjectServices.Implementations;

public class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<UserModal> GetUserDefaultInfoAsync(int userId)
    {
        var user = await userRepository.GetUserInfo(userId);
        if (user == null)
        {
            throw new Exception($"User with id {userId} not found");
        }

        return user;
    }
}