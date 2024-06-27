using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace W.Ind.Core.Entity;

public abstract class AuditUserRoleBase 
    : AuditUserRoleBase<CoreUser>, IAuditable;

public abstract class AuditUserRoleBase<TUser> 
    : AuditUserRoleBase<long, TUser>, IAuditable<TUser>
    where TUser : UserBase;

/// <summary>
/// An <see langword="abstract"/> <see langword="class"/> that both inherits from <see cref="IdentityUserRole{TKey}"/> and implements the <see cref="IAuditable"/> <see langword="interface"/>
/// </summary>
/// <remarks>
/// <para>
/// Used to define repetitive boilerplate properties outside of the actual entity <see langword="class"/> file
/// </para>
/// <para>
/// In order to implement this, the PKs of your User and Role entities must be the same data type
/// </para>
/// </remarks>
/// <typeparam name="TKey">The data type of your <c>User</c> and <c>Role</c> enties' Primary Key</typeparam>
public abstract class AuditUserRoleBase<TKey, TUser> 
    : IdentityUserRole<TKey>, IAuditable<TKey, TUser>
    where TKey : struct, IEquatable<TKey> where TUser : UserBase<TKey>
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
    public TKey CreatedById { get; set; }

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
