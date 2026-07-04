//
using System;
//
using System.Collections.Generic;
//
using System.IO;
///File:    E:\BDC\Projects\SASZombieAssaultTD\Engine\Player\SaveLoadController_Load.cs
///Purpose: Player action validation and execution system for SAS Zombie Assault TD.
///Features: Tower placement validation, upgrade processing, damage handling, and game state management.
///Validation: Comprehensive action validation with game state checking and affordability validation.
///Performance: Optimized for frequent action processing with minimal overhead.
///Threading: Thread-safe operations with proper locking for concurrent access.
///Integration: Designed for use with PlayerSystem, TowerManager, and WaveManager.
///Persistence: Action logging for debugging and player feedback.
///****************************************************************************************************
//
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Snapshot;
namespace SASZombieAssaultTD.Engine.Player
{
    public partial class SaveLoadController
    {
        public PlayerData playerData;
        private object Lives;
        private object Cash;

        public PlayerProgression Progression { get; set; }
        public int CurrentExperience { get; private set; }

        //public object Progression { get; private set; }

        ///<summary>
        ///Loads player game state from file.
        ///</summary>
        ///<param name="playerSystem">The PlayerSystem instance to restore with loaded data.</param>
        ///<returns>True if the load was successful, false otherwise.</returns>
        //public void RestoreExperience(int experience) not needed since we can directly
        //set CurrentExperience from the loaded data in RestorePlayerSystemFromData method
        //{
        //   if (Progression != null)
        //   {
        //       CurrentExperience = experience;
        //   }
        //}

        public bool LoadGame(PlayerSystem playerSystem)
        {
            if (playerSystem == null)
                throw new ArgumentNullException(nameof(playerSystem));

            lock (_lock)
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine("Info", "SaveLoadController: Starting load operation");

                    //Check if save file exists
                    if (!File.Exists(_savePath))
                    {
                        System.Diagnostics.Debug.WriteLine("Info", "SaveLoadController: No save file found");
                        return false;
                    }

                    //Read save file
                    string json = ReadSaveFile();

                    if (string.IsNullOrEmpty(json))
                    {
                        System.Diagnostics.Debug.WriteLine("Error", "SaveLoadController: Save file is empty or unreadable");
                        return AttemptBackupLoad(playerSystem);
                    }

                    //Deserialize player data using SaveLoadCore
                    var playerData = SaveLoadCore.Deserialize(json);

                    if (playerData == null)
                    {
                        System.Diagnostics.Debug.WriteLine("Error", "SaveLoadController: Failed to deserialize player data");
                        return AttemptBackupLoad(playerSystem);
                    }

                    //Validate loaded data using SaveLoadCore
                    if (!SaveLoadCore.ValidatePlayerData(playerData))
                    {
                        System.Diagnostics.Debug.WriteLine("Error", "SaveLoadController: Loaded player data validation failed");
                        return AttemptBackupLoad(playerSystem);
                    }

                    //Restore player system state
                    if (!RestorePlayerSystemFromData(playerSystem, playerData))
                    {
                        System.Diagnostics.Debug.WriteLine("Error", "SaveLoadController: Failed to restore player system");
                        return false;
                    }

                    System.Diagnostics.Debug.WriteLine(
                        "Info",
                        $"SaveLoadController: Load completed successfully - Level: {playerData.Progression.GetType}, " +
                        $"Lives: {playerData.State.GetType}, Cash: ${playerData.State.GetType}");

                    return true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error", $"SaveLoadController: Load failed - Error: {ex.Message}");

                    //Attempt to load backup on failure
                    return AttemptBackupLoad(playerSystem);
                }
            }
        }

        ///<summary>
        ///Attempts to load from the backup save file.
        ///</summary>
        private bool AttemptBackupLoad(PlayerSystem playerSystem)
        {
            try
            {
                if (!File.Exists(_backupPath))
                {
                    System.Diagnostics.Debug.WriteLine("Info", "SaveLoadController: No backup file available");
                    return false;
                }

                System.Diagnostics.Debug.WriteLine("Info", "SaveLoadController: Attempting to load from backup");

                //Load backup data
                string backupJson = File.ReadAllText(_backupPath);
                var backupData = SaveLoadCore.Deserialize(backupJson);

                if (backupData != null && SaveLoadCore.ValidatePlayerData(backupData))
                {
                    //Restore from backup and create new backup
                    if (RestorePlayerSystemFromData(playerSystem, backupData))
                    {
                        //Save backup as new main save
                        var newPlayerData = CreatePlayerDataFromSystem(playerSystem);
                        string json = SaveLoadCore.Serialize(newPlayerData);
                        WriteSaveFile(json);

                        System.Diagnostics.Debug.WriteLine("Info", "SaveLoadController: Backup load successful, new save created");
                        return true;
                    }
                }

                System.Diagnostics.Debug.WriteLine("Error", "SaveLoadController: Backup load failed - invalid data");
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error", $"SaveLoadController: Backup load failed: {ex.Message}");
                return false;
            }
        }

        ///<summary>
        ///Restores player system state from loaded data.
        ///</summary>

        private bool RestorePlayerSystemFromData(PlayerSystem playerSystem, PlayerData PlayerStateData)
        {
            try
            {
                //Restore player state using dedicated restore methods
                PlayerStateData.Progression = new PlayerProgression(); //Placeholder for actual progression data
                PlayerStateData.State = new PlayerState(); //Placeholder for actual state data
                playerSystem.Cash = PlayerStateData.Cash;
                playerSystem.Score = PlayerStateData.Score;
                playerSystem.Lives = PlayerStateData.Lives;
                playerSystem.WaveNumber = (int)PlayerStateData.WaveNumber;
                playerSystem.IsGameOver = PlayerStateData.IsGameOver;
                playerSystem.IsPaused = PlayerStateData.IsPaused;


                //playerSystem.RestoreScore(PlayerStateData.State.Score);
                playerSystem.RestoreExperience(PlayerStateData.Progression);
                //playerSystem.RestoreLevel(PlayerStateData.Progression.CurrentLevel);
                //playerSystem.RestoreUnlockedTowers(new List<string>(PlayerStateData.Progression.UnlockedTowers));
                ////Restore additional state properties
                //playerSystem.State.Lives = PlayerStateData.State.Lives;
                //playerSystem.State.MaxLives = PlayerStateData.State.MaxLives;
                //playerSystem.State.WaveNumber = PlayerStateData.State.WaveNumber;
                //playerSystem.State.IsGameOver = PlayerStateData.State.IsGameOver;
                //playerSystem.State.IsPaused = PlayerStateData.State.IsPaused;

                //Validate the restored state
                playerSystem.Validate();

                System.Diagnostics.Debug.WriteLine(
                    "Info",
                    category: $"SaveLoadController: Player system restored - Level: {Progression}, Lives: {Lives}, Cash: ${Cash}");

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error", $"SaveLoadController: Player system restoration failed: {ex.Message}");
                return false;
            }
        }
    }

    //internal class PlayerData Already defined in SaveLoadCore.cs, so we can use it directly here without redefining it.
    //{
    //   internal object Progression;
    //   internal object State;
    //   internal object Cash;
    //   internal object Score;
    //   internal object Lives;
    //   internal object WaveNumber;
    //   internal bool IsGameOver;
    //   internal bool IsPaused;
    //}

    public class PlayerProgression
    {
        internal int CurrentExperience;
        internal int CurrentLevel;
        internal HashSet<string> UnlockedTowers;
    }
    public class PlayerData
    {
        internal object Cash;
        internal object Score;
        internal object Lives;
        internal object WaveNumber;
        internal bool IsGameOver;
        internal bool IsPaused;

        public PlayerState State { get; set; }
        public PlayerProgression Progression { get; set; }

        internal void Validate()
        {
            NI.Hit();
        }
    }

    //public class PlayerState already defined in SaveLoadCore.cs, so we can use it directly here without redefining it.
    //{
    //   public int Cash { get; set; }
    //   public int Score { get; set; }
    //   public int Lives { get; set; }
    //   public int WaveNumber { get; set; }
    //   public bool IsGameOver { get; set; }
    //   public bool IsPaused { get; set; }

    //   public void Validate()
    //   {
    //       //Add your validation rules here if needed
    //   }
    //}

    //public class PlayerProgression already defined in SaveLoadCore.cs, so we can use it directly here without redefining it.
    //{
    //   public int CurrentExperience { get; set; }
    //   public int CurrentLevel { get; set; }
    //   public HashSet<string> UnlockedTowers { get; set; } = new();
    //}

}

