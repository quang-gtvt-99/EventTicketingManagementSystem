using EventTicketingManagementSystem.Services.Services.Implements;
using EventTicketingManagementSystem.Services.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace EventTicketingManagementSystem.Services
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServiceLayer(this IServiceCollection services)
        {
            // Add services.
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IVNPayService, VnPayService>();
            services.AddScoped<IJwtAuth, JwtAuth>();
            services.AddScoped<IBookingService, BookingService>();

            // Add External Services
            services.AddSingleton<IObjectStorageService, ObjectStorageService>();
            services.AddSingleton<ICacheService, CacheService>();
            services.AddSingleton<ISendMailService, SendMailService>();

            return services;
        }
    }
}
