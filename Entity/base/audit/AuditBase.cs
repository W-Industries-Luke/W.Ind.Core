using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace W.Ind.Core.Entity;

public abstract class AuditBase 
    : AuditBase<User>, IAuditable;

public abstract class AuditBase<TUser> 
    : AuditBase<long, TUser>, IAuditable<TUser> 
    where TUser : UserBase<long>;

public abstract class AuditBase<TUserKey, TUser> 
    : IAuditable<TUserKey, TUser> 
    where TUserKey : IEquatable<TUserKey> where TUser : UserBase<TUserKey>
{
    /// Derived from <see cref="IAuditable"/>
    /// </summary>
    /// <remarks>
    /// Defined with the [<see cref="TimestampAttribute"/>] so there's no need to configure for each inheritting entity
    /// </remarks>
    [Timestamp]
    public byte[] Timestamp { get; set; }

    /// <summary>
    /// Derived from <see cref="IAuditable"/>
    /// </summary>
    /// <remarks>
    /// Defined with the [<see cref="RequiredAttribute"/>] and [<see cref="ForeignKeyAttribute"/>] so there's no need to configure for each inheritting entity
    /// </remarks>
    [Required]
    [ForeignKey(nameof(CreatedBy))]
    public TUserKey CreatedById { get; set; }

    /// <summary>
    /// Implemented from <see cref="IAuditable"/>
    /// </summary>
    /// <remarks>
    /// Defined with the [<see cref="DeleteBehaviorAttribute"/>] so there's no need to configure for each entity AND because its Foreign Key is <see langword="required"/>
    /// </remarks>
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public TUser? CreatedBy { get; set; }

    /// <summary>
    /// Implemented from <see cref="IAuditable"/>
    /// </summary>
    /// <remarks>
    /// Defined with the [<see cref="ForeignKeyAttribute"/>] so there's no need to configure for each entity
    /// </remarks>
    [ForeignKey(nameof(ModifiedBy))]
    public TUserKey? ModifiedById { get; set; }

    /// <summary>
    /// Implemented from <see cref="IAuditable"/>
    /// </summary>
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public TUser? ModifiedBy { get; set; }

    /// <summary>
    /// Implemented from <see cref="IAuditable"/>
    /// </summary>
    public DateTime CreatedOn { get; set; }

    /// <summary>
    /// Implemented from <see cref="IAuditable"/>
    /// </summary>
    public DateTime? ModifiedOn { get; set; }
}