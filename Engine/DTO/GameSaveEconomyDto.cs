// =====================================================================================================
//  FILE: GameSaveEconomyDto.cs
//  PATH: Engine/DTO/GameSaveEconomyDto.cs
//  SUBSYSTEM: Engine DTO
//
//  ROLE:
//      Serialized data container representing the economy-related save-state. This DTO mirrors
//      the EconomyRuntimeState and EconomyRuntimeSummary structures and is used by
//      GameSaveLoadService and GameSaveWriteService for deterministic save/load operations.
//
//  RESPONSIBILITIES:
//      - Provide a pure data representation of economy values for serialization.
//      - Maintain strict separation from engine runtime logic and subsystem execution.
//      - Serve as the economy-specific component of the root GameSaveDto.
//
//  NON-RESPONSIBILITIES:
//      - Executing game logic, subsystem logic, or lifecycle sequencing.
//      - Managing runtime state, summaries, or engine-hosted program behavior.
//      - Performing validation, mutation, or transformation of data.
//
//  ARCHITECTURAL NOTES:
//      - DTOs exist at the Engine root because they are external-facing serialization artifacts.
//      - DTOs must contain pure fields only: no methods, no properties, no logic.
//      - This DTO mirrors the fields exported by EconomyRuntimeState and EconomyRuntimeSummary.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.DTO;

public sealed class GameSaveEconomyDto
{
    public int Currency;
    public int TotalEarned;
    public int TotalSpent;
}
