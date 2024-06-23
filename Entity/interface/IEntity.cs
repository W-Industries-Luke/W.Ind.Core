namespace W.Ind.Core.Entity;

public interface IEntity : IEntity<long>;

/// <summary>
/// Base <see langword="inteface"/> implemented by non-join entity types to establish the type of its Primary Key
/// </summary>
/// <typeparam name="TKey">The data type of its Primary Key</typeparam>
public interface IEntity<TKey> where TKey : struct, IEquatable<TKey>
{
    /// <summary>
    /// Defines an implicit Primary Key with a <see langword="type"/> that matches <typeparamref name="TKey"/>
    /// </summary>
    TKey Id { get; set; }
}