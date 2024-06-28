using W.Ind.Core.Dto;

namespace W.Ind.Core.Repository;

public abstract class RefreshTokenRepositoryBase : RefreshTokenRepositoryBase<CoreRefreshToken> 
{
    public RefreshTokenRepositoryBase(DbContext context, IJwtService jwtService) : base(context, jwtService) { }
}

public abstract class RefreshTokenRepositoryBase<TRefreshToken> : RefreshTokenRepositoryBase<TRefreshToken, IJwtService>
    where TRefreshToken : class, IRefreshToken, new()
{
    public RefreshTokenRepositoryBase(DbContext context, IJwtService jwtService) : base(context, jwtService) { }
}

public abstract class RefreshTokenRepositoryBase<TRefreshToken, TJwtService> : RefreshTokenRepositoryBase<TRefreshToken, TJwtService, CoreUser>
    where TRefreshToken : class, IRefreshToken, new() where TJwtService : IJwtService
{
    public RefreshTokenRepositoryBase(DbContext context, TJwtService jwtService) : base(context, jwtService) { }
}

public abstract class RefreshTokenRepositoryBase<TRefreshToken, TJwtService, TUser> : RefreshTokenRepositoryBase<TRefreshToken, long, TJwtService, TUser>
    where TRefreshToken : class, IRefreshToken<TUser>, new() where TUser : UserBase, new()
    where TJwtService : IJwtService<TUser>
{
    public RefreshTokenRepositoryBase(DbContext context, TJwtService jwtService) : base(context, jwtService) { }
}

public abstract class RefreshTokenRepositoryBase<TRefreshToken, TKey, TJwtService, TUser> : RefreshTokenRepositoryBase<TRefreshToken, TKey, TJwtService, TUser, TKey>
    where TRefreshToken : class, IRefreshToken<TKey, TUser>, new() where TKey : struct, IEquatable<TKey> where TUser : UserBase<TKey>, new()
    where TJwtService : IJwtService<TKey, TUser>
{
    public RefreshTokenRepositoryBase(DbContext context, TJwtService jwtService) : base(context, jwtService) { }
}

public abstract class RefreshTokenRepositoryBase<TRefreshToken, TKey, TJwtService, TUser, TUserKey> 
    : RepositoryBase<TRefreshToken, TKey>, IRefreshTokenRepository<TRefreshToken, TKey, TUser, TUserKey>
    where TRefreshToken : class, IRefreshToken<TKey, TUser, TUserKey>, new() where TKey : struct, IEquatable<TKey> where TUserKey : struct, IEquatable<TUserKey> where TUser : UserBase<TUserKey>, new()
    where TJwtService : IJwtService<TUserKey, TUser>
{
    protected readonly TJwtService _jwtService;

    public RefreshTokenRepositoryBase(DbContext context, TJwtService jwtService) : base(context) 
    {
        _jwtService = jwtService;
    }

    public virtual TokenResponse Generate(string accessToken)
    {
        return Generate<TokenResponse>(accessToken);
    }

    public virtual TTokenResponse Generate<TTokenResponse>(string accessToken)
        where TTokenResponse : ITokenResponse, new()
    {
        var userId = _jwtService.GetUserIdFromToken(accessToken);
        return Generate<TTokenResponse>(userId);
    }

    public virtual async Task<TokenResponse> GenerateAsync(string accessToken)
    {
        return await GenerateAsync<TokenResponse>(accessToken);
    }

    public virtual async Task<TTokenResponse> GenerateAsync<TTokenResponse>(string accessToken)
        where TTokenResponse: ITokenResponse, new()
    {
        var userId = _jwtService.GetUserIdFromToken(accessToken);
        return await GenerateAsync<TTokenResponse>(userId);
    }

    public virtual TokenResponse Generate(TUserKey userId)
    {
        return Generate<TokenResponse>(userId);
    }

    public virtual TTokenResponse Generate<TTokenResponse>(TUserKey userId)
        where TTokenResponse : ITokenResponse, new()
    {
        var record = BuildToken(userId);
        record = Create(record, true);

        return GetTokenResponse<TTokenResponse>(record);
    }

    public virtual async Task<TokenResponse> GenerateAsync(TUserKey userId)
    {
        return await GenerateAsync<TokenResponse>(userId);
    }

    public virtual async Task<TTokenResponse> GenerateAsync<TTokenResponse>(TUserKey userId)
        where TTokenResponse: ITokenResponse, new()
    {
        var record = BuildToken(userId);
        record = await CreateAsync(record, true);

        return GetTokenResponse<TTokenResponse>(record);
    }

    public virtual TokenResponse Refresh(string token)
    {
        return Refresh<TokenResponse>(token);
    }

    public virtual TTokenResponse Refresh<TTokenResponse>(string token) 
        where TTokenResponse : ITokenResponse, new()
    {
        var record = Get(record => record.Token == token).FirstOrDefault();

        if (record != null)
        {
            var newToken = BuildToken(record.UserId);

            InvalidateToken(token);
            Create(newToken, true);

            return GetTokenResponse<TTokenResponse>(newToken);
        }
        else
        {
            throw new InvalidOperationException("Invalid Refresh Token");
        }
    }

    public virtual async Task<TokenResponse> RefreshAsync(string token)
    {
        return await RefreshAsync<TokenResponse>(token);
    }

    public virtual async Task<TTokenResponse> RefreshAsync<TTokenResponse>(string token) where TTokenResponse : ITokenResponse, new()
    {
        var existing = await GetAsync(record => record.Token == token);

        if (existing.Any())
        {
            var record = existing.First();
            var newToken = BuildToken(record.UserId);

            InvalidateToken(token);
            Create(newToken, true);

            return GetTokenResponse<TTokenResponse>(newToken);
        }
        else
        {
            throw new InvalidOperationException("Invalid Refresh Token");
        }
    }

    protected virtual TRefreshToken BuildToken(TUserKey userId)
    {
        return new TRefreshToken
        {
            Token = Guid.NewGuid().ToString(),
            Expires = DateTime.UtcNow.AddDays(7),
            UserId = userId
        };
    }

    protected virtual void InvalidateToken(string token)
    {
        var existing = Get(entity => entity.Token == token).FirstOrDefault();

        if (existing != null)
        {
            Delete(existing.Id);
        }
    }

    protected virtual TTokenResponse GetTokenResponse<TTokenResponse>(TRefreshToken record)
        where TTokenResponse : ITokenResponse, new()
    {
        return new TTokenResponse
        {
            TokenType = Enum.CoreTokenTypes.Refresh,
            Expires = record.Expires,
            Token = record.Token
        };
    }
}
