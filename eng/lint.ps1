<#
.SYNOPSIS
    Deterministic lint for W.Ind.Core. Implements the checkable rules in .claude/rules/.

.DESCRIPTION
    Builds the project with the analyzers configured in .editorconfig and fails when:
      - the build has an error (this includes API compatibility breaks against the last release),
      - a dependency has a known vulnerability,
      - an analyzer warning sits on a line this change adds or modifies,
      - one of the repo-level checks below fails (WIC0001 to WIC0006).

    Warnings on lines the change does not touch are counted and reported, but do not fail.
    The mapping from each rule to its check is in .claude/rules/linting.md.

    Runs on Windows PowerShell 5.1 and PowerShell 7.

.PARAMETER Base
    The ref the change will merge into. Changed lines are everything since the merge base
    with this ref, including uncommitted and untracked work.

.PARAMETER Branch
    Branch name to check. Defaults to the current branch.

.PARAMETER PrTitle
    Pull request title to check. It becomes the commit subject when the PR is squash-merged.

.PARAMETER SkipGit
    Skip the branch name, commit subject and PR title checks.

.EXAMPLE
    powershell -File eng/lint.ps1
#>
[CmdletBinding()]
param(
    [string]$Base = 'origin/master',
    [string]$Branch = '',
    [string]$PrTitle = '',
    [switch]$SkipGit
)

$ErrorActionPreference = 'Stop'

$RootNamespace = 'W.Ind.Core'
$BranchPattern = '^(feat|fix|docs|chore|refactor|test|ci|release|maintenance)/[a-z0-9][a-z0-9.-]*$'
$SubjectMaxLength = 72
$PastTenseWords = @(
    'Added', 'Updated', 'Fixed', 'Removed', 'Changed', 'Renamed', 'Moved', 'Created', 'Deleted',
    'Implemented', 'Refactored', 'Merged', 'Reworked', 'Bumped', 'Cleaned', 'Improved', 'Corrected'
)

$failures = New-Object System.Collections.Generic.List[object]

function Add-Failure([string]$File, [int]$Line, [string]$Id, [string]$Message) {
    $script:failures.Add([pscustomobject]@{ File = $File; Line = $Line; Id = $Id; Message = $Message })
}

function Get-RelativePath([string]$Path, [string]$Root) {
    $normalized = $Path.Trim().Replace('\', '/')
    $rootPrefix = $Root.TrimEnd('/') + '/'
    if ($normalized.ToLowerInvariant().StartsWith($rootPrefix.ToLowerInvariant())) {
        return $normalized.Substring($rootPrefix.Length)
    }
    return $normalized
}

# Removes string literals and line comments so text checks only see code.
function Get-CodeText([string]$Text) {
    $code = [regex]::Replace($Text, '"(\\.|[^"\\])*"', '""')
    return [regex]::Replace($code, '//.*$', '')
}

function Test-Subject([string]$Subject, [string]$Id, [string]$What) {
    if ($Subject -match '^Merge ') { return }
    if ($Subject.Length -gt $SubjectMaxLength) {
        Add-Failure '' 0 $Id "$What is $($Subject.Length) characters; the limit is $SubjectMaxLength`: '$Subject'"
    }
    if ($Subject.TrimEnd().EndsWith('.')) {
        Add-Failure '' 0 $Id "$What ends with a full stop: '$Subject'"
    }
    if ($Subject -match '^\s*[-*]\s') {
        Add-Failure '' 0 $Id "$What starts with a bullet: '$Subject'"
    }
    $firstWord = ($Subject.Trim() -split '\s+')[0]
    if ($firstWord -cnotmatch '^[A-Z]') {
        Add-Failure '' 0 $Id "$What does not start with a capital letter: '$Subject'"
    }
    if ($PastTenseWords -contains $firstWord) {
        Add-Failure '' 0 $Id "$What is not in the imperative mood ('$firstWord'): '$Subject'"
    }
}

# ---- Locate the repo and the change ----

$root = (& git rev-parse --show-toplevel)
if ($LASTEXITCODE -ne 0 -or -not $root) { Write-Host 'lint: not inside a git repository'; exit 2 }
$root = $root.Trim().Replace('\', '/')
Set-Location $root

$mergeBase = (& git merge-base $Base HEAD)
if ($LASTEXITCODE -ne 0 -or -not $mergeBase) { Write-Host "lint: cannot find a merge base with '$Base'"; exit 2 }
$mergeBase = $mergeBase.Trim()

# changedLines[file] = set of line numbers; addedText = every added line with its text
$changedLines = @{}
$addedText = New-Object System.Collections.Generic.List[object]

function Add-ChangedLine([string]$File, [int]$Number, [string]$Text) {
    $key = $File.ToLowerInvariant()
    if (-not $script:changedLines.ContainsKey($key)) {
        $script:changedLines[$key] = New-Object 'System.Collections.Generic.HashSet[int]'
    }
    [void]$script:changedLines[$key].Add($Number)
    $script:addedText.Add([pscustomobject]@{ File = $File; Line = $Number; Text = $Text })
}

$currentFile = $null
$lineNumber = 0
foreach ($diffLine in (& git -c core.quotepath=off diff --no-color --unified=0 $mergeBase --)) {
    if ($diffLine -match '^\+\+\+ (?:b/(.+)|/dev/null)$') {
        $currentFile = $Matches[1]
    }
    elseif ($diffLine -match '^@@ -\d+(?:,\d+)? \+(\d+)(?:,\d+)? @@') {
        $lineNumber = [int]$Matches[1]
    }
    elseif ($currentFile -and $diffLine.StartsWith('+') -and -not $diffLine.StartsWith('+++')) {
        Add-ChangedLine $currentFile $lineNumber $diffLine.Substring(1)
        $lineNumber++
    }
}

foreach ($untracked in (& git -c core.quotepath=off ls-files --others --exclude-standard)) {
    if (-not $untracked) { continue }
    $n = 1
    foreach ($text in (Get-Content -LiteralPath $untracked)) {
        Add-ChangedLine $untracked $n $text
        $n++
    }
}

# ---- Build with analyzers ----

Write-Host 'lint: building with analyzers...'
$buildOutput = & dotnet build -c Release --no-incremental --nologo -v:q -tl:off
$buildExit = $LASTEXITCODE

$located = '^\s*(?<file>.+?)\((?<line>\d+),(?<col>\d+)\): (?<sev>warning|error) (?<id>[A-Za-z]+\d+): (?<msg>.*?)(?: \[[^\]]+\])?$'
$unlocated = '^\s*(?<file>[^(:]+?)\s*: (?<sev>warning|error) (?<id>[A-Za-z]+\d+): (?<msg>.*?)(?: \[[^\]]+\])?$'
$seen = New-Object 'System.Collections.Generic.HashSet[string]'
$existing = @{}
$buildErrors = 0

foreach ($outputLine in $buildOutput) {
    $text = [string]$outputLine
    $match = [regex]::Match($text, $located)
    $hasLocation = $match.Success
    if (-not $hasLocation) { $match = [regex]::Match($text, $unlocated) }
    if (-not $match.Success) { continue }

    $file = Get-RelativePath $match.Groups['file'].Value $root
    $line = 0
    if ($hasLocation) { $line = [int]$match.Groups['line'].Value }
    $id = $match.Groups['id'].Value
    $severity = $match.Groups['sev'].Value
    $message = $match.Groups['msg'].Value

    if (-not $seen.Add("$file|$line|$id|$message")) { continue }

    if ($severity -eq 'error') {
        $buildErrors++
        Add-Failure $file $line $id $message
    }
    elseif ($id -match '^NU19\d\d$') {
        Add-Failure $file $line $id $message
    }
    elseif ($changedLines.ContainsKey($file.ToLowerInvariant()) -and $changedLines[$file.ToLowerInvariant()].Contains($line)) {
        Add-Failure $file $line $id $message
    }
    else {
        if (-not $existing.ContainsKey($id)) { $existing[$id] = 0 }
        $existing[$id]++
    }
}

if ($buildExit -ne 0 -and $buildErrors -eq 0) {
    Add-Failure '' 0 'BUILD' "dotnet build exited with code $buildExit"
}

# ---- WIC0001: string keyword, not String (IDE0049 is not reported on build) ----

foreach ($added in $addedText) {
    if (-not $added.File.EndsWith('.cs')) { continue }
    if ((Get-CodeText $added.Text) -cmatch '\bString\b') {
        Add-Failure $added.File $added.Line 'WIC0001' "Use the 'string' keyword instead of 'String'"
    }
}

# ---- WIC0002: namespace is W.Ind.Core.<top-level folder> ----

$sourceFiles = @(& git -c core.quotepath=off ls-files --cached --others --exclude-standard -- '*.cs')
foreach ($sourceFile in $sourceFiles) {
    if (-not $sourceFile -or -not (Test-Path -LiteralPath $sourceFile)) { continue }
    $segments = $sourceFile.Split('/')
    if ($segments.Length -lt 2) { continue }
    if ($segments[0] -eq 'tests' -or $segments[0] -eq 'eng') { continue }

    $expected = "$RootNamespace.$($segments[0])"
    $n = 0
    foreach ($text in (Get-Content -LiteralPath $sourceFile)) {
        $n++
        if ($text -match '^\s*namespace\s+([\w.]+)') {
            if ($Matches[1] -cne $expected) {
                Add-Failure $sourceFile $n 'WIC0002' "Namespace is '$($Matches[1])'; files under '$($segments[0])/' belong in '$expected'"
            }
            break
        }
    }
}

# ---- WIC0003: no ASP.NET Core 2.x packages ----

$n = 0
foreach ($text in (Get-Content -LiteralPath "$RootNamespace.csproj")) {
    $n++
    if ($text -match '<PackageReference\s+Include="(Microsoft\.AspNetCore\.[^"]+)"\s+Version="2\.') {
        Add-Failure "$RootNamespace.csproj" $n 'WIC0003' "'$($Matches[1])' is a legacy ASP.NET Core 2.x package; use the Microsoft.AspNetCore.App framework reference"
    }
}

# ---- WIC0004 to WIC0006: branch name, commit subjects, PR title ----

if (-not $SkipGit) {
    if (-not $Branch) { $Branch = (& git rev-parse --abbrev-ref HEAD).Trim() }
    if ($Branch -ne 'HEAD' -and $Branch -ne 'master' -and $Branch -cnotmatch $BranchPattern) {
        Add-Failure '' 0 'WIC0004' "Branch '$Branch' does not match <type>/<short-description> in lowercase (types: feat, fix, docs, chore, refactor, test, ci, release, maintenance)"
    }

    foreach ($subject in (& git log --format=%s "$mergeBase..HEAD")) {
        if ($subject) { Test-Subject $subject 'WIC0005' 'Commit subject' }
    }

    if ($PrTitle) { Test-Subject $PrTitle 'WIC0006' 'PR title' }
}

# ---- Report ----

$existingTotal = 0
foreach ($count in $existing.Values) { $existingTotal += $count }
if ($existingTotal -gt 0) {
    $byId = ($existing.Keys | Sort-Object | ForEach-Object { "$_ x$($existing[$_])" }) -join ', '
    Write-Host "lint: $existingTotal existing warnings on lines this change does not touch (not failing): $byId"
}

if ($failures.Count -eq 0) {
    Write-Host 'lint: passed'
    exit 0
}

foreach ($failure in $failures) {
    $location = ''
    if ($failure.File -and $failure.Line -gt 0) { $location = "$($failure.File)($($failure.Line)): " }
    elseif ($failure.File) { $location = "$($failure.File): " }
    Write-Host "$location$($failure.Id): $($failure.Message)"

    if ($env:GITHUB_ACTIONS -eq 'true') {
        $annotation = '::error'
        if ($failure.File -and $failure.Line -gt 0) { $annotation += " file=$($failure.File),line=$($failure.Line),title=$($failure.Id)" }
        elseif ($failure.File) { $annotation += " file=$($failure.File),title=$($failure.Id)" }
        else { $annotation += " title=$($failure.Id)" }
        Write-Host "$annotation::$($failure.Message)"
    }
}

Write-Host "lint: failed with $($failures.Count) problem(s)"
exit 1
