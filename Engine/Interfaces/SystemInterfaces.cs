// File: E:\BDC\Projects\SASZombieAssaultTD\Engine\SystemInterfaces.cs
// Defines core system interfaces for system management.

namespace SASZombieAssaultTD.Engine.Interfaces
{
    /// <summary>
    /// Base interface for all managed systems in the engine.
    /// </summary>
    public interface IManagedSystem
    {
        void Initialize();
        void Update(float deltaTime);
        void Shutdown();
    }

    /// <summary>
    /// Interface for systems that require per-frame updates.
    /// </summary>
    public interface IUpdatableSystem : IManagedSystem
    {
        new void Update(float deltaTime);
    }

    /// <summary>
    /// Interface for systems that require rendering.
    /// </summary>
    public interface IRenderableSystem : IManagedSystem
    {
        void Render();
    }
}
