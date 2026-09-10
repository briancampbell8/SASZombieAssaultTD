/*
Program Name: SASZombieAssaultTD
File Path: Engine\Save\SaveMigration.cs
Purpose: Save migration system for version compatibility.
Features: Version-aware migration, backup creation, automatic migration on load, P120/P100 field migration.
*/

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.IO;
using System.Text.Json;
using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

//
using SASZombieAssaultTD.Engine.GameRoot.GamePlay;
namespace SASZombieAssaultTD.Engine.Save
{
    ///<summary>
    ///Save migration system for handling save format version changes.
    ///P140-06: Implements migration strategies for save version compatibility.
    ///</summary>
    public class SaveMigration
    {
        private readonly int _targetVersion;
        private readonly string _saveDirectory;

        ///<summary>
        ///Gets the target save version.
        ///</summary>
        public int TargetVersion => _targetVersion;

        ///<summary>
        ///Event fired when migration starts.
        ///</summary>
        public event Action<string, int, int> OnMigrationStarted;

        ///<summary>
        ///Event fired when migration completes.
        ///</summary>
        public event Action<string, bool> OnMigrationCompleted;

        ///<summary>
        ///Event fired when migration fails.
        ///</summary>
        public event Action<string, Exception> OnMigrationFailed;

        ///<summary>
        ///Initializes a new save migration system.
        ///</summary>
        ///<param name="targetVersion">The target save version.</param>
        ///<param name="saveDirectory">The save directory.</param>
        public SaveMigration(int targetVersion = 2, string? saveDirectory = null)
        {
            _targetVersion = targetVersion;
            _saveDirectory = saveDirectory ?? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "SASZombieAssaultTD",
                "Saves");

            DLogger.Log(LogSubsystems.Unknown, LogEnums.LogLevel.Info, $"SaveMigration: Initialized with target version {_targetVersion}");
        }

        ///<summary>
        ///Migrates save data to the target version.
        ///</summary>
        ///<param name="saveData">The save data to migrate.</param>
        ///<returns>The migrated save data, or null if migration failed.</returns>
        public SaveData? MigrateSaveData(SaveData saveData)
        {
            if (saveData == null)
            {
                DLogger.Log(LogEnums.LogLevel.Warning, "SaveMigration: Cannot migrate null save data");
                return null;
            }

            if (saveData.Version >= _targetVersion)
            {
                DLogger.Log(LogSubsystems.Unknown, LogEnums.LogLevel.Info, $"SaveMigration: Save is already at version {saveData.Version}, no migration needed");
                return saveData;
            }

            try
            {
                var sourceVersion = saveData.Version;
                OnMigrationStarted?.Invoke("SaveData", sourceVersion, _targetVersion);

                //Migrate through each version
                var currentData = saveData;
                for (int version = sourceVersion; version < _targetVersion; version++)
                {
                    currentData = MigrateToNextVersion(currentData, version, version + 1);
                    if (currentData == null)
                    {
                        throw new Exception($"Migration from version {version} to {version + 1} failed");
                    }
                }

                //Note: Version property is read-only in base SaveData
                //Version tracking handled by SaveManager
                OnMigrationCompleted?.Invoke("SaveData", true);

                DLogger.Log(LogSubsystems.Unknown, LogEnums.LogLevel.Info, $"SaveMigration: Successfully migrated from version {sourceVersion} to {_targetVersion}");
                return currentData;
            }
            catch (Exception ex)
            {
                OnMigrationFailed?.Invoke("SaveData", ex);
                DLogger.Log(LogSubsystems.Save, LogEnums.LogLevel.Info, $"SaveMigration: Migration failed - {ex.Message}");
                return null;
            }
        }

        ///<summary>
        ///Migrates save data to the next version.
        ///</summary>
        ///<param name="saveData">The save data to migrate.</param>
        ///<param name="fromVersion">The source version.</param>
        ///<param name="toVersion">The target version.</param>
        ///<returns>The migrated save data.</returns>
        private SaveData MigrateToNextVersion(SaveData saveData, int fromVersion, int toVersion)
        {
            DLogger.Log(LogSubsystems.Unknown, LogEnums.LogLevel.Info, $"SaveMigration: Migrating from version {fromVersion} to {toVersion}");

            return (fromVersion, toVersion) switch
            {
                (1, 2) => MigrateV1ToV2(saveData),
                _ => throw new NotSupportedException($"Migration from version {fromVersion} to {toVersion} is not supported")
            };
        }

        ///<summary>
        ///Migrates from version 1 to version 2.
        ///P140-06: Adds P120/P100 integration fields.
        ///</summary>
        ///<param name="saveData">The version 1 save data.</param>
        ///<returns>The version 2 save data.</returns>
        private SaveData MigrateV1ToV2(SaveData saveData)
        {
            //Convert to SaveDataExtended to add P120/P100 fields
            var extendedData = new SaveDataExtended
            {
                PlayerName = saveData.PlayerName,
                HighScore = saveData.HighScore,
                CurrentLevel = saveData.CurrentLevel,
                TotalKills = saveData.TotalKills,
                TotalWavesCompleted = saveData.TotalWavesCompleted,
                TotalPlayTime = saveData.TotalPlayTime,
                Settings = saveData.Settings.Clone()
            };

            //Copy level progress to battlefield progress if applicable
            //This is a heuristic migration - level IDs might map to battlefield types
            foreach (var levelProgress in saveData.LevelProgress)
            {
                //Try to map level ID to battlefield type
                if (TryMapLevelToBattlefield(levelProgress.LevelId, out var battlefield))
                {
                    extendedData.UpdateBattlefieldProgress(
                        battlefield,
                        levelProgress.Completed,
                        saveData.TotalWavesCompleted, //Use total waves as a fallback
                        levelProgress.HighScore
                    );
                }
            }

            //Initialize default P100 values
            extendedData.CurrentDifficulty = SASZombieAssaultTD.Engine.Waves.DifficultyScaling.DifficultyLevel.Normal;
            extendedData.CurrentWave = 0;

            DLogger.Log(LogSubsystems.Unknown, LogEnums.LogLevel.Info, $"SaveMigration: Migrated V1 to V2 - Added {extendedData.BattlefieldProgress.Count} battlefield progress entries");
            return extendedData;
        }

        ///<summary>
        ///Attempts to map a level ID to a battlefield type.
        ///</summary>
        ///<param name="levelId">The level ID.</param>
        ///<param name="battlefield">The mapped battlefield type.</param>
        ///<returns>True if mapping succeeded.</returns>
        private bool TryMapLevelToBattlefield(string levelId, out BattlefieldType battlefield)
        {
            battlefield = BattlefieldType.MeanStreet; //Default

            //Try to match level ID to battlefield name
            var levelIdLower = levelId.ToLowerInvariant();

            if (levelIdLower.Contains("meanstreet") || levelIdLower.Contains("mean_street"))
            {
                battlefield = BattlefieldType.MeanStreet;
                return true;
            }
            else if (levelIdLower.Contains("subzero") || levelIdLower.Contains("sub_zero"))
            {
                battlefield = BattlefieldType.SubZero;
                return true;
            }
            else if (levelIdLower.Contains("deadwarehouse") || levelIdLower.Contains("dead_warehouse"))
            {
                battlefield = BattlefieldType.DeadWarehouse;
                return true;
            }
            else if (levelIdLower.Contains("shop") || levelIdLower.Contains("shop_til"))
            {
                battlefield = BattlefieldType.ShopTilYouDrop;
                return true;
            }
            else if (levelIdLower.Contains("killtop"))
            {
                battlefield = BattlefieldType.Killtop;
                return true;
            }
            else if (levelIdLower.Contains("touchdown"))
            {
                battlefield = BattlefieldType.Touchdown;
                return true;
            }
            else if (levelIdLower.Contains("cleanup") || levelIdLower.Contains("aisle"))
            {
                battlefield = BattlefieldType.Cleanup;
                return true;
            }
            else if (levelIdLower.Contains("outbreak") || levelIdLower.Contains("mansion"))
            {
                battlefield = BattlefieldType.OutbreakMansion;
                return true;
            }

            return false;
        }

        ///<summary>
        ///Creates a backup of a save file before migration.
        ///</summary>
        ///<param name="filePath">The file path to backup.</param>
        ///<returns>The backup file path, or null if backup failed.</returns>
        public string? CreateBackup(string filePath)
        {
            if (!File.Exists(filePath))
            {
                DLogger.Log(LogEnums.LogLevel.Warning, $"SaveMigration: Cannot backup non-existent file: {filePath}");
                return null;
            }

            try
            {
                var backupPath = $"{filePath}.backup_{DateTime.Now:yyyyMMdd_HHmmss}";
                File.Copy(filePath, backupPath, overwrite: true);
                DLogger.Log(LogSubsystems.Unknown, LogEnums.LogLevel.Info, $"SaveMigration: Created backup at {backupPath}");
                return backupPath;
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.Unknown, LogEnums.LogLevel.Info, $"SaveMigration: Backup failed - {ex.Message}");
                return null;
            }
        }

        ///<summary>
        ///Migrates all save files in the save directory.
        ///</summary>
        ///<returns>Migration result with statistics.</returns>
        public MigrationResult MigrateAllSaves()
        {
            var result = new MigrationResult();

            if (!Directory.Exists(_saveDirectory))
            {
                DLogger.Log(LogEnums.LogLevel.Warning, $"SaveMigration: Save directory does not exist: {_saveDirectory}");
                return result;
            }

            var files = Directory.GetFiles(_saveDirectory, "*.json");

            foreach (var filePath in files)
            {
                try
                {
                    var json = File.ReadAllText(filePath);
                    var saveData = JsonSerializer.Deserialize<SaveData>(json);

                    if (saveData == null)
                    {
                        result.FailedMigrations++;
                        continue;
                    }

                    if (saveData.Version < _targetVersion)
                    {
                        //Create backup
                        var backupPath = CreateBackup(filePath);
                        if (backupPath != null)
                        {
                            result.BackupsCreated++;
                        }

                        //Migrate
                        var migratedData = MigrateSaveData(saveData);
                        if (migratedData != null)
                        {
                            //Save migrated data
                            var migratedJson = JsonSerializer.Serialize(migratedData, new JsonSerializerOptions
                            {
                                WriteIndented = true,
                                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                            });
                            File.WriteAllText(filePath, migratedJson);

                            result.SuccessfulMigrations++;
                        }
                        else
                        {
                            result.FailedMigrations++;
                        }
                    }
                    else
                    {
                        result.SkippedMigrations++;
                    }
                }
                catch (Exception ex)
                {
                    result.FailedMigrations++;
                    DLogger.Log(LogSubsystems.Save, LogEnums.LogLevel.Error,
                        $"SaveMigration: Failed to migrate {Path.GetFileName(filePath)} - {ex.Message}");
                }
            }

            result.TotalSaves = files.Length;
            DLogger.Log(LogSubsystems.Unknown, LogEnums.LogLevel.Info,
                $"SaveMigration: Migration complete - Total: {result.TotalSaves}, Success: {result.SuccessfulMigrations}, " +
                $"Failed: {result.FailedMigrations}, Skipped: {result.SkippedMigrations}, Backups: {result.BackupsCreated}");

            return result;
        }

        ///<summary>
        ///Checks if a save needs migration.
        ///</summary>
        ///<param name="saveData">The save data to check.</param>
        ///<returns>True if migration is needed.</returns>
        public bool NeedsMigration(SaveData saveData)
        {
            return saveData != null && saveData.Version < _targetVersion;
        }
    }

    ///<summary>
    ///Migration result statistics.
    ///</summary>
    public class MigrationResult
    {
        public int TotalSaves { get; set; }
        public int SuccessfulMigrations { get; set; }
        public int FailedMigrations { get; set; }
        public int SkippedMigrations { get; set; }
        public int BackupsCreated { get; set; }

        public override string ToString()
        {
            return $"MigrationResult: Total={TotalSaves}, Success={SuccessfulMigrations}, " +
                   $"Failed={FailedMigrations}, Skipped={SkippedMigrations}, Backups={BackupsCreated}";
        }
    }
}
