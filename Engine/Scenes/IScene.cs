// File: IScene.cs
// Purpose: Defines the required lifecycle methods for all scenes in the engine,
//          including initialization, updating, rendering, and shutdown.

using SASZombieAssaultTD.Engine.Rendering;
using System;

namespace SASZombieAssaultTD.Engine.Scenes
{
    public interface IScene
    {
        void Initialize();
        void Update(TimeSpan deltaTime);
        void Render(IRenderContext context);
        void Shutdown();
    }
}