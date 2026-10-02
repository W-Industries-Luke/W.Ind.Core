# Git

## Remotes

- `origin`: GitHub, `W-Industries-Luke/W.Ind.Core`. Public; nuget.org links here.
- `devops`: Azure DevOps, `W-Industries/W.Ind.Core`. Work items and the original pipeline.
- The two are kept in step with sync branches and PRs ("Merge Devops master into GitHub master").

## Branches

- Long-lived promotion branches: `Dev` → `Review` → `Staging` → `master`. `Integration` and `Test` also exist on DevOps.
- Work branches are named for the work item: `19-Implement-Base-Repository`, `4-Update-Namespace`, `BUG-CoreUser-Table-Always-Created`. Title-Case words joined by hyphens.
- Changes reach `master` through a pull request.

## Commits

- Subject in the past tense: "Updated Copyright tag", "Added refresh token entity".
- Reference the issue or PR number in the subject: `Story #4 - Updated Namespace`, `Refactor done (#3)`.
- List several changes as `- ` bullets after the subject.
- Release commits point to the release notes: "View Release notes for v0.9.99 for full details".
