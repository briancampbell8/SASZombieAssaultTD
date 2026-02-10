# FileAndDirectory-Builder � Engine + Automation Scaffolder
# Structure-first, additive-only, UTF-8, no BOM

$ErrorActionPreference = 'Stop'

# Root (assumes script is run from repo root)
$root = Get-Location

# Markdown files
$readmePath    = Join-Path $root 'README.md'
$structurePath = Join-Path $root 'ProjectStructure.md'
$tasklistPath  = Join-Path $root 'Tasklist.md'
$logPath       = Join-Path $root 'PowerShellLog.md'

$timestamp = Get-Date -Format 'yyyy-MM-dd HH:mm:ss'

function Ensure-File {
    param(
        [string]$Path,
        [string]$InitialContent = ''
    )

    if (-not (Test-Path -LiteralPath $Path)) {
        $dir = Split-Path $Path -Parent
        if ($dir -and -not (Test-Path -LiteralPath $dir)) {
            New-Item -ItemType Directory -Path $dir | Out-Null
        }
        # Create UTF-8 without BOM
        [System.IO.File]::WriteAllText($Path, $InitialContent, [System.Text.UTF8Encoding]::new($false))
        return $true
    }
    return $false
}

function Append-Line {
    param(
        [string]$Path,
        [string]$Line
    )
    Add-Content -Path $Path -Value $Line -Encoding UTF8
}

# -----------------------------
# 1. Define directories
# -----------------------------
$directories = @(
    'Engine',
    'Engine/Core',
    'Engine/Systems',
    'Engine/Systems/Assets',
    'Engine/Systems/Gameplay',
    'Engine/Systems/UI',
    'Engine/Tools',
    'Automation'
)

# -----------------------------
# 2. Define C# files
# -----------------------------
$csFiles = @(
    'Engine/GameRoot.cs',
    'Engine/Core/GameLoop.cs',
    'Engine/Core/TimingController.cs',
    'Engine/Core/GameStateManager.cs',
    'Engine/Systems/RenderQueue.cs',
    'Engine/Systems/InputRouter.cs',
    'Engine/Systems/EventDispatcher.cs',
    'Engine/Systems/Assets/AssetDiscovery.cs',
    'Engine/Systems/Assets/TextureLoader.cs',
    'Engine/Systems/Assets/DataLoader.cs',
    'Engine/Systems/Assets/AssetRegistry.cs',
    'Engine/Systems/LoggingSystem.cs',
    'Engine/Systems/Gameplay/TowerBase.cs',
    'Engine/Systems/Gameplay/EnemyBase.cs',
    'Engine/Systems/Gameplay/WaveController.cs',
    'Engine/Systems/Gameplay/ProjectileSystem.cs',
    'Engine/Systems/Gameplay/DamageResolver.cs',
    'Engine/Systems/UI/UIElementBase.cs',
    'Engine/Systems/UI/LayoutSystem.cs',
    'Engine/Systems/UI/HUD.cs',
    'Engine/Systems/UI/Menus.cs',
    'Engine/Tools/StructureGenerator.cs',
    'Engine/Tools/CleanupScripts.cs',
    'Engine/Tools/AppendixScripts.cs',
    'Engine/Tools/AutomationEnhancements.cs'
)

# -----------------------------
# 3. Define PS1 build scripts
# -----------------------------
$ps1Files = @(
    'Automation/GameLoop-Build.ps1',
    'Automation/TimingController-Build.ps1',
    'Automation/GameStateManager-Build.ps1',
    'Automation/RenderQueue-Build.ps1',
    'Automation/InputRouter-Build.ps1',
    'Automation/EventDispatcher-Build.ps1',
    'Automation/AssetDiscovery-Build.ps1',
    'Automation/TextureLoader-Build.ps1',
    'Automation/DataLoader-Build.ps1',
    'Automation/AssetRegistry-Build.ps1',
    'Automation/LoggingSystem-Build.ps1',
    'Automation/TowerBase-Build.ps1',
    'Automation/EnemyBase-Build.ps1',
    'Automation/WaveController-Build.ps1',
    'Automation/ProjectileSystem-Build.ps1',
    'Automation/DamageResolver-Build.ps1',
    'Automation/UIElementBase-Build.ps1',
    'Automation/LayoutSystem-Build.ps1',
    'Automation/HUD-Build.ps1',
    'Automation/Menus-Build.ps1',
    'Automation/StructureGenerator-Build.ps1',
    'Automation/CleanupScripts-Build.ps1',
    'Automation/AppendixScripts-Build.ps1',
    'Automation/AutomationEnhancements-Build.ps1'
)

# -----------------------------
# 4. Ensure markdown files exist
# -----------------------------
Ensure-File -Path $readmePath    | Out-Null
Ensure-File -Path $structurePath | Out-Null
Ensure-File -Path $tasklistPath  | Out-Null
Ensure-File -Path $logPath       | Out-Null

# -----------------------------
# 5. Create directories
# -----------------------------
$createdDirs = @()
foreach ($dirRel in $directories) {
    $dirPath = Join-Path $root $dirRel
    if (-not (Test-Path -LiteralPath $dirPath)) {
        New-Item -ItemType Directory -Path $dirPath | Out-Null
        $createdDirs += $dirRel
    }
}

# -----------------------------
# 6. Create C# files
# -----------------------------
$createdCs = @()
foreach ($fileRel in $csFiles) {
    $filePath = Join-Path $root $fileRel
    if (Ensure-File -Path $filePath) {
        $createdCs += $fileRel
    }
}

# -----------------------------
# 7. Create PS1 files
# -----------------------------
$createdPs1 = @()
foreach ($fileRel in $ps1Files) {
    $filePath = Join-Path $root $fileRel
    if (Ensure-File -Path $filePath) {
        $createdPs1 += $fileRel
    }
}

# -----------------------------
# 8. Update ProjectStructure.md
# -----------------------------
Append-Line -Path $structurePath -Line "
Append-Line -Path $structurePath -Line "## Engine and Automation Scaffolding ($timestamp)"
if ($createdDirs.Count -gt 0) {
    Append-Line -Path $structurePath -Line "- Directories created:"
    foreach ($d in $createdDirs) {
        Append-Line -Path $structurePath -Line "  - $d"
    }
}
if ($createdCs.Count -gt 0) {
    Append-Line -Path $structurePath -Line "- C# files created:"
    foreach ($f in $createdCs) {
        Append-Line -Path $structurePath -Line "  - $f"
    }
}
if ($createdPs1.Count -gt 0) {
    Append-Line -Path $structurePath -Line "- PowerShell build scripts created:"
    foreach ($f in $createdPs1) {
        Append-Line -Path $structurePath -Line "  - $f"
    }
}

# -----------------------------
# 9. Update README.md
# -----------------------------
Append-Line -Path $readmePath -Line "
Append-Line -Path $readmePath -Line "### Engine Scaffolding Update ($timestamp)"
Append-Line -Path $readmePath -Line "- Engine directories and stub files have been created for all core, systems, UI, gameplay, and tools components."
Append-Line -Path $readmePath -Line "- Automation build scripts exist for each subsystem, ready for population."

# -----------------------------
# 10. Update Tasklist.md
# -----------------------------
Append-Line -Path $tasklistPath -Line "
Append-Line -Path $tasklistPath -Line "## Engine Scaffolding Milestone ($timestamp)"
Append-Line -Path $tasklistPath -Line "- File and directory builder executed."
if ($createdCs.Count -gt 0) {
    Append-Line -Path $tasklistPath -Line "- C# stubs created: $($createdCs.Count)"
}
if ($createdPs1.Count -gt 0) {
    Append-Line -Path $tasklistPath -Line "- PS1 build stubs ensured: $($createdPs1.Count)"
}
# -----------------------------
# 11. Update PowerShellLog.md
# -----------------------------
Append-Line -Path $logPath -Line "
Append-Line -Path $logPath -Line "[$timestamp] FileAndDirectory-Builder executed."
Append-Line -Path $logPath -Line "- Directories created: $($createdDirs.Count)"
Append-Line -Path $logPath -Line "- C# files created: $($createdCs.Count)"
Append-Line -Path $logPath -Line "- PS1 files created: $($createdPs1.Count)"
Append-Line -Path $logPath -Line "- Mode: structure-first, additive-only, UTF-8, no BOM."

Write-Host "File and directory builder complete." -ForegroundColor Green
Write-Host "Directories created: $($createdDirs.Count)"
Write-Host "C# files created: $($createdCs.Count)"
Write-Host "PS1 files created: $($createdPs1.Count)"
