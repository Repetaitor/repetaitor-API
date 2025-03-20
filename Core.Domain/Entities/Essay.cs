namespace Core.Domain.Entities;

public class Essay
{
    public int Id { get; set; }
    public int CreatorId { get; set; }
    public string EssayTitle { get; set; }
    public string EssayDescription { get; set; }
    public int ExpectedWordCount { get; set; }
}