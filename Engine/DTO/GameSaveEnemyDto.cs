// =====================================================================================================
//  FILE: GameSaveEnemyDto.cs
//  PATH: Engine/DTO/GameSaveEnemyDto.cs
//  SUBSYSTEM: Engine DTO
//
//  ROLE:
//      Placeholder header for format compliance. Represents the serialized save-state for a single
//      enemy instance. Used by GameSaveLoadService and GameSaveWriteService during deterministic
//      save/load operations.
//
//  RESPONSIBILITIES:
//      - Provide pure field-based data for enemy position, health, type, and wave association.
//      - Maintain strict separation from engine runtime logic and subsystem execution.
//      - Serve as the enemy-specific component of the root GameSaveDto.
//
//  NON-RESPONSIBILITIES:
//      - Executing game logic, subsystem logic, or lifecycle sequencing.
//      - Managing runtime state, summaries, or engine-hosted program behavior.
//      - Performing validation, mutation, or transformation of data.
//
//  ARCHITECTURAL NOTES:
//      - DTOs exist at the Engine root because they are external-facing serialization artifacts.
//      - DTOs contain pure fields only: no methods, no properties, no logic.
//      - This header is a placeholder for format compliance and does not imply subsystem behavior.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.DTO;

public sealed class GameSaveEnemyDto
{
    public string EnemyId;
    public string EnemyType;
    public float X;
    public float Y;
    public int Health;
    public int WaveIndex;
}
