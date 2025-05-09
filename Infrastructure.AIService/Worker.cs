using Core.Application.Interfaces.Services;
using Core.Application.Models.DTO.Assignments;
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
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }

            using (var scope = scopeFactory.CreateScope())
            {
                var assignmentService = scope.ServiceProvider.GetRequiredService<IAssignmentService>();
                var aiService = scope.ServiceProvider.GetRequiredService<IAICommunicateService>();
                var assignments = await assignmentService.GetUserAssignmentViewForAI(1019, 5);
                if (assignments != null && assignments.Count != 0)
                {
                    foreach (var assignment in assignments)
                    {
                        var result = await aiService.GetAIResponse(assignment.EssayTitle, assignment.EssayText,
                            assignment.ExpectedWordCount);
                        if (result == null) continue;
                        var res = await assignmentService.EvaluateAssignments(1019, new EvaluateAssignmentRequest()
                        {
                            UserId = assignment.UserId,
                            AssignmentId = assignment.AssignmentId,
                            FluencyScore = result.FluencyScore,
                            GrammarScore = result.GrammarScore,
                            EvaluationTextComments = result.EvaluationTextComments,
                            GeneralComments = result.GeneralComments
                        });
                        if (!res.Result)
                        {
                            Console.WriteLine(
                                $"Something went wrong while evaluating assignment for {assignment.UserId} assignment {assignment.AssignmentId}");
                        }
                    }
                }
            }

            await Task.Delay(1000, stoppingToken);
        }
    }
}