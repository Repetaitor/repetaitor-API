using Core.Application.Models.ReturnViewModels;

namespace Core.Application.Models.RequestsDTO.Assignments;

public class EvaluateAssignmentRequest
{
    public int UserId { get; set; }
    public int AssignmentId { get; set; }
    public int FluencyScore { get; set; }
    public int GrammarScore { get; set; }
    public List<EvaluationTextCommentModal> EvaluationTextComments { get; set; } = [];
    public List<GeneralCommentModal> GeneralComments { get; set; } = [];
}