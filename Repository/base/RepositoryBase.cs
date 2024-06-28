using System.Linq.Expressions;

namespace W.Ind.Core.Repository;

/// <summary>
/// An <see langword="abstract"/> base class for generically implementing repositories through an entity
/// </summary>
/// <remarks>
/// Use this <see langword="class"/> signature when the PK <see langword="type"/> is <see cref="long"/>
/// </remarks>
/// <typeparam name="TEntity">The entity <see langword="type"/> this repository references</typeparam>
public abstract class RepositoryBase<TEntity> : RepositoryBase<TEntity, long>, IRepository<TEntity>
    where TEntity : class, IEntity
{
    public RepositoryBase(DbContext context) : base(context) { }
}

/// <summary>
/// An <see langword="abstract"/> base class for generically implementing repositories through an entity
/// </summary>
/// <remarks>
/// Use this <see langword="class"/> signature when the PK is any <see langword="type"/> other than <see cref="long"/>
/// </remarks>
/// <typeparam name="TEntity">The entity <see langword="type"/> this repository references</typeparam>
/// <typeparam name="TKey">
/// <para>
/// The specific PK <see langword="type"/> of <typeparamref name="TEntity"/>
/// </para>
/// <para>
/// Defaults to <see cref="long"/> when omitted
/// </para>
/// </typeparam>
public abstract class RepositoryBase<TEntity, TKey> : IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey> where TKey : struct, IEquatable<TKey>
{
    /// <summary>
    /// A reference to the specified <see cref="DbContext"/> <see langword="class"/>
    /// </summary>
    protected readonly DbContext _context;

    /// <summary>
    /// A reference to the <see cref="DbSet{TEntity}"/> that references this entity
    /// </summary>
    protected readonly DbSet<TEntity> _dbSet;

    /// <summary>
    /// Base Constructor
    /// </summary>
    /// <remarks>
    /// Pass whichever context class you want to reference via injection
    /// </remarks>
    /// <param name="context"></param>
    public RepositoryBase(DbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    /// <summary>
    /// Gets a queryable set of <typeparamref name="TEntity"/> instances specified by <paramref name="predicate"/>
    /// </summary>
    /// <param name="predicate">
    /// <para>
    /// </para>
    /// Treat like the predicate in the <c>.Where(predicate)</c> method
    /// <para>
    /// The result will not be enumerated
    /// </para>
    /// </param>
    /// <returns><see cref="IQueryable{TEntity}"/> containing a set of <typeparamref name="TEntity"/> instances matching the <paramref name="predicate"/></returns>
    public virtual IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> predicate) 
    {
        return _dbSet.Where(predicate);
    }

    /// <summary>
    /// An <see langword="async"/> method that gets a queryable set of <typeparamref name="TEntity"/> instances specified by <paramref name="predicate"/>
    /// </summary>
    /// <param name="predicate">
    /// <para>
    /// </para>
    /// Treat like the predicate in the <c>.Where(predicate)</c> method
    /// <para>
    /// The result will not be enumerated
    /// </para>
    /// </param>
    /// <returns><see cref="IQueryable{TEntity}"/> containing a set of <typeparamref name="TEntity"/> instances matching the <paramref name="predicate"/></returns>
    public virtual async Task<IQueryable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await Task.FromResult(Get(predicate));
    }

    /// <summary>
    /// Gets a single <typeparamref name="TEntity"/> instance with a PK property value matching the param <paramref name="id"/>
    /// </summary>
    /// <param name="id">The PK value to check against</param>
    /// <returns>The matching <typeparamref name="TEntity"/> instance</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <see cref="_dbSet"/> is <see langword="null"/></exception>
    public virtual TEntity GetById(TKey id)
    {
        return _dbSet.First(entity => entity.Id.Equals(id));
    }

    /// <summary>
    /// An <see langword="async"/> that gets a single <typeparamref name="TEntity"/> instance with a PK property value matching the param <paramref name="id"/>
    /// </summary>
    /// <param name="id">The PK value to check against</param>
    /// <returns>The matching <typeparamref name="TEntity"/> instance</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <see cref="_dbSet"/> is <see langword="null"/></exception>
    public virtual async Task<TEntity> GetByIdAsync(TKey id) 
    {
        return await _dbSet.FirstAsync(entity => entity.Id.Equals(id));
    }

    /// <summary>
    /// Saves a <see langword="new"/> <typeparamref name="TEntity"/> record to the database
    /// </summary>
    /// <param name="entity">The <typeparamref name="TEntity"/> instance to save</param>
    /// <returns>The resulting <typeparamref name="TEntity"/> instance with PK and/or Audit values updated</returns>
    public virtual TEntity Create(TEntity entity, bool saveChanges = false)
    {
        _dbSet.Add(entity);
        ShouldSaveChanges(saveChanges);
        return entity;
    }

    /// <summary>
    /// An <see langword="async"/> method that saves a <see langword="new"/> <typeparamref name="TEntity"/> record to the database
    /// </summary>
    /// <param name="entity">The <typeparamref name="TEntity"/> instance to save</param>
    /// <returns>The resulting <typeparamref name="TEntity"/> instance with the PK property updated</returns>
    public virtual async Task<TEntity> CreateAsync(TEntity entity, bool saveChanges = false)
    {
        await _dbSet.AddAsync(entity);
        await ShouldSaveChangesAsync(saveChanges);
        return entity;
    }

    /// <summary>
    /// Updates and saves a <typeparamref name="TEntity"/> record
    /// </summary>
    /// <param name="entity">The <typeparamref name="TEntity"/> instance to update</param>
    /// <returns>The resulting <typeparamref name="TEntity"/> instance</returns>
    public virtual TEntity Update(TEntity entity, bool saveChanges = false)
    {
        _dbSet.Update(entity);
        ShouldSaveChanges(saveChanges);
        return entity;
    }

    /// <summary>
    /// An <see langword="async"/> method that updates and saves a <typeparamref name="TEntity"/> record
    /// </summary>
    /// <param name="entity">The <typeparamref name="TEntity"/> instance to update</param>
    /// <returns>The resulting <typeparamref name="TEntity"/> instance</returns>
    public virtual async Task<TEntity> UpdateAsync(TEntity entity, bool saveChanges = false)
    {
        _dbSet.Update(entity);
        await ShouldSaveChangesAsync(saveChanges);
        return entity;
    }

    /// <summary>
    /// Deletes a <typeparamref name="TEntity"/> record from the database with the specified PK value (<paramref name="id"/>)
    /// </summary>
    /// <param name="id">The PK value of the record to delete</param>
    public virtual void Delete(TKey id, bool saveChanges = false)
    {
        TEntity entity = GetById(id);
        _dbSet.Remove(entity);
        ShouldSaveChanges(saveChanges);
    }

    /// <summary>
    /// An <see langword="async"/> method that deletes a <typeparamref name="TEntity"/> record from the database with the specified PK value (<paramref name="id"/>)
    /// </summary>
    /// <param name="id">The PK value of the record to delete</param>
    public virtual async Task DeleteAsync(TKey id, bool saveChanges = false) 
    {
        TEntity entity = await GetByIdAsync(id);
        _dbSet.Remove(entity);
        await ShouldSaveChangesAsync(saveChanges);
    }

    protected virtual void ShouldSaveChanges(bool saveChanges)
    {
        if (saveChanges)
        {
            _context.SaveChanges();
        }
    }

    protected virtual async Task ShouldSaveChangesAsync(bool saveChanges)
    {
        if (saveChanges)
        {
            await _context.SaveChangesAsync();
        }
    }
}
