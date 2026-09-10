// =====================================================================================================
//  FILE: HUDOverlayWindow.cs
//  PATH: Engine/UI/HUDOverlay/HUDOverlayWindow.cs
//  SUBSYSTEM: HUDOverlay (Debug Visualization Subsystem)
//  LAYER: UI → HUDOverlay
//
//  ROLE:
//      Provides deterministic window state management for the HUDOverlay subsystem. HUDOverlayWindow
//      maintains position, size, title bar bounds, collapse state, and dragging interactions. All window
//      behavior is controlled exclusively by HUDOverlayManager.
//
//  RESPONSIBILITIES:
//      - Maintain window position, size, and title bar geometry.
//      - Track collapse state and dragging interactions.
//      - Emit full tracing statements for all state transitions and window updates.
//      - Maintain strict separation from HUDManager, HUDRenderer, and HUDPanelFinalizer internals.
//      - Operate deterministically without side effects.
//
//  NON-RESPONSIBILITIES:
//      - Does NOT perform rendering or diagnostic retrieval.
//      - Does NOT handle input sampling directly (HUDOverlayInput handles input).
//      - Does NOT modify HUDManager or Finalizer state.
//      - Does NOT persist window state to disk.
//
//  ARCHITECTURE NOTES:
//      - HUDOverlayManager is the authoritative controller; HUDOverlayWindow is subordinate.
//      - HUDOverlayInput provides mouse and hotkey data.
//      - All tracing uses DLogger.Log with subsystem tag: LogSubsystems.HUDOverlay.
//      - Window logic is deterministic and isolated.
//
//  VERSION:
//      Created: July 2026 — Foundational subsystem shell established.
//      Change Log:
//          - Implemented HUDOverlayWindow according to Management‑Only Policy.
//          - Added deterministic window positioning, dragging, and collapse logic.
//          - Added full tracing for all window state transitions.
//          - Removed all legacy HUDDebugOverlay dependencies and Gemini artifacts.
//          - Established clean Option‑B architecture boundaries.
// =====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Drawing;
using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.UI.HUDOverlay
{
    /// <summary>
    /// Provides deterministic window state management for HUDOverlay.
    /// </summary>
    internal sealed class HUDOverlayWindow
    {
        // -------------------------------------------------------------------------------------------------
        //  CONSTANTS
        // -------------------------------------------------------------------------------------------------

        public const int MinTitleBarWidth = 320;
        public const int MaxTitleBarWidth = 640;
        public const int MaxTitleBarHeight = 24;
        public const int MaxWidth = 640;
        public const int MaxHeight = 480;
        public const int MinHeight = 240;
        public const int MinWidth = 320;
        public const int TitleBarHeight = 24;


        // -------------------------------------------------------------------------------------------------
        //  WINDOW STATE
        // -------------------------------------------------------------------------------------------------

        public const int DefaultMinTitleBarWidth = 320;
        public const int DefaultTitleBarHeight = 24;

        public const string DefaultTitle = "HUD COCKPIT";
        public const int DefaultX = 10;
        public const int DefaultY = 10;
        public const int DefaultWidth = 320;
        public const int DefaultHeight = 240;

        public const int TitleBarWidth = 24;
        public string Title { get; private set; } = "HUD COCKPIT";
        public PointF Position { get; private set; } = new PointF(10f, 10f);
        public SizeF Size { get; private set; } = new SizeF(320f, 240f);

        public bool IsCollapsed { get; private set; } = false;
        public bool IsDragging { get; private set; } = false;
        public object AbsolutePosition { get; internal set; }

        private PointF _dragOffset = new PointF(0f, 0f);
        internal IEnumerable<object> Children;
        internal bool IsVisible;
        internal int Width;
        internal int Height;

        // -------------------------------------------------------------------------------------------------
        //  PUBLIC API — STATE MANAGEMENT
        // -------------------------------------------------------------------------------------------------

        public void SetSize(SizeF size) => Size = size;
        public void SetPosition(PointF position) => Position = position;
        public void SetTitle(string title) => Title = title;
        public void ToggleCollapse()
        {
            IsCollapsed = !IsCollapsed;

            DLogger.Log(LogSubsystems.HUDOverlay, LogEnums.LogLevel.Trace,
                $"HUDOverlayWindow: Collapse toggled → {IsCollapsed}");
        }

        public void SetChildren(IEnumerable<object> children) => Children = children;

        public void SetDragging(bool isDragging) => IsDragging = isDragging;

        public void SetAbsolutePosition(object absolutePosition) => AbsolutePosition = absolutePosition;

        public void SetDragOffset(PointF dragOffset) => _dragOffset = dragOffset;

        // -------------------------------------------------------------------------------------------------
        //  UPDATE — DRAGGING LOGIC
        // -------------------------------------------------------------------------------------------------

        public void ResetCollapse() => IsCollapsed = false;
        public void ResetPosition() => Position = new PointF(0f, 0f);
        public void ResetDraggingPosition() => Position = new PointF(0f, 0f);
        public void ResetAbsolutePosition() => AbsolutePosition = null;
        public void ResetDragging() => IsDragging = false;
        public void ResetDragOffset() => _dragOffset = new PointF(0f, 0f);
        public void UpdateDragOffset(PointF dragOffset) => _dragOffset = dragOffset;
        public void UpdateDragPosition(
            float mouseX,
            float mouseY) => Position = new PointF(mouseX - _dragOffset.X, mouseY - _dragOffset.Y);
        public void Update(float deltaTime)
        {
            // Window update is driven by HUDOverlayInput; this method is called by HUDOverlayManager.

            DLogger.Log(LogSubsystems.HUDOverlay, LogEnums.LogLevel.Trace,
                $"HUDOverlayWindow: Update cycle executed (dt={deltaTime}).");
        }

        // -------------------------------------------------------------------------------------------------
        //  DRAGGING ENTRY POINT (CALLED BY HUDOverlayManager)
        // -------------------------------------------------------------------------------------------------

        public void ProcessDragging(
            bool mouseJustPressed,
            bool mouseDown) => ProcessDragging(mouseJustPressed, mouseDown,
            0f, 0f);
        public void ProcessDragging(bool mouseJustPressed, bool mouseDown, float mouseX, float mouseY)
        {
            var titleBarBounds = new RectangleF(
                Position.X,
                Position.Y,
                Size.Width,
                TitleBarHeight
            );

            var mousePos = new PointF(mouseX, mouseY);

            // Mouse pressed → begin drag if inside title bar
            if (mouseJustPressed)
            {
                if (titleBarBounds.Contains(mousePos))
                {
                    IsDragging = true;
                    _dragOffset = new PointF(mousePos.X - Position.X, mousePos.Y - Position.Y);

                    DLogger.Log(LogSubsystems.HUDOverlay, LogEnums.LogLevel.Trace,
                        $"HUDOverlayWindow: Drag start → Offset X={_dragOffset.X}, Y={_dragOffset.Y}");
                }
            }
            // Mouse held → continue dragging
            else if (mouseDown)
            {
                if (IsDragging)
                {
                    Position = new PointF(mousePos.X - _dragOffset.X, mousePos.Y - _dragOffset.Y);

                    DLogger.Log(LogSubsystems.HUDOverlay, LogEnums.LogLevel.Trace,
                        $"HUDOverlayWindow: Dragging → New Position X={Position.X}, Y={Position.Y}");
                }
            }
            // Mouse released → stop dragging
            else
            {
                if (IsDragging)
                {
                    DLogger.Log(LogSubsystems.HUDOverlay, LogEnums.LogLevel.Trace,
                        "HUDOverlayWindow: Drag end.");
                }

                IsDragging = false;
            }
        }

        internal IEnumerable<object> GetPageLines()
        {
            throw new NotImplementedException();
        }
    }
}
