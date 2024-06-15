using Microsoft.AspNetCore.Identity;

namespace W.Ind.Core.Entity;

/// <summary>
/// Core entity <see langword="type"/> that can be used as an <see cref="IdentityUserLogin{TKey}"/>
/// </summary>
/// <remarks>
/// <para>
/// Implements <see cref="ISoftDelete"/>, which is the only property defined here
/// </para>
/// </remarks>
public class UserLogin : IdentityUserLogin<long>, ISoftDelete
{
    /// <summary>
    /// <para>
    /// Implemented from <see cref="ISoftDelete"/>
    /// </para>
    /// Adds an <c>IsDeleted</c> flag to the <c>UserLogin</c> table
    /// </summary>
    /// <remarks>
    /// See <see cref="ContextHelper.HandleSoftDelete(IEnumerable{Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry{ISoftDelete}})"/> for usage
    /// </remarks>
    public bool IsDeleted { get; set; }
}
