using Microsoft.AspNetCore.Identity;

namespace W.Ind.Core.Entity;

/// <summary>
/// Concrete core entity <see langword="class"/> that can be used as an <see cref="IdentityUserToken{TKey}"/>
/// </summary>
/// <remarks>
/// <para>
/// Implements <see cref="ISoftDelete"/>, which is the only property defined here
/// </para>
/// </remarks>
public class UserToken : IdentityUserToken<long>, ISoftDelete
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
