namespace Core.Application.Models;

public class GroupAssignmentBaseModal
{
    public int Id { get; set; }
    public string Instructions { get; set; }
    public int GroupId { get; set; }
    public EssayModal? Essay { get; set; }
    public UserModal? Creator { get; set; }
    public DateTime CreationTime { get; set; } 
    public DateTime DueDate { get; set; }
    public decimal CompletedPercentage { get; set; }
}