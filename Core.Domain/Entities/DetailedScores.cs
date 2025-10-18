namespace Core.Domain.Entities;

public class DetailedScores
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int AssignmentId { get; set; }
    public int Vocabulary { get; set; } 
    public int SpellingAndPunctuation { get; set; } 
    public int Grammar { get; set; } 
}