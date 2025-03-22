using System.ComponentModel.DataAnnotations;

namespace Core.Domain.Entities;

public class GeneralComment
{
    public int Id { get; set; }
    public int UserAssignmentId { get; set; }
    public int StatusId { get; set; }
    [DataType("Text")]
    public string Comment { get; set; }
    public virtual UserAssignment UserAssignment { get; set; }
}