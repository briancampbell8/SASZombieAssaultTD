$root = "E:\BDC\Projects\SASZombieAssaultTD\Engine"

function Write-File($path, $content) {
    $dir = Split-Path $path
    if (!(Test-Path $dir)) { New-Item -ItemType Directory -Path $dir -Force | Out-Null }
    $tmp = "$path.tmp"
    $content | Out-File -FilePath $tmp -Encoding utf8
    Move-Item -Force $tmp $path
}

# -------------------------
# Patch: AssetInitializer.cs
# -------------------------

Write-File "$root\Systems\Assets\AssetInitializer.cs" @"
namespace Engine.Systems.Assets
{
    public static class AssetInitializer
    {
        public static void Initialize()
        {
            AssetRegistry.Clear();
            AssetDiscovery.Discover();
            TextureLoader.LoadAll();
            DataLoader.LoadAll();
        }
    }
}
"@

# -------------------------
# Patch: DebugOverlay.cs
# -------------------------

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


"Patch complete: AssetInitializer.cs and DebugOverlay.cs have been overwritten with clean versions."
"@
