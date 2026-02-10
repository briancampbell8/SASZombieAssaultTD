# =====================================================================
# Verify-Change0018.ps1
# Deterministic, non‑destructive audit script for Change0018
# Mode: read‑only, ASCII‑safe, no BOM, no mutation of any file
# =====================================================================

Write-Host "[Verify‑Change0018] Starting verification for Change0018"

# ---------------------------------------------------------------------
# Expected folders created by Change0018
# ---------------------------------------------------------------------
$ExpectedFolders = @(
    "Scripts",
    "Scripts\Paths",
    "Scripts\Zombies",
    "Scripts\TestScenes",
    "GameData",
    "GameData\Paths"
)

# ---------------------------------------------------------------------
# Expected files created or modified by Change0018
# ---------------------------------------------------------------------
$ExpectedFiles = @(
    "Scripts\TestScenes\MeanStreetsTest.cs",
    "Scripts\Zombies\ZombieMovement.cs",
    "Scripts\Paths\PathLoader.cs",
    "Scripts\Zombies\ZombieSpawner.cs"
)

# ---------------------------------------------------------------------
# Root detection (auto‑root like your ChangeXXXX.ps1 scripts)
# ---------------------------------------------------------------------
$Root = Split-Path -Parent $MyInvocation.MyCommand.Path
while ($Root -and -not (Test-Path "$Root\SASZombieAssaultTD.csproj")) {
    $Root = Split-Path -Parent $Root
}
if (-not $Root) {
    Write-Host "[Verify‑Change0018] ERROR: Could not auto‑locate project root." -ForegroundColor Red
    exit 1
}

Write-Host "[Verify‑Change0018] Project root detected: $Root"

# ---------------------------------------------------------------------
# Folder verification
# ---------------------------------------------------------------------
foreach ($folder in $ExpectedFolders) {
    $path = Join-Path $Root $folder
    if (Test-Path $path) {
        Write-Host "[Verify‑Change0018] PASS: Folder exists: $folder"
    } else {
        Write-Host "[Verify‑Change0018] FAIL: Missing folder: $folder" -ForegroundColor Red
    }
}

# ---------------------------------------------------------------------
# File existence verification
# ---------------------------------------------------------------------
foreach ($file in $ExpectedFiles) {
    $path = Join-Path $Root $file
    if (Test-Path $path) {
        Write-Host "[Verify‑Change0018] PASS: File exists: $file"
    } else {
        Write-Host "[Verify‑Change0018] FAIL: Missing file: $file" -ForegroundColor Red
    }
}

# ---------------------------------------------------------------------
# Byte‑for‑byte integrity check (only if Change0018 injected content)
# ---------------------------------------------------------------------
# NOTE: Insert expected payloads here if Change0018 had deterministic
#       injections. If not, this section remains a placeholder.
# ---------------------------------------------------------------------

Write-Host "[Verify‑Change0018] Verification complete."