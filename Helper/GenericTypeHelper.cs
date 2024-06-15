namespace W.Ind.Core.Helper;

/// <summary>
/// This <see langword="static"/> helper <see langword="class"/> is used to help identify generic types
/// </summary>
public static class GenericTypeHelper
{
    private static readonly Type[] IntegerTypes = {
        typeof(byte), typeof(sbyte),
        typeof(short), typeof(ushort),
        typeof(int), typeof(uint),
        typeof(long), typeof(ulong)
    };

    /// <summary>
    /// Determines if <see langword="typeof"/>(<typeparamref name="T"/>) is integer based
    /// </summary>
    /// <remarks>Meaning that it can only ever be a whole number value</remarks>
    /// <typeparam name="T">The generic <see langword="type"/> to check against</typeparam>
    /// <returns>
    /// <para><see langword="true"/>, if <typeparamref name="T"/> is integer based</para>
    /// <para><see langword="false"/>, if <typeparamref name="T"/> is not integer based</para>
    /// </returns>
    public static bool IsIntegerType<T>()
    {
        return IntegerTypes.Contains(typeof(T));
    }

    /// <summary>
    /// Determines if <paramref name="type"/> is integer based
    /// </summary>
    /// <remarks>Meaning that it can only ever be a whole number value</remarks>
    /// <returns>
    /// <para><see langword="true"/>, if <paramref name="type"/> is integer based</para>
    /// <para><see langword="false"/>, if <paramref name="type"/> is not integer based</para>
    /// </returns>
    public static bool IsIntegerType(Type type)
    {
        return IntegerTypes.Contains(type);
    }
}
