# =====================================================================
# Build-DebugOverlay.ps1
# Generates DebugOverlay.cs and logs the operation
# =====================================================================

param(
    [string]$ProjectRoot = "E:\BDC\Projects\SASZombieAssaultTD"
)

$engineRoot   = Join-Path $ProjectRoot "SASZombieAssaultTD\Engine"
$renderPath   = Join-Path $engineRoot "Rendering"
$logPath      = Join-Path $ProjectRoot "PowerShellLog.md"

New-Item -ItemType Directory -Force -Path $renderPath | Out-Null

$debugOverlayContent = @"
using System;
using SASZombieAssaultTD.Engine.Systems.Diagnostics;

namespace SASZombieAssaultTD.Engine.Rendering
{
    public sealed class DebugOverlay
    {
        private readonly TextRenderer _textRenderer = new TextRenderer();

        public void Render(Framebuffer framebuffer, RenderQueue renderQueue, double fps)
        {
            // Background panel (semi-transparent dark)
            renderQueue.EnqueueFillRect(8, 8, 180, 48, unchecked((int)0x80000000));

            // Optional border
            renderQueue.EnqueueLine(8, 8, 188, 8, unchecked((int)0xFFFFFFFF));
            renderQueue.EnqueueLine(188, 8, 188, 56, unchecked((int)0xFFFFFFFF));
            renderQueue.EnqueueLine(188, 56, 8, 56, unchecked((int)0xFFFFFFFF));
            renderQueue.EnqueueLine(8, 56, 8, 8, unchecked((int)0xFFFFFFFF));

            // FPS text (uses TextRenderer stub for now)
            string fpsText = $"FPS: {fps:F1}";
            _textRenderer.DrawString(framebuffer, 16, 20, fpsText, unchecked((int)0xFFFFFFFF));
        }
    }
}
"@

$targetFile = Join-Path $renderPath "DebugOverlay.cs"
$debugOverlayContent | Set-Content -Path $targetFile -Encoding ASCII

$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
$logEntry = @"
## Build-DebugOverlay.ps1 — $timestamp
- Wrote Engine\Rendering\DebugOverlay.cs with FPS overlay implementation.
- Mode: overwrite, ASCII, deterministic, PowerShell-driven.
"@

Add-Content -Path $logPath -Value $logEntry

Write-Host "DebugOverlay.cs written to $targetFile and logged to PowerShellLog.md"
