// =====================================================================================================
//  FILE: HUDDebugOverlay.cs
//  PATH: Engine/UI/HUDDebugOverlay.cs
//  MODULE: HUD Debug Overlay Configuration
//  LAYER: UI → HUD Core
//
//  ROLE:
//      Provides a modular debugging cockpit for the HUD system. This overlay is drawn by
//      HUDOverlayRenderer and exposes multiple diagnostic pages, a draggable window, a global
//      collapse toggle, and a page selector. It is fully isolated from HUDManager and the Finalizer
//      pipeline.
//
//  RESPONSIBILITIES:
//      - Track absolute bounds, placement offsets, and drag states for the debug viewport window.
//      - Monitor function keys (F4/F5) to collapse frames or cycle selector indices.
//      - Display diagnostic strings for Finalizer, Crosshair, Render Layers, and Engine Stats.
//
//  NON-RESPONSIBILITIES:
//      - Does NOT modify HUDManager or Finalizer pipeline state.
//      - Does NOT access legacy HUDManager fields.
//      - Does NOT persist troubleshooting snapshots.
//
//  CHANGE LOG (2026 Modernization):
//      - Removed ALL HUDManager references.
//      - Replaced geometry reads with Finalizer UIState.
//      - Replaced dragging mouse position logic with Vector2.
//      - Added deterministic tracing.
//      - Fully aligned with Option‑A deterministic input architecture.
// =====================================================================================================

using System.Drawing;
using System.Numerics;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Input;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using SASZombieAssaultTD.Engine.UI.Elements;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using static SASZombieAssaultTD.Engine.UI.UIEnums;

namespace SASZombieAssaultTD.Engine.UI
{
    public sealed class HUDDebugOverlay : UIElement
    {
        private DebugPage _currentPage = DebugPage.Finalizer;
        private bool _isCollapsed = false;

        private bool _isDragging = false;
        private Components.PointF _dragOffset = new Components.PointF(0f, 0f);
        private RectangleF _titleBarBounds;

        private readonly UIInputRouter _router;
        private readonly HUDPanelFinalizer_Manager _finalizer;

        public DebugPage CurrentPage
        {
            get => _currentPage;
            set => _currentPage = value;
        }

        public bool IsCollapsed
        {
            get => _isCollapsed;
            set => _isCollapsed = value;
        }
        public int Layer { get; private set; }

        public HUDDebugOverlay(UIInputRouter router, HUDPanelFinalizer_Manager finalizer)
        {
            Id = "hud_debug_overlay";
            Layer = 9999;

            Position = new Components.PointF(10f, 10f);
            Size = new SizeF(320f, 240f);

            _titleBarBounds = new RectangleF(0f, 0f, Size.Width, 24f);

            _router = router;
            _finalizer = finalizer;

            DLogger.Log(LogSubsystems.HUDOverlay, LogLevel.Info,
                "HUDDebugOverlay: Initialized.");
        }

        public override void Update(float deltaTime)
        {
            if (_router.IsKeyPressed(InputKey.F4))
            {
                _isCollapsed = !_isCollapsed;
                DLogger.Log(LogSubsystems.HUDOverlay, LogLevel.Trace,
                    $"HUDDebugOverlay: Collapse toggled → now {_isCollapsed}");
            }

            if (_router.IsKeyPressed(InputKey.F5))
            {
                _currentPage = (DebugPage)(((int)_currentPage + 1) % 5);
                DLogger.Log(LogSubsystems.HUDOverlay, LogLevel.Trace,
                    $"HUDDebugOverlay: Page switched → {_currentPage}");
            }

            HandleWindowDragging();
            base.Update(deltaTime);
        }

        // -------------------------------------------------------------------------------------------------
        //  WINDOW DRAGGING
        // -------------------------------------------------------------------------------------------------

        private void HandleWindowDragging()
        {
            var pos = _router.GetMousePosition();
            Vector2 mouse = new Vector2(pos.Item1, pos.Item2);

            bool justPressed = _router.IsMouseButtonPressed(0);
            bool isDown = _router.IsMouseButtonDown(0);

            var absPos = AbsolutePosition;

            var titleBar = new RectangleF(
                absPos.X + _titleBarBounds.X,
                absPos.Y + _titleBarBounds.Y,
                _titleBarBounds.Width,
                _titleBarBounds.Height);

            var mousePoint = new Components.PointF(mouse.X, mouse.Y);

            if (justPressed)
            {
                if (titleBar.Contains(mousePoint.X, mousePoint.Y))
                {
                    _isDragging = true;
                    _dragOffset = new Components.PointF(
                        mousePoint.X - absPos.X,
                        mousePoint.Y - absPos.Y);

                    DLogger.Log(LogSubsystems.HUDOverlay, LogLevel.Trace,
                        $"HUDDebugOverlay: Drag start at ({mousePoint.X}, {mousePoint.Y}) " +
                        $"offset ({_dragOffset.X}, {_dragOffset.Y})");
                }
            }

            if (_isDragging && isDown)
            {
                Position = new Components.PointF(
                    mousePoint.X - _dragOffset.X,
                    mousePoint.Y - _dragOffset.Y);
            }
            else if (!isDown && _isDragging)
            {
                _isDragging = false;

                DLogger.Log(LogSubsystems.HUDOverlay, LogLevel.Trace,
                    "HUDDebugOverlay: Drag end.");
            }
        }

        // -------------------------------------------------------------------------------------------------
        //  DRAW
        // -------------------------------------------------------------------------------------------------

        public void Draw(D3D11Adapter_Core adapter_Core)
        {
            if (adapter_Core == null || !IsVisible)
                return;

            var absPos = AbsolutePosition;
            int x = (int)absPos.X;
            int y = (int)absPos.Y;
            int w = (int)Size.Width;
            int h = _isCollapsed ? 24 : (int)Size.Height;

            adapter_Core.FillRectangle(new Rectangle(x, y, w, h),
                new Color(16, 16, 16, 220));

            var titleColor = _isDragging
                ? new Color(48, 63, 159, 255)
                : new Color(33, 150, 243, 255);

            adapter_Core.FillRectangle(new Rectangle(x, y, w, 24), titleColor);

            string titleText = _isCollapsed
                ? "HUD COCKPIT [COLLAPSED]"
                : $"HUD COCKPIT [{_currentPage}] (F4: Collapse, F5: Cycle)";

            adapter_Core.DrawText(titleText, x + 6, y + 4, 1.0f, Color.White);

            if (_isCollapsed)
            {
                base.Draw(adapter_Core);
                return;
            }

            int textY = y + 32;
            int spacing = 16;
            var cyan = new Color(0, 224, 255, 255);

            var state = _finalizer.GetResolvedState();

            switch (_currentPage)
            {
                case DebugPage.Finalizer:
                    adapter_Core.DrawText("--- FINALIZER PIPELINE METRICS ---",
                        x + 10, textY, 1.0f, cyan);

                    adapter_Core.DrawText($"Override Mode: {state.ManualOverrideEnabled}",
                        x + 10, textY + spacing, 1.0f, Color.White);

                    adapter_Core.DrawText($"FillColor: {state.FillColor}",
                        x + 10, textY + spacing * 2, 1.0f, Color.White);

                    adapter_Core.DrawText($"TextColor: {state.TextColor}",
                        x + 10, textY + spacing * 3, 1.0f, Color.White);
                    break;

                case DebugPage.Crosshair:
                    adapter_Core.DrawText("--- CROSSHAIR ANCHOR GEOMETRY ---",
                        x + 10, textY, 1.0f, cyan);

                    adapter_Core.DrawText($"Center: X={state.CrosshairCenterX}, Y={state.CrosshairCenterY}",
                        x + 10, textY + spacing, 1.0f, Color.White);

                    adapter_Core.DrawText($"Arm Length: {state.CrosshairArmLength}px",
                        x + 10, textY + spacing * 2, 1.0f, Color.White);

                    adapter_Core.DrawText($"Thickness: {state.CrosshairThickness}px",
                        x + 10, textY + spacing * 3, 1.0f, Color.White);
                    break;

                case DebugPage.RenderOrder:
                    adapter_Core.DrawText("--- ACTIVE HIERARCHICAL LAYERS ---",
                        x + 10, textY, 1.0f, cyan);

                    adapter_Core.DrawText("0: [CASH_PANEL] Layer: 999 (Authoritative)",
                        x + 10, textY + spacing, 1.0f, Color.White);
                    break;

                case DebugPage.HUDBounds:
                    adapter_Core.DrawText("--- SPATIAL VIEWPORT BOUNDARIES ---",
                        x + 10, textY, 1.0f, cyan);

                    adapter_Core.DrawText($"Panel Position: X={state.X}, Y={state.Y}",
                        x + 10, textY + spacing, 1.0f, Color.White);

                    adapter_Core.DrawText($"Panel Size: W={state.Width}, H={state.Height}",
                        x + 10, textY + spacing * 2, 1.0f, Color.White);
                    break;

                case DebugPage.EngineStats:
                    adapter_Core.DrawText("--- INPUT ROUTER TELEMETRY ---",
                        x + 10, textY, 1.0f, cyan);

                    InputRouterStats stats = (InputRouterStats)_router.GetStats();

                    adapter_Core.DrawText($"Keys Tracked: {stats.KeysTracked}",
                        x + 10, textY + spacing, 1.0f, Color.White);

                    adapter_Core.DrawText($"Mouse Events: {stats.MouseEventsProcessed}",
                        x + 10, textY + spacing * 2, 1.0f, Color.White);

                    adapter_Core.DrawText($"Router Enabled: {stats.IsEnabled}",
                        x + 10, textY + spacing * 3, 1.0f, Color.White);
                    break;
            }

            base.Draw(adapter_Core);
        }
    }
}
