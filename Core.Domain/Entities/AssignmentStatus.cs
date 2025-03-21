using System.ComponentModel.DataAnnotations;

namespace Core.Domain.Entities;

public class AssignmentStatus
{
    public int Id { get; set; }
    [DataType("nvarchar(64)")]
    public string Name { get; set; }
    [DataType("nvarchar(64)")]
    public string Color { get; set; }
}