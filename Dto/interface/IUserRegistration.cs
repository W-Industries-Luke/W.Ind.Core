namespace W.Ind.Core.Dto;

/// <summary>
/// Concrete DTO <see langword="interface"/> for passing data on Register
/// </summary>
/// <remarks>
/// Derive from this <see langword="interface"/> to pass more custom data to the Register method
/// </remarks>
public interface IUserRegistration
{
    /// <summary>
    /// The desired UserName value
    /// </summary>
    string UserName { get; set; }

    /// <summary>
    /// The desired Password value
    /// </summary>
    string Password { get; set; }

    /// <summary>
    /// Enterred email value
    /// </summary>
    string Email { get; set; }
}
