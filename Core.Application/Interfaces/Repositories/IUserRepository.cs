using Application.Models;
using Core.Application.Models;

namespace Core.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<ResponseView<bool>> EmailExists(string email);
    Task<ResponseView<UserModal>> CheckIfUser(string email, string password);
    Task<ResponseView<int>> AddUser(string firstName, string lastName, string email, string password, string role);
    Task<ResponseView<bool>> ActivateUser(int userId);
    Task<ResponseView<UserModal>> GetUserInfo(int userId);
}