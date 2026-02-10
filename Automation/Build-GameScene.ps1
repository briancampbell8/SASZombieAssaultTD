# =====================================================================
# Build-GameScene.ps1
# Generates RenderQueue-powered GameScene.cs and logs the operation
# =====================================================================

param(
    [string]$ProjectRoot = "E:\BDC\Projects\SASZombieAssaultTD"
)

$engineRoot   = Join-Path $ProjectRoot "SASZombieAssaultTD\Engine"
$scenePath    = Join-Path $engineRoot "Scenes"
$logPath      = Join-Path $ProjectRoot "PowerShellLog.md"

New-Item -ItemType Directory -Force -Path $scenePath | Out-Null

$gameSceneContent = @"
using SASZombieAssaultTD.Engine.Rendering;

namespace SASZombieAssaultTD.Engine.Scenes
{
    public sealed class GameScene : Scene
    {
        private Texture2D? _soldier;
        private double _t;

        public override void Init()
        {
            string texPath = @"Assets\Sprites\Soldiers\rifleman.png";
            if (File.Exists(texPath))
                _soldier = Texture2D.FromPng(texPath);

            _t = 0.0;
        }

        public override void Update()
        {
            _t += 1.0;
        }

        public override void Render(RenderQueue rq)
        {
            rq.EnqueueFillRect(0, 0, 800, 600, unchecked((int)0xFF1A1A1A));

            int cx = 400 + (int)(System.Math.Sin(_t * 0.02) * 100);
            int cy = 300;
            rq.EnqueueCircle(cx, cy, 40, unchecked((int)0xFFFFAA00));

            rq.EnqueueLine(390, 300, 410, 300, unchecked((int)0xFFFFFFFF));
            rq.EnqueueLine(400, 290, 400, 310, unchecked((int)0xFFFFFFFF));

            if (_soldier != null)
                rq.EnqueueTexture(_soldier, 100, 100);
        }
    }
}
"@

$targetFile = Join-Path $scenePath "GameScene.cs"
$gameSceneContent | Set-Content -Path $targetFile -Encoding ASCII

# ---------------- Logging ----------------
$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
$logEntry = @"
## Build-GameScene.ps1 — $timestamp
- Wrote Engine\Scenes\GameScene.cs with RenderQueue-powered implementation.
- Mode: overwrite, ASCII, deterministic, PowerShell-driven.
"@

Add-Content -Path $logPath -Value $logEntry

Write-Host "GameScene.cs written to $targetFile and logged to PowerShellLog.md"
