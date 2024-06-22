using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace W.Ind.Core.Entity;

/// <summary>
/// A base <see langword="interface"/> that defines table columns required for audit logging
/// </summary>
/// <remarks>
/// <para>
/// Entities that implement this <see langword="interface"/> will automatically be visible to the
/// <see cref="ContextHelper.HandleAudit(IEnumerable{Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry{IAuditable}}, long)"/> method on save
/// </para>
/// <para>
/// Derive from this <see langword="interface"/> for further customization
/// </para>
/// </remarks>
public interface IAuditable : IAuditable<User>;

public interface IAuditable<TUser> : IAuditable<long, TUser> where TUser : UserBase<long> { }


/// <summary>
/// A base <see langword="interface"/> that defines table columns required for audit logging
/// </summary>
/// <remarks>
/// <para>
/// Entities that implement this <see langword="interface"/> will automatically be visible to the
/// <see cref="ContextHelper.HandleAudit(IEnumerable{Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry{IAuditable}}, long)"/> method on save
/// </para>
/// <para>
/// Derive from this <see langword="interface"/> for further customization
/// </para>
/// </remarks>
/// <typeparam name="TUser">User entity <see langword="type"/> (derives from <see cref="UserBase{TKey}"/>)</typeparam>
/// <typeparam name="TKey">The Primary Key <see langword="type"/> of <typeparamref name="TUser"/></typeparam>
public interface IAuditable<TKey, TUser> where TKey : IEquatable<TKey> where TUser : UserBase<TKey>
{
    /// <summary>
    /// The <see cref="DateTime"/> value representing when this entity was initially saved
    /// </summary>
    DateTime CreatedOn { get; set; }

    /// <summary>
    /// The <see cref="DateTime"/> value representing when this entity was last updated
    /// </summary>
    DateTime? ModifiedOn { get; set; }

    /// <summary>
    /// The UserId (FK) of the <typeparamref name="TUser"/> that created this record
    /// </summary>
    /// <remarks>
    /// <para>Any <see langword="class"/> implementing this should declare this a Foriegn Key column type via:</para>
    /// <para>
    /// (Preferred) Using the [<see cref="ForeignKeyAttribute"/>] on an abstract class that implements this interface, or
    /// </para>
    /// <para>
    /// Within your <see cref="DbContext.OnModelCreating(ModelBuilder)"/> method for each entity individually
    /// </para>
    /// </remarks>
    TKey CreatedById { get; set; }

    /// <summary>
    /// The navigation property pointing to the <typeparamref name="TUser"/> who created this record
    /// </summary>
    /// <remarks>
    /// <para>
    /// Nullable so you aren't required to configure <c>.Navigation().AutoInclude()</c> or <c>.DeleteBehavior(<see cref="DeleteBehavior.NoAction"/>)</c>
    /// </para>
    /// <para>
    /// If you want it non-nullable, you must at least specify the <see cref="DeleteBehavior"/> before migration
    /// </para>
    /// </remarks>
    TUser? CreatedBy { get; set; }

    TKey? ModifiedById { get; set; }

    /// <summary>
    /// The navigation property pointing to the <typeparamref name="TUser"/> who last updated this record
    /// </summary>
    /// <remarks>
    /// <para>
    /// Nullable so you aren't required to configure <c>.Navigation().AutoInclude()</c> or <c>.DeleteBehavior(<see cref="DeleteBehavior.NoAction"/>)</c>
    /// </para>
    /// <para>
    /// If you want it non-nullable, you must at least specify the <see cref="DeleteBehavior"/> before migration
    /// </para>
    /// </remarks>
    TUser? ModifiedBy { get; set; }

    /// <summary>
    /// Audit Log Timestamp
    /// </summary>
    /// <remarks>
    /// <para>
    /// Any <see langword="class"/> implementing this should declare this a Timestamp column type via:
    /// </para>
    /// <para>
    /// (Preferred) Using the [<see cref="TimestampAttribute"/>] on an <see langword="abstract"/> <see langword="class"/> that implements this <see langword="interface"/>, or
    /// </para>
    /// <para>
    /// Within your <see cref="DbContext.OnModelCreating(ModelBuilder)"/> method for each entity individually
    /// </para>
    /// </remarks>
    byte[] Timestamp { get; set; }
}
