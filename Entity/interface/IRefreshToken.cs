using System.ComponentModel.DataAnnotations;

namespace W.Ind.Core.Entity;

public interface IRefreshToken : IRefreshToken<CoreUser>, IEntity;

public interface IRefreshToken<TUser> : IRefreshToken<long, TUser>, IEntity
    where TUser : UserBase;

public interface IRefreshToken<TKey, TUser> : IRefreshToken<TKey, TUser, TKey>, IEntity<TKey>
    where TKey : struct, IEquatable<TKey> where TUser : UserBase<TKey>;

public interface IRefreshToken<TKey, TUser, TUserKey> : IEntity<TKey>
    where TKey : struct, IEquatable<TKey> where TUser : UserBase<TUserKey> where TUserKey : struct, IEquatable<TUserKey>
{
    /// <summary>
    /// The <see langword="string"/> token value
    /// </summary>
    [Required]
    public string Token { get; set; }

    /// <summary>
    /// Refresh token expiration date
    /// </summary>
    [Required]
    public DateTime Expires { get; set; }

    /// <summary>
    /// The Foreign Key of the User entity
    /// </summary>
    [Required]
    public TUserKey UserId { get; set; }

    /// <summary>
    /// The Navigation property pointing towards the User entity
    /// </summary>
    public TUser? User { get; set; }
}
