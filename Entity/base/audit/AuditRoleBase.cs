using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace W.Ind.Core.Entity;

/// <summary>
/// An <see langword="abstract"/> <see langword="class"/> that both inherits from <see cref="RoleBase{TKey}"/> and implements the <see cref="IAuditable"/> <see langword="interface"/>
/// </summary>
/// <remarks>
/// <para>Used to define repetitive boilerplate properties outside of the actual entity <see langword="class"/> file</para>
/// <para>Inherits from <see cref="AuditEntityBase{TUser}"/> where <c>TUser</c> derives from <see cref="UserBase"/></para>
/// </remarks>
public abstract class AuditRoleBase 
    : AuditRoleBase<CoreUser>, IAuditable, IEntity;

/// <summary>
/// An <see langword="abstract"/> <see langword="class"/> that both inherits from <see cref="RoleBase{TKey}"/> and implements the <see cref="IAuditable"/> <see langword="interface"/>
/// </summary>
/// <remarks>
/// <para>Used to define repetitive boilerplate properties outside of the actual entity <see langword="class"/> file</para>
/// <para>Inherits from <see cref="AuditEntityBase{TKey, TUser}"/></para>
/// </remarks>
/// <typeparam name="TUser">Your User entity (PK <see langword="type"/> of <see cref="long"/>)</typeparam>
public abstract class AuditRoleBase<TUser> 
    : AuditRoleBase<long, TUser>, IAuditable<long, TUser>, IEntity
    where TUser : UserBase;

/// <summary>
/// An <see langword="abstract"/> <see langword="class"/> that both inherits from <see cref="RoleBase{TKey}"/> and implements the <see cref="IAuditable"/> <see langword="interface"/>
/// </summary>
/// <remarks>
/// <para>Used to define repetitive boilerplate properties outside of the actual entity <see langword="class"/> file</para>
/// <para>Inherits from <see cref="AuditEntityBase{TKey, TUser, TUserKey}"/></para>
/// </remarks>
/// <typeparam name="TKey">The data type of its Primary Key</typeparam>
/// <typeparam name="TUser">Your User entity (PK <see langword="type"/> of <typeparamref name="TKey"/></typeparam>
public abstract class AuditRoleBase<TKey, TUser> 
    : AuditRoleBase<TKey, TUser, TKey>, IAuditable<TKey, TUser>, IEntity<TKey>
    where TKey : struct, IEquatable<TKey> where TUser : UserBase<TKey>;


/// <summary>
/// An <see langword="abstract"/> <see langword="class"/> that both inherits from <see cref="RoleBase{TKey}"/> and implements the <see cref="IAuditable"/> <see langword="interface"/>
/// </summary>
/// <remarks>
/// <para>Used to define repetitive boilerplate properties outside of the actual entity <see langword="class"/> file</para>
/// </remarks>
/// <typeparam name="TKey">The data type of its Primary Key</typeparam>
/// <typeparam name="TUser">Your User entity (PK <see langword="type"/> of <typeparamref name="TKey"/></typeparam>
/// <typeparam name="TUserKey">The PK <see langword="type"/> of your User entity</typeparam>
public abstract class AuditRoleBase<TKey, TUser, TUserKey> 
    : RoleBase<TKey>, IAuditable<TUserKey, TUser>, IEntity<TKey>
    where TKey : struct, IEquatable<TKey> where TUserKey : struct, IEquatable<TUserKey> where TUser : UserBase<TUserKey>
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
