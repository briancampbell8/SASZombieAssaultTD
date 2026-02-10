<# 
    Phase1_VerifyCsFiles.ps1
    Verifies:
      1. All required .cs files exist
      2. No file is empty or comment‑only
      3. Namespace alignment matches expected engine namespace
    Output:
      - Console summary
      - VerificationLog.md appended with results
#>

$ProjectRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$SrcRoot     = Join-Path $ProjectRoot "Engine"
$LogFile     = Join-Path $ProjectRoot "VerificationLog.md"

# Expected namespace for all engine files
$ExpectedNamespace = "SASZombieAssaultTD.Engine"

# Collect all .cs files under Engine
$CsFiles = Get-ChildItem -Path $SrcRoot -Recurse -Filter "*.cs"

# Track results
$MissingFiles   = @()
$EmptyFiles     = @()
$NamespaceDrift = @()

# If you have a canonical list of required files, define it here:
$RequiredFiles = @(
    "Program.cs",
    "GameRoot.cs",
    "GameLoop.cs",
    "AssetPipeline.cs",
    "AssetBundle.cs",
    "TextureLoader.cs",
    "DataLoader.cs",
    "DebugLogger.cs",
    "HeartbeatMonitor.cs",
    "FrameStats.cs",
    "Scene.cs"
)

# 1. Verify required files exist
foreach ($req in $RequiredFiles) {
    $found = $CsFiles | Where-Object { $_.Name -eq $req }
    if (-not $found) {
        $MissingFiles += $req
    }
}

# 2. Verify files are not empty or comment-only
foreach ($file in $CsFiles) {
    $content = Get-Content $file.FullName | Where-Object { $_.Trim() -ne "" -and -not $_.Trim().StartsWith("//") }

    if ($content.Count -eq 0) {
        $EmptyFiles += $file.FullName
    }
}

# 3. Verify namespace alignment
foreach ($file in $CsFiles) {
    $lines = Get-Content $file.FullName
    $nsLine = $lines | Where-Object { $_ -match "^namespace\s+" }

    if ($nsLine) {
        if ($nsLine -notmatch [regex]::Escape($ExpectedNamespace)) {
            $NamespaceDrift += "$($file.FullName) — $nsLine"
        }
    }
    else {
        $NamespaceDrift += "$($file.FullName) — <no namespace found>"
    }
}

# Write results to log
Add-Content $LogFile "`n## Phase1 Verification — $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')"
Add-Content $LogFile "### Missing Files:`n$($MissingFiles -join "`n")"
Add-Content $LogFile "### Empty or Comment-Only Files:`n$($EmptyFiles -join "`n")"
Add-Content $LogFile "### Namespace Drift:`n$($NamespaceDrift -join "`n")"
Add-Content $LogFile "`n---"

# Console summary
Write-Host "Phase 1 Verification Complete" -ForegroundColor Cyan
Write-Host "Missing Files: $($MissingFiles.Count)"
Write-Host "Empty Files: $($EmptyFiles.Count)"
Write-Host "Namespace Drift: $($NamespaceDrift.Count)"