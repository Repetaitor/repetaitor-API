using Core.Application.Interfaces.Repositories;
using Core.Application.Interfaces.Services;
using Core.Application.Models;

namespace Infrastructure.ProjectServices.Implementations;

public class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<ResponseViewModel<UserModal>> GetUserDefaultInfoAsync(int userId)
    {
        var user = await userRepository.GetUserInfo(userId);
        if (user == null)
        {
            return new ResponseViewModel<UserModal>()
            {
                Code = -1,
                Message = $"User with id {userId} not found"
            };
        }

        return new ResponseViewModel<UserModal>()
        {
            Code = 0,
            Message = "",
            Data = user
        };
    }
}