// =====================================================================================================
//  FILE: GameSaveMetadataDto.cs
//  PATH: Engine/DTO/GameSaveMetadataDto.cs
//  SUBSYSTEM: Engine DTO
//
//  ROLE:
//      Serialized data container representing save-file metadata. Used by GameSaveLoadService and
//      GameSaveWriteService to track versioning, map idECSEntityCore, difficulty mode, and timestamps.
//
//  RESPONSIBILITIES:
//      - Provide pure field-based metadata for deterministic serialization.
//      - Maintain strict separation from engine runtime logic and subsystem execution.
//      - Serve as the metadata-specific component of the root GameSaveDto.
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

public sealed class GameSaveMetadataDto
{
    public string SaveVersion;
    public string MapName;
    public string Difficulty;
    public long CreatedTimestamp;
    public long UpdatedTimestamp;
}
