// ====================================================================================================
//  FILE: HUDDebugOverlay.cs
//  PATH: Engine/UI/
//  MODULE: HUD Debug Overlay
//
//  ROLE:
//      Provides a modular debugging cockpit for the HUD system. This overlay is drawn by HUDManager
//      and exposes multiple diagnostic pages, a draggable window, a global collapse toggle, and a
//      page selector. It is isolated from HUDManager to maintain clean separation of responsibilities.
//
//  RESPONSIBILITIES:
//      - Draws a debug window in the top-left region.
//      - Supports window dragging.
//      - Supports global collapse/expand (F4).
//      - Supports page selection (F5).
//      - Displays Finalizer, Crosshair, Render Order, HUD Bounds, Player/Input/FPS,
//        and Layout/Snapshot diagnostics.
//
//  NON-RESPONSIBILITIES:
//      - Does not modify HUD elements.
//      - Does not modify Finalizer pipeline behavior.
//      - Does not persist debug state.
//      - Does not interfere with bottom HUD panels.
//
//  NOTES:
//      HUDManager only forwards Update() and Draw() calls. All debugging logic resides here to
//      prevent HUDManager.cs from becoming overly large or tightly coupled.
// ====================================================================================================

using System.Collections.Generic;
using System.Windows.Forms;
using SASZombieAssaultTD.Engine.Input;
using SASZombieAssaultTD.Engine.Rendering;

namespace SASZombieAssaultTD.Engine.UI
{
    internal class HUDDebugOverlay
    {
        private readonly HUDManager _hud;

        // Window state
        private bool _enabled = true;
        private bool _collapsed = false;
        private int _windowX = 10;
        private int _windowY = 10;
        private int _windowWidth = 420;
        private int _windowHeight = 260;

        // Dragging
        private bool _dragging = false;
        private int _dragOffsetX;
        private int _dragOffsetY;

        // Pages
        private int _currentPage = 0;

        private enum DebugPage
        {
            Finalizer,
            Crosshair,
            RenderOrder,
            HudBounds,
            PlayerInputFps,
            LayoutSnapshot
        }

        private readonly DebugPage[] _pages =
        {
            DebugPage.Finalizer,
            DebugPage.Crosshair,
            DebugPage.RenderOrder,
            DebugPage.HudBounds,
            DebugPage.PlayerInputFps,
            DebugPage.LayoutSnapshot
        };

        // FPS
        private float _fpsTimer = 0f;
        private int _fpsCounter = 0;
        private int _fpsDisplay = 0;

        public HUDDebugOverlay(HUDManager hud)
        {
            _hud = hud;
        }

        public void Update(float deltaTime)
        {
            // Toggle entire overlay
            if (InputSystem.IsKeyJustPressed(Keys.F3))
                _enabled = !_enabled;

            if (!_enabled)
                return;

            // Collapse/expand
            if (InputSystem.IsKeyJustPressed(Keys.F4))
                _collapsed = !_collapsed;

            // Page selector
            if (InputSystem.IsKeyJustPressed(Keys.F5))
            {
                _currentPage++;
                if (_currentPage >= _pages.Length)
                    _currentPage = 0;
            }

            // Dragging
            var mouse = InputSystem.GetMousePosition();

            if (InputSystem.IsMouseButtonJustPressed(0))
            {
                if (IsMouseOverHeader((int)mouse.X, (int)mouse.Y))
                {
                    _dragging = true;
                    _dragOffsetX = (int)(mouse.X - _windowX);
                    _dragOffsetY = (int)(mouse.Y - _windowY);
                }
            }
            else if (!InputSystem.IsMouseButtonPressed(0))
            {
                _dragging = false;
            }

            if (_dragging)
            {
                _windowX = (int)(mouse.X - _dragOffsetX);
                _windowY = (int)(mouse.Y - _dragOffsetY);
            }

            // FPS
            _fpsTimer += deltaTime;
            _fpsCounter++;

            if (_fpsTimer >= 1f)
            {
                _fpsDisplay = _fpsCounter;
                _fpsCounter = 0;
                _fpsTimer = 0f;
            }
        }

        private bool IsMouseOverHeader(int mx, int my)
        {
            return mx >= _windowX &&
                   mx <= _windowX + _windowWidth &&
                   my >= _windowY &&
                   my <= _windowY + 24;
        }

        public void Draw(IDrawingContext context, dynamic resolved, IList<IHUDElement> renderOrder)
        {
            if (!_enabled)
                return;

            if (_collapsed)
            {
                DrawCollapsedBar(context);
                return;
            }

            DrawWindowFrame(context);

            int cx = _windowX + 10;
            int cy = _windowY + 40;

            switch (_pages[_currentPage])
            {
                case DebugPage.Finalizer:
                    DrawFinalizerPage(context, resolved, cx, cy);
                    break;

                case DebugPage.Crosshair:
                    DrawCrosshairPage(context, resolved, cx, cy);
                    break;

                case DebugPage.RenderOrder:
                    DrawRenderOrderPage(context, renderOrder, cx, cy);
                    break;

                case DebugPage.HudBounds:
                    DrawHudBoundsPage(context, renderOrder, cx, cy);
                    break;

                case DebugPage.PlayerInputFps:
                    DrawPlayerInputFpsPage(context, cx, cy);
                    break;

                case DebugPage.LayoutSnapshot:
                    DrawLayoutSnapshotPage(context, cx, cy);
                    break;
            }
        }

        private void DrawWindowFrame(IDrawingContext context)
        {
            var bg = new Color(0f, 0f, 0f, 0.70f);
            var header = new Color(0.2f, 0.2f, 0.2f, 0.9f);
            var title = new Color(1f, 1f, 0f, 1f);

            context.FillRectangle(new Rectangle(_windowX, _windowY, _windowWidth, _windowHeight), bg);
            context.FillRectangle(new Rectangle(_windowX, _windowY, _windowWidth, 24), header);

            context.DrawText("[HUD DEBUG OVERLAY]", _windowX + 8, _windowY + 4, title);
            context.DrawText($"Page: {_pages[_currentPage]} (F5 to change, F4 collapse)", _windowX + 8, _windowY + 22, Color.White);
        }

        private void DrawCollapsedBar(IDrawingContext context)
        {
            var header = new Color(0.2f, 0.2f, 0.2f, 0.9f);
            var title = new Color(1f, 1f, 0f, 1f);

            context.FillRectangle(new Rectangle(_windowX, _windowY, 260, 24), header);
            context.DrawText("[HUD DEBUG OVERLAY] (F4 expand)", _windowX + 8, _windowY + 4, title);
        }

        private void DrawFinalizerPage(IDrawingContext context, dynamic r, int x, int y)
        {
            context.DrawText("FINALIZER STATE", x, y, Color.White);
            y += 20;

            context.DrawText($"Manual Override: {r.ManualOverrideEnabled}", x, y, Color.White); y += 18;
            context.DrawText($"FillColor: {r.FillColor}", x, y, Color.White); y += 18;
            context.DrawText($"TextColor: {r.TextColor}", x, y, Color.White); y += 18;

            context.FillRectangle(new Rectangle(x + 220, y - 54, 40, 18), r.FillColor);
            context.FillRectangle(new Rectangle(x + 220, y - 36, 40, 18), r.TextColor);
        }

        private void DrawCrosshairPage(IDrawingContext context, dynamic r, int x, int y)
        {
            context.DrawText("CROSSHAIR STATE", x, y, Color.White);
            y += 20;

            context.DrawText($"Center X: {r.CrosshairCenterX}", x, y, Color.White); y += 18;
            context.DrawText($"Center Y: {r.CrosshairCenterY}", x, y, Color.White); y += 18;
            context.DrawText($"Arm Length: {r.CrosshairArmLength}", x, y, Color.White); y += 18;
            context.DrawText($"Thickness: {r.CrosshairThickness}", x, y, Color.White);
        }

        private void DrawRenderOrderPage(IDrawingContext context, IList<IHUDElement> order, int x, int y)
        {
            context.DrawText("RENDER ORDER", x, y, Color.White);
            y += 20;

            int i = 0;
            foreach (var e in order)
            {
                context.DrawText($"{i++}: {e.Id}", x, y, Color.White);
                y += 16;
            }
        }

        private void DrawHudBoundsPage(IDrawingContext context, IList<IHUDElement> order, int x, int y)
        {
            context.DrawText("HUD BOUNDS", x, y, Color.White);
            y += 20;

            foreach (var e in order)
            {
                context.DrawText($"{e.Id}", x, y, Color.White);
                y += 16;
            }
        }

        private void DrawPlayerInputFpsPage(IDrawingContext context, int x, int y)
        {
            context.DrawText("PLAYER / INPUT / FPS", x, y, Color.White);
            y += 20;

            var mouse = InputSystem.GetMousePosition();
            context.DrawText($"Mouse: {mouse.X},{mouse.Y}", x, y, Color.White); y += 16;

            context.DrawText($"FPS: {_fpsDisplay}", x, y, new Color(0f, 1f, 0f, 1f));
        }

        private void DrawLayoutSnapshotPage(IDrawingContext context, int x, int y)
        {
            context.DrawText("LAYOUT / SNAPSHOT", x, y, Color.White);
            y += 20;

            context.DrawText("Layout and snapshot diagnostics will go here.", x, y, Color.White);
        }
    }
}
