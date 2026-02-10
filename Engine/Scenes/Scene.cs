/*
    File:    Scene.cs
    Author:  BDC
    Created: 2026-02-07
    Purpose: Base abstract scene class providing lifecycle hooks for all game scenes.
    Notes:   All scenes inherit from this class. Defines Initialize, Update, Render, and Shutdown.
             Implements IScene so all subclasses satisfy the interface automatically.
*/

using SASZombieAssaultTD.Engine.Rendering;
using System;

namespace SASZombieAssaultTD.Engine.Scenes
{
    public abstract class Scene : IScene
    {
        public virtual void Initialize() { }

        public virtual void Update(TimeSpan deltaTime) { }

        public virtual void Render(IRenderContext context) { }

        public virtual void Shutdown() { }
    }
}