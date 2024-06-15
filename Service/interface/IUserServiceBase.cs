using W.Ind.Core.Dto;

namespace W.Ind.Core.Service;

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
public interface IUserServiceBase<TUser, TKey> where TUser : UserBase<TKey>, new() where TKey : IEquatable<TKey>
{
    /// <summary>
    /// An <see langword="async"/> method to validate if <paramref name="email"/> is unique to the system
    /// </summary>
    /// <remarks>
    /// Will <see langword="throw"/> <see cref="System.ComponentModel.DataAnnotations.ValidationException"/> or <see cref="ArgumentNullException"/> on fail
    /// </remarks>
    /// <param name="email">The email value to validate</param>
    /// <returns>Treat as <see langword="void"/></returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="email"/> is <see langword="null"/></exception>
    /// <exception cref="System.ComponentModel.DataAnnotations.ValidationException">Thrown when <paramref name="email"/> is in use</exception>
    Task EnsureUniqueEmailAsync(string email);

    /// <summary>
    /// An <see langword="async"/> method to validate if <paramref name="name"/> is unique to the system
    /// </summary>
    /// <remarks>
    /// Will <see langword="throw"/> <see cref="System.ComponentModel.DataAnnotations.ValidationException"/> on fail
    /// </remarks>
    /// <param name="name">The UserName value to validate</param>
    /// <returns>Treat as <see langword="void"/></returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is <see langword="null"/></exception>
    /// <exception cref="System.ComponentModel.DataAnnotations.ValidationException">Thrown when <paramref name="name"/> is in use</exception>
    Task EnsureUniqueNameAsync(string name);

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
    Task<Microsoft.AspNetCore.Identity.IdentityResult> RegisterAsync<TUserRegistration>(TUserRegistration dto) where TUserRegistration : class, IUserRegistration;

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
    Task<TLoginResponse> ValidateLoginAsync<TLoginRequest, TLoginResponse>(TLoginRequest dto)
        where TLoginRequest : class, ILoginRequest where TLoginResponse : class, ILoginResponse, new();
}
