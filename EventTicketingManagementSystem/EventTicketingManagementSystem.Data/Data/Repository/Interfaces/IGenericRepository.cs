using EventTicketingMananagementSystem.Core.Models.BaseModels;
using System.Linq.Expressions;

namespace EventTicketingManagementSystem.Data.Data.Repository.Interfaces
{
    public interface IGenericRepository<TEntity, in TKey> where TEntity : EntityBase<TKey> where TKey : struct
    {
        Task<TEntity?> GetByIdAsync(TKey id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> AddAsync(TEntity entity);
        void Update(TEntity entity);
        void UpdateRange(IEnumerable<TEntity> entities);
        void Delete(TEntity entity);
        void DeleteRange(IEnumerable<TEntity> entities);
        Task<int> SaveChangeAsync();
    }
}
