using System.ComponentModel.DataAnnotations;

namespace Core.Domain.Entities;

public class UserAssignmentImages
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int AssignmentId { get; set; }
    [DataType("nvarchar(MAX)")] public string ImageBase64 { get; set; } = "";
}