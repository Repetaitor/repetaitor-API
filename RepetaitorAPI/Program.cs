using System.Text;
using Core.Application.Interfaces.Repositories;
using Core.Application.Interfaces.Services;
using Core.Domain.Data;
using Core.Domain.Repositories;
using infrastructure.MailService.Implementations;
using Infrastructure.ProjectServices.Implementations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationContext>();
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
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "RepetaitorApi", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    var securityScheme = new OpenApiSecurityScheme
    {
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    var securityRequirement = new OpenApiSecurityRequirement
    {
        { securityScheme, new string[] { } }
    };

    c.AddSecurityRequirement(securityRequirement);
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {

            ValidateIssuer = true,

            ValidIssuer = AuthOptions.ISSUER,

            ValidateAudience = true,

            ValidAudience = AuthOptions.AUDIENCE,

            ValidateLifetime = false,

            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),

            ValidateIssuerSigningKey = false,
        };
        options.TokenValidationParameters.RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
    });
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "_myAllowSpecificOrigins",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
        });
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
var app = builder.Build();
app.UseCors("_myAllowSpecificOrigins");
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();

public static class AuthOptions
{
    public const string ISSUER = "RepetaitorManager";
    public const string AUDIENCE = "RepetaitorClient";
    const string KEY = "mysupersecret_secretsecretsecretkey!123";
    public static SymmetricSecurityKey GetSymmetricSecurityKey() => new(Encoding.UTF8.GetBytes(KEY));
}