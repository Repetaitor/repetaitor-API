namespace Core.Application.Models;

public class EssayModal
{
    public int Id { get; set; }
    public string EssayTitle { get; set; }
    public string EssayDescription { get; set; }
    public int ExpectedWordCount { get; set; }
    public int CreatorId { get; set; }
    public DateTime CreateDate { get; set; }
}