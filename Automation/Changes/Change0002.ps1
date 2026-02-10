# ================================
# Change0002.ps1
# Purpose: Inject GameLoop.cs implementation
# Author: BDC Automation Pipeline
# Mode: Additive-only, UTF-8, atomic write
# ================================

Write-Host "Executing Change0002.ps1..."

# ------------------------
# 1. C# CODE PAYLOAD
# ------------------------
$code = @"
using System;

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
            Initialize();

            while (_isRunning)
            {
                _timing.Tick();

                PreUpdate();
                Update();
                PostUpdate();

                PreRender();
                Render();
                PostRender();
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

# ------------------------
# 2. WRITE TO GameLoop.cs
# ------------------------
$target = "E:\BDC\Projects\SASZombieAssaultTD\Engine\Core\GameLoop.cs"

Set-Content -Path $target -Value $code -Encoding UTF8
Write-Host "Injected GameLoop.cs"

# ------------------------
# 3. APPEND TO PowerShellLog.md
# ------------------------
$logPath = "E:\BDC\Projects\SASZombieAssaultTD\PowerShellLog.md"
$timestamp = (Get-Date).ToString("yyyy-MM-dd HH:mm:ss")

$logEntry = @"
## Change0002.ps1 � $timestamp
- Injected full GameLoop.cs implementation (Option B structured loop).
- Added Initialize, PreUpdate, Update, PostUpdate, PreRender, Render, PostRender, Stop.
- Mode: additive-only, UTF-8, atomic write.
"@

Add-Content -Path $logPath -Value $logEntry -Encoding UTF8
Write-Host "Logged Change0002 in PowerShellLog.md"

# ------------------------
# 4. UPDATE Tasklist.md
# ------------------------
$tasklist = "E:\BDC\Projects\SASZombieAssaultTD\Tasklist.md"

(Get-Content $tasklist) |
    ForEach-Object {
        if ($_ -match "Game Loop \(the concrete slab\)") {
            $_ -replace "\[ \]", "[x]"
        } else {
            $_
        }
    } | Set-Content $tasklist -Encoding UTF8

Write-Host "Updated Tasklist.md (Game Loop checked off)"

# ------------------------
# DONE
# ------------------------
Write-Host "Change0002.ps1 completed successfully."
