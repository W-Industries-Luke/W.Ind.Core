namespace W.Ind.Core.Repository;

public class GuidKeyRefreshTokenRepositoryBase : GuidKeyRefreshTokenRepositoryBase<GuidKeyRefreshToken>
{
    public GuidKeyRefreshTokenRepositoryBase(DbContext context, IJwtService jwtService) : base(context, jwtService) { }
}

public class GuidKeyRefreshTokenRepositoryBase<TRefreshToken> : GuidKeyRefreshTokenRepositoryBase<TRefreshToken, IJwtService>
    where TRefreshToken : class, IRefreshToken<Guid, CoreUser, long>, new()
{
    public GuidKeyRefreshTokenRepositoryBase(DbContext context, IJwtService jwtService) : base(context, jwtService) { }
}

public class GuidKeyRefreshTokenRepositoryBase<TRefreshToken, TJwtService> : GuidKeyRefreshTokenRepositoryBase<TRefreshToken, TJwtService, CoreUser>
    where TRefreshToken : class, IRefreshToken<Guid, CoreUser, long>, new()
    where TJwtService : IJwtService
{
    public GuidKeyRefreshTokenRepositoryBase(DbContext context, TJwtService jwtService) : base(context, jwtService) { }
}

public class GuidKeyRefreshTokenRepositoryBase<TRefreshToken, TJwtService, TUser> : GuidKeyRefreshTokenRepositoryBase<TRefreshToken, TJwtService, TUser, long>
    where TRefreshToken : class, IRefreshToken<Guid, TUser, long>, new() where TUser : UserBase, new()
    where TJwtService : IJwtService<TUser>
{
    public GuidKeyRefreshTokenRepositoryBase(DbContext context, TJwtService jwtService) : base(context, jwtService) { }
}

public class GuidKeyRefreshTokenRepositoryBase<TRefreshToken, TJwtService, TUser, TUserKey> : RefreshTokenRepositoryBase<TRefreshToken, Guid, TJwtService, TUser, TUserKey>
    where TRefreshToken : class, IRefreshToken<Guid, TUser, TUserKey>, new() where TUser : UserBase<TUserKey>, new() where TUserKey : struct, IEquatable<TUserKey>
    where TJwtService : IJwtService<TUserKey, TUser>
{
    public GuidKeyRefreshTokenRepositoryBase(DbContext context, TJwtService jwtService) : base(context, jwtService) { }
}
