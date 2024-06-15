namespace W.Ind.Core.Service;

/// <summary>
/// A derived <see langword="interface"/> for an injectable (Scoped) Service
/// </summary>
/// <remarks>
/// <para>Handles <see cref="Microsoft.AspNetCore.Identity.UserManager{TUser}"/> related tasks</para>
/// <para>Derive from this <see langword="interface"/> to add more functionality via interface injection</para>
/// </remarks>
/// <typeparam name="TUser">
/// <para>
/// The CLR type corresponding to your User entity type
/// </para>
/// <para>Ensure this inherits from <see cref="UserBase{TKey}"/></para>
/// </typeparam>
/// <typeparam name="TKey">
/// The data type of <typeparamref name="TUser"/>'s Primary Key
/// </typeparam>
public interface IUserService<TUser, TKey> : IUserServiceBase<TUser, TKey> where TUser : UserBase<TKey>, new() where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Gets the ID of the current user making this request
    /// </summary>
    /// <remarks>
    /// <para>Defaults to a pre-defined System user when the JSON Web Token is missing or invalid</para>
    /// </remarks>
    /// <returns>A User ID</returns>
    /// <exception cref="ObjectDisposedException">Thrown when <see cref="Microsoft.AspNetCore.Identity.UserManager{TUser}"/> instance has already been disposed</exception>
    /// <exception cref="ArgumentNullException">Thrown when <see cref="GetSystem(string?)"/> fails</exception>
    Task<TKey> GetCurrent();

    /// <summary>
    /// Gets the ID of a pre-defined System user for unauthenticated requests
    /// </summary>
    /// <remarks>
    /// Useful for setting the <see cref="IAuditable.CreatedById"/> property on any auditable entity that can be created anonymously
    /// </remarks>
    /// <param name="systemUserName">The UserName of the System user</param>
    /// <returns>System User's ID</returns>
    /// <exception cref="ObjectDisposedException">Thrown when <see cref="Microsoft.AspNetCore.Identity.UserManager{TUser}"/> instance has already been disposed</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="systemUserName"/> is <see langword="null"/> or empty when invoking <c>_userManager.FindByNameAsync</c></exception>
    Task<TKey> GetSystem(string? systemUserName = null);
}
