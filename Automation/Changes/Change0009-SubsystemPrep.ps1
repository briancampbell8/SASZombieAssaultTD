<#
Change0009-SubsystemPrep.ps1
Purpose:
- Generate design markdowns for upcoming subsystems
- Create and populate C# files for:
  - Rendering (WinForms)
  - Scene system
  - Asset pipeline initialization
  - Diagnostics
- Write UTF-8 (no BOM), additive-only
- Append operation summary to PowerShellLog.md

Usage:
- Place in /Automation
- Run from project root:
  pwsh .\Automation\Change0009-SubsystemPrep.ps1
#>

[CmdletBinding()]
param()

# -----------------------------
# Helpers
# -----------------------------
function Write-Utf8NoBom {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [Parameter(Mandatory = $true)][string]$Content
    )

    $directory = Split-Path -Parent $Path
    if (-not [string]::IsNullOrWhiteSpace($directory) -and -not (Test-Path $directory)) {
        New-Item -ItemType Directory -Path $directory -Force | Out-Null
    }

    $encoding = New-Object System.Text.UTF8Encoding($false)
    [System.IO.File]::WriteAllText($Path, $Content, $encoding)
}

function Append-Log {
    param(
        [Parameter(Mandatory = $true)][string]$Message
    )

    $logPath = Join-Path -Path $ProjectRoot -ChildPath "PowerShellLog.md"
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $line = "$timestamp � $Message"
    Add-Content -Path $logPath -Value $line
}

# -----------------------------
# Project root resolution
# -----------------------------
$scriptDir   = Split-Path -Parent $MyInvocation.MyCommand.Path
$ProjectRoot = Split-Path -Parent $scriptDir

Push-Location $ProjectRoot
try {
    Append-Log "Change0009-SubsystemPrep.ps1 started � staging rendering, scenes, asset init, diagnostics."

    # -------------------------
    # 1. Markdown Design Docs
    # -------------------------
    $docsDir = Join-Path $ProjectRoot "Docs"

    $renderingMd = @"
# Rendering System (WinForms Surface)

## Purpose
Provide a WinForms-based rendering surface for the engine, suitable for 2D tower-defense visuals and debug overlays.

## Components
- WindowHost: Owns the WinForms Form and message loop.
- RenderSurface: Custom control used as the drawing surface.
- IRenderContext: Abstraction over drawing operations.
- DebugOverlay: Renders frame stats and engine heartbeat.

## Integration
- GameRoot constructs WindowHost and passes engine callbacks.
- GameLoop drives Update/Render; RenderSurface invalidates and repaints.
- DebugOverlay draws last to ensure visibility.

## Future Evolution
- Swap GDI+ drawing with GPU-backed renderer if needed.
- Add camera, layers, and sprite batching on top of this surface.
"@

    $sceneMd = @"
# Scene System

## Purpose
Provide a structured way to manage game states:
- Main menu
- In-game
- Pause

## Components
- Scene (base): Defines Load, Unload, Update, Render.
- SceneManager: Owns the active scene and handles transitions.
- MainMenuScene, GameScene, PauseScene: Concrete implementations.

## Lifecycle
1. SceneManager sets initial scene.
2. GameLoop calls SceneManager.Update(deltaTime).
3. GameLoop calls SceneManager.Render(renderContext).
4. Scenes request transitions via SceneManager.

## Notes
- Scenes should not know about WinForms directly.
- Rendering is done via IRenderContext to keep things engine-agnostic.
"@

    $assetInitMd = @"
# Asset Pipeline Initialization

## Purpose
Wire the existing asset subsystems into a single initialization flow.

## Components
- AssetDiscovery: Scans asset directories.
- TextureLoader: Loads texture binaries.
- DataLoader: Loads JSON data.
- AssetRegistry: Stores loaded assets.
- AssetBundle: Convenience container for grouped assets.
- AssetInitializer: Orchestrates the initialization.

## Flow
1. AssetInitializer.Init() is called during engine startup.
2. Discovery finds assets under /Assets.
3. TextureLoader and DataLoader load resources.
4. AssetRegistry is populated.
5. Scenes query assets via AssetRegistry.

## Notes
- Initialization should be done before the window is shown.
- Failures should be logged via DebugLogger.
"@

    $debugMd = @"
# Debug & Diagnostics

## Purpose
Provide visibility into engine behavior during development.

## Components
- FrameStats: Tracks FPS, UPS, and delta time.
- HeartbeatMonitor: Simple health indicator for the main loop.
- DebugLogger: Lightweight logging abstraction.

## Usage
- FrameStats updated each frame and rendered via DebugOverlay.
- HeartbeatMonitor can be used to detect stalls or hangs.
- DebugLogger writes to console or future log targets.

## Visual Studio Workflow
- Run in Debug Mode.
- Use breakpoints in scenes and rendering.
- Compare behavior against reference footage and documentation.
"@

    Write-Utf8NoBom -Path (Join-Path $docsDir "RenderingSystem.md")     -Content $renderingMd
    Write-Utf8NoBom -Path (Join-Path $docsDir "SceneSystem.md")         -Content $sceneMd
    Write-Utf8NoBom -Path (Join-Path $docsDir "AssetPipelineInit.md")   -Content $assetInitMd
    Write-Utf8NoBom -Path (Join-Path $docsDir "DebugDiagnostics.md")    -Content $debugMd

    Append-Log "Change0009 � Generated RenderingSystem.md, SceneSystem.md, AssetPipelineInit.md, DebugDiagnostics.md."

    # -------------------------
    # 2. Rendering Subsystem
    # -------------------------
    $renderingDir = Join-Path $ProjectRoot "Engine\Rendering"

    $iRenderContextCs = @"
using System.Drawing;

namespace Engine.Rendering
{
    public interface IRenderContext
    {
        Graphics Graphics { get; }
        int Width { get; }
        int Height { get; }
    }
}
"@

    $renderSurfaceCs = @"
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Engine.Rendering
{
    public class RenderSurface : Control, IRenderContext
    {
        public Graphics Graphics { get; private set; }

        public RenderSurface()
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint |
                          ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics = e.Graphics;
            base.OnPaint(e);
            OnRender(e.Graphics);
        }

        public event Action<IRenderContext>? RenderRequested;

        private void OnRender(Graphics g)
        {
            RenderRequested?.Invoke(this);
        }
    }
}
"@

    $debugOverlayCs = @"
using System;
using System.Drawing;
using Engine.Systems.Diagnostics;

namespace Engine.Rendering
{
    public static class DebugOverlay
    {
        public static FrameStats? FrameStats { get; set; }
        public static string? CurrentSceneName { get; set; }

        public static void Draw(IRenderContext context)
        {
            if (context.Graphics == null) return;

            var g = context.Graphics;
            using var font = new Font("Consolas", 10);
            using var brush = new SolidBrush(Color.Lime);
            using var shadow = new SolidBrush(Color.FromArgb(160, 0, 0, 0));

            string line1 = FrameStats != null
                ? $"FPS: {FrameStats.FramesPerSecond:0.0}  UPS: {FrameStats.UpdatesPerSecond:0.0}"
                : "FPS: --  UPS: --";

            string line2 = !string.IsNullOrWhiteSpace(CurrentSceneName)
                ? $"Scene: {CurrentSceneName}"
                : "Scene: (none)";

            var x = 8;
            var y = 8;

            // Shadow
            g.DrawString(line1, font, shadow, x + 1, y + 1);
            g.DrawString(line2, font, shadow, x + 1, y + 17);

            // Text
            g.DrawString(line1, font, brush, x, y);
            g.DrawString(line2, font, brush, x, y + 16);
        }
    }
}
"@

    $windowHostCs = @"
using System;
using System.Windows.Forms;

namespace Engine.Rendering
{
    public class WindowHost
    {
        private readonly RenderSurface _surface;
        private readonly Form _form;

        public RenderSurface Surface => _surface;

        public WindowHost(string title, int width, int height)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            _surface = new RenderSurface
            {
                Dock = DockStyle.Fill
            };

            _form = new Form
            {
                Text = title,
                ClientSize = new System.Drawing.Size(width, height),
                StartPosition = FormStartPosition.CenterScreen
            };

            _form.Controls.Add(_surface);
        }

        public void Show()
        {
            _form.Show();
        }

        public void DoEvents()
        {
            Application.DoEvents();
        }

        public bool IsClosed => _form.IsDisposed || !_form.Created;
    }
}
"@

    Write-Utf8NoBom -Path (Join-Path $renderingDir "IRenderContext.cs") -Content $iRenderContextCs
    Write-Utf8NoBom -Path (Join-Path $renderingDir "RenderSurface.cs")  -Content $renderSurfaceCs
    Write-Utf8NoBom -Path (Join-Path $renderingDir "DebugOverlay.cs")   -Content $debugOverlayCs
    Write-Utf8NoBom -Path (Join-Path $renderingDir "WindowHost.cs")     -Content $windowHostCs

    Append-Log "Change0009 � Populated rendering subsystem (IRenderContext, RenderSurface, DebugOverlay, WindowHost)."

    # -------------------------
    # 3. Scene System
    # -------------------------
    $scenesDir = Join-Path $ProjectRoot "Engine\Scenes"

    $sceneBaseCs = @"
using Engine.Rendering;

namespace Engine.Scenes
{
    public abstract class Scene
    {
        public virtual string Name => GetType().Name;

        public virtual void Load() { }
        public virtual void Unload() { }
        public virtual void Update(double deltaTime) { }
        public virtual void Render(IRenderContext context) { }
    }
}
"@

    $sceneManagerCs = @"
using System;
using Engine.Rendering;

namespace Engine.Scenes
{
    public class SceneManager
    {
        private Scene? _current;

        public Scene? Current => _current;

        public void SetScene(Scene scene)
        {
            _current?.Unload();
            _current = scene;
            _current.Load();
        }

        public void Update(double deltaTime)
        {
            _current?.Update(deltaTime);
        }

        public void Render(IRenderContext context)
        {
            _current?.Render(context);
        }

        public string CurrentSceneName => _current?.Name ?? "(none)";
    }
}
"@

    $mainMenuSceneCs = @"
using System.Drawing;
using Engine.Rendering;

namespace Engine.Scenes
{
    public class MainMenuScene : Scene
    {
        public override string Name => "Main Menu";

        public override void Render(IRenderContext context)
        {
            var g = context.Graphics;
            if (g == null) return;

            g.Clear(Color.Black);
            using var font = new Font("Consolas", 16);
            using var brush = new SolidBrush(Color.White);

            var text = "SAS TD � Main Menu (stub)";
            var size = g.MeasureString(text, font);
            var x = (context.Width - size.Width) / 2;
            var y = (context.Height - size.Height) / 2;

            g.DrawString(text, font, brush, x, y);
        }
    }
}
"@

    $gameSceneCs = @"
using System.Drawing;
using Engine.Rendering;

namespace Engine.Scenes
{
    public class GameScene : Scene
    {
        public override string Name => "Game";

        public override void Render(IRenderContext context)
        {
            var g = context.Graphics;
            if (g == null) return;

            g.Clear(Color.DarkGreen);
            using var font = new Font("Consolas", 14);
            using var brush = new SolidBrush(Color.White);

            var text = "SAS TD � Game Scene (stub)";
            var size = g.MeasureString(text, font);
            var x = (context.Width - size.Width) / 2;
            var y = (context.Height - size.Height) / 2;

            g.DrawString(text, font, brush, x, y);
        }
    }
}
"@

    $pauseSceneCs = @"
using System.Drawing;
using Engine.Rendering;

namespace Engine.Scenes
{
    public class PauseScene : Scene
    {
        public override string Name => "Pause";

        public override void Render(IRenderContext context)
        {
            var g = context.Graphics;
            if (g == null) return;

            using var overlay = new SolidBrush(Color.FromArgb(160, 0, 0, 0));
            g.FillRectangle(overlay, 0, 0, context.Width, context.Height);

            using var font = new Font("Consolas", 16);
            using var brush = new SolidBrush(Color.Yellow);

            var text = "PAUSED";
            var size = g.MeasureString(text, font);
            var x = (context.Width - size.Width) / 2;
            var y = (context.Height - size.Height) / 2;

            g.DrawString(text, font, brush, x, y);
        }
    }
}
"@

    Write-Utf8NoBom -Path (Join-Path $scenesDir "Scene.cs")          -Content $sceneBaseCs
    Write-Utf8NoBom -Path (Join-Path $scenesDir "SceneManager.cs")   -Content $sceneManagerCs
    Write-Utf8NoBom -Path (Join-Path $scenesDir "MainMenuScene.cs")  -Content $mainMenuSceneCs
    Write-Utf8NoBom -Path (Join-Path $scenesDir "GameScene.cs")      -Content $gameSceneCs
    Write-Utf8NoBom -Path (Join-Path $scenesDir "PauseScene.cs")     -Content $pauseSceneCs

    Append-Log "Change0009 � Populated scene system (Scene, SceneManager, MainMenuScene, GameScene, PauseScene)."

    # -------------------------
    # 4. Asset Pipeline Init
    # -------------------------
    $assetsDir = Join-Path $ProjectRoot "Engine\Systems\Assets"

    $assetBundleCs = @"
using System.Collections.Generic;

namespace Engine.Systems.Assets
{
    public class AssetBundle
    {
        public Dictionary<string, object> Assets { get; } = new();

        public void Add(string key, object asset)
        {
            Assets[key] = asset;
        }

        public T? Get<T>(string key) where T : class
        {
            return Assets.TryGetValue(key, out var value) ? value as T : null;
        }
    }
}
"@

    $assetInitializerCs = @"
using System;
using Engine.Systems.Assets;

namespace Engine.Systems.Assets
{
    public static class AssetInitializer
    {
        public static AssetBundle InitializeAll()
        {
            var bundle = new AssetBundle();

            // NOTE:
            // This is a placeholder orchestration layer.
            // It assumes AssetDiscovery, TextureLoader, DataLoader, and AssetRegistry
            // are already implemented and available in this namespace.

            try
            {
                // Example flow (to be wired in Monday):
                // var discovered = AssetDiscovery.Discover("Assets");
                // TextureLoader.LoadAll(discovered, bundle);
                // DataLoader.LoadAll(discovered, bundle);
                // AssetRegistry.RegisterBundle(bundle);
            }
            catch (Exception ex)
            {
                // Hook into DebugLogger once wired.
                Console.WriteLine($"[AssetInitializer] Error during initialization: {ex.Message}");
            }

            return bundle;
        }
    }
}
"@

    Write-Utf8NoBom -Path (Join-Path $assetsDir "AssetBundle.cs")     -Content $assetBundleCs
    Write-Utf8NoBom -Path (Join-Path $assetsDir "AssetInitializer.cs") -Content $assetInitializerCs

    Append-Log "Change0009 � Populated asset pipeline initialization (AssetBundle, AssetInitializer)."

    # -------------------------
    # 5. Diagnostics Layer
    # -------------------------
    $diagDir = Join-Path $ProjectRoot "Engine\Systems\Diagnostics"
    if (-not (Test-Path $diagDir)) {
        New-Item -ItemType Directory -Path $diagDir -Force | Out-Null
    }

    $frameStatsCs = @"
using System;

namespace Engine.Systems.Diagnostics
{
    public class FrameStats
    {
        private int _frameCount;
        private int _updateCount;
        private double _accumulator;

        public double FramesPerSecond { get; private set; }
        public double UpdatesPerSecond { get; private set; }

        public void TickFrame(double deltaTime)
        {
            _frameCount++;
            _accumulator += deltaTime;
            UpdateIfReady();
        }

        public void TickUpdate(double deltaTime)
        {
            _updateCount++;
            _accumulator += deltaTime;
            UpdateIfReady();
        }

        private void UpdateIfReady()
        {
            if (_accumulator >= 1.0)
            {
                FramesPerSecond = _frameCount / _accumulator;
                UpdatesPerSecond = _updateCount / _accumulator;
                _frameCount = 0;
                _updateCount = 0;
                _accumulator = 0;
            }
        }
    }
}
"@

    $heartbeatCs = @"
using System;

namespace Engine.Systems.Diagnostics
{
    public class HeartbeatMonitor
    {
        public DateTime LastBeat { get; private set; } = DateTime.UtcNow;

        public void Beat()
        {
            LastBeat = DateTime.UtcNow;
        }

        public TimeSpan TimeSinceLastBeat => DateTime.UtcNow - LastBeat;
    }
}
"@

    $debugLoggerCs = @"
using System;

namespace Engine.Systems.Diagnostics
{
    public static class DebugLogger
    {
        public static void Info(string message)
        {
            Console.WriteLine($"[INFO] {message}");
        }

        public static void Warn(string message)
        {
            Console.WriteLine($"[WARN] {message}");
        }

        public static void Error(string message)
        {
            Console.WriteLine($"[ERROR] {message}");
        }
    }
}
"@

    Write-Utf8NoBom -Path (Join-Path $diagDir "FrameStats.cs")    -Content $frameStatsCs
    Write-Utf8NoBom -Path (Join-Path $diagDir "HeartbeatMonitor.cs") -Content $heartbeatCs
    Write-Utf8NoBom -Path (Join-Path $diagDir "DebugLogger.cs")  -Content $debugLoggerCs

    Append-Log "Change0009 � Populated diagnostics layer (FrameStats, HeartbeatMonitor, DebugLogger)."

    # -------------------------
    # 6. Integration Notes (Manual Wiring Monday)
    # -------------------------
    Append-Log "Change0009 � NOTE: New subsystems created but not yet wired into GameRoot/GameLoop. Integration to be done in Debug Mode."

    Write-Host "Change0009-SubsystemPrep.ps1 completed successfully." -ForegroundColor Green
}
finally {
    Pop-Location
}
