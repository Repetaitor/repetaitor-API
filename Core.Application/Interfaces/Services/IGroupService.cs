using Core.Application.Models;
using Core.Application.Models.DTO.Essays;
using Core.Application.Models.DTO.Groups;

namespace Core.Application.Interfaces.Services;

public interface IGroupService
{
    Task<ResponseViewModel<GroupBaseModal>> CreateGroup(string groupName, int ownerId);
    Task<ResponseViewModel<ResultResponse>> ChangeGroupState(int userId, int groupId, bool isActive);
    Task<ResponseViewModel<GroupBaseModal?>> GetStudentGroup(int userId);
    Task<ResponseViewModel<List<GroupBaseModal>>> GetTeacherGroups(int userId, bool isActive);
    Task<ResponseViewModel<GroupBaseModal>> UpdateGroupTitle(int userId, int groupId, string groupTitle);
    Task<ResponseViewModel<NewGroupCodeResponse>> RegenerateGroupCode(int userId, int groupId);
    Task<ResponseViewModel<ResultResponse>> AddUserToGroup(int userId, string groupCode);
    Task<ResponseViewModel<ResultResponse>> RemoveUserFromGroup(int groupId, int userId);
    Task<ResponseViewModel<List<UserModal>>> GetGroupUsers(int userId, int groupId);
    Task<ResponseViewModel<List<GroupBaseModal>>> SearchGroup(string groupName, bool isActive);
    Task<ResponseViewModel<GroupBaseModal>> GetGroupBaseInfoById(int userId, int groupId);
}