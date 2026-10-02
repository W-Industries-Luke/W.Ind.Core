---
paths:
  - "Entity/**"
  - "Repository/**"
  - "Helper/ContextHelper.cs"
  - "Helper/UtcDateHelper.cs"
---

# EF Core

## Entities

- Every entity implements `IEntity<TKey>` (an `Id` property). The default key type is `long`.
- Entities derive from a base class: `EntityBase`, `AuditEntityBase`, `UserBase`, `RoleBase`, `RefreshTokenBase`.
- Identity entities derive from the matching `Identity*<TKey>` type.
- Soft delete: implement `ISoftDelete` (`bool IsDeleted`).
- Auditing: implement `IAuditable<TKey, TUser>`: `CreatedOn`, `ModifiedOn`, `CreatedById`, `CreatedBy`, `ModifiedBy`, `Timestamp`.
  - `ModifiedById` is a shadow property, referred to by the string `"ModifiedById"`.
  - Audit navigations are nullable and use `DeleteBehavior.NoAction`.
  - `Timestamp` is a `byte[]` row version.
- Date handling: implement `IStartDate`, `IEndDate` or `IEnterredDate` (all derive from `IDate`) to have `DateTime` properties converted to UTC on save.
- Join entities implement `IJoinTable`.
- Mapping uses data annotations on the entity (`[Required]`, `[ForeignKey]`, `[Timestamp]`, `[DeleteBehavior]`) plus the fluent helpers below.

## Model configuration

`ContextHelper` holds extension methods on `EntityTypeBuilder<TEntity>` that return the builder so they chain:

```cs
builder.Entity<Contact>().ConfigureAudit().FilterDeleted()
    .OneToMany(c => c.Reference, c => c.HasForeignKey(fk => fk.ReferenceId));
```

- `ConfigureAudit` applies `AuditConfiguration` and makes the table temporal.
- `FilterDeleted` adds the `!IsDeleted` query filter.
- `OneToOne`, `OneToMany`, `ManyToOne`, `ManyToMany`, `CompositeKey`, `BuildIndexes` wrap the relationship and key builders.
- `SeedFromJson` seeds with `HasData` from a JSON file.

## Temporal tables

- Period columns default to `SysStartTime` and `SysEndTime`.
- The history table is `{EntityName}-History`.
- `TemporalConfig` overrides the names.
- The provider is SQL Server.

## Saving

Consuming contexts override `SaveChanges` and `SaveChangesAsync` and call the `ChangeTracker` helpers before `base.SaveChanges`:

- `HandleSoftDelete` turns a delete of an `ISoftDelete` entity into an update that sets `IsDeleted`.
- `HandleAudit` sets `CreatedOn`/`CreatedById` on insert and `ModifiedOn`/`ModifiedById` on update, using `DateTime.UtcNow` and the current user id from `IUserService`.
- `ParseUtcDates` converts `IDate` properties to UTC.

## Repositories

- `RepositoryBase<TEntity, TKey>` takes a `DbContext` and exposes `_context` and `_dbSet` to derived classes.
- Operations: `Get(predicate)`, `GetById`, `Create`, `Update`, `Delete`, each with an `Async` twin.
- `Get` returns an un-enumerated `IQueryable<TEntity>`.
- Write operations take `bool saveChanges = false`; the caller decides when to save.
