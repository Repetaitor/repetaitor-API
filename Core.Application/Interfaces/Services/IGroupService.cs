using Core.Application.Models;
using Core.Application.Models.DTO.Essays;
using Core.Application.Models.DTO.Groups;

namespace Core.Application.Interfaces.Services;

public interface IGroupService
{
    Task<GroupBaseModal?> CreateGroup(string groupName, int ownerId);
    Task<ResultResponse> DeleteGroup(int userId, int groupId);
    Task<GroupBaseModal?> GetStudentGroup(int userId);
    Task<List<GroupBaseModal>?> GetTeacherGroups(int userId);
    Task<GroupBaseModal?> UpdateGroupTitle(int userId, int groupId, string groupTitle);
    Task<NewGroupCodeResponse?> RegenerateGroupCode(int userId, int groupId);
    Task<ResultResponse> AddUserToGroup(int userId, string groupCode);
    Task<ResultResponse> RemoveUserFromGroup(int callerId, int groupId, int userId);
    Task<List<UserModal>?> GetGroupUsers(int userId, int groupId);
    Task<List<GroupBaseModal>?> SearchGroup(string groupName);
    Task<GroupBaseModal?> GetGroupBaseInfoById(int userId, int groupId);
}