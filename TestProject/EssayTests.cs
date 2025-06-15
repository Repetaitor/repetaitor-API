using Core.Application.Interfaces.Repositories;
using Core.Application.Interfaces.Services;
using Core.Application.Models;
using Core.Domain.Data;
using Core.Domain.Repositories;
using Infrastructure.ProjectServices.Implementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TestProject;

public class EssayTests
{
    private IServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>())
            .Build();

        services.AddSingleton<IConfiguration>(configuration);
        services.AddDbContext<ApplicationContext>(options =>
            options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

        services.AddScoped<IEssayRepository, EssayRepository>();
        services.AddScoped<IEssayService, EssayService>();

        return services.BuildServiceProvider();
    }
    [Fact]
    public async void EssayCreateTest()
    {
        var serviceProvider = BuildServiceProvider();
        
        var essayService = serviceProvider.GetRequiredService<IEssayService>();

        var ess = await essayService.CreateNewEssay("New Essay", 
            "This is a test essay", 500, 1);
        Assert.True(ess.Code == StatusCodesEnum.Success);
        Assert.NotNull(ess.Data);
        Assert.Equal("New Essay", ess.Data!.EssayTitle);
        Assert.Equal("This is a test essay", ess.Data.EssayDescription);
        Assert.Equal(500, ess.Data.ExpectedWordCount);
        Assert.Equal(1, ess.Data.CreatorId);
    }
    
    [Fact]
    public async void GetEssayByIdTest()
    {
        var serviceProvider = BuildServiceProvider();
        var essayService = serviceProvider.GetRequiredService<IEssayService>();

        var ess = await essayService.CreateNewEssay("New Essay", 
            "This is a test essay", 500, 1);
        
        var getEss = await essayService.GetEssayById(ess.Data!.Id);
        Assert.True(getEss.Code == StatusCodesEnum.Success);
        Assert.NotNull(getEss.Data);
        Assert.Equal("New Essay", getEss.Data!.EssayTitle);
        Assert.Equal("This is a test essay", getEss.Data.EssayDescription);
        Assert.Equal(500, getEss.Data.ExpectedWordCount);
        Assert.Equal(1, getEss.Data.CreatorId);
    }
    
    [Fact]
    public async void DeleteEssayTest()
    {
        var serviceProvider = BuildServiceProvider();
        var essayService = serviceProvider.GetRequiredService<IEssayService>();

        var ess = await essayService.CreateNewEssay("New Essay", 
            "This is a test essay", 500, 1);
        
        var getEss = await essayService.DeleteEssay(ess.Data!.Id, 1);
        Assert.True(getEss.Code == StatusCodesEnum.Success);
        Assert.NotNull(getEss.Data);
        Assert.True(getEss.Data!.Result);
        
        var deletedEss = await essayService.GetEssayById(ess.Data.Id);
        Assert.True(deletedEss.Code == StatusCodesEnum.InternalServerError);
    }
    
    [Fact]
    public async void GetUserEssaysTest()
    {
        var serviceProvider = BuildServiceProvider();
        var essayService = serviceProvider.GetRequiredService<IEssayService>();

        var ess1 = await essayService.CreateNewEssay("New Essay 1", 
            "This is a test essay 1", 500, 2);
        
        var userEssays = await essayService.GetUserEssays(2);
        Assert.True(userEssays.Code == StatusCodesEnum.Success);
        Assert.NotNull(userEssays.Data);
        Assert.Single(userEssays.Data);
        Assert.Equal(userEssays.Data![0].Id, ess1.Data!.Id);
        
        var ess2 = await essayService.CreateNewEssay("New Essay 2", 
            "This is a test essay 2", 300, 2);
        
        userEssays = await essayService.GetUserEssays(2);
        Assert.True(userEssays.Code == StatusCodesEnum.Success);
        Assert.NotNull(userEssays.Data);
        Assert.Equal(2, userEssays.Data!.Count);
        Assert.Contains(userEssays.Data, e => e.Id == ess1.Data!.Id);
        Assert.Contains(userEssays.Data, e => e.Id == ess2.Data!.Id);
        
        var res = await essayService.DeleteEssay(ess1.Data!.Id, 2);
        
        Assert.True(res.Code == StatusCodesEnum.Success);
        Assert.NotNull(res.Data);
        Assert.True(res.Data!.Result);
        userEssays = await essayService.GetUserEssays(2);
        Assert.True(userEssays.Code == StatusCodesEnum.Success);
        Assert.NotNull(userEssays.Data);
        Assert.Single(userEssays.Data);
        Assert.DoesNotContain(userEssays.Data, e => e.Id == ess1.Data!.Id);
        Assert.Contains(userEssays.Data, e => e.Id == ess2.Data!.Id);
    }
    
    [Fact]
    public async void UpdateEssayTest()
    {
        var serviceProvider = BuildServiceProvider();
        var essayService = serviceProvider.GetRequiredService<IEssayService>();

        var ess = await essayService.CreateNewEssay("New Essay", 
            "This is a test essay", 500, 3);
        
        var updatedEss = await essayService.UpdateEssay(ess.Data!.Id, "Updated Essay", 
            "This is an updated test essay", 600, 3);
        
        Assert.True(updatedEss.Code == StatusCodesEnum.Success);
        Assert.NotNull(updatedEss.Data);
        Assert.Equal("Updated Essay", updatedEss.Data!.EssayTitle);
        Assert.Equal("This is an updated test essay", updatedEss.Data.EssayDescription);
        Assert.Equal(600, updatedEss.Data.ExpectedWordCount);
        Assert.Equal(3, updatedEss.Data.CreatorId);
    }
}