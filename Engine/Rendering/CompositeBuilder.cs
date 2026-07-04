// ====================================================================================================
//  FILE: CompositeBuilder.cs
//  PATH: Engine/Rendering/ 
//  PROGRAM: CompositeBuilder.cs
//  MODULE: Resource Management Framework
//  ROLE:
//      Defines the structures, loaders, and integration points responsible for discovering, validating, and providing engine resources in a deterministic manner.
//
//  RESPONSIBILITIES:
//      - Provide a unified API for loading, caching, and resolving engine resources.
//      - Enforce deterministic resource lookup and lifecycle rules.
//      - Abstract file formats, storage locations, and integration layers behind a stable interface.
//      - Ensure resource availability for all engine subsystems (Rendering, Audio, Gameplay, UI).
//
//  NON-RESPONSIBILITIES:
//      - Performing rendering or GPU upload operations.
//      - Managing gameplay logic or scene entities.
//      - Handling diagnostics, logging, or performance metrics.
//      - Encoding or authoring resource files.
//
//  ARCHITECTURAL NOTES:
//      - The Resource Management Framework acts as the central authority for all asset retrieval.
//      - Resource modules must remain pure: no side effects outside resource acquisition and validation.
//      - All resource types (textures, data files, definitions, metadata) must follow deterministic load rules.
//  ====================================================================================================

using Engine.Rendering.Interfaces;
using SASZombieAssaultTD.Engine.Diagnostics;
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
            DLogger.Log(
                LogSubsystems.Rendering,
                LogLevel.Debug,
                "CompositeBuilder.BuildComposite.Start",
                $"map='{mapPath}', hud='{hudPath}', support='{supportHudPath}'");

            //--------------------------------------------------------------------
            //LOAD TEXTURES
            //--------------------------------------------------------------------
            DLogger.Log(
                LogSubsystems.Rendering,
                LogLevel.Debug,
                "CompositeBuilder.BuildComposite.Load.MapTexture.Start", mapPath);
            EngineTexture mapTexture = _loader.LoadPng(mapPath);
            DLogger.Log(
                LogSubsystems.Rendering,
                LogLevel.Debug,
                "CompositeBuilder.BuildComposite.Load.MapTexture.Complete",
                $"{mapTexture.Width}x{mapTexture.Height}");

            DLogger.Log(
                LogSubsystems.Rendering,
                LogLevel.Debug,
                "CompositeBuilder.BuildComposite.Load.HudTexture.Start", hudPath);
            EngineTexture hudTexture = _loader.LoadPng(hudPath);
            DLogger.Log(
                LogSubsystems.Rendering,
                LogLevel.Debug,
                "CompositeBuilder.BuildComposite.Load.HudTexture.Complete",
                $"{hudTexture.Width}x{hudTexture.Height}");

            DLogger.Log(
                LogSubsystems.Rendering,
                LogLevel.Debug,
                "CompositeBuilder.BuildComposite.Load.SupportHudTexture.Start", supportHudPath);
            EngineTexture supportHudTexture = _loader.LoadPng(supportHudPath);
            DLogger.Log(
                LogSubsystems.Rendering,
                LogLevel.Debug,
                "CompositeBuilder.BuildComposite.Load.SupportHudTexture.Complete",
                $"{supportHudTexture.Width}x{supportHudTexture.Height}");

            //--------------------------------------------------------------------
            //CREATE SURFACE
            //--------------------------------------------------------------------
            DLogger.Log(
                LogSubsystems.Rendering,
                LogLevel.Debug,
                "CompositeBuilder.BuildComposite.Surface.Create.Start",
                $"{mapTexture.Width}x{mapTexture.Height}");

            ICompositeSurface surface = _creator.CreateSurface(
                mapTexture.Width,
                mapTexture.Height);

            DLogger.Log(
                LogSubsystems.Rendering,
                LogLevel.Debug,
                "CompositeBuilder.BuildComposite.Surface.Create.Complete", "OK");

            //--------------------------------------------------------------------
            //DRAW LAYERS
            //--------------------------------------------------------------------
            DLogger.Log(
                LogSubsystems.Rendering,
                LogLevel.Debug,
                "CompositeBuilder.BuildComposite.Draw.Map.Start", "0,0");
            _mergeTool.DrawTexture(surface, mapTexture, 0, 0);
            DLogger.Log(
                LogSubsystems.Rendering,
                LogLevel.Debug,
                "CompositeBuilder.BuildComposite.Draw.Map.Complete", "OK");

            DLogger.Log(
                LogSubsystems.Rendering,
                LogLevel.Debug,
                "CompositeBuilder.BuildComposite.Draw.Hud.Start", $"{hudX},{hudY}");
            _mergeTool.DrawTexture(surface, hudTexture, hudX, hudY);
            DLogger.Log(
                LogSubsystems.Rendering,
                LogLevel.Debug,
                "CompositeBuilder.BuildComposite.Draw.Hud.Complete", "OK");
            DLogger.Log(
                LogSubsystems.Rendering,
                LogLevel.Debug,
                "CompositeBuilder.BuildComposite.Draw.SupportHud.Start",
                $"{supportHudX},{supportHudY}");
            _mergeTool.DrawTexture(surface, supportHudTexture, supportHudX, supportHudY);
            DLogger.Log(
                LogSubsystems.Rendering,
                LogLevel.Debug,
                "CompositeBuilder.BuildComposite.Draw.SupportHud.Complete", "OK");

            //--------------------------------------------------------------------
            //FINALIZE
            //--------------------------------------------------------------------
            DLogger.Log(
                LogSubsystems.Rendering,
                LogLevel.Debug,
                "CompositeBuilder.BuildComposite.Texture.Create.Start", "Converting surface");
            EngineTexture finalTexture = _creator.CreateTexture(surface);
            DLogger.Log(
                LogSubsystems.Rendering,
                LogLevel.Debug,
                "CompositeBuilder.BuildComposite.Texture.Create.Complete", "OK");

            DLogger.Log(
                LogSubsystems.Rendering,
                LogLevel.Debug,
                "CompositeBuilder.BuildComposite.End", "OK");
            DLogger.Log(
                LogSubsystems.Rendering,
                LogLevel.Debug,
                "CompositeBuilder.BuildComposite.End", "Success");

            return finalTexture;
        }
    }
}
