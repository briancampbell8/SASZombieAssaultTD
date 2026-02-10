<# 
    Change0012_RepairCorruption.ps1
    Purpose: Deterministically repair encoding/syntax corruption across engine .cs files.
    Scope:   Engine\ and Automation\Engine\ trees only.
#>

$ProjectRoot = "E:\BDC\Projects\SASZombieAssaultTD"
$LogFile     = Join-Path $ProjectRoot "Logs\Change0012_RepairCorruption.md"
$BackupRoot  = Join-Path $ProjectRoot "Backups\Change0012_RepairCorruption"

New-Item -ItemType Directory -Force -Path (Split-Path $LogFile)     | Out-Null
New-Item -ItemType Directory -Force -Path $BackupRoot               | Out-Null

$header = @"
# Change0012 — Corruption Repair Sweep

- Date: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")
- ProjectRoot: $ProjectRoot
- BackupRoot: $BackupRoot

| File | Action | Notes |
|------|--------|-------|
"@
$header | Out-File -FilePath $LogFile -Encoding UTF8 -Force

# Target: engine + automation engine .cs files only
$targets = Get-ChildItem -Path $ProjectRoot -Recurse -Include *.cs |
    Where-Object {
        $_.FullName -like "*\Engine\*" -or
        $_.FullName -like "*\Automation\Engine\*"
    }

foreach ($file in $targets) {
    $originalContent = Get-Content -LiteralPath $file.FullName -Raw -Encoding UTF8

    # Null guard
    if ([string]::IsNullOrEmpty($originalContent)) {
        continue
    }

    $modifiedContent = $originalContent
    $changed = $false
    $notes   = @()

    # 1) Remove stray leading '@' on very first non-whitespace character
    $patternLeadingAt = '^\s*@\s*(\r?\n)'
    if ($modifiedContent -match $patternLeadingAt) {
        $modifiedContent = [regex]::Replace($modifiedContent, $patternLeadingAt, "`$1", 1)
        $changed = $true
        $notes  += "Removed stray leading '@' at file start"
    }

    # 2) Remove Unicode replacement char '�' (0xFFFD)
    $replacementChar = [char]0xFFFD
    if ($modifiedContent.Contains($replacementChar)) {
        $modifiedContent = $modifiedContent.Replace([string]$replacementChar, ")
        $changed = $true
        $notes  += "Removed Unicode replacement char (�)"
    }

    # 3) Remove lines that are just '$' or '$ '
    $patternLoneDollar = '^\s*\$\s*$'
    if ($modifiedContent -match $patternLoneDollar) {
        $modifiedContent =
            -join (
                ($modifiedContent -split "(`r`n|`n)") |
                Where-Object { $_ -notmatch $patternLoneDollar }
            )
        $changed = $true
        $notes  += "Removed lone '$' lines"
    }

    if ($changed -and $modifiedContent -ne $originalContent) {
        # Backup original
        $relativePath = $file.FullName.Substring($ProjectRoot.Length).TrimStart('\')
        $backupPath   = Join-Path $BackupRoot $relativePath
        New-Item -ItemType Directory -Force -Path (Split-Path $backupPath) | Out-Null
        Copy-Item -LiteralPath $file.FullName -Destination $backupPath -Force

        # Write modified
        $modifiedContent | Out-File -LiteralPath $file.FullName -Encoding UTF8 -Force

        $noteText = if ($notes.Count -gt 0) { ($notes -join "; ") } else { "Modified" }
        "| $relativePath | Repaired | $noteText |" | Out-File -FilePath $LogFile -Append -Encoding UTF8
    }
    elseif ($changed -and $modifiedContent -eq $originalContent) {
        $relativePath = $file.FullName.Substring($ProjectRoot.Length).TrimStart('\')
        "| $relativePath | Skipped | Pattern matched but no net change |" | Out-File -FilePath $LogFile -Append -Encoding UTF8
    }
}

"Done. Review the log and rebuild in VS." | Out-File -FilePath $LogFile -Append -Encoding UTF8
Write-Host "Change0012_RepairCorruption complete. See log at $LogFile"
