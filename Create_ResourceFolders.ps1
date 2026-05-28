<#
    Script: Create_ResourceFolders.ps1
    Purpose:
        Creates the deterministic folder scaffolding for the SAS Zombie Assault TD
        Engine/Resources subsystem.

    Behavior:
        - Idempotent (safe to run multiple times)
        - Creates ONLY folders, no files
        - Matches architect-approved structure
        - Emits clear diagnostic output for each folder

    Usage:
        Run inside Visual Studio Terminal (PowerShell 7):
            ./Create_ResourceFolders.ps1
#>

$root = "Engine/Resources"

$folders = @(
    "$root/Integration",
    "$root/Pipeline",
    "$root/Manager",
    "$root/Bundles",
    "$root/Types",
    "$root/Metadata"
)

Write-Host ""
Write-Host "=== Creating Resource Folder Scaffolding ===" -ForegroundColor Cyan
Write-Host ""

foreach ($folder in $folders) {
    if (-not (Test-Path $folder)) {
        New-Item -ItemType Directory -Path $folder | Out-Null
        Write-Host "[Created] $folder" -ForegroundColor Green
    }
    else {
        Write-Host "[Exists ] $folder" -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "=== Folder scaffolding complete ===" -ForegroundColor Cyan
Write-Host ""
