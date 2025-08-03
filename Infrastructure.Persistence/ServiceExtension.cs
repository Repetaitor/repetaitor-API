using Core.Application.Interfaces.Repositories;
using Infrastructure.Persistence.AppContext;
using Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence;

public static class ServiceExtension
{
    public static void AddRepositoriesLayer(this IServiceCollection services)
    {
        services.AddDbContext<ApplicationContext>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAuthCodesRepository, AuthCodesRepository>();
        services.AddScoped<IChatRepository, ChatRepository>();
        services.AddScoped<IEssayRepository, EssayRepository>();
        services.AddScoped<IGroupRepository, GroupRepository>();
        services.AddScoped<IAssignmentRepository, AssignmentRepository>();
    }
}