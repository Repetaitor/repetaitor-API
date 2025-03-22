using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Core.Domain.Entities;

public class Assignment
{
    public int Id { get; set; }
    [DataType("Text")]
    public string Instructions { get; set; } = "";
    public int GroupId { get; set; }
    public int EssayId { get; set; }
    public int CreatorId { get; set; }
    [DataType("DateTime")]
    public DateTime CreationTime { get; set; } = DateTime.Now;
    [DataType("DateTime")]
    public DateTime DueDate { get; set; }
    public virtual RepetaitorGroup Group { get; set; }
    public virtual Essay Essay { get; set; }
    public virtual User Creator { get; set; }
    public ICollection<UserAssignment> UserAssignments { get; set; }
}