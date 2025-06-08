using Core.Application.Models;
using Microsoft.AspNetCore.Http;

namespace Core.Application.Interfaces.Services;

public interface IAICommunicateService
{
    public Task<AIReturnViewModel?> GetAIResponse(string essayTitle, string essayText, int expectedWordCount);
    public Task<string> GetEssayTextFromImage(List<String> images);
}