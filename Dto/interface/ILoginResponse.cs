namespace W.Ind.Core.Dto;

/// <summary>
/// Base DTO <see langword="interface"/> containing response data from Login
/// </summary>
/// <remarks>
/// Deriving from this <see langword="interface"/> allows you to return more data from a Login
/// </remarks>
public interface ILoginResponse
{
    /// <summary>
    /// JSON Web Token value
    /// </summary>
    string? Token { get; set; }

    /// <summary>
    /// JSON Web Token expiration date
    /// </summary>
    DateTime? Expires { get; set; }

    /// <summary>
    /// Indicates whether or not the JSON Web Token was successfully generated
    /// </summary>
    bool Success { get; set; }

    /// <summary>
    /// Indicates whether or not the User is currently locked out
    /// </summary>
    public bool LockedOut { get; set; }
}
