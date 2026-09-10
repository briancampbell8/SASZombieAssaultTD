// ====================================================================================================
//  FILE: IWaveSpawnGroup.cs
//  PATH: ./Engine/Waves/
//  MODULE: WaveDirector
//
//  ROLE:
//      Load, validate, and construct wave definitions for the WaveDirector subsystem.
//
//  RESPONSIBILITIES:
//      - Provide core functionality for the WaveDirector subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
/*
File:    IWaveSpawnGroup.cs
Purpose: Interface to break circular dependency between Enemy and WaveSpawnGroup.
Features: Defines the WaveSpawnGroup interface that Enemy can reference without circular dependency.
*/

using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Waves
{
    ///<summary>
    ///Interface for wave spawn group to avoid circular dependencies.
    ///</summary>
    public interface IWaveSpawnGroup
    {
        ///<summary>
        ///Trigger enemy spawned callback.
        ///</summary>
        ///<param name="enemy">Spawned enemy.</param>
        void OnEnemySpawnedCallback(Enemy enemy);

        ///<summary>
        ///Get the count of enemies in this spawn group.
        ///</summary>
        int Count { get; }
    }
}

