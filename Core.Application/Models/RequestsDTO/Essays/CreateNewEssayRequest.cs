namespace Core.Application.Models.RequestsDTO.Essays;

public class CreateNewEssayRequest
{
    public string EssayTitle { get; set; }
    public string EssayDescription { get; set; }
    public int ExpectedWordCount { get; set; }
}