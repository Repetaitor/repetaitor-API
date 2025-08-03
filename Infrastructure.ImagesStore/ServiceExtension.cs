using Core.Application.Interfaces.Services;
using Infrastructure.ImagesStore.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.ImagesStore;

public static class ServiceExtension
{
    public static void AddImageStore(this IServiceCollection services)
    {
        //services.AddScoped<IImagesStoreService, ImagesStoreService>();
    }
}