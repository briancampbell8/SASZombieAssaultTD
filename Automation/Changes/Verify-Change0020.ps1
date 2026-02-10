# =====================================================================
# Verify-Change0020.ps1
# Purpose: Validate Zombie AI Foundation installation
# Author: Automation Engine
# Mode: read-only, deterministic
# =====================================================================

Write-Host "[Verify-Change0020] Starting verification..." -ForegroundColor Cyan

$root = Split-Path -Parent $MyInvocation.MyCommand.Path
while (-not (Test-Path "$root\SASZombieAssaultTD.csproj")) {
    $root = Split-Path -Parent $root
    if ($root -eq "") {
        Write-Host "[Verify-Change0020] ERROR: Could not locate project root." -ForegroundColor Red
        exit 1
    }
}

$zombieDir = Join-Path $root "Engine\Systems\Gameplay\Zombies"

$expected = @(
    "ZombieBase.cs",
    "BasicZombie.cs",
    "ZombieController.cs"
)

foreach ($file in $expected) {
    $path = Join-Path $zombieDir $file
    if (Test-Path $path) {
        Write-Host "[Verify-Change0020] PASS: $file exists." -ForegroundColor Green
    } else {
        Write-Host "[Verify-Change0020] FAIL: Missing $file" -ForegroundColor Red
    }
}

Write-Host "[Verify-Change0020] Verification complete." -ForegroundColor Cyan