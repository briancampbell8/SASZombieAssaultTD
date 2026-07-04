///File:    E:\BDC\Projects\SASZombieAssaultTD\Engine\Player\SaveLoadController_Core.cs
///Purpose: Player action validation and execution system for SAS Zombie Assault TD.
///Features: Tower placement validation, upgrade processing, damage handling, and game state management.
///Validation: Comprehensive action validation with game state checking and affordability validation.
///Performance: Optimized for frequent action processing with minimal overhead.
///Threading: Thread-safe operations with proper locking for concurrent access.
///Integration: Designed for use with PlayerSystem, TowerManager, and WaveManager.
///Persistence: Action logging for debugging and player feedback.
///****************************************************************************************************
//

using System;
using System.IO;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Player
{
    ///<summary>
    ///Orchestrates save/load operations for player data persistence.
    ///Manages file I/O, error handling, and thread safety.
    ///</summary>
    public partial class SaveLoadController
    {
        private readonly string _savePath;
        private readonly string _backupPath;
        private readonly object _lock = new();

        ///<summary>
        ///Gets the path to the main save file.
        ///</summary>
        public string SavePath => _savePath;

        ///<summary>
        ///Gets the path to the backup save file.
        ///</summary>
        public string BackupPath => _backupPath;

        ///<summary>
        ///Initializes a new instance of SaveLoadController with default paths.
        ///</summary>
        public SaveLoadController()
            : this(
                Path.Combine("Data", "player_save.json"),
                Path.Combine("Data", "player_save_backup.json"))
        {
        }

        ///<summary>
        ///Initializes a new instance of SaveLoadController with custom paths.
        ///</summary>
        ///<param name="savePath">Path to the main save file.</param>
        ///<param name="backupPath">Path to the backup save file.</param>
        public SaveLoadController(string savePath, string backupPath)
        {
            _savePath = savePath ?? throw new ArgumentNullException(nameof(savePath));
            _backupPath = backupPath ?? throw new ArgumentNullException(nameof(backupPath));
        }
    }
}
