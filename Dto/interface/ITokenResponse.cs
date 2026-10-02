using W.Ind.Core.Enum;

namespace W.Ind.Core.Dto;

/// <summary>
/// An <see langword="interface"/> that defines an individual token response
/// </summary>
public interface ITokenResponse
{
    /// <summary>
    /// An <see langword="enum"/> indicating what type of token this is (bearer, refresh, etc)
    /// </summary>
    CoreTokenTypes TokenType { get; set; }

    /// <summary>
    /// JWT <see cref="string"/> token value
    /// </summary>
    string Token { get; set; }

    /// <summary>
    /// Token expiration date (nullable in case no token was generate)
    /// </summary>
    DateTime? Expires { get; set; }
}
