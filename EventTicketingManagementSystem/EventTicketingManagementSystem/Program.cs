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
            options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
            options.AddPolicy("User", policy => policy.RequireRole("User"));
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