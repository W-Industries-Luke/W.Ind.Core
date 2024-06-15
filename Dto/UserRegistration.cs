namespace W.Ind.Core.Dto;

/// <summary>
/// Concrete DTO <see langword="class"/> for passing data on Register
/// </summary>
/// <remarks>
/// Derive from this <see langword="class"/> to pass more custom data to the Register method
/// </remarks>
public class UserRegistration : IUserRegistration
{
    /// <summary>
    /// The desired UserName value
    /// </summary>
    public string UserName { get; set; } = String.Empty;

    /// <summary>
    /// The desired Password value
    /// </summary>
    public required string Password { get; set; }

    /// <summary>
    /// Enterred email value
    /// </summary>
    public required string Email { get; set; }
}
