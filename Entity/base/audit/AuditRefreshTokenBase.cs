using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace W.Ind.Core.Entity;

public abstract class AuditRefreshTokenBase 
    : AuditRefreshTokenBase<User>, IAuditable, IEntity;

public abstract class AuditRefreshTokenBase<TUser> 
    : AuditRefreshTokenBase<long, TUser>, IAuditable<long, TUser>, IEntity
    where TUser : UserBase<long>;

public abstract class AuditRefreshTokenBase<TKey, TUser> 
    : AuditRefreshTokenBase<TKey, TUser, TKey>, IAuditable<TKey, TUser>, IEntity<TKey>
    where TKey : struct, IEquatable<TKey> where TUser : UserBase<TKey>;

public abstract class AuditRefreshTokenBase<TKey, TUser, TUserKey> 
    : AuditEntityBase<TKey, TUser, TUserKey>, IAuditable<TUserKey, TUser>, IEntity<TKey>
    where TKey : struct, IEquatable<TKey> where TUserKey : struct, IEquatable<TUserKey> where TUser : UserBase<TUserKey>
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
    /// The Foreign Key of your User entity
    /// </summary>
    [Required]
    [ForeignKey(nameof(User))]
    public TUserKey UserId { get; set; }

    /// <summary>
    /// The navigation property to the User via <see cref="UserId"/>
    /// </summary>
    public TUser? User { get; set; }
}