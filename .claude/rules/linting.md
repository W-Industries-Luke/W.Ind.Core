---
paths:
  - ".editorconfig"
  - "BannedSymbols.txt"
  - "eng/lint.ps1"
  - ".github/workflows/lint.yml"
  - ".claude/rules/**"
---

# Linting

The rules in this folder that a machine can check are enforced by `eng/lint.ps1`, which runs on every pull request (`.github/workflows/lint.yml`) and locally with `powershell -File eng/lint.ps1`.

## Keeping lint and rules in step

The rule files are the source of truth. The lint configuration is derived from them.

- When a rule in this folder is added, changed or removed, update its check in the same pull request and update the table below.
- When a rule can't be checked mechanically, list it under "Not linted" so the gap is visible.
- Never add a lint check that has no rule behind it. Write the rule first.
- Don't weaken a check (lower a severity, add a suppression or `NoWarn`) to get a change through. Change the rule, with its reasoning, or fix the code.

## How it decides

- Analyzer warnings fail a change only when they sit on a line the change adds or modifies. Warnings on untouched lines are counted and printed but don't fail, which is how "existing code predates the rule" is implemented.
- Build errors, API compatibility breaks, vulnerable dependencies and the repo-level `WIC` checks always fail.
- The .NET SDK is pinned by `global.json` so the analyzers behave the same locally and in CI.

## Where each check lives

| Mechanism | File |
|---|---|
| Roslyn analyzer severities and options | `.editorconfig` |
| Banned APIs (`RS0030`) | `BannedSymbols.txt` |
| Analyzer packages, package validation, NuGet audit | `W.Ind.Core.csproj` |
| Changed-line gate and `WIC` checks | `eng/lint.ps1` |

## Rule to check

### csharp-style.md

| Rule | Check |
|---|---|
| File-scoped namespaces | `IDE0161` |
| `using` directives above the namespace | `IDE0065` |
| Braces on every block | `IDE0011` |
| Interfaces start with `I`; type parameters start with `T`; private fields are `_camelCase`; async methods end in `Async` | `IDE1006` (naming rules in `.editorconfig`) |
| Protected state is a property, not a field | `CA1051` |
| Seal internal types | `CA1852` |
| `CancellationToken` is the last parameter | `CA1068` |
| Pass the `CancellationToken` on | `CA2016` |
| Don't block on async code | `RS0030` (`Task.Result`, `Task.Wait`, `GetResult` banned) |
| `string` keyword, not `String` | `WIC0001` |
| `var` when the type is apparent, explicit type otherwise | `IDE0007`, `IDE0008` |
| `is null` / `is not null` | `RCS1248`, `IDE0041` |
| Throw helpers for argument guards | `CA1510`, `CA1511`, `CA1512`, `CA1513` |
| Collection expressions | `IDE0028`, `IDE0300` to `IDE0305` |
| Time comes from `TimeProvider` | `RS0030` (`DateTime.Now`, `UtcNow`, `Today` and `DateTimeOffset.Now`, `UtcNow` banned) |
| Don't throw `Exception`, `NullReferenceException` | `CA2201` |
| Argument exceptions get the parameter name in the right place | `CA2208` |

### xml-docs.md

| Rule | Check |
|---|---|
| Every public and protected member is documented | `CS1591` |
| Tags are well formed and match the signature | `CS1570`, `CS1572`, `CS1573`, `CS1574`, `CS1711`, `CS1712`, `CS1735` |

### project-layout.md

| Rule | Check |
|---|---|
| Namespace is `W.Ind.Core.<top-level folder>` | `WIC0002` |

### packaging.md

| Rule | Check |
|---|---|
| No removed or changed public API in a minor or patch release | Package validation against `PackageValidationBaselineVersion` (`CP` errors) |
| No dependencies with known vulnerabilities | NuGet audit, `NU1901` to `NU1904` |
| No `Microsoft.AspNetCore.*` 2.x packages | `WIC0003` |
| A change adds no compiler warnings | The changed-line gate |

### git.md

| Rule | Check |
|---|---|
| Branch is `<type>/<short-description>`, lowercase | `WIC0004` |
| Commit subject: 72 characters or fewer, capitalised, imperative, no full stop, no bullet | `WIC0005` |
| PR title follows the same rules, since it becomes the squash commit subject | `WIC0006` |

## Not linted

These need a person or a reviewer agent:

- **csharp-style.md:** new public async methods take a `CancellationToken`; no fake-async wrappers; no pointless `catch { throw; }`; `abstract` instead of a `virtual` method that throws; constants in a static class; no `!` to silence nullable warnings; naming suffixes and prefixes (`Base`, `Core`, `GuidKey`, `Config`).
- **xml-docs.md:** the quality of the text; `<inheritdoc>` on shorter ladder forms; plain code samples; spelling.
- **project-layout.md:** one public type per file; the shape of the generic ladder; `TryAdd` and options-pattern registration.
- **ef-core.md:** all of it.
- **packaging.md:** Semantic Versioning choices; the two READMEs changing together; release notes.
- **git.md:** one logical change per commit; the PR description.
- Imperative mood is checked only against a list of common past-tense first words (`Added`, `Updated`, `Fixed`, …).
