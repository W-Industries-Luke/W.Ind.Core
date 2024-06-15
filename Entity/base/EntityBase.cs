using System.ComponentModel.DataAnnotations;

namespace W.Ind.Core.Entity;

/// <summary>
/// The base <see langword="abstract"/> <see langword="class"/> derived from <see cref="IEntity{TKey}"/> and implemented by all non-join entities
/// </summary>
/// <remarks>
/// Defined to specify the [<see cref="KeyAttribute"/>] on the <see cref="Id"/> Primary Key property
/// </remarks>
/// <typeparam name="TKey">The data type of its Primary Key</typeparam>
public abstract class EntityBase<TKey> : IEntity<TKey> where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Defines an implicit Primary Key that matches type <typeparamref name="TKey"/>
    /// </summary>
    [Key]
    public TKey Id { get; set; }
}
