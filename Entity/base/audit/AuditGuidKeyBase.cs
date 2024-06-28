namespace W.Ind.Core.Entity;

/// <summary>
/// An <see langword="abstract"/> base class used to define auditable <see cref="Guid"/> keyed entities
/// </summary>
/// <remarks>
/// Use this non-generic type signature to define auditable entities where the <see langword="default"/> <see cref="GuidKeyUser"/> class is your User entity
/// </remarks>
public abstract class AuditGuidKeyBase : AuditGuidKeyBase<Guid, GuidKeyUser>;

/// <summary>
/// An <see langword="abstract"/> base class used to define auditable <see cref="Guid"/> keyed entities
/// </summary>
/// <remarks>
/// Use this generic type signature to define auditable entities where the audited <typeparamref name="TUser"/> derives from <see cref="GuidKeyUser"/>
/// </remarks>
/// <typeparam name="TUser">Any User entity class derived from <see cref="GuidKeyUser"/></typeparam>
public abstract class AuditGuidKeyBase<TUser> : AuditGuidKeyBase<Guid, TUser>, IAuditableGuidKey<TUser>, IGuidKey
    where TUser : UserBase<Guid>;

/// <summary>
/// An <see langword="abstract"/> base class used to define auditable <see cref="Guid"/> keyed entities
/// </summary>
/// <remarks>
/// Use this generic type signature to define auditable entities where <typeparamref name="TUser"/> has any PK <see langword="type"/> other than <see cref="Guid"/>
/// </remarks>
/// <typeparam name="TUserKey">The PK <see langword="type"/> of <typeparamref name="TUser"/></typeparam>
/// <typeparam name="TUser">Any User entity class derived from <see cref="UserBase{TKey}"/> where the PK <see langword="type"/> is <typeparamref name="TUserKey"/></typeparam>
public abstract class AuditGuidKeyBase<TUserKey, TUser> : AuditBase<TUserKey, TUser>, IAuditable<TUserKey, TUser>, IGuidKey
    where TUserKey : struct, IEquatable<TUserKey> where TUser : UserBase<TUserKey>
{
    public Guid Id { get; set; }
}
