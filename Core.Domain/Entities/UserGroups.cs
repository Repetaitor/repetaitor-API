using System.Text.RegularExpressions;

namespace Core.Domain.Entities;

public class UserGroups
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int GroupId { get; set; }
    
    public virtual User User { get; set; }
    public virtual RepetaitorGroup Group { get; set; }
}