// ============================================================================
// File Path: Engine/Rendering/CompositeBuilder.cs
// File: CompositeBuilder.cs
// Program: CompositeBuilder
// Subsystem: Rendering / Compositing
//
// Purpose:
//     Builds a composite texture by loading individual PNG textures,
//     drawing them onto a composite surface in the correct order,
//     and producing a final EngineTexture.
//
// Diagnostics:
//     - Uses engine-native Diagnostics.Trace()
//     - Fully doctrine-compliant naming
//     - Subsystem.Operation.Stage.Event format
// ============================================================================

using Engine.Rendering.Interfaces;
using EngineDiagnostics = SASZombieAssaultTD.Engine.Diagnostics;
using SystemDiagnostics = System.Diagnostics;

namespace SASZombieAssaultTD.Engine.Rendering
{
    public class CompositeBuilder
    {
        private readonly CompositeImageLoader _loader;
        private readonly CompositeMergeTool _mergeTool;
        private readonly CompositeCreator _creator;

        public CompositeBuilder(
            CompositeImageLoader loader,
            CompositeMergeTool mergeTool,
            CompositeCreator creator)
        {
            _loader = loader;
            _mergeTool = mergeTool;
            _creator = creator;
        }

        public CompositeBuilder(CompositeImageLoader loader, CompositeCreator creator, CompositeMergeTool merger)
        {
            _loader = loader;
            _creator = creator;
        }

        public EngineTexture BuildComposite(
            string mapPath,
            string hudPath,
            string supportHudPath,
            int hudX,
            int hudY,
            int supportHudX,
            int supportHudY)
        {
            Engine.Diagnostics.DebugLogger.Trace("CompositeBuilder.BuildComposite.Start",
                $"map='{mapPath}', hud='{hudPath}', support='{supportHudPath}'");

            // --------------------------------------------------------------------
            // LOAD TEXTURES
            // --------------------------------------------------------------------
            Engine.Diagnostics.DebugLogger.Trace("CompositeBuilder.BuildComposite.Load.MapTexture.Start", mapPath);
            EngineTexture mapTexture = _loader.LoadPng(mapPath);
            Engine.Diagnostics.DebugLogger.Trace("CompositeBuilder.BuildComposite.Load.MapTexture.Complete",
                $"{mapTexture.Width}x{mapTexture.Height}");

            Engine.Diagnostics.DebugLogger.Trace("CompositeBuilder.BuildComposite.Load.HudTexture.Start", hudPath);
            EngineTexture hudTexture = _loader.LoadPng(hudPath);
            Engine.Diagnostics.DebugLogger.Trace("CompositeBuilder.BuildComposite.Load.HudTexture.Complete",
                $"{hudTexture.Width}x{hudTexture.Height}");

            Engine.Diagnostics.DebugLogger.Trace("CompositeBuilder.BuildComposite.Load.SupportHudTexture.Start", supportHudPath);
            EngineTexture supportHudTexture = _loader.LoadPng(supportHudPath);
            Engine.Diagnostics.DebugLogger.Trace("CompositeBuilder.BuildComposite.Load.SupportHudTexture.Complete",
                $"{supportHudTexture.Width}x{supportHudTexture.Height}");

            // --------------------------------------------------------------------
            // CREATE SURFACE
            // --------------------------------------------------------------------
            Engine.Diagnostics.DebugLogger.Trace("CompositeBuilder.BuildComposite.Surface.Create.Start",
                $"{mapTexture.Width}x{mapTexture.Height}");

            ICompositeSurface surface = _creator.CreateSurface(
                mapTexture.Width,
                mapTexture.Height);

            Engine.Diagnostics.DebugLogger.Trace("CompositeBuilder.BuildComposite.Surface.Create.Complete", "OK");

            // --------------------------------------------------------------------
            // DRAW LAYERS
            // --------------------------------------------------------------------
            Engine.Diagnostics.DebugLogger.Trace("CompositeBuilder.BuildComposite.Draw.Map.Start", "0,0");
            _mergeTool.DrawTexture(surface, mapTexture, 0, 0);
            Engine.Diagnostics.DebugLogger.Trace("CompositeBuilder.BuildComposite.Draw.Map.Complete", "OK");

            Engine.Diagnostics.DebugLogger.Trace("CompositeBuilder.BuildComposite.Draw.Hud.Start", $"{hudX},{hudY}");
            _mergeTool.DrawTexture(surface, hudTexture, hudX, hudY);
            Engine.Diagnostics.DebugLogger.Trace("CompositeBuilder.BuildComposite.Draw.Hud.Complete", "OK");
            Engine.Diagnostics.DebugLogger.Trace("CompositeBuilder.BuildComposite.Draw.SupportHud.Start",
                $"{supportHudX},{supportHudY}");
            _mergeTool.DrawTexture(surface, supportHudTexture, supportHudX, supportHudY);
            Engine.Diagnostics.DebugLogger.Trace("CompositeBuilder.BuildComposite.Draw.SupportHud.Complete", "OK");

            // --------------------------------------------------------------------
            // FINALIZE
            // --------------------------------------------------------------------
            Engine.Diagnostics.DebugLogger.Trace("CompositeBuilder.BuildComposite.Texture.Create.Start", "Converting surface");
            EngineTexture finalTexture = _creator.CreateTexture(surface);
            Engine.Diagnostics.DebugLogger.Trace("CompositeBuilder.BuildComposite.Texture.Create.Complete", "OK");

            Engine.Diagnostics.DebugLogger.Trace("CompositeBuilder.BuildComposite.End", "Success");
            return finalTexture;
        }
    }
}
