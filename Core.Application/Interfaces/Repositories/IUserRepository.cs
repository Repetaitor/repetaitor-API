using Application.Models;
using Core.Application.Models;

namespace Core.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<bool> EmailExists(string email);
    Task<UserModal> CheckIfUser(string email, string password);
    Task<int> AddUser(string firstName, string lastName, string email, string password, string role);
    Task<bool> ActivateUser(int userId);
    Task<UserModal> GetUserInfo(int userId);
}