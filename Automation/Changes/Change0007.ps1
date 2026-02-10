# Change0007.ps1
# Purpose: Replace corrupted GameRoot.cs with clean, modern engine-wiring version
# Mode: overwrite, UTF-8, atomic write

$root      = "E:\BDC\Projects\SASZombieAssaultTD"
$engineDir = Join-Path $root "Engine"
$coreDir   = Join-Path $engineDir "Core"
$gameRootPath = Join-Path $engineDir "GameRoot.cs"
$logPath   = Join-Path $root "PowerShellLog.md"

if (-not (Test-Path $engineDir)) {
    Write-Host "Engine directory not found at $engineDir"
    exit
}

# Correct GameRoot.cs content
$gameRootContent = @"
using Engine.Core;
using Engine.Systems;

namespace Engine
{
    public sealed class GameRoot
    {
        private readonly TimingController _timing;
        private readonly GameStateManager _stateManager;
        private readonly RenderQueue _renderQueue;
        private readonly GameLoop _loop;

        public GameRoot()
        {
            _timing = new TimingController();
            _stateManager = new GameStateManager();
            _renderQueue = new RenderQueue();

            _loop = new GameLoop(_timing, _stateManager, _renderQueue);
        }

        public void Run()
        {
            _loop.Run();
        }
    }
}
"@

Write-Host "Overwriting GameRoot.cs with clean, correct implementation"
$gameRootContent | Set-Content -Path $gameRootPath -Encoding UTF8

# Log Change0007
$timestamp = Get-Date -Format 'yyyy-MM-dd HH:mm:ss'
$logEntry = @"
## Change0007.ps1 � $timestamp
- Replaced corrupted GameRoot.cs with clean, modern engine-wiring implementation.
- Restored correct namespace (Engine) and subsystem wiring.
- Ensured GameRoot constructs TimingController, GameStateManager, RenderQueue, and GameLoop.
- Mode: overwrite, UTF-8, atomic write.

"@

$logEntry | Add-Content -Path $logPath -Encoding UTF8

Write-Host "Change0007.ps1 completed."
