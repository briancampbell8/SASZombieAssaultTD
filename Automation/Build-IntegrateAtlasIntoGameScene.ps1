# =====================================================================
# Build-IntegrateAtlasIntoGameScene.ps1
# Integrates TextureAtlas + SpriteBatch into GameScene.cs
# =====================================================================

param(
    [string]$ProjectRoot = "E:\BDC\Projects\SASZombieAssaultTD"
)

$engineRoot = Join-Path $ProjectRoot "SASZombieAssaultTD\Engine"
$scenePath  = Join-Path $engineRoot "Scenes"
$logPath    = Join-Path $ProjectRoot "PowerShellLog.md"

New-Item -ItemType Directory -Force -Path $scenePath | Out-Null

# ---------------------------------------------------------------------
# New GameScene.cs (Atlas + SpriteBatch integrated)
# ---------------------------------------------------------------------
$gameSceneContent = @"
using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.Scenes;

namespace SASZombieAssaultTD.Engine.Scenes
{
    public sealed class GameScene : Scene
    {
        private TextureAtlas? _atlas;
        private SpriteBatch _batch = new SpriteBatch();
        private double _t;

        public override void Init()
        {
            // Build atlas from Assets/Sprites folder
            var builder = new AtlasBuilder();
            _atlas = builder.Build(@"Assets\Sprites");

            _t = 0.0;
        }

        public override void Update()
        {
            _t += 1.0;
        }

        public override void Render(RenderQueue rq)
        {
            // Background
            rq.EnqueueFillRect(0, 0, 800, 600, unchecked((int)0xFF1A1A1A));

            if (_atlas != null)
            {
                // Example sprite: soldier
                var soldier = _atlas.Get("rifleman");
                if (soldier != null)
                {
                    int dx = 100 + (int)(System.Math.Sin(_t * 0.02) * 20);
                    int dy = 100;

                    _batch.Enqueue(_atlas.AtlasTexture, soldier, dx, dy);
                }

                // Flush batched sprites into framebuffer
                _batch.Flush(rq.Framebuffer);
            }
        }
    }
}
"@

$targetFile = Join-Path $scenePath "GameScene.cs"
$gameSceneContent | Set-Content -Path $targetFile -Encoding ASCII

# ---------------------------------------------------------------------
# Logging
# ---------------------------------------------------------------------
$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
$logEntry = @"
## Build-IntegrateAtlasIntoGameScene.ps1 — $timestamp
- Updated GameScene.cs to use TextureAtlas + SpriteBatch.
- Added atlas loading, sprite lookup, and batched rendering.
- Mode: overwrite, ASCII, deterministic, PowerShell-driven.
"@

Add-Content -Path $logPath -Value $logEntry

Write-Host "GameScene.cs updated with atlas integration and logged successfully."
