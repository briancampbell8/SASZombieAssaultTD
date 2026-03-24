# ================================
# P80 SAFE AUTO-REPAIR SCRIPT
# ================================
# Fixes:
# - Negative brace counts (too many closing braces)
# - Prematurely closed classes/namespaces
# - Namespace-level orphaned methods
# - Reconstructs class boundaries
# - Re-indents file
# - Creates backups
# ================================

$root = Get-Location
$csFiles = Get-ChildItem -Path $root -Recurse -Filter *.cs

$backupRoot = Join-Path $root "_safe_repair_backup_$(Get-Date -Format 'yyyyMMdd_HHmmss')"
New-Item -ItemType Directory -Path $backupRoot | Out-Null

Write-Host "Backup directory: $backupRoot"

foreach ($file in $csFiles) {
    $content = Get-Content $file.FullName -Raw
    $origContent = $content

    # Backup original
    $relPath = $file.FullName.Substring($root.Path.Length).TrimStart('\')
    $backupPath = Join-Path $backupRoot $relPath
    $backupDir = Split-Path $backupPath -Parent
    if (!(Test-Path $backupDir)) { New-Item -ItemType Directory -Path $backupDir | Out-Null }
    Set-Content -Path $backupPath -Value $origContent

    $lines = $content -split "`r?`n"

    # -------------------------------
    # 1) Detect brace imbalance
    # -------------------------------
    $braceCount = 0
    foreach ($line in $lines) {
        $braceCount += ($line.ToCharArray() | Where-Object { $_ -eq '{' }).Count
        $braceCount -= ($line.ToCharArray() | Where-Object { $_ -eq '}' }).Count
    }

    # -------------------------------
    # 2) Fix NEGATIVE brace counts
    #    (too many closing braces)
    # -------------------------------
    if ($braceCount -lt 0) {
        $removeCount = -$braceCount
        Write-Host "[FIX] $($file.FullName): removing $removeCount extra closing brace(s)"

        $fixed = New-Object System.Collections.Generic.List[string]
        $removed = 0

        foreach ($line in $lines) {
            if ($removed -lt $removeCount -and $line.Trim() -eq "}") {
                $removed++
                continue
            }
            $fixed.Add($line)
        }

        $lines = $fixed
    }

    # -------------------------------
    # 3) Fix POSITIVE brace counts
    #    (missing closing braces)
    # -------------------------------
    if ($braceCount -gt 0) {
        Write-Host "[FIX] $($file.FullName): adding $braceCount closing brace(s)"
        for ($i = 0; $i -lt $braceCount; $i++) {
            $lines += "}"
        }
    }

    # -------------------------------
    # 4) Move namespace-level methods
    # -------------------------------
    $scopeLevel = 0
    $fixedLines = New-Object System.Collections.Generic.List[string]
    $orphanedMethods = New-Object System.Collections.Generic.List[string]

    foreach ($line in $lines) {
        $trim = $line.Trim()

        # Update scope BEFORE checking
        $openCount  = ($line.ToCharArray() | Where-Object { $_ -eq '{' }).Count
        $closeCount = ($line.ToCharArray() | Where-Object { $_ -eq '}' }).Count
        $scopeLevel += $openCount - $closeCount

        $isMethodSig =
            ($trim -match '^(public|private|protected|internal)\s+[\w\<\>\[\]]+\s+\w+\s*\(') -and
            -not ($trim -match 'class|struct|interface|enum')

        if ($isMethodSig -and $scopeLevel -le 1) {
            Write-Host "[MOVE] $($file.FullName): orphaned method moved -> $trim"
            $orphanedMethods.Add($line)
            continue
        }

        $fixedLines.Add($line)
    }

    # -------------------------------
    # 5) Reinsert orphaned methods
    #    into the last class found
    # -------------------------------
    if ($orphanedMethods.Count -gt 0) {
        $insertIndex = $fixedLines.FindLastIndex({ $_ -match 'class\s+\w+' })

        if ($insertIndex -ge 0) {
            Write-Host "[INSERT] Reinserting orphaned methods into class in $($file.FullName)"
            $fixedLines.Insert($insertIndex + 1, "")
            $fixedLines.Insert($insertIndex + 2, "    // --- AUTO-REPAIRED ORPHANED METHODS ---")
            foreach ($m in $orphanedMethods) {
                $fixedLines.Insert($insertIndex + 3, "    $m")
            }
            $fixedLines.Insert($insertIndex + 3 + $orphanedMethods.Count, "    // --- END AUTO-REPAIRED METHODS ---")
        }
        else {
            Write-Host "[WARN] No class found to insert orphaned methods in $($file.FullName)"
        }
    }

    # -------------------------------
    # 6) Re-indent file
    # -------------------------------
    $indent = 0
    $final = New-Object System.Collections.Generic.List[string]

    foreach ($line in $fixedLines) {
        $trim = $line.Trim()

        if ($trim.StartsWith("}")) { $indent = [Math]::Max(0, $indent - 1) }

        $final.Add(("    " * $indent) + $trim)

        if ($trim.EndsWith("{")) { $indent++ }
    }

    # -------------------------------
    # 7) Write repaired file
    # -------------------------------
    $finalText = $final -join "`r`n"
    Set-Content -Path $file.FullName -Value $finalText
}