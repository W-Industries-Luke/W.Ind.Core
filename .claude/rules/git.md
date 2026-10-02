# Git

## Remotes

- `origin` (GitHub, `W-Industries-Luke/W.Ind.Core`) is the single source of truth. nuget.org links here, and issues, PRs and releases live here.
- `devops` (Azure DevOps) is a legacy mirror. Don't push to it or open sync PRs against it. Work that exists only there is brought over once and reviewed like any other change.

## Branches

- `master` is always releasable. There are no promotion branches; `Dev`, `Review`, `Staging`, `Integration` and `Test` are legacy and not used.
- Work happens on short-lived branches off `master`, named `<type>/<short-description>` in lowercase with hyphens: `fix/missing-auth-header-401`, `feat/save-changes-interceptor`, `chore/claude-rules`, `maintenance/1.0.1`.
- Changes reach `master` only through a pull request with CI passing. Squash-merge, so `master` has one commit per change.
- Delete the branch after it merges.

## Commits

- Subject: imperative mood, under about 70 characters, no trailing full stop: "Add refresh token repository", "Fix 500 on missing Authorization header".
- Body, when needed: what changed and why, wrapped at about 72 characters. Lists use `- ` bullets in the body, never in the subject.
- One logical change per commit. Don't commit work-in-progress checkpoints to a shared branch.
- Reference the issue in the body or PR (`Fixes #12`), not as a subject prefix.

## Pull requests

- The description says what changed, why, and how it was verified.
- Call out any public API change and whether it is breaking.
- A release PR updates the version, `CHANGELOG.md` and both READMEs together.

## Releases

- Tag the merge commit `vX.Y.Z` and create a GitHub Release from the changelog entry. See `packaging.md`.
