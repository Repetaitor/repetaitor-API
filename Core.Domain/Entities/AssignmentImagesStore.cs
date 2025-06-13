namespace Core.Domain.Entities;

public class AssignmentImagesStore
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int AssignmentId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}