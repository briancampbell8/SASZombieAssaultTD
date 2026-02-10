// File: SceneManager.cs
// Purpose: Owns the active scene and forwards Update/Render calls.

using SASZombieAssaultTD.Engine.Rendering;
using System;

namespace SASZombieAssaultTD.Engine.Scenes
{
    public sealed class SceneManager
    {
        private IScene? _currentScene;
        public SceneManager()
        {
        }

        public void SetScene(IScene scene)
        {
            if (scene == null)
                throw new ArgumentNullException(nameof(scene));

            _currentScene = scene;
            _currentScene.Initialize();
        }

        public void Update(TimeSpan deltaTime)
        {
            if (_currentScene == null)
                return;

            _currentScene.Update(deltaTime);
        }

        public void Render(IRenderContext context)
        {
            if (_currentScene == null)
                return;

            if (context == null)
                throw new ArgumentNullException(nameof(context));

            _currentScene.Render(context);
        }
    }
}