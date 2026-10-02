namespace W.Ind.Core.Dto;

public interface ILoginResponse : ILoginResponse<TokenResponse>;

public interface ILoginResponse<TTokenResponse>
{
    /// <summary>
    /// Stores a list of different types of tokens to be returned on login success
    /// </summary>
    List<TTokenResponse> Tokens { get; set; }

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
