﻿using System.ComponentModel.DataAnnotations;

namespace W.Ind.Core.Entity;

public abstract class AuditEntityBase 
    : AuditEntityBase<User>, IAuditable, IEntity;

public abstract class AuditEntityBase<TUser> 
    : AuditEntityBase<long, TUser>, IAuditable<TUser>, IEntity 
    where TUser : UserBase<long>;

/// <summary>
/// An <see langword="abstract"/> <see langword="class"/> that both inherits from <see cref="EntityBase{TKey}"/> and implements the <see cref="IAuditable"/> <see langword="interface"/>
/// </summary>
/// <remarks>Used to define repetitive boilerplate properties outside of the actual entity <see langword="class"/> file</remarks>
/// <typeparam name="TKey">The data type of its Primary Key</typeparam>
public abstract class AuditEntityBase<TKey, TUser> 
    : AuditEntityBase<TKey, TUser, TKey>, IAuditable<TKey, TUser>, IEntity<TKey> 
    where TKey : IEquatable<TKey> where TUser : UserBase<TKey>;

public abstract class AuditEntityBase<TKey, TUser, TUserKey> 
    : AuditBase<TUserKey, TUser>, IAuditable<TUserKey, TUser>, IEntity<TKey> 
    where TKey : IEquatable<TKey> where TUserKey : IEquatable<TUserKey> where TUser : UserBase<TUserKey> 
{
    [Key]
    public TKey Id { get; set; }
}