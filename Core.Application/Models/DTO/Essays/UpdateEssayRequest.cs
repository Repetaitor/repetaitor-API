namespace Core.Application.Models.DTO.Essays;

public class UpdateEssayRequest
{
    public int EssayId { get; set; }
    public string EssayTitle { get; set; }
    public string EssayDescription { get; set; }
    public int ExpectedWordCount { get; set; }
}