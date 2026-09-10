// =====================================================================================================
//  FILE: IScene.cs
//  PATH: Engine/Scenes/IScene.cs
//  SUBSYSTEM: Engine Core — Scene Lifecycle Contract
//
//  ROLE:
//      Defines the deterministic lifecycle surface for all engine scenes. Every scene (MainMenu,
//      MapMenu, HUD, Gameplay, PauseMenu, etc.) MUST implement this interface. GameRootMain uses this
//      contract to drive initialization, updating, rendering, ticking, and shutdown.
//
//  RESPONSIBILITIES:
//      - Provide a strict lifecycle: Initialize → Update → Render → Shutdown.
//      - Serve as the engine-facing boundary for all scene modules.
//      - Support both GPU-context rendering and generic object-based render forwarding.
//      - Allow SceneManager to switch scenes deterministically.
//
//  NON‑RESPONSIBILITIES:
//      - Asset loading (handled by ResourceManager / UIAssetLoader).
//      - Input dispatch (UIEventSystem).
//      - GPU device creation or swap-chain management.
//      - Scene transition logic (SceneManager).
//
//  ARCHITECTURAL NOTES:
//      - Replaces legacy partial lifecycle methods.
//      - GameRootMain delegates lifecycle operations to:
//          • GameRootInitialization
//          • GameRootUpdateLoop
//          • GameRootStateController
//          • GameRootSystemRegistration
//      - All engine-hosted programs MUST implement this interface without exception.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.Scenes
{
    public interface IScene
    {
        /// <summary>
        /// Called once when the scene becomes active.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Called every frame to update scene logic.
        /// </summary>
        void Update(float deltaTime);

        /// <summary>
        /// Called every frame to render the scene.
        /// </summary>
        void Render();

        /// <summary>
        /// Called once when the scene is being removed or replaced.
        /// </summary>
        void Shutdown();
    }
}
