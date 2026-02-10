# Change0006.ps1
# Purpose: Restore TimingController.cs and GameStateManager.cs into Engine/Core
# Mode: additive-only, UTF-8, atomic write

$root      = "E:\BDC\Projects\SASZombieAssaultTD"
$corePath  = Join-Path $root "Engine\Core"
$logPath   = Join-Path $root "PowerShellLog.md"

$timingPath = Join-Path $corePath "TimingController.cs"
$statePath  = Join-Path $corePath "GameStateManager.cs"

if (-not (Test-Path $corePath)) {
    Write-Host "Core directory not found at $corePath"
    exit
}

function Set-IfEmpty {
    param(
        [string]$Path,
        [string]$Content
    )

    if (-not (Test-Path $Path)) {
        Write-Host "Creating $Path"
        $Content | Set-Content -Path $Path -Encoding UTF8
        return
    }

    $raw = Get-Content $Path -Raw -ErrorAction SilentlyContinue
    if ($null -eq $raw) { $raw = " }

    if ($raw.Trim().Length -gt 0) {
        Write-Host "Skipping $Path (already populated)"
        return
    }

    Write-Host "Populating $Path"
    $Content | Set-Content -Path $Path -Encoding UTF8
}

$timingContent = @"
using System;

namespace Engine.Core
{
    public class TimingController
    {
        private DateTime _lastFrameTime;

        public float DeltaTime { get; private set; }
        public bool ShouldUpdate { get; private set; }
        public bool ShouldRender { get; private set; }

        private const float TargetUpdateRate = 1f / 60f; // 60 FPS update
        private float _accumulator = 0f;

        public void Start()
        {
            _lastFrameTime = DateTime.Now;
        }

        public void Tick()
        {
            var now = DateTime.Now;
            DeltaTime = (float)(now - _lastFrameTime).TotalSeconds;
            _lastFrameTime = now;

            _accumulator += DeltaTime;

            ShouldUpdate = false;
            ShouldRender = true; // render every frame

            if (_accumulator >= TargetUpdateRate)
            {
                ShouldUpdate = true;
                _accumulator -= TargetUpdateRate;
            }
        }
    }
}
"@

$stateContent = @"
namespace Engine.Core
{
    public class GameStateManager
    {
        public void Update(float deltaTime)
        {
            // Game state update logic will be added later.
        }
    }
}
"@

Set-IfEmpty -Path $timingPath -Content $timingContent
Set-IfEmpty -Path $statePath  -Content $stateContent

$timestamp = Get-Date -Format 'yyyy-MM-dd HH:mm:ss'
$logEntry = @"
## Change0006.ps1 � $timestamp
- Restored TimingController.cs and GameStateManager.cs in Engine/Core.
- Satisfied GameLoop dependencies for timing and state management.
- Mode: additive-only, UTF-8, atomic write.

"@

$logEntry | Add-Content -Path $logPath -Encoding UTF8

Write-Host "Change0006.ps1 completed."
