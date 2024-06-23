namespace W.Ind.Core.Entity;

/// <summary>
/// Concrete core entity <see langword="class"/> that can be used as an <see cref="Microsoft.AspNetCore.Identity.IdentityRole{TKey}"/>
/// </summary>
/// <remarks>
/// <para>
/// Inherits from <see langword="abstract"/> <see cref="AuditRoleBase{TKey}"/> which implements <see cref="IAuditable"/> and <see cref="IEntity{TKey}"/>
/// </para>
/// <para>
/// Implements <see cref="ISoftDelete"/>, which is the only property defined here
/// </para>
/// </remarks>
public class Role 
    : Role<User>, ISoftDelete, IAuditable, IEntity;

public class Role<TUser> 
    : Role<long, TUser>, ISoftDelete, IAuditable<TUser>, IEntity 
    where TUser : UserBase<long>;

public class Role<TKey, TUser> 
    : Role<TKey, TUser, TKey>, ISoftDelete, IAuditable<TKey, TUser>, IEntity<TKey> 
    where TKey : struct, IEquatable<TKey> where TUser : UserBase<TKey>;

public class Role<TKey, TUser, TUserKey> 
    : AuditRoleBase<TKey, TUser, TUserKey>, ISoftDelete, IAuditable<TUserKey, TUser> , IEntity<TKey> 
    where TKey : struct, IEquatable<TKey> where TUserKey : struct, IEquatable<TUserKey> where TUser : UserBase<TUserKey>
{
    /// <summary>
    /// <para>Implemented from <see cref="ISoftDelete"/></para>
    /// <para>Adds an <c>IsDeleted</c> flag to the <c>Role</c> table</para>
    /// </summary>
    /// <remarks>
    /// See <see cref="ContextHelper.HandleSoftDelete(IEnumerable{Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry{ISoftDelete}})"/> for usage
    /// </remarks>
    public bool IsDeleted { get; set; }
}