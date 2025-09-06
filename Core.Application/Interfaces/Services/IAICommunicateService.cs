using Core.Application.Models;
using Core.Application.Models.RequestsDTO.Assignments;

namespace Core.Application.Interfaces.Services;

public interface IAICommunicateService
{
    public Task<string> GetEssayTextFromImage(List<String> images);
    public Task<EvaluateAssignmentRequest?> GetAIResponse(string essayTitle, string essayText, int expectedWordCount);
}