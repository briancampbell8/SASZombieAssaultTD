<#
    Title: Change0022_ReconstructEngineFiles.ps1
    Purpose:
        - Reconstruct and normalize namespace/class structure for all engine .cs files
        - Enforce Allman brace style and consistent indentation
        - Remove stray braces / malformed wrappers from extraction damage
    Scope:
        - Engine\Core
        - Engine\Systems
        - Engine\Rendering
        - Engine\Scenes
        - Decompiled\Engine
    Mode:
        - B-2b: Moderate detection, full structural reconstruction
    Author: BDC + Copilot
    Date: 2026-02-02
#>

[CmdletBinding()]
param()

$Root   = Split-Path -Parent $PSScriptRoot
$LogDir = Join-Path $Root 'Automation\Logs'
$null   = New-Item -ItemType Directory -Path $LogDir -Force
$Log    = Join-Path $LogDir 'Change0022_ReconstructEngineFiles.log'

function Write-ChangeLog {
    param(
        [string]$Path,
        [string]$Message
    )
    $line = "[{0}] {1} :: {2}" -f (Get-Date -Format 'yyyy-MM-dd HH:mm:ss'), $Path, $Message
    Add-Content -Path $Log -Value $line
}

function Get-NamespaceAndClass {
    param([string[]]$Lines)

    $ns   = $null
    $cls  = $null
    $usings = New-Object System.Collections.Generic.List[string]

    foreach ($line in $Lines) {
        $trim = $line.Trim()

        if ($trim -like 'using *;') {
            $usings.Add($trim)
            continue
        }

        if (-not $ns -and $trim -like 'namespace *') {
            $ns = $trim -replace '^namespace\s+', '' -replace '\s*{?\s*$', ''
        }

        if (-not $cls -and $trim -match '^\s*(public|internal|protected|private)?\s*(abstract\s+|sealed\s+)?(class|struct|interface)\s+([A-Za-z0-9_]+)') {
            $cls = $Matches[4]
        }
    }

    return [pscustomobject]@{
        Namespace = $ns
        ClassName = $cls
        Usings    = $usings
    }
}

function Extract-ClassBody {
    param([string[]]$Lines)

    # Very simple heuristic: everything after the first class/struct/interface line
    # until the end of file, minus trailing unmatched braces, is treated as body.
    $bodyLines = New-Object System.Collections.Generic.List[string]
    $inClass   = $false

    foreach ($line in $Lines) {
        if (-not $inClass -and $line -match '^\s*(public|internal|protected|private)?\s*(abstract\s+|sealed\s+)?(class|struct|interface)\s+[A-Za-z0-9_]+') {
            $inClass = $true
            continue
        }

        if ($inClass) {
            $bodyLines.Add($line)
        }
    }

    # Trim leading/trailing empty lines
    while ($bodyLines.Count -gt 0 -and $bodyLines[0].Trim() -eq '') {
        $bodyLines.RemoveAt(0)
    }
    while ($bodyLines.Count -gt 0 -and $bodyLines[$bodyLines.Count - 1].Trim() -eq '') {
        $bodyLines.RemoveAt($bodyLines.Count - 1)
    }

    return ,$bodyLines.ToArray()
}

function Normalize-AllmanAndIndent {
    param([string[]]$Lines)

    $out   = New-Object System.Collections.Generic.List[string]
    $depth = 0

    foreach ($line in $Lines) {
        $raw  = $line
        $trim = $raw.Trim()

        if ($trim -eq '') {
            $out.Add('')
            continue
        }

        # Split lines that have code + brace on same line into Allman style
        if ($trim -match '^(.*)\{(.*)$') {
            $before = $Matches[1].TrimEnd()
            $after  = $Matches[2].Trim()

            if ($before -ne '') {
                $indent = '    ' * $depth
                $out.Add($indent + $before)
            }

            $indent = '    ' * $depth
            $out.Add($indent + '{')
            $depth++

            if ($after -ne '') {
                # If there's a closing brace or code after, handle it recursively-ish
                if ($after -match '^(.*)\}(.*)$') {
                    $innerBefore = $Matches[1].TrimEnd()
                    $innerAfter  = $Matches[2].Trim()

                    if ($innerBefore -ne '') {
                        $indent = '    ' * $depth
                        $out.Add($indent + $innerBefore)
                    }

                    if ($depth -gt 0) { $depth-- }
                    $indent = '    ' * $depth
                    $out.Add($indent + '}')

                    if ($innerAfter -ne '') {
                        $indent = '    ' * $depth
                        $out.Add($indent + $innerAfter)
                    }
                }
                else {
                    $indent = '    ' * $depth
                    $out.Add($indent + $after)
                }
            }

            continue
        }

        if ($trim.StartsWith('}')) {
            if ($depth -gt 0) { $depth-- }
            $indent = '    ' * $depth
            $out.Add($indent + '}')

            $rest = $trim.Substring(1).Trim()
            if ($rest -ne '') {
                $indent = '    ' * $depth
                $out.Add($indent + $rest)
            }

            continue
        }

        $indent = '    ' * $depth
        $out.Add($indent + $trim)

        # Adjust depth based on braces
        $opens  = ($trim.ToCharArray() | Where-Object { $_ -eq '{' }).Count
        $closes = ($trim.ToCharArray() | Where-Object { $_ -eq '}' }).Count
        $depth += ($opens - $closes)
        if ($depth -lt 0) { $depth = 0 }
    }

    while ($depth -gt 0) {
        $out.Add('}')
        $depth--
    }

    return ,$out.ToArray()
}

function Reconstruct-File {
    param([string]$Path)

    $original = Get-Content -Path $Path -Raw
    $lines    = $original -split "`r?`n"

    $info = Get-NamespaceAndClass -Lines $lines

    if (-not $info.Namespace -or -not $info.ClassName) {
        Write-ChangeLog -Path $Path -Message "Missing namespace or class; reconstructing with defaults."
    }

    $ns   = if ($info.Namespace) { $info.Namespace } else { 'SASZombieAssaultTD.Engine' }
    $cls  = if ($info.ClassName) { $info.ClassName } else { [IO.Path]::GetFileNameWithoutExtension($Path) }
    $usings = if ($info.Usings.Count -gt 0) { $info.Usings } else { @('using System;') }

    $body = Extract-ClassBody -Lines $lines

    # If body is empty, keep original content inside as comment to avoid data loss
    if ($body.Count -eq 0 -and $lines.Count -gt 0) {
        $body = @('    // Original content could not be structurally parsed; preserved below as comment.')
        $body += $lines | ForEach-Object { '    // ' + $_ }
    }

    $reconstructed = New-Object System.Collections.Generic.List[string]

    # Usings
    $uniqueUsings = $usings | Select-Object -Unique
    foreach ($u in $uniqueUsings) {
        $reconstructed.Add($u.TrimEnd())
    }
    $reconstructed.Add('')

    # Namespace + class wrapper (Allman style)
    $reconstructed.Add("namespace $ns")
    $reconstructed.Add('{')
    $reconstructed.Add("    public class $cls")
    $reconstructed.Add('    {')

    foreach ($line in $body) {
        $reconstructed.Add('        ' + $line.TrimEnd())
    }

    $reconstructed.Add('    }')
    $reconstructed.Add('}')

    $normalized = Normalize-AllmanAndIndent -Lines $reconstructed.ToArray()
    $final      = ($normalized -join "`r`n")

    if ($final -ne $original) {
        $backup = "$Path.bak_Change0022"
        if (-not (Test-Path $backup)) {
            Copy-Item -Path $Path -Destination $backup
        }

        Set-Content -Path $Path -Value $final -Encoding UTF8
        Write-ChangeLog -Path $Path -Message "Reconstructed namespace/class wrapper and normalized formatting."
    }
    else {
        Write-ChangeLog -Path $Path -Message "No changes applied (already normalized)."
    }
}

Write-Host "Change0022: Starting full engine reconstruction (Allman style)..."

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

    Get-ChildItem -Path $dir -Recurse -Filter '*.cs' | ForEach-Object {
        Reconstruct-File -Path $_.FullName
    }
}

Write-Host "Change0022: Completed. See log at $Log"