<#
    Phase1_VerifyEngineStructure.ps1
    Full-engine verification sweep for Phase 1.

    Verifies:
      1. All .cs files under Engine\ exist and are readable
      2. No file is empty or comment-only
      3. Namespace alignment matches expected engine namespace root
      4. Logs results to Automation\VerificationLog.md

    Scope:
      <ProjectRoot>\Engine\
      (All subfolders included)
#>

# Correct project root (script is inside Automation\)
$ProjectRoot = Split-Path -Parent (Split-Path -Parent $MyInvocation.MyCommand.Path)

$EngineRoot  = Join-Path $ProjectRoot "Engine"
$LogFile     = Join-Path $ProjectRoot "Automation\VerificationLog.md"

# Expected namespace root
$ExpectedNamespaceRoot = "SASZombieAssaultTD.Engine"

# Collect all .cs files under Engine\
$CsFiles = Get-ChildItem -Path $EngineRoot -Recurse -Filter "*.cs"

# Tracking
$EmptyFiles     = @()
$NamespaceDrift = @()

# 1. Detect empty or comment-only files
foreach ($file in $CsFiles) {
    $content = Get-Content $file.FullName | Where-Object {
        $_.Trim() -ne "" -and -not $_.Trim().StartsWith("//")
    }

    if ($content.Count -eq 0) {
        $EmptyFiles += $file.FullName
    }
}

# 2. Detect namespace drift
foreach ($file in $CsFiles) {
    $lines = Get-Content $file.FullName
    $nsLine = $lines | Where-Object { $_ -match "^namespace\s+" }

    if ($nsLine) {
        if ($nsLine -notmatch [regex]::Escape($ExpectedNamespaceRoot)) {
            $NamespaceDrift += "$($file.FullName) — $nsLine"
        }
    }
    else {
        $NamespaceDrift += "$($file.FullName) — <no namespace found>"
    }
}

# Write results
Add-Content $LogFile "`n## Phase1 Engine Verification — $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')"
Add-Content $LogFile "### Empty or Comment-Only Files:`n$($EmptyFiles -join "`n")"
Add-Content $LogFile "### Namespace Drift:`n$($NamespaceDrift -join "`n")"
Add-Content $LogFile "`n---"

Write-Host "Full Engine Verification Complete" -ForegroundColor Cyan
Write-Host "Empty Files: $($EmptyFiles.Count)"
Write-Host "Namespace Drift: $($NamespaceDrift.Count)"