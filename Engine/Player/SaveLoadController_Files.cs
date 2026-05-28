/// File:    E:\BDC\Projects\SASZombieAssaultTD\Engine\Player\SaveLoadController_Files.cs
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
using System.IO;

namespace SASZombieAssaultTD.Engine.Player
{
    public partial class SaveLoadController
    {
        /// <summary>
        /// Checks if a save file exists.
        /// </summary>
        /// <returns>True if a valid save file exists, false otherwise.</returns>
        public bool HasSaveFile()
        {
            lock (_lock)
            {
                try
                {
                    if (!File.Exists(_savePath))
                        return false;

                    // Check file size using SaveLoadCore
                    var fileInfo = new FileInfo(_savePath);

                    if (!SaveLoadCore.IsValidFileSize(fileInfo.Length))
                    {
                        System.Diagnostics.Debug.WriteLine("Warning", $"SaveLoadController: Save file has invalid size: {fileInfo.Length} bytes");
                        return false;
                    }

                    // Basic readability check
                    try
                    {
                        using var reader = new StreamReader(_savePath);
                        string firstLine = reader.ReadLine();

                        if (string.IsNullOrEmpty(firstLine) || !firstLine.TrimStart().StartsWith("{"))
                        {
                            System.Diagnostics.Debug.WriteLine("Warning", "SaveLoadController: Save file does not appear to be valid JSON");
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Warning", $"SaveLoadController: Save file readability check failed: {ex.Message}");
                        return false;
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error", $"SaveLoadController: HasSaveFile check failed: {ex.Message}");
                    return false;
                }
            }
        }

        /// <summary>
        /// Deletes the save file and backup.
        /// </summary>
        /// <returns>True if the deletion was successful, false otherwise.</returns>
        public bool DeleteSaveFile()
        {
            lock (_lock)
            {
                try
                {
                    bool mainDeleted = false;
                    bool backupDeleted = false;

                    // Delete main save file
                    if (File.Exists(_savePath))
                    {
                        File.Delete(_savePath);
                        mainDeleted = true;
                        System.Diagnostics.Debug.WriteLine("Info", $"SaveLoadController: Main save file deleted: {_savePath}");
                    }

                    // Delete backup save file
                    if (File.Exists(_backupPath))
                    {
                        File.Delete(_backupPath);
                        backupDeleted = true;
                        System.Diagnostics.Debug.WriteLine("Info", $"SaveLoadController: Backup save file deleted: {_backupPath}");
                    }

                    if (mainDeleted || backupDeleted)
                    {
                        System.Diagnostics.Debug.WriteLine("Info", "SaveLoadController: Save file deletion completed successfully");
                        return true;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Info", "SaveLoadController: No save files to delete");
                        return true; // Success if no files existed
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error", $"SaveLoadController: Delete save file failed: {ex.Message}");
                    return false;
                }
            }
        }

        /// <summary>
        /// Creates a backup of the existing save file.
        /// </summary>
        private void CreateBackup()
        {
            try
            {
                if (File.Exists(_savePath))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(_backupPath));
                    File.Copy(_savePath, _backupPath, true);
                    System.Diagnostics.Debug.WriteLine("Debug", $"SaveLoadController: Backup created: {_backupPath}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Warning", $"SaveLoadController: Backup creation failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Writes the save data to file.
        /// </summary>
        private void WriteSaveFile(string json)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_savePath));
                File.WriteAllText(_savePath, json);
                System.Diagnostics.Debug.WriteLine("Debug", $"SaveLoadController: Save file written: {_savePath}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error", $"SaveLoadController: Save file write failed: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Reads the save data from file.
        /// </summary>
        private string ReadSaveFile()
        {
            try
            {
                return File.ReadAllText(_savePath);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error", $"SaveLoadController: Save file read failed: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Verifies the integrity of the save file.
        /// </summary>
        private bool VerifySaveFile()
        {
            try
            {
                string json = ReadSaveFile();
                var data = SaveLoadCore.Deserialize(json);
                return SaveLoadCore.ValidatePlayerData(data);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error", $"SaveLoadController: Save file verification failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Restores the backup save file.
        /// </summary>
        private void RestoreBackup()
        {
            try
            {
                if (File.Exists(_backupPath))
                {
                    File.Copy(_backupPath, _savePath, true);
                    System.Diagnostics.Debug.WriteLine("Info", $"SaveLoadController: Backup restored: {_savePath}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error", $"SaveLoadController: Backup restoration failed: {ex.Message}");
            }
        }
    }
}
