namespace W.Ind.Core.Dto;

/// <summary>
/// Concrete DTO <see langword="class"/> containing response data from Login
/// </summary>
/// <remarks>
/// Deriving from this <see langword="class"/> allows you to return more data from a Login
/// </remarks>
public class LoginResponse : LoginResponse<TokenResponse>, ILoginResponse, ILoginResponse<TokenResponse> 
{
    public static LoginResponse FromGenericType(LoginResponse<TokenResponse> dto)
    {
        return new LoginResponse { Success = dto.Success, LockedOut = dto.LockedOut, NotAllowed = dto.NotAllowed, NotFound = dto.NotFound, Tokens = dto.Tokens };
    }
}

public class LoginResponse<TTokenResponse> : ILoginResponse<TTokenResponse> 
    where TTokenResponse : ITokenResponse, new()
{
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