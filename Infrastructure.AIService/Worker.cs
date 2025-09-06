using Core.Application.Interfaces.Services;
using Core.Application.Models;
using Core.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AIService;

public class AITeacher(ILogger<AITeacher> logger, IServiceScopeFactory scopeFactory)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                try
                {
                    var assignmentService = scope.ServiceProvider.GetRequiredService<IAssignmentService>();
                    var aiService = scope.ServiceProvider.GetRequiredService<IAICommunicateService>();
                    var assignments = await assignmentService.GetUserAssignmentViewForAI(1019, 5);
                    if (assignments.Code == StatusCodesEnum.Success && assignments.Data != null && assignments.Data.Count != 0)
                    {
                        foreach (var assignment in assignments.Data)
                        {
                            var result = await aiService.GetAIResponse(assignment.EssayTitle, assignment.EssayText,
                                assignment.ExpectedWordCount);
                            if (result == null) continue;
                            result.AssignmentId = assignment.AssignmentId;
                            result.UserId = assignment.UserId;
                            var res = await assignmentService.EvaluateAssignments(1019, result);
                            if (res.Code != 0 || !res.Data!.Result)
                            {
                                Console.WriteLine(
                                    $"Something went wrong while evaluating assignment for {assignment.UserId} assignment {assignment.AssignmentId}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while processing assignments.");
                }
            }
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
}