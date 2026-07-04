/*
Program Name: SASZombieAssaultTD
File Path: Engine\Save\SaveManager.cs
Purpose: Unified save manager consolidating SaveSystem and SASGameSaveManager functionality.
Features: Slot-based and named saves, auto-save, validation, migration, P120/P100 integration.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
//
//
using SASZombieAssaultTD.Engine.Diagnostics;
namespace SASZombieAssaultTD.Engine.Save
{
    ///<summary>
    ///Unified save manager for SAS Zombie Assault TD.
    ///P140-03: Consolidates SaveSystem and SASGameSaveManager into single unified system.
    ///Supports both slot-based (0-9) and named saves with full P120/P100 integration.
    ///</summary>
    public class SaveManager
    {
        private readonly string _saveDirectory;
        private readonly string _fileExtension;
        private readonly Dictionary<string, SaveData> _namedSaves;
        private readonly Dictionary<int, SaveData> _slotSaves;
        private int _currentSaveSlot;
        private bool _autoSave;
        private float _autoSaveInterval;
        private float _lastAutoSaveTime;
        private static SaveManager _instance;
        private int _currentSaveVersion;

        ///<summary>
        ///Gets the save directory path.
        ///</summary>
        public string SaveDirectory => _saveDirectory;

        ///<summary>
        ///Gets the number of named saves.
        ///</summary>
        public int NamedSaveCount => _namedSaves.Count;

        ///<summary>
        ///Gets the number of slot saves with data.
        ///</summary>
        public int SlotSaveCount => _slotSaves.Count(kvp => kvp.Value != null);

        ///<summary>
        ///Gets or sets the current save slot (0-9).
        ///</summary>
        public int CurrentSaveSlot
        {
            get => _currentSaveSlot;
            set => _currentSaveSlot = System.Math.Clamp(value, 0, 9);
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
            set => _autoSaveInterval = System.Math.Max(10f, value);
        }

        ///<summary>
        ///Gets the current save version.
        ///</summary>
        public int CurrentSaveVersion => _currentSaveVersion;

        ///<summary>
        ///Event fired when a save is created.
        ///</summary>
        public event Action<string, SaveData> OnNamedSaveCreated;

        ///<summary>
        ///Event fired when a slot save is created.
        ///</summary>
        public event Action<int, SaveData> OnSlotSaveCreated;

        ///<summary>
        ///Event fired when a save is loaded.
        ///</summary>
        public event Action<SaveData> OnSaveLoaded;

        ///<summary>
        ///Event fired when a save is deleted.
        ///</summary>
        public event Action OnSaveDeleted;

        ///<summary>
        ///Event fired when an error occurs.
        ///</summary>
        public event Action<string, Exception> OnError;

        ///<summary>
        ///Singleton instance for global access.
        ///</summary>
        public static SaveManager Instance => _instance ??= new SaveManager();

        ///<summary>
        ///Initializes a new save manager.
        ///</summary>
        ///<param name="saveDirectory">Directory to save files in.</param>
        ///<param name="fileExtension">File extension for save files.</param>
        private SaveManager(string? saveDirectory = null, string fileExtension = ".json")
        {
            _saveDirectory = saveDirectory ?? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "SASZombieAssaultTD",
                "Saves");
            _fileExtension = fileExtension;
            _namedSaves = new Dictionary<string, SaveData>();
            _slotSaves = new Dictionary<int, SaveData>();
            _currentSaveSlot = 0;
            _autoSave = false;
            _autoSaveInterval = 300f; //5 minutes
            _lastAutoSaveTime = 0f;
            _currentSaveVersion = 2; //P140: Version 2 with P120/P100 integration

            InitializeSaveDirectory();
            LoadAllSaves();

            DLogger.Log(LogSubsystems.Unknown, LogLevel.Info, "INFO",
                $"SaveManager initialized with directory '{_saveDirectory}'");
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
                    DLogger.Log(LogSubsystems.Unknown, LogLevel.Info, "INFO", $"Created save directory '{_saveDirectory}'");
                }
            }
            catch (Exception ex)
            {
                HandleError("Create save directory", ex);
                throw;
            }
        }

        ///<summary>
        ///Loads all saves from disk.
        ///</summary>
        private void LoadAllSaves()
        {
            _namedSaves.Clear();
            _slotSaves.Clear();

            //Load named saves
            var namedFiles = Directory.GetFiles(_saveDirectory, $"*{_fileExtension}")
                .Where(f => !Path.GetFileNameWithoutExtension(f).StartsWith("slot"));

            foreach (var filePath in namedFiles)
            {
                try
                {
                    var saveName = Path.GetFileNameWithoutExtension(filePath);
                    var json = File.ReadAllText(filePath);
                    var saveData = DeserializeSaveData(json);

                    if (saveData != null && IsVersionCompatible(saveData.Version))
                    {
                        _namedSaves[saveName] = saveData;
                    }
                }
                catch (Exception ex)
                {
                    DLogger.Log(LogSubsystems.Unknown, LogLevel.Warning, "Warning", $"Failed to load named save from {filePath}: {ex.Message}");
                }
            }

            //Load slot saves (0-9)
            for (int slot = 0; slot <= 9; slot++)
            {
                try
                {
                    var filePath = GetSlotSaveFilePath(slot);
                    if (File.Exists(filePath))
                    {
                        var json = File.ReadAllText(filePath);
                        var saveData = DeserializeSaveData(json);

                        if (saveData != null && IsVersionCompatible(saveData.Version))
                        {
                            _slotSaves[slot] = saveData;
                        }
                    }
                }
                catch (Exception ex)
                {
                    DLogger.Log(LogSubsystems.Unknown, LogLevel.Warning, "Warning", $"Failed to load slot {slot}: {ex.Message}");
                }
            }

            DLogger.Log(LogSubsystems.Unknown, LogLevel.Info, "INFO", $"Loaded {_namedSaves.Count} named saves and {_slotSaves.Count} slot saves");
        }

        ///<summary>
        ///Creates a new named save.
        ///</summary>
        ///<param name="saveName">Name for the save.</param>
        ///<param name="saveData">The save data to save.</param>
        ///<returns>True if save was created successfully.</returns>
        public bool CreateNamedSave(string saveName, SaveData saveData)
        {
            if (string.IsNullOrEmpty(saveName) || saveData == null)
            {
                DLogger.Log(LogSubsystems.Unknown, LogLevel.Warning, "Warning", "Invalid save name or null save data");
                return false;
            }

            if (_namedSaves.ContainsKey(saveName))
            {
                DLogger.Log(LogSubsystems.Unknown, LogLevel.Warning, "Warning", $"Save '{saveName}' already exists");
                return false;
            }

            try
            {
                ValidateSaveData(saveData);
                //Note: Version property is read-only in base SaveData
                //Version tracking handled by SaveManager

                var filePath = GetNamedSaveFilePath(saveName);
                File.WriteAllText(filePath, SerializeSaveData(saveData));

                _namedSaves[saveName] = saveData;
                OnNamedSaveCreated?.Invoke(saveName, saveData);

                DLogger.Log(LogSubsystems.Unknown, LogLevel.Info, "INFO", $"Created named save '{saveName}'");
                return true;
            }
            catch (Exception ex)
            {
                HandleError($"Create named save '{saveName}'", ex);
                return false;
            }
        }

        ///<summary>
        ///Saves data to a specific slot.
        ///</summary>
        ///<param name="slot">The save slot (0-9).</param>
        ///<param name="saveData">The save data to save.</param>
        ///<returns>True if save was successful.</returns>
        public bool SaveToSlot(int slot, SaveData saveData)
        {
            if (!IsValidSlot(slot) || saveData == null)
            {
                DLogger.Log(LogSubsystems.Unknown, LogLevel.Warning, "Warning", $"Invalid slot {slot} or null save data");
                return false;
            }

            try
            {
                ValidateSaveData(saveData);
                //Note: Version property is read-only in base SaveData
                //Version tracking handled by SaveManager

                var filePath = GetSlotSaveFilePath(slot);
                File.WriteAllText(filePath, SerializeSaveData(saveData));

                _slotSaves[slot] = saveData;
                OnSlotSaveCreated?.Invoke(slot, saveData);

                DLogger.Log(LogSubsystems.Unknown, LogLevel.Info, "INFO", $"Saved to slot {slot}");
                return true;
            }
            catch (Exception ex)
            {
                HandleError($"Save to slot {slot}", ex);
                return false;
            }
        }

        ///<summary>
        ///Loads a named save.
        ///</summary>
        ///<param name="saveName">Name of save to load.</param>
        ///<returns>The save data, or null if not found.</returns>
        public SaveData LoadNamedSave(string saveName)
        {
            if (string.IsNullOrEmpty(saveName))
            {
                DLogger.Log(LogSubsystems.Unknown, LogLevel.Warning, "Warning", "Invalid save name");
                return null;
            }

            if (_namedSaves.TryGetValue(saveName, out var saveData))
            {
                OnSaveLoaded?.Invoke(saveData);
                DLogger.Log(LogSubsystems.Unknown, LogLevel.Info, "INFO", $"Loaded named save '{saveName}'");
                return saveData;
            }

            DLogger.Log(LogSubsystems.Unknown, LogLevel.Warning, "Warning", $"Named save '{saveName}' not found");
            return null;
        }

        ///<summary>
        ///Loads a slot save.
        ///</summary>
        ///<param name="slot">The save slot (0-9).</param>
        ///<returns>The save data, or null if not found.</returns>
        public SaveData LoadSlotSave(int slot)
        {
            if (!IsValidSlot(slot))
            {
                DLogger.Log(LogSubsystems.Unknown, LogLevel.Warning, "Warning", $"Invalid slot {slot}");
                return null;
            }

            if (_slotSaves.TryGetValue(slot, out var saveData))
            {
                OnSaveLoaded?.Invoke(saveData);
                DLogger.Log(LogSubsystems.Unknown, LogLevel.Info, "INFO", $"Loaded slot {slot}");
                return saveData;
            }

            DLogger.Log(LogSubsystems.Unknown, LogLevel.Warning, "Warning", $"Slot {slot} has no data");
            return null;
        }

        ///<summary>
        ///Deletes a named save.
        ///</summary>
        ///<param name="saveName">Name of save to delete.</param>
        ///<returns>True if deletion succeeded.</returns>
        public bool DeleteNamedSave(string saveName)
        {
            if (string.IsNullOrEmpty(saveName) || !_namedSaves.ContainsKey(saveName))
            {
                return false;
            }

            try
            {
                var filePath = GetNamedSaveFilePath(saveName);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                _namedSaves.Remove(saveName);
                OnSaveDeleted?.Invoke();

                DLogger.Log(LogSubsystems.Unknown, LogLevel.Info, "INFO", $"Deleted named save '{saveName}'");
                return true;
            }
            catch (Exception ex)
            {
                HandleError($"Delete named save '{saveName}'", ex);
                return false;
            }
        }

        ///<summary>
        ///Deletes a slot save.
        ///</summary>
        ///<param name="slot">The save slot (0-9).</param>
        ///<returns>True if deletion succeeded.</returns>
        public bool DeleteSlotSave(int slot)
        {
            if (!IsValidSlot(slot))
            {
                return false;
            }

            try
            {
                var filePath = GetSlotSaveFilePath(slot);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                _slotSaves.Remove(slot);
                OnSaveDeleted?.Invoke();

                DLogger.Log(LogSubsystems.Unknown, LogLevel.Info, "INFO", $"Deleted slot {slot}");
                return true;
            }
            catch (Exception ex)
            {
                HandleError($"Delete slot {slot}", ex);
                return false;
            }
        }

        ///<summary>
        ///Gets save data for a named save.
        ///</summary>
        ///<param name="saveName">Name of save.</param>
        ///<returns>The save data, or null if not found.</returns>
        public SaveData GetNamedSaveData(string saveName)
        {
            return _namedSaves.TryGetValue(saveName, out var saveData) ? saveData : null;
        }

        ///<summary>
        ///Gets save data for a slot.
        ///</summary>
        ///<param name="slot">The save slot (0-9).</param>
        ///<returns>The save data, or null if not found.</returns>
        public SaveData GetSlotSaveData(int slot)
        {
            return IsValidSlot(slot) && _slotSaves.TryGetValue(slot, out var saveData) ? saveData : null;
        }

        ///<summary>
        ///Checks if a named save exists.
        ///</summary>
        ///<param name="saveName">Name of save.</param>
        ///<returns>True if the save exists.</returns>
        public bool NamedSaveExists(string saveName)
        {
            return _namedSaves.ContainsKey(saveName);
        }

        ///<summary>
        ///Checks if a slot has data.
        ///</summary>
        ///<param name="slot">The save slot (0-9).</param>
        ///<returns>True if the slot has data.</returns>
        public bool SlotHasData(int slot)
        {
            return IsValidSlot(slot) && _slotSaves.ContainsKey(slot) && _slotSaves[slot] != null;
        }

        ///<summary>
        ///Gets all named save names.
        ///</summary>
        ///<returns>List of named save names.</returns>
        public List<string> GetAllNamedSaveNames()
        {
            return _namedSaves.Keys.ToList();
        }

        ///<summary>
        ///Gets information about all slot saves.
        ///</summary>
        ///<returns>Dictionary of slot information.</returns>
        public Dictionary<int, SaveSlotInfo> GetSlotSaveInfos()
        {
            var infos = new Dictionary<int, SaveSlotInfo>();
            for (int slot = 0; slot <= 9; slot++)
            {
                var saveData = GetSlotSaveData(slot);
                infos[slot] = new SaveSlotInfo
                {
                    Slot = slot,
                    HasData = saveData != null,
                    PlayerName = saveData?.PlayerName ?? "Empty",
                    Level = saveData?.CurrentLevel ?? 1,
                    HighScore = saveData?.HighScore ?? 0,
                    LastSaveTime = saveData?.LastSaveTime ?? DateTime.MinValue,
                    PlayTime = saveData?.TotalPlayTime ?? 0f
                };
            }
            return infos;
        }

        ///<summary>
        ///Updates the save manager (for auto-save functionality).
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
            var currentSaveData = GetSlotSaveData(_currentSaveSlot);
            if (currentSaveData != null)
            {
                SaveToSlot(_currentSaveSlot, currentSaveData);
                DLogger.Log(LogSubsystems.Unknown,
                    LogLevel.Info,
                    "INFO", $"Auto-saved to slot {_currentSaveSlot}");
            }
        }

        ///<summary>
        ///Checks if a save version is compatible.
        ///</summary>
        ///<param name="version">The save version.</param>
        ///<returns>True if compatible.</returns>
        private bool IsVersionCompatible(int version)
        {
            //P140: Support versions 1 and 2
            //Version 1: Original format
            //Version 2: P120/P100 integration
            return version >= 1 && version <= _currentSaveVersion;
        }

        ///<summary>
        ///Validates save data.
        ///</summary>
        ///<param name="saveData">The save data to validate.</param>
        private void ValidateSaveData(SaveData saveData)
        {
            var validationIssues = saveData.Validate();
            if (validationIssues.Count > 0)
            {
                DLogger.Log(LogSubsystems.Unknown, LogLevel.Warning, "Warning", $"Save data validation failed: {string.Join(", ", validationIssues)}");
            }
        }

        ///<summary>
        ///Serializes save data to JSON.
        ///</summary>
        private string SerializeSaveData(SaveData saveData)
        {
            return JsonSerializer.Serialize(saveData, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }

        ///<summary>
        ///Deserializes save data from JSON.
        ///</summary>
        private SaveData DeserializeSaveData(string json)
        {
            return JsonSerializer.Deserialize<SaveData>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }

        ///<summary>
        ///Gets file path for a named save.
        ///</summary>
        private string GetNamedSaveFilePath(string saveName)
        {
            return Path.Combine(_saveDirectory, $"{SanitizeFileName(saveName)}{_fileExtension}");
        }

        ///<summary>
        ///Gets file path for a slot save.
        ///</summary>
        private string GetSlotSaveFilePath(int slot)
        {
            return Path.Combine(_saveDirectory, $"slot{slot}{_fileExtension}");
        }

        ///<summary>
        ///Sanitizes file name for safe file system usage.
        ///</summary>
        private string SanitizeFileName(string fileName)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            var sanitized = fileName;

            foreach (var c in invalidChars)
            {
                sanitized = sanitized.Replace(c, '_');
            }

            return sanitized;
        }

        ///<summary>
        ///Checks if a slot is valid.
        ///</summary>
        private bool IsValidSlot(int slot) => slot >= 0 && slot <= 9;

        ///<summary>
        ///Handles errors.
        ///</summary>
        private void HandleError(string context, Exception ex)
        {
            DLogger.Log(LogSubsystems.Unknown, LogLevel.Info, "ERROR",
                $"SaveManager: {context} - {ex.Message}");
            OnError?.Invoke(context, ex);
        }

        ///<summary>
        ///Gets save manager information as a string.
        ///</summary>
        public override string ToString()
        {
            return $"SaveManager: Directory='{_saveDirectory}', NamedSaves={_namedSaves.Count}, " +
                   $"SlotSaves={_slotSaves.Count}, CurrentSlot={_currentSaveSlot}, " +
                   $"AutoSave={_autoSave}, Version={_currentSaveVersion}";
        }
    }

    internal class LogLevels
    {
        internal static string Info;
        internal static string Warning;
        internal static string Error;
    }
}
