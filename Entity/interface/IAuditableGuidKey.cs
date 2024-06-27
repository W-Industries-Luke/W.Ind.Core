namespace W.Ind.Core.Entity;

public interface IAuditableGuidKey : IAuditableGuidKey<GuidKeyUser>;

public interface IAuditableGuidKey<TUser> : IAuditable<Guid, TUser>
    where TUser : UserBase<Guid>;