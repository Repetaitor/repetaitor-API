using Core.Application.Models;
using Core.Application.Models.DTO;

namespace Core.Application.Interfaces.Repositories;

public interface IGroupRepository
{
    Task<GroupBaseModal> CreateGroup(string groupName, string groupCode, int ownerId);
    Task<bool> DeleteGroup(int userId, int groupId);
    Task<GroupBaseModal> GetStudentGroup(int userId);
    Task<List<GroupBaseModal>> GetTeacherGroups(int userId);
    Task<GroupBaseModal> UpdateGroupTitle(int userId, int groupId, string groupTitle);
    Task<bool> UpdateGroupCode(int userId, int groupId, string groupCode);
    Task<bool> AddUserToGroup(int userId, string groupCode);
    Task<bool> RemoveUserFromGroup(int callerId, int groupId, int userId);
    Task<List<UserModal>> GetGroupUsers(int userId, int groupId);
    Task<GroupBaseModal> GetGroupBaseInfoById(int userId, int groupId);
    Task<List<GroupBaseModal>> SearchGroup(string groupName);
    Task<int> TeacherGroupsCount(int teacherId);
    Task<int> TeacherGroupsEnrolledStudentsCount(int teacherId);
    Task<bool> GroupExists(int groupId);
    Task<bool> IsUserInGroup(int userId, int groupId);
    Task<bool> IsUserGroupOwner(int userId, int groupId);
}