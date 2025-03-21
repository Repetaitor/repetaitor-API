using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Core.Domain.Entities;

public class AuthenticationCodes
{
    public int Id { get; set; }
    public int UserId { get; set; }
    [DataType("nvarchar(64)")]
    public string Guid { get; set; }
    [DataType("nvarchar(64)")]
    public string Email { get; set; }
    [DataType("nvarchar(64)")]
    public string Code { get; set; }
    [DataType("DateTime")] 
    public DateTime CreateDate { get; set; } = DateTime.Now;
    [DataType("boolean")]
    public bool IsVerified { get; set; }
}