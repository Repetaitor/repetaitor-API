using Core.Application.Mappers;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;

namespace Core.Application;

public static class ServiceExtension
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AssignmentProfile>();
            cfg.AddProfile<EssayProfile>();
            cfg.AddProfile<GroupProfile>();
            cfg.AddProfile<UserProfile>();
            cfg.AddProfile<ChatProfile>();
        });

        var mapper = config.CreateMapper();
        services.AddSingleton(mapper);
    }
}