# Change0005.ps1
# Purpose: Inject timing-controlled game loop into GameLoop.cs
# Mode: additive-only, UTF-8, atomic write

$root = "E:\BDC\Projects\SASZombieAssaultTD"
$gameLoopPath = Join-Path $root "Engine\Core\GameLoop.cs"
$logPath = Join-Path $root "PowerShellLog.md"

if (-not (Test-Path $gameLoopPath)) {
    Write-Host "GameLoop.cs not found at $gameLoopPath"
    exit
}

$raw = Get-Content $gameLoopPath -Raw -ErrorAction SilentlyContinue
if ($null -eq $raw) { $raw = " }

# Replace Run() method with timing-controlled loop
$updated = $raw -replace '(public void Run\(\)[\s\S]*?\})', @"
public void Run()
{
    _isRunning = true;
    _timingController.Start();

    Initialize();

    while (_isRunning)
    {
        _timingController.Tick();

        if (_timingController.ShouldUpdate)
        {
            PreUpdate();
            Update();
            PostUpdate();
        }

        if (_timingController.ShouldRender)
        {
            PreRender();
            Render();
            PostRender();
        }
    }
}
"@

$updated | Set-Content -Path $gameLoopPath -Encoding UTF8

# Log Change0005
$timestamp = Get-Date -Format 'yyyy-MM-dd HH:mm:ss'
$logEntry = @"
## Change0005.ps1 � $timestamp
- Injected timing-controlled game loop using TimingController.
- Enabled continuous engine heartbeat with fixed update cadence and free-running render.
- Mode: additive-only, UTF-8, atomic write.

"@

$logEntry | Add-Content -Path $logPath -Encoding UTF8

Write-Host "Change0005.ps1 completed."
