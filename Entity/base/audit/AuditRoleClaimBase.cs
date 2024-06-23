using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace W.Ind.Core.Entity;

public abstract class AuditRoleClaimBase 
    : AuditRoleClaimBase<User>, IAuditable, IEntity<int>;

public abstract class AuditRoleClaimBase<TUser> 
    : AuditRoleClaimBase<long, TUser>, IAuditable<TUser>, IEntity<int>
    where TUser : UserBase<long>;

public abstract class AuditRoleClaimBase<TRoleKey, TUser> 
    : AuditRoleClaimBase<TRoleKey, TUser, TRoleKey>, IAuditable<TRoleKey, TUser>, IEntity<int>
    where TRoleKey : struct, IEquatable<TRoleKey> where TUser : UserBase<TRoleKey>;

/// <summary>
/// An <see langword="abstract"/> <see langword="class"/> that both inherits from <see cref="IdentityRoleClaim{TKey}"/> and implements the <see cref="IAuditable"/> <see langword="interface"/>
/// </summary>
/// <remarks>Used to define repetitive boilerplate properties outside of the actual entity <see langword="class"/> file</remarks>
/// <typeparam name="TRoleKey">The data type of your <c>Role</c> entity's Primary Key</typeparam>
public abstract class AuditRoleClaimBase<TRoleKey, TUser, TUserKey> 
    : IdentityRoleClaim<TRoleKey>, IAuditable<TUserKey, TUser>, IEntity<int> 
    where TRoleKey : struct, IEquatable<TRoleKey> where TUserKey : struct, IEquatable<TUserKey> where TUser : UserBase<TUserKey>
{
    /// <summary>
    /// Derived from <see cref="IAuditable"/>
    /// </summary>
    /// <remarks>
    /// Defined with the [<see cref="TimestampAttribute"/>] so there's no need to configure for each inheritting entity
    /// </remarks>
    [Timestamp]
    public byte[] Timestamp { get; set; }

    /// <summary>
    /// Derived from <see cref="IAuditable"/>
    /// </summary>
    /// <remarks>
    /// Defined with the [<see cref="RequiredAttribute"/>] and [<see cref="ForeignKeyAttribute"/>] so there's no need to configure for each inheritting entity
    /// </remarks>
    [Required]
    [ForeignKey(nameof(CreatedBy))]
    public TUserKey CreatedById { get; set; }

    /// <summary>
    /// Implemented from <see cref="IAuditable"/>
    /// </summary>
    /// <remarks>
    /// Defined with the [<see cref="DeleteBehaviorAttribute"/>] so there's no need to configure for each entity AND because its Foreign Key is <see langword="required"/>
    /// </remarks>
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public TUser? CreatedBy { get; set; }

    /// <summary>
    /// Implemented from <see cref="IAuditable"/>
    /// </summary>
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public TUser? ModifiedBy { get; set; }

    /// <summary>
    /// Implemented from <see cref="IAuditable"/>
    /// </summary>
    public DateTime CreatedOn { get; set; }

    /// <summary>
    /// Implemented from <see cref="IAuditable"/>
    /// </summary>
    public DateTime? ModifiedOn { get; set; }

}
