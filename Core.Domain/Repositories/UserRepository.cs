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

    public async Task<bool> EmailExists(string email)
    {
        return await context.Users.AnyAsync(x => x.Email == email && x.isActive);
    }

    public async Task<UserModal> CheckIfUser(string email, string password)
    {
        var user = await context.Users.FirstOrDefaultAsync(x => x.Email == email && x.Password == password);
        if (user == null)
            throw new Exception("User not found or password is incorrect");
        return UserMapper.ToUserModal(user);
    }

    public async Task<int> AddUser(string firstName, string lastName, string email, string password,
        string role)
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
        return user.Id;
    }

    public async Task<bool> ActivateUser(int userId)
    {
        var user = await context.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if (user == null)
            throw new Exception("User not found");
        user.isActive = true;
        await GroupRepository.AddUserToGroup(userId, "$$$$$");
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<UserModal> GetUserInfo(int userId)
    {
        var user = await context.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if (user == null)
            throw new Exception("User not found");
        return UserMapper.ToUserModal(user);
    }
}