using EventTicketingManagementSystem.Models;
using Persistence.Repositories.Interfaces.Generic;

namespace EventTicketingManagementSystem.Data.Repository
{
    public interface IUserRepository : IGenericRepository<User, int>
    {
    }
}
