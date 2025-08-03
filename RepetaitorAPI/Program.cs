using AIService;
using Core.Application;
using Core.Application.Interfaces.Repositories;
using Core.Application.Interfaces.Services;
using Infrastructure.ImagesStore;
using Infrastructure.MailSenderService;
using infrastructure.MailSenderService.Implementations;
using Infrastructure.Persistence;
using Infrastructure.Persistence.AppContext;
using Infrastructure.Persistence.Repositories;
using Infrastructure.ProjectServices;
using Infrastructure.ProjectServices.Implementations;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Extensions.Logging.AzureAppServices;
using RepetaitorAPI;
using RepetaitorAPI.Hubs;

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
builder.Services.AddApplicationServices();
builder.Services.AddAiService();
builder.Services.AddMailService();
builder.Services.AddProjectServices();
builder.Services.AddImageStore();
builder.Services.AddRepositoriesLayer();
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.ConfigureSwaggGen();
builder.Services.ConfigureAuthorization();
builder.Services.ConfigureCors();
builder.Services.AddControllers();
builder.Services.AddSignalR();


var app = builder.Build();
app.UseCors("_myAllowSpecificOrigins");
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
	
app.MapHub<ChatHub>("/chatHub",
    options => {
        options.ApplicationMaxBufferSize = 128;
        options.TransportMaxBufferSize = 128;
        options.LongPolling.PollTimeout = TimeSpan.FromMinutes(1);
        options.Transports = HttpTransportType.LongPolling | HttpTransportType.WebSockets;
    });
app.Run();