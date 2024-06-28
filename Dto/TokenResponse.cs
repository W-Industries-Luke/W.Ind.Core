using W.Ind.Core.Enum;

namespace W.Ind.Core.Dto;

public class TokenResponse : ITokenResponse
{
    public CoreTokenTypes TokenType { get; set; }
    public string Token { get; set; } = String.Empty;
    public DateTime? Expires { get; set; } = null;
}
