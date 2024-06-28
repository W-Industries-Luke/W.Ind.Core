using System.Linq.Expressions;

namespace W.Ind.Core.Repository;

public interface IRepository<TEntity> : IRepository<TEntity, long>
    where TEntity : class, IEntity;

public interface IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey> where TKey : struct, IEquatable<TKey>
{
    IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> predicate);
    TEntity GetById(TKey id);
    TEntity Create(TEntity entity, bool saveChanges);
    TEntity Update(TEntity entity, bool saveChanges);
    void Delete(TKey id, bool saveChanges);
    Task<IQueryable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate);
    Task<TEntity> GetByIdAsync(TKey id);
    Task<TEntity> CreateAsync(TEntity entity, bool saveChanges);
    Task<TEntity> UpdateAsync(TEntity entity, bool saveChanges);
    Task DeleteAsync(TKey id, bool saveChanges);
}
