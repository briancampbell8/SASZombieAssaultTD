# =====================================================================
# Build-UIFramework.ps1
# Generates UIElementBase.cs, Panel.cs, Button.cs, Label.cs, LayoutSystem.cs
# and logs the operation.
# =====================================================================

param(
    [string]$ProjectRoot = "E:\BDC\Projects\SASZombieAssaultTD"
)

$engineRoot = Join-Path $ProjectRoot "SASZombieAssaultTD\Engine"
$uiPath     = Join-Path $engineRoot "Systems\UI"
$logPath    = Join-Path $ProjectRoot "PowerShellLog.md"

New-Item -ItemType Directory -Force -Path $uiPath | Out-Null

# ---------------------------------------------------------------------
# UIElementBase.cs
# ---------------------------------------------------------------------
$uiElementBase = @"
namespace SASZombieAssaultTD.Engine.Systems.UI
{
    public abstract class UIElementBase
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public bool Visible { get; set; } = true;

        public virtual void Update() { }
        public virtual void Render(RenderQueue rq) { }

        public bool HitTest(int px, int py)
        {
            return px >= X && px <= X + Width &&
                   py >= Y && py <= Y + Height;
        }
    }
}
"@

Set-Content -Path (Join-Path $uiPath "UIElementBase.cs") -Value $uiElementBase -Encoding ASCII

# ---------------------------------------------------------------------
# Panel.cs
# ---------------------------------------------------------------------
$panelContent = @"
namespace SASZombieAssaultTD.Engine.Systems.UI
{
    public sealed class Panel : UIElementBase
    {
        public int BackgroundColor { get; set; } = unchecked((int)0xFF202020);

        public override void Render(RenderQueue rq)
        {
            if (!Visible) return;
            rq.EnqueueFillRect(X, Y, Width, Height, BackgroundColor);
        }
    }
}
"@

Set-Content -Path (Join-Path $uiPath "Panel.cs") -Value $panelContent -Encoding ASCII

# ---------------------------------------------------------------------
# Label.cs
# ---------------------------------------------------------------------
$labelContent = @"
using SASZombieAssaultTD.Engine.Rendering;

namespace SASZombieAssaultTD.Engine.Systems.UI
{
    public sealed class Label : UIElementBase
    {
        public string Text { get; set; } = string.Empty;
        public int Color { get; set; } = unchecked((int)0xFFFFFFFF);

        private readonly TextRenderer _text = new TextRenderer();

        public override void Render(RenderQueue rq)
        {
            if (!Visible) return;
            _text.DrawString(rq.Framebuffer, X, Y, Text, Color);
        }
    }
}
"@

Set-Content -Path (Join-Path $uiPath "Label.cs") -Value $labelContent -Encoding ASCII

# ---------------------------------------------------------------------
# Button.cs
# ---------------------------------------------------------------------
$buttonContent = @"
namespace SASZombieAssaultTD.Engine.Systems.UI
{
    public sealed class Button : UIElementBase
    {
        public string Text { get; set; } = string.Empty;
        public int BackgroundColor { get; set; } = unchecked((int)0xFF404040);
        public int HoverColor { get; set; } = unchecked((int)0xFF606060);
        public int TextColor { get; set; } = unchecked((int)0xFFFFFFFF);

        private readonly TextRenderer _text = new TextRenderer();
        private bool _hover;

        public void SetHover(bool hover) => _hover = hover;

        public override void Render(RenderQueue rq)
        {
            if (!Visible) return;

            int color = _hover ? HoverColor : BackgroundColor;
            rq.EnqueueFillRect(X, Y, Width, Height, color);

            int tx = X + 6;
            int ty = Y + 6;
            _text.DrawString(rq.Framebuffer, tx, ty, Text, TextColor);
        }
    }
}
"@

Set-Content -Path (Join-Path $uiPath "Button.cs") -Value $buttonContent -Encoding ASCII

# ---------------------------------------------------------------------
# LayoutSystem.cs
# ---------------------------------------------------------------------
$layoutContent = @"
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Systems.UI
{
    public sealed class LayoutSystem
    {
        public void VerticalStack(List<UIElementBase> elements, int startX, int startY, int spacing)
        {
            int y = startY;
            foreach (var e in elements)
            {
                e.X = startX;
                e.Y = y;
                y += e.Height + spacing;
            }
        }
    }
}
"@

Set-Content -Path (Join-Path $uiPath "LayoutSystem.cs") -Value $layoutContent -Encoding ASCII

# ---------------------------------------------------------------------
# Logging
# ---------------------------------------------------------------------
$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
$logEntry = @"
## Build-UIFramework.ps1 — $timestamp
- Wrote UIElementBase.cs, Panel.cs, Button.cs, Label.cs, LayoutSystem.cs.
- Installed foundational UI subsystem (panels, buttons, labels, layout).
- Mode: overwrite, ASCII, deterministic, PowerShell-driven.
"@

Add-Content -Path $logPath -Value $logEntry

Write-Host "UI Framework written and logged successfully."
