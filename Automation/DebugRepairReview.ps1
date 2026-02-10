<# 
    DebugRepairVerify.ps1
    Purpose:
        Verify that GameScene.cs and GameLoop.cs contain no remaining corruption
        after DebugRepairRun.ps1 has executed.

    Verification checks:
        - Raw string literal integrity
        - No stray backslashes around """ tokens
        - No unterminated raw string literals
        - No missing commas in object initializers
        - Balanced braces in GameLoop.cs
        - No stray 'public' tokens outside type definitions

    Logging:
        - Overwrites DebugRepairVerify.log each run
        - Uses narrative, audit-friendly format matching PowerShellLog.md
        - Each finding includes:
            * What
            * Reason
            * Impact

    Safety:
        - Read-only verification
        - No modifications to source files
#>

$ErrorActionPreference = "Stop"

Write-Host "=== DebugRepairVerify.ps1 starting ==="

# Paths
$root = "E:\BDC\Projects\SASZombieAssaultTD\SASZombieAssaultTD\Engine"
$sceneFile = Join-Path $root "Scenes\GameScene.cs"
$loopFile  = Join-Path $root "Core\GameLoop.cs"
$logFile   = Join-Path $root "DebugRepairVerify.log"

# Overwrite log with header
"## DebugRepairVerify.ps1 — $(Get-Date)" | Out-File $logFile -Encoding UTF8
"" | Out-File $logFile -Append

function Log-Finding {
    param($what, $reason, $impact)
    Add-Content $logFile "- What:   $what"
    Add-Content $logFile "  Reason: $reason"
    Add-Content $logFile "  Impact: $impact"
    Add-Content $logFile ""
}

# Track overall status
$global:VerificationPassed = $true

# -------------------------------
# 1. Verify GameScene.cs
# -------------------------------
if (Test-Path $sceneFile) {

    Add-Content $logFile "### GameScene.cs Verification — $(Get-Date)"
    $content = Get-Content $sceneFile -Raw

    # Check for stray backslash after raw string start
    if ($content -match '"""\\') {
        $global:VerificationPassed = $false
        Log-Finding `
            "Stray backslash detected after raw string start." `
            "Indicates malformed raw string literal (CS1056, CS8997)." `
            "Compiler may misinterpret string boundaries."
    }

    # Check for stray backslash before raw string end
    if ($content -match '\\\s*"""') {
        $global:VerificationPassed = $false
        Log-Finding `
            "Stray backslash detected before raw string end." `
            "Indicates unterminated raw string literal." `
            "Compiler may read into subsequent lines."
    }

    # Check for unterminated raw string literal
    if ($content -match '"""[^"]*$') {
        $global:VerificationPassed = $false
        Log-Finding `
            "Unterminated raw string literal detected." `
            "Closing triple quotes missing (CS8997)." `
            "Compiler will fail to parse file."
    }

    # Check for missing comma in object initializer
    if ($content -match '\w+\s+\w+\s*=\s*new\s*\(') {
        $global:VerificationPassed = $false
        Log-Finding `
            "Possible missing comma in object initializer." `
            "Matches CS1003 corruption signature." `
            "Initializer may not compile."
    }

    if ($global:VerificationPassed) {
        Add-Content $logFile "- No issues detected in GameScene.cs"
        Add-Content $logFile ""
    }
}

# -------------------------------
# 2. Verify GameLoop.cs
# -------------------------------
if (Test-Path $loopFile) {

    Add-Content $logFile "### GameLoop.cs Verification — $(Get-Date)"
    $content = Get-Content $loopFile -Raw

    # Check for stray 'public' outside class
    if ($content -match '^\s*public\s*$') {
        $global:VerificationPassed = $false
        Log-Finding `
            "Stray 'public' keyword found outside type definition." `
            "Matches CS0106 corruption signature." `
            "Compiler will reject modifier in this position."
    }

    # Check brace balance
    $openBraces  = ($content -split '\{').Count
    $closeBraces = ($content -split '\}').Count

    if ($openBraces -ne $closeBraces) {
        $global:VerificationPassed = $false
        Log-Finding `
            "Brace imbalance detected in GameLoop.cs." `
            "Indicates missing or extra brace (CS1022)." `
            "Class or method structure may be invalid."
    }

    if ($global:VerificationPassed) {
        Add-Content $logFile "- No issues detected in GameLoop.cs"
        Add-Content $logFile ""
    }
}

# -------------------------------
# Final Result
# -------------------------------
if ($global:VerificationPassed) {
    Add-Content $logFile "### Verification Result: PASS — $(Get-Date)"
    Add-Content $logFile "- All corruption signatures cleared."
    Add-Content $logFile "- Files ready for debugging."
    Write-Host "Verification PASSED"
} else {
    Add-Content $logFile "### Verification Result: FAIL — $(Get-Date)"
    Add-Content $logFile "- One or more corruption signatures remain."
    Add-Content $logFile "- Review findings above."
    Write-Host "Verification FAILED"
}

Write-Host "=== DebugRepairVerify.ps1 completed ==="