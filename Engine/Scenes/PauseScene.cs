// =====================================================================================================
//  FILE: PauseScene.cs
//  PATH: Engine/Scenes/PauseScene.cs
//  SUBSYSTEM: Scene System / Pause Overlay Scene
//
//  ROLE:
//      Deterministic pause overlay scene that displays a pause UI and waits for user input to resume.
//      Lightweight overlay that renders on top of the active gameplay scene.
//
//  RESPONSIBILITIES:
//      - Implement full BaseScene lifecycle deterministically.
//      - Display pause overlay text.
//      - Detect resume input (ESC).
//      - Transition back to the previous scene via SceneManager.
//
//  NON-RESPONSIBILITIES:
//      - Rendering gameplay content.
//      - Managing HUD or UI overlays.
//      - Performing GPU resource creation.
//      - Handling gameplay logic.
//
//  ARCHITECTURAL NOTES:
//      - Permanently coded right and complete as the canonical PauseScene foundation.
//      - SceneManager invokes OnLoad → OnStart → OnUpdate → OnRender → OnUnload deterministically.
// =====================================================================================================

using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Input;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using SASZombieAssaultTD.Engine.VectorMath;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Scenes
{
    public sealed class PauseScene : BaseScene
    {
        private bool _resumeRequested;
        private string? _previousSceneName;

        // -------------------------------------------------------------------------------------------------
        // CONSTRUCTOR
        // -------------------------------------------------------------------------------------------------

        public PauseScene(string? previousSceneName = null)
        {
            _previousSceneName = previousSceneName;

            DLogger.Log(LogSubsystems.Scenes, LogLevel.Info,
                "[PauseScene] Constructor: PauseScene created.");
        }

        // -------------------------------------------------------------------------------------------------
        // WIRING
        // -------------------------------------------------------------------------------------------------

        public override void SetSceneManager(SceneManager manager)
        {
            base.SetSceneManager(manager);

            DLogger.Log(LogSubsystems.Scenes, LogLevel.Info,
                "[PauseScene] SceneManager wired.");
        }

        // -------------------------------------------------------------------------------------------------
        // LIFECYCLE: LOAD / START / UPDATE / RENDER / UNLOAD
        // -------------------------------------------------------------------------------------------------

        internal override void OnLoad()
        {
            DLogger.Log(LogSubsystems.Scenes, LogLevel.Info,
                "[PauseScene] OnLoad ENTRY");

            _resumeRequested = false;

            DLogger.Log(LogSubsystems.Scenes, LogLevel.Info,
                "[PauseScene] OnLoad EXIT");
        }

        internal override void OnStart()
        {
            DLogger.Log(LogSubsystems.Scenes, LogLevel.Info,
                "[PauseScene] OnStart ENTRY");

            _resumeRequested = false;

            DLogger.Log(LogSubsystems.Scenes, LogLevel.Info,
                "[PauseScene] OnStart EXIT");
        }

        internal override void OnUpdate(float deltaTime)
        {
            if (InputRouter != null &&
                InputSystem.IsKeyPressed(KeyCode.Escape))
            {
                _resumeRequested = true;
            }

            if (_resumeRequested && _previousSceneName != null)
            {
                DLogger.Log(LogSubsystems.Scenes, LogLevel.Info,
                    "[PauseScene] Resume requested — transitioning back to previous scene.");

                SceneManager?.SetScene(_previousSceneName);
            }
        }

        internal override void OnRender(D3D11Adapter_Core context)
        {
            if (context == null)
                return;

            context.DrawRectangle(
                0, 0,
                context.ScreenWidth,
                context.ScreenHeight,
                Color.FromArgb(128, 0, 0, 0));

            context.DrawText(
                "PAUSED",
                new Vector3(context.ScreenWidth / 2 - 80, context.ScreenHeight / 2 - 40, 0),
                Color.White,
                32);

            context.DrawText(
                "Press ESC to Resume",
                new Vector3(context.ScreenWidth / 2 - 140, context.ScreenHeight / 2 + 10, 0),
                Color.White,
                20);
        }

        internal override void OnUnload()
        {
            DLogger.Log(LogSubsystems.Scenes, LogLevel.Info,
                "[PauseScene] OnUnload ENTRY");

            _resumeRequested = false;

            DLogger.Log(LogSubsystems.Scenes, LogLevel.Info,
                "[PauseScene] OnUnload EXIT");
        }

        // -------------------------------------------------------------------------------------------------
        // PUBLIC API
        // -------------------------------------------------------------------------------------------------

        public void SetPreviousScene(string sceneName)
        {
            _previousSceneName = sceneName;
        }

        public void RequestResume()
        {
            _resumeRequested = true;
        }
    }
}
