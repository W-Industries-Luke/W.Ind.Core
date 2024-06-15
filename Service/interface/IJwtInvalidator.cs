namespace W.Ind.Core.Service;

/// <summary>
/// Base <see langword="interface"/> implemented by the <see cref="JwtInvalidator"/>
/// </summary>
/// <remarks>
/// Used to invalidate unexpired JSON Web Tokens
/// </remarks>
public interface IJwtInvalidator : IDisposable
{
    /// <summary>
    /// Marks an unexpired JSON Web <paramref name="token"/> as invalid
    /// </summary>
    /// <param name="token"><see langword="string"/> JSON Web Token to invalidate</param>
    /// <param name="expiry">
    /// <para>How long the token will be marked invalid</para>
    /// <para>The time it will take until the <paramref name="token"/> naturally expires</para>
    /// </param>
    /// <returns>Treat as <see langword="void"/></returns>
    void InvalidateToken(string token, TimeSpan expiry);

    /// <summary>
    /// Determines if the <paramref name="token"/> has already been invalidated
    /// </summary>
    /// <param name="token"><see langword="string"/> JSON Web Token</param>
    /// <returns><see langword="true"/> if the <paramref name="token"/> is invalid</returns>
    bool IsTokenInvalid(string token);
}
