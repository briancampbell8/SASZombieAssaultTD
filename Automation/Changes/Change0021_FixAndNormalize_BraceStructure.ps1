<#
    Title: Change0021_FixAndNormalize_BraceStructure.ps1
    Purpose: Mass-repair brace / namespace / class structure corruption
             and normalize basic formatting for engine .cs files.
    Scope:   Engine\Core, Engine\Rendering, Engine\Systems, Engine\Scenes, Decompiled\Engine
    Author:  BDC + Copilot
    Date:    2026-02-02
#>

[CmdletBinding()]
param()

$Root = Split-Path -Parent $PSScriptRoot
$LogDir = Join-Path $Root 'Automation\Logs'
$null = New-Item -ItemType Directory -Path $LogDir -Force
$LogPath = Join-Path $LogDir 'Change0021_BraceRepair.log'

function Write-ChangeLog {
    param(
        [string]$Path,
        [string]$Message
    )
    $line = "[{0}] {1} :: {2}" -f (Get-Date -Format 'yyyy-MM-dd HH:mm:ss'), $Path, $Message
    Add-Content -Path $LogPath -Value $line
}

function Fix-DoubleNamespaceBrace {
    param([string]$Content)

    # Fix: namespace X.Y.Z { {  -> namespace X.Y.Z {
    $pattern = '(namespace\s+[^{\r\n]+?\s*\r?\n\{)\s*\r?\n\{'
    $replacement = '$1'
    return [System.Text.RegularExpressions.Regex]::Replace($Content, $pattern, $replacement)
}

function Ensure-ClassBrace {
    param([string]$Content)

    # If a class/struct/interface line is not followed by '{', insert it.
    $lines = $Content -split "`r?`n"
    $out   = New-Object System.Collections.Generic.List[string]

    for ($i = 0; $i -lt $lines.Count; $i++) {
        $line = $lines[$i]
        $out.Add($line)

        if ($line -match '^\s*(public|internal|protected|private)?\s*(abstract\s+|sealed\s+)?(class|struct|interface)\s+[A-Za-z0-9_]+') {
            $next = if ($i + 1 -lt $lines.Count) { $lines[$i + 1] } else { '' }
            if ($next -notmatch '^\s*\{') {
                $out.Add('    {')
                Write-ChangeLog -Path '<inline>' -Message "Inserted missing class brace after: $line"
            }
        }
    }

    return ($out -join "`r`n")
}

function Normalize-BracesAndIndent {
    param([string]$Content)

    $lines = $Content -split "`r?`n"
    $out   = New-Object System.Collections.Generic.List[string]
    $depth = 0

    foreach ($line in $lines) {
        $trim = $line.Trim()

        if ($trim -eq '') {
            $out.Add('')
            continue
        }

        # If line starts with closing brace, reduce depth first
        if ($trim.StartsWith('}')) {
            if ($depth -gt 0) { $depth-- }
        }

        $indent = '    ' * $depth
        $out.Add($indent + $trim)

        # Count braces on this line
        $opens  = ($trim.ToCharArray() | Where-Object { $_ -eq '{' }).Count
        $closes = ($trim.ToCharArray() | Where-Object { $_ -eq '}' }).Count

        $depth += ($opens - $closes)
        if ($depth -lt 0) { $depth = 0 }
    }

    # If depth > 0, append closing braces
    while ($depth -gt 0) {
        $out.Add('}')
        $depth--
    }

    return ($out -join "`r`n")
}

function Process-CsFile {
    param([string]$Path)

    $original = Get-Content -Path $Path -Raw
    $fixed    = $original

    $fixed = Fix-DoubleNamespaceBrace -Content $fixed
    $fixed = Ensure-ClassBrace        -Content $fixed
    $fixed = Normalize-BracesAndIndent -Content $fixed

    if ($fixed -ne $original) {
        $backupPath = "$Path.bak_Change0021"
        if (-not (Test-Path $backupPath)) {
            Copy-Item -Path $Path -Destination $backupPath
        }

        Set-Content -Path $Path -Value $fixed -Encoding UTF8
        Write-ChangeLog -Path $Path -Message "Repaired brace/structure and normalized formatting."
    }
}

Write-Host "Change0021: Starting brace/structure repair and normalization..."

$targets = @(
    'Engine\Core',
    'Engine\Rendering',
    'Engine\Systems',
    'Engine\Scenes',
    'Decompiled\Engine'
)

foreach ($rel in $targets) {
    $dir = Join-Path $Root $rel
    if (-not (Test-Path $dir)) {
        Write-ChangeLog -Path $dir -Message "Skipped (not found)."
        continue
    }

    Get-ChildItem -Path $dir -Recurse -Filter '*.cs' | ForEach-Object {
        Process-CsFile -Path $_.FullName
    }
}

Write-Host "Change0021: Completed. See log at $LogPath"