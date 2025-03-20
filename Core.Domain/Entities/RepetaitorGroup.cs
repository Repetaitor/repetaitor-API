using System.ComponentModel.DataAnnotations;

namespace Core.Domain.Entities;

public class RepetaitorGroup
{
    public int Id { get; set; }
    public int OwnerId { get; set; }
    [DataType("nvarchar(100)")]
    public string GroupName { get; set; }
    [DataType("nvarchar(64)")]
    public string GroupCode { get; set; }
    [DataType("boolean")]
    public bool IsActive { get; set; }
    [DataType("DateTime")]
    public DateTime CreateDate { get; set; }
}