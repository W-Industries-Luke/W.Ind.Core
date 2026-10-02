namespace W.Ind.Core.Dto;

/// <summary>
/// Concrete DTO <see langword="class"/> containing response data from Login
/// </summary>
/// <remarks>
/// <para>
/// Implements <see cref="ILoginResponse"/>
/// </para>
/// <para>
/// Generic type parameter defaulted to <see cref="TokenResponse"/>
/// </para>
/// </remarks>
public class LoginResponse : LoginResponse<TokenResponse>, ILoginResponse, ILoginResponse<TokenResponse> 
{
    public static LoginResponse FromGenericType(LoginResponse<TokenResponse> dto)
    {
        return new LoginResponse { Success = dto.Success, LockedOut = dto.LockedOut, NotAllowed = dto.NotAllowed, NotFound = dto.NotFound, Tokens = dto.Tokens };
    }
}

/// <summary>
/// Concrete DTO <see langword="class"/> containing response data from Login
/// </summary>
/// <typeparam name="TTokenResponse">Implements <see cref="ITokenResponse"/> for returning different types of tokens generically</typeparam>
public class LoginResponse<TTokenResponse> : ILoginResponse<TTokenResponse> 
    where TTokenResponse : ITokenResponse, new()
{
    /// <summary>
    /// Stores a list of different types of tokens to be returned on login success
    /// </summary>
    public List<TTokenResponse> Tokens { get; set; } = new List<TTokenResponse>();

    /// <summary>
    /// Indicates whether or not the JSON Web Token was successfully generated
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Indicates whether or not the User is currently locked out
    /// </summary>
    public bool LockedOut { get; set; }

    /// <summary>
    /// Indicates whether or not the User is allow to login
    /// </summary>
    public bool NotAllowed { get; set; }

    /// <summary>
    /// Indicates whether or not a User with the given UserName/Email was found
    /// </summary>
    public bool NotFound { get; set; }
}