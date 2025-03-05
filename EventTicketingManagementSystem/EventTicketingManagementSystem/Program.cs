using EventTicketingManagementSystem.API.Middlewares;
using EventTicketingManagementSystem.Data;
using EventTicketingManagementSystem.Data.Data;
using EventTicketingManagementSystem.Services;
using EventTicketingMananagementSystem.Core.Hubs;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using EventTicketingMananagementSystem.Core.Constants;

[ExcludeFromCodeCoverage]
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Load User Secrets and Environment Variables
        builder.Configuration.AddUserSecrets<Program>()
                             .AddEnvironmentVariables();

        var connectionString = builder.Configuration["ConnectionString"] ?? throw new Exception("Cannot get Connection String");

        // Add repository.
        builder.Services.AddDataServices(connectionString);

        // Add services.
        builder.Services.AddServiceLayer();

        builder.Services.AddControllers();

        // Add SignalR
        builder.Services.AddSignalR();


        // Add FluentValidation
        builder.Services
            .AddFluentValidationAutoValidation()
            .AddFluentValidationClientsideAdapters();

        var fullUrl = builder.Configuration["vnp_ReturnUrl"] ?? throw new Exception("Cannot get Connection String"); ;
        Uri uri = new Uri(fullUrl.ToString());
        string originUrl = uri.GetLeftPart(UriPartial.Authority);

        // Add CORS services
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy.WithOrigins(originUrl)
                      .AllowAnyMethod()                   // Allow any HTTP method (GET, POST, etc.)
                      .AllowAnyHeader()                   // Allow any header
                      .AllowCredentials();                // Allow credentials (cookies, authorization headers)
            });
        });

        // Add rate limiting services
        builder.Services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    
            options.OnRejected = async (context, _) =>
            {  
                await context.HttpContext.Response.WriteAsJsonAsync(new
                {
                    message = "Too many requests. Please try again later.",
                });
            };
            
            options.AddFixedWindowLimiter(RateLimitConst.FixedRateLimit, opt =>
            {
                opt.PermitLimit = 50; 
                opt.Window = TimeSpan.FromMinutes(1);
                opt.QueueLimit = 0; 
            });

            // Global rate limit configuration
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: context.User?.Identity?.Name ?? context.Request.Headers.Host.ToString(),
                    factory: partition => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = 300,
                        Window = TimeSpan.FromMinutes(1)
                    }));
        });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(x =>
        {
            var jwtSecurityScheme = new OpenApiSecurityScheme
            {
                BearerFormat = "JWT",
                Name = "JWT Authentication",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",
                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };

            x.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
            x.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
        { jwtSecurityScheme, Array.Empty<string>() }
            });
        });

        // Token-based authentication configuration
        string key = builder.Configuration["JwtKey"] ?? throw new Exception("Cannot get JwtKey");
        builder.Services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = false;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key))
            };
        });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy(RoleConsts.Admin, policy => policy.RequireRole(RoleConsts.Admin));
            options.AddPolicy(RoleConsts.User, policy => policy.RequireRole(RoleConsts.User));
        });

        var app = builder.Build();

        // Apply any pending migrations at startup
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.Migrate();  // Automatically applies migrations at startup
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors("AllowFrontend");

        app.UseRateLimiter();

        app.UseHttpsRedirection();

        app.UseMiddleware<RequestTimingMiddleware>();

        app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.UseAuthorization();

        app.UseMiddleware<CurrentUserMiddleware>();

        app.MapHub<NotificationHub>("/notificationHub");

        app.MapControllers();

        app.Run();
    }
}