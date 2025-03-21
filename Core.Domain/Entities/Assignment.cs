using System.ComponentModel.DataAnnotations;

namespace Core.Domain.Entities;

public class Assignment
{
    public int Id { get; set; }
    [DataType("nvarchar(100)")]
    public string AssignmentTitle { get; set; }
    [DataType("Text")]
    public string Instructions { get; set; } = "";
    public int GroupId { get; set; }
    public int EssayId { get; set; }
    public int CreatorId { get; set; }
    [DataType("DateTime")]
    public DateTime CreationTime { get; set; } = DateTime.Now;
    [DataType("DateTime")]
    public DateTime DueDate { get; set; }
}