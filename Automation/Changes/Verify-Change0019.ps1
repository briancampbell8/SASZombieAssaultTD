<#
    Verify-Change0019.ps1
    Deterministic audit script for Change0019
    Mode: read‑only, ASCII‑safe, UTF‑8, no BOM
#>

Write-Host "[Verify‑Change0019] Starting verification for Change0019"

# --- Auto‑detect project root ---
$projectRoot = Split-Path -Parent $PSScriptRoot | Split-Path -Parent
Write-Host "[Verify‑Change0019] Project root detected: $projectRoot"

# --- Expected folders created/validated by Change0019 ---
$expectedFolders = @(
    "Scripts",
    "Scripts\AI",
    "Scripts\AI\Behaviors",
    "Scripts\AI\Blackboard",
    "GameData",
    "GameData\AI"
)

# --- Expected files created/modified by Change0019 ---
$expectedFiles = @(
    "Scripts\AI\Behaviors\BasicChase.cs",
    "Scripts\AI\Behaviors\WanderBehavior.cs",
    "Scripts\AI\Blackboard\AIBlackboard.cs",
    "Scripts\AI\Blackboard\BlackboardKeys.cs",
    "GameData\AI\BehaviorConfig.json"
)

# --- Folder checks ---
foreach ($folder in $expectedFolders) {
    $path = Join-Path $projectRoot $folder
    if (Test-Path $path) {
        Write-Host "[Verify‑Change0019] PASS: Folder exists: $folder"
    } else {
        Write-Host "[Verify‑Change0019] FAILURE: Missing folder: $folder"
    }
}

# --- File checks ---
foreach ($file in $expectedFiles) {
    $path = Join-Path $projectRoot $file
    if (Test-Path $path) {
        Write-Host "[Verify‑Change0019] PASS: File exists: $file"
    } else {
        Write-Host "[Verify‑Change0019] FAILURE: Missing file: $file"
    }
}

Write-Host "[Verify‑Change0019] Verification complete."