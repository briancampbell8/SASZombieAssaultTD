// ====================================================================================================
//  FILE: IGameSystem.cs
//  PATH: ./Engine/Gameplay/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the IGameSystem module.
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
using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.GameRoot.GamePlay
{
    ///<summary>
    ///Interface for game systems that can be initialized, updated, and managed.
    ///</summary>
    public interface IGameSystem
    {
        ///<summary>
        ///Initialize the game system.
        ///</summary>
        void Initialize();

        ///<summary>
        ///Update the game system.
        ///</summary>
        ///<param name="deltaTime">Time elapsed since last update.</param>
        void Update(float deltaTime);
    }
}

