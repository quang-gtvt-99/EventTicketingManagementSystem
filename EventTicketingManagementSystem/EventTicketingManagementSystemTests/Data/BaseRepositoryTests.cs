using EventTicketingManagementSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace EventTicketingManagementSystemTests.Data
{
    public abstract class BaseRepositoryTests : IDisposable
    {
        protected readonly AppDbContext Context;

        protected BaseRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            Context = new AppDbContext(options);
            Context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            Context.Database.EnsureDeleted();
            Context.Dispose();
        }
    }
}
