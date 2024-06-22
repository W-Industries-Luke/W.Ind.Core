namespace W.Ind.Core.Dto;

public interface ITokenResponse
{
    string Token { get; set; }
    DateTime? Expires { get; set; }
}
