namespace W.Ind.Core.Dto;

public class LoginResponse : LoginResponse<TokenResponse>, ILoginResponse<TokenResponse>;

/// <summary>
/// Concrete DTO <see langword="class"/> containing response data from Login
/// </summary>
/// <remarks>
/// Deriving from this <see langword="class"/> allows you to return more data from a Login
/// </remarks>
public class LoginResponse<TTokenResponse>: ILoginResponse<TTokenResponse> where TTokenResponse : ITokenResponse, new()
{
    /// <summary>
    /// References a JWT Access Token value with expiration
    /// </summary>
    public TTokenResponse? AccessToken { get; set; }

    /// <summary>
    /// References a JWT Refresh Token value with expiration
    /// </summary>
    public TTokenResponse? RefreshToken { get; set; }

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
