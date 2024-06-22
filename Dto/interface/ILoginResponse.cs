namespace W.Ind.Core.Dto;

public interface ILoginResponse : ILoginResponse<TokenResponse>;

/// <summary>
/// Base DTO <see langword="interface"/> containing response data from Login
/// </summary>
/// <remarks>
/// Deriving from this <see langword="interface"/> allows you to return more data from a Login
/// </remarks>
public interface ILoginResponse<TTokenResponse> where TTokenResponse : ITokenResponse
{
    /// <summary>
    /// JWT Access Token data
    /// </summary>
    TTokenResponse? AccessToken { get; set; }

    /// <summary>
    /// JWT Refresh Token data
    /// </summary>
    TTokenResponse? RefreshToken { get; set; }

    /// <summary>
    /// Indicates whether or not the JSON Web Token was successfully generated
    /// </summary>
    bool Success { get; set; }

    /// <summary>
    /// Indicates whether or not the User is currently locked out
    /// </summary>
    bool LockedOut { get; set; }

    bool NotAllowed { get; set; }

    /// <summary>
    /// Indicates whether or not the login email/username exists on a User record
    /// </summary>
    bool NotFound { get; set; }
}
