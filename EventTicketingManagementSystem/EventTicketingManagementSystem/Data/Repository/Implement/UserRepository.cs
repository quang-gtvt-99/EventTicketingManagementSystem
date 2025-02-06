using EventTicketingManagementSystem.Models;

namespace EventTicketingManagementSystem.Data.Repository.Implement
{
    public class UserRepository : GenericRepository<User, int>, IUserRepository 
    {
        public UserRepository(AppDbContext context) : base(context)
        {
        }
    }
}
