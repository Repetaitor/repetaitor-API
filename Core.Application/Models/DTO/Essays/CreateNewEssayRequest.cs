namespace Core.Application.Models.DTO.Essays;

public class CreateNewEssayRequest
{
    public string EssayTitle { get; set; }
    public string EssayDescription { get; set; }
    public int ExpectedWordCount { get; set; }
}