using Core.Application.Interfaces.Repositories;
using Core.Application.Interfaces.Services;
using Infrastructure.ProjectServices.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.ProjectServices;

public static class ServiceExtension
{
    public static void AddProjectServices(this IServiceCollection services)
    {
        services.AddScoped<IUserAuthorizationService, UserAuthorizationService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<IEssayService, EssayService>();
        services.AddScoped<IGroupService, GroupService>();
        services.AddScoped<IAssignmentService, AssignmentService>();
    }
}