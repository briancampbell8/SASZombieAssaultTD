<#
.SYNOPSIS
    ReadMeReview.ps1
    Manual, surgical maintenance script for README.md

.DESCRIPTION
    - Ensures canonical sections exist in README.md before the Appendixes
    - Updates existing sections if they drift
    - Removes misplaced or truncated versions
    - Prevents duplicates
    - Maintains a Milestone History section
    - Logs each run to Automation/ReadmeUpdateLog.md

.NOTES
    Run manually from the repo root:
        ./Automation/ReadMeReview.ps1
#>

param(
    [string]$ReadmePath = ".\README.md",
    [string]$LogPath    = ".\Automation\ReadmeUpdateLog.md"
)

# Ensure Automation directory exists for logs
$automationDir = Split-Path $LogPath -Parent
if (-not (Test-Path $automationDir)) {
    New-Item -ItemType Directory -Path $automationDir | Out-Null
}

if (-not (Test-Path $ReadmePath)) {
    Write-Host "README not found at path: $ReadmePath" -ForegroundColor Red
    exit 1
}

# Load README content
$readme = Get-Content -Path $ReadmePath -Raw

# Canonical section definitions
$Sections = [ordered]@{}

$Sections["DevelopmentPhilosophy"] = @"
## Development Philosophy: Build Upward

The engine for SAS: Zombie Assault TD is built on a strict, sequential workflow designed to prevent drift, ambiguity, and partial states. Every change follows a build-upward model: foundations are stabilized before new layers are added, and no step is skipped or reordered once defined.

Milestones are treated as hard boundaries. A phase is not considered complete until its structure, behavior, and automation are fully verified. This approach ensures that debugging happens at the correct layer, that regressions are easy to trace, and that the engine remains predictable as it grows.

Automation plays a central role in this philosophy. PowerShell tools are used to maintain structure, enforce naming and layout rules, and keep the project audit-friendly. Logs, appendixes, and structural snapshots exist so that future developers—and future versions of this project—can understand exactly how and why the engine evolved.

In short: the engine is not built through ad-hoc changes. It is built upward, one verified layer at a time.
"@

$Sections["FuturePlatformDirection"] = @"
## Future Platform Direction

The modern engine for SAS: Zombie Assault TD is intentionally designed to be platform-agnostic. While the current focus is on stabilizing the desktop version and ensuring the engine runs cleanly at the grassroots level, the long-term goal is to support WebAssembly (WASM) as a primary deployment target.

WASM allows the entire engine—gameplay logic, systems, timing, scenes, and core architecture—to run directly inside a web browser without rewriting the underlying code. Only the platform layer (rendering, input, and asset loading paths) needs to adapt, while the engine’s scaffolding, namespaces, and modular subsystems remain unchanged.

This approach ensures that once the engine is fully operational on desktop, it can be brought to the web smoothly and predictably. The project is being built with this transition in mind so that future changes, enhancements, and platform shifts can be made without requiring another full rewrite.

In short: get the plane flying on desktop first, then land it safely on the web—without rebuilding the plane.
"@

$Sections["CurrentEngineStatus"] = @"
## Current Engine Status

The engine is currently in the foundational stabilization phase, with a focus on getting the core systems running cleanly at the grassroots level.

- Phase 1 structural verification: **Complete**
  - Engine directory structure established
  - Core, systems, UI, and gameplay stubs populated
  - Legacy Flash-derived scaffolding removed in favor of the modern /Engine layout
- Namespace alignment: **Complete**
- Empty file population for required engine components: **Complete**
- PowerShell automation pipeline: **Operational**
  - ProjectStructure and PowerShellLog appendixes maintained
  - Engine scaffolding updates tracked and auditable

The next major milestone is DebugSession-0001, focused on:
- Launching the engine under F5
- Verifying GameLoop and initial scene initialization
- Observing runtime behavior and capturing the first baseline

Once the engine is running reliably at this level, subsequent sessions will target rendering, asset loading, and eventually the platform abstraction required for WASM.
"@

$Sections["MilestoneHistory"] = @"
## Milestone History

This section records major structural and architectural milestones for the modern SAS: Zombie Assault TD engine. Entries are added intentionally to reflect completed phases, not in-progress work.

- [2026-01-21] Engine scaffolding established for core, systems, UI, gameplay, and tools components.
- [2026-01-21] Event Dispatcher subsystem stub created and integrated into the engine structure.
"@

# Desired order of sections
$SectionOrder = @(
    "DevelopmentPhilosophy",
    "FuturePlatformDirection",
    "CurrentEngineStatus",
    "MilestoneHistory"
)

# Anchor for insertion
$appendixAnchor = "# Appendixes"

# Split README into main content and appendixes
if ($readme -match [regex]::Escape($appendixAnchor)) {
    $parts = $readme -split [regex]::Escape($appendixAnchor), 2
    $main = $parts[0].TrimEnd()
    $appendixes = $appendixAnchor + "`n" + $parts[1].TrimStart()
} else {
    $main = $readme.TrimEnd()
    $appendixes = ""
}

# Remove any existing canonical sections from main content
foreach ($key in $SectionOrder) {
    $content = $Sections[$key]
    $title = ($content -split "`n")[0].Trim().Substring(3)
    $regex = "(?ms)^##\s+$([regex]::Escape($title)).*?(?=^## |\z)"
    $main = [regex]::Replace($main, $regex, "").Trim()
}

# Rebuild main content with canonical sections in order
$newMain = $main + "`n`n"
foreach ($key in $SectionOrder) {
    $newMain += $Sections[$key].Trim() + "`n`n"
}

# Combine and write README
$final = ($newMain.Trim() + "`n`n" + $appendixes.Trim() + "`n")
Set-Content -Path $ReadmePath -Value $final -NoNewline

# Log the run
$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
$logLines = @()
$logLines += "[$timestamp] ReadMeReview.ps1 run"
$logLines += "  Canonical sections inserted/updated before Appendixes."
$logLines += ""

Add-Content -Path $LogPath -Value ($logLines -join "`n")

Write-Host "ReadMeReview.ps1 completed successfully." -ForegroundColor Green