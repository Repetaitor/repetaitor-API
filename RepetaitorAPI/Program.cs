using AIService;
using Core.Application.Interfaces.Repositories;
using Core.Application.Interfaces.Services;
using Core.Domain.Data;
using Core.Domain.Repositories;
using infrastructure.MailSenderService.Implementations;
using Infrastructure.ProjectServices.Implementations;
using Microsoft.Extensions.Logging.AzureAppServices;
using RepetaitorAPI;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();
builder.Logging.AddAzureWebAppDiagnostics();
builder.Services.Configure<AzureFileLoggerOptions>(options =>
{
    options.FileName = "repetaitor-logs-";
    options.FileSizeLimit = 10 * 1024 * 1024; // 10 MB
    options.RetainedFileCountLimit = 10; // Keep the last 10 log files
});
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Error);
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Error);
builder.Services.AddDbContext<ApplicationContext>();
builder.Services.AddHostedService<AITeacher>();
builder.Services.AddScoped<IUserAuthorizationService, UserAuthorizationService>();
builder.Services.AddScoped<IImagesStoreService, ImagesStoreService.ImagesStoreService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthCodesRepository, AuthCodesRepository>();
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddScoped<IJWTTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IEssayRepository, EssayRepository>();
builder.Services.AddScoped<IEssayService, EssayService>();
builder.Services.AddScoped<IGroupRepository, GroupRepository>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IAssignmentRepository, AssignmentRepository>();
builder.Services.AddScoped<IAssignmentService, AssignmentService>();
builder.Services.AddScoped<IAICommunicateService, AICommunicateService>();
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.ConfigureSwaggGen();
builder.Services.ConfigureAuthorization();
builder.Services.ConfigureCors();
builder.Services.AddControllers();


var app = builder.Build();
app.UseCors("_myAllowSpecificOrigins");
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.Run();