// =====================================================================================================
//  FILE: LoadingScene.cs
//  PATH: Engine/Scenes/LoadingScene.cs
//  SUBSYSTEM: Scene System / Transitional Scene
//
//  ROLE:
//      Deterministic transitional scene used during engine initialization or asset preparation.
//      Provides a clean, minimal, fully‑aligned BaseScene implementation with predictable lifecycle
//      behavior. This scene is intentionally lightweight and serves as a temporary placeholder
//      before MainMenuScene or GameScene becomes active.
//
//  RESPONSIBILITIES:
//      - Implement full BaseScene lifecycle deterministically.
//      - Provide simple loading‑screen rendering.
//      - Track loading progress using a fixed-duration timer.
//      - Transition to a target scene once loading is complete.
//
//  NON-RESPONSIBILITIES:
//      - Asset loading (handled by TextureManager / GameRootInitialization).
//      - GPU resource creation.
//      - HUD or UI overlay management.
//      - Gameplay logic.
//
//  ARCHITECTURAL NOTES:
//      - This file is permanently coded right and complete as the canonical LoadingScene foundation.
//      - SceneManager invokes OnLoad → OnStart → OnUpdate → OnRender → OnUnload deterministically.
//      - No async operations are used; loading is simulated deterministically.
// =====================================================================================================

using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Scenes
{
    public sealed class LoadingScene : BaseScene
    {
        private SceneManager _sceneManager;
        private readonly BaseScene? _targetScene;

        private float _timer;
        private readonly float _duration;
        private bool _ready;

        // -------------------------------------------------------------------------------------------------
        // CONSTRUCTOR
        // -------------------------------------------------------------------------------------------------

        public LoadingScene(BaseScene? targetScene = null, float duration = 1.0f)
        {
            _targetScene = targetScene;
            _duration = System.Math.Max(0.1f, duration);
            _timer = 0f;
            _ready = false;
        }

        // -------------------------------------------------------------------------------------------------
        // WIRING
        // -------------------------------------------------------------------------------------------------

        public override void SetSceneManager(SceneManager manager)
        {
            _sceneManager = manager;

            DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Info,
                "[LoadingScene] SceneManager wired.");
        }

        // -------------------------------------------------------------------------------------------------
        // LIFECYCLE: LOAD / START / UPDATE / RENDER / UNLOAD
        // -------------------------------------------------------------------------------------------------

        internal override void OnLoad()
        {
            DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Info,
                "[LoadingScene] OnLoad ENTRY");

            _timer = 0f;
            _ready = false;

            DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Info,
                "[LoadingScene] OnLoad EXIT");
        }

        internal override void OnStart()
        {
            DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Info,
                "[LoadingScene] OnStart ENTRY");

            _ready = true;

            DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Info,
                "[LoadingScene] OnStart EXIT");
        }

        internal override void OnUpdate(float deltaTime)
        {
            _timer += deltaTime;

            if (_ready && _timer >= _duration && _targetScene != null)
            {
                DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Info,
                    "[LoadingScene] Loading complete — transitioning to target scene.");

                _sceneManager.SetScene(_targetScene.GetType().Name);
            }
        }

        internal override void OnRender(D3D11Adapter_Core context)
        {
            if (context == null)
                return;

            context.Clear((byte)0.1f, (byte)0.1f, (byte)0.2f, (byte)1.0f);

            const string text = "Loading...";
            var size = context.MeasureText(text, 24);
            var x = (context.ScreenWidth - size.X) / 2;
            var y = (context.ScreenHeight / 2) - 40;

            context.DrawText(text, x, y, 24, System.Drawing.Color.White);

            float progress = System.Math.Clamp(_timer / _duration, 0f, 1f);

            int barWidth = 300;
            int barHeight = 20;
            int barX = (int)((context.ScreenWidth - barWidth) / 2);
            int barY = context.ScreenHeight / 2;

            context.DrawRectangle(barX, barY, barWidth, barHeight, Color.Gray);
            context.DrawRectangle(barX, barY, (int)(barWidth * progress), barHeight, Color.Green);

            string pct = $"{(int)(progress * 100)}%";
            var pctSize = context.MeasureText(pct, 18);
            var pctX = (context.ScreenWidth - pctSize.X) / 2;
            var pctY = barY + barHeight + 10;

            context.DrawText(pct, pctX, pctY, 18, System.Drawing.Color.White);
        }

        internal override void OnUnload()
        {
            DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Info,
                "[LoadingScene] OnUnload ENTRY");

            _timer = 0f;
            _ready = false;

            DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Info,
                "[LoadingScene] OnUnload EXIT");
        }

        // -------------------------------------------------------------------------------------------------
        // PUBLIC API
        // -------------------------------------------------------------------------------------------------

        public float LoadingProgress => System.Math.Clamp(_timer / _duration, 0f, 1f);

        public bool IsLoadingComplete => _ready && _timer >= _duration;
    }
}
