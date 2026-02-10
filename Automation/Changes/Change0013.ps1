<#
Change0013.ps1
Purpose:
    Roll back Change0012-style injection of:
        - GameStateManager.cs
        - RenderQueue.cs
        - InputRouter.cs
        - EventDispatcher.cs

Behavior:
    - If a .bak exists, restore it
    - If no .bak, replace with minimal safe stub
    - Log rollback to PowerShellLog.md
#>

$ErrorActionPreference = "Stop"

# --- Paths -------------------------------------------------------------

$root = "E:\BDC\Projects\SASZombieAssaultTD\Engine\Systems"

$targets = @{
    "GameStateManager.cs" = "$root\GameStateManager.cs"
    "RenderQueue.cs"      = "$root\RenderQueue.cs"
    "InputRouter.cs"      = "$root\InputRouter.cs"
    "EventDispatcher.cs"  = "$root\EventDispatcher.cs"
}

# --- Minimal safe stubs -----------------------------------------------

$Stub_GameStateManager = @"
namespace Engine.Systems
{
    public class GameStateManager
    {
    }
}
"@

$Stub_RenderQueue = @"
namespace Engine.Systems
{
    public class RenderQueue
    {
        public void Process()
        {
        }
    }
}
"@

$Stub_InputRouter = @"
namespace Engine.Systems
{
    public class InputRouter
    {
    }
}
"@

$Stub_EventDispatcher = @"
namespace Engine.Systems
{
    public class EventDispatcher
    {
    }
}
"@

$stubs = @{
    "GameStateManager.cs" = $Stub_GameStateManager
    "RenderQueue.cs"      = $Stub_RenderQueue
    "InputRouter.cs"      = $Stub_InputRouter
    "EventDispatcher.cs"  = $Stub_EventDispatcher
}

# --- Rollback ---------------------------------------------------------

Write-Host "Starting Change0013 rollback..." -ForegroundColor Cyan

foreach ($name in $targets.Keys)
{
    $path    = $targets[$name]
    $bakPath = "$path.bak"

    if (Test-Path $bakPath)
    {
        Write-Host "Restoring backup for $name" -ForegroundColor Yellow
        Copy-Item -Path $bakPath -Destination $path -Force
    }
    else
    {
        Write-Host "No backup for $name, writing minimal stub." -ForegroundColor DarkYellow
        $stub = $stubs[$name]
        Set-Content -Path $path -Value $stub -Encoding UTF8
    }
}

# --- Logging -----------------------------------------------------------

$logPath = "E:\BDC\Projects\SASZombieAssaultTD\PowerShellLog.md"

Add-Content -Path $logPath -Value "`n### Change0013 Rollback $(Get-Date)"
foreach ($name in $targets.Keys)
{
    Add-Content -Path $logPath -Value "- Rolled back: $name"
}

Write-Host "Change0013 rollback complete." -ForegroundColor Green
