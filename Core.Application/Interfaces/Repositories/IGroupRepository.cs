using System.Text.RegularExpressions;
using Core.Application.Models;

namespace Core.Application.Interfaces.Repositories;

public interface IGroupRepository
{
    Task<ResponseView<GroupBaseModal>> CreateGroup(string groupName, string groupCode, int ownerId);
    Task<ResponseView<bool>> DeleteGroup(int userId, int groupId);
    Task<ResponseView<GroupBaseModal>> GetStudentGroup(int userId);
    Task<ResponseView<List<GroupBaseModal>>> GetTeacherGroups(int userId);
    Task<ResponseView<GroupBaseModal>> UpdateGroupTitle(int userId, int groupId, string groupTitle);
    Task<ResponseView<bool>> UpdateGroupCode(int userId, int groupId, string groupCode);
    Task<ResponseView<bool>> AddUserToGroup(int userId, string groupCode);
    Task<ResponseView<bool>> RemoveUserFromGroup(int callerId, int groupId, int userId);
    Task<ResponseView<List<UserModal>>> GetGroupUsers(int userId, int groupId);
    Task<ResponseView<GroupBaseModal>> GetGroupBaseInfoById(int userId, int groupId);
    Task<ResponseView<List<GroupBaseModal>>> SearchGroup(string groupName);
}