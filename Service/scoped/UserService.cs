using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;

namespace W.Ind.Core.Service;

public class UserService : UserService<CoreUser>, IUserService
{
    public UserService(UserManager<CoreUser> userManager, SignInManager<CoreUser> signInManager, IJwtService jwtService, IHttpContextAccessor contextAccessor)
        : base(userManager, signInManager, jwtService, contextAccessor) { }
}

public class UserService<TUser> : UserService<long, TUser>, IUserService<TUser> where TUser : UserBase, new()
{
    public UserService(UserManager<TUser> userManager, SignInManager<TUser> signInManager, IJwtService<TUser> jwtService, IHttpContextAccessor contextAccessor)
        : base(userManager, signInManager, jwtService, contextAccessor) { }
}

/// <summary>
/// A <see langword="class"/> derived from <see cref="UserServiceBase{TUser,TKey}"/> and <see cref="IUserService{TUser,TKey}"/> 
/// </summary>
/// <remarks>
/// Handles <see cref="UserManager{TUser}"/> related tasks using <see cref="Microsoft.AspNetCore.Identity"/>
/// </remarks>
/// <typeparam name="TUser">
/// <para>The CLR <see langword="type"/> corresponding to your User entity/table</para> 
/// <para>Ensure this <see langword="type"/> derives from <see cref="UserBase{TKey}"/></para>
/// </typeparam>
/// <typeparam name="TKey">
/// The data type of <typeparamref name="TUser"/>'s Primary Key
/// </typeparam>
public class UserService<TKey, TUser> 
    : UserServiceBase<TKey, TUser>, IUserService<TKey, TUser> 
    where TUser : UserBase<TKey>, new() where TKey : struct, IEquatable<TKey>
{
    /// <summary>
    /// <see langword="protected"/> <see langword="readonly"/> field used to access <see cref="HttpContext"/> within derived classes
    /// </summary>
    /// <remarks>
    /// Used to get the authentication token from HTTP "Authorization" Header
    /// </remarks>
    protected readonly IHttpContextAccessor _contextAccessor;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <remarks>
    /// <para>Injects <paramref name="userManager"/> and <paramref name="jwtService"/> to pass along to base constructor</para>
    /// <para>Injects <paramref name="contextAccessor"/> to set on this class</para>
    /// </remarks>
    /// <param name="userManager">
    /// <para><see cref="UserManager{TUser}"/> instance</para> 
    /// <para><typeparamref name="TUser"/> being the same <see langword="type"/> as <see cref="UserService{TUser,TKey}"/></para>
    /// </param>
    /// <param name="signInManager">
    /// <para>Inject while configuring services in your <c>Program.cs</c> or <c>Startup.cs</c> file:</para>
    /// <example>
    /// <code>
    /// <see langword="using"/> <see cref="Microsoft.Extensions.DependencyInjection"/>;
    /// 
    /// builder.Services.AddIdentity&lt;<see cref="CoreUser"/>, <see cref="CoreRole"/>&gt;(...);
    /// </code>
    /// </example>
    /// </param>
    /// <param name="jwtService">
    /// <para><see cref="IJwtService{TUser,TKey}"/> instance</para>
    /// <para><typeparamref name="TUser"/> being the same <see langword="type"/> as <see cref="UserService{TUser,TKey}"/></para>
    /// </param>
    /// <param name="contextAccessor">
    /// <para><see cref="IHttpContextAccessor"/> instance</para>
    /// </param>
    public UserService(UserManager<TUser> userManager, SignInManager<TUser> signInManager, IJwtService<TKey, TUser> jwtService, IHttpContextAccessor contextAccessor) 
        : base(userManager, signInManager, jwtService)
    {
        _contextAccessor = contextAccessor;
    }

    public virtual TKey GetCurrent() 
    {
        TKey result;
        string? retrievedClaim = _contextAccessor?.HttpContext?.User?.Claims?.FirstOrDefault(a => a.Type == JwtRegisteredClaimNames.NameId)?.Value;

        if (retrievedClaim == null)
        {
            result = GetSystem();
        }
        else
        {
            result = ContextHelper.ParsePrimaryKey<TKey>(retrievedClaim);
        }

        return result;
    }

    /// <summary>
    /// Gets the ID of the current user making this request
    /// </summary>
    /// <remarks>
    /// <para>Utilizes <see cref="_contextAccessor"/> to retrive the JWT claim for the current user's ID</para>
    /// <para>Defaults to a pre-defined System user when the JSON Web Token is missing or invalid</para>
    /// </remarks>
    /// <returns>A User ID</returns>
    /// <exception cref="ObjectDisposedException">Thrown when <see cref="UserManager{TUser}"/> instance has already been disposed</exception>
    /// <exception cref="ArgumentNullException">Thrown when GetSystem() fails</exception>
    public virtual async Task<TKey> GetCurrentAsync() 
    {
        TKey result;
        string? retrievedClaim = _contextAccessor?.HttpContext?.User?.Claims?.FirstOrDefault(a => a.Type == JwtRegisteredClaimNames.NameId)?.Value;

        if (retrievedClaim == null) 
        {
            result = await GetSystemAsync();
        }
        else
        {
            result = ContextHelper.ParsePrimaryKey<TKey>(retrievedClaim);
        }

        return result;
    }

    public virtual TKey GetSystem(string? systemUserName = null) 
    {
        if (String.IsNullOrWhiteSpace(systemUserName))
        {
            systemUserName = "SYSTEM";
        }

        var user = _userManager.FindByNameAsync(systemUserName).Result;
        if (user != null)
        {
            return user.Id;
        }

        throw new InvalidOperationException($"System UserName \"${systemUserName}\" Not Found");
    }

    /// <summary>
    /// Gets the ID of a pre-defined System user for unauthenticated requests
    /// </summary>
    /// <remarks>
    /// Useful for setting the <see cref="IAuditable.CreatedById"/> property on any auditable entity that can be created anonymously
    /// </remarks>
    /// <param name="systemUserName">The UserName of the System user</param>
    /// <returns>System User's ID</returns>
    /// <exception cref="ObjectDisposedException">Thrown when <see cref="UserManager{TUser}"/> instance has already been disposed</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="systemUserName"/> is <see langword="null"/> or empty when invoking <c>_userManager.FindByNameAsync</c></exception>
    public virtual async Task<TKey> GetSystemAsync(string? systemUserName = null)
    {
        if (String.IsNullOrWhiteSpace(systemUserName))
        {
            systemUserName = "SYSTEM";
        }

        var user = await _userManager.FindByNameAsync(systemUserName);
        if (user != null) 
        {
            return user.Id;
        }

        throw new InvalidOperationException($"System UserName \"${systemUserName}\" Not Found");
    }
}
