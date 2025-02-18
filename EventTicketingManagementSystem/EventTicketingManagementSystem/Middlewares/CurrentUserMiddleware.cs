using EventTicketingManagementSystem.Services.Services.Interfaces;
using System.Security.Claims;

namespace EventTicketingManagementSystem.API.Middlewares
{
    public class CurrentUserMiddleware
    {
        private readonly RequestDelegate _next;

        public CurrentUserMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ICurrentUserService currentUserService)
        {
            if (context.User.Identity is ClaimsIdentity identity)
            {
                var emailClaim = identity.FindFirst(ClaimTypes.Email);
                if (emailClaim != null)
                {
                    currentUserService.Email = emailClaim.Value;
                }

                var idClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
                if (idClaim != null)
                {
                    currentUserService.Id = idClaim.Value;
                }

                var fullNameClaim = identity.FindFirst(ClaimTypes.Name);
                if (fullNameClaim != null)
                {
                    currentUserService.FullName = fullNameClaim.Value;
                }
            }

            await _next(context);
        }
    }

}
