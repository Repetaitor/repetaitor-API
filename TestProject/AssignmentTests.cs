using System.Text.RegularExpressions;
using AIService;
using Core.Application.Interfaces.Repositories;
using Core.Application.Interfaces.Services;
using Core.Application.Models;
using Core.Domain.Data;
using Core.Domain.Entities;
using Core.Domain.Repositories;
using Infrastructure.ProjectServices.Implementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TestProject;

public class AssignmentTests
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
        services.AddLogging();
        services.AddScoped<IImagesStoreService, FakeImageStoreService>();
        services.AddScoped<IEssayService, EssayService>();
        services.AddScoped<IAssignmentRepository, AssignmentRepository>();
        services.AddScoped<IAssignmentService, AssignmentService>();
        services.AddScoped<IGroupService, GroupService>();
        services.AddScoped<IGroupRepository, GroupRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IEssayRepository, EssayRepository>();
        services.AddScoped<IEssayService, EssayService>();
        services.AddScoped<IAICommunicateService, AICommunicateService>();

        return services.BuildServiceProvider();
    }

    private async Task SetupTestUsers(ApplicationContext context)
    {
        var teacher = new User
        {
            Id = 1,
            FirstName = "Teacher",
            LastName = "Test",
            Email = "teacher@test.com",
            Password = "test",
            Role = "Teacher"
        };
        
        var group = new RepetaitorGroup()
        {
            Id = 1,
            GroupCode = "TEST123",
            GroupName = "Test Group",
            OwnerId = teacher.Id,
            CreateDate = DateTime.UtcNow
        };
        
        var essay1 = new Essay()
        {
            Id = 1,
            EssayTitle = "Essay1",
            EssayDescription = "Test Essay Description",
            CreatorId = teacher.Id,
            CreateDate = DateTime.UtcNow
        };
        var essay2 = new Essay()
        {
            Id = 2,
            EssayTitle = "Essay2",
            EssayDescription = "Test Essay Description2",
            CreatorId = teacher.Id,
            CreateDate = DateTime.UtcNow
        };
        
        var student = new User
        {
            Id = 2,
            FirstName = "Student",
            LastName = "Test",
            Email = "student@test.com",
            Password = "test",
            Role = "Student"
        };
        
        var userGroup = new UserGroups()
        {
            Id = 1,
            UserId = student.Id,
            GroupId = group.Id
        };
        
        context.Users.AddRange([teacher, student]);
        context.Groups.Add(group);
        context.Essays.Add(essay1);
        context.Essays.Add(essay2);
        context.UserGroups.Add(userGroup);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async void GetAssignmentByIdTest()
    {
        var serviceProvider = BuildServiceProvider();
        var assignmentService = serviceProvider.GetRequiredService<IAssignmentService>();
        var context = serviceProvider.GetRequiredService<ApplicationContext>();
        
        await SetupTestUsers(context);

        var assignment = new Assignment
        {
            Id = 1,
            Instructions = "Test Description",
            EssayId = 1,
            GroupId = 1,
            CreatorId = 1,
            CreationTime = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(7)
        };
        
        context.Assignments.Add(assignment);
        await context.SaveChangesAsync();

        var result = await assignmentService.GetAssignmentBaseInfoById(assignment.Id);

        Assert.True(result.Code == StatusCodesEnum.Success);
        Assert.NotNull(result.Data);
        Assert.Equal("Test Description", result.Data.Instructions);
        Assert.NotNull(result.Data.Creator);
        Assert.Equal(1, result.Data.Creator.Id);
    }

    [Fact]
    public async void DeleteAssignmentTest()
    {
        var serviceProvider = BuildServiceProvider();
        var assignmentService = serviceProvider.GetRequiredService<IAssignmentService>();
        var context = serviceProvider.GetRequiredService<ApplicationContext>();
        
        await SetupTestUsers(context);

        var assignment = new Assignment
        {
            Id = 1,
            Instructions = "Test Description",
            CreatorId = 1,
            EssayId = 1,
            GroupId = 1,
            CreationTime = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(7)
        };
        
        context.Assignments.Add(assignment);
        await context.SaveChangesAsync();

        var deleteResult = await assignmentService.DeleteAssignment(1, assignment.Id);

        Assert.True(deleteResult.Code == StatusCodesEnum.Success);
        Assert.NotNull(deleteResult.Data);
        Assert.True(deleteResult.Data.Result);

        var getDeleted = await assignmentService.GetAssignmentBaseInfoById( assignment.Id);
        Assert.False(getDeleted.Code == StatusCodesEnum.Success);
    }

    [Fact]
    public async void GetTeacherAssignmentsTest()
    {
        var serviceProvider = BuildServiceProvider();
        var assignmentService = serviceProvider.GetRequiredService<IAssignmentService>();
        var context = serviceProvider.GetRequiredService<ApplicationContext>();
        
        await SetupTestUsers(context);

        var assignments = new List<Assignment>
        {
            new()
            {
                Id = 1,
                Instructions = "Description 1",
                CreatorId = 1,
                GroupId = 1,
                EssayId = 1,
                CreationTime = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(7)
            },
            new()
            {
                Id = 2,
                Instructions = "Description 2",
                CreatorId = 1,
                GroupId = 1,
                EssayId = 1,
                CreationTime = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(7)
            }
        };
        
        context.Assignments.AddRange(assignments);
        await context.SaveChangesAsync();

        var result = await assignmentService.GetGroupAssignments(1, 1,null, null);

        Assert.True(result.Code == StatusCodesEnum.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Result!.Count);
        Assert.Contains(result.Data.Result, a => a.Id == 1);
        Assert.Contains(result.Data.Result, a => a.Id == 2);
    }

    [Fact]
    public async void UpdateAssignmentTest()
    {
        var serviceProvider = BuildServiceProvider();
        var assignmentService = serviceProvider.GetRequiredService<IAssignmentService>();
        var context = serviceProvider.GetRequiredService<ApplicationContext>();
        
        await SetupTestUsers(context);

        var assignment = new Assignment
        {
            Id = 1,
            Instructions = "Test Description",
            EssayId = 1,
            GroupId = 1,
            CreatorId = 1,
            CreationTime = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(7)
        };
        
        context.Assignments.Add(assignment);
        await context.SaveChangesAsync();

        var updateResult = await assignmentService.UpdateAssignment(1, new UpdateAssignmentRequest()
        {
            AssignmentId = assignment.Id,
            EssayId = 2,
            Instructions = "Updated Description",
            DueDate = DateTime.UtcNow.AddDays(14)
        });

        Assert.True(updateResult.Code == StatusCodesEnum.Success);
        Assert.NotNull(updateResult.Data);
        Assert.Equal("Updated Description", updateResult.Data.Instructions);
        Assert.Equal(2, updateResult.Data.Essay!.Id);
    }

    [Fact]
    public async void UnauthorizedAssignmentOperationsTest()
    {
        var serviceProvider = BuildServiceProvider();
        var assignmentService = serviceProvider.GetRequiredService<IAssignmentService>();
        var context = serviceProvider.GetRequiredService<ApplicationContext>();
        
        await SetupTestUsers(context);

        var assignment = new Assignment
        {
            Id = 1,
            Instructions = "Test Description",
            EssayId = 1,
            GroupId = 1,
            CreatorId = 1,
            CreationTime = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(7)
        };
        
        context.Assignments.Add(assignment);
        await context.SaveChangesAsync();

        var updateResult = await assignmentService.UpdateAssignment(2, new UpdateAssignmentRequest()
        {
            AssignmentId = assignment.Id,
            EssayId = assignment.EssayId,
            Instructions = "Updated Description",
            DueDate = DateTime.UtcNow.AddDays(14)
        });

        Assert.True(updateResult.Code == StatusCodesEnum.InternalServerError);

        var deleteResult = await assignmentService.DeleteAssignment(2, assignment.Id);
        Assert.True(deleteResult.Code == StatusCodesEnum.InternalServerError);
    }
}