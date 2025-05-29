using System.Text;
using AIService;
using Core.Application.Interfaces.Repositories;
using Core.Application.Interfaces.Services;
using Core.Domain.Data;
using Core.Domain.Repositories;
using infrastructure.MailService.Implementations;
using Infrastructure.ProjectServices.Implementations;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationContext>();
//builder.Services.AddHostedService<AITeacher>();
builder.Services.AddScoped<IUserAuthorizationService, UserAuthorizationService>();
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
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "RepetaitorApi", Version = "v1" });

    c.AddSecurityDefinition("Cookies", new OpenApiSecurityScheme
    {
        Description =
            "Cookie-based authentication. The authentication cookie will be sent automatically by the browser.",
        Name = ".AspNetCore.Cookies",
        In = ParameterLocation.Cookie,
        Type = SecuritySchemeType.ApiKey
    });

    var securityScheme = new OpenApiSecurityScheme
    {
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Cookies"
        }
    };

    var securityRequirement = new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    };

    c.AddSecurityRequirement(securityRequirement);
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie("Cookies", options =>
{
    options.LoginPath = "/api/User/SignIn";
    options.Cookie.Name = ".AspNetCore.Cookies";
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.Path = "/";
    options.Cookie.HttpOnly = true; 
    options.Cookie.MaxAge = TimeSpan.FromDays(7);
    options.SlidingExpiration = false;
    options.Events = new CookieAuthenticationEvents
    {
        OnRedirectToLogin = ctx =>
        {
            if (ctx.Request.Path.StartsWithSegments("/api"))
            {
                ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
            }
            else
            {
                ctx.Response.Redirect(ctx.RedirectUri);
            }
            return Task.CompletedTask;
        },
        OnRedirectToAccessDenied = ctx =>
        {
            if (ctx.Request.Path.StartsWithSegments("/api"))
            {
                ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
            }
            else
            {
                ctx.Response.Redirect(ctx.RedirectUri);
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "_myAllowSpecificOrigins",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000", "https://localhost:3000", "https://repetaitor.netlify.app").AllowCredentials().AllowAnyMethod().AllowAnyHeader();
        });
});
builder.Services.AddControllers();
var app = builder.Build();
app.UseCors("_myAllowSpecificOrigins");
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

app.Run();

public partial class MyProgram
{
}

public static class AuthOptions
{
    public const string ISSUER = "RepetaitorManager";
    public const string AUDIENCE = "RepetaitorClient";
    const string KEY = "mysupersecret_secretsecretsecretkey!123";
    public static SymmetricSecurityKey GetSymmetricSecurityKey() => new(Encoding.UTF8.GetBytes(KEY));
}