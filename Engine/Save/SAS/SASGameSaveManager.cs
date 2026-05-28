using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SASZombieAssaultTD.Engine.Save.SAS
{
    /// <summary>
    /// SAS TD game save manager.
    /// Handles save file operations, management, and persistence.
    /// </summary>
    public class SASGameSaveManager
    {
        private readonly string _saveDirectory;
        private readonly Dictionary<string, SASGameSave> _saveCache;
        private readonly Dictionary<string, string> _saveFilePaths;
        private bool _isInitialized;
        private static SASGameSaveManager _instance;

        // Events
        public event Action<string> OnSaveCreated;
        public event Action<string> OnSaveLoaded;
        public event Action<string> OnSaveDeleted;
        public event Action<string> OnSaveCorrupted;

        // Properties
        public bool IsInitialized => _isInitialized;
        public int SaveCount => _saveCache.Count;
        public string SaveDirectory => _saveDirectory;
        public IReadOnlyDictionary<string, SASGameSave> SaveCache => _saveCache;
        public IReadOnlyDictionary<string, string> SaveFilePaths => _saveFilePaths;

        // Singleton
        public static SASGameSaveManager Instance => _instance ??= new SASGameSaveManager();

        private SASGameSaveManager()
        {
            _saveDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SASZombieAssaultTD", "Saves");
            _saveCache = new Dictionary<string, SASGameSave>();
            _saveFilePaths = new Dictionary<string, string>();
        }

        /// <summary>
        /// Initialize the save manager.
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized) return;

            System.Diagnostics.Debug.WriteLine("Initializing SAS Game Save Manager");

            try
            {
                // Create save directory
                Directory.CreateDirectory(_saveDirectory);

                // Load existing saves
                LoadAllSaves();

                _isInitialized = true;
                System.Diagnostics.Debug.WriteLine($"SAS Game Save Manager initialized with {_saveCache.Count} saves");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to initialize SAS Game Save Manager: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Create a new save.
        /// </summary>
        /// <param name="saveName">Name for the save.</param>
        /// <param name="description">Optional description.</param>
        /// <returns>True if save was created successfully.</returns>
        public bool CreateSave(string saveName, string description = null)
        {
            if (string.IsNullOrEmpty(saveName))
            {
                System.Diagnostics.Debug.WriteLine("Save name cannot be empty");
                return false;
            }

            // Check if save already exists
            if (_saveCache.ContainsKey(saveName))
            {
                System.Diagnostics.Debug.WriteLine($"Save '{saveName}' already exists");
                return false;
            }

            try
            {
                // Create save from current state
                var save = SASGameSave.CreateFromCurrentState(saveName);

                if (description != null)
                {
                    save.Metadata.Description = description;
                }

                // Save to file
                var filePath = GetSaveFilePath(saveName);
                var json = save.SerializeToJson();

                File.WriteAllText(filePath, json);

                // Add to cache
                _saveCache[saveName] = save;
                _saveFilePaths[saveName] = filePath;

                // Trigger event
                OnSaveCreated?.Invoke(saveName);

                System.Diagnostics.Debug.WriteLine($"Created save: {saveName}");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating save '{saveName}': {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Load a save by name.
        /// </summary>
        /// <param name="saveName">Name of save to load.</param>
        /// <returns>Loaded save data, or null if failed.</returns>
        public SASGameSave LoadSave(string saveName)
        {
            if (string.IsNullOrEmpty(saveName))
            {
                System.Diagnostics.Debug.WriteLine("Save name cannot be empty");
                return null;
            }

            // Check if save exists in cache
            if (_saveCache.TryGetValue(saveName, out var cachedSave))
            {
                return cachedSave;
            }

            // Try to load from file
            var filePath = GetSaveFilePath(saveName);
            if (!File.Exists(filePath))
            {
                System.Diagnostics.Debug.WriteLine($"Save file not found: {filePath}");
                return null;
            }

            try
            {
                var json = File.ReadAllText(filePath);
                var save = SASGameSave.DeserializeFromJson(json);

                if (save != null)
                {
                    // Add to cache
                    _saveCache[saveName] = save;
                    _saveFilePaths[saveName] = filePath;

                    // Trigger event
                    OnSaveLoaded?.Invoke(saveName);

                    System.Diagnostics.Debug.WriteLine($"Loaded save: {saveName}");
                    return save;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to deserialize save: {saveName}");
                    OnSaveCorrupted?.Invoke(saveName);
                    return null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading save '{saveName}': {ex.Message}");
                OnSaveCorrupted?.Invoke(saveName);
                return null;
            }
        }

        /// <summary>
        /// Save current game state to existing save.
        /// </summary>
        /// <param name="saveName">Name of save to overwrite.</param>
        /// <returns>True if save was updated successfully.</returns>
        public bool UpdateSave(string saveName)
        {
            if (string.IsNullOrEmpty(saveName))
            {
                System.Diagnostics.Debug.WriteLine("Save name cannot be empty");
                return false;
            }

            if (!_saveCache.ContainsKey(saveName))
            {
                System.Diagnostics.Debug.WriteLine($"Save '{saveName}' does not exist");
                return false;
            }

            try
            {
                // Create new save from current state
                var save = SASGameSave.CreateFromCurrentState(saveName);

                // Preserve existing metadata
                var existingSave = _saveCache[saveName];
                save.Metadata.Creator = existingSave.Metadata.Creator;
                save.Metadata.Tags = existingSave.Metadata.Tags;
                save.Metadata.CustomData = existingSave.Metadata.CustomData;

                // Save to file
                var filePath = GetSaveFilePath(saveName);
                var json = save.SerializeToJson();

                File.WriteAllText(filePath, json);

                // Update cache
                _saveCache[saveName] = save;

                System.Diagnostics.Debug.WriteLine($"Updated save: {saveName}");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating save '{saveName}': {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Delete a save.
        /// </summary>
        /// <param name="saveName">Name of save to delete.</param>
        /// <returns>True if save was deleted successfully.</returns>
        public bool DeleteSave(string saveName)
        {
            if (string.IsNullOrEmpty(saveName))
            {
                System.Diagnostics.Debug.WriteLine("Save name cannot be empty");
                return false;
            }

            if (!_saveCache.ContainsKey(saveName))
            {
                System.Diagnostics.Debug.WriteLine($"Save '{saveName}' does not exist");
                return false;
            }

            try
            {
                // Delete file
                var filePath = GetSaveFilePath(saveName);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                // Remove from cache
                _saveCache.Remove(saveName);
                _saveFilePaths.Remove(saveName);

                // Trigger event
                OnSaveDeleted?.Invoke(saveName);

                System.Diagnostics.Debug.WriteLine($"Deleted save: {saveName}");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting save '{saveName}': {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Create a backup of a save.
        /// </summary>
        /// <param name="saveName">Name of save to backup.</param>
        /// <param name="backupName">Name for the backup (optional).</param>
        /// <returns>True if backup was created successfully.</returns>
        public bool CreateBackup(string saveName, string backupName = null)
        {
            if (string.IsNullOrEmpty(saveName))
            {
                System.Diagnostics.Debug.WriteLine("Save name cannot be empty");
                return false;
            }

            if (!_saveCache.ContainsKey(saveName))
            {
                System.Diagnostics.Debug.WriteLine($"Save '{saveName}' does not exist");
                return false;
            }

            try
            {
                var originalSave = _saveCache[saveName];
                var backup = originalSave.CreateBackup();

                if (string.IsNullOrEmpty(backupName))
                {
                    backupName = $"{saveName}_Backup_{DateTime.Now:yyyyMMdd_HHmmss}";
                }

                backup.SaveName = backupName;

                // Save backup to file
                var backupFilePath = GetSaveFilePath(backupName);
                var json = backup.SerializeToJson();

                File.WriteAllText(backupFilePath, json);

                // Add to cache
                _saveCache[backupName] = backup;
                _saveFilePaths[backupName] = backupFilePath;

                System.Diagnostics.Debug.WriteLine($"Created backup: {backupName}");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating backup for '{saveName}': {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Get all save information.
        /// </summary>
        /// <returns>List of save information.</returns>
        public List<SaveInfo> GetAllSaveInfo()
        {
            var saveInfos = new List<SaveInfo>();

            foreach (var kvp in _saveCache)
            {
                var saveName = kvp.Key;
                var save = kvp.Value;

                var info = new SaveInfo
                {
                    Name = saveName,
                    SaveTime = save.SaveTime,
                    Difficulty = save.Difficulty,
                    MapName = save.MapName,
                    PlayTime = save.PlayTime,
                    CurrentWave = save.Waves?.CurrentWave ?? 0,
                    TotalWaves = save.Waves?.TotalWaves ?? 0,
                    CurrentCash = save.Economy?.CurrentCash ?? 0,
                    TowerCount = save.Towers?.TowerCount ?? 0,
                    FileSize = save.GetSaveSize(),
                    IsCompatible = save.IsCompatible(),
                    Summary = save.GetSummary()
                };

                saveInfos.Add(info);
            }

            return saveInfos.OrderByDescending(s => s.SaveTime).ToList();
        }

        /// <summary>
        /// Get save information by name.
        /// </summary>
        /// <param name="saveName">Name of save.</param>
        /// <returns>Save information, or null if not found.</returns>
        public SaveInfo GetSaveInfo(string saveName)
        {
            if (!_saveCache.TryGetValue(saveName, out var save))
                return null;

            return new SaveInfo
            {
                Name = saveName,
                SaveTime = save.SaveTime,
                Difficulty = save.Difficulty,
                MapName = save.MapName,
                PlayTime = save.PlayTime,
                CurrentWave = save.Waves?.CurrentWave ?? 0,
                TotalWaves = save.Waves?.TotalWaves ?? 0,
                CurrentCash = save.Economy?.CurrentCash ?? 0,
                TowerCount = save.Towers?.TowerCount ?? 0,
                FileSize = save.GetSaveSize(),
                IsCompatible = save.IsCompatible(),
                Summary = save.GetSummary()
            };
        }

        /// <summary>
        /// Check if a save exists.
        /// </summary>
        /// <param name="saveName">Name of save to check.</param>
        /// <returns>True if save exists.</returns>
        public bool SaveExists(string saveName)
        {
            return _saveCache.ContainsKey(saveName);
        }

        /// <summary>
        /// Get save file path.
        /// </summary>
        /// <param name="saveName">Name of save.</param>
        /// <returns>Full file path.</returns>
        public string GetSaveFilePath(string saveName)
        {
            return Path.Combine(_saveDirectory, $"{SanitizeFileName(saveName)}.json");
        }

        /// <summary>
        /// Validate all save files.
        /// </summary>
        /// <returns>Validation result.</returns>
        public ValidationResult ValidateAllSaves()
        {
            var result = new ValidationResult { IsValid = true };

            foreach (var kvp in _saveCache)
            {
                var saveName = kvp.Key;
                var save = kvp.Value;

                if (!save.ValidateSaveData())
                {
                    result.IsValid = false;
                    result.AddError($"Invalid save data: {saveName}");
                }

                // Check file exists
                var filePath = GetSaveFilePath(saveName);
                if (!File.Exists(filePath))
                {
                    result.IsValid = false;
                    result.AddError($"Save file missing: {saveName}");
                }
            }

            return result;
        }

        /// <summary>
        /// Clean up corrupted saves.
        /// </summary>
        /// <returns>Number of corrupted saves cleaned up.</returns>
        public int CleanupCorruptedSaves()
        {
            var corruptedSaves = new List<string>();

            foreach (var kvp in _saveCache)
            {
                var saveName = kvp.Key;
                var save = kvp.Value;

                if (!save.ValidateSaveData())
                {
                    corruptedSaves.Add(saveName);
                }
            }

            // Remove corrupted saves
            foreach (var saveName in corruptedSaves)
            {
                DeleteSave(saveName);
            }

            System.Diagnostics.Debug.WriteLine($"Cleaned up {corruptedSaves.Count} corrupted saves");
            return corruptedSaves.Count;
        }

        /// <summary>
        /// Export save to external location.
        /// </summary>
        /// <param name="saveName">Name of save to export.</param>
        /// <param name="exportPath">Export file path.</param>
        /// <returns>True if exported successfully.</returns>
        public bool ExportSave(string saveName, string exportPath)
        {
            if (!_saveCache.TryGetValue(saveName, out var save))
            {
                System.Diagnostics.Debug.WriteLine($"Save '{saveName}' not found");
                return false;
            }

            try
            {
                var json = save.SerializeToJson();
                File.WriteAllText(exportPath, json);
                System.Diagnostics.Debug.WriteLine($"Exported save '{saveName}' to {exportPath}");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error exporting save '{saveName}': {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Import save from external location.
        /// </summary>
        /// <param name="importPath">Import file path.</param>
        /// <param name="newSaveName">New save name (optional).</param>
        /// <returns>True if imported successfully.</returns>
        public bool ImportSave(string importPath, string newSaveName = null)
        {
            if (!File.Exists(importPath))
            {
                System.Diagnostics.Debug.WriteLine($"Import file not found: {importPath}");
                return false;
            }

            try
            {
                var json = File.ReadAllText(importPath);
                var save = SASGameSave.DeserializeFromJson(json);

                if (save == null)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to deserialize save from {importPath}");
                    return false;
                }

                if (string.IsNullOrEmpty(newSaveName))
                {
                    newSaveName = Path.GetFileNameWithoutExtension(importPath);
                }

                // Check if save name already exists
                if (_saveCache.ContainsKey(newSaveName))
                {
                    System.Diagnostics.Debug.WriteLine($"Save '{newSaveName}' already exists");
                    return false;
                }

                // Add to cache
                _saveCache[newSaveName] = save;
                var filePath = GetSaveFilePath(newSaveName);
                _saveFilePaths[newSaveName] = filePath;

                // Save to file
                File.WriteAllText(filePath, json);

                System.Diagnostics.Debug.WriteLine($"Imported save as '{newSaveName}'");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error importing save from {importPath}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Get total size of all save files.
        /// </summary>
        /// <returns>Total size in bytes.</returns>
        public long GetTotalSaveSize()
        {
            long totalSize = 0;

            foreach (var kvp in _saveCache)
            {
                totalSize += kvp.Value.GetSaveSize();
            }

            return totalSize;
        }

        /// <summary>
        /// Refresh save cache from disk.
        /// </summary>
        public void RefreshCache()
        {
            _saveCache.Clear();
            _saveFilePaths.Clear();
            LoadAllSaves();
        }

        ///  Private Methods

        /// <summary>
        /// Load all saves from disk.
        /// </summary>
        private void LoadAllSaves()
        {
            if (!Directory.Exists(_saveDirectory))
                return;

            var files = Directory.GetFiles(_saveDirectory, "*.json");

            foreach (var filePath in files)
            {
                try
                {
                    var fileName = Path.GetFileNameWithoutExtension(filePath);
                    var json = File.ReadAllText(filePath);
                    var save = SASGameSave.DeserializeFromJson(json);

                    if (save != null)
                    {
                        _saveCache[fileName] = save;
                        _saveFilePaths[fileName] = filePath;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error loading save from {filePath}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Sanitize file name for safe file system usage.
        /// </summary>
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

        /// 
    }

    /// <summary>
    /// Save information for display purposes.
    /// </summary>
    public class SaveInfo
    {
        public string Name { get; set; }
        public DateTime SaveTime { get; set; }
        public string Difficulty { get; set; }
        public string MapName { get; set; }
        public TimeSpan PlayTime { get; set; }
        public int CurrentWave { get; set; }
        public int TotalWaves { get; set; }
        public int CurrentCash { get; set; }
        public int TowerCount { get; set; }
        public long FileSize { get; set; }
        public bool IsCompatible { get; set; }
        public string Summary { get; set; }
    }

    /// <summary>
    /// Validation result for save operations.
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; } = new List<string>();
        public List<string> Warnings { get; } = new List<string>();

        public void AddError(string error)
        {
            Errors.Add(error);
        }

        public void AddWarning(string warning)
        {
            Warnings.Add(warning);
        }
    }
}
