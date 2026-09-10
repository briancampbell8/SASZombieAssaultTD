// =====================================================================================================
//  FILE: Renderer-LegacyDraw.cs
//  PATH: Engine/Render/LegacyRender/Renderer-LegacyDraw.cs
//  SUBSYSTEM: Rendering (Legacy Driver Program)
//
//  ROLE:
//      Provides legacy-compatible immediate-mode drawing helpers for rectangles, sprites, text,
//      and simple shapes. These APIs exist to support older gameplay and UI code paths while
//      the modern deterministic render pipeline is brought online.
// =====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Drawing;
using System.Numerics;
using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Render.Sprites;

namespace SASZombieAssaultTD.Engine.Render
{
    public partial class Renderer
    {
        /// <summary>
        /// Legacy instance rectangle draw helper. Fully qualified to prevent System.Drawing conflicts.
        /// </summary>
        public void DrawRectangle(
            float x1,
            int x,
            Rectangle rect,
            Color color)
        {
            DLogger.Log(
                LogSubsystems.Rendering,
                LogEnums.LogLevel.Trace,
                $"Renderer: DrawRectangle (legacy) — " +
                $"X1={x1}, X={x}, Rect=({rect.X},{rect.Y},{rect.Width},{rect.Height}), Color={color}"
            );
        }

        /// <summary>
        /// Legacy instance sprite draw helper. Currently logs the request without GPU work.
        /// </summary>
        public void DrawSprite(string sprite1, Sprite sprite, Vector3 position, Color color)
        {
            DLogger.Log(
                LogSubsystems.Rendering,
                LogEnums.LogLevel.Trace,
                $"Renderer: DrawSprite (legacy) — " +
                $"Id={sprite1}, Pos=({position.X},{position.Y},{position.Z}), Color={color}"
            );
        }

        /// <summary>
        /// Legacy instance string draw helper. Currently logs the request without GPU work.
        /// </summary>
        public void DrawString(string text, VectorMath.Vector3 vector3, Vector3 position, Color color)
        {
            DLogger.Log(
                LogSubsystems.Rendering,
                LogEnums.LogLevel.Trace,
                $"Renderer: DrawString (legacy) — " +
                $"Text=\"{text}\", Pos=({position.X},{position.Y},{position.Z}), Color={color}"
            );
        }

        internal static void DrawRectangle(
            VectorMath.Vector3 previewPosition1,
            VectorMath.Vector3 previewPosition2,
            VectorMath.Vector3 previewSize,
            Color towerColor,
            float thickness)
        {
            DLogger.Log(
                LogSubsystems.Rendering,
                LogEnums.LogLevel.Trace,
                $"Renderer: DrawRectangle (legacy static Vector3) — " +
                $"P1={previewPosition1}, P2={previewPosition2}, " +
                $"Size={previewSize}, Color={towerColor}, Thickness={thickness}"
            );
        }

        internal static void DrawSprite(
            string sprite,
            VectorMath.Vector3 previewPosition,
            VectorMath.Vector3 previewSize,
            float rotation,
            Color towerColor,
            float opacity)
        {
            DLogger.Log(
                LogSubsystems.Rendering,
                LogEnums.LogLevel.Trace,
                $"Renderer: DrawSprite (legacy static) — " +
                $"Id={sprite}, Pos={previewPosition}, " +
                $"Size={previewSize}, Rot={rotation}, Color={towerColor}, Opacity={opacity}"
            );
        }

        internal static void DrawString(
            string statusText,
            VectorMath.Vector3 statusPosition,
            Color statusTextColor,
            CachedFont statusFont)
        {
            DLogger.Log(
                LogSubsystems.Rendering,
                LogEnums.LogLevel.Trace,
                $"Renderer: DrawString (legacy static) — " +
                $"Text=\"{statusText}\", Pos={statusPosition}, " +
                $"Color={statusTextColor}, Font={statusFont}"
            );
        }

        internal static void DrawRectangle(
            int x1,
            int y1,
            int x2,
            int y2,
            Color borderColor,
            float thickness)
        {
            DLogger.Log(
                LogSubsystems.Rendering,
                LogEnums.LogLevel.Trace,
                $"Renderer: DrawRectangle (legacy static border int) — " +
                $"({x1},{y1})-({x2},{y2}), Border={borderColor}, Thickness={thickness}"
            );
        }

        internal static void DrawRectangle(
            int x,
            int y,
            int width,
            int height,
            Color backgroundColor)
        {
            DLogger.Log(
                LogSubsystems.Rendering,
                LogEnums.LogLevel.Trace,
                $"Renderer: DrawRectangle (legacy static bounds int) — Pos=({x},{y})," +
                $" Size=({width},{height}), Fill={backgroundColor}"
            );
        }

        internal static void DrawSprite(
            string sprite,
            VectorMath.Vector3 previewPosition,
            VectorMath.Vector3 previewSize,
            float r,
            float g,
            float b,
            Func<Color> colorProvider,
            float opacity)
        {
            Color color = colorProvider != null
                ? colorProvider()
                : Color.FromArgb(
                    255,
                    (int)System.Math.Max(0, System.Math.Min(255, r * 255f)),
                    (int)System.Math.Max(0, System.Math.Min(255, g * 255f)),
                    (int)System.Math.Max(0, System.Math.Min(255, b * 255f))
                );

            DLogger.Log(
                LogSubsystems.Rendering,
                LogEnums.LogLevel.Trace,
                $"Renderer: DrawSprite (legacy static delegate) — " +
                $"Id={sprite}, Pos={previewPosition}, Size={previewSize}, " +
                $"Color={color}, Opacity={opacity}"
            );
        }

        internal static void DrawSprite(
            string sprite,
            VectorMath.Vector3 previewPosition,
            VectorMath.Vector3 previewSize,
            Color towerColor,
            float opacity)
        {
            DLogger.Log(
                LogSubsystems.Rendering,
                LogEnums.LogLevel.Trace,
                $"Renderer: DrawSprite (legacy static basic) — " +
                $"Id={sprite}, Pos={previewPosition}, " +
                $"Size={previewSize}, Color={towerColor}, Opacity={opacity}"
            );
        }

        internal static void DrawRectangle(
            float x1,
            float y1,
            float x2,
            float y2,
            Color bg)
        {
            DLogger.Log(
                LogSubsystems.Rendering,
                LogEnums.LogLevel.Trace,
                $"Renderer: DrawRectangle (legacy static bounds float) — " +
                $"({x1},{y1})-({x2},{y2}), Fill={bg}"
            );
        }

        internal static void DrawRectangle(
            float x1,
            float y1,
            float x2,
            float y2,
            Color border,
            float thickness)
        {
            DLogger.Log(
                LogSubsystems.Rendering,
                LogEnums.LogLevel.Trace,
                $"Renderer: DrawRectangle (legacy static border float) — " +
                $"({x1},{y1})-({x2},{y2}), Border={border}, Thickness={thickness}"
            );
        }

        internal static void DrawString(
            string title,
            VectorMath.Vector3 position,
            Color sectionColor,
            Font titleFont)
        {
            DLogger.Log(
                LogSubsystems.Rendering,
                LogEnums.LogLevel.Trace,
                $"Renderer: DrawString (legacy static font) —" +
                $" Title=\"{title}\", Pos={position}, Color={sectionColor}, Font={titleFont?.Name}"
            );
        }

        internal static void DrawCircle(
            float x,
            float y,
            float radius,
            Color color,
            float thickness)
        {
            DLogger.Log(
                LogSubsystems.Rendering,
                LogEnums.LogLevel.Trace,
                $"Renderer: DrawCircle (legacy static) — " +
                $"Center=({x},{y}), Radius={radius}, Color={color}, Thickness={thickness}"
            );
        }

        internal static void FillCircle(
            float x,
            float y,
            float radius,
            Color color)
        {
            DLogger.Log(
                LogSubsystems.Rendering,
                LogEnums.LogLevel.Trace,
                $"Renderer: FillCircle (legacy static) — Center=({x},{y}), Radius={radius}, Color={color}"
            );
        }

        internal static void DrawString(
            string text,
            VectorMath.Vector3 position,
            Color color,
            float size)
        {
            DLogger.Log(
                LogSubsystems.Rendering,
                LogEnums.LogLevel.Trace,
                $"Renderer: DrawString (legacy static sized) — " +
                $"Text=\"{text}\", Pos={position}, Color={color}, Size={size}"
            );
        }

        internal static void DrawRectangle(
            float x,
            float y,
            float width,
            float height,
            float intensity)
        {
            DLogger.Log(
                LogSubsystems.Rendering,
                LogEnums.LogLevel.Trace,
                $"Renderer: DrawRectangle (legacy static intensity) — " +
                $"Pos=({x},{y}), Size=({width},{height}), Intensity={intensity}"
            );
        }

        internal static void DrawString(
            string text,
            VectorMath.Vector3 pos,
            Color textColor,
            object font)
        {
            DLogger.Log(
                LogSubsystems.Rendering,
                LogEnums.LogLevel.Trace,
                $"Renderer: DrawString (legacy static font object) — " +
                $"Text=\"{text}\", Pos={pos}, Color={textColor}, Font={font}"
            );
        }
    }
}
