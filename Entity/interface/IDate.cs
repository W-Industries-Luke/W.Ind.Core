namespace W.Ind.Core.Entity;

/// <summary>
/// A base <see langword="interface"/> used to identify any derived <see langword="interface"/> via Generic&lt;<see cref="Type"/>&gt; expression
/// </summary>
/// <remarks>
/// <para>
/// Derive from this <see langword="interface"/> to reference its specific <see cref="DateTime"/> properties through generic helper/service methods
/// </para>
/// <para>
/// See <see cref="UtcDateHelper.SetUtcIfDataExists{TType}(IDate)"/> for an example
/// </para>
/// </remarks>
public interface IDate { }
