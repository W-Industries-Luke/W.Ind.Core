using Microsoft.AspNetCore.Identity;

namespace W.Ind.Core.Entity;

public class CoreUserToken : CoreUserToken<long>, ISoftDelete;

/// <summary>
/// Concrete core entity <see langword="class"/> that can be used as an <see cref="IdentityUserToken{TKey}"/>
/// </summary>
/// <remarks>
/// <para>
/// Implements <see cref="ISoftDelete"/>, which is the only property defined here
/// </para>
/// </remarks>
public class CoreUserToken<TUserKey> : IdentityUserToken<TUserKey>, ISoftDelete where TUserKey : IEquatable<TUserKey>
{
    /// <summary>
    /// <para>
    /// Implemented from <see cref="ISoftDelete"/>
    /// </para>
    /// Adds an <c>IsDeleted</c> flag to the <c>UserToken</c> table
    /// </summary>
    /// <remarks>
    /// See <see cref="ContextHelper.HandleSoftDelete(IEnumerable{Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry{ISoftDelete}})"/> for usage
    /// </remarks>
    public bool IsDeleted { get; set; }
}
