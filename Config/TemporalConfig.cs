namespace W.Ind.Core.Config;

/// <summary>
/// A DTO <see langword="class"/> that holds customizaton options for temporal tables
/// </summary>
public class TemporalConfig
{
    /// <summary>
    /// Gets or sets a custom name for the history table
    /// </summary>
    /// <value>Custom name for the history table</value>
    /// <remarks>
    /// <para>The resulting table name will be appended with <c>"-History"</c>.</para>
    /// <para>This value will default to <c><see langword="typeof"/>(Entity).<see cref="System.Reflection.MemberInfo.Name">Name</see></c></para>
    /// </remarks>
    public String HistoryTableName { get; set; } = String.Empty;

    /// <summary>
    /// Gets or sets a custom column name for the temporal table's period start time.
    /// </summary>
    /// <value>Custom name for temporal table's period start column.</value>
    /// <remarks>
    /// <para>Default <c>"SysStartTime"</c>.</para>
    /// </remarks>
    public String PeriodStartName { get; set; } = "SysStartTime";

    /// <summary>
    /// Gets or sets a custom column name for the temporal table's period end time.
    /// </summary>
    /// <value>Custom name for temporal table's period end column.</value>
    /// <remarks>
    /// <para>Default: <c>"SysEndTime"</c></para>
    /// </remarks>
    public String PeriodEndName { get; set; } = "SysEndTime";
}
