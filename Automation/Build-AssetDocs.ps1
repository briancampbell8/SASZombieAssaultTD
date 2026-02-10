# Build-AssetDocs.ps1
param(
    [string]$RootPath
)

if (-not $RootPath -or -not (Test-Path $RootPath)) {
    $RootPath = Split-Path -Parent $PSCommandPath
    while ($RootPath -and -not (Test-Path (Join-Path $RootPath 'SASZombieAssaultTD.csproj'))) {
        $parent = Split-Path -Parent $RootPath
        if ($parent -eq $RootPath) { break }
        $RootPath = $parent
    }
}

if (-not (Test-Path (Join-Path $RootPath 'SASZombieAssaultTD.csproj'))) {
    Write-Error "Could not locate project root from $RootPath"
    exit 1
}

$docsDir = Join-Path $RootPath 'Docs'
New-Item -ItemType Directory -Path $docsDir -Force | Out-Null

$assetPipelinePath = Join-Path $docsDir 'AssetPipelineInit.md'
$renderingPath = Join-Path $docsDir 'RenderingSystem.md'
$sceneSystemPath = Join-Path $docsDir 'SceneSystem.md'
$debugDiagPath = Join-Path $docsDir 'DebugDiagnostics.md'

$assetPipelineContent = @'
Asset Pipeline Initialization
=============================

Overview
--------

The asset pipeline is responsible for discovering, loading, and registering
all textures and JSON data required by the engine.

Flow
----

1. AssetDiscovery scans the configured asset root.
2. TextureLoader loads all supported texture files.
3. DataLoader parses JSON configuration and gameplay data.
4. AssetRegistry exposes a unified lookup surface to the rest of the engine.
'@

$renderingContent = @'
Rendering System
================

Overview
--------

The rendering system provides a framework-agnostic abstraction for drawing
frames, managing render surfaces, and coordinating batched sprite rendering.

Key Components
--------------

- IRenderContext: Minimal interface for issuing draw commands.
- RenderSurface: Optional off-screen target for advanced effects.
- RenderQueue / SpriteBatch: Ordered, batched draw submission.
'@

$sceneSystemContent = @'
Scene System
============

Overview
--------

The scene system organizes game flow into discrete scenes (main menu, game,
pause, etc.) and coordinates transitions between them.

Architecture
------------

- Scene: Base class with Initialize, Update, Render.
- SceneManager: Owns the active scene stack.
- SceneStack / SceneTransition: Push/pop/replace semantics for navigation.
'@

$debugDiagContent = @'
Debug Diagnostics
=================

Overview
--------

The diagnostics subsystem provides lightweight runtime insight into engine
health, frame timing, and logging.

Components
----------

- FrameStats: Tracks delta time and FPS.
- HeartbeatMonitor: Detects stalls in the main loop.
- DebugLogger: Centralized, buffered logging surface.
'@

Set-Content -Path $assetPipelinePath -Value $assetPipelineContent -Encoding ASCII
Set-Content -Path $renderingPath -Value $renderingContent -Encoding ASCII
Set-Content -Path $sceneSystemPath -Value $sceneSystemContent -Encoding ASCII
Set-Content -Path $debugDiagPath -Value $debugDiagContent -Encoding ASCII

Write-Host "Build-AssetDocs.ps1 — Wrote AssetPipelineInit.md, RenderingSystem.md, SceneSystem.md, DebugDiagnostics.md (ASCII, deterministic)."
