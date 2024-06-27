namespace W.Ind.Core.Entity;

public class GuidKeyUser : GuidKeyUser<GuidKeyUser>, ISoftDelete, IAuditableGuidKey, IGuidKey;

public class GuidKeyUser<TUser> : AuditUserBase<Guid, TUser>, ISoftDelete, IAuditableGuidKey<TUser>, IGuidKey
    where TUser : UserBase<Guid>
{
    public bool IsDeleted { get; set; }
}
