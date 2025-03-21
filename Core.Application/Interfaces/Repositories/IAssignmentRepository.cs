using Core.Application.Models;
using Core.Application.Models.DTO;

namespace Core.Application.Interfaces.Repositories;

public interface IAssignmentRepository
{
    Task<AssignmentBaseModal?> CreateNewAssignment(int userId, string assignmentTitle, string instructions, int groupId,
        int essayId, DateTime dueDate);

    Task<List<AssignmentBaseModal>?> GetGroupAssignments(int userId, int groupId);
    Task<List<StatusBaseModal>?> GetAssignmentStatuses();
    Task<bool> SaveOrSubmitAssignment(int userId, int assignmentId, string text, int wordCount, bool isSubmitted);
    Task<List<StatusBaseModal>?> GetEvaluationStatuses();
    Task<List<AssignmentBaseModal>?> GetUserAssignments(int userId, int statusId);
    Task<List<AssignmentBaseModal>?> GetUserNotSeenEvaluatedAssignments(int userId);
    Task<UserAssignmentModal?> GetUserAssignment(int callerId, int userId, int assignmentId);
    Task<AssignmentBaseModal?> GetAssignmentById(int assignmentId);

    Task<AssignmentBaseModal?> UpdateAssignment(int userId, int assignmentId, string assignmentTitle,
        string instructions, int essayId);

    Task<bool> EvaluateAssignment(int teacherId, int userId, int assignmentId, int fluencyScore, int grammarScore,
        List<EvaluationTextCommentModal> evaluationTextComments, List<GeneralCommentModal> generalComments);

    Task<List<AssignmentBaseModal>?> GetTeacherAssignments(int userId);
}