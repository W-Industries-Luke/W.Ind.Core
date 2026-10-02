---
paths:
  - "W.Ind.Core.csproj"
  - "nuget/**"
  - "README.md"
---

# Packaging

- Target framework: `net8.0`.
- The package is produced on every build (`GeneratePackageOnBuild`), with a `.snupkg` symbols package and embedded source (`IncludeSymbols`, `IncludeSource`).
- `GenerateDocumentationFile` is on; the XML file ships in the package.
- ASP.NET Core types come from `<FrameworkReference Include="Microsoft.AspNetCore.App" />`. EF Core and Identity.EntityFrameworkCore are package references pinned to one version.
- Package metadata lives in the csproj: `PackageId`, `Version`, `Authors`, `Company`, `Description`, `PackageTags`, `PackageProjectUrl`, `RepositoryUrl`, `Copyright`.
- Licence: `MIT`, via `PackageLicenseExpression`. `LICENSE.txt` is at the repo root.
- Two READMEs:
  - `README.md` at the root is for GitHub and GitHub Pages.
  - `nuget/README.md` is packed as the package README. Keep the two in step.
- Version: set `<Version>` in the csproj by hand for each release.
- Release notes: `PackageReleaseNotes` in the csproj. Add a `vX.Y.Z` heading and `- ` bullets at the top; older versions stay below.
- Deprecate a bad release on nuget.org rather than unlisting it.
