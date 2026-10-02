# W.Ind.Core

NuGet package `W.Ind.Core`: base entities, services, repositories and middleware for ASP.NET Core apps that use EF Core (SQL Server) with `Microsoft.AspNetCore.Identity`. It provides temporal audit logging, soft deletes, UTC date parsing and JWT generation, validation and invalidation.

Everything public here is API that other projects compile against. Removing or renaming a public type or member, or changing a signature, is a breaking change.

## Commands

- Build and pack: `dotnet build -c Release` (packing happens on build; output in `bin/Release`)
- Check dependencies: `dotnet list package --vulnerable --include-transitive`

There is no test project.

## Rules

The conventions for this repo are in `.claude/rules/`. Read the ones that apply before editing:

- `csharp-style.md`: formatting, naming and language usage for all `.cs` files
- `project-layout.md`: folders, namespaces and the generic overload ladder
- `xml-docs.md`: XML documentation comments
- `ef-core.md`: entities, auditing, soft delete and model configuration
- `packaging.md`: the csproj, versioning and releases
- `git.md`: branches, commits and pull requests
