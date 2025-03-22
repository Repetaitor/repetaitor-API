using System.Text.RegularExpressions;
using Core.Application.Models;

namespace Core.Application.Interfaces.Repositories;

public interface IGroupRepository
{
    Task<GroupBaseModal?> CreateGroup(string groupName, string groupCode, int ownerId);
    Task<bool> ChangeGroupState(int userId, int groupId, bool isActive);
    Task<GroupBaseModal?> GetStudentGroup(int userId);
    Task<List<GroupBaseModal>?> GetTeacherGroups(int userId, bool isActive);
    Task<GroupBaseModal?> UpdateGroupTitle(int userId, int groupId, string groupTitle);
    Task<bool> UpdateGroupCode(int userId, int groupId, string groupCode);
    Task<bool> AddUserToGroup(int userId, string groupCode);
    Task<bool> RemoveUserFromGroup(int groupId, int userId);
    Task<List<UserModal>?> GetGroupUsers(int userId, int groupId);
    Task<GroupBaseModal?> GetGroupBaseInfoById(int userId, int groupId);
    Task<List<GroupBaseModal>> SearchGroup(string groupName, bool isActive);
}