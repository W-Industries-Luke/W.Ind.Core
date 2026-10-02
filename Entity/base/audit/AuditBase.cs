using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace W.Ind.Core.Entity;

/// <summary>
/// An <see langword="abstract"/> base <see langword="class"/> for defining <see cref="IAuditable"/> properties
/// </summary>
/// <remarks>
/// <para>Inherits from <see cref="AuditBase{TUser}"/></para>
/// <para>User will be defaulted to <see cref="CoreUser"/> with a PK type of <see cref="long"/></para>
/// </remarks>
public abstract class AuditBase 
    : AuditBase<CoreUser>, IAuditable;


/// <summary>
/// An <see langword="abstract"/> base <see langword="class"/> for defining <see cref="IAuditable{TUser}"/> properties
/// </summary>
/// <remarks>
/// <para>Inherits from <see cref="AuditBase{TUserKey, TUser}"/></para>
/// <para>User Key will be defaulted to <see cref="long"/></para>
/// </remarks>
/// <typeparam name="TUser">Your User entity (derives from <see cref="UserBase"/> with a PK type <see cref="long"/>)</typeparam>
public abstract class AuditBase<TUser> 
    : AuditBase<long, TUser>, IAuditable<TUser> 
    where TUser : UserBase;

/// <summary>
/// An <see langword="abstract"/> base <see langword="class"/> for defining <see cref="IAuditable{TKey, TUser}"/> properties
/// </summary>
/// <remarks>
/// <para></para>
/// </remarks>
/// <typeparam name="TUserKey"></typeparam>
/// <typeparam name="TUser"></typeparam>
public abstract class AuditBase<TUserKey, TUser> 
    : IAuditable<TUserKey, TUser> 
    where TUserKey : struct, IEquatable<TUserKey> where TUser : UserBase<TUserKey>
{
    /// Derived from <see cref="IAuditable"/>
    /// </summary>
    /// <remarks>
    /// Defined with the [<see cref="TimestampAttribute"/>] so there's no need to configure for each inheritting entity
    /// </remarks>
    public byte[] Timestamp { get; set; }

    /// <summary>
    /// Derived from <see cref="IAuditable{TUserKey, TUser}"/>
    /// </summary>
    /// <remarks>
    /// The FK of the User who created this record
    /// </remarks>
    public TUserKey CreatedById { get; set; }

    /// <summary>
    /// Implemented from <see cref="IAuditable{TUserKey, TUser}"/>
    /// </summary>
    /// <remarks>
    /// The navigation property pointing to the User who created this record
    /// </remarks>
    public TUser? CreatedBy { get; set; }

    /// <summary>
    /// Implemented from <see cref="IAuditable{TUserKey, TUser}"/>
    /// </summary>
    /// <remarks>
    /// <para>
    /// The navigation property pointing to the User who last modified this record
    /// </para>
    /// <para>
    /// Nullable ModifiedById FK property will be defined via audit configuration
    /// </para>
    /// </remarks>
    public TUser? ModifiedBy { get; set; }

    /// <summary>
    /// Implemented from <see cref="IAuditable{TUserKey, TUser}"/>
    /// </summary>
    /// <remarks>
    /// The <see cref="DateTime"/> value representing when this record was created
    /// </remarks>
    public DateTime CreatedOn { get; set; }

    /// <summary>
    /// Implemented from <see cref="IAuditable{TUserKey, TUser}"/>
    /// </summary>
    /// <remarks>
    /// The <see cref="DateTime"/> value representing when this record was last modified
    /// </remarks>
    public DateTime? ModifiedOn { get; set; }
}