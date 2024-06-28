namespace W.Ind.Core.Entity;

/// <summary>
/// Concrete core entity <see langword="class"/> that can be used as an <see cref="Microsoft.AspNetCore.Identity.IdentityUserClaim{TKey}"/>
/// </summary>
/// <remarks>
/// <para>
/// Inherits from <see langword="abstract"/> <see cref="AuditUserClaimBase{TRoleKey}"/> which implements <see cref="IAuditable"/>
/// </para>
/// <para>
/// Implements <see cref="ISoftDelete"/>, which is the only property defined here
/// </para>
/// </remarks>
public class CoreUserClaim : CoreUserClaim<CoreUser>, ISoftDelete, IAuditable;

public class CoreUserClaim<TUser> : CoreUserClaim<long, TUser> where TUser : UserBase;

public class CoreUserClaim<TUserKey, TUser> 
    : AuditUserClaimBase<TUserKey, TUser>, ISoftDelete 
    where TUserKey : struct, IEquatable<TUserKey> where TUser : UserBase<TUserKey> 
{
    /// <summary>
    /// <para>
    /// Implemented from <see cref="ISoftDelete"/>
    /// </para>
    /// Adds an <c>IsDeleted</c> flag to the <c>CoreUserClaim</c> table
    /// </summary>
    /// <remarks>
    /// See <see cref="ContextHelper.HandleSoftDelete(IEnumerable{Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry{ISoftDelete}})"/> for usage
    /// </remarks>
    public bool IsDeleted { get; set; }
}
