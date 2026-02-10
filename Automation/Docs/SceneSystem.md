# Scene System

## Purpose
Provide a structured way to manage game states:
- Main menu
- In-game
- Pause

## Components
- Scene (base): Defines Load, Unload, Update, Render.
- SceneManager: Owns the active scene and handles transitions.
- MainMenuScene, GameScene, PauseScene: Concrete implementations.

## Lifecycle
1. SceneManager sets initial scene.
2. GameLoop calls SceneManager.Update(deltaTime).
3. GameLoop calls SceneManager.Render(renderContext).
4. Scenes request transitions via SceneManager.

## Notes
- Scenes should not know about WinForms directly.
- Rendering is done via IRenderContext to keep things engine-agnostic.