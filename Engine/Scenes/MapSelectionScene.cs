// =====================================================================================================
//  FILE: MapSelectionScene.cs
//  PATH: Engine/Scenes/MapSelectionScene.cs
//  SUBSYSTEM: Scene Layer / Map Selection Foundation
//
//  ROLE:
//      Canonical, permanently defined foundation scene for map selection. Provides the full,
//      deterministic lifecycle surface required by the engine (BaseScene contract) while
//      intentionally containing no gameplay, UI, or map-selection logic.
//
//  RESPONSIBILITIES:
//      - Implement the complete BaseScene lifecycle:
//          • SetGameRoot(GameRootMain root)
//          • SetSceneManager(SceneManager manager)
//          • OnLoad()
//          • OnStart()
//          • OnUpdate(float deltaTime)
//          • OnRender(D3D11Adapter_Core context)
//          • OnUnload()
//      - Provide deterministic logging hooks for lifecycle entry/exit.
//      - Serve as a structurally complete, engine-ready scene that can be safely extended.
//
//  NON-RESPONSIBILITIES:
//      - Presenting map-selection UI.
//      - Handling user input or map choice logic.
//      - Managing gameplay state, HUD, or Finalizer subsystems.
//      - Performing any rendering beyond being a valid scene target.
//
//  ARCHITECTURAL CONTRACT:
//      - This class remains a valid, compilable, fully-formed BaseScene implementation at all times.
//      - SceneManager.CreateScene("MapSelection") returns an instance of this type.
//      - GameRootMain.Initialize() may safely call SceneManager.SetScene("MapSelection").
// =====================================================================================================
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Scenes
{
    public sealed class MapSelectionScene : BaseScene
    {
        // -------------------------------------------------------------------------------------------------
        // Wiring
        // -------------------------------------------------------------------------------------------------

        public override void SetGameRoot(GameRootMain root)
        {
            base.SetGameRoot(root);

            DLogger.Log(LogSubsystems.Scenes, LogLevel.Info,
                "[MapSelectionScene] GameRootMain wired.");
        }

        public override void SetSceneManager(SceneManager manager)
        {
            base.SetSceneManager(manager);

            DLogger.Log(LogSubsystems.Scenes, LogLevel.Info,
                "[MapSelectionScene] SceneManager wired.");
        }

        // -------------------------------------------------------------------------------------------------
        // Lifecycle: OnLoad
        // -------------------------------------------------------------------------------------------------

        internal override void OnLoad()
        {
            DLogger.Log(LogSubsystems.Scenes, LogLevel.Info,
                "[MapSelectionScene] OnLoad ENTRY (foundation, no behavior).");

            DLogger.Log(LogSubsystems.Scenes, LogLevel.Info,
                "[MapSelectionScene] OnLoad EXIT (foundation, no behavior).");
        }

        // -------------------------------------------------------------------------------------------------
        // Lifecycle: OnStart
        // -------------------------------------------------------------------------------------------------

        internal override void OnStart()
        {
            DLogger.Log(LogSubsystems.Scenes, LogLevel.Info,
                "[MapSelectionScene] OnStart ENTRY (foundation, no behavior).");

            DLogger.Log(LogSubsystems.Scenes, LogLevel.Info,
                "[MapSelectionScene] OnStart EXIT (foundation, no behavior).");
        }

        // -------------------------------------------------------------------------------------------------
        // Lifecycle: OnUpdate
        // -------------------------------------------------------------------------------------------------

        internal override void OnUpdate(float deltaTime)
        {
            // Intentionally empty: stable foundation scene.
        }

        // -------------------------------------------------------------------------------------------------
        // Lifecycle: OnRender
        // -------------------------------------------------------------------------------------------------

        internal override void OnRender(D3D11Adapter_Core context)
        {
            // Intentionally empty: valid scene target, no rendering at foundation stage.
        }

        // -------------------------------------------------------------------------------------------------
        // Lifecycle: OnUnload
        // -------------------------------------------------------------------------------------------------

        internal override void OnUnload()
        {
            DLogger.Log(LogSubsystems.Scenes, LogLevel.Info,
                "[MapSelectionScene] OnUnload ENTRY (foundation, no behavior).");

            DLogger.Log(LogSubsystems.Scenes, LogLevel.Info,
                "[MapSelectionScene] OnUnload EXIT (foundation, no behavior).");
        }
    }
}
