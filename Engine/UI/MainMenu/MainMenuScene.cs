// =====================================================================================================
//  FILE: MainMenuScene.cs
//  PATH: Engine/UI/MainMenu/MainMenuScene.cs
//  SUBSYSTEM: UI.MainMenu
//
//  ROLE:
//      Scene wrapper for the Main Menu subsystem. Owns the MainMenuUI instance, forwards lifecycle
//      events (Update/Render), and provides a clean boundary between the engine scene manager and the
//      UIRoot‑based UI pipeline.
//
//  RESPONSIBILITIES:
//      - Construct MainMenuUI and its UIRoot.
//      - Forward Update() calls to the UI subsystem.
//      - Forward Render() calls to the MainMenuRenderer.
//      - Serve as the active scene for the engine’s SceneManager.
//
//  NON‑RESPONSIBILITIES:
//      - Rendering logic (delegated to MainMenuRenderer).
//      - Input dispatch (UIEventSystem).
//      - Asset loading (UIAssetLoader).
//      - Scene transitions (SceneManager).
//
//  AUTHORSHIP:
//      Modernized by Copilot and Brian Campbell — 2026‑09‑10
// =====================================================================================================

using SASZombieAssaultTD.Engine.Scenes;

namespace SASZombieAssaultTD.Engine.UI.MainMenu
{
    public sealed class MainMenuScene : IScene
    {
        private readonly MainMenuUI _ui;
        private readonly MainMenuRenderer _renderer;

        public MainMenuScene()
        {
            _ui = new MainMenuUI();
            _renderer = new MainMenuRenderer();
        }

        public void Initialize()
        {
            // If your engine requires explicit scene initialization, add it here.
        }

        public void Update(float deltaTime)
        {
            _ui.Root.Update(deltaTime);
        }

        public void Render()
        {
            _renderer.Render(_ui);
        }

        public void Shutdown()
        {
            _ui.Root.Shutdown();
        }
    }
}
