using Core.Application.Models;
using Core.Application.Models.DTO.Essays;
using Core.Application.Models.DTO.Groups;

namespace Core.Application.Interfaces.Services;

public interface IGroupService
{
    Task<GroupBaseModal?> CreateGroup(string groupName, int ownerId);
    Task<ResultResponse> ChangeGroupState(int userId, int groupId, bool isActive);
    Task<GroupBaseModal?> GetStudentGroup(int userId);
    Task<List<GroupBaseModal>?> GetTeacherGroups(int userId, bool isActive);
    Task<GroupBaseModal?> UpdateGroupTitle(int userId, int groupId, string groupTitle);
    Task<NewGroupCodeResponse?> RegenerateGroupCode(int userId, int groupId);
    Task<ResultResponse> AddUserToGroup(int userId, string groupCode);
    Task<ResultResponse> RemoveUserFromGroup(int groupId, int userId);
    Task<List<UserModal>?> GetGroupUsers(int userId, int groupId);
    Task<List<GroupBaseModal>?> SearchGroup(string groupName, bool isActive);
    Task<GroupBaseModal?> GetGroupBaseInfoById(int userId, int groupId);
}