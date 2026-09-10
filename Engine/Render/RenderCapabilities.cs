/*===========================================================================================================
    File: RenderCapabilities.cs
    Project: SASZombieAssaultTD – Engine Modernization (Deterministic Render Pipeline)
    Author: BDC
    Created: 2026-07-18
    Description:
        RenderCapabilities defines the deterministic configuration object used to control rendering
        behavior for each subsystem during a frame. Unlike RenderMode, RenderPhase, and RenderFeatures,
        which are enums representing single dimensions of rendering behavior, RenderCapabilities acts
        as a composite descriptor that bundles these dimensions together into one unified capability set.

        This object is passed directly into RenderContextD3D11.Configure(), allowing the render context
        to deterministically enable or disable specific operations based on:
            • RenderMode      – Which subsystem is currently rendering (UI, World, HUD, Finalizer, Debug).
            • RenderPhase     – Which stage of the frame is active (BeginFrame, Draw, EndFrame).
            • RenderFeatures  – Fine-grained capability switches (viewport, clear, texture, mesh, present).

        RenderCapabilities is intentionally implemented as a class rather than an enum. This allows
        multiple independent configuration dimensions to be combined without creating a monolithic or
        overloaded enum. The result is a clean, modular, deterministic configuration system that avoids
        namespace collisions, accidental feature combinations, and nondeterministic branching.

    Engine Determinism Notes:
        • RenderCapabilities is the single configuration object consumed by RenderContextD3D11.
        • Subsystems construct deterministic capability sets before issuing draw calls.
        • Prevents subsystems from performing operations outside their designated mode or phase.
        • Ensures strict control over rendering behavior across the entire pipeline.

    Dependencies:
        • Uses: RenderMode.cs, RenderPhase.cs, RenderFeatures.cs
        • Used by: RenderContextD3D11.cs
        • Namespace: SASZombieAssaultTD.Engine.Render

    Revision History:
        • 2026-07-18 – Initial deterministic version created by BDC.
===========================================================================================================*/

using static SASZombieAssaultTD.Engine.Render.RenderEnums;

namespace SASZombieAssaultTD.Engine.Render
{
    public class RenderCapabilities
    {
        public RenderMode Mode;
        public RenderPhase Phase;
        public RenderFeatures Features;
    }
}
