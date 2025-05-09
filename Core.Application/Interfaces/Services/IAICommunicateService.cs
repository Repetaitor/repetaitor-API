using Core.Application.Models;

namespace Core.Application.Interfaces.Services;

public interface IAICommunicateService
{
    public Task<AIReturnViewModel?> GetAIResponse(string essayTitle, string essayText, int expectedWordCount);
}