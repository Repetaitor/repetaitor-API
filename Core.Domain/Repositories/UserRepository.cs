using Core.Application.Interfaces.Repositories;
using Core.Application.Models;
using Core.Domain.Data;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Core.Domain.Repositories;

public class UserRepository(ApplicationContext context) : IUserRepository
{
    public async Task<bool> EmailExists(string email)
    {
        try
        {
            return await context.Users.AnyAsync(x => x.Email == email);
        }
        catch (Exception)
        {
            return false;
        }
    }
    public async Task<UserModal?> CheckIfUser(string email, string password)
    {
        try
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Email == email && x.Password == password);
            if (user == null) return null;
            return new UserModal()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
            };
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<bool> AddUser(string firstName, string lastName, string email, string password)
    {
        try
        {
            var user = new User()
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Password = password,
            };
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<UserModal?> GetUserInfo(int userId)
    {
        try
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null) return null;
            return new UserModal()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
            };
        }
        catch
        {
            return null;
        }
    }
}