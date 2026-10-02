# W.Ind.Core

NuGet package `W.Ind.Core`: base entities, services, repositories and middleware for ASP.NET Core apps that use EF Core (SQL Server) with `Microsoft.AspNetCore.Identity`. It provides temporal audit logging, soft deletes, UTC date parsing and JWT generation, validation and invalidation.

Everything public here is API that other projects compile against. Removing or renaming a public type or member, or changing a signature, is a breaking change.

## Commands

- Build and pack: `dotnet build -c Release` (packing happens on build; output in `bin/Release`)
- Lint: `powershell -File eng/lint.ps1` (what the pull request check runs; see `.claude/rules/linting.md`)
- Check dependencies: `dotnet list package --vulnerable --include-transitive`

There is no test project.

## Rules

The conventions for this repo are in `.claude/rules/`. Read the ones that apply before editing or reviewing.

- `csharp-style.md`: formatting, naming and language usage for all `.cs` files
- `project-layout.md`: folders, namespaces and the generic overload ladder
- `xml-docs.md`: XML documentation comments
- `ef-core.md`: entities, auditing, soft delete and model configuration
- `packaging.md`: the csproj, versioning and releases
- `git.md`: branches, commits and pull requests
- `linting.md`: which rules are enforced by `eng/lint.ps1`, and how to keep the two in step

## Applying the rules

This holds for writing code and for reviewing it:

- The rules describe how new and changed code should look. Apply them to the lines a change adds or modifies.
- Some existing code predates a rule, and each file says where. That code is not a violation. Don't rewrite it, or ask for it to be rewritten, as part of an unrelated change.
- Compatibility outranks style. Outside a major version, never remove, rename or change the signature or behaviour of a public or protected member to satisfy a rule. A change that does so is a defect, whatever its reason.
- `packaging.md` lists targets that are not in place yet. Their absence is not a violation.
- When two rules seem to conflict, the more specific file wins over `csharp-style.md`.
- The rule files are the source of truth for the lint configuration. A change to a rule updates its check and the table in `linting.md` in the same pull request.
