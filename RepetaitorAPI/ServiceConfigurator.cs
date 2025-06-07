using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.OpenApi.Models;

namespace RepetaitorAPI;

public static class ServiceExtensions
{
    public static void ConfigureAuthorization(this IServiceCollection services)
    {
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie("Cookies", options =>
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
    }

    public static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(name: "_myAllowSpecificOrigins",
                policy =>
                {
                    policy.WithOrigins("http://localhost:3000", "https://localhost:3000",
                        "https://repetaitor.netlify.app").AllowCredentials().AllowAnyMethod().AllowAnyHeader();
                });
        });
    }

    public static void ConfigureSwaggGen(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
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
    }
}