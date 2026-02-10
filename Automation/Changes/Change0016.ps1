# ============================================
# Change0016.ps1
# Path Debug Overlay System
# Adds PathDebugRenderer + integrates into DebugOverlay + GameScene
# Deterministic, additive-only, ASCII-safe
# ============================================

$ChangeId = "Change0016"
$LogFile = "PowerShellLog.md"

function Log {
    param([string]$Message)
    Add-Content -Path $LogFile -Value "[$ChangeId] $Message"
}

Log "Starting $ChangeId"

# --------------------------------------------
# 1. Ensure folder structure exists
# --------------------------------------------

$folders = @(
    "Engine",
    "Engine/Rendering",
    "Engine/Rendering/Debug",
    "Engine/Scenes",
    "Scripts",
    "Scripts/Paths",
    "Scripts/Zombies",
    "Scripts/TestScenes",
    "GameData",
    "GameData/Paths"
)

foreach ($folder in $folders) {
    if (!(Test-Path $folder)) {
        New-Item -ItemType Directory -Path $folder | Out-Null
        Log "Created folder: $folder"
    }
}

# --------------------------------------------
# 2. Create new file: PathDebugRenderer.cs
# --------------------------------------------

$PathDebugFile = "Engine/Rendering/Debug/PathDebugRenderer.cs"

if (!(Test-Path $PathDebugFile)) {
    New-Item -ItemType File -Path $PathDebugFile | Out-Null
    Log "Created file: $PathDebugFile"
}

# --------------------------------------------
# 3. Inject PathDebugRenderer.cs content
# --------------------------------------------

$PathDebugCode = @"
using System;
using System.Numerics;
using System.Collections.Generic;

namespace Engine.Rendering.Debug
{
    // <$ChangeId>
    public class PathDebugRenderer
    {
        public PathDebugRenderer() {}

        public void DrawPath(IRenderContext ctx, List<Vector2> points)
        {
            if (points == null || points.Count < 2)
                return;

            for (int i = 0; i < points.Count - 1; i++)
            {
                ctx.DrawLine(points[i], points[i + 1], 255, 255, 0); // Yellow path
                ctx.DrawCircle(points[i], 4, 255, 0, 0); // Red waypoint
            }

            // Draw final waypoint
            ctx.DrawCircle(points[points.Count - 1], 4, 255, 0, 0);
        }
    }
    // </$ChangeId>
}
"@

Add-Content -Path $PathDebugFile -Value $PathDebugCode
Log "Injected $ChangeId into $PathDebugFile"

# --------------------------------------------
# 4. Inject debug overlay hook into DebugOverlay.cs
# --------------------------------------------

$DebugOverlayFile = "Engine/Rendering/DebugOverlay.cs"

if (Test-Path $DebugOverlayFile) {

    $DebugOverlayCode = @"
// <$ChangeId>
private Engine.Rendering.Debug.PathDebugRenderer pathDebug = new Engine.Rendering.Debug.PathDebugRenderer();
// </$ChangeId>
"@

    Add-Content -Path $DebugOverlayFile -Value $DebugOverlayCode
    Log "Injected $ChangeId into $DebugOverlayFile"
}
else {
    Log "WARNING: DebugOverlay.cs not found — skipping injection"
}

# --------------------------------------------
# 5. Inject path drawing call into GameScene.cs
# --------------------------------------------

$GameSceneFile = "Engine/Scenes/GameScene.cs"

if (Test-Path $GameSceneFile) {

    $GameSceneCode = @"
// <$ChangeId>
if (path != null)
{
    var pts = new List<System.Numerics.Vector2>();
    foreach (var wp in path.waypoints)
        pts.Add(new System.Numerics.Vector2(wp.x, wp.y));

    debugOverlay?.pathDebug?.DrawPath(ctx, pts);
}
// </$ChangeId>
"@

    Add-Content -Path $GameSceneFile -Value $GameSceneCode
    Log "Injected $ChangeId into $GameSceneFile"
}
else {
    Log "WARNING: GameScene.cs not found — skipping injection"
}

# --------------------------------------------
# 6. Completion
# --------------------------------------------

Log "Completed $ChangeId"