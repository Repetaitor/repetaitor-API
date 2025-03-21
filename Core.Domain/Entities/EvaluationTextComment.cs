using System.ComponentModel.DataAnnotations;

namespace Core.Domain.Entities;

public class EvaluationTextComment
{
    public int Id { get; set; }
    public int UserAssignmentId { get; set; }
    public int StartIndex { get; set; }
    public int EndIndex { get; set; }
    public string Comment { get; set; }
    public int StatusId { get; set; }
}