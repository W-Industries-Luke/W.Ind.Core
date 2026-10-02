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
- Entities derive from a base class: `EntityBase`, `AuditEntityBase`, `UserBase`, `RoleBase`, `RefreshTokenBase`. Identity entities derive from the matching `Identity*<TKey>` type.
- Soft delete: implement `ISoftDelete` (`bool IsDeleted`).
- Auditing: implement `IAuditable<TKey, TUser>`: `CreatedOn`, `ModifiedOn`, `CreatedById`, `CreatedBy`, `ModifiedBy`, `Timestamp`.
  - Audit navigations are nullable and use `DeleteBehavior.NoAction`.
  - `Timestamp` is a `byte[]` row version.
  - `ModifiedById` is a shadow property. Refer to it through a single constant, never a repeated string literal.
- Collection navigations are `ICollection<T>` initialised to `[]`. Reference navigations are nullable unless the relationship is required and always loaded.
- Store instants as UTC. Prefer `DateTimeOffset` for new timestamp properties; the existing `DateTime` audit columns stay, since changing them alters consumers' schemas.

## Model configuration

- Mapping that belongs to the package goes in `IEntityTypeConfiguration<T>` classes under `Entity/config/` (like `AuditConfiguration`), with a chainable `EntityTypeBuilder<TEntity>` extension that applies it:

  ```cs
  builder.Entity<Contact>().ConfigureAudit().FilterDeleted();
  ```

- Use fluent configuration for relationships, keys, indexes and delete behaviour. Keep data annotations to validation-style attributes (`[Required]`, `[MaxLength]`). Mapping attributes on interface members (`[ForeignKey]`, `[Timestamp]` in `IAuditable`) are not read by EF from the interface, so don't add more; `AuditConfiguration` is what applies them.
- Don't add thin wrappers over EF's own relationship API. The existing `OneToOne`/`OneToMany`/`ManyToOne`/`ManyToMany` helpers stay, but new configuration uses `HasOne`/`HasMany` directly.
- Soft-delete filter: `FilterDeleted` adds `!IsDeleted`. When the package targets EF Core 10, give the filter a name (`HasQueryFilter("SoftDelete", ...)`) so consumers can add their own filters and disable this one selectively. On EF Core 8 an entity has one filter, and a second `HasQueryFilter` call replaces it.
- Seeding: `SeedFromJson` (`HasData`) is for static reference data only, because it is baked into migrations. Don't seed users or secrets with it. On EF Core 9 and later, use `UseSeeding`/`UseAsyncSeeding` for anything else.

## Temporal tables

- Period columns default to `SysStartTime` and `SysEndTime`; the history table is `{EntityName}-History`; `TemporalConfig` overrides the names.
- Temporal tables are SQL Server only. Anything provider-specific stays behind the SQL Server dependency and out of types a non-SQL Server consumer would need.

## Saving

Audit, soft-delete and UTC handling run when changes are saved.

- New work implements this as a `SaveChangesInterceptor` (override `SavingChanges` and `SavingChangesAsync`) that consumers register with `AddInterceptors`. One registration covers both the sync and async paths and every context.
- The existing `ChangeTracker` helpers (`HandleSoftDelete`, `HandleAudit`, `ParseUtcDates`) stay supported for consumers who override `SaveChanges` themselves. Such a context must override both `SaveChanges(bool)` and `SaveChangesAsync(bool, CancellationToken)`, or one path skips auditing.
- Take the current time from `TimeProvider` and the current user from an injected service. Set both once per save, not per entry.
- Find properties by interface and cast, not by reflection over property names, on the save path.

## Repositories

- `RepositoryBase<TEntity, TKey>` takes a `DbContext` and gives derived classes the context and the `DbSet`.
- Async methods take a `CancellationToken` and pass it to EF.
- Async methods return materialised results (`Task<TEntity?>`, `Task<List<TEntity>>`). Only synchronous query methods return `IQueryable<TEntity>`; there is nothing to await in building a query. `GetAsync` returning `Task<IQueryable<T>>` is legacy.
- Look up by key with `FindAsync` or `FirstOrDefaultAsync` and return `null` when missing, rather than letting `First` throw.
- Read-only queries use `AsNoTracking()`.
- Write operations take `bool saveChanges = false`; the caller decides when to save.
- New operations are async only. Don't add a synchronous twin.
