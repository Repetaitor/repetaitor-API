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

    public bool IsAIGroup { get; set; } = false;
    
    public int ChatId { get; set; }
    
    [DataType("DateTime")]
    public DateTime CreateDate { get; set; }
    public virtual User Owner { get; set; }
    public virtual ICollection<User> UsersInGroup { get; set; }
    public virtual ICollection<Assignment> Assignments { get; set; }
    public virtual ICollection<UserGroups> UserGroups { get; set; }
}