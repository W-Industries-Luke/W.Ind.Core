using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace W.Ind.Core.Entity;

/// <summary>
/// Base <see langword="interface"/> used to define a <see langword="bool"/> <see cref="IsDeleted"/> column on the implementing entity
/// </summary>
/// <remarks>
/// Handling Soft Deletes:
/// <example>
/// <code>
/// <see langword="protected"/> <see langword="override"/> <see langword="void"/> <see cref="DbContext"/>.SaveChanges() {
///     <see cref="ContextHelper"/>.<see cref="ContextHelper.HandleSoftDelete(IEnumerable{EntityEntry{ISoftDelete}})">HandleSoftDelete</see>(<see cref="ChangeTracker"/>.<see cref="ChangeTracker.Entries{TEntity}()">Entries</see>&lt;<see cref="ISoftDelete"/>&gt;());
///     
///     <see langword="return"/> <see langword="base"/>.SaveChanges();
/// }
/// </code>
/// </example>
/// </remarks>
public interface ISoftDelete
{
    /// <summary>
    /// Defines a <see langword="bool"/> <c>IsDeleted</c> flag on any implementing entity/table
    /// </summary>
    bool IsDeleted { get; set; }
}
