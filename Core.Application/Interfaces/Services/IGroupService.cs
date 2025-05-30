using Core.Application.Models;
using Core.Application.Models.DTO.Essays;
using Core.Application.Models.DTO.Groups;

namespace Core.Application.Interfaces.Services;

public interface IGroupService
{
    Task<ResponseView<GroupBaseModal>> CreateGroup(string groupName, int ownerId);
    Task<ResponseView<ResultResponse>> DeleteGroup(int userId, int groupId);
    Task<ResponseView<GroupBaseModal>> GetStudentGroup(int userId);
    Task<ResponseView<List<GroupBaseModal>>> GetTeacherGroups(int userId);
    Task<ResponseView<GroupBaseModal>> UpdateGroupTitle(int userId, int groupId, string groupTitle);
    Task<ResponseView<NewGroupCodeResponse>> RegenerateGroupCode(int userId, int groupId);
    Task<ResponseView<ResultResponse>> AddUserToGroup(int userId, string groupCode);
    Task<ResponseView<ResultResponse>> RemoveUserFromGroup(int callerId, int groupId, int userId);
    Task<ResponseView<List<UserModal>>> GetGroupUsers(int userId, int groupId);
    Task<ResponseView<List<GroupBaseModal>>> SearchGroup(string groupName);
    Task<ResponseView<GroupBaseModal>> GetGroupBaseInfoById(int userId, int groupId);
}