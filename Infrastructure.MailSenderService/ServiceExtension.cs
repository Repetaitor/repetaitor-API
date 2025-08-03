using Core.Application.Interfaces.Services;
using infrastructure.MailSenderService.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.MailSenderService;

public static class ServiceExtension
{
    public static void AddMailService(this IServiceCollection services)
    {
        services.AddScoped<IMailService, MailService>();
    }
}