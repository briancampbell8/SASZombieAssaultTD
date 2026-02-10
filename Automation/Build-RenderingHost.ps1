# Build-RenderingHost.ps1
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

$renderDir = Join-Path $RootPath 'Engine\Rendering'
New-Item -ItemType Directory -Path $renderDir -Force | Out-Null

$irePath = Join-Path $renderDir 'IRenderContext.cs'
$surfacePath = Join-Path $renderDir 'RenderSurface.cs'
$windowHostPath = Join-Path $renderDir 'WindowHost.cs'

$ireContent = @'
using System;

namespace Engine.Rendering
{
    public interface IRenderContext : IDisposable
    {
        int BackBufferWidth { get; }
        int BackBufferHeight { get; }

        void Clear(float r, float g, float b, float a);
        void Begin();
        void End();
    }
}
'@

$surfaceContent = @'
using System;

namespace Engine.Rendering
{
    public sealed class RenderSurface : IDisposable
    {
        public int Width { get; }
        public int Height { get; }

        public RenderSurface(int width, int height)
        {
            if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
            if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));

            Width = width;
            Height = height;
        }

        public void Bind(IRenderContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            // TODO: Bind off-screen surface if the backend supports it.
        }

        public void Dispose()
        {
            // TODO: Release GPU resources if allocated.
        }
    }
}
'@

$windowHostContent = @'
using System;

namespace Engine.Rendering
{
    public sealed class WindowHost : IDisposable
    {
        public int Width { get; }
        public int Height { get; }
        public string Title { get; }

        public WindowHost(int width, int height, string title)
        {
            if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
            if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));

            Width = width;
            Height = height;
            Title = title ?? "SAS Zombie Assault TD";
        }

        public IRenderContext CreateRenderContext()
        {
            // TODO: Return a framework-specific render context implementation.
            throw new NotImplementedException("Render context creation must be provided by the host application.");
        }

        public void PumpEvents()
        {
            // TODO: Integrate with actual windowing/event system.
        }

        public void Dispose()
        {
            // TODO: Tear down window resources.
        }
    }
}
'@

Set-Content -Path $irePath -Value $ireContent -Encoding ASCII
Set-Content -Path $surfacePath -Value $surfaceContent -Encoding ASCII
Set-Content -Path $windowHostPath -Value $windowHostContent -Encoding ASCII

Write-Host "Build-RenderingHost.ps1 — Wrote IRenderContext.cs, RenderSurface.cs, WindowHost.cs (ASCII, deterministic)."
