namespace W.Ind.Core.Dto;

public class TokenResponse : ITokenResponse
{
    public string Token { get; set; } = String.Empty;
    public DateTime? Expires { get; set; } = null;
}
