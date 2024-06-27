using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace W.Ind.Core.Entity;

public abstract class AuditUserClaimBase 
    : AuditUserClaimBase<CoreUser>, IAuditable, IEntity<int>;

public abstract class AuditUserClaimBase<TUser> 
    : AuditUserClaimBase<long, TUser>, IAuditable<long, TUser>, IEntity<int>
    where TUser : UserBase;

/// <summary>
/// An <see langword="abstract"/> <see langword="class"/> that both inherits from <see cref="IdentityUserClaim{TKey}"/> and implements the <see cref="IAuditable"/> <see langword="interface"/>
/// </summary>
/// <remarks>Used to define repetitive boilerplate properties outside of the actual entity <see langword="class"/> file</remarks>
/// <typeparam name="TUserKey">The data type of your <c>User</c> entity's Primary Key</typeparam>
public abstract class AuditUserClaimBase<TUserKey, TUser> 
    : IdentityUserClaim<TUserKey>, IAuditable<TUserKey, TUser>, IEntity<int> 
    where TUserKey : struct, IEquatable<TUserKey> where TUser : UserBase<TUserKey>
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
