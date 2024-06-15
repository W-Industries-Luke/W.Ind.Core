namespace W.Ind.Core.Dto;

/// <summary>
/// Concrete DTO <see langword="class"/> containing response data from Login
/// </summary>
/// <remarks>
/// Deriving from this <see langword="class"/> allows you to return more data from a Login
/// </remarks>
public class LoginResponse: ILoginResponse
{
    /// <summary>
    /// JSON Web Token value
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    /// JSON Web Token expiration date
    /// </summary>
    public DateTime? Expires { get; set; }

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
}
