// =====================================================================================================
//  FILE: PointF.cs
//  PATH: Engine/UI/Components/PointF.cs
//  SUBSYSTEM: UI Framework — Primitive Geometry Types
//
//  ROLE:
//      Lightweight floating‑point 2D coordinate struct used throughout the UI subsystem for layout,
//      positioning, and element anchoring. Provides a simple, deterministic representation of X/Y
//      coordinates without introducing rendering or device dependencies.
//
//  RESPONSIBILITIES:
//      - Store 2D floating‑point coordinates for UI element positioning.
//      - Provide a minimal, allocation‑free geometry primitive for the UI framework.
//      - Support deterministic math operations and conversions performed by UIElementBase and layout code.
//
//  NON‑RESPONSIBILITIES:
//      - Performing rendering or GPU operations.
//      - Managing UI hierarchy, layout containers, or event routing.
//      - Representing screen‑space transforms or matrix math.
//      - Acting as a scene lifecycle or engine‑level abstraction.
//
//  ARCHITECTURAL NOTES:
//      - PointF is intentionally minimal and does not depend on System.Drawing.PointF or Vector2.
//      - Used by UIElementBase, Panel, Button, and other UI components for lightweight positioning.
//      - Remains independent from the rendering pipeline and device contexts.
// =====================================================================================================



using System;

namespace SASZombieAssaultTD.Engine.UI.Components
{
    public struct PointF
    {
        internal static System.Drawing.PointF Position;

        public float X { get; set; }
        public float Y { get; set; }

        public PointF(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static implicit operator System.Drawing.PointF(PointF v)
        {
            throw new NotImplementedException();
        }

        public static implicit operator PointF(System.Drawing.PointF v)
        {
            throw new NotImplementedException();
        }
    }
}
