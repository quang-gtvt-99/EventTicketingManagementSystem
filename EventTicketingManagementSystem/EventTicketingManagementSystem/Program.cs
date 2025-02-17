using EventTicketingManagementSystem.Data;
using EventTicketingManagementSystem.Data.Repository;
using EventTicketingManagementSystem.Data.Repository.Implement;
using EventTicketingManagementSystem.Data.Repository.Interfaces;
using EventTicketingManagementSystem.Middlewares;
using EventTicketingManagementSystem.Services.Implements;
using EventTicketingManagementSystem.Services.Interfaces;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Load User Secrets and Environment Variables
builder.Configuration.AddUserSecrets<Program>()
                     .AddEnvironmentVariables();

var connectionString = builder.Configuration["ConnectionString"];
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Add repository.
builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();

// Add services.
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Add External Services
builder.Services.AddSingleton<IObjectStorageService, ObjectStorageService>();
builder.Services.AddSingleton<ICacheService, CacheService>();
builder.Services.AddSingleton<ISendMailService, SendMailService>();

builder.Services.AddControllers();

// Add FluentValidation
builder.Services
    .AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters();

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()  // Allow any origin
              .AllowAnyMethod()  // Allow any HTTP method (GET, POST, etc.)
              .AllowAnyHeader(); // Allow any headers
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
var key = builder.Configuration["JwtKey"];
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

builder.Services.AddScoped<IJwtAuth>(provider =>
{
    var userRepository = provider.GetRequiredService<IUserRepository>();
    var cacheService = provider.GetRequiredService<ICacheService>();
    return new JwtAuth(key, userRepository, cacheService);
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

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseMiddleware<RequestTimingMiddleware>();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthorization();

app.UseMiddleware<CurrentUserMiddleware>();

app.MapControllers();

app.Run();
