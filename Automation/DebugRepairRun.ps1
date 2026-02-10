<# 
    DebugRepairRun.ps1
    Purpose:
        Repair known debug‑blocking corruption patterns in:
          - GameScene.cs
          - GameLoop.cs

    Logging:
        - Overwrites DebugRepairRun.log each run
        - Uses the same narrative, audit‑friendly format as PowerShellLog.md
        - Each repair includes:
            * Timestamp
            * What was repaired
            * Why it was repaired
            * Impact on engine stability

    Safety:
        - Only touches known-bad patterns
        - Creates .bak backups
        - ASCII-safe, deterministic, reversible
#>

$ErrorActionPreference = "Stop"

Write-Host "=== DebugRepairRun.ps1 starting ==="

# Paths
$root = "E:\BDC\Projects\SASZombieAssaultTD\SASZombieAssaultTD\Engine"
$sceneFile = Join-Path $root "Scenes\GameScene.cs"
$loopFile  = Join-Path $root "Core\GameLoop.cs"
$logFile   = Join-Path $root "DebugRepairRun.log"

# Overwrite log with header
"## DebugRepairRun.ps1 — $(Get-Date)" | Out-File $logFile -Encoding UTF8
"" | Out-File $logFile -Append

function Backup-File {
    param($path)
    $backup = "$path.bak"
    Copy-Item $path $backup -Force
    Add-Content $logFile "- Backup created: $backup"
}

# -------------------------------
# 1. Repair GameScene.cs
# -------------------------------
if (Test-Path $sceneFile) {

    Add-Content $logFile "### GameScene.cs Repairs — $(Get-Date)"
    Backup-File $sceneFile

    $content = Get-Content $sceneFile -Raw
    $changes = @()

    # Fix malformed raw string literal (stray backslash)
    if ($content -match '"""\\') {
        $content = $content -replace '"""\\', '"""'
        $changes += @{
            What   = "Removed stray backslash after raw string start."
            Reason = "Fix for CS1056 and CS8997 (malformed raw string literal)."
            Impact = "Restored valid string literal syntax."
        }
    }

    # Fix stray backslash before raw string end
    if ($content -match '\\\s*"""') {
        $content = $content -replace '\\\s*"""', '"""'
        $changes += @{
            What   = "Removed stray backslash before raw string end."
            Reason = "Fix for unterminated raw string literal."
            Impact = "Ensures string literal closes correctly."
        }
    }

    # Fix missing comma in object initializer
    if ($content -match '\w+\s+\w+\s*=\s*new\s*\(') {
        $content = $content -replace '(\w+)\s+(\w+)\s*=\s*new\s*\(', '$1, $2 = new('
        $changes += @{
            What   = "Inserted missing comma in object initializer."
            Reason = "Fix for CS1003 (',' expected)."
            Impact = "Restored valid initializer syntax."
        }
    }

    # Fix unterminated raw string literal
    if ($content -match '"""[^"]*$') {
        $content += '"""'
        $changes += @{
            What   = "Closed unterminated raw string literal."
            Reason = "Fix for CS8997."
            Impact = "Prevents compiler from reading into next lines."
        }
    }

    Set-Content $sceneFile $content -Encoding UTF8

    if ($changes.Count -eq 0) {
        Add-Content $logFile "- No repairs required."
    } else {
        foreach ($c in $changes) {
            Add-Content $logFile "- What:   $($c.What)"
            Add-Content $logFile "  Reason: $($c.Reason)"
            Add-Content $logFile "  Impact: $($c.Impact)"
            Add-Content $logFile ""
        }
    }
}

# -------------------------------
# 2. Repair GameLoop.cs
# -------------------------------
if (Test-Path $loopFile) {

    Add-Content $logFile "### GameLoop.cs Repairs — $(Get-Date)"
    Backup-File $loopFile

    $content = Get-Content $loopFile -Raw
    $changes = @()

    # Fix stray 'public' outside class
    if ($content -match '^\s*public\s*$') {
        $content = $content -replace '^\s*public\s*$', ''
        $changes += @{
            What   = "Removed stray 'public' keyword outside type definition."
            Reason = "Fix for CS0106."
            Impact = "Restores valid class/method structure."
        }
    }

    # Fix brace imbalance
    $openBraces  = ($content -split '\{').Count
    $closeBraces = ($content -split '\}').Count

    if ($openBraces -gt $closeBraces) {
        $content += "`n}"
        $changes += @{
            What   = "Added missing closing brace."
            Reason = "Fix for CS1022 (EOF expected)."
            Impact = "Restores class boundary integrity."
        }
    }

    if ($closeBraces -gt $openBraces) {
        $content = $content -replace '\}\s*$', ''
        $changes += @{
            What   = "Removed extra closing brace."
            Reason = "Fix for CS1022 (extra brace)."
            Impact = "Prevents premature class termination."
        }
    }

    Set-Content $loopFile $content -Encoding UTF8

    if ($changes.Count -eq 0) {
        Add-Content $logFile "- No repairs required."
    } else {
        foreach ($c in $changes) {
            Add-Content $logFile "- What:   $($c.What)"
            Add-Content $logFile "  Reason: $($c.Reason)"
            Add-Content $logFile "  Impact: $($c.Impact)"
            Add-Content $logFile ""
        }
    }
}

Add-Content $logFile "### DebugRepairRun.ps1 Completed — $(Get-Date)"
Write-Host "=== DebugRepairRun.ps1 completed ==="