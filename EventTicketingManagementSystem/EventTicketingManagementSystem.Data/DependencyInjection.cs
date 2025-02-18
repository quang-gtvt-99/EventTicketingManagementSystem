using EventTicketingManagementSystem.Data.Data;
using EventTicketingManagementSystem.Data.Data.Repository.Implement;
using EventTicketingManagementSystem.Data.Data.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EventTicketingManagementSystem.Data
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDataServices(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));

            // Add repository.
            services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<ITicketRepository, TicketRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();

            return services;
        }
    }
}
