# =====================================================================
# RenderingEnhancements-Build.ps1 (Corrected)
# Writes rendering systems into the REAL Engine folder
# =====================================================================

# The correct engine root based on your .csproj structure
$engineRoot   = "E:\BDC\Projects\SASZombieAssaultTD\SASZombieAssaultTD\Engine"
$renderRoot   = Join-Path $engineRoot "Rendering"
$systemsRoot  = Join-Path $engineRoot "Systems"
$diagRoot     = Join-Path $systemsRoot "Diagnostics"

New-Item -ItemType Directory -Force -Path $renderRoot  | Out-Null
New-Item -ItemType Directory -Force -Path $systemsRoot | Out-Null
New-Item -ItemType Directory -Force -Path $diagRoot    | Out-Null

# ---------------------------------------------------------------------
# 1. Framebuffer.cs (with primitives)
# ---------------------------------------------------------------------
@"
namespace SASZombieAssaultTD.Engine.Rendering
{
    public sealed class Framebuffer
    {
        private readonly int _width;
        private readonly int _height;
        private readonly int[] _pixels;

        public int Width  => _width;
        public int Height => _height;
        public int[] Pixels => _pixels;

        public Framebuffer(int width, int height)
        {
            _width  = width;
            _height = height;
            _pixels = new int[width * height];
        }

        public void Clear(int color)
        {
            for (int i = 0; i < _pixels.Length; i++)
                _pixels[i] = color;
        }

        private void PutPixel(int x, int y, int color)
        {
            if ((uint)x >= (uint)_width || (uint)y >= (uint)_height)
                return;

            _pixels[y * _width + x] = color;
        }

        public void DrawRect(int x, int y, int w, int h, int color)
        {
            for (int yy = y; yy < y + h; yy++)
            {
                for (int xx = x; xx < x + w; xx++)
                {
                    PutPixel(xx, yy, color);
                }
            }
        }

        public void DrawLine(int x0, int y0, int x1, int y1, int color)
        {
            int dx = System.Math.Abs(x1 - x0);
            int sx = x0 < x1 ? 1 : -1;
            int dy = -System.Math.Abs(y1 - y0);
            int sy = y0 < y1 ? 1 : -1;
            int err = dx + dy;

            while (true)
            {
                PutPixel(x0, y0, color);
                if (x0 == x1 && y0 == y1) break;
                int e2 = 2 * err;
                if (e2 >= dy) { err += dy; x0 += sx; }
                if (e2 <= dx) { err += dx; y0 += sy; }
            }
        }

        public void DrawCircle(int cx, int cy, int radius, int color)
        {
            int x = radius;
            int y = 0;
            int err = 1 - x;

            while (x >= y)
            {
                PutPixel(cx + x, cy + y, color);
                PutPixel(cx + y, cy + x, color);
                PutPixel(cx - y, cy + x, color);
                PutPixel(cx - x, cy + y, color);
                PutPixel(cx - x, cy - y, color);
                PutPixel(cx - y, cy - x, color);
                PutPixel(cx + y, cy - x, color);
                PutPixel(cx + x, cy - y, color);

                y++;
                if (err < 0) err += 2 * y + 1;
                else { x--; err += 2 * (y - x + 1); }
            }
        }

        public void FillCircle(int cx, int cy, int radius, int color)
        {
            int r2 = radius * radius;
            for (int y = -radius; y <= radius; y++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    if (x * x + y * y <= r2)
                        PutPixel(cx + x, cy + y, color);
                }
            }
        }
    }
}
"@ | Set-Content (Join-Path $renderRoot "Framebuffer.cs") -Encoding ASCII

# ---------------------------------------------------------------------
# 2. Texture2D.cs (PNG loader + blit)
# ---------------------------------------------------------------------
@"
using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace SASZombieAssaultTD.Engine.Rendering
{
    public sealed class Texture2D
    {
        public int Width  { get; }
        public int Height { get; }
        public int[] Pixels { get; }

        private Texture2D(int width, int height, int[] pixels)
        {
            Width  = width;
            Height = height;
            Pixels = pixels;
        }

        public static Texture2D FromPng(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("Texture not found", path);

            using var bmp = new Bitmap(path);
            int w = bmp.Width;
            int h = bmp.Height;
            int[] data = new int[w * h];

            var rect = new Rectangle(0, 0, w, h);
            var bmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            unsafe
            {
                byte* src = (byte*)bmpData.Scan0;
                int stride = bmpData.Stride;

                for (int y = 0; y < h; y++)
                {
                    byte* row = src + y * stride;
                    for (int x = 0; x < w; x++)
                    {
                        int b = row[x * 4 + 0];
                        int g = row[x * 4 + 1];
                        int r = row[x * 4 + 2];
                        int a = row[x * 4 + 3];
                        data[y * w + x] = (a << 24) | (r << 16) | (g << 8) | b;
                    }
                }
            }

            bmp.UnlockBits(bmpData);
            return new Texture2D(w, h, data);
        }

        public void Blit(Framebuffer fb, int dstX, int dstY)
        {
            for (int y = 0; y < Height; y++)
            {
                int fy = dstY + y;
                if (fy < 0 || fy >= fb.Height) continue;

                for (int x = 0; x < Width; x++)
                {
                    int fx = dstX + x;
                    if (fx < 0 || fx >= fb.Width) continue;

                    int srcColor = Pixels[y * Width + x];
                    int a = (srcColor >> 24) & 0xFF;
                    if (a == 0) continue;

                    fb.Pixels[fy * fb.Width + fx] = srcColor;
                }
            }
        }
    }
}
"@ | Set-Content (Join-Path $renderRoot "Texture2D.cs") -Encoding ASCII

# ---------------------------------------------------------------------
# 3. TextRenderer.cs (stub)
# ---------------------------------------------------------------------
@"
namespace SASZombieAssaultTD.Engine.Rendering
{
    public sealed class TextRenderer
    {
        public void DrawString(Framebuffer fb, int x, int y, string text, int color)
        {
            // TODO: Implement bitmap font rendering
        }
    }
}
"@ | Set-Content (Join-Path $renderRoot "TextRenderer.cs") -Encoding ASCII

# ---------------------------------------------------------------------
# 4. RenderQueue.cs
# ---------------------------------------------------------------------
@"
using System;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Rendering
{
    public enum RenderCommandType
    {
        FillRect,
        Line,
        Circle,
        Texture
    }

    public readonly struct RenderCommand
    {
        public RenderCommandType Type { get; }
        public int X0 { get; }
        public int Y0 { get; }
        public int X1 { get; }
        public int Y1 { get; }
        public int Radius { get; }
        public int Color { get; }
        public Texture2D? Texture { get; }

        public RenderCommand(RenderCommandType type, int x0, int y0, int x1, int y1, int radius, int color, Texture2D? texture)
        {
            Type    = type;
            X0      = x0;
            Y0      = y0;
            X1      = x1;
            Y1      = y1;
            Radius  = radius;
            Color   = color;
            Texture = texture;
        }
    }

    public sealed class RenderQueue
    {
        private readonly List<RenderCommand> _commands = new();

        public void Clear() => _commands.Clear();

        public void EnqueueFillRect(int x, int y, int w, int h, int color)
        {
            _commands.Add(new RenderCommand(RenderCommandType.FillRect, x, y, w, h, 0, color, null));
        }

        public void EnqueueLine(int x0, int y0, int x1, int y1, int color)
        {
            _commands.Add(new RenderCommand(RenderCommandType.Line, x0, y0, x1, y1, 0, color, null));
        }

        public void EnqueueCircle(int cx, int cy, int radius, int color)
        {
            _commands.Add(new RenderCommand(RenderCommandType.Circle, cx, cy, 0, 0, radius, color, null));
        }

        public void EnqueueTexture(Texture2D tex, int x, int y)
        {
            _commands.Add(new RenderCommand(RenderCommandType.Texture, x, y, 0, 0, 0, 0, tex));
        }

        public void Flush(Framebuffer fb)
        {
            foreach (var cmd in _commands)
            {
                switch (cmd.Type)
                {
                    case RenderCommandType.FillRect:
                        fb.DrawRect(cmd.X0, cmd.Y0, cmd.X1, cmd.Y1, cmd.Color);
                        break;

                    case RenderCommandType.Line:
                        fb.DrawLine(cmd.X0, cmd.Y0, cmd.X1, cmd.Y1, cmd.Color);
                        break;

                    case RenderCommandType.Circle:
                        fb.DrawCircle(cmd.X0, cmd.Y0, cmd.Radius, cmd.Color);
                        break;

                    case RenderCommandType.Texture:
                        cmd.Texture?.Blit(fb, cmd.X0, cmd.Y0);
                        break;
                }
            }
        }
    }
}
"@ | Set-Content (Join-Path $renderRoot "RenderQueue.cs") -Encoding ASCII

# ---------------------------------------------------------------------
# 5. Timing.cs (FrameTimer + FpsCounter)
# ---------------------------------------------------------------------
@"
using System;
using System.Diagnostics;

namespace SASZombieAssaultTD.Engine.Systems.Diagnostics
{
    public sealed class FrameTimer
    {
        private readonly Stopwatch _sw = new Stopwatch();
        private double _lastFrameTime;

        public double DeltaTime { get; private set; }

        public void Start()
        {
            _sw.Restart();
            _lastFrameTime = 0.0;
            DeltaTime = 0.0;
        }

        public void Tick()
        {
            double now = _sw.Elapsed.TotalSeconds;
            DeltaTime = now - _lastFrameTime;
            _lastFrameTime = now;
        }
    }

    public sealed class FpsCounter
    {
        private double _accum;
        private int _frames;
        private double _fps;

        public double Fps => _fps;

        public void Update(double deltaTime)
        {
            _accum += deltaTime;
            _frames++;

            if (_accum >= 1.0)
            {
                _fps = _frames / _accum;
                _accum = 0.0;
                _frames = 0;
            }
        }
    }
}
"@ | Set-Content (Join-Path $diagRoot "Timing.cs") -Encoding ASCII

Write-Host "Rendering enhancements written to the correct Engine folder."
