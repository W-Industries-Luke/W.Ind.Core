---
paths:
  - "**/*.cs"
---

# C# style

## Files and formatting

- UTF-8 with BOM, CRLF line endings, four-space indentation.
- File-scoped namespaces.
- Allman braces. Every `if`, `foreach` and `else` body has braces, including one-line bodies.
- `using` directives sit above the namespace. `Microsoft.EntityFrameworkCore`, `W.Ind.Core.Entity`, `W.Ind.Core.Helper` and `W.Ind.Core.Service` are global usings declared in the csproj, so files don't repeat them.
- Nullable reference types and implicit usings are enabled.

## Naming

- Interfaces: `I` prefix (`IJwtService`, `IAuditable`).
- Abstract base classes: `Base` suffix (`UserBase`, `RepositoryBase`, `JwtServiceBase`).
- Default concrete entities: `Core` prefix (`CoreUser`, `CoreRole`, `CoreRefreshToken`).
- `Guid`-keyed wrappers: `GuidKey` prefix (`GuidKeyUser`, `GuidKeyRefreshToken`).
- Static classes of helpers and extension methods: `Helper` suffix (`ContextHelper`, `TokenHelper`).
- Configuration POCOs: `Config` suffix (`JwtConfig`, `TemporalConfig`).
- DTOs carry no suffix (`LoginRequest`, `LoginResponse`, `TokenResponse`).
- Fields: `_camelCase`.
- Async methods: `Async` suffix.
- Generic type parameters: descriptive `T` names (`TKey`, `TUser`, `TEntity`, `TConfig`).

## Members

- Injected dependencies are stored in `protected readonly` fields so derived classes can use them.
- Constructors are explicit. A constructor that only forwards to the base goes on one line: `public JwtService(JwtConfig jwtConfig) : base(jwtConfig) { }`.
- Instance methods on services, repositories and middleware are `public virtual` or `protected virtual` so consumers can override them.
- Repository operations come in sync and async pairs (`Create` / `CreateAsync`).
- Extension methods on `IServiceCollection` and `EntityTypeBuilder<T>` return the builder so calls chain.
- String constants are grouped as `public const string` members of a `struct` in the `Enum` namespace (`CoreClaimTypes`).

## Language usage

- Use `String` for static members (`String.Empty`, `String.IsNullOrWhiteSpace`) and `string` for declarations.
- Initialise string properties with `String.Empty`.
- Locals use `var` or the explicit type; both appear.
- Guard clauses are one line: `if (x == null) { throw new ArgumentNullException(nameof(x)); }`.
- Null checks use `== null` and `!= null`.
- Use `DateTime.UtcNow`, never `DateTime.Now`.
- Throw BCL exception types: `ArgumentNullException`, `ArgumentException`, `InvalidOperationException`, `FormatException`.
- Mark superseded members `[Obsolete("...")]` and name the replacement in the message.
