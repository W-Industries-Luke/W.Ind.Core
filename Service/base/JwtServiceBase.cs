using W.Ind.Core.Config;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using W.Ind.Core.Enum;

namespace W.Ind.Core.Service;

public abstract class JwtServiceBase 
    : JwtServiceBase<CoreUser>
{
    public JwtServiceBase(JwtConfig jwtConfig) : base(jwtConfig) { }
}

public abstract class JwtServiceBase<TUser> 
    : JwtServiceBase<long, TUser> 
    where TUser : UserBase
{
    public JwtServiceBase(JwtConfig jwtConfig) : base(jwtConfig) { }
}

public abstract class JwtServiceBase<TKey, TUser> 
    : JwtServiceBase<TKey, TUser, JwtConfig> 
    where TUser : UserBase<TKey> where TKey : struct, IEquatable<TKey>
{
    public JwtServiceBase(JwtConfig jwtConfig) : base(jwtConfig) { }
}

/// <summary>
/// <see langword="abstract"/> <see langword="class"/> implemented by <see cref="JwtService{TUser, TKey, TConfig}"/>
/// </summary>
/// <remarks>
/// <para>Contains <see langword="protected"/> instance methods that can be invoked in any derived <see langword="class"/></para>
/// <para>Built for extensibility</para>
/// </remarks>
/// <typeparam name="TUser">
/// <para>The CLR <see langword="type"/> corresponding to your User entity/table</para> 
/// <para>Ensure this <see langword="type"/> derives from <see cref="UserBase{TKey}"/></para>
/// </typeparam>
/// <typeparam name="TKey">
/// The data type of <typeparamref name="TUser"/>'s Primary Key
/// </typeparam>
/// <typeparam name="TConfig">
/// The <see langword="type"/> of your JWT Configuration Options DTO
/// </typeparam>
public abstract class JwtServiceBase<TKey, TUser, TConfig> 
    where TUser : UserBase<TKey> where TKey : struct, IEquatable<TKey> where TConfig : JwtConfig
{
    /// <summary>
    /// Options for JWT mapped directly from the application's Configuration file
    /// </summary>
    protected readonly TConfig _jwtConfig;

    /// <summary>
    /// Base constructor
    /// </summary>
    /// <param name="config">Mappped from Configuration and injected</param>
    public JwtServiceBase(TConfig config) 
    {
        _jwtConfig = config;
    }

    /// <summary>
    /// Returns a <see cref="ClaimsPrincipal"/> instance retreived from <paramref name="token"/>
    /// </summary>
    /// <param name="token"><see langword="string"/> JWT Access Token</param>
    /// <returns><see cref="ClaimsPrincipal"/> from passed <paramref name="token"/></returns>
    /// <exception cref="SecurityTokenException">Thrown when the validated <see cref="SecurityToken"/> is <see langword="null"/> or invalid</exception>
    public virtual ClaimsPrincipal GetPrincipalFromToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = _jwtConfig.ValidateAudience,
            ValidateIssuer = _jwtConfig.ValidateIssuer,
            ValidateIssuerSigningKey = _jwtConfig.ValidateIssuerSigningKey,
            ValidAudience = _jwtConfig.Audience,
            ValidIssuer = _jwtConfig.Issuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.SecretKey))
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
        var jwtSecurityToken = securityToken as JwtSecurityToken;

        if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }

    protected virtual DateTime AccessTokenExpiry(bool rememberMe = false) => rememberMe ? DateTime.UtcNow.AddDays(30) : DateTime.UtcNow.AddMinutes(30);

    /// <summary>
    /// Configures and returns an Access Token for JWT validation
    /// </summary>
    /// <remarks>
    /// <para>
    /// To get the JSON Web Token as a <see langword="string"/>:
    /// </para>
    /// <para>
    /// Pass the returned <see cref="JwtSecurityToken"/> instance into <see cref="JwtSecurityTokenHandler.WriteToken(SecurityToken)"/> as it's param
    /// </para>
    /// </remarks>
    /// <param name="user">
    /// <para>The CLR <see langword="type"/> corresponding to your User entity/table</para> 
    /// <para>Ensure this <see langword="type"/> derives from <see cref="UserBase{TUser}"/></para>
    /// </param>
    /// <param name="expires">The expiration date to set</param>
    /// <returns>An instance of <see cref="JwtSecurityToken"/></returns>
    protected virtual JwtSecurityToken ConfigureAccessToken(TUser user, DateTime expires)
    {        
        Claim[] claims = GetUserClaims(user);
        return ConfigureAccessToken(claims, expires);
    }

    protected virtual JwtSecurityToken ConfigureAccessToken(IEnumerable<Claim> claims, DateTime expires) 
    {
        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.SecretKey));
        SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        return new JwtSecurityToken(
            issuer: _jwtConfig.Issuer,
            audience: _jwtConfig.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds);

    }

    /// <summary>
    /// Gets the passed <typeparamref name="TUser"/>'s <see cref="Claim"/>s in order to configure a JSON Web token
    /// </summary>
    /// <remarks>
    /// Override this method to get more claims added to your JSON Web Token
    /// </remarks>
    /// <param name="user">
    /// <para>The CLR <see langword="type"/> corresponding to your User entity/table</para> 
    /// <para>Ensure this <see langword="type"/> derives from <see cref="UserBase{TUser}"/></para>
    /// </param>
    /// <returns>An <see cref="Array"/> of the <paramref name="user"/>'s <see cref="Claim"/>s</returns>
    /// <exception cref="NullReferenceException">Thrown when either <paramref name="user"/> or <c>user.Id</c> is <see langword="null"/></exception>
    protected virtual Claim[] GetUserClaims(TUser user)
    {
        // Add claims here
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(CoreClaimTypes.UserId, user.Id.ToString()!),
            new Claim(CoreClaimTypes.UserEmail, user.Email!)
        };

        return claims;
    }
}