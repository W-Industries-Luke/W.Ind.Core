using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using W.Ind.Core.Dto;

namespace W.Ind.Core.Service;

public abstract class UserServiceBase : UserServiceBase<User>
{
    public UserServiceBase(UserManager<User> userManager, SignInManager<User> signInManager, IJwtService jwtService)
    : base(userManager, signInManager, jwtService) { }

}

public abstract class UserServiceBase<TUser> : UserServiceBase<long, TUser> where TUser : UserBase<long>, new()
{
    public UserServiceBase(UserManager<TUser> userManager, SignInManager<TUser> signInManager, IJwtService<TUser> jwtService)
        : base(userManager, signInManager, jwtService) { }
}

/// <summary>
/// An <see langword="abstract"/> <see langword="class"/> implemented by <see cref="UserService{TUser, TKey}"/>
/// </summary>
/// <remarks>
/// <para>Handles <see cref="UserManager{TUser}"/> related tasks using <see cref="Microsoft.AspNetCore.Identity"/></para>
/// </remarks>
/// <typeparam name="TUser">
/// <para>The CLR <see langword="type"/> corresponding to your User entity/table</para> 
/// <para>Ensure this <see langword="type"/> derives from <see cref="UserBase{TUser}"/></para>
/// </typeparam>
/// <typeparam name="TKey">
/// The data type of <typeparamref name="TUser"/>'s Primary Key
/// </typeparam>
public abstract class UserServiceBase<TKey, TUser> 
    : IUserServiceBase<TKey, TUser> 
    where TUser : UserBase<TKey>, new() where TKey : struct, IEquatable<TKey>
{
    /// <summary>
    /// A <see langword="protected"/> <see langword="readonly"/> field used to access <see cref="UserManager{TUser}"/> within derived <see langword="class"/> implementation
    /// </summary>
    protected readonly UserManager<TUser> _userManager;

    /// <summary>
    /// A <see langword="protected"/> <see langword="readonly"/> field used to access <see cref="SignInManager{TUser}"/> within derived <see langword="class"/> implementation
    /// </summary>
    protected readonly SignInManager<TUser> _signInManager;

    /// <summary>
    /// A <see langword="protected"/> <see langword="readonly"/> field used to access <see cref="IJwtService{TKey, TUser}"/> within derived <see langword="class"/> implementation
    /// </summary>
    protected readonly IJwtService<TKey, TUser> _jwtService;

    /// <summary>
    /// Base Constructor
    /// Base constructor for the "UserService" class that takes injected service dependencies.
    /// </summary>
    /// <remarks>
    /// Args are meant to be injected service dependencies
    /// </remarks>
    /// <param name="userManager">
    /// <para>Inject while configuring services in your <c>Program.cs</c> or <c>Startup.cs</c> file:</para>
    /// <example>
    /// <code>
    /// <see langword="using"/> <see cref="Microsoft.Extensions.DependencyInjection"/>;
    /// 
    /// builder.Services.AddIdentity&lt;<see cref="User"/>, <see cref="Role"/>&gt;(...);
    /// </code>
    /// </example>
    /// </param>
    /// <param name="jwtService">Injected instance of <see cref="IJwtService{TUser, TKey}"/> singleton</param>
    /// <param name="signInManager">
    /// <para>Inject while configuring services in your <c>Program.cs</c> or <c>Startup.cs</c> file:</para>
    /// <example>
    /// <code>
    /// <see langword="using"/> <see cref="Microsoft.Extensions.DependencyInjection"/>;
    /// 
    /// builder.Services.AddIdentity&lt;<see cref="User"/>, <see cref="Role"/>&gt;(...);
    /// </code>
    /// </example>
    /// </param>
    public UserServiceBase(UserManager<TUser> userManager, SignInManager<TUser> signInManager, IJwtService<TKey, TUser> jwtService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _signInManager = signInManager;
    }

    public virtual async Task<IdentityResult> RegisterAsync(UserRegistration dto)
    {
        return await RegisterAsync<UserRegistration>(dto);
    }

    /// <summary>
    /// An <see langword="async"/> method that adds a new <typeparamref name="TUser"/> to the System
    /// </summary>
    /// <typeparam name="TUserRegistration">
    /// <para>Any concrete <see langword="class"/> derived from <see cref="IUserRegistration"/> to use as the <paramref name="dto"/> parameter</para>
    /// </typeparam>
    /// <param name="dto">
    /// <para>An instance of any concrete <see langword="class"/> derived from <see cref="IUserRegistration"/></para>
    /// <para>Matches generic type <typeparamref name="TUserRegistration"/></para>
    /// </param>
    /// <returns>An insance of <see cref="IdentityResult"/> from the <see cref="UserManager{TUser}.CreateAsync(TUser, string)"/> method</returns>
    /// <exception cref="ArgumentNullException">Thrown when either the Email or Password is <see langword="null"/></exception>
    /// <exception cref="ObjectDisposedException">Thrown when the <see cref="_userManager"/> has already been disposed</exception>
    /// <exception cref="ValidationException">Thrown when either the Email or UserName is taken</exception>
    public virtual async Task<IdentityResult> RegisterAsync<TUserRegistration>(TUserRegistration dto) where TUserRegistration : class, IUserRegistration
    {
        IdentityResult result;
        TUser user = new TUser
        {
            Email = dto.Email,
            UserName = dto.UserName,
            ConcurrencyStamp = new Guid().ToString(),
            SecurityStamp = new Guid().ToString()
        };

        try { result = await _userManager.CreateAsync(user, dto.Password); }
        catch (ArgumentNullException) { throw; }
        catch (ObjectDisposedException) { throw; }
        catch (ValidationException) { throw; }
        catch (Exception) { throw; }

        return result;
    }

    public virtual async Task<LoginResponse> ValidateLoginAsync(LoginRequest dto)
    {
        LoginResponse<TokenResponse> response = await ValidateLoginAsync<TokenResponse>(dto);

        return LoginResponse.FromGenericType(response);
    }

    public virtual async Task<LoginResponse<TTokenResponse>> ValidateLoginAsync<TTokenResponse>(LoginRequest dto)
        where TTokenResponse : class, ITokenResponse, new()
    {
        return await ValidateLoginAsync<LoginResponse<TTokenResponse>, TTokenResponse>(dto);
    }

    public virtual async Task<TLoginResponse> ValidateLoginAsync<TLoginResponse, TTokenResponse>(LoginRequest dto)
        where TLoginResponse : ILoginResponse<TTokenResponse>, new() where TTokenResponse : class, ITokenResponse, new()
    {
        return await ValidateLoginAsync<LoginRequest, TLoginResponse, TTokenResponse>(dto);
    }

    /// <summary>
    /// An <see langword="async"/> method that validates a <typeparamref name="TLoginRequest"/>
    /// </summary>
    /// <remarks>
    /// Will check the number of consecutive failed login attempts and lockout the user after the 7th failed attempt
    /// </remarks>
    /// <typeparam name="TLoginRequest">Any class derived from <see cref="ILoginRequest"/></typeparam>
    /// <typeparam name="TLoginResponse">A concrete <see langword="type"/> derived from <see cref="ILoginResponse"/></typeparam>
    /// <param name="dto">A concrete <see langword="type"/> instance that matches <typeparamref name="TLoginRequest"/></param>
    /// <returns>A concrete <see langword="type"/> instance that matches <typeparamref name="TLoginResponse"/></returns>
    /// <exception cref="ArgumentNullException">Thrown when UserName or Password is <see langword="null"/> or empty</exception>
    /// <exception cref="ObjectDisposedException">Thrown when the <see cref="_userManager"/> has already been disposed</exception>
    /// <exception cref="ValidationException">Thrown when the user has been locked out</exception>
    public virtual async Task<TLoginResponse> ValidateLoginAsync<TLoginRequest, TLoginResponse, TTokenResponse>(TLoginRequest dto)
        where TLoginRequest : class, ILoginRequest where TLoginResponse : ILoginResponse<TTokenResponse>, new() where TTokenResponse : class, ITokenResponse, new()
    {
        TLoginResponse response = new TLoginResponse();
        try
        {
            TUser? user = await _userManager.FindByNameAsync(dto.UserName) ?? await _userManager.FindByEmailAsync(dto.UserName);
            if (user != null)
            {
                if (!await _userManager.IsLockedOutAsync(user))
                {
                    response = await ValidatePasswordAsync<TLoginRequest, TLoginResponse, TTokenResponse>(user, dto);
                }
                else
                {
                    response = new TLoginResponse { LockedOut = true };
                }

                return response;
            }

            response.NotFound = true;
            
            return response;
        }
        catch (ArgumentNullException) { throw; }
        catch (ObjectDisposedException) { throw; }
        catch (ValidationException) { throw; }
        catch (Exception) { throw; }
    }

    protected virtual async Task<LoginResponse> ValidatePasswordAsync(TUser user, LoginRequest dto)
    {
        return await ValidatePasswordAsync<LoginRequest, LoginResponse>(user, dto);
    }

    protected virtual async Task<TLoginResponse> ValidatePasswordAsync<TLoginRequest, TLoginResponse>(TUser user, TLoginRequest dto)
        where TLoginRequest : class, ILoginRequest where TLoginResponse : ILoginResponse, new()
    {
        return await ValidatePasswordAsync<TLoginRequest, TLoginResponse, TokenResponse>(user, dto);
    }

    /// <summary>
    /// Validates if the password is valid for the given user
    /// </summary>
    /// <typeparam name="TLoginRequest">Any concrete <see langword="type"/> derived from <see cref="ILoginRequest"/></typeparam>
    /// <typeparam name="TLoginResponse">Any concrete <see langword="type"/> derived from <see cref="ILoginResponse"/></typeparam>
    /// <param name="user">An instance of <typeparamref name="TUser"/></param>
    /// <param name="dto">A concrete <see langword="type"/> instance of <typeparamref name="TLoginRequest"/></param>
    /// <returns>A concrete <see langword="type"/> instance that matches <typeparamref name="TLoginResponse"/></returns>
    /// <exception cref="ObjectDisposedException">Thrown when the <see cref="_userManager"/> has already been disposed</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="user"/> is <see langword="null"/></exception>
    protected virtual async Task<TLoginResponse> ValidatePasswordAsync<TLoginRequest, TLoginResponse, TTokenResponse>(TUser user, TLoginRequest dto)
        where TLoginRequest : class, ILoginRequest where TLoginResponse : ILoginResponse<TTokenResponse>, new() where TTokenResponse : class, ITokenResponse, new()
    {
        SignInResult signInResult = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, true);
        TLoginResponse response = new TLoginResponse();

        if (signInResult.Succeeded)
        {
            await _userManager.ResetAccessFailedCountAsync(user);

            response = new TLoginResponse { Success = true };

            if (_jwtService.UseBearerToken)
            {
                var bearerToken = _jwtService.GenerateAccessToken<TTokenResponse>(user, AccessTokenExpiration(dto.RememberMe));
                response.Tokens.Add(bearerToken);
            }

            if (_jwtService.UseRefreshToken)
            {
                var refreshToken = _jwtService.GenerateRefreshToken<TTokenResponse>();
                response.Tokens.Add(refreshToken);
            }

            return response;
        }

        response.NotAllowed = signInResult.IsNotAllowed;
        response.LockedOut = signInResult.IsLockedOut;

        return response;
    }

    /// <summary>
    /// A <see langword="protected"/> method for determining how long until the access token expires
    /// </summary>
    /// <remarks>
    /// <see langword="override"/> this method to customize the possible expiration dates
    /// </remarks>
    /// <param name="rememberMe">The value of the <c>Remember Me</c> checkbox on login</param>
    /// <returns>The <see cref="DateTime"/> representation of a new Access Token's expiration date</returns>
    protected virtual DateTime AccessTokenExpiration(bool rememberMe) => rememberMe ? DateTime.UtcNow.AddDays(30) : DateTime.UtcNow.AddMinutes(30);
}
