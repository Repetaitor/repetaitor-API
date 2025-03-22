using Core.Application.Models;
using Core.Domain.Entities;

namespace Core.Domain.Mappers;

public static class UserMapper
{
    public static UserModal ToUserModal(User user)
    {
        return new UserModal()
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = user.Role
        };
    }
}