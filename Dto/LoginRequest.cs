namespace W.Ind.Core.Dto;

/// <summary>
/// Concrete DTO <see langword="class"/> for Login Requests
/// </summary>
/// <remarks>
/// Deriving from this <see langword="class"/> allows you to pass more data into various Login methods
/// </remarks>
public class LoginRequest : ILoginRequest
{
    /// <summary>
    /// Login UserName (or email)
    /// </summary>
    public required string UserName { get; set; }

    /// <summary>
    /// Login Password
    /// </summary>
    public required string Password { get; set; }

    /// <summary>
    /// Determines how long until the response token expires
    /// </summary>
    public bool RememberMe { get; set; }
}
