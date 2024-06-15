using W.Ind.Core.Dto;
using W.Ind.Core.Config;
using System.IdentityModel.Tokens.Jwt;

namespace W.Ind.Core.Service;

/// <summary>
/// An injectible (scoped) service to handle JWT-related functions
/// </summary>
/// <remarks>
/// <para>Ideally for unauthenticated controller routes (such as 'Login')</para>
/// </remarks>
/// <typeparam name="TUser">
/// <para>The CLR <see langword="type"/> corresponding to your User entity/table</para> 
/// <para>Ensure this <see langword="type"/> derives from <see cref="UserBase{TKey}"/></para>
/// </typeparam>
/// <typeparam name="TKey">
/// The data type of <typeparamref name="TUser"/>'s Primary Key
/// </typeparam>
/// /// <typeparam name="TConfig">
/// The <see langword="type"/> of your JWT Configuration Options DTO
/// </typeparam>
public class JwtService<TUser, TKey, TConfig> : JwtServiceBase<TUser, TKey, TConfig>, IJwtService<TUser, TKey> where TUser : UserBase<TKey>, new() where TKey : IEquatable<TKey> where TConfig : JwtConfig
{
    /// <summary>
    /// A <see langword="protected"/> field containing the service reference for <see cref="IJwtInvalidator"/> (singleton)
    /// </summary>
    protected readonly IJwtInvalidator _jwtInvalidator;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <remarks>
    /// Injected instance of <typeparamref name="TConfig"/> <paramref name="config"/> is passed along to the <see langword="abstract"/> parent <see href="JwtServiceBase"/>'s constructor
    /// </remarks>
    /// <param name="config">Preferably injected config options</param>
    /// <param name="jwtInvalidator">Preferably injected singleton service</param>
    public JwtService(TConfig config, IJwtInvalidator jwtInvalidator): base(config) 
    {
        _jwtInvalidator = jwtInvalidator;
    }

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
    /// <param name="rememberMe">If true, token expiration set for 30 days from now. Otherwise it defaults to 30 minutes.</param>
    /// <returns>A derived instance of <see cref="ILoginResponse"/></returns>
    /// <exception cref="ArgumentNullException">Thrown if the generated <see cref="JwtSecurityToken"/> is <see langword="null"/></exception>
    /// <exception cref="ArgumentException">Thrown if the generated token is not of type <see cref="JwtSecurityToken"/></exception>
    /// <exception cref="Microsoft.IdentityModel.Tokens.SecurityTokenEncryptionFailedException">Thrown if encryption fails for any reason</exception>
    public virtual TLoginResponse GenerateToken<TLoginResponse>(TUser user, bool rememberMe = false) 
        where TLoginResponse : class, ILoginResponse, new()
    {
        DateTime expires = rememberMe ? DateTime.UtcNow.AddDays(30) : DateTime.UtcNow.AddMinutes(30);
        JwtSecurityToken token = ConfigureToken(user, expires);
        return new TLoginResponse { Token = new JwtSecurityTokenHandler().WriteToken(token), Expires = expires, Success = true };
    }

    /// <summary>
    /// Uses instance of <see cref="IJwtInvalidator"/> singleton service to invalidate <paramref name="token"/>
    /// </summary>
    /// <param name="token"><see langword="string"/>? JSON Web Token value</param>
    /// <exception cref="InvalidOperationException">Thrown when <paramref name="token"/> is unreadable</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="token" /> is too long</exception>
    public virtual void InvalidateToken(string? token) 
    {
        if (!String.IsNullOrWhiteSpace(token))
        {
            _jwtInvalidator.InvalidateToken(token, TokenHelper.GetInvalidatedTimeSpan(token));
        }
    }

    /// <summary>
    /// Uses instance of <see cref="IJwtInvalidator"/> singleton service to check if <paramref name="token"/> is invalid
    /// </summary>
    /// <param name="token"><see langword="string"/>? JSON Web Token value</param>
    /// <returns>
    /// <para><see langword="true"/>, if <paramref name="token"/> is invalid</para>
    /// <para><see langword="false"/>, if <paramref name="token"/> is valid</para>
    /// </returns>
    public virtual bool IsTokenInvalid(string? token)
    {
        if (!String.IsNullOrWhiteSpace(token))
        {
            return _jwtInvalidator.IsTokenInvalid(token);
        }
        return true;
    }
}
