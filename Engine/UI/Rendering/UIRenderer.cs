// ====================================================================================================
//  FILE: UIRenderer.cs
//  PATH: ./Engine/UI/Rendering/
//  SUBSYSTEM: Rendering
//
//  ROLE:
//      Deterministic UI rendering and transition execution pipeline.
//      Provides draw calls, frame lifecycle, and transition queue processing.
//
//  RESPONSIBILITIES:
//      - Provide BeginFrame() and EndFrame() behavior.
//      - Provide RenderRectangle(), RenderText(), RenderLine(), RenderSprite().
//      - Maintain a deterministic queue of UI transitions.
//      - Execute transitions each frame using stable timing rules.
//      - Accept transition registrations from UI subsystems (MainMenu, HUD, etc.).
//
//  NON-RESPONSIBILITIES:
//      - UI layout, hierarchy, or input routing.
//      - Asset loading or file I/O.
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Drawing;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Render;
using SASZombieAssaultTD.Engine.VectorMath;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.UI.Rendering
{
    public class UIRenderer
    {
        private bool _isInitialized;
        internal static object Instance;
        private static Color _backingColor;

        // NEW: Deterministic transition queue
        private static readonly List<RegisterTransition> _transitionQueue = new();

        public bool IsInitialized => _isInitialized;

        public UIRenderer() => DLogger.Log(LogSubsystems.ResourcesPipeline, "UIRenderer: Constructed (modern wrapper)");

        public void Initialize()
        {
            if (_isInitialized)
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline, "UIRenderer: Already initialized");
                return;
            }

            _isInitialized = true;
            DLogger.Log(LogSubsystems.ResourcesPipeline, "UIRenderer: Initialized successfully");
        }

        public void Shutdown()
        {
            if (!_isInitialized)
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline, "UIRenderer: Already shut down");
                return;
            }

            _isInitialized = false;
            DLogger.Log(LogSubsystems.ResourcesPipeline, "UIRenderer: Shut down successfully");
        }

        // ====================================================================================================
        //  NEW: RegisterTransition METHOD (fixes CS0118 + CS1955)
        // ====================================================================================================
        public static void RegisterTransition(RegisterTransition registration)
        {
            _transitionQueue.Add(registration);
            DLogger.Log($"UIRenderer: Registered transition → {registration.Type} Mode={registration.Mode}");
        }

        // ====================================================================================================
        //  NEW: Transition execution (simple deterministic loop)
        // ====================================================================================================
        public void ProcessTransitions(float deltaTime)
        {
            if (!_isInitialized) return;

            for (int i = _transitionQueue.Count - 1; i >= 0; i--)
            {
                var reg = _transitionQueue[i];
                var t = reg.Transition;

                t.Elapsed += deltaTime;

                float progress = System.Math.Clamp(t.Elapsed / t.Duration, 0f, 1f);

                t.Apply?.Invoke(progress);

                if (progress >= 1f)
                {
                    _transitionQueue.RemoveAt(i);
                    DLogger.Log($"UIRenderer: Transition completed → {reg.Type}");
                }
            }
        }

        // ====================================================================================================
        //  FRAME LIFECYCLE
        // ====================================================================================================
        public void BeginFrame()
        {
            if (!_isInitialized)
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline, "UIRenderer: BeginFrame called before Initialize");
                return;
            }
        }

        public void EndFrame()
        {
            if (!_isInitialized)
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline, "UIRenderer: EndFrame called before Initialize");
                return;
            }
        }

        // ====================================================================================================
        //  DRAW CALLS
        // ====================================================================================================
        public void RenderRectangle(float x, float y, float width, float height, Color color,
                                    float borderThickness = 0f, Color? borderColor = null)
        {
            if (!_isInitialized) return;

            Renderer.DrawRectangle(x, y, width, height, color);

            if (borderThickness > 0f && borderColor.HasValue)
                Renderer.DrawRectangle(x, y, width, height, borderColor.Value, borderThickness);
        }

        public void RenderRectangle(Vector3 position, Vector3 size, Color color,
                                    float borderThickness = 0f, Color? borderColor = null)
        {
            RenderRectangle(position.X, position.Y, size.X, size.Y, color, borderThickness, borderColor);
        }

        public void RenderText(string text, Vector3 position, Color color, Font font)
        {
            if (!_isInitialized) return;
            if (string.IsNullOrEmpty(text)) return;

            Renderer.DrawString(text, position, color, font);
        }

        public void RenderLine(Vector3 start, Vector3 end, Color color, float thickness = 1f)
        {
            if (!_isInitialized) return;

            float dx = end.X - start.X;
            float dy = end.Y - start.Y;
            float length = MathF.Sqrt(dx * dx + dy * dy);
            if (length <= 0f) return;

            Renderer.DrawRectangle(start.X, start.Y, length, thickness, color);
        }

        public void RenderSprite(object sprite, Vector3 position, Vector3 size, Color color, float alpha = 1f)
        {
            if (!_isInitialized) return;
            if (sprite == null) return;

            Renderer.DrawSprite((string)sprite, position, size, color, alpha);
        }

        // ====================================================================================================
        //  CLIPPING + ALPHA (no-op)
        // ====================================================================================================
        public void SetClipRect(RectangleF rect)
        {
            DLogger.Log($"UIRenderer: SetClipRect requested ({rect}) - no-op");
        }

        public static void ClearClipRect()
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "UIRenderer: ClearClipRect requested - no-op");
        }

        public static void SetGlobalAlpha(float alpha)
        {
            DLogger.Log($"UIRenderer: SetGlobalAlpha({alpha}) requested - no-op");
        }

        // ====================================================================================================
        //  LEGACY HOOK
        // ====================================================================================================
        public void RenderElement(object element)
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "UIRenderer: RenderElement(object) called - legacy no-op");
        }

        internal static void Clear(Color black)
        {
            // Set the renderer's backing/background color and reset common state used for clearing.
            // _backingColor is the stored background color used by the renderer when drawing frames.
            _backingColor = black;

            // Ensure no clipping remains from previous operations and restore full opacity for the frame.
            ClearClipRect();
            SetGlobalAlpha(1.0f);
        }
    }
}
