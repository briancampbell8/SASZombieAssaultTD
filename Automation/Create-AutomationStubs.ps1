# Creates all empty automation scripts needed for the project

$projectRoot = Split-Path -Parent (Split-Path -Parent $PSCommandPath)
$automationDir = Join-Path $projectRoot "Automation"

# Ensure Automation directory exists
if (-not (Test-Path $automationDir)) {
    New-Item -ItemType Directory -Path $automationDir | Out-Null
}

# List of all automation scripts required for the full project
$scriptNames = @(
    "GameLoop-Build.ps1",
    "TimingController-Build.ps1",
    "GameStateManager-Build.ps1",
    "RenderQueue-Build.ps1",
    "InputRouter-Build.ps1",
    "EventDispatcher-Build.ps1",
    "AssetDiscovery-Build.ps1",
    "TextureLoader-Build.ps1",
    "DataLoader-Build.ps1",
    "AssetRegistry-Build.ps1",
    "LoggingSystem-Build.ps1",
    "TowerBase-Build.ps1",
    "EnemyBase-Build.ps1",
    "WaveController-Build.ps1",
    "ProjectileSystem-Build.ps1",
    "DamageResolver-Build.ps1",
    "UIElementBase-Build.ps1",
    "LayoutSystem-Build.ps1",
    "HUD-Build.ps1",
    "Menus-Build.ps1",
    "StructureGenerator-Build.ps1",
    "CleanupScripts-Build.ps1",
    "AppendixScripts-Build.ps1",
    "AutomationEnhancements-Build.ps1"
)

foreach ($name in $scriptNames) {
    $path = Join-Path $automationDir $name

    if (-not (Test-Path $path)) {
        New-Item -ItemType File -Path $path | Out-Null
        Write-Host "Created: $name" -ForegroundColor Green
    }
    else {
        Write-Host "Exists:  $name" -ForegroundColor Yellow
    }
}
