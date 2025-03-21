using Core.Application.Models.DTO;

namespace Core.Application.Models;

public class UserAssignmentModal
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int AssignmentId { get; set; }
    public StatusBaseModal? Status { get; set; }
    public string Text { get; set; }
    public int WordCount { get; set; }
    public int GrammarScore { get; set; }
    public int FluencyScore { get; set; }
    public bool FeedbackSeen { get; set; } 
    public bool IsEvaluated { get; set; }
    public List<EvaluationTextCommentModal> EvaluationComments { get; set; }
    public List<GeneralCommentModal> GeneralCommentS { get; set; }
    public DateTime AssignDate { get; set; } 
}