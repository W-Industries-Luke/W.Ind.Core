---
paths:
  - "**/*.cs"
---

# C# style

`.editorconfig` at the repo root is the source of truth for formatting and the style preferences below. This file covers what it can't express. Where existing code predates a rule, the rule applies to new and touched code; don't reformat untouched files in the same change.

## Files and formatting

- File-scoped namespaces, `using` directives above the namespace, four-space indentation.
- Allman braces. Every `if`, `foreach` and `else` body has braces.
- Nullable reference types and implicit usings stay enabled. Don't silence nullable warnings with `!`; fix the nullability.
- `Microsoft.EntityFrameworkCore`, `W.Ind.Core.Entity`, `W.Ind.Core.Helper` and `W.Ind.Core.Service` are global usings declared in the csproj, so files don't repeat them.
- Leave each file's existing encoding and line endings alone.

## Naming

- Interfaces: `I` prefix. Abstract base classes: `Base` suffix. Async methods: `Async` suffix.
- Default concrete entities: `Core` prefix (`CoreUser`). `Guid`-keyed wrappers: `GuidKey` prefix.
- Configuration classes: `Config` suffix (`JwtConfig`). DTOs carry no suffix (`LoginRequest`).
- Generic type parameters: descriptive `T` names (`TKey`, `TUser`, `TEntity`).
- Static classes of extension methods: name them for what they extend (`ServiceCollectionExtensions`, `EntityTypeBuilderExtensions`). The existing `*Helper` classes keep their names because they are public API.
- Private fields: `_camelCase`.

## Members

- Constructors on public, inheritable types stay explicit. Primary constructors are fine on sealed or internal types.
- State that derived classes need is exposed as a `protected` get-only property, not a field. Existing `protected readonly _field` members stay as they are until 2.0, because renaming them breaks consumers.
- Instance methods on services, repositories and middleware are `public virtual` or `protected virtual` so consumers can override them.
- Make a class `sealed` when it isn't meant to be derived from.
- Extension methods on `IServiceCollection` and `EntityTypeBuilder<T>` return the builder so calls chain.
- Groups of constants go in a `static class`. `CoreClaimTypes` is a struct today; leave it until 2.0.

## Async

- Every public async method takes `CancellationToken cancellationToken = default` as its last parameter and passes it to every call it awaits. Existing methods don't; add the parameter as an overload, since changing a signature is breaking.
- A method is async only if it awaits real I/O. Don't wrap synchronous work in `Task.FromResult` or `Task.Run` to make an `Async` twin.
- Don't block on async code with `.Result`, `.Wait()` or `.GetAwaiter().GetResult()`.
- `ConfigureAwait(false)` is not used: the package only runs under ASP.NET Core, which has no synchronization context.

## Language usage

- Use the `string` keyword everywhere, including static members: `string.Empty`, `string.IsNullOrWhiteSpace`.
- Use `var` when the type is obvious from the right-hand side, the explicit type otherwise.
- Null checks use `is null` and `is not null`.
- Guard arguments with the throw helpers: `ArgumentNullException.ThrowIfNull(x)`, `ArgumentException.ThrowIfNullOrWhiteSpace(x)`, `ArgumentOutOfRangeException.ThrowIfNegative(x)`.
- Use collection expressions (`[]`, `[a, b]`) for collection initialisation.
- Use `required` and `init` for values that must be set at construction, instead of defaulting to `string.Empty`.
- Get the current time from an injected `TimeProvider` (`timeProvider.GetUtcNow()`), so time-dependent code can be tested. Never use `DateTime.Now`. Existing code calls `DateTime.UtcNow` directly.

## Exceptions

- Throw the most specific BCL type: `ArgumentNullException`, `ArgumentException`, `InvalidOperationException`, `FormatException`.
- The first argument of `ArgumentNullException` and `ArgumentException` overloads that take one is the parameter name, via `nameof`, not a message.
- Never throw `NullReferenceException`, `InvalidCastException` or `Exception` yourself.
- Don't write `catch (Exception) { throw; }`. Catch only what you handle.
- A member that must be overridden is `abstract`, not `virtual` with a body that throws `NotImplementedException`.

## Deprecation

- Mark superseded members `[Obsolete("Use X instead.")]` and keep them working until the next major version.
