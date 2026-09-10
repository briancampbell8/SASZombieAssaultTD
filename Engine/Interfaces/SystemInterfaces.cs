// ====================================================================================================
//  FILE: SystemInterfaces.cs
//  PATH: ./Engine/Interfaces/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the SystemInterfaces module.
//
//  RESPONSIBILITIES:
//      - Provide core functionality for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
//File: E:\BDC\Projects\SASZombieAssaultTD\Engine\SystemInterfaces.cs
//Defines core system interfaces for system management.

using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Interfaces
{
    ///<summary>
    ///Base interface for all managed systems in the engine.
    ///</summary>
    public interface IManagedSystem
    {
        void Initialize();
        void Update(float deltaTime);
        void Shutdown();
    }

    ///<summary>
    ///Interface for systems that require per-frame updates.
    ///</summary>
    public interface IUpdatableSystem : IManagedSystem
    {
        new void Update(float deltaTime);
    }

    ///<summary>
    ///Interface for systems that require rendering.
    ///</summary>
    public interface IRenderableSystem : IManagedSystem
    {
        void Render();
    }
}

