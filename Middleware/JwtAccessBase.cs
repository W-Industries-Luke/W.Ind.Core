using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace W.Ind.Core.Middleware;

public abstract class JwtAccessBase : JwtAccessBase<IJwtService>
{
    public JwtAccessBase(RequestDelegate next) : base(next) { }
}

public abstract class JwtAccessBase<TJwtService> : JwtAccessBase<TJwtService, CoreUser>
    where TJwtService : class, IJwtService
{
    public JwtAccessBase(RequestDelegate next) : base(next) { }
}

public abstract class JwtAccessBase<TJwtService, TUser> : JwtAccessBase<TJwtService, long, TUser>
    where TJwtService : class, IJwtService<TUser> where TUser : UserBase, new()
{
    public JwtAccessBase(RequestDelegate next) : base(next) { }
}

/// <summary>
/// An extensible <see langword="abstract"/> base middleware <see langword="class"/> for handling JWT
/// </summary>
public abstract class JwtAccessBase<TJwtService, TKey, TUser> 
    where TJwtService : class, IJwtService<TKey, TUser> where TKey : struct, IEquatable<TKey> where TUser : UserBase<TKey>, new()
{
    /// <summary>
    /// Middleware specific <see cref="RequestDelegate"/>
    /// </summary>
    protected readonly RequestDelegate _next;

    /// <summary>
    /// Constructor with injected service args
    /// </summary>
    /// <param name="next">Middleware specific <see cref="RequestDelegate"/></param>
    public JwtAccessBase(RequestDelegate next)
    {
        _next = next;
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
            ClaimsPrincipal? principal = await ValidateToken(context, token);
            await AttachAccessToken(context, principal);
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }
    }

    protected virtual async Task AttachAccessToken(HttpContext context, ClaimsPrincipal? principal) 
    {
        if (principal != null)
        {
            var newJwtToken = GetJwtService(context).GenerateAccessToken(principal.Claims);
            context.Response.Headers.Append("Authorization", "Bearer " + newJwtToken.Token);
            await _next(context);
        }
    }

    protected virtual async Task<ClaimsPrincipal?> ValidateToken(HttpContext context, string token)
    {
        TJwtService jwtService = GetJwtService(context);

        if (jwtService.IsTokenInvalid(token))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return null;
        }

        return jwtService.GetPrincipalFromToken(token);
    }

    protected virtual TJwtService GetJwtService(HttpContext context) 
    {
        var service = context.RequestServices.GetService(typeof(TJwtService)) as TJwtService;

        if (service == null) { throw new InvalidOperationException($"JWT Service: {typeof(TJwtService).Name} not configured"); }

        return service;
    }
}
