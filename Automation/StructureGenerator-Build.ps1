<#
.SYNOPSIS
    Generates the full SAS Zombie Assault TD directory and file structure.

.DESCRIPTION
    This script creates all directories and empty file stubs defined in the
    authoritative ProjectStructure.md snapshot (2026‑01‑23 15:25:45).
    It is deterministic, idempotent, and safe for repeated execution.

.NOTES
    Author: BDC
    Script: StructureGenerator.ps1
    Mode: structure-first, additive-only, UTF-8, no BOM
#>

# ---------------------------------------------------------
# Canonical Directory + File Map (from ProjectStructure.md)
# ---------------------------------------------------------

$Structure = @{

    ".config" = @()

    "Assets" = @()
    "Assets/Audio" = @()
    "Assets/Effects" = @()
    "Assets/Fonts" = @()
    "Assets/Maps" = @()
    "Assets/Sprites" = @()
    "Assets/Sprites/Soldiers" = @(
        "grenadier.png",
        "medic.png",
        "rifleman.png",
        "sawgunner.png",
        "sniper.png"
    )
    "Assets/Textures" = @()
    "Assets/Tiles" = @()

    "Automation" = @(
        "AppendixScripts-Build.ps1",
        "AssetDiscovery-Build.ps1",
        "AssetRegistry-Build.ps1",
        "AutomationEnhancements-Build.ps1",
        "CleanupScripts-Build.ps1",
        "Create-AutomationStubs.ps1",
        "DamageResolver-Build.ps1",
        "DataLoader-Build.ps1",
        "EnemyBase-Build.ps1",
        "EngineCore.ps1",
        "EventDispatcher-Build.ps1",
        "FileAndDirectory-Builder.ps1",
        "GameLoop-Build.ps1",
        "GameStateManager-Build.ps1",
        "Generate-ProjectStructure.ps1",
        "HUD-Build.ps1",
        "InputRouter-Build.ps1",
        "LayoutSystem-Build.ps1",
        "LoggingSystem-Build.ps1",
        "Menus-Build.ps1",
        "Next-Subsystem.ps1",
        "ProjectileSystem-Build.ps1",
        "RenderQueue-Build.ps1",
        "StructureGenerator-Build.ps1",
        "tasklist.ps1",
        "TextureLoader-Build.ps1",
        "TimingController-Build.ps1",
        "TowerBase-Build.ps1",
        "UIElementBase-Build.ps1",
        "WaveController-Build.ps1"
    )
    "Automation/Engine" = @()
    "Automation/Logs" = @()
    "Automation/Modules" = @()
    "Automation/Templates" = @()

    "Core" = @(
        "AssetManager.cs",
        "Config.cs",
        "GameEngine.cs",
        "InputHandler.cs",
        "TimeManager.cs"
    )
    "Core/Config" = @("Config.cs")
    "Core/Managers" = @(
        "AssetLoader.cs",
        "InputManager.cs",
        "TimeManager.cs"
    )
    "Core/Utilities" = @("MathHelperExtensions.cs")

    "Docs" = @(
        "AnimationSystem.md",
        "AudioPlan.md",
        "BossLogic.md",
        "CameraSystem.md",
        "DeployableLogic.md",
        "EnemyRoster.md",
        "EntityManager.md",
        "MapFlow.md",
        "PathfindingSystem.md",
        "PremiumItemEffects.md",
        "PremiumItems.md",
        "ProjectileSystem.md",
        "SaveSystem.md",
        "SceneArchitecture.md",
        "SpriteAtlasPlan.md",
        "TileSystem.md",
        "TurretBehavior.md",
        "UI_Design.md",
        "UpgradeSystem.md",
        "WaveConfig.md"
    )

    "Engine" = @("GameRoot.cs")
    "Engine/Core" = @(
        "GameLoop.cs",
        "GameStateManager.cs",
        "TimingController.cs"
    )
    "Engine/Entities" = @(
        "Enemy.cs",
        "Projectile.cs",
        "Soldier.cs",
        "Turret.cs"
    )
    "Engine/Managers" = @(
        "EnemyManager.cs",
        "EntityManager.cs",
        "ProjectileManager.cs",
        "WaveManager.cs"
    )
    "Engine/Rendering" = @()
    "Engine/Systems" = @(
        "AnimationSystem.cs",
        "CameraSystem.cs",
        "EventDispatcher.cs",
        "InputRouter.cs",
        "LoggingSystem.cs",
        "PathfindingSystem.cs",
        "RenderQueue.cs"
    )
    "Engine/Systems/Assets" = @(
        "AssetDiscovery.cs",
        "AssetRegistry.cs",
        "DataLoader.cs",
        "TextureLoader.cs"
    )
    "Engine/Systems/Gameplay" = @(
        "DamageResolver.cs",
        "EnemyBase.cs",
        "ProjectileSystem.cs",
        "TowerBase.cs",
        "WaveController.cs"
    )
    "Engine/Systems/UI" = @(
        "HUD.cs",
        "LayoutSystem.cs",
        "Menus.cs",
        "UIElementBase.cs"
    )
    "Engine/Tools" = @(
        "AppendixScripts.cs",
        "AutomationEnhancements.cs",
        "CleanupScripts.cs",
        "StructureGenerator.cs"
    )
    "Engine/UI" = @()

    "Entities" = @()
    "Entities/Abilities" = @()
    "Entities/AirSupport" = @()
    "Entities/Environment" = @()
    "Entities/Graphics" = @()
    "Entities/Graphics/Animations" = @()
    "Entities/Graphics/Backgrounds" = @()
    "Entities/Graphics/Particles" = @()
    "Entities/Graphics/Sprites" = @()
    "Entities/Graphics/UIArt" = @()
    "Entities/Projectiles" = @()
    "Entities/Soldiers" = @()
    "Entities/Towers" = @()
    "Entities/Zombies" = @()

    "LogAnalysisReports" = @(
        "Category_Statistics.csv",
        "Daily_Summary.csv",
        "Detailed_Events.csv"
    )

    "Maps" = @()

    "Scenes" = @()
    "Scenes/Game" = @("GameScene.cs")
    "Scenes/MainMenu" = @("MainMenuScene.cs")
    "Scenes/Pause" = @("PauseScene.cs")
    "Scenes/Shared" = @()
    # Duplicate entries in snapshot preserved for determinism
    "Scenes" = @("MainMenuScene.cs", "PauseScene.cs")

    "Systems" = @(
        "AbilitySystem.cs",
        "CollisionSystem.cs",
        "EconomySystem.cs",
        "ProjectileSystem.cs",
        "TowerSystem.cs",
        "UpgradeSystem.cs",
        "WaveSystem.cs",
        "ZombieSystem.cs"
    )
    "Systems/AI" = @("ZombieSystem.cs")
    "Systems/Collision" = @("CollisionSystem.cs")
    "Systems/Combat" = @(
        "ProjectileSystem.cs",
        "TowerSystem.cs"
    )
    "Systems/Economy" = @("EconomySystem.cs")
    "Systems/Placement" = @()
    "Systems/Survival" = @()
    "Systems/Upgrades" = @(
        "AbilitySystem.cs",
        "UpgradeSystem.cs"
    )
    "Systems/Waves" = @("WaveSystem.cs")

    "UI" = @(
        "Button.cs",
        "HUD.cs",
        "Menu.cs",
        "Panel.cs"
    )
    "UI/Components" = @(
        "Button.cs",
        "Panel.cs"
    )
    "UI/HUD" = @("HUD.cs")
    "UI/Layouts" = @()
    "UI/Menus" = @("Menu.cs")

    # Root-level files
    " = @(
        ".gitattributes",
        ".gitignore",
        "app.manifest",
        "BootStrap.ps1",
        "FreePremiumItems.md",
        "Generate-PowerShellScripts.ps1",
        "Icon.ico",
        "Inject-InfantryManager-Into-GameScene.ps1",
        "PowerShellLog-Analyzer.ps1",
        "PowerShellLog.md",
        "Program.cs",
        "ProjectStructure.md",
        "README_appendix.ps1",
        "README.md",
        "ReadMeSource.txt",
        "Safe-ACL-Reset.ps1",
        "SASZombieAssaultTD.csproj",
        "SASZombieAssaultTD.slnx",
        "Storytelling_Engine_Workflow.md",
        "Tasklist.md"
    )
}

# ---------------------------------------------------------
# Function: Create Directories + File Stubs
# ---------------------------------------------------------

function New-CanonicalStructure {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Root
    )

    Write-Host "Generating canonical directory + file structure..." -ForegroundColor Cyan
    Write-Host "Root: $Root" -ForegroundColor DarkCyan

    foreach ($entry in $Structure.GetEnumerator()) {

        $dir = $entry.Key
        $files = $entry.Value

        $dirPath = if ($dir -eq ") { $Root } else { Join-Path $Root $dir }

        if (-not (Test-Path $dirPath)) {
            New-Item -ItemType Directory -Path $dirPath | Out-Null
            Write-Host "[Created] Directory: $dir" -ForegroundColor Green
        }
        else {
            Write-Host "[Exists]  Directory: $dir" -ForegroundColor DarkGray
        }

        foreach ($file in $files) {
            $filePath = Join-Path $dirPath $file

            if (-not (Test-Path $filePath)) {
                New-Item -ItemType File -Path $filePath -Encoding utf8NoBOM | Out-Null
                Write-Host "[Created] File: $dir/$file" -ForegroundColor Yellow
            }
            else {
                Write-Host "[Exists]  File: $dir/$file" -ForegroundColor DarkGray
            }
        }
    }

    Write-Host "Structure generation complete." -ForegroundColor Green
}

# ---------------------------------------------------------
# Execution
# ---------------------------------------------------------

param(
    [Parameter(Mandatory = $true)]
    [string]$Path
)

New-CanonicalStructure -Root $Path
# ---------------------------------------------------------
# Post-Snapshot BOM Cleaner (UTF-8 no BOM enforcement)
# ---------------------------------------------------------

Write-Host "Running BOM validation..." -ForegroundColor Cyan

$exclude = @(".vs", "bin", "obj", ".git")

Get-ChildItem -Recurse -File | Where-Object {
    $path = $_.FullName
    -not ($exclude | ForEach-Object { $path -like "*\$_\*" })
} | ForEach-Object {
    try {
        $bytes = [System.IO.File]::ReadAllBytes($_.FullName)
        if ($bytes.Length -ge 3 -and $bytes[0..2] -eq 0xEF,0xBB,0xBF) {
            $text = Get-Content $_.FullName -Raw
            $text | Set-Content $_.FullName -Encoding utf8NoBOM
            Write-Host "[Fixed BOM] $($_.FullName)" -ForegroundColor Yellow
        }
    }
    catch {
        Write-Host "[Skipped] Locked or unreadable: $($_.FullName)" -ForegroundColor DarkGray
    }
}

Write-Host "BOM validation complete." -ForegroundColor Green
