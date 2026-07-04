using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using SASZombieAssaultTD.Engine.Save.SAS;
using SASZombieAssaultTD.Engine.Scenes.Battlefields;
using SASZombieAssaultTD.Engine.Towers.TowerControl;
using SASZombieAssaultTD.Engine.VectorMath;
namespace SASZombieAssaultTD.Engine.Save
//
{
    ///<summary>
    ///Save system for managing game save files.
    ///Implements SaveSystem with Write, Read, and Auto-Save functionality.
    ///</summary>
    public class SaveSystem
    {
        private readonly string _saveDirectory;
        private readonly string _fileExtension;
        private readonly Dictionary<string, SaveData> _saveSlots = new();
        private int _currentSaveSlot;
        private bool _autoSave;
        private float _autoSaveInterval = 300f; //Default to 5 minutes
        private float _lastAutoSaveTime;

        ///<summary>
        ///Gets the save directory path.
        ///</summary>
        public string SaveDirectory => _saveDirectory;

        ///<summary>
        ///Gets the number of save slots.
        ///</summary>
        public int SaveSlotCount => _saveSlots.Count;

        ///<summary>
        ///Gets or sets the current save slot.
        ///</summary>
        public int CurrentSaveSlot
        {
            get => _currentSaveSlot;
            set => _currentSaveSlot = System.Math.Clamp(value, 0, 9); //Support 0-9 slots
        }

        ///<summary>
        ///Gets or sets whether auto-save is enabled.
        ///</summary>
        public bool AutoSave
        {
            get => _autoSave;
            set => _autoSave = value;
        }

        ///<summary>
        ///Gets or sets the auto-save interval in seconds.
        ///</summary>
        public float AutoSaveInterval
        {
            get => _autoSaveInterval;
            set => _autoSaveInterval = System.Math.Max(10f, value); //Minimum 10 seconds
        }

        ///<summary>
        ///Event fired when a save is created.
        ///</summary>
        public event Action<int, SaveData> OnSaveCreated;

        ///<summary>
        ///Event fired when a save is loaded.
        ///</summary>
        public event Action<int, SaveData> OnSaveLoaded;

        ///<summary>
        ///Event fired when a save is deleted.
        ///</summary>
        public event Action<int> OnSaveDeleted;

        ///<summary>
        ///Event fired when an error occurs.
        ///</summary>
        public event Action<string, Exception> OnError;

        ///<summary>
        ///Initializes a new save system.
        ///</summary>
        ///<param name="saveDirectory">Directory to save files in.</param>
        ///<param name="fileExtension">File extension for save files.</param>
        public SaveSystem(string? saveDirectory = null, string fileExtension = ".sav")
        {
            _saveDirectory = saveDirectory ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SASZombieAssaultTD", "Saves");
            _fileExtension = fileExtension;

            InitializeSaveDirectory();
            LoadAllSaveSlots();

            Dlogger.Log("INFO", $"SaveSystem initialized with directory '{_saveDirectory}'");
        }

        ///<summary>
        ///Writes save data to a specific slot.
        ///P20-10-02: Implements Write functionality.
        ///</summary>
        ///<param name="slot">The save slot (0-9).</param>
        ///<param name="saveData">The save data to write.</param>
        ///<returns>True if save succeeded.</returns>
        public bool WriteSave(int slot, SaveData saveData)
        {
            if (!IsValidSlot(slot) || saveData == null)
            {
                LogWarning($"Invalid save slot {slot} or null save data");
                return false;
            }

            try
            {
                ValidateSaveData(saveData);

                var filePath = GetSaveFilePath(slot);
                File.WriteAllText(filePath, SerializeSaveData(saveData));

                _saveSlots[$"slot{slot}"] = saveData.Clone();
                LogInfo($"Saved to slot {slot} ({filePath})");
                OnSaveCreated?.Invoke(slot, saveData);

                return true;
            }
            catch (Exception ex)
            {
                HandleError($"Save to slot {slot}", ex);
                return false;
            }
        }

        ///<summary>
        ///Reads save data from a specific slot.
        ///P20-10-02: Implements Read functionality.
        ///</summary>
        ///<param name="slot">The save slot (0-9).</param>
        ///<returns>The save data, or null if read failed.</returns>
        public SaveData ReadSave(int slot)
        {
            if (!IsValidSlot(slot))
            {
                LogWarning($"Invalid save slot {slot}");
                return null;
            }

            try
            {
                var filePath = GetSaveFilePath(slot);
                if (!File.Exists(filePath))
                {
                    LogWarning($"Save file does not exist: {filePath}");
                    return null;
                }

                var saveData = DeserializeSaveData(File.ReadAllText(filePath));
                if (saveData == null || !IsVersionCompatible(saveData.Version))
                {
                    LogWarning($"Save version {saveData?.Version} is not compatible");
                    return null;
                }

                _saveSlots[$"slot{slot}"] = saveData;
                LogInfo($"Loaded from slot {slot} ({filePath})");
                OnSaveLoaded?.Invoke(slot, saveData);

                return saveData;
            }
            catch (Exception ex)
            {
                HandleError($"Load from slot {slot}", ex);
                return null;
            }
        }

        ///<summary>
        ///Deletes a save file from a specific slot.
        ///</summary>
        ///<param name="slot">The save slot (0-9).</param>
        ///<returns>True if deletion succeeded.</returns>
        public bool DeleteSave(int slot)
        {
            if (!IsValidSlot(slot))
            {
                LogWarning($"Invalid save slot {slot}");
                return false;
            }

            try
            {
                var filePath = GetSaveFilePath(slot);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                _saveSlots.Remove($"slot{slot}");
                LogInfo($"Deleted save slot {slot}");
                OnSaveDeleted?.Invoke(slot);

                return true;
            }
            catch (Exception ex)
            {
                HandleError($"Delete save slot {slot}", ex);
                return false;
            }
        }

        ///<summary>
        ///Gets save data for a specific slot.
        ///</summary>
        ///<param name="slot">The save slot (0-9).</param>
        ///<returns>The save data, or null if not found.</returns>
        public SaveData GetSaveData(int slot) => IsValidSlot(slot) && _saveSlots.TryGetValue($"slot{slot}", out var saveData) ? saveData : null;

        ///<summary>
        ///Checks if a save slot has data.
        ///</summary>
        ///<param name="slot">The save slot (0-9).</param>
        ///<returns>True if the slot has save data.</returns>
        public bool HasSaveData(int slot) => GetSaveData(slot) != null;

        ///<summary>
        ///Gets information about all save slots.
        ///</summary>
        ///<returns>Dictionary of slot information.</returns>
        public Dictionary<int, SaveSlotInfo> GetSaveSlotInfos()
        {
            return Enumerable.Range(0, 10).ToDictionary(
                slot => slot,
                slot =>
                {
                    var saveData = GetSaveData(slot);
                    return new SaveSlotInfo
                    {
                        Slot = slot,
                        HasData = saveData != null,
                        PlayerName = saveData?.PlayerName ?? "Empty",
                        Level = saveData?.CurrentLevel ?? 1,
                        HighScore = saveData?.HighScore ?? 0,
                        LastSaveTime = saveData?.LastSaveTime ?? DateTime.MinValue,
                        PlayTime = saveData?.TotalPlayTime ?? 0f
                    };
                });
        }

        ///<summary>
        ///Updates the save system (for auto-save functionality).
        ///</summary>
        ///<param name="deltaTime">Time elapsed since last update.</param>
        public void Update(float deltaTime)
        {
            if (!_autoSave) return;

            _lastAutoSaveTime += deltaTime;
            if (_lastAutoSaveTime >= _autoSaveInterval)
            {
                AutoSaveCurrentSlot();
                _lastAutoSaveTime = 0f;
            }
        }

        ///<summary>
        ///Performs auto-save for the current slot.
        ///</summary>
        public void AutoSaveCurrentSlot()
        {
            var currentSaveData = GetSaveData(_currentSaveSlot);
            if (currentSaveData != null)
            {
                WriteSave(_currentSaveSlot, currentSaveData);
                LogInfo($"Auto-saved to slot {_currentSaveSlot}");
            }
        }

        ///<summary>
        ///Exports save data to a specific file.
        ///</summary>
        ///<param name="slot">The save slot to export.</param>
        ///<param name="exportPath">The export file path.</param>
        ///<returns>True if export succeeded.</returns>
        public bool ExportSave(int slot, string exportPath)
        {
            var saveData = GetSaveData(slot);
            if (saveData == null)
            {
                LogWarning($"No data to export from slot {slot}");
                return false;
            }

            try
            {
                var json = JsonSerializer.Serialize(saveData, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                File.WriteAllText(exportPath, json);
                LogInfo($"Exported slot {slot} to {exportPath}");
                return true;
            }
            catch (Exception ex)
            {
                HandleError($"Export slot {slot}", ex);
                return false;
            }
        }

        ///<summary>
        ///Imports save data from a specific file.
        ///</summary>
        ///<param name="importPath">The import file path.</param>
        ///<param name="targetSlot">The target save slot.</param>
        ///<returns>True if import succeeded.</returns>
        public bool ImportSave(string importPath, int targetSlot)
        {
            try
            {
                var json = File.ReadAllText(importPath);
                var saveData = JsonSerializer.Deserialize<SaveData>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                if (saveData != null)
                {
                    return WriteSave(targetSlot, saveData);
                }

                return false;
            }
            catch (Exception ex)
            {
                HandleError($"Import from {importPath}", ex);
                return false;
            }
        }

        ///<summary>
        ///Initializes the save directory.
        ///</summary>
        private void InitializeSaveDirectory()
        {
            try
            {
                if (!Directory.Exists(_saveDirectory))
                {
                    Directory.CreateDirectory(_saveDirectory);
                    LogInfo($"Created save directory '{_saveDirectory}'");
                }
            }
            catch (Exception ex)
            {
                HandleError("Create save directory", ex);
            }
        }

        ///<summary>
        ///Loads all save slots.
        ///</summary>
        private void LoadAllSaveSlots()
        {
            _saveSlots.Clear();
            for (int slot = 0; slot <= 9; slot++)
            {
                var saveData = ReadSave(slot);
                if (saveData != null)
                {
                    _saveSlots[$"slot{slot}"] = saveData;
                }
            }
            LogInfo($"Loaded {_saveSlots.Count} save slots");
        }

        ///<summary>
        ///Checks if a save version is compatible.
        ///P20-10-08: Add versioning for save files.
        ///</summary>
        ///<param name="version">The save version.</param>
        ///<returns>True if compatible.</returns>
        private bool IsVersionCompatible(int version) => version <= 1;

        ///<summary>
        ///Gets save system statistics.
        ///</summary>
        ///<returns>Save system information as a string.</returns>
        public override string ToString()
        {
            return $"SaveSystem: Directory='{_saveDirectory}', Slots={_saveSlots.Count}, " +
            $"Current={_currentSaveSlot}, AutoSave={_autoSave}, Interval={_autoSaveInterval}s";
        }

        private static void CaptureTowerState(SAS.SASGameSave save)
        {
            //Adapted to use TowerUpgradeManager for tower state management
            var towerUpgradeManager = new TowerUpgradeManager();

            save.Towers = new TowerSaveData
            {
                //GetUpgrades returns an int?, so we just grab the value directly
                TowerCount = towerUpgradeManager.GetUpgrades("default") ?? 0,
                TotalValue = 0, //Fallback placeholder since there is no collection to sum
                TowerTypes = new List<string>(), //Fallback placeholder since there is no collection to select from
                TowerPositions = new Dictionary<string, Vector3>(),
                TowerLevels = new Dictionary<string, int>()
            };

        }

        private bool IsValidSlot(int slot) => slot >= 0 && slot <= 9;

        private string GetSaveFilePath(int slot) => Path.Combine(_saveDirectory, $"save{slot}{_fileExtension}");

        private string SerializeSaveData(SaveData saveData) => JsonSerializer.Serialize(saveData, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        private SaveData DeserializeSaveData(string json) => JsonSerializer.Deserialize<SaveData>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        private void ValidateSaveData(SaveData saveData)
        {
            var validationIssues = saveData.Validate();
            if (validationIssues.Count > 0)
            {
                LogWarning($"Save data validation failed: {string.Join(", ", validationIssues)}");
            }
        }

        private void LogInfo(string message) => Dlogger.Log("INFO", $"SaveSystem: {message}");

        private void LogWarning(string message) => Dlogger.Log("WARNING", $"SaveSystem: {message}");

        private void HandleError(string context, Exception ex)
        {
            Dlogger.Log("ERROR", $"SaveSystem: {context} - {ex.Message}");
            OnError?.Invoke(context, ex);
        }
    }

    ///<summary>
    ///Information about a save slot.
    ///</summary>
    public class SaveSlotInfo
    {
        public int Slot { get; set; }
        public bool HasData { get; set; }
        public string PlayerName { get; set; }
        public int Level { get; set; }
        public int HighScore { get; set; }
        public DateTime LastSaveTime { get; set; }
        public float PlayTime { get; set; }

        public override string ToString()
        {
            return $"Slot {Slot}: {(HasData ? $"{PlayerName} (L{Level}, {HighScore}pts)" : "Empty")}";
        }
    }
}







