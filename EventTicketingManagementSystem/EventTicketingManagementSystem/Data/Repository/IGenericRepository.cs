using EventTicketingManagementSystem.Models.BaseModels;
using System.Linq.Expressions;

namespace Persistence.Repositories.Interfaces.Generic
{
    public interface IGenericRepository<TEntity, in TKey> where TEntity : EntityBase<TKey> where TKey : struct
    {
        Task<TEntity?> GetByIdAsync(TKey id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> AddAsync(TEntity entity);
        Task<bool> UpdateAsync(TEntity entity);
        Task<bool> DeleteAsync(TEntity entity);
    }
}
