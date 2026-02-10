<#
    Title: Change0023_RemoveLegacyChangeCsFiles.ps1
    Purpose:
        Permanently remove all .cs files in the engine whose filename
        begins with "Change". These are legacy artifacts from old Change
        scripts and must not be compiled.
    Scope:
        Engine\Core
        Engine\Systems
        Engine\Rendering
        Engine\Scenes
        Decompiled\Engine
    Author: BDC + Copilot
    Date: 2026-02-02
#>

[CmdletBinding()]
param()

$Root   = Split-Path -Parent $PSScriptRoot
$LogDir = Join-Path $Root 'Automation\Logs'
$null   = New-Item -ItemType Directory -Path $LogDir -Force
$Log    = Join-Path $LogDir 'Change0023_RemoveLegacyChangeCsFiles.log'

function Write-ChangeLog {
    param(
        [string]$Path,
        [string]$Message
    )
    $line = "[{0}] {1} :: {2}" -f (Get-Date -Format 'yyyy-MM-dd HH:mm:ss'), $Path, $Message
    Add-Content -Path $Log -Value $line
}

Write-Host "Change0023: Removing legacy Change*.cs files..."

$targets = @(
    'Engine\Core',
    'Engine\Systems',
    'Engine\Rendering',
    'Engine\Scenes',
    'Decompiled\Engine'
)

foreach ($rel in $targets) {
    $dir = Join-Path $Root $rel
    if (-not (Test-Path $dir)) {
        Write-ChangeLog -Path $dir -Message "Skipped (directory not found)."
        continue
    }

    Get-ChildItem -Path $dir -Recurse -Filter 'Change*.cs' | ForEach-Object {
        $path = $_.FullName

        # Backup before deletion
        $backup = "$path.bak_Change0023"
        if (-not (Test-Path $backup)) {
            Copy-Item -Path $path -Destination $backup
        }

        Remove-Item -Path $path -Force
        Write-ChangeLog -Path $path -Message "Deleted Change*.cs legacy file."
    }
}

Write-Host "Change0023: Completed. See log at $Log"