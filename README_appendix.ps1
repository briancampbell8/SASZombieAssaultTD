# README_appendix.ps1
# Safe Appendix Inserter — Additive Only, No Overwrites, No Rewrites

$projectRoot = Get-Location
$readmePath  = Join-Path $projectRoot "README.md"
$logPath     = Join-Path $projectRoot "PowerShellLog.md"
$timestamp   = Get-Date -Format "yyyy-MM-dd HH:mm:ss"

# Load README
$readme = Get-Content -Path $readmePath -Raw

# Remove corrupted characters (safe)
$readme = $readme -replace "�", "-"

# Check if appendixes already exist
if ($readme -match "## Appendix A") {
    Write-Output "Appendixes already present. No changes made."
    Add-Content -Path $logPath -Value "## Appendix Insert — $timestamp`n- Appendixes already present. No action taken.`n"
    exit
}

# Build appendix section (window-display style, double-spaced)
$appendixBlock = ---

'@
# Appendixes  


## Appendix A — ProjectStructure.md  
A complete, auto-generated snapshot of the project’s directory layout.  
This appendix serves as the structural map of the engine, allowing developers and archivists to understand how the project is organized at any point in time.

This file is maintained by the automation pipeline and reflects the most recent structure after cleanup, refactoring, or expansion.


## Appendix B — PowerShellLog.md  
A chronological audit trail of every automated action performed by the project’s PowerShell tools.  
This appendix preserves the historical lineage of changes, ensuring that every modification is traceable, reviewable, and reproducible.

It functions as the project’s black box recorder, documenting the evolution of the engine and its supporting files.


## Appendix C — Storytelling_Engine_Workflow.md  
The master workflow document that defines the philosophy, sequencing, and operational logic behind the storytelling engine.  
This appendix acts as the high-level guide for how the engine is built, maintained, and extended.

It is the source-of-truth reference for restoration, modernization, and future development.


## Appendix D — FreePremiumItems.md  
A dedicated reference for all premium items from the original Flash release, now fully unlocked and free in the modern rebuild.  
This appendix documents each item’s purpose, tactical role, and visual identity so players and developers can understand how premium content integrates into the preservation-first engine.

It serves as the authoritative catalog for premium assets, ensuring they remain accessible, consistent, and clearly defined across future updates.

@'

# Add appendix block to end of README (safe: only appends)
$readme = $readme + "`n`n" + $appendixBlock

# Write updated README
Set-Content -Path $readmePath -Value $readme -Encoding UTF8

# Log the operation
Add-Content -Path $logPath -Value "## Appendix Insert — $timestamp`n- Added Appendixes A, B, C, and D (non-destructive).`n"
