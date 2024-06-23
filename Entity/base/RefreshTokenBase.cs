using System.ComponentModel.DataAnnotations;

namespace W.Ind.Core.Entity;

/// <summary>
/// Defines the <see langword="default"/> Refresh Token entity who's PK is <see langword="type"/> <see cref="long"/> and references the default <see cref="W.Ind.Core.Entity.User"/> entity
/// </summary>
/// <remarks>
/// See <see cref="RefreshTokenBase{TKey, TUser, TUserKey}"/> for full implementation
/// </remarks>
public abstract class RefreshTokenBase 
    : RefreshTokenBase<User>;

/// <summary>
/// Defines a refresh token entity <see langword="class"/> whose PK <see langword="type"/> is generic
/// </summary>
/// <remarks>
/// <para>
/// Refresh Token will use default <see cref="W.Ind.Core.Entity.User"/> class for navigation
/// </para>
/// </remarks>
/// <typeparam name="TKey">The Primary Key <see langword="type"/> of this entity</typeparam>
public abstract class RefreshTokenBase<TUser> 
    : RefreshTokenBase<long, TUser> where TUser : UserBase<long>;

/// <summary>
/// Defines a refresh token entity <see langword="class"/> where the User entity's PK <see langword="type"/> is <see cref="long"/>
/// </summary>
/// <remarks>
/// <para>
/// Use this class signature when your Refresh Token has a different PK <see langword="type"/> than your custom <typeparamref name="TUser"/> entity
/// </para>
/// <para>
/// <c>Note:</c> <typeparamref name="TUser"/>'s PK <see langword="type"/> MUST be the default (<see cref="long"/>)
/// </para>
/// </remarks>
/// <typeparam name="TKey">The Primary Key <see langword="type"/> of your Refresh Token entity</typeparam>
/// <typeparam name="TUser">The <see langword="type"/> of your User entity (PK <see langword="type"/> of <see cref="long"/>)</typeparam>
public abstract class RefreshTokenBase<TKey, TUser>
    : RefreshTokenBase<TKey, TUser, TKey>, IEntity<TKey> 
    where TKey: struct, IEquatable<TKey> where TUser : UserBase<TKey>;

/// <summary>
/// Defines a refresh token entity <see langword="class"/> where the Refresh Token's Primary Key <see langword="type"/> doesn't match your User entity's Primary key <see langword="type"/>
/// </summary>
/// <remarks>
/// <para>
/// Use this class signature when you have a custom User entity whose PK <see langword="type"/> is not the default (<see cref="long"/>) AND your RefreshToken's PK <see langword="type"/> is not the same as the User entity
/// </para>
/// </remarks>
/// <typeparam name="TKey">The Primary Key <see langword="type"/> of the Refresh Token entity</typeparam>
/// <typeparam name="TUser">The <see langword="type"/> of the User entity (Derives from <see cref="UserBase{TKey}"/>)</typeparam>
/// <typeparam name="TUserKey">The Primary Key <see langword="type"/> of the User entity</typeparam>
public abstract class RefreshTokenBase<TKey, TUser, TUserKey> 
    : EntityBase<TKey>, IEntity<TKey> 
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
    /// The Foreign Key of the User entity
    /// </summary>
    [Required]
    public TKey UserId { get; set; }
    
    /// <summary>
    /// The Navigation property pointing towards the User entity
    /// </summary>
    public TUser? User { get; set; }
}