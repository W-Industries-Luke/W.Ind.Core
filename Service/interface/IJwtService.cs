using W.Ind.Core.Dto;
namespace W.Ind.Core.Service;

/// <summary>
/// An injectible (scoped) service to handle JWT-related functions
/// </summary>
/// <remarks>
/// <para>Ideally for unauthenticated controller routes (such as 'Login')</para>
/// </remarks>
/// <typeparam name="TUser">
/// <para>The CLR <see langword="type"/> corresponding to your User entity/table</para> 
/// <para>Ensure this <see langword="type"/> derives from <see cref="UserBase{TUser}"/></para>
/// </typeparam>
/// <typeparam name="TKey">
/// The data type of <typeparamref name="TUser"/>'s Primary Key
/// </typeparam>
public interface IJwtService<TUser, TKey> where TUser : UserBase<TKey>, new() where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Generates a JSON Web Token for the given <typeparamref name="TUser"/>
    /// </summary>
    /// <remarks>
    /// <para>Expiration options being either 30 minutes or 30 days from <see cref="DateTime.UtcNow"/></para>
    /// <para>Use for login requests</para>
    /// </remarks>
    /// <typeparam name="TLoginResponse">The return type (any derived instance of <see cref="ILoginResponse"/>)</typeparam>
    /// <param name="user">
    /// <para>An instance of the CLR <see langword="type"/> corresponding to your User entity/table</para> 
    /// <para>Ensure this <see langword="type"/> derives from <see cref="UserBase{TUser}"/></para>
    /// </param>
    /// <param name="rememberMe">
    /// <para>If <see langword="true"/>, token expires in 30 days</para>
    /// <para>If <see langword="false"/>, token expires in 30 minutes</para>
    /// </param>
    /// <returns>A derived instance of <see cref="ILoginResponse"/></returns>
    /// <exception cref="ArgumentNullException">Thrown if the generated <see cref="System.IdentityModel.Tokens.Jwt.JwtSecurityToken"/> is <see langword="null"/></exception>
    /// <exception cref="ArgumentException">Thrown if the generated token is not of type <see cref="System.IdentityModel.Tokens.Jwt.JwtSecurityToken"/></exception>
    /// <exception cref="Microsoft.IdentityModel.Tokens.SecurityTokenEncryptionFailedException">Thrown if encryption ever fails</exception>

    TLoginResponse GenerateToken<TLoginResponse>(TUser user, bool rememberMe) 
        where TLoginResponse : class, ILoginResponse, new();

    /// <summary>
    /// Invalidates <paramref name="token"/>
    /// </summary>
    /// <param name="token"><see langword="string"/> JSON Web Token value</param>
    /// <exception cref="InvalidOperationException">Thrown when <paramref name="token"/> is unreadable</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="token" /> is too long</exception>
    void InvalidateToken(string? token);

    /// <summary>
    /// Checks if <paramref name="token"/> is invalid
    /// </summary>
    /// <param name="token"><see langword="string"/> JSON Web Token value</param>
    /// <returns>
    /// <para><see langword="true"/>, if <paramref name="token"/> is invalid</para>
    /// <para><see langword="false"/>, if <paramref name="token"/> is valid</para>
    /// </returns>
    bool IsTokenInvalid(string? token);
}
