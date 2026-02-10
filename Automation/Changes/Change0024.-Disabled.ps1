<#
    Title: Change0024_ForceReconstructAllEngineFiles.ps1
    Purpose:
        Force reconstruction and normalization of all engine .cs files,
        and log all changes to PowerShellLog.md.
    Author: BDC + Copilot
    Date: 2026-02-02
#>

[CmdletBinding()]
param()

$Root = Split-Path -Parent $PSScriptRoot
$LogMd = Join-Path $Root 'Automation\PowerShellLog.md'

function Add-MarkdownLog {
    param(
        [string]$Path,
        [string]$Message
    )

    $rel = $Path.Replace($Root, '').TrimStart('\','/')
    $entry = @(
        "",
        "### Change0024 – $rel",
        "- **Time:** $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')",
        "- **Action:** $Message"
    )
    Add-Content -Path $LogMd -Value $entry
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
        $trim = $line.Trim()

        if ($trim -eq '') {
            $out.Add('')
            continue
        }

        if ($trim -match '^(.*)\{(.*)$') {
            $before = $Matches[1].TrimEnd()
            $after  = $Matches[2].Trim()

            if ($before -ne '') {
                $out.Add(('    ' * $depth) + $before)
            }

            $out.Add(('    ' * $depth) + '{')
            $depth++

            if ($after -ne '') {
                if ($after -match '^(.*)\}(.*)$') {
                    $innerBefore = $Matches[1].TrimEnd()
                    $innerAfter  = $Matches[2].Trim()

                    if ($innerBefore -ne '') {
                        $out.Add(('    ' * $depth) + $innerBefore)
                    }

                    if ($depth -gt 0) { $depth-- }
                    $out.Add(('    ' * $depth) + '}')

                    if ($innerAfter -ne '') {
                        $out.Add(('    ' * $depth) + $innerAfter)
                    }
                }
                else {
                    $out.Add(('    ' * $depth) + $after)
                }
            }

            continue
        }

        if ($trim.StartsWith('}')) {
            if ($depth -gt 0) { $depth-- }
            $out.Add(('    ' * $depth) + '}')

            $rest = $trim.Substring(1).Trim()
            if ($rest -ne '') {
                $out.Add(('    ' * $depth) + $rest)
            }

            continue
        }

        $out.Add(('    ' * $depth) + $trim)

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

    if (-not (Test-Path $Path)) { return }

    $original = Get-Content -Path $Path -Raw
    $lines    = $original -split "`r?`n"

    $info = Get-NamespaceAndClass -Lines $lines

    $ns   = if ($info.Namespace) { $info.Namespace } else { 'SASZombieAssaultTD.Engine' }
    $cls  = if ($info.ClassName) { $info.ClassName } else { [IO.Path]::GetFileNameWithoutExtension($Path) }
    $usings = if ($info.Usings.Count -gt 0) { $info.Usings } else { @('using System;') }

    $body = Extract-ClassBody -Lines $lines
    if ($body.Count -eq 0 -and $lines.Count -gt 0) {
        $body = @('    // Original content could not be structurally parsed; preserved below as comment.')
        $body += $lines | ForEach-Object { '    // ' + $_ }
    }

    $reconstructed = New-Object System.Collections.Generic.List[string]

    $uniqueUsings = $usings | Select-Object -Unique
    foreach ($u in $uniqueUsings) {
        $reconstructed.Add($u.TrimEnd())
    }
    $reconstructed.Add('')

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
        $backup = "$Path.bak_Change0024"
        if (-not (Test-Path $backup)) {
            Copy-Item -Path $Path -Destination $backup
        }

        Set-Content -Path $Path -Value $final -Encoding UTF8
        Add-MarkdownLog -Path $Path -Message "Reconstructed namespace/class wrapper and normalized formatting (Allman)."
    }
}

$targets = @(
    'Engine\Core',
    'Engine\Systems',
    'Engine\Rendering',
    'Engine\Scenes',
    'Decompiled\Engine'
)

foreach ($rel in $targets) {
    $dir = Join-Path $Root $rel
    if (-not (Test-Path $dir)) { continue }

    Get-ChildItem -Path $dir -Recurse -Filter '*.cs' | ForEach-Object {
        Reconstruct-File -Path $_.FullName
    }
}