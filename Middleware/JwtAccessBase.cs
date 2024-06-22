using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using W.Ind.Core.Config;

namespace W.Ind.Core.Middleware;

public abstract class JwtAccessBase : JwtAccessBase<JwtConfig>
{
    public JwtAccessBase(RequestDelegate next, JwtConfig jwtConfig, IJwtInvalidator jwtInvalidator) 
        : base(next, jwtConfig, jwtInvalidator) { }
}

/// <summary>
/// An extensible <see langword="abstract"/> base middleware <see langword="class"/> for handling JWT
/// </summary>
public abstract class JwtAccessBase<TConfig> where TConfig : JwtConfig
{
    /// <summary>
    /// Preferably injected JWT Configuration options
    /// </summary>
    protected readonly TConfig _jwtConfig;

    /// <summary>
    /// Middleware specific <see cref="RequestDelegate"/>
    /// </summary>
    protected readonly RequestDelegate _next;

    /// <summary>
    /// Injected instance of <see cref="IJwtInvalidator"/> singleton service
    /// </summary>
    protected readonly IJwtInvalidator _jwtInvalidator;

    /// <summary>
    /// Constructor with injected service args
    /// </summary>
    /// <param name="next">Middleware specific <see cref="RequestDelegate"/></param>
    /// <param name="jwtConfig">Preferably inject JWT Configuration options</param>
    /// <param name="jwtInvalidator">Injected instance of <see cref="IJwtInvalidator"/> singleton service</param>
    public JwtAccessBase(RequestDelegate next, TConfig jwtConfig, IJwtInvalidator jwtInvalidator)
    {
        _next = next;
        _jwtConfig = jwtConfig;
        _jwtInvalidator = jwtInvalidator;
    }

    /// <summary>
    /// Checks via <paramref name="context"/> if the current route should be skipped from token processing
    /// </summary>
    /// <remarks>
    /// You must implement this method on your inheritting middleware <see langword="class"/>
    /// </remarks>
    /// <param name="context">The current <see cref="HttpContext"/> instance</param>
    /// <returns>
    /// <para><see langword="true"/>, if token processing can be skipped</para>
    /// <para><see langword="false"/>, if token processing cannot be skipped</para>
    /// </returns>
    /// <exception cref="NotImplementedException">You must <see langword="override"/> this method to use it</exception>
    protected virtual bool ShouldSkip(HttpContext context)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Generates a new <see langword="string"/> JSON Web Token from the given User <paramref name="claims"/>
    /// </summary>
    /// <param name="claims"></param>
    /// <returns></returns>
    protected virtual string GenerateToken(IEnumerable<Claim> claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry = DateTime.Now.AddMinutes(30);

        var token = new JwtSecurityToken(
            issuer: _jwtConfig.Issuer,
            audience: _jwtConfig.Audience,
            claims: claims,
            expires: expiry,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Returns a <see cref="ClaimsPrincipal"/> instance retreived from <paramref name="token"/> (even when expired)
    /// </summary>
    /// <param name="token"><see langword="string"/> JSON Web Token</param>
    /// <returns><see cref="ClaimsPrincipal"/> from passed <paramref name="token"/></returns>
    /// <exception cref="SecurityTokenException">Thrown when the validated <see cref="SecurityToken"/> is <see langword="null"/> or invalid</exception>
    protected virtual ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
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

    /// <summary>
    /// An <see langword="async"/> method that checks the current JWT value's validity in the given <paramref name="context"/>
    /// </summary>
    /// <remarks>
    /// <para>Sets the response <c>"Authorization"</c> header to new token value if successful</para>
    /// </remarks>
    /// <param name="context">The current <see cref="HttpContext"/> instance</param>
    /// <returns>Treat as <see langword="void"/></returns>
    protected virtual async Task ProcessTokenAsync(HttpContext context)
    {
        var token = context.GetAccessToken();
        if (token != null)
        {
            if (_jwtInvalidator.IsTokenInvalid(token)) 
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return; 
            }

            var principal = GetPrincipalFromExpiredToken(token);

            if (principal != null)
            {
                var newJwtToken = GenerateToken(principal.Claims);
                context.Response.Headers.Append("Authorization", "Bearer " + newJwtToken);
                await _next(context);
            }
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }
    }

    /// <summary>
    /// Resets the <c>"Authorization"</c> response header to use <paramref name="token"/>
    /// </summary>
    /// <param name="context">The current <see cref="HttpContext"/></param>
    /// <param name="token"><see langword="string"/> JSON Web Token</param>
    protected virtual void SetToken(HttpContext context, string token)
    {
        context.Response.Headers.Append("Authorization", "Bearer " + token);
    }
}
