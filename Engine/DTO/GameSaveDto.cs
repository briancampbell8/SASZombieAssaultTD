// =====================================================================================================
//  FILE: GameSaveDto.cs
//  PATH: Engine/DTO/GameSaveDto.cs
//  SUBSYSTEM: Engine DTO Root Container
//
//  ROLE:
//      Aggregates all subsystem-specific DTOs into a single deterministic structure used by
//      GameSaveLoadService and GameSaveWriteService. This class represents the external-facing
//      serialized form of the entire game state.
//
//  RESPONSIBILITIES:
//      - Serve as the top-level container for all save data.
//      - Provide a stable, version-safe structure for serialization and deserialization.
//      - Maintain strict separation from engine runtime logic and subsystem execution.
//
//  NON-RESPONSIBILITIES:
//      - Executing game logic, subsystem logic, or lifecycle sequencing.
//      - Managing runtime state, summaries, or engine-hosted program behavior.
//      - Performing validation, mutation, or transformation of data.
//
//  ARCHITECTURAL NOTES:
//      - DTOs exist at the Engine root because they are external-facing serialization artifacts.
//      - DTOs must contain pure fields only: no methods, no properties, no logic.
//      - DTOs mirror the structure of RuntimeStates and RuntimeSummaries indirectly through
//        their subsystem-specific DTOs.
// =====================================================================================================
using SASZombieAssaultTD.Engine.DTO;
public sealed class GameSaveDto
{
    public GameSaveMetadataDto Metadata;
    public GameSavePlayerDto Player;
    public GameSaveEconomyDto Economy;
    public GameSaveStatisticsDto Statistics;
    public GameSaveUnlockDto Unlocks;
    public GameSaveWaveDto Wave;
    public GameSaveTowerDto[] Towers;
    public GameSaveEnemyDto[] Enemies;
    public GameSaveAchievementDto[] Achievements;
}
