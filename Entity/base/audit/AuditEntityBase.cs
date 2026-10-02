using System.ComponentModel.DataAnnotations;

namespace W.Ind.Core.Entity;

/// <summary>
/// An <see langword="abstract"/> <see langword="class"/> that both inherits from <see cref="EntityBase{TKey}"/> and implements the <see cref="IAuditable{TKey, TUser}"/> <see langword="interface"/>
/// </summary>
/// <remarks>
/// <para>Used to define repetitive boilerplate properties outside of the actual entity <see langword="class"/> file</para>
/// <para>Inherits from <see cref="AuditEntityBase{TUser}"/> where <c>TUser</c> is defaulted to <see cref="CoreUser"/></para>
/// </remarks>
public abstract class AuditEntityBase 
    : AuditEntityBase<CoreUser>, IAuditable, IEntity;

/// <summary>
/// An <see langword="abstract"/> <see langword="class"/> that both inherits from <see cref="EntityBase{TKey}"/> and implements the <see cref="IAuditable{TKey, TUser}"/> <see langword="interface"/>
/// </summary>
/// <remarks>
/// <para>Used to define repetitive boilerplate properties outside of the actual entity <see langword="class"/> file</para>
/// <para>Inherits from <see cref="AuditEntityBase{TKey, TUser}"/> where <c>TKey</c> is defaulted to <see cref="long"/></para>
/// </remarks>
/// <typeparam name="TUser">The User entity (PK <see langword="type"/> is the same as <typeparamref name="TKey"/>)</typeparam>
public abstract class AuditEntityBase<TUser> 
    : AuditEntityBase<long, TUser>, IAuditable<TUser>, IEntity 
    where TUser : UserBase;

/// <summary>
/// An <see langword="abstract"/> <see langword="class"/> that both inherits from <see cref="EntityBase{TKey}"/> and implements the <see cref="IAuditable{TKey, TUser}"/> <see langword="interface"/>
/// </summary>
/// <remarks>
/// <para>Used to define repetitive boilerplate properties outside of the actual entity <see langword="class"/> file</para>
/// <para>Inherits from <see cref="AuditEntityBase{TKey, TUser, TUserKey}"/></para>
/// </remarks>
/// <typeparam name="TKey">The entity's PK <see langword="type"/></typeparam>
/// <typeparam name="TUser">The User entity (PK <see langword="type"/> is the same as <typeparamref name="TKey"/>)</typeparam>
public abstract class AuditEntityBase<TKey, TUser> 
    : AuditEntityBase<TKey, TUser, TKey>, IAuditable<TKey, TUser>, IEntity<TKey> 
    where TKey : struct, IEquatable<TKey> where TUser : UserBase<TKey>;

/// <summary>
/// An <see langword="abstract"/> <see langword="class"/> that both inherits from <see cref="EntityBase{TKey}"/> and implements the <see cref="IAuditable{TKey, TUser}"/> <see langword="interface"/>
/// </summary>
/// <remarks>
/// <para>Used to define repetitive boilerplate properties outside of the actual entity <see langword="class"/> file</para>
/// </remarks>
/// <typeparam name="TKey">The PK <see langword="type"/> of the entity</typeparam>
/// <typeparam name="TUser">Your User entity (PK <see langword="type"/> of <typeparamref name="TUserKey"/></typeparam>
/// <typeparam name="TUserKey">The PK <see langword="type"/> of your User entity</typeparam>
public abstract class AuditEntityBase<TKey, TUser, TUserKey> 
    : AuditBase<TUserKey, TUser>, IAuditable<TUserKey, TUser>, IEntity<TKey> 
    where TKey : struct, IEquatable<TKey> where TUserKey : struct, IEquatable<TUserKey> where TUser : UserBase<TUserKey> 
{
    [Key]
    public TKey Id { get; set; }
}