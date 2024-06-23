using W.Ind.Core.Enum;

namespace W.Ind.Core.Dto;

public interface ITokenResponse
{
    TokenTypes TokenType { get; set; }
    string Token { get; set; }
    DateTime? Expires { get; set; }
}
