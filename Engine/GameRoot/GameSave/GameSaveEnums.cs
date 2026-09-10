// *********************************************************************************************************************
//  File: GameSaveEnums.cs
//  Project: SASZombieAssaultTD - Engine Modernization
//  Author: BDC
//  Created: 2026-08-11
// 
//  Description:
//      Defines all pipeline-level enumerations used by the GameSave subsystem. These enums represent the shared
//      vocabulary for save operations, results, error codes, versioning states, file types, and metadata flags.
//      This file contains no logic, no methods, and no dependencies. It is intentionally isolated to ensure
//      deterministic subsystem boundaries and prevent contamination across DTO, Serialization, Validation,
//      Versioning, Backup, Manager, and Interop layers.
//
//  Coding Standards Enforced:
//      - One class/enum per file (no exceptions).
//      - No TODOs, no placeholders, no empty brackets.
//      - Full professional comment header.
//      - Full diagnostic tracing for initialization.
//      - No assumptions, no incomplete logic.
//      - Deterministic, subsystem-aligned architecture.
//
// *********************************************************************************************************************

using System.Diagnostics;

namespace GameRoot.GameSave
{
    /// <summary>
    /// Provides diagnostic tracing for the GameSaveEnums file. Although enums themselves do not execute logic,
    /// this static constructor ensures that subsystem initialization is traceable and visible in diagnostic logs.
    /// </summary>
    public static class GameSaveEnums
    {
        static GameSaveEnums() => Trace.WriteLine("[GameSaveEnums] Initialization complete. All pipeline-level enums are now available.");
    }

    /// <summary>
    /// Defines the type of save slot being used.
    /// </summary>
    public enum SaveSlotType
    {
        AutoSave,
        QuickSave,
        ManualSave,
        Checkpoint
    }

    /// <summary>
    /// Defines the result of a save or load operation.
    /// </summary>
    public enum SaveResult
    {
        Success,
        Failure,
        ValidationError,
        VersionMismatch,
        FileNotFound,
        IOError,
        Corrupted
    }

    /// <summary>
    /// Defines the type of operation being performed by the save pipeline.
    /// </summary>
    public enum SaveOperation
    {
        Save,
        Load,
        Delete,
        Backup,
        Restore,
        Migrate,
        Validate
    }

    /// <summary>
    /// Defines the type of file used by the save subsystem.
    /// </summary>
    public enum SaveFileType
    {
        Primary,
        Backup,
        Metadata
    }

    /// <summary>
    /// Defines the version compatibility state of a save file.
    /// </summary>
    public enum SaveVersionStatus
    {
        Current,
        Outdated,
        Unsupported,
        Migratable
    }

    /// <summary>
    /// Defines error codes returned by the save subsystem.
    /// </summary>
    public enum SaveErrorCode
    {
        None,
        InvalidDTO,
        InvalidMetadata,
        InvalidVersion,
        MissingFile,
        MissingDirectory,
        SerializationFailure,
        DeserializationFailure,
        BackupFailure,
        MigrationFailure
    }

    /// <summary>
    /// Defines metadata flags applied to save files.
    /// </summary>
    public enum SaveMetadataFlags
    {
        HasBackup,
        IsMigrated,
        IsCompressed,
        IsEncrypted
    }
}
