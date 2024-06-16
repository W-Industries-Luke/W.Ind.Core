using System.Collections.Concurrent;

namespace W.Ind.Core.Service;

/// <summary>
/// A derived Singleton Service class to be injected into custom Middleware.
/// </summary>
/// <remarks>
/// Inject this as a Singleton Service, and not a Scoped Service.
/// </remarks>
public class JwtInvalidator : IJwtInvalidator
{
    /// <summary>
    /// Stores invalid tokens in-memory until they expire.
    /// </summary>
    /// <value>
    /// <para>The Key will contain a JSON Web Token <see cref="string"/>.</para>
    /// <para>The Value is a <see cref="DateTime"/> representing when this token can be removed.</para>
    /// </value>
    protected readonly ConcurrentDictionary<string, DateTime> _invalidTokens = new ConcurrentDictionary<string, DateTime>();

    /// <summary>
    /// <see cref="Timer"/> instance used to check on invalid tokens
    /// </summary>
    protected Timer _cleanupTimer;

    /// <summary>
    /// Default Constructor
    /// </summary>
    /// <remarks>
    /// Starts a <see cref="Timer"/> with the callback removing expired tokens from <see cref="_invalidTokens"/>
    /// </remarks>
    public JwtInvalidator()
    {
        _cleanupTimer = new Timer(CleanupExpiredTokens, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
    }

    /// <summary>
    /// Derived from <see cref="IDisposable"/>
    /// </summary>
    /// <remarks>
    /// <para>
    /// Sets <see href="_cleanupTimer"/> interval to <see cref="Timeout.Infinite"/> before disposing
    /// </para>
    /// <para>
    /// Calls <see cref="GC.SuppressFinalize(object)"/> so further abstraction is still an option
    /// </para>
    /// </remarks>
    public void Dispose()
    {
        _cleanupTimer?.Change(Timeout.Infinite, 0);
        _cleanupTimer?.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Marks an unexpired JSON Web <paramref name="token"/> as invalid
    /// </summary>
    /// <remarks>
    /// Attempts to add <paramref name="token"/> param to the <see cref="_invalidTokens"/> property with (<see cref="DateTime.UtcNow"/> + <paramref name="expiry"/> param) as the value
    /// </remarks>
    /// <param name="token"><see langword="string"/> JSON Web Token to invalidate</param>
    /// <param name="expiry">
    /// <para>How long the <paramref name="token"/> will be marked invalid</para>
    /// <para>Time it takes until the <paramref name="token"/> naturally expires</para>
    /// </param>
    /// <returns>Treat as <see langword="void"/></returns>
    public void InvalidateToken(string token, TimeSpan expiry)
    {
        var expirationTime = DateTime.UtcNow.Add(expiry);
        _invalidTokens.TryAdd(GetCacheKey(token), expirationTime);
    }

    /// <summary>
    /// Determines if the <paramref name="token"/> has already been invalidated
    /// </summary>
    /// <remarks>
    /// Simply checks if <see cref="_invalidTokens"/> already contains a key for the <paramref name="token"/> param
    /// </remarks>
    /// <param name="token"><see langword="string"/> JSON Web Token</param>
    /// <returns><see langword="true"/> if the <paramref name="token"/> is invalid</returns>
    public bool IsTokenInvalid(string token)
    {
        return _invalidTokens.ContainsKey(GetCacheKey(token));
    }

    /// <summary>
    /// A callback method to remove expired tokens from <see cref="_invalidTokens"/>
    /// </summary>
    /// <remarks>
    /// <para>
    /// Invoked by <see cref="_cleanupTimer"/>
    /// </para>
    /// <para>Removes entries where the value is &lt;= <see cref="DateTime.UtcNow"/></para>
    /// </remarks>
    /// <param name="state">The arg passed into the callback. Unused in this scenario.</param>
    protected virtual void CleanupExpiredTokens(object? state)
    {
        var now = DateTime.UtcNow;
        var expiredTokens = _invalidTokens.Where(kvp => kvp.Value <= now).Select(kvp => kvp.Key).ToList();

        foreach (var token in expiredTokens)
        {
            _invalidTokens.TryRemove(token, out _);
        }
    }

    /// <summary>
    /// Gets the interpolated Key value used to store an invalid <paramref name="token"/>
    /// </summary>
    /// <param name="token">JSON Web Token <see langword="string"/></param>
    /// <returns>The corresponding key (<see langword="string"/>) for an invalid token</returns>
    protected virtual string GetCacheKey(string token)
    {
        return $"blacklisted_token:{token}";
    }
}
