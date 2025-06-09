using Core.Application.Models;
using Core.Application.Models.DTO.Assignments;

namespace Core.Application.Interfaces.Services;

public interface IAICommunicateService
{
    public Task<EvaluateAssignmentRequest?> GetAIResponse(string essayTitle, string essayText, int expectedWordCount);
}