param(
    [string]E:\BDC\Projects\SASZombieAssaultTD
)

function Write-Log {
    param([string])
    \E:\BDC\Projects\SASZombieAssaultTD\PowerShellLog.md = Join-Path \E:\BDC\Projects\SASZombieAssaultTD "PowerShellLog.md"
    \2026-01-19 14:47:14 = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    \ = "## EngineCore.ps1 � \2026-01-19 14:47:14
- \
"
    Add-Content -Path \E:\BDC\Projects\SASZombieAssaultTD\PowerShellLog.md -Value \
}

function Ensure-AutomationStructure {
    \ = Join-Path \E:\BDC\Projects\SASZombieAssaultTD "Automation"
    \    = Join-Path \ "Modules"
    \       = Join-Path \ "Logs"
    \  = Join-Path \ "Templates"

    if (-not (Test-Path \)) {
        New-Item -ItemType Directory -Path \ | Out-Null
        Write-Log "Created Automation directory."
    }
    if (-not (Test-Path \)) {
        New-Item -ItemType Directory -Path \ | Out-Null
        Write-Log "Created Automation\Modules directory."
    }
    if (-not (Test-Path \)) {
        New-Item -ItemType Directory -Path \ | Out-Null
        Write-Log "Created Automation\Logs directory."
    }
    if (-not (Test-Path \)) {
        New-Item -ItemType Directory -Path \ | Out-Null
        Write-Log "Created Automation\Templates directory."
    }
}

function Ensure-GitIgnoreRules {
    \ = Join-Path \E:\BDC\Projects\SASZombieAssaultTD ".gitignore"
    \ = @(
        "Automation/Logs/",
        "bin/",
        "obj/"
    )

    if (-not (Test-Path \)) {
        New-Item -ItemType File -Path \ | Out-Null
        Write-Log "Created .gitignore."
    }

    \ = Get-Content -Path \ -ErrorAction SilentlyContinue
    \  = \False

    foreach (\ in \) {
        if (\ -notcontains \) {
            Add-Content -Path \ -Value \
            \ = \True
        }
    }

    if (\) {
        Write-Log "Updated .gitignore with required rules."
    } else {
        Write-Log ".gitignore already contained required rules."
    }
}

function Ensure-ProjectStructure {
function Ensure-CoreCSharpFile {
    $coreDir = Join-Path $ProjectRoot "Core"
    if (-not (Test-Path $coreDir)) {
        New-Item -ItemType Directory -Path $coreDir | Out-Null
        Write-Log "Created Core directory."
    }

    $coreFilePath = Join-Path $coreDir "GameRoot.cs"
    if (-not (Test-Path $coreFilePath)) {
        $content = @"
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
"@
        Set-Content -Path $coreFilePath -Value $content
        Write-Log "Created Core\GameRoot.cs."
    }
    else {
        Write-Log "Core\GameRoot.cs already exists."
    }
}

Ensure-AutomationStructure
Ensure-GitIgnoreRules
Ensure-CoreCSharpFile
Write-Log "EngineCore.ps1 execution completed."

