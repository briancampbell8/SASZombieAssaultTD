# Phase 3 – Create missing engine system files

$files = @(
    "Engine/Economy/EconomyManager.cs",
    "Engine/Economy/IsAfford.cs",

    "Engine/Difficulty/DifficultyManager.cs",
    "Engine/Difficulty/DifficultyDatabase.cs",

    "Engine/Player/ModernPlayerStateSystem.cs",

    "Engine/Navigation/NavigationGrid.cs",

    "Engine/Rendering/CameraSystem.cs",

    "Engine/Audio/PlaySound.cs",
    "Engine/Audio/PlayErrorSound.cs",
    "Engine/Audio/PlaySuccessSound.cs",

    "Engine/UI/Rendering/RenderBackground.cs",
    "Engine/UI/Rendering/GetTransitionProgress.cs",
    "Engine/UI/Rendering/GetUpgradeStatusColor.cs",
    "Engine/UI/Rendering/GetUpgradeInfoText.cs",
    "Engine/UI/Rendering/GetUpgradeInfoTextColor.cs",

    "Engine/Towers/Upgrades/InitializeCompleteUpgrade.cs",
    "Engine/Towers/Upgrades/UpdateUpgradeOptions.cs",

    "Engine/Towers/Tower.cs"
)

foreach ($file in $files) {
    $fullPath = Join-Path -Path $PSScriptRoot -ChildPath $file
    $dir = Split-Path $fullPath

    if (!(Test-Path $dir)) {
        New-Item -ItemType Directory -Path $dir -Force | Out-Null
    }

    if (!(Test-Path $fullPath)) {
        New-Item -ItemType File -Path $fullPath -Force | Out-Null
    }
}