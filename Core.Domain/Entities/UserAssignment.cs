using System.ComponentModel.DataAnnotations;

namespace Core.Domain.Entities;

public class UserAssignment
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int AssignmentId { get; set; }
    public int StatusId { get; set; }
    [DataType("nvarchar(MAX)")] 
    public string Text { get; set; } = "";
    public int WordCount { get; set; }
    public int GrammarScore { get; set; } = 0;
    public int FluencyScore { get; set; } = 0;
    [DataType("boolean")] 
    public bool FeedbackSeen { get; set; } =  false; 
    [DataType("boolean")] 
    public bool IsEvaluated { get; set; } =  false;
    [DataType("DateTime")]
    public DateTime AssignDate { get; set; } = DateTime.Now;
    [DataType("DateTime")]
    public DateTime SubmitDate { get; set; }
    
    [DataType("Boolean")]
    public bool IsPublic { get; set; } = false;
    public virtual User User { get; set; }
    public virtual Assignment Assignment { get; set; }
    public virtual AssignmentStatus Status { get; set; }
    public virtual ICollection<EvaluationTextComment> EvaluationTextComments { get; set; }
    public virtual ICollection<GeneralComment> GeneralComments { get; set; }
}