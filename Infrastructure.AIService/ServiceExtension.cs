using Core.Application.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AIService;

public static class ServiceExtension
{
    public static void AddAiService(this IServiceCollection services)
    {
        services.AddHostedService<AITeacher>();
        services.AddScoped<IAICommunicateService, AICommunicateService>();
    }
}