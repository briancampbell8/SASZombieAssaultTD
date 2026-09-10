// =====================================================================================================
//  FILE: GameScene.cs
//  PATH: Engine/Scenes/GameScene.cs
//  SUBSYSTEM: Scene System / Gameplay Scene Orchestration
//
//  ROLE:
//      Authoritative gameplay scene responsible for wiring core engine subsystems, validating
//      deterministic runtime dependencies, and coordinating the update/render lifecycle for
//      gameplay‑specific visual and simulation components.
//
//      GameScene acts as the root execution context for the MeanStreets map, gameplay render
//      systems, HUDManager (state only), HUDRenderer (static HUD drawing), and any future
//      tower‑defense logic. It ensures that all required engine services (RenderManager,
//      UpdateManager, SystemManager, D3D11Adapter_Core) are correctly resolved from GameRootMain
//      and SystemRegistry before gameplay begins.
// =====================================================================================================

using System;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using SASZombieAssaultTD.Engine.Systems;
using SASZombieAssaultTD.Engine.TextureRendering;
using SASZombieAssaultTD.Engine.TextureRendering.HUD;
using SASZombieAssaultTD.Engine.UI.HUD;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Scenes
{
    public sealed class GameScene : BaseScene
    {
        private SystemManager? _systemManager;
        private RenderManager? _renderManager;
        private UpdateManager? _updateManager;
        private D3D11Adapter_Core? _renderContext;

        private TextureManager? _textureManager;

        private Texture2D? _meanStreetsTexture;
        private Texture2D? _hudSupportTexture;
        private Texture2D? _hudMainTexture;

        private RenderSystem? _renderSystem;

        // -------------------------------------------------------------------------------------------------
        // Wiring from SceneManager / GameRootMain
        // -------------------------------------------------------------------------------------------------

        public override void SetGameRoot(GameRootMain root)
        {
            if (root == null)
                throw new ArgumentNullException(nameof(root));

            base.SetGameRoot(root);

            _systemManager = (SystemManager)root.SystemManager;
            var registry = root.SystemRegistry;

            _renderManager = registry.GetService<RenderManager>();
            _updateManager = registry.GetService<UpdateManager>();
            _renderContext = registry.GetService<D3D11Adapter_Core>();
            _textureManager = registry.GetService<TextureManager>();
            _renderSystem = registry.GetService<RenderSystem>();

            DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Info,
                "[GameScene] SetGameRoot: GameRootMain wired. " +
                $"SystemManager={(_systemManager == null ? "NULL" : "VALID")}, " +
                $"RenderManager={(_renderManager == null ? "NULL" : "VALID")}, " +
                $"UpdateManager={(_updateManager == null ? "NULL" : "VALID")}, " +
                $"RenderContext={(_renderContext == null ? "NULL" : "VALID")}, " +
                $"TextureManager={(_textureManager == null ? "NULL" : "VALID")}, " +
                $"RenderSystem={(_renderSystem == null ? "NULL" : "VALID")}");
        }

        public override void SetSceneManager(SceneManager manager)
        {
            if (manager == null)
                throw new ArgumentNullException(nameof(manager));

            base.SetSceneManager(manager);

            DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Info,
                "[GameScene] SetSceneManager: SceneManager wired.");
        }

        // -------------------------------------------------------------------------------------------------
        // Lifecycle: OnLoad / OnStart / OnUnload
        // -------------------------------------------------------------------------------------------------

        internal override void OnLoad()
        {
            DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Info,
                "[GameScene] OnLoad ENTRY");

            if (GameRoot == null || _systemManager == null || _textureManager == null || _renderSystem == null)
            {
                DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Error,
                    "[GameScene] OnLoad: Critical references are NULL. Gameplay scene cannot initialize.");
                return;
            }

            // ==============================================================================================
            // TEXTURE ACQUISITION
            // ==============================================================================================

            _meanStreetsTexture = _textureManager.Get("MeanStreets");
            _hudSupportTexture = _textureManager.Get("HUD_Support");
            _hudMainTexture = _textureManager.Get("HUD_Main");

            if (_meanStreetsTexture == null)
                DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Error, "[GameScene] Missing texture: MeanStreets");

            if (_hudSupportTexture == null)
                DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Error, "[GameScene] Missing texture: HUD_Support");

            if (_hudMainTexture == null)
                DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Error, "[GameScene] Missing texture: HUD_Main");

            // ==============================================================================================
            // RENDER SYSTEM REGISTRATION (GameplayRenderSystem + HUDRenderer)
            // ==============================================================================================

            if (_renderManager != null)
            {
                if (_meanStreetsTexture != null)
                {
                    var gameplaySystem = new GameplayRenderSystem(_meanStreetsTexture);
                    gameplaySystem.SetRenderSystem(_renderSystem);
                    _renderManager.RegisterSystem(gameplaySystem);

                    DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Info,
                        "[GameScene] Registered GameplayRenderSystem (RenderSystem bound).");
                }

                var hudManager = new HUDManager(_systemManager);
                var hudRenderer = new HUDRenderer(hudManager, _systemManager);
                hudRenderer.SetRenderSystem(_renderSystem);
                _renderManager.RegisterSystem(hudRenderer);

                DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Info,
                    "[GameScene] Registered HUDRenderer (RenderSystem bound).");
            }

            DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Info,
                "[GameScene] OnLoad EXIT");
        }

        internal override void OnStart()
        {
            DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Info,
                "[GameScene] OnStart ENTRY");

            DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Info,
                "[GameScene] OnStart EXIT");
        }

        internal override void OnUnload()
        {
            DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Info,
                "[GameScene] OnUnload ENTRY");

            DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Info,
                "[GameScene] OnUnload EXIT");
        }

        // -------------------------------------------------------------------------------------------------
        // Update / Render
        // -------------------------------------------------------------------------------------------------

        internal override void OnUpdate(float deltaTime)
        {
            DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Debug,
                $"[GameScene] OnUpdate: deltaTime={deltaTime:F4}");
        }

        internal void OnRender(D3D11Adapter_Core context)
        {
            // RenderManager handles all actual drawing.
            DLogger.Log(LogSubsystems.Scenes, LogEnums.LogLevel.Debug,
                "[GameScene] OnRender: Frame submitted.");
        }
    }
}
