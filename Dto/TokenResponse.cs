using W.Ind.Core.Enum;

namespace W.Ind.Core.Dto;

/// <summary>
/// A concrete <see langword="class"/> for returning a token w/ expiration
/// </summary>
public class TokenResponse : ITokenResponse
{
    /// <summary>
    /// An <see langword="enum"/> indicating what type of token this is (bearer, refresh, etc)
    /// </summary>
    public CoreTokenTypes TokenType { get; set; }

    /// <summary>
    /// JWT <see cref="string"/> token value
    /// </summary>
    public string Token { get; set; } = String.Empty;

    /// <summary>
    /// Token expiration date (nullable in case no token was generate)
    /// </summary>
    public DateTime? Expires { get; set; } = null;
}
