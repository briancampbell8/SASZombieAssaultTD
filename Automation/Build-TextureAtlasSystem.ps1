# =====================================================================
# Build-TextureAtlasSystem.ps1
# Generates TextureAtlas.cs, SpriteInfo.cs, SpriteBatch.cs, AtlasBuilder.cs
# and logs the operation.
# =====================================================================

param(
    [string]$ProjectRoot = "E:\BDC\Projects\SASZombieAssaultTD"
)

$engineRoot = Join-Path $ProjectRoot "SASZombieAssaultTD\Engine"
$renderPath = Join-Path $engineRoot "Rendering"
$logPath    = Join-Path $ProjectRoot "PowerShellLog.md"

New-Item -ItemType Directory -Force -Path $renderPath | Out-Null

# ---------------------------------------------------------------------
# TextureAtlas.cs
# ---------------------------------------------------------------------
$textureAtlasContent = @"
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Rendering
{
    public sealed class TextureAtlas
    {
        public Texture2D AtlasTexture { get; }
        public Dictionary<string, SpriteInfo> Sprites { get; }

        public TextureAtlas(Texture2D atlasTexture, Dictionary<string, SpriteInfo> sprites)
        {
            AtlasTexture = atlasTexture;
            Sprites = sprites;
        }

        public SpriteInfo Get(string key)
        {
            return Sprites.TryGetValue(key, out var info) ? info : null;
        }
    }
}
"@

Set-Content -Path (Join-Path $renderPath "TextureAtlas.cs") -Value $textureAtlasContent -Encoding ASCII

# ---------------------------------------------------------------------
# SpriteInfo.cs
# ---------------------------------------------------------------------
$spriteInfoContent = @"
namespace SASZombieAssaultTD.Engine.Rendering
{
    public sealed class SpriteInfo
    {
        public int X { get; }
        public int Y { get; }
        public int Width { get; }
        public int Height { get; }

        public SpriteInfo(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
}
"@

Set-Content -Path (Join-Path $renderPath "SpriteInfo.cs") -Value $spriteInfoContent -Encoding ASCII

# ---------------------------------------------------------------------
# SpriteBatch.cs
# ---------------------------------------------------------------------
$spriteBatchContent = @"
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Rendering
{
    public sealed class SpriteBatch
    {
        private readonly List<(Texture2D tex, int sx, int sy, int sw, int sh, int dx, int dy)> _queue
            = new List<(Texture2D, int, int, int, int, int, int)>();

        public void Enqueue(Texture2D tex, SpriteInfo info, int dx, int dy)
        {
            _queue.Add((tex, info.X, info.Y, info.Width, info.Height, dx, dy));
        }

        public void Flush(Framebuffer fb)
        {
            foreach (var q in _queue)
            {
                fb.Blit(q.tex, q.sx, q.sy, q.sw, q.sh, q.dx, q.dy);
            }
            _queue.Clear();
        }
    }
}
"@

Set-Content -Path (Join-Path $renderPath "SpriteBatch.cs") -Value $spriteBatchContent -Encoding ASCII

# ---------------------------------------------------------------------
# AtlasBuilder.cs
# ---------------------------------------------------------------------
$atlasBuilderContent = @"
using System.Collections.Generic;
using System.IO;

namespace SASZombieAssaultTD.Engine.Rendering
{
    public sealed class AtlasBuilder
    {
        public TextureAtlas Build(string directory)
        {
            var files = Directory.GetFiles(directory, "*.png");
            var sprites = new Dictionary<string, SpriteInfo>();

            int x = 0;
            int y = 0;
            int rowHeight = 0;

            // First pass: load textures
            var textures = new List<(string key, Texture2D tex)>();
            foreach (var f in files)
            {
                var tex = Texture2D.FromPng(f);
                string key = Path.GetFileNameWithoutExtension(f);
                textures.Add((key, tex));
            }

            // Compute atlas size (simple row packer)
            int atlasWidth = 0;
            int atlasHeight = 0;

            foreach (var t in textures)
            {
                if (x + t.tex.Width > 2048)
                {
                    x = 0;
                    y += rowHeight;
                    rowHeight = 0;
                }

                sprites[t.key] = new SpriteInfo(x, y, t.tex.Width, t.tex.Height);

                x += t.tex.Width;
                rowHeight = System.Math.Max(rowHeight, t.tex.Height);

                atlasWidth = System.Math.Max(atlasWidth, x);
                atlasHeight = System.Math.Max(atlasHeight, y + rowHeight);
            }

            // Create atlas texture
            var atlas = new Texture2D(atlasWidth, atlasHeight);

            // Blit textures into atlas
            foreach (var t in textures)
            {
                var info = sprites[t.key];
                atlas.Blit(t.tex, 0, 0, t.tex.Width, t.tex.Height, info.X, info.Y);
            }

            return new TextureAtlas(atlas, sprites);
        }
    }
}
"@

Set-Content -Path (Join-Path $renderPath "AtlasBuilder.cs") -Value $atlasBuilderContent -Encoding ASCII

# ---------------------------------------------------------------------
# Logging
# ---------------------------------------------------------------------
$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
$logEntry = @"
## Build-TextureAtlasSystem.ps1 — $timestamp
- Wrote TextureAtlas.cs, SpriteInfo.cs, SpriteBatch.cs, AtlasBuilder.cs.
- Installed texture atlas + sprite batching subsystem.
- Mode: overwrite, ASCII, deterministic, PowerShell-driven.
"@

Add-Content -Path $logPath -Value $logEntry

Write-Host "Texture Atlas System written and logged successfully."
