using W.Ind.Core.Dto;

namespace W.Ind.Core.Repository;

public interface IRefreshTokenRepository : IRefreshTokenRepository<CoreRefreshToken>, IRepository<CoreRefreshToken>;

public interface IRefreshTokenRepository<TRefreshToken> : IRefreshTokenRepository<TRefreshToken, CoreUser>, IRepository<TRefreshToken>
    where TRefreshToken : class, IRefreshToken, IEntity, new();

public interface IRefreshTokenRepository<TRefreshToken, TUser> : IRefreshTokenRepository<TRefreshToken, long, TUser>, IRepository<TRefreshToken, long>
    where TRefreshToken : class, IRefreshToken<TUser>, IEntity, new() where TUser : UserBase;

public interface IRefreshTokenRepository<TRefreshToken, TKey, TUser> : IRefreshTokenRepository<TRefreshToken, TKey, TUser, TKey>, IRepository<TRefreshToken, TKey>
    where TRefreshToken : class, IRefreshToken<TKey, TUser>, IEntity<TKey>, new() where TKey : struct, IEquatable<TKey>
    where TUser : UserBase<TKey>;

public interface IRefreshTokenRepository<TRefreshToken, TKey, TUser, TUserKey> : IRepository<TRefreshToken, TKey>
    where TRefreshToken : class, IRefreshToken<TKey, TUser, TUserKey>, IEntity<TKey>, new() where TKey : struct, IEquatable<TKey>
    where TUser : UserBase<TUserKey> where TUserKey : struct, IEquatable<TUserKey>
{
    TokenResponse Refresh(string token);

    TTokenResponse Refresh<TTokenResponse>(string token)
        where TTokenResponse : ITokenResponse, new();

    Task<TokenResponse> RefreshAsync(string token);

    Task<TTokenResponse> RefreshAsync<TTokenResponse>(string token)
        where TTokenResponse : ITokenResponse, new();

    TokenResponse Generate(string accessToken);

    TTokenResponse Generate<TTokenResponse>(string accessToken)
        where TTokenResponse : ITokenResponse, new();

    TokenResponse Generate(TUserKey userId);

    TTokenResponse Generate<TTokenResponse>(TUserKey userId)
        where TTokenResponse : ITokenResponse, new();

    Task<TokenResponse> GenerateAsync(string accessToken);

    Task<TTokenResponse> GenerateAsync<TTokenResponse>(string accessToken)
        where TTokenResponse : ITokenResponse, new();

    Task<TokenResponse> GenerateAsync(TUserKey userId);

    Task<TTokenResponse> GenerateAsync<TTokenResponse>(TUserKey userId)
        where TTokenResponse : ITokenResponse, new();
}
