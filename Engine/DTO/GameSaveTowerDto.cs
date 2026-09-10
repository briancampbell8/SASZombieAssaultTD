// =====================================================================================================
//  FILE: GameSaveTowerDto.cs
//  PATH: Engine/DTO/GameSaveTowerDto.cs
//  SUBSYSTEM: Engine DTO
//
//  ROLE:
//      Serialized data container representing a single tower's save-state. Used by
//      GameSaveLoadService and GameSaveWriteService during deterministic save/load operations.
//
//  RESPONSIBILITIES:
//      - Provide pure field-based data for tower idECSEntityCore, type, position, level, and shot counts.
//      - Maintain strict separation from engine runtime logic and subsystem execution.
//      - Serve as the tower-specific component of the root GameSaveDto.
//
//  NON-RESPONSIBILITIES:
//      - Executing game logic, subsystem logic, or lifecycle sequencing.
//      - Managing runtime state, summaries, or engine-hosted program behavior.
//      - Performing validation, mutation, or transformation of data.
//
//  ARCHITECTURAL NOTES:
//      - DTOs exist at the Engine root because they are external-facing serialization artifacts.
//      - DTOs contain pure fields only: no methods, no properties, no logic.
//      - This header is a template updated to match the purpose of this DTO file.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.DTO;

public sealed class GameSaveTowerDto
{
    public string TowerId;
    public string TowerType;
    public int Level;
    public float X;
    public float Y;
    public int ShotsFired;
}
