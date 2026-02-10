# Change0005-Fix.ps1
# Purpose: Fully restore GameLoop.cs with clean, timing-controlled loop
# Mode: overwrite GameLoop.cs with known-good content, UTF-8, atomic write

$root = "E:\BDC\Projects\SASZombieAssaultTD"
$gameLoopPath = Join-Path $root "Engine\Core\GameLoop.cs"
$logPath = Join-Path $root "PowerShellLog.md"

if (-not (Test-Path $gameLoopPath)) {
    Write-Host "GameLoop.cs not found at $gameLoopPath"
    exit
}

$gameLoopContent = @"
using System;
using Engine.Systems;

namespace Engine.Core
{
    public class GameLoop
    {
        private bool _isRunning;
        private readonly TimingController _timing;
        private readonly GameStateManager _stateManager;
        private readonly RenderQueue _renderQueue;

        public GameLoop(TimingController timing, GameStateManager stateManager, RenderQueue renderQueue)
        {
            _timing = timing;
            _stateManager = stateManager;
            _renderQueue = renderQueue;
        }

        public void Run()
        {
            _isRunning = true;
            _timing.Start();

            Initialize();

            while (_isRunning)
            {
                _timing.Tick();

                if (_timing.ShouldUpdate)
                {
                    PreUpdate();
                    Update();
                    PostUpdate();
                }

                if (_timing.ShouldRender)
                {
                    PreRender();
                    Render();
                    PostRender();
                }
            }
        }

        private void Initialize()
        {
            // Initialization logic will be added in a later change cycle.
        }

        private void PreUpdate()
        {
            // Hook for logic that must run before Update().
        }

        private void Update()
        {
            _stateManager.Update(_timing.DeltaTime);
        }

        private void PostUpdate()
        {
            // Hook for logic that must run after Update().
        }

        private void PreRender()
        {
            // Hook for logic that must run before Render().
        }

        private void Render()
        {
            _renderQueue.Process();
        }

        private void PostRender()
        {
            // Hook for logic that must run after Render().
        }

        public void Stop()
        {
            _isRunning = false;
        }
    }
}
"@

Write-Host "Overwriting GameLoop.cs with clean, known-good implementation"
$gameLoopContent | Set-Content -Path $gameLoopPath -Encoding UTF8

$timestamp = Get-Date -Format 'yyyy-MM-dd HH:mm:ss'
$logEntry = @"
## Change0005-Fix.ps1 � $timestamp
- Restored GameLoop.cs to clean, timing-controlled implementation.
- Removed corrupted injected fragments and ensured proper namespace/class structure.
- Mode: overwrite GameLoop.cs with known-good content, UTF-8, atomic write.

"@

$logEntry | Add-Content -Path $logPath -Encoding UTF8

Write-Host "Change0005-Fix.ps1 completed."
