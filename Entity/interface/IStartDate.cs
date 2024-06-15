using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace W.Ind.Core.Entity;

/// <summary>
/// An <see langword="interface"/> derived from <see cref="IDate"/> that specifies any implementing entity will have an <see cref="StartDate"/> property
/// </summary>
/// <remarks>
/// <para>
/// Implement this on all entities with a <c><see langword="public"/> <see cref="DateTime"/> StartDate</c> property
/// </para>
/// </remarks>
public interface IStartDate: IDate
{
    /// <summary>
    /// A <see cref="DateTime"/>? property representing an <c>StartDate</c> table column
    /// </summary>
    /// <remarks>
    /// <para>
    /// While nullable, you can still make this field required to EF via:
    /// </para>
    /// <para>
    /// The [<see cref="RequiredAttribute"/>], or
    /// </para>
    /// <para>
    /// Within your <see cref="DbContext.OnModelCreating(ModelBuilder)"/> method
    /// </para>
    /// </remarks>
    DateTime? StartDate { get; set; }
}
