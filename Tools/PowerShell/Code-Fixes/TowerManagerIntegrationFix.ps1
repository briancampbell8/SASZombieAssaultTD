<#
.SYNOPSIS
    Surgical rewrite of TowerManagerIntegration.cs to align with the new ActionResult API
    and remove all legacy EconomyManager references.
#>

$target = "E:\BDC\Projects\SASZombieAssaultTD\Engine\Player\TowerManagerIntegration.cs"

if (-not (Test-Path $target)) {
    Write-Host "ERROR: File not found:" $target -ForegroundColor Red
    exit 1
}

# Create backup
$timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
$backup = "$target.bak.$timestamp"
Copy-Item $target $backup -Force
Write-Host "Backup created:" $backup -ForegroundColor Green

# Load file
$content = Get-Content $target -Raw

# Apply deterministic rewrites (wrapped in parentheses so -replace works)
$updated = (
    $content `
        -replace 'ActionResult\.Failed', 'ActionResult.Failure' `
        -replace 'ActionResult\.Success\([^)]*\)', 'ActionResult.Success()' `
        -replace '\.Success\b', '.IsSuccess' `
        -replace '!\s*(\w+)\.IsSuccess', '!$1.IsSuccess' `
        -replace 'EconomyManager\.Instance', '_playerSystem'
)

# Write updated file
Set-Content $target $updated -Encoding UTF8

Write-Host "Rewrite complete." -ForegroundColor Cyan
Write-Host "All ActionResult and EconomyManager references normalized." -ForegroundColor Cyan
Write-Host "TowerManagerIntegration.cs is now aligned with the new PlayerSystem and ActionResult API." -ForegroundColor Green