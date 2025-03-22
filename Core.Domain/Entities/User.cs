using System.ComponentModel.DataAnnotations;

namespace Core.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        [DataType("nvarchar(64)")]
        public string FirstName { get; set; }
        [DataType("nvarchar(64)")]
        public string LastName { get; set; }
        [DataType("nvarchar(64)")]
        public string Email { get; set; }
        [DataType("nvarchar(64)")] 
        public string Role { get; set; } = "Student";
        [DataType("nvarchar(64)")]
        public string Password { get; set; }
        [DataType("boolean")] 
        public bool isActive { get; set; } = false;
        [DataType("DateTime")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public virtual ICollection<RepetaitorGroup> RepetaitorGroups { get; set; }
        public virtual ICollection<Assignment> CreatedAssignments { get; set; }
        public virtual ICollection<UserAssignment> AssignedAssignments { get; set; }
        public virtual ICollection<Essay> CreatedEssays { get; set; }
    }
}
