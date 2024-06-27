namespace W.Ind.Core.Entity;

public class GuidKeyRefreshToken : GuidKeyRefreshToken<CoreUser>;

public class GuidKeyRefreshToken<TUser> : GuidKeyRefreshToken<TUser, long>, IEntity<Guid>
    where TUser : UserBase;

public class GuidKeyRefreshToken<TUser, TUserKey> : RefreshTokenBase<Guid, TUser, TUserKey>
    where TUserKey : struct, IEquatable<TUserKey> where TUser : UserBase<TUserKey>
{ }
