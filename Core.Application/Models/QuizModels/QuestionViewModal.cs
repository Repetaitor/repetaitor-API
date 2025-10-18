namespace Core.Application.Models.QuizModels;

public class QuestionViewModal
{
    public int QuestionId { get; set; }
    public int QuestionType { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string Explanation { get; set; } = string.Empty;
    public List<string> Options { get; set; } = [];
    public int CorrectAnswerIndex { get; set; }
}