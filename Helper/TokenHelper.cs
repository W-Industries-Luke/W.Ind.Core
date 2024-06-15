using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;

namespace W.Ind.Core.Helper;

/// <summary>
/// This <see langword="static"/> helper class contains methods useful for JWT Validation
/// </summary>
public static class TokenHelper
{
    /// <summary>
    /// A <see langword="static"/> extension method that retrieves a bearer token from the <paramref name="context"/>
    /// </summary>
    /// <param name="context">The current <see cref="HttpContext"/></param>
    /// <returns><see langword="string"/> JWT value (if present)</returns>
    public static string? GetBearerToken(this HttpContext context) 
    {
        return context.Request.Headers[HeaderNames.Authorization].ToString().Split(" ").LastOrDefault();
    }

    /// <summary>
    /// Gets the <see href="TimeSpan"/> from now until the <paramref name="token"/> expires
    /// </summary>
    /// <remarks>
    /// <para>Useful for invalidating JWT in-memory until it naturally expires</para>
    /// </remarks>
    /// <param name="token"><see langword="string"/> JSON Web Token</param>
    /// <returns>The <see cref="TimeSpan"/> representing how long until this <paramref name="token"/> naturally expires</returns>
    /// <exception cref="InvalidOperationException">Thrown when <paramref name="token"/> is unreadable</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="token" /> is too long</exception>
    public static TimeSpan GetInvalidatedTimeSpan(string token)
    {
        // Default 30 minutes
        TimeSpan tokenExpiry = DateTime.UtcNow.AddMinutes(30) - DateTime.UtcNow;
        DateTime? getExpiration = GetTokenExpiration(token);

        if (getExpiration.HasValue)
        {
            tokenExpiry = getExpiration.Value - DateTime.UtcNow;
        }

        return tokenExpiry;
    }

    /// <summary>
    /// Get the current Expiration Date of a JSON Web Token (if readable)
    /// </summary>
    /// <param name="token">JSON Web Token</param>
    /// <returns>The expiration date of <paramref name="token"/></returns>
    /// <exception cref="InvalidOperationException">Thrown when <paramref name="token"/> is unreadable</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="token" /> is too long</exception>
    public static DateTime? GetTokenExpiration(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        // Validate if the token is a valid JWT token
        if (tokenHandler.CanReadToken(token))
        {
            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
            if (jwtToken != null)
            {
                var expClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Exp);
                if (expClaim != null && long.TryParse(expClaim.Value, out long exp))
                {
                    // Convert from Unix time to DateTime
                    var expirationTime = DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime;
                    return expirationTime;
                }
            }
        }

        return null;
    }
}