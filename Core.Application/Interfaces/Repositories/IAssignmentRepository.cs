using Core.Application.Models;
using Core.Application.Models.DTO;
using Core.Application.Models.DTO.Essays;

namespace Core.Application.Interfaces.Repositories;

public interface IAssignmentRepository
{
    Task<UserPerformanceViewModel> GetUserPerformance(int userId, DateTime? fromDate = null, DateTime? toDate = null);
    Task<UserAssignmentsStatusesStats> GetUserAssignmentsStatusStat(int userId);
    Task<ResultResponse> DeleteAssignment(int userId, int assignmentId);

    Task<UserScoresStatsModel> GetAverageUserScoreByDate(int userId, DateTime? startDate = null,
        DateTime? endDate = null);

    Task<AssignmentBaseModal> CreateNewAssignment(int userId, string instructions, int groupId,
        int essayId, DateTime dueDate);

    Task<(List<AssignmentBaseModal>?, int)> GetGroupAssignments(int userId, int groupId, int? offset, int? limit);
    Task<List<StatusBaseModal>> GetAssignmentStatuses();
    Task<bool> SaveOrSubmitAssignment(int userId, int assignmentId, string text, int wordCount, bool isSubmitted);
    Task<List<StatusBaseModal>> GetEvaluationStatuses();

    Task<(List<UserAssignmentBaseModal>?, int)> GetUserAssignments(int userId, string statusName, bool IsAIAssignment, int? offset, int? limit);
    Task<(List<UserAssignmentBaseModal>?, int)> GetUserNotSeenEvaluatedAssignments(int userId, int? offset, int? limit);
    Task<UserAssignmentModal> GetUserAssignment(int callerId, int userId, int assignmentId);
    Task<AssignmentBaseModal> GetAssignmentById(int assignmentId);

    Task<AssignmentBaseModal> UpdateAssignment(int userId, int assignmentId, string instructions, int essayId,
        DateTime dueDate);

    Task<bool> EvaluateAssignment(int teacherId, int userId, int assignmentId, int fluencyScore, int grammarScore,
        List<EvaluationTextCommentModal> evaluationTextComments, List<GeneralCommentModal> generalComments);

    Task<(List<UserAssignmentBaseModal>?, int)> GetTeacherAssignments(int userId, int? offset, int? limit);

    Task<(List<UserAssignmentBaseModal>?, int)> GetAssigmentUsersTasks(int userId, int assignmentId, string statusName, int? offset,
        int? limit);

    Task<List<UserAssignmentViewForAI>> GetUserAssignmentViewForAI(int aiTeacherId, int count);
    Task<int> GetTeacherCreatedAssignmentsCount(int teacherId);
    Task<int> GetTeacherNeedToEvaluateAssignmentsCount(int teacherId);
    Task<GroupsPerformance> GetTeacherGroupsPerformanceByDate(int teacherId, DateTime? fromDate = null, DateTime? toDate = null);

    Task<UserPerformanceViewModel> GetAllUserPerformanceForTeacher(int teacherId, DateTime? fromDate = null,
        DateTime? toDate = null);
    Task<ResultResponse> MakeUserAssignmentPublic(int userId, int assignmentId);
    Task<List<UserAssignmentBaseModal>> GetPublicUserAssignments(int userId, int assignmentId, int? offset, int? limit);
    Task<ResultResponse> IsAssignmentPublic(int userId, int assignmentId);
    Task<bool> SaveImagesForAssignment(int userId, int assignmentId, string imageUrl);
    Task<List<string>> GetUserAssignmentImagesUrl(int userId, int assignmentId);
    Task<bool> ClearUserAssignemntImagesUrl(int userId, int assignmentId);
}
