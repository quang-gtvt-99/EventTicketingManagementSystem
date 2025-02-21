
using EventTicketingManagementSystem.Services;
using EventTicketingManagementSystem.Data;
using EventTicketingManagementSystem.Worker.BackgroundServices;
using System.Diagnostics.CodeAnalysis;

namespace EventTicketingManagementSystem.Worker
{
    [ExcludeFromCodeCoverage]
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddUserSecrets<Program>()
                     .AddEnvironmentVariables();

            var connectionString = builder.Configuration["ConnectionString"] ?? throw new Exception("Cannot get Connection String");
            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddHttpContextAccessor();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddServiceLayer();
            builder.Services.AddDataServices(connectionString);

            // Register UserBackgroundService
            builder.Services.AddHostedService<BookingTaskService>();
            builder.Services.AddHostedService<DailyTaskService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
