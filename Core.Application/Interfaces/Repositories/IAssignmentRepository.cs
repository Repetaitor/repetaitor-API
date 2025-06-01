using Core.Application.Models;
using Core.Application.Models.DTO;

namespace Core.Application.Interfaces.Repositories;

public interface IAssignmentRepository
{
    Task<ResponseView<AssignmentBaseModal>> CreateNewAssignment(int userId, string instructions, int groupId,
        int essayId, DateTime dueDate);

    Task<ResponseView<(List<AssignmentBaseModal>?, int)>> GetGroupAssignments(int userId, int groupId, int? offset, int? limit);
    Task<ResponseView<List<StatusBaseModal>>> GetAssignmentStatuses();
    Task<ResponseView<bool>> SaveOrSubmitAssignment(int userId, int assignmentId, string text, int wordCount, bool isSubmitted);
    Task<ResponseView<List<StatusBaseModal>>> GetEvaluationStatuses();
    Task<ResponseView<(List<UserAssignmentBaseModal>?, int)>> GetUserAssignments(int userId, string statusName, bool IsAIAssignment, int? offset, int? limit);
    Task<ResponseView<(List<UserAssignmentBaseModal>?, int)>> GetUserNotSeenEvaluatedAssignments(int userId, int? offset, int? limit);
    Task<ResponseView<UserAssignmentModal>> GetUserAssignment(int callerId, int userId, int assignmentId);
    Task<ResponseView<AssignmentBaseModal>> GetAssignmentById(int assignmentId);

    Task<ResponseView<AssignmentBaseModal>> UpdateAssignment(int userId, int assignmentId, string instructions, int essayId,
        DateTime dueDate);

    Task<ResponseView<bool>> EvaluateAssignment(int teacherId, int userId, int assignmentId, int fluencyScore, int grammarScore,
        List<EvaluationTextCommentModal> evaluationTextComments, List<GeneralCommentModal> generalComments);

    Task<ResponseView<(List<UserAssignmentBaseModal>?, int)>> GetTeacherAssignments(int userId, int? offset, int? limit);

    Task<ResponseView<(List<UserAssignmentBaseModal>?, int)>> GetAssigmentUsersTasks(int userId, int assignmentId, string statusName, int? offset,
        int? limit);
    Task<ResponseView<List<UserAssignmentViewForAI>>> GetUserAssignmentViewForAI(int aiTeacherId, int count);
}