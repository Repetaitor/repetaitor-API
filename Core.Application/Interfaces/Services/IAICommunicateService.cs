using Core.Application.Models;
using Core.Application.Models.QuizModels;
using Core.Application.Models.RequestsDTO.Assignments;
using Core.Application.Models.ReturnViewModels;

namespace Core.Application.Interfaces.Services;

public interface IAICommunicateService
{
    public Task<string> GetEssayTextFromImage(List<String> images);
    public Task<(EvaluateAssignmentRequest?, QuizViewModel?)> GetAIResponse(string essayTitle, string essayText, int expectedWordCount);
    public Task<QuizViewModel?> GetQuizQuestions(List<string> questionTypes, string promptPath);
    public Task<string> GetEssayTitle();
}