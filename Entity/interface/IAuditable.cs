using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

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
public interface IAuditable
{
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

    /// <summary>
    /// The UserId (FK) of the User that created this record
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
    long CreatedById { get; set; }

    /// <summary>
    /// The navigation property pointing to the User who created this record
    /// </summary>
    /// <remarks>
    /// <para>
    /// Nullable so you aren't required to configure <c>.Navigation().AutoInclude()</c> or <c>.DeleteBehavior(<see cref="DeleteBehavior.NoAction"/>)</c>
    /// </para>
    /// <para>
    /// If you want it non-nullable, you must at least specify the <see cref="DeleteBehavior"/> before migration
    /// </para>
    /// </remarks>
    User? CreatedBy { get; set; }

    /// <summary>
    /// The <see cref="DateTime"/> value representing when this entity was initially saved
    /// </summary>
    DateTime CreatedOn { get; set; }

    /// <summary>
    /// The UserId (FK) of the User that last updated this record
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
    long? ModifiedById { get; set; }

    /// <summary>
    /// The navigation property pointing to the User who last updated this record
    /// </summary>
    /// <remarks>
    /// <para>
    /// Nullable so you aren't required to configure <c>.Navigation().AutoInclude()</c> or <c>.DeleteBehavior(<see cref="DeleteBehavior.NoAction"/>)</c>
    /// </para>
    /// <para>
    /// If you want it non-nullable, you must at least specify the <see cref="DeleteBehavior"/> before migration
    /// </para>
    /// </remarks>
    User? ModifiedBy { get; set; }

    /// <summary>
    /// The <see cref="DateTime"/> value representing when this entity was last updated
    /// </summary>
    DateTime? ModifiedOn { get; set; }
}
