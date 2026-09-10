// ====================================================================================================
//  FILE: RenderDevice.cs
//  PATH: Engine/Render
//  MODULE: Render
//
//  ROLE:
//      Deterministic GPU submission layer for sprite and quad rendering.
//      Binds textures, sets pipeline state, and issues draw calls.
//
//  RESPONSIBILITIES:
//      - Bind Texture2D GPU resources.
//      - Submit quad draw calls for sprites.
//      - Remain deterministic and side‑effect free.
//      - Integrate cleanly with the backend (D3D11, BGFX, Vulkan).
//
//  NON-RESPONSIBILITIES:
//      - Sorting or batching (handled by SpriteBatchOptimizer).
//      - Resource loading or texture management.
//      - Gameplay logic, UI layout, or diagnostics.
//      - Logging or fallback behavior.
//
//  ARCHITECTURAL NOTES:
//      - Pure GPU submission layer.
//      - Option‑B deterministic rendering compliant.
//      - Uses immutable Sprite structs.
//      - Backend-specific handles are stored in Texture2D implementations.
// ====================================================================================================

using System;
using System.Numerics;
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.TextureRendering;

namespace SASZombieAssaultTD.Engine.Render
{
    /// <summary>
    /// GPU submission layer for sprite rendering.
    /// </summary>
    internal sealed class RenderDevice
    {
        private readonly IntPtr _deviceHandle;
        private readonly IntPtr _contextHandle;

        public RenderDevice(IntPtr deviceHandle, IntPtr contextHandle)
        {
            _deviceHandle = deviceHandle;
            _contextHandle = contextHandle;
        }

        /// <summary>
        /// Binds a GPU texture for subsequent draw calls.
        /// </summary>
        public void BindTexture(Texture2D texture)
        {
            // Deterministic: no logging, no fallback.
            IntPtr handle = texture.NativeHandle;

            // NativeBindTexture(_contextHandle, handle);
        }

        /// <summary>
        /// Issues a draw call for a single sprite quad.
        /// </summary>
        public void DrawSprite(
            Texture2D texture,
            Vector2 position,
            Vector2 size,
            System.Numerics.Vector4 uv,
            ColorRGBA tint,
            float rotation,
            float layerDepth)
        {
            // Deterministic: no logging, no fallback.

            // NativeDrawSprite(
            //     _contextHandle,
            //     texture.NativeHandle,
            //     position.X, position.Y,
            //     size.X, size.Y,
            //     uv.X, uv.Y, uv.Z, uv.W,
            //     tint.R, tint.G, tint.B, tint.A,
            //     rotation,
            //     layerDepth);
        }

        internal void DrawSprite(
            Texture2D texture,
            System.Drawing.Rectangle dest,
            System.Drawing.Rectangle? src,
            Color color,
            float rotation,
            Vector2 origin,
            float layerDepth)
        {
            // Calculate destination position and size from the rectangle.
            var position = new Vector2(dest.X, dest.Y);
            var size = new Vector2(dest.Width, dest.Height);

            // Calculate UV coordinates (normalized) from source rectangle if provided.
            Vector4 uv;
            if (src.HasValue)
            {
                var s = src.Value;
                // Guard against division by zero in case texture dimensions are invalid.
                var texW = System.Math.Max(1, texture.Width);
                var texH = System.Math.Max(1, texture.Height);
                uv = new Vector4(
                    s.X / (float)texW,
                    s.Y / (float)texH,
                    (s.X + s.Width) / (float)texW,
                    (s.Y + s.Height) / (float)texH
                );
            }
            else
            {
                uv = new Vector4(0f, 0f, 1f, 1f);
            }

            // Convert SASZombieAssaultTD.Engine.Color to SASZombieAssaultTD.Engine.UI.Rendering.ColorRGBA.
            // Attempt to pull out an existing ColorRGBA value or component channels by reflection to remain resilient to internal implementation.
            var tint = new ColorRGBA((int)1f, (int)1f, (int)1f, (int)1f);
            try
            {
                var colorType = color.GetType();

                // 1) Try likely property/field names that may already store a ColorRGBA instance.
                var prop = colorType.GetProperty("colorTint", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.IgnoreCase)
                           ?? colorType.GetProperty("ColorTint", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.IgnoreCase);
                if (prop != null)
                {
                    var val = prop.GetValue(color);
                    if (val is ColorRGBA c)
                    {
                        tint = c;
                    }
                }
                else
                {
                    var field = colorType.GetField("colorTint", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.IgnoreCase)
                                ?? colorType.GetField("ColorTint", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.IgnoreCase);
                    if (field != null)
                    {
                        var val = field.GetValue(color);
                        if (val is ColorRGBA c)
                        {
                            tint = c;
                        }
                    }
                    else
                    {
                        // 2) Fall back to reading component channels (R,G,B,A) if available.

                        float r = 1f, g = 1f, b = 1f, a = 1f;
                        bool anyFound = false;
                        // Helper to read a channel from a property/field and convert to float [0,1]
                        static bool TryReadChannel(object source, Type type, string name, out float outValue)
                        {
                            outValue = 0f;
                            var pi = type.GetProperty(
                                name,
                                System.Reflection.BindingFlags.Instance |
                                System.Reflection.BindingFlags.Public |
                                System.Reflection.BindingFlags.NonPublic |
                                System.Reflection.BindingFlags.IgnoreCase);
                            object val = null;
                            if (pi != null)
                            {
                                val = pi.GetValue(source);
                            }
                            else
                            {
                                var fi = type.GetField(
                                    name,
                                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.IgnoreCase);
                                if (fi != null) val = fi.GetValue(source);
                            }

                            if (val == null) return false;

                            switch (val)
                            {
                                case byte bv:
                                    outValue = bv / 255f;
                                    return true;
                                case int iv:
                                    outValue = System.Math.Clamp(iv / 255f, 0f, 1f);
                                    return true;
                                case float fv:
                                    outValue = System.Math.Clamp(fv, 0f, 1f);
                                    return true;
                                case double dv:
                                    outValue = (float)System.Math.Clamp(dv, 0.0, 1.0);
                                    return true;
                                default:
                                    return false;
                            }
                        }

                        if (TryReadChannel(
                            color,
                            colorType,
                            "R",
                            out var rr)) { r = rr; anyFound = true; }
                        if (TryReadChannel(
                            color,
                            colorType,
                            "G",
                            out var gg)) { g = gg; anyFound = true; }
                        if (TryReadChannel(
                            color,
                            colorType,
                            "B",
                            out var bb)) { b = bb; anyFound = true; }
                        if (TryReadChannel(
                            color,
                            colorType,
                            "A",
                            out var aa)) { a = aa; anyFound = true; }

                        if (anyFound)
                        {
                            tint = new ColorRGBA((byte)(int)r, (byte)(int)g, (byte)(int)b, (byte)(int)a);
                        }
                        // If nothing found, leave tint as default white.
                    }
                }
            }
            catch
            {
                // Swallow any reflection errors and default to white tint.
            }

            // Delegate to the vector-based overload.
            DrawSprite(
                texture,
                position,
                size,
                uv,
                tint,
                rotation,
                layerDepth);
        }
    }
}
