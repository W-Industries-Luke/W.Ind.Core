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
public class RoleClaim : AuditRoleClaimBase<long>, ISoftDelete, IAuditable 
{
    /// <summary>
    /// <para>
    /// Implemented from <see cref="ISoftDelete"/>
    /// </para>
    /// Adds an <c>IsDeleted</c> flag to the <c>RoleClaim</c> table
    /// </summary>
    /// <remarks>
    /// See <see cref="ContextHelper.HandleSoftDelete(IEnumerable{Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry{ISoftDelete}})"/> for usage
    /// </remarks>
    public bool IsDeleted { get; set; }
}
