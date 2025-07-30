using Core.Application.Interfaces.Repositories;
using Core.Application.Interfaces.Services;
using Core.Application.Models;
using Core.Domain.Entities;
using Infrastructure.Persistence.AppContext;
using Infrastructure.Persistence.Repositories;
using Infrastructure.ProjectServices.Implementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TestProject;

public class GroupTests
{
    private IServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>()!)
            .Build();
        services.AddLogging();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddDbContext<ApplicationContext>(options =>
            options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

        services.AddScoped<IGroupRepository, GroupRepository>();
        services.AddScoped<IGroupService, GroupService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();

        return services.BuildServiceProvider();
    }

    [Fact]
    public async void CreateGroupTest()
    {
        var serviceProvider = BuildServiceProvider();
        var groupService = serviceProvider.GetRequiredService<IGroupService>();
        var context = serviceProvider.GetRequiredService<ApplicationContext>();
        var user = new User { Id = 1, FirstName = "user1", LastName = "user1", Email = "Test1@gmail.com" , 
            Password = "test", Role = "Teacher" };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        
        var group = await groupService.CreateGroup("Test Group", 1);
        
        Assert.True(group.Code == StatusCodesEnum.Success);
        Assert.NotNull(group.Data);
        Assert.Equal("Test Group", group.Data!.GroupName);
        Assert.Equal(1, group.Data.Owner.Id);
    }

    [Fact]
    public async void GetGroupByIdTest()
    {
        var serviceProvider = BuildServiceProvider();
        var groupService = serviceProvider.GetRequiredService<IGroupService>();
        
        var context = serviceProvider.GetRequiredService<ApplicationContext>();
        var user = new User { Id = 1, FirstName = "user1", LastName = "user1", Email = "Test1@gmail.com" , 
            Password = "test", Role = "Teacher" };
        context.Users.Add(user);
        
        var createdGroup = await groupService.CreateGroup("Test Group", 1);
        var group = await groupService.GetGroupBaseInfoById(1, createdGroup.Data!.Id);

        Assert.True(group.Code == StatusCodesEnum.Success);
        Assert.NotNull(group.Data);
        Assert.Equal("Test Group", group.Data!.GroupName);
        Assert.Equal(1, group.Data.Owner.Id);
    }

    [Fact]
    public async void DeleteGroupTest()
    {
        var serviceProvider = BuildServiceProvider();
        var groupService = serviceProvider.GetRequiredService<IGroupService>();
        
        var context = serviceProvider.GetRequiredService<ApplicationContext>();
        var user = new User { Id = 1, FirstName = "user1", LastName = "user1", Email = "Test1@gmail.com" , 
            Password = "test", Role = "Teacher" };
        context.Users.Add(user);
        
        var createdGroup = await groupService.CreateGroup("Test Group", 1);
        var deleteResult = await groupService.DeleteGroup(1, createdGroup.Data!.Id);

        Assert.True(deleteResult.Code == StatusCodesEnum.Success);
        Assert.NotNull(deleteResult.Data);
        Assert.True(deleteResult.Data.Result);

        var getDeleted = await groupService.GetGroupBaseInfoById(1, createdGroup.Data.Id);
        Assert.False(getDeleted.Code == StatusCodesEnum.Success);
    }

    [Fact]
    public async void GetUserGroupsTest()
    {
        var serviceProvider = BuildServiceProvider();
        var groupService = serviceProvider.GetRequiredService<IGroupService>();
        
        var context = serviceProvider.GetRequiredService<ApplicationContext>();
        var user1 = new User { Id = 1, FirstName = "user1", LastName = "user1", Email = "Test1@gmail.com" , 
            Password = "test", Role = "Teacher" };
        var user2 = new User
        {
            Id = 2, FirstName = "user2", LastName = "user2", Email = "Test2@gmail.com",
            Password = "test", Role = "Teacher"
        };
        context.Users.AddRange([user1, user2]);
        
        var group1 = await groupService.CreateGroup("Test Group 1", 2);
        var group2 = await groupService.CreateGroup("Test Group 2", 2);

        var userGroups = await groupService.GetTeacherGroups(2);
        
        Assert.True(userGroups.Code == StatusCodesEnum.Success);
        Assert.NotNull(userGroups.Data);
        Assert.Equal(2, userGroups.Data.Count);
        Assert.Contains(userGroups.Data, g => g.Id == group1.Data!.Id);
        Assert.Contains(userGroups.Data, g => g.Id == group2.Data!.Id);

        await groupService.DeleteGroup(2, group1.Data!.Id);
        
        userGroups = await groupService.GetTeacherGroups(2);
        Assert.True(userGroups.Code == StatusCodesEnum.Success);
        Assert.NotNull(userGroups.Data);
        Assert.Single(userGroups.Data);
        Assert.DoesNotContain(userGroups.Data, g => g.Id == group1.Data.Id);
        Assert.Contains(userGroups.Data, g => g.Id == group2.Data!.Id);
    }

    [Fact]
    public async void UpdateGroupTest()
    {
        var serviceProvider = BuildServiceProvider();
        var groupService = serviceProvider.GetRequiredService<IGroupService>();
        
        var context = serviceProvider.GetRequiredService<ApplicationContext>();
        var user = new User { Id = 1, FirstName = "user1", LastName = "user1", Email = "Test1@gmail.com" , 
            Password = "test", Role = "Teacher" };
        context.Users.Add(user);
        
        var group = await groupService.CreateGroup("Test Group", 1);
        var updatedGroup = await groupService.UpdateGroupTitle(1, group.Data!.Id, "Updated Group");

        Assert.True(updatedGroup.Code == StatusCodesEnum.Success);
        Assert.NotNull(updatedGroup.Data);
        Assert.Equal("Updated Group", updatedGroup.Data!.GroupName);
        Assert.Equal(1, updatedGroup.Data.Owner.Id);
    }

    [Fact]
    public async void UnauthorizedGroupOperationsTest()
    {
        var serviceProvider = BuildServiceProvider();
        var groupService = serviceProvider.GetRequiredService<IGroupService>();
        
        var context = serviceProvider.GetRequiredService<ApplicationContext>();
        var user1 = new User { Id = 1, FirstName = "user1", LastName = "user1", Email = "Test1@gmail.com" , 
            Password = "test", Role = "Teacher" };
        var user2 = new User
        {
            Id = 2, FirstName = "user2", LastName = "user2", Email = "Test2@gmail.com",
            Password = "test", Role = "Student"
        };
        context.Users.AddRange([user1, user2]);
        
        
        var group = await groupService.CreateGroup("Test Group", 1);
        
        var updateResult = await groupService.UpdateGroupTitle(2, group.Data!.Id, "Updated Group");
        Assert.True(updateResult.Code == StatusCodesEnum.InternalServerError);

        var deleteResult = await groupService.DeleteGroup(2, group.Data.Id);
        Assert.True(deleteResult.Code == StatusCodesEnum.InternalServerError);
    }
}