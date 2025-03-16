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
        [DataType("boolean")] 
        public bool IsTeacher { get; set; } = false;
        [DataType("nvarchar(64)")]
        public string Password { get; set; }
    }
}
