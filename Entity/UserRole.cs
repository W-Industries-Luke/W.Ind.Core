namespace W.Ind.Core.Entity;

public class UserRole : UserRole<User>, ISoftDelete, IAuditable, IJoinTable;

public class UserRole<TUser> 
    : UserRole<long, TUser>, ISoftDelete, IAuditable<TUser>, IJoinTable
    where TUser : UserBase<long>;

/// <summary>
/// Concrete core entity <see langword="class"/> that can be used as an <see cref="Microsoft.AspNetCore.Identity.IdentityUserLogin{TKey}"/>
/// </summary>
/// <remarks>
/// <para>
/// Inherits from <see langword="abstract"/> <see cref="AuditUserRoleBase{TRoleKey}"/> which implements <see cref="IAuditable"/>
/// </para>
/// <para>
/// Implements <see cref="ISoftDelete"/>, which is the only property defined here
/// </para>
/// </remarks>
public class UserRole<TKey, TUser> 
    : AuditUserRoleBase<TKey, TUser>, ISoftDelete, IAuditable<TKey, TUser>, IJoinTable
    where TKey : struct, IEquatable<TKey> where TUser : UserBase<TKey> 
{
    /// <summary>
    /// <para>
    /// Implemented from <see cref="ISoftDelete"/>
    /// </para>
    /// Adds an <c>IsDeleted</c> flag to the <c>UserRole</c> table
    /// </summary>
    /// <remarks>
    /// See <see cref="ContextHelper.HandleSoftDelete(IEnumerable{Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry{ISoftDelete}})"/> for usage
    /// </remarks>
    public bool IsDeleted { get; set; }
}
