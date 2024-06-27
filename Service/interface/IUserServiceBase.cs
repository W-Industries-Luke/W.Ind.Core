using Microsoft.AspNetCore.Identity;
using W.Ind.Core.Dto;

namespace W.Ind.Core.Service;

public interface IUserServiceBase : IUserServiceBase<CoreUser>;

public interface IUserServiceBase<TUser> : IUserServiceBase<long, TUser> where TUser : UserBase, new();

/// <summary>
/// Base <see langword="interface"/> implemented by <see langword="abstract"/> <see langword="class"/> <see cref="UserServiceBase{TUser,TKey}"/>
/// </summary>
/// <typeparam name="TUser">
/// <para>The CLR <see langword="type"/> corresponding to your User entity/table</para> 
/// <para>Ensure this <see langword="type"/> derives from <see cref="UserBase{TKey}"/></para>
/// </typeparam>
/// <typeparam name="TKey">
/// The data type of <typeparamref name="TUser"/>'s Primary Key
/// </typeparam>
public interface IUserServiceBase<TKey, TUser> 
    where TUser : UserBase<TKey>, new() where TKey : struct, IEquatable<TKey>
{
    Task<IdentityResult> RegisterAsync(UserRegistration dto);

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
    /// <returns>An insance of <see cref="Microsoft.AspNetCore.Identity.IdentityResult"/> from the <see cref="Microsoft.AspNetCore.Identity.UserManager{TUser}.CreateAsync(TUser, string)"/> method</returns>
    /// <exception cref="ArgumentNullException">Thrown when either the Email or Password is <see langword="null"/></exception>
    /// <exception cref="ObjectDisposedException">Thrown when <see cref="Microsoft.AspNetCore.Identity.UserManager{TUser}"/> instance has already been disposed</exception>
    /// <exception cref="System.ComponentModel.DataAnnotations.ValidationException">Thrown when either the Email or UserName is taken</exception>
    Task<IdentityResult> RegisterAsync<TUserRegistration>(TUserRegistration dto) 
        where TUserRegistration : class, IUserRegistration;

    Task<LoginResponse> ValidateLoginAsync(LoginRequest dto);

    Task<TLoginResponse> ValidateLoginAsync<TLoginRequest, TLoginResponse>(TLoginRequest dto)
        where TLoginRequest : class, ILoginRequest where TLoginResponse : ILoginResponse<TokenResponse>, new();

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
    /// <exception cref="ObjectDisposedException">Thrown when <see cref="Microsoft.AspNetCore.Identity.UserManager{TUser}"/> instance has already been disposed</exception>
    /// <exception cref="System.ComponentModel.DataAnnotations.ValidationException">Thrown when the user has been locked out</exception>
    Task<TLoginResponse> ValidateLoginAsync<TLoginRequest, TLoginResponse, TTokenResponse>(TLoginRequest dto)
        where TLoginRequest : class, ILoginRequest where TLoginResponse : ILoginResponse<TTokenResponse>, new() where TTokenResponse : class, ITokenResponse, new();
}