namespace W.Ind.Core.Dto;

/// <summary>
/// Base DTO <see langword="interface"/> for Login Requests
/// </summary>
/// <remarks>
/// Deriving from this <see langword="interface"/> allows you to pass more data into various Login methods
/// </remarks>

public interface ILoginRequest
{
    /// <summary>
    /// Login UserName (or email)
    /// </summary>
    string UserName { get; set; }

    /// <summary>
    /// Login Password
    /// </summary>
    string Password { get; set; }

    /// <summary>
    /// Determines how long until the response token expires
    /// </summary>
    bool RememberMe { get; set; }
}
