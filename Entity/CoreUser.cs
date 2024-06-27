namespace W.Ind.Core.Entity;

/// <summary>
/// Concrete core entity <see langword="class"/> that can be used as an <see cref="Microsoft.AspNetCore.Identity.IdentityUser{TKey}"/>
/// </summary>
/// <remarks>
/// <para>
/// Inherits from <see langword="abstract"/> <see cref="AuditUserBase{TKey}"/> which implements <see cref="IAuditable"/> and <see cref="IEntity{TKey}"/>
/// </para>
/// <para>
/// Implements <see cref="ISoftDelete"/>, which is the only property defined here
/// </para>
/// </remarks>
public class CoreUser 
    : AuditUserBase, ISoftDelete, IAuditable<CoreUser>, IEntity<long>
{
    public bool IsDeleted { get; set; }
}

public class CoreUser<TKey> 
    : AuditUserBase<TKey, CoreUser<TKey>>, ISoftDelete, IAuditable<TKey, CoreUser<TKey>>, IEntity<TKey>
    where TKey : struct, IEquatable<TKey> // where TUser : UserBase<TKey>
{
    /// <summary>
    /// <para>Implemented from <see cref="ISoftDelete"/></para>
    /// <para>Adds an <c>IsDeleted</c> flag to the <c>User</c> table</para>
    /// </summary>
    /// <remarks>
    /// See <see cref="ContextHelper.HandleSoftDelete(IEnumerable{Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry{ISoftDelete}})"/> for usage
    /// </remarks>
    public bool IsDeleted { get; set; }

}