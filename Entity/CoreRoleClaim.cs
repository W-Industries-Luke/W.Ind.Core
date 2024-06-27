namespace W.Ind.Core.Entity;

/// <summary>
/// Concrete core entity <see langword="class"/> that can be used as an <see cref="Microsoft.AspNetCore.Identity.IdentityRoleClaim{TKey}"/>
/// </summary>
/// <remarks>
/// <para>
/// Inherits from <see langword="abstract"/> <see cref="AuditRoleClaimBase{TRoleKey}"/> which implements <see cref="IAuditable"/>
/// </para>
/// <para>
/// Implements <see cref="ISoftDelete"/>, which is the only property defined here
/// </para>
/// </remarks>
public class CoreRoleClaim : CoreRoleClaim<CoreUser>, ISoftDelete, IAuditable;

public class CoreRoleClaim<TUser>: CoreRoleClaim<long, TUser>, ISoftDelete, IAuditable<long, TUser> where TUser : UserBase;

public class CoreRoleClaim<TKey, TUser> 
    : CoreRoleClaim<TKey, TUser, TKey>, ISoftDelete, IAuditable<TKey, TUser>
    where TKey : struct, IEquatable<TKey> where TUser : UserBase<TKey>;

public class CoreRoleClaim<TKey, TUser, TUserKey> 
    : AuditRoleClaimBase<TKey, TUser, TUserKey>, ISoftDelete, IAuditable<TUserKey, TUser> 
    where TKey : struct, IEquatable<TKey> where TUserKey : struct, IEquatable<TUserKey> where TUser : UserBase<TUserKey>
{
    /// <summary>
    /// <para>
    /// Implemented from <see cref="ISoftDelete"/>
    /// </para>
    /// Adds an <c>IsDeleted</c> flag to the <c>CoreRoleClaim</c> table
    /// </summary>
    /// <remarks>
    /// See <see cref="ContextHelper.HandleSoftDelete(IEnumerable{Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry{ISoftDelete}})"/> for usage
    /// </remarks>
    public bool IsDeleted { get; set; }
}
