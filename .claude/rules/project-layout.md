# Project layout

## Folders and namespaces

- One project, `W.Ind.Core.csproj`, at the repo root.
- Top-level folders are PascalCase and map to namespaces: `Config`, `Dto`, `Entity`, `Enum`, `Helper`, `Middleware`, `Repository`, `Service`.
- Subfolders are lowercase and do not add a namespace segment. A file in `Entity/base/audit/` is in `W.Ind.Core.Entity`.
- Subfolder names:
  - `base`: abstract base classes
  - `interface`: interfaces
  - `audit`: auditable variants of base entities
  - `config`: `IEntityTypeConfiguration` classes
  - `scoped`, `singleton`: service implementations, by DI lifetime
- Concrete default types sit directly in the top-level folder (`Entity/CoreUser.cs`).

## The generic overload ladder

Most types exist as a family that differs by number of type parameters. Each shorter form inherits from the next longer one and fills in a default; only the longest form has a body.

```cs
public class JwtService : JwtService<CoreUser>, IJwtService { ... }

public class JwtService<TUser> : JwtService<long, TUser>, IJwtService<TUser>
    where TUser : UserBase, new() { ... }

public class JwtService<TKey, TUser> : JwtService<TKey, TUser, JwtConfig>, IJwtService<TKey, TUser>
    where TKey : struct, IEquatable<TKey> where TUser : UserBase<TKey>, new() { ... }

public class JwtService<TKey, TUser, TConfig> : JwtServiceBase<TKey, TUser, TConfig>, IJwtService<TKey, TUser, TConfig>
    where TUser : UserBase<TKey>, new() where TKey : struct, IEquatable<TKey> where TConfig : JwtConfig
{
    // implementation
}
```

- Defaults: key type `long`, user type `CoreUser`, config type `JwtConfig`.
- Type parameter order: `TKey`, then `TUser`, then anything else.
- Key constraint: `where TKey : struct, IEquatable<TKey>`. User constraint: `where TUser : UserBase<TKey>, new()`.
- All forms of a family live in one file, shortest first.
- A form with no members of its own ends in `;` rather than `{ }`.
- Constraints go on their own indented line under the declaration.
- Interfaces mirror the ladder of the classes that implement them.
- `ConfigurationHelper.ConfigureWicServices` has one overload per rung and registers the matching service forms.

## Dependency injection

- `ConfigureWicServices` registers everything the package needs.
- Per-request services are `AddScoped`. Shared state (`IJwtInvalidator`, the config instance) is `TryAddSingleton`.
- Services are registered against their interface.
