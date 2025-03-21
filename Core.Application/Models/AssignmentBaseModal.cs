namespace Core.Application.Models;

public class AssignmentBaseModal
{
    public int Id { get; set; }
    public string AssignmentTitle { get; set; }
    public string Instructions { get; set; }
    public int GroupId { get; set; }
    public EssayModal? Essay { get; set; }
    public UserModal? Creator { get; set; }
    public StatusBaseModal? Status { get; set; }
    public int TotalScore { get; set; } = -1;
    public DateTime CreationTime { get; set; } 
    public DateTime DueDate { get; set; }
}