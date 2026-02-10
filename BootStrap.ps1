# BootStrap.ps1
# Unified, modular, idempotent bootstrap for SASZombieAssaultTD

# -----------------------------
# Core paths and initialization
# -----------------------------
$ProjectRoot   = Get-Location
$AutomationDir = Join-Path $ProjectRoot "Automation"
$ModulesDir    = Join-Path $AutomationDir "Modules"
$LogsDir       = Join-Path $AutomationDir "Logs"
$TemplatesDir  = Join-Path $AutomationDir "Templates"

# -----------------------------
# Ensure core Automation layout
# -----------------------------
if (-not (Test-Path $AutomationDir)) {
    New-Item -ItemType Directory -Path $AutomationDir | Out-Null
}

if (-not (Test-Path $ModulesDir)) {
    New-Item -ItemType Directory -Path $ModulesDir | Out-Null
}

if (-not (Test-Path $LogsDir)) {
    New-Item -ItemType Directory -Path $LogsDir | Out-Null
}

if (-not (Test-Path $TemplatesDir)) {
    New-Item -ItemType Directory -Path $TemplatesDir | Out-Null
}

# -----------------------------
# EngineCore.ps1 content
# -----------------------------
$engineCoreContent = @'
param(
    [string]$ProjectRoot
)
'@
function Write-Log {
    param([string]$Message)
    $logPath = Join-Path $ProjectRoot "PowerShellLog.md"
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $entry = "## EngineCore.ps1 � $timestamp`n- $Message`n"
    Add-Content -Path $logPath -Value $entry
}

function Ensure-AutomationStructure {
    $automationDir = Join-Path $ProjectRoot "Automation"
    $modulesDir    = Join-Path $automationDir "Modules"
    $logsDir       = Join-Path $automationDir "Logs"
    $templatesDir  = Join-Path $automationDir "Templates"

    if (-not (Test-Path $automationDir)) {
        New-Item -ItemType Directory -Path $automationDir | Out-Null
        Write-Log "Created Automation directory."
    }

    if (-not (Test-Path $modulesDir)) {
        New-Item -ItemType Directory -Path $modulesDir | Out-Null
        Write-Log "Created Automation\\Modules directory."
    }

    if (-not (Test-Path $logsDir)) {
        New-Item -ItemType Directory -Path $logsDir | Out-Null
        Write-Log "Created Automation\\Logs directory."
    }

    if (-not (Test-Path $templatesDir)) {
        New-Item -ItemType Directory -Path $templatesDir | Out-Null
        Write-Log "Created Automation\\Templates directory."
    }
}

function Ensure-GitIgnoreRules {
    $gitIgnorePath = Join-Path $ProjectRoot ".gitignore"
    $requiredRules = @(
        "Automation/Logs/",
        "bin/",
        "obj/"
    )

    if (-not (Test-Path $gitIgnorePath)) {
        New-Item -ItemType File -Path $gitIgnorePath | Out-Null
        Write-Log "Created .gitignore."
    }

    $existing = Get-Content -Path $gitIgnorePath -ErrorAction SilentlyContinue
    $updated  = $false

    foreach ($rule in $requiredRules) {
        if ($existing -notcontains $rule) {
            Add-Content -Path $gitIgnorePath -Value $rule
            $updated = $true
        }
    }

    if ($updated) {
        Write-Log "Updated .gitignore with required rules."
    } else {
        Write-Log ".gitignore already contained required rules."
    }
}

function Ensure-ProjectStructure {
    $structurePath = Join-Path $ProjectRoot "ProjectStructure.md"

    $structureContent = 
@'
# Project Structure Snapshot

Directories:
- Engine/
- Entities/
- Maps/
- Scenes/
- Systems/
- UI/
- Automation/
  - Modules/
  - Logs/
  - Templates/

Files:
- .gitattributes
- .gitignore
- app.manifest
- FreePremiumItems.md
- Game1.cs
- Icon.ico
- Inject-InfantryManager-Into-GameScene.ps1
- PowerShellLog.md
- Program.cs
- ProjectStructure.md
- README.md
- ReadMeSource.txt
- SASZombieAssaultTD.csproj
- Storytelling_Engine_Workflow.md
'@

    Set-Content -Path $structurePath -Value $structureContent
    Write-Log "Overwrote ProjectStructure.md with current structure snapshot."
}

function Ensure-CoreCSharpFile {
    param([string]$Root = $ProjectRoot)

    $coreDir = Join-Path $Root "Core"
    if (-not (Test-Path $coreDir)) {
        New-Item -ItemType Directory -Path $coreDir | Out-Null
        Write-Log "Created Core directory."
    }

    $coreFilePath = Join-Path $coreDir "GameRoot.cs"
    if (-not (Test-Path $coreFilePath)) {
        $coreFileContent = 
@'
using System;

namespace SASZombieAssaultTD.Core
{
    public class GameRoot
    {
        public void Initialize()
        {
            // Core initialization logic placeholder
        }

        public void Update()
        {
            // Core update logic placeholder
        }
    }
}
'@
        Set-Content -Path $coreFilePath -Value $coreFileContent
        Write-Log "Created Core\\GameRoot.cs."
    }
    else {
        Write-Log "Core\\GameRoot.cs already exists."
    }
}

Ensure-AutomationStructure
Ensure-GitIgnoreRules
Ensure-ProjectStructure
Ensure-CoreCSharpFile
Write-Log "EngineCore.ps1 execution completed."


# -----------------------------
# Ensure EngineCore.ps1 exists
# -----------------------------
$engineCorePath = Join-Path $AutomationDir "EngineCore.ps1"
if (-not (Test-Path $engineCorePath)) {
    Set-Content -Path $engineCorePath -Value $engineCoreContent
}

# -----------------------------
# Ensure PowerShellLog.md exists
# -----------------------------
$logPath = Join-Path $ProjectRoot "PowerShellLog.md"
if (-not (Test-Path $logPath)) {
    New-Item -ItemType File -Path $logPath | Out-Null
}

# -----------------------------
# Log bootstrap execution
# -----------------------------
$bootstrapTimestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
$bootstrapEntry = "## BootStrap.ps1 � $bootstrapTimestamp`n- Unified bootstrap executed in $ProjectRoot`n"
Add-Content -Path $logPath -Value $bootstrapEntry

# -----------------------------
# Invoke EngineCore.ps1
# -----------------------------
& $engineCorePath -ProjectRoot $ProjectRoot
