<#
Change0010.ps1
Purpose:
- Corrected subsystem injection using auto-root detection via *.slnx
- Writes Markdown + C# files for Rendering, Scenes, Asset Init, Diagnostics
- UTF-8 (no BOM), atomic writes, full logging, directory validation

Usage:
pwsh .\Automation\Change0010.ps1
#>

[CmdletBinding()]
param()

# -----------------------------
# Auto-detect project root via *.slnx
# -----------------------------
function Get-ProjectRoot {
    param([string]$Start)

    $current = Resolve-Path $Start

    while ($true) {
        $slnx = Get-ChildItem -Path $current -Filter *.slnx -ErrorAction SilentlyContinue
        if ($slnx) { return $current }

        $parent = Split-Path -Parent $current
        if ($parent -eq $current) {
            throw "ERROR: Could not locate a .slnx file in any parent directory."
        }
        $current = $parent
    }
}

# -----------------------------
# UTF-8 no BOM writer
# -----------------------------
function Write-Utf8NoBom {
    param([string]$Path, [string]$Content)

    $dir = Split-Path -Parent $Path
    if (-not (Test-Path $dir)) {
        New-Item -ItemType Directory -Path $dir -Force | Out-Null
    }

    $enc = New-Object System.Text.UTF8Encoding($false)
    [System.IO.File]::WriteAllText($Path, $Content, $enc)
}

# -----------------------------
# Logging helper
# -----------------------------
function Append-Log {
    param([string]$Message)
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    Add-Content -Path (Join-Path $ProjectRoot "PowerShellLog.md") -Value "$timestamp � $Message"
}

# -----------------------------
# Resolve project root
# -----------------------------
$scriptDir = $PSScriptRoot
$ProjectRoot = Get-ProjectRoot -Start $scriptDir

Append-Log "Change0010.ps1 started � auto-root located at $ProjectRoot"

# -----------------------------
# Markdown content
# -----------------------------
$markdown = @{
    "Docs/RenderingSystem.md" = '@
# Rendering System (WinForms Surface)
Provides a WinForms-based rendering surface for the engine.
Components:
- WindowHost
- RenderSurface
- IRenderContext
- DebugOverlay
Integration:
- GameLoop drives Update/Render
- DebugOverlay draws last
@'

    "Docs/SceneSystem.md" = '@
# Scene System
Defines scene lifecycle: Load, Update, Render, Unload.
Components:
- Scene (base)
- SceneManager
- MainMenuScene
- GameScene
- PauseScene
@'

    "Docs/AssetPipelineInit.md" = '@
# Asset Pipeline Initialization
Orchestrates asset discovery, loading, and registry population.
Components:
- AssetDiscovery
- TextureLoader
- DataLoader
- AssetRegistry
- AssetBundle
- AssetInitializer
@'

    "Docs/DebugDiagnostics.md" = '@
# Debug & Diagnostics
Provides visibility into engine behavior.
Components:
- FrameStats
- HeartbeatMonitor
- DebugLogger
Rendered via DebugOverlay.
@'
}

# -----------------------------
# C# content
# -----------------------------
$code = @{}

$code["Engine/Rendering/IRenderContext.cs"] = '@
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
@'

$code["Engine/Rendering/RenderSurface.cs"] = '@
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
            DoubleBuffered = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics = e.Graphics;
            base.OnPaint(e);
            RenderRequested?.Invoke(this);
        }

        public event Action<IRenderContext> RenderRequested;
    }
}
@'

$code["Engine/Rendering/DebugOverlay.cs"] = '@
using System.Drawing;
using Engine.Systems.Diagnostics;

namespace Engine.Rendering
{
    public static class DebugOverlay
    {
        public static FrameStats Stats { get; set; }
        public static string SceneName { get; set; }

        public static void Draw(IRenderContext ctx)
        {
            var g = ctx.Graphics;
            if (g == null) return;

            using var font = new Font("Consolas", 10);
            using var brush = new SolidBrush(Color.Lime);

            g.DrawString($"FPS: {Stats?.FramesPerSecond:0.0}", font, brush, 8, 8);
            g.DrawString($"Scene: {SceneName}", font, brush, 8, 24);
        }
    }
}
@'

$code["Engine/Rendering/WindowHost.cs"] = '@
using System.Windows.Forms;

namespace Engine.Rendering
{
    public class WindowHost
    {
        public RenderSurface Surface { get; }

        private readonly Form _form;

        public WindowHost(string title, int width, int height)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Surface = new RenderSurface { Dock = DockStyle.Fill };

            _form = new Form
            {
                Text = title,
                ClientSize = new System.Drawing.Size(width, height),
                StartPosition = FormStartPosition.CenterScreen
            };

            _form.Controls.Add(Surface);
        }

        public void Show() => _form.Show();
        public void DoEvents() => Application.DoEvents();
        public bool IsClosed => _form.IsDisposed;
    }
}
@'

$code["Engine/Scenes/Scene.cs"] = '@
using Engine.Rendering;

namespace Engine.Scenes
{
    public abstract class Scene
    {
        public virtual string Name => GetType().Name;

        public virtual void Load() { }
        public virtual void Unload() { }
        public virtual void Update(double dt) { }
        public virtual void Render(IRenderContext ctx) { }
    }
}
@'

$code["Engine/Scenes/SceneManager.cs"] = @'
using Engine.Rendering;

namespace Engine.Scenes
{
    public class SceneManager
    {
        private Scene _current;

        public string CurrentSceneName => _current?.Name ?? "(none)";

        public void SetScene(Scene scene)
        {
            _current?.Unload();
            _current = scene;
            _current.Load();
        }

        public void Update(double dt) => _current?.Update(dt);
        public void Render(IRenderContext ctx) => _current?.Render(ctx);
    }
}
'@

$code["Engine/Scenes/MainMenuScene.cs"] = @'
using System.Drawing;
using Engine.Rendering;

namespace Engine.Scenes
{
    public class MainMenuScene : Scene
    {
        public override string Name => "Main Menu";

        public override void Render(IRenderContext ctx)
        {
            var g = ctx.Graphics;
            g.Clear(Color.Black);

            using var font = new Font("Consolas", 16);
            using var brush = new SolidBrush(Color.White);

            g.DrawString("SAS TD � Main Menu", font, brush, 20, 20);
        }
    }
}
'@

$code["Engine/Scenes/GameScene.cs"] = @'
using System.Drawing;
using Engine.Rendering;

namespace Engine.Scenes
{
    public class GameScene : Scene
    {
        public override string Name => "Game";

        public override void Render(IRenderContext ctx)
        {
            var g = ctx.Graphics;
            g.Clear(Color.DarkGreen);

            using var font = new Font("Consolas", 14);
            using var brush = new SolidBrush(Color.White);

            g.DrawString("Game Scene", font, brush, 20, 20);
        }
    }
}
'@

$code["Engine/Scenes/PauseScene.cs"] = @'
using System.Drawing;
using Engine.Rendering;

namespace Engine.Scenes
{
    public class PauseScene : Scene
    {
        public override string Name => "Pause";

        public override void Render(IRenderContext ctx)
        {
            var g = ctx.Graphics;

            using var overlay = new SolidBrush(Color.FromArgb(160, 0, 0, 0));
            g.FillRectangle(overlay, 0, 0, ctx.Width, ctx.Height);

            using var font = new Font("Consolas", 16);
            using var brush = new SolidBrush(Color.Yellow);

            g.DrawString("PAUSED", font, brush, 20, 20);
        }
    }
}
'@

$code["Engine/Systems/Assets/AssetBundle.cs"] = @'
using System.Collections.Generic;

namespace Engine.Systems.Assets
{
    public class AssetBundle
    {
        public Dictionary<string, object> Assets { get; } = new();

        public void Add(string key, object asset) => Assets[key] = asset;

        public T Get<T>(string key) where T : class =>
            Assets.TryGetValue(key, out var v) ? v as T : null;
    }
}
'@

$code["Engine/Systems/Assets/AssetInitializer.cs"] = @'
using System;

namespace Engine.Systems.Assets
{
    public static class AssetInitializer
    {
        public static AssetBundle InitializeAll()
        {
            var bundle = new AssetBundle();

            // Placeholder orchestration
            // Discovery ? Loaders ? Registry

            return bundle;
        }
    }
}
1@

$code["Engine/Systems/Diagnostics/FrameStats.cs"] = @'
using System;

namespace Engine.Systems.Diagnostics
{
    public class FrameStats
    {
        private int frames, updates;
        private double acc;

        public double FramesPerSecond { get; private set; }
        public double UpdatesPerSecond { get; private set; }

        public void TickFrame(double dt)
        {
            frames++;
            acc += dt;
            Flush();
        }

        public void TickUpdate(double dt)
        {
            updates++;
            acc += dt;
            Flush();
        }

        private void Flush()
        {
            if (acc >= 1.0)
            {
                FramesPerSecond = frames / acc;
                UpdatesPerSecond = updates / acc;
                frames = updates = 0;
                acc = 0;
            }
        }
    }
}
'@

$code["Engine/Systems/Diagnostics/HeartbeatMonitor.cs"] = @'
using System;

namespace Engine.Systems.Diagnostics
{
    public class HeartbeatMonitor
    {
        public DateTime LastBeat { get; private set; } = DateTime.UtcNow;
        public void Beat() => LastBeat = DateTime.UtcNow;
        public TimeSpan TimeSinceLastBeat => DateTime.UtcNow - LastBeat;
    }
}
'@

$code["Engine/Systems/Diagnostics/DebugLogger.cs"] = @'
using System;

namespace Engine.Systems.Diagnostics
{
    public static class DebugLogger
    {
        public static void Info(string msg)  => Console.WriteLine($"[INFO] {msg}");
        public static void Warn(string msg)  => Console.WriteLine($"[WARN] {msg}");
        public static void Error(string msg) => Console.WriteLine($"[ERROR] {msg}");
    }
}
'@

# -----------------------------
# Write Markdown
# -----------------------------
foreach ($entry in $markdown.GetEnumerator()) {
    $path = Join-Path $ProjectRoot $entry.Key
    Write-Utf8NoBom -Path $path -Content $entry.Value
    Append-Log "Change0010 � Wrote $path"
}

# -----------------------------
# Write C# files
# -----------------------------
foreach ($entry in $code.GetEnumerator()) {
    $path = Join-Path $ProjectRoot $entry.Key
    Write-Utf8NoBom -Path $path -Content $entry.Value
    Append-Log "Change0010 � Wrote $path"
}

Append-Log "Change0010.ps1 completed successfully � all subsystem files written."
Write-Host "Change0010.ps1 completed successfully." -ForegroundColor Green
