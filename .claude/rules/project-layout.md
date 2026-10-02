# Project layout

## Folders and namespaces

- The package project, `W.Ind.Core.csproj`, is at the repo root. Tests go in `tests/W.Ind.Core.Tests/`. Because the root project compiles every `.cs` file beneath it, it must exclude that folder (`<Compile Remove="tests/**" />`).
- One public type per file, with the file named for the type. The generic ladder below is the only exception.
- Top-level folders are PascalCase and map to namespaces: `Config`, `Dto`, `Entity`, `Enum`, `Helper`, `Middleware`, `Repository`, `Service`.
- Namespaces stop at the top-level folder. A file in `Entity/base/audit/` is in `W.Ind.Core.Entity`. This is deliberate: consumers need one `using` per area, and the namespaces are public API. The folder/namespace analyzer (IDE0130) is turned off for this reason.
- Existing subfolders are lowercase (`base`, `interface`, `audit`, `config`, `scoped`, `singleton`). Keep using them; don't rename, and don't add a new top-level namespace without a major version.
- Concrete default types sit directly in the top-level folder (`Entity/CoreUser.cs`).

## The generic overload ladder

Most types exist as a family that differs by number of type parameters. Each shorter form inherits from the next longer one and fills in a default; only the longest form has a body. This is the package's core design: keep it for every new public type that depends on the user or key type.

```cs
public class JwtService : JwtService<CoreUser>, IJwtService { ... }

public class JwtService<TUser> : JwtService<long, TUser>, IJwtService<TUser>
    where TUser : UserBase, new() { ... }

public class JwtService<TKey, TUser> : JwtService<TKey, TUser, JwtConfig>, IJwtService<TKey, TUser>
    where TKey : struct, IEquatable<TKey>
    where TUser : UserBase<TKey>, new() { ... }

public class JwtService<TKey, TUser, TConfig> : JwtServiceBase<TKey, TUser, TConfig>, IJwtService<TKey, TUser, TConfig>
    where TKey : struct, IEquatable<TKey>
    where TUser : UserBase<TKey>, new()
    where TConfig : JwtConfig
{
    // implementation
}
```

- Defaults: key type `long`, user type `CoreUser`, config type `JwtConfig`.
- Type parameter order: `TKey`, then `TUser`, then anything else. `AuditConfiguration<TEntity, TUser, TUserKey>` predates this; don't copy its order.
- Constraints: one `where` clause per line, in the same order as the type parameters.
- Only require `new()` on a new type when the code calls `new T()`. Many existing constraints carry it without needing to; leave those alone.
- All forms of a family live in one file, shortest first.
- A form with no members of its own ends in `;` rather than `{ }`.
- Interfaces mirror the ladder of the classes that implement them.
- Add a rung only when it saves consumers a type argument they would otherwise always repeat. Each rung is public API that has to be kept working.

## Dependency injection

- One extension method on `IServiceCollection` registers everything the package needs, with one overload per rung. Name new registration methods `Add…` (the .NET convention). `ConfigureWicServices` keeps its name.
- Register against interfaces. Per-request services are scoped; shared state is singleton.
- Use `TryAdd…` for new registrations, so a consumer who registers their own implementation first wins. The existing `AddScoped` calls in `ConfigureWicServices` predate this.
- Bind configuration with the options pattern: `services.AddOptions<JwtConfig>().BindConfiguration("Jwt").ValidateDataAnnotations().ValidateOnStart()`, and inject `IOptions<JwtConfig>`. Existing services take the `JwtConfig` instance directly; moving them is a 2.0 change.
- Take dependencies through the constructor. Resolving from `HttpContext.RequestServices` is for middleware only, where the instance outlives the request.
