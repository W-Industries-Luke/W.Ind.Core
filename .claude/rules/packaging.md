---
paths:
  - "W.Ind.Core.csproj"
  - "nuget/**"
  - "README.md"
  - "CHANGELOG.md"
  - ".github/workflows/**"
---

# Packaging

Some of this describes where the project is heading rather than where it is. See "Not yet in place" at the end before treating a gap as a violation.

## Targets and dependencies

- Target the .NET versions Microsoft currently supports, LTS first (`net8.0;net10.0` while both are in support). Drop a target only in a major version.
- ASP.NET Core types come from `<FrameworkReference Include="Microsoft.AspNetCore.App" />`. Never reference the `Microsoft.AspNetCore.*` 2.x packages.
- EF Core and Identity.EntityFrameworkCore are package references, conditioned per target framework so each target gets its matching major version.
- Reference the lowest patch that is free of known vulnerabilities, so consumers aren't forced to upgrade further than they need.
- `dotnet list package --vulnerable --include-transitive` must report nothing before a release. Keep NuGet audit on for transitive packages (`<NuGetAuditMode>all</NuGetAuditMode>`).
- Every new package dependency is one every consumer inherits. Don't add one for something the framework already does.

## Build settings

- `GenerateDocumentationFile` stays on; the XML file ships in the package.
- Symbols: `.snupkg` with Source Link (`PublishRepositoryUrl`, `EmbedUntrackedSources`, and `ContinuousIntegrationBuild` in CI). This replaces `IncludeSource`.
- API compatibility: `EnablePackageValidation` with `PackageValidationBaselineVersion` set to the last release. A minor or patch release must pass it with no suppressions.
- Pack explicitly with `dotnet pack -c Release` rather than on every build (`GeneratePackageOnBuild`).
- A change must not add compiler warnings. Don't add `NoWarn` to hide them. Once the existing backlog is cleared, CI builds with warnings as errors.

## Metadata and docs

- Package metadata lives in the csproj. Description and tags describe what the package does, without version numbers that go stale ("EF Core 8").
- Licence: `MIT` via `PackageLicenseExpression`; `LICENSE.txt` at the repo root.
- Two READMEs: `README.md` at the root is for GitHub and GitHub Pages; `nuget/README.md` is packed as the package README. Change both together. Code in either must compile against the current version.

## Versioning and releases

- Semantic Versioning. Patch: fixes, no API change. Minor: additions only. Major: anything that removes, renames or changes behaviour consumers rely on.
- Deprecate before removing: `[Obsolete]` in a minor release, removal in the next major.
- Release notes go in `CHANGELOG.md` (Keep a Changelog format) and the GitHub Release. `PackageReleaseNotes` holds the current version's notes or a link to the changelog, not the whole history. Until `CHANGELOG.md` exists, add the new version's notes at the top of `PackageReleaseNotes`.
- A release is a `vX.Y.Z` tag on `master`. The tag triggers the GitHub Actions workflow that builds, tests, packs and pushes to nuget.org. Don't push packages from a developer machine.
- Publish with nuget.org Trusted Publishing (`NuGet/login` exchanging the workflow's OIDC token for a short-lived key), not a long-lived API key in repo secrets.
- Deprecate a bad release on nuget.org and point to the fix; don't unlist it.

## Not yet in place

These rules are targets. Their absence in the repo today is not a defect in an unrelated change, but a change must not move further from them, and work that touches the area should adopt them.

- Multi-targeting (the project targets `net8.0` only).
- Source Link, package validation and explicit `dotnet pack` (the csproj still uses `IncludeSource` and `GeneratePackageOnBuild`).
- `CHANGELOG.md`.
- The GitHub Actions build and release workflows, and Trusted Publishing.
- A test project.
