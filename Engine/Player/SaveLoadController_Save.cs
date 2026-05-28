/// File:    E:\BDC\Projects\SASZombieAssaultTD\Engine\Player\SaveLoadController_Save.cs
/// Purpose: Player action validation and execution system for SAS Zombie Assault TD.
/// Features: Tower placement validation, upgrade processing, damage handling, and game state management.
/// Validation: Comprehensive action validation with game state checking and affordability validation.
/// Performance: Optimized for frequent action processing with minimal overhead.
/// Threading: Thread-safe operations with proper locking for concurrent access.
/// Integration: Designed for use with PlayerSystem, TowerManager, and WaveManager.
/// Persistence: Action logging for debugging and player feedback.
/// ****************************************************************************************************
using SASZombieAssaultTD.Engine.Diagnostics;

using System;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Player
{
    public partial class SaveLoadController
    {
        /// <summary>
        /// Saves the current player game state to file.
        /// </summary>
        /// <param name="playerSystem">The PlayerSystem instance containing the current game state to save.</param>
        /// <returns>True if the save was successful, false otherwise.</returns>
        public bool SaveGame(PlayerSystem playerSystem)
        {
            if (playerSystem == null)
                throw new ArgumentNullException(nameof(playerSystem));

            lock (_lock)
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine("Info", "SaveLoadController: Starting save operation");

                    // Validate player system state
                    if (!SaveLoadCore.ValidatePlayerSystemForSave(playerSystem))
                    {
                        System.Diagnostics.Debug.WriteLine("Error", "SaveLoadController: Player system validation failed");
                        return false;
                    }

                    // Create player data structure
                    var playerData = CreatePlayerDataFromSystem(playerSystem);

                    // Validate player data
                    if (!SaveLoadCore.ValidatePlayerData(playerData))
                    {
                        System.Diagnostics.Debug.WriteLine("Error", "SaveLoadController: Player data validation failed");
                        return false;
                    }

                    // Serialize to JSON using SaveLoadCore
                    string json = SaveLoadCore.Serialize(playerData);

                    // Create backup of existing save file
                    CreateBackup();

                    // Write save file
                    WriteSaveFile(json);

                    // Verify file integrity
                    if (!VerifySaveFile())
                    {
                        System.Diagnostics.Debug.WriteLine("Error", "SaveLoadController: Save file verification failed");
                        RestoreBackup();
                        return false;
                    }

                    System.Diagnostics.Debug.WriteLine(
                        "Info",
                        $"SaveLoadController: Save completed successfully - File: {_savePath}, Size: {json.Length} bytes");

                    return true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error", $"SaveLoadController: Save failed - Error: {ex.Message}");

                    // Attempt to restore backup on failure
                    try
                    {
                        RestoreBackup();
                        System.Diagnostics.Debug.WriteLine("Info", "SaveLoadController: Backup restored after save failure");
                    }
                    catch (Exception backupEx)
                    {
                        System.Diagnostics.Debug.WriteLine("Error", $"SaveLoadController: Backup restore failed - Error: {backupEx.Message}");
                    }

                    return false;
                }
            }
        }

        private PlayerData CreatePlayerDataFromSystem(PlayerSystem playerSystem)
        {
            NI.Hit();
            return default(PlayerData);
        }

        /// <summary>
        /// Creates player data structure from player system.
        /// </summary>
        internal void RestoreExperience(int experience)
        {
            // Access Progression directly on the system, not via State
            if (this.Progression != null)
            {
                this.Progression.CurrentExperience = experience;
            }
        }

        internal void RestoreLevel(int level)
        {
            if (this.Progression != null)
            {
                this.Progression.CurrentLevel = level;
            }
        }

        internal void RestoreUnlockedTowers(List<string> list)
        {
            if (this.Progression != null)
            {
                // Reconstruct the HashSet container from the incoming list
                this.Progression.UnlockedTowers = new HashSet<string>(list);
            }
        }

    }
}
