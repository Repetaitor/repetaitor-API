using Core.Application.Interfaces.Repositories;
using Core.Application.Models;
using Core.Domain.Data;
using Core.Domain.Entities;
using Core.Domain.Mappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Domain.Repositories;

public class UserRepository(ApplicationContext context, IServiceProvider _serviceProvider) : IUserRepository
{
    private IGroupRepository GroupRepository => _serviceProvider.GetRequiredService<IGroupRepository>();
    public async Task<ResponseView<bool>> EmailExists(string email)
    {
        try
        {
            var res = await context.Users.AnyAsync(x => x.Email == email && x.isActive);
            return new ResponseView<bool>()
            {
                Code = StatusCodesEnum.Success,
                Data = res,
            };
        }
        catch (Exception ex)
        {
            return new ResponseView<bool>()
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = false
            };
        }
    }

    public async Task<ResponseView<UserModal>> CheckIfUser(string email, string password)
    {
        try
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Email == email && x.Password == password);
            if (user == null) return new ResponseView<UserModal>()
            {
                Code = StatusCodesEnum.NotFound,
                Message = "User not found or invalid credentials",
                Data = null
            };
            return new ResponseView<UserModal>()
            {
                Code = StatusCodesEnum.Success,
                Data = UserMapper.ToUserModal(user)
            };
        }
        catch (Exception ex)
        {
            return new ResponseView<UserModal>()
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = null
            };
        }
    }

    public async Task<ResponseView<int>> AddUser(string firstName, string lastName, string email, string password, string role)
    {
        try
        {
            var user = new User()
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Role = role,
                Password = password,
            };
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
            return new ResponseView<int>()
            {
                Code = StatusCodesEnum.Success,
                Data = user.Id,
                Message = "User added successfully"
            };
        }
        catch (Exception ex)
        {
            return new ResponseView<int>()
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = -1
            };
        }
    }

    public async Task<ResponseView<bool>> ActivateUser(int userId)
    {
        try
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null) return new ResponseView<bool>()
            {
                Code = StatusCodesEnum.NotFound,
                Message = "User not found",
                Data = false
            };
            user.isActive = true;
            await GroupRepository.AddUserToGroup(userId, "$$$$$");
            await context.SaveChangesAsync();
            return new ResponseView<bool>()
            {
                Code = StatusCodesEnum.Success,
                Data = true,
                Message = "User activated successfully"
            };
        }
        catch (Exception ex)
        {
            return new ResponseView<bool>()
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = false
            };
        }
    }

    public async Task<ResponseView<UserModal>> GetUserInfo(int userId)
    {
        try
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if(user == null) return new ResponseView<UserModal>()
            {
                Code = StatusCodesEnum.NotFound,
                Message = "User not found",
                Data = null
            };
            return new ResponseView<UserModal>()
            {
                Code = StatusCodesEnum.Success,
                Data = UserMapper.ToUserModal(user)
            };
        }
        catch (Exception ex)
        {
            return new ResponseView<UserModal>()
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = null
            };
        }
    }
}