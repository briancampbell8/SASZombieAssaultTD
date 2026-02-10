$root = "E:\BDC\Projects\SASZombieAssaultTD\Engine"

function Write-File($path, $content) {
    $dir = Split-Path $path
    if (!(Test-Path $dir)) { New-Item -ItemType Directory -Path $dir -Force | Out-Null }
    $tmp = "$path.tmp"
    $content | Out-File -FilePath $tmp -Encoding utf8
    Move-Item -Force $tmp $path
}

# -------------------------
# Rendering Subsystem
# -------------------------

Write-File "$root\Rendering\IRenderContext.cs" @"
namespace Engine.Rendering
{
    public interface IRenderContext
    {
        void Begin();
        void End();
        void DrawText(string text, int x, int y);
        void DrawTexture(object texture, int x, int y);
        void Clear();
    }
}
"@

Write-File "$root\Rendering\RenderSurface.cs" @"
namespace Engine.Rendering
{
    public class RenderSurface
    {
        public int Width { get; }
        public int Height { get; }

        public RenderSurface(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public void Initialize() { }
        public void Resize(int width, int height) { }
    }
}
"@

Write-File "$root\Rendering\WindowHost.cs" @"
namespace Engine.Rendering
{
    public class WindowHost
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public string Title { get; private set; }

        public WindowHost(int width, int height, string title)
        {
            Width = width;
            Height = height;
            Title = title;
        }

        public void Initialize() { }
        public void PumpEvents() { }
        public void Present() { }
    }
}
"@

Write-File "$root\Rendering\DebugOverlay.cs" @"
using Engine.Systems.Diagnostics;

namespace Engine.Rendering
{
    public class DebugOverlay
    {
        public bool Enabled { get; set; } = true;

        public void Draw(IRenderContext context)
        {
            if (!Enabled)
                return;

            string fps = $"FPS: {FrameStats.FramesPerSecond}";
            string ups = $"UPS: {FrameStats.UpdatesPerSecond}";

            context.DrawText(fps, 10, 10);
            context.DrawText(ups, 10, 30);
        }
    }
}
"@

# -------------------------
# Scene Subsystem
# -------------------------

Write-File "$root\Scenes\Scene.cs" @"
namespace Engine.Scenes
{
    public abstract class Scene
    {
        public virtual void Initialize() { }
        public virtual void Update() { }
        public virtual void Render() { }
        public virtual void Shutdown() { }
    }
}
"@

Write-File "$root\Scenes\SceneManager.cs" @"
namespace Engine.Scenes
{
    public class SceneManager
    {
        private Scene? _current;

        public void SetScene(Scene scene)
        {
            _current?.Shutdown();
            _current = scene;
            _current.Initialize();
        }

        public void Update() => _current?.Update();
        public void Render() => _current?.Render();
    }
}
"@

Write-File "$root\Scenes\GameScene.cs" @"
namespace Engine.Scenes
{
    public class GameScene : Scene
    {
        public override void Initialize() { }
        public override void Update() { }
        public override void Render() { }
    }
}
"@

Write-File "$root\Scenes\MainMenuScene.cs" @"
namespace Engine.Scenes
{
    public class MainMenuScene : Scene
    {
        public override void Initialize() { }
        public override void Update() { }
        public override void Render() { }
    }
}
"@

Write-File "$root\Scenes\PauseScene.cs" @"
namespace Engine.Scenes
{
    public class PauseScene : Scene
    {
        public override void Initialize() { }
        public override void Update() { }
        public override void Render() { }
    }
}
"@

# -------------------------
# Diagnostics Subsystem
# -------------------------

Write-File "$root\Systems\Diagnostics\DebugLogger.cs" @"
namespace Engine.Systems.Diagnostics
{
    public static class DebugLogger
    {
        public static void Log(string message)
        {
            System.Console.WriteLine(message);
        }
    }
}
"@

Write-File "$root\Systems\Diagnostics\FrameStats.cs" @"
namespace Engine.Systems.Diagnostics
{
    public static class FrameStats
    {
        public static int FramesPerSecond { get; set; }
        public static int UpdatesPerSecond { get; set; }
    }
}
"@

Write-File "$root\Systems\Diagnostics\HeartbeatMonitor.cs" @"
namespace Engine.Systems.Diagnostics
{
    public static class HeartbeatMonitor
    {
        public static void Pulse() { }
    }
}
"@

# -------------------------
# Asset Subsystem
# -------------------------

Write-File "$root\Systems\Assets\AssetInitializer.cs" @"
namespace Engine.Systems.Assets
{
	
    public static class Asset
"@
