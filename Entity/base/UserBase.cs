using Microsoft.AspNetCore.Identity;

namespace W.Ind.Core.Entity;

/// <summary>
/// An <see langword="abstract"/> <see langword="class"/> that inherits from <see cref="IdentityUser{TKey}"/> 
/// and <see cref="IEntity{TKey}"/> in order to define its own inheritable properties and reference generically
/// </summary>
/// <remarks>
/// Extend this <see langword="class"/> to add custom columns to your <c>Users</c> table
/// </remarks>
/// <typeparam name="TKey">The data type of its Primary Key</typeparam>
public abstract class UserBase<TKey> : IdentityUser<TKey>, IEntity<TKey> where TKey : IEquatable<TKey> { }
