// =====================================================================================================
//  FILE: GameSaveAchievementDto.cs
//  PATH: Engine/DTO/GameSaveAchievementDto.cs
//  SUBSYSTEM: Engine DTO
//
//  ROLE:
//      Serialized data container representing a single achievement's save-state. This DTO mirrors
//      the AchievementRuntimeState and AchievementRuntimeSummary structures and is used by
//      GameSaveLoadService and GameSaveWriteService for deterministic save/load operations.
//
//  NOTES:
//      - Pure data only.
//      - No logic, no methods, no properties.
//      - Mirrors RuntimeState and RuntimeSummary fields exactly.
//      - DTOs exist at the Engine root because they are external-facing serialization artifacts.
// =====================================================================================================

public sealed class GameSaveAchievementDto
{
    public string AchievementId;
    public bool Completed;
    public int Progress;
}
