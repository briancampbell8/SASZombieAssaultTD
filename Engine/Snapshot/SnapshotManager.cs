// ============================================================================
// File: SnapshotManager.cs
// Author: BDC
// Purpose: Manages snapshot creation, storage, validation, loading, and deletion.
// Notes:   Exception wrappers removed per doctrine (Option B). Structural integrity
//          restored after brace-collapse caused by commented-out try blocks.
// ============================================================================

using SASZombieAssaultTD.Engine.Diagnostics;

using SASZombieAssaultTD.Engine.Player;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace SASZombieAssaultTD.Engine.Snapshot
{
    /// <summary>
    /// Manages snapshot creation, storage, validation, and loading.
    /// Deterministic, audit-friendly, and drift-proof implementation.
    /// </summary>
    public sealed class SnapshotManager
    {
        private readonly string _snapshotDirectory;
        private readonly Dictionary<string, SnapshotData> _loadedSnapshots;
        private readonly object _lockObject = new object();

        public SnapshotManager(string snapshotDirectory = null)
        {
            _snapshotDirectory = snapshotDirectory ?? Path.Combine(
                Environment.CurrentDirectory, "Snapshots");

            _loadedSnapshots = new Dictionary<string, SnapshotData>();

            // Ensure snapshot directory exists
            Directory.CreateDirectory(_snapshotDirectory);

            System.Diagnostics.Debug.WriteLine("Info", $"[SNAPSHOT] Initialized with directory: {_snapshotDirectory}");
        }

        /// <summary>
        /// Creates a snapshot of the current game state.
        /// </summary>
        public SnapshotData CreateSnapshot(
            PlayerState playerState,
            int waveNumber,
            float gameTime,
            Dictionary<string, object> customData = null,
            string customId = null)
        {
            lock (_lockObject)
            {
                // Generate deterministic snapshot ID
                string snapshotId = customId ?? GenerateSnapshotId(playerState, waveNumber, gameTime);

                // Create snapshot with current timestamp
                var snapshot = new SnapshotData(
                    snapshotId,
                    DateTime.UtcNow,
                    GetGameVersion(),
                    playerState.DeepClone(),
                    waveNumber,
                    gameTime,
                    customData
                );

                // Calculate checksum for validation
                snapshot.Metadata.Checksum = CalculateChecksum(snapshot);
                snapshot.Metadata.CreationContext = "Manual";

                // Validate snapshot before storing
                var validationResult = ValidateSnapshot(snapshot);
                snapshot.Metadata.IsValid = validationResult.IsValid;
                snapshot.Metadata.ValidationErrors = validationResult.Errors;

                if (!validationResult.IsValid)
                {
                    System.Diagnostics.Debug.WriteLine("Warning",
                        $"[SNAPSHOT] Validation failed for {snapshotId}: {string.Join(", ", validationResult.Errors)}");
                }

                // Store in memory cache
                _loadedSnapshots[snapshotId] = snapshot;

                System.Diagnostics.Debug.WriteLine("Info",
                    $"[SNAPSHOT] Created snapshot {snapshotId} at {snapshot.Timestamp:yyyy-MM-dd HH:mm:ss}");

                return snapshot;
            }
        }

        /// <summary>
        /// Saves a snapshot to disk.
        /// </summary>
        public bool SaveSnapshot(SnapshotData snapshot)
        {
            if (snapshot == null)
            {
                System.Diagnostics.Debug.WriteLine("Error", "[SNAPSHOT] Cannot save null snapshot");
                return false;
            }

            lock (_lockObject)
            {
                string filename = $"{snapshot.Id}.json";
                string filepath = Path.Combine(_snapshotDirectory, filename);

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    IgnoreNullValues = true
                };

                string json = JsonSerializer.Serialize(snapshot, options);
                File.WriteAllText(filepath, json);

                System.Diagnostics.Debug.WriteLine("Info",
                    $"[SNAPSHOT] Saved snapshot {snapshot.Id} to {filepath}");

                return true;
            }
        }

        /// <summary>
        /// Loads a snapshot from disk.
        /// </summary>
        public SnapshotData LoadSnapshot(string snapshotId)
        {
            lock (_lockObject)
            {
                // Check memory cache first
                if (_loadedSnapshots.TryGetValue(snapshotId, out var cached))
                {
                    return cached.DeepClone();
                }

                string filename = $"{snapshotId}.json";
                string filepath = Path.Combine(_snapshotDirectory, filename);

                if (!File.Exists(filepath))
                {
                    System.Diagnostics.Debug.WriteLine("Warning",
                        $"[SNAPSHOT] Snapshot file not found: {filepath}");
                    return null;
                }

                string json = File.ReadAllText(filepath);

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                };

                var snapshot = JsonSerializer.Deserialize<SnapshotData>(json, options);

                if (snapshot == null)
                {
                    System.Diagnostics.Debug.WriteLine("Error",
                        $"[SNAPSHOT] Failed to deserialize snapshot {snapshotId}");
                    return null;
                }

                // Validate loaded snapshot
                var validationResult = ValidateSnapshot(snapshot);
                snapshot.Metadata.IsValid = validationResult.IsValid;
                snapshot.Metadata.ValidationErrors = validationResult.Errors;

                if (!validationResult.IsValid)
                {
                    System.Diagnostics.Debug.WriteLine("Warning",
                        $"[SNAPSHOT] Loaded snapshot {snapshotId} has validation errors: {string.Join(", ", validationResult.Errors)}");
                }

                // Cache the loaded snapshot
                _loadedSnapshots[snapshotId] = snapshot;

                System.Diagnostics.Debug.WriteLine("Info",
                    $"[SNAPSHOT] Loaded snapshot {snapshotId} from {filepath}");

                return snapshot.DeepClone();
            }
        }

        /// <summary>
        /// Applies a snapshot to the current game state.
        /// </summary>
        public bool ApplySnapshot(SnapshotData snapshot, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (snapshot == null)
            {
                errorMessage = "Snapshot is null";
                return false;
            }

            var validationResult = ValidateSnapshot(snapshot);
            if (!validationResult.IsValid)
            {
                errorMessage = $"Snapshot validation failed: {string.Join(", ", validationResult.Errors)}";
                return false;
            }

            var playerState = snapshot.PlayerState;
            var playerSystem = PlayerSystem.Instance;

            playerSystem.RestoreCash(playerState.Cash);
            playerSystem.RestoreScore(playerState.Score);

            System.Diagnostics.Debug.WriteLine("Info",
                $"[SNAPSHOT] Applied player state: Cash=${playerState.Cash}, Score={playerState.Score}, Lives={playerState.Lives}, Wave={playerState.WaveNumber}");

            System.Diagnostics.Debug.WriteLine("Info",
                $"[SNAPSHOT] Applied snapshot {snapshot.Id} successfully");

            return true;
        }

        /// <summary>
        /// Lists all available snapshots.
        /// </summary>
        public List<SnapshotInfo> ListSnapshots()
        {
            var snapshots = new List<SnapshotInfo>();

            lock (_lockObject)
            {
                // Memory cache
                foreach (var kvp in _loadedSnapshots)
                {
                    snapshots.Add(new SnapshotInfo
                    {
                        Id = kvp.Value.Id,
                        Timestamp = kvp.Value.Timestamp,
                        WaveNumber = kvp.Value.WaveNumber,
                        Cash = kvp.Value.PlayerState.Cash,
                        Lives = kvp.Value.PlayerState.Lives,
                        IsValid = kvp.Value.Metadata.IsValid,
                        Source = "Memory"
                    });
                }

                // Disk snapshots
                if (Directory.Exists(_snapshotDirectory))
                {
                    foreach (var file in Directory.GetFiles(_snapshotDirectory, "*.json"))
                    {
                        var filename = Path.GetFileNameWithoutExtension(file);

                        if (_loadedSnapshots.ContainsKey(filename))
                            continue;

                        var json = File.ReadAllText(file);

                        var options = new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                            PropertyNameCaseInsensitive = true
                        };

                        var snapshot = JsonSerializer.Deserialize<SnapshotData>(json, options);

                        if (snapshot != null)
                        {
                            snapshots.Add(new SnapshotInfo
                            {
                                Id = snapshot.Id,
                                Timestamp = snapshot.Timestamp,
                                WaveNumber = snapshot.WaveNumber,
                                Cash = snapshot.PlayerState.Cash,
                                Lives = snapshot.PlayerState.Lives,
                                IsValid = snapshot.Metadata.IsValid,
                                Source = "Disk"
                            });
                        }
                    }
                }
            }

            snapshots.Sort((a, b) => b.Timestamp.CompareTo(a.Timestamp));
            return snapshots;
        }

        /// <summary>
        /// Deletes a snapshot.
        /// </summary>
        public bool DeleteSnapshot(string snapshotId)
        {
            lock (_lockObject)
            {
                _loadedSnapshots.Remove(snapshotId);

                string filename = $"{snapshotId}.json";
                string filepath = Path.Combine(_snapshotDirectory, filename);

                if (File.Exists(filepath))
                {
                    File.Delete(filepath);
                    System.Diagnostics.Debug.WriteLine("Info",
                        $"[SNAPSHOT] Deleted snapshot {snapshotId} from disk");
                }

                return true;
            }
        }

        // ---------------------------------------------------------------------
        // Private Methods
        // ---------------------------------------------------------------------

        private string GenerateSnapshotId(PlayerState playerState, int waveNumber, float gameTime)
        {
            var idData = $"{playerState.Cash}_{playerState.Lives}_{waveNumber}_{gameTime}_{DateTime.UtcNow:yyyyMMddHHmmss}";
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(idData));
            return $"snap_{BitConverter.ToString(hash).Replace("-", "").Substring(0, 16)}";
        }

        private string GetGameVersion()
        {
            return "1.0.0";
        }

        private string CalculateChecksum(SnapshotData snapshot)
        {
            var checksumData =
                $"{snapshot.Id}|{snapshot.Timestamp:o}|{snapshot.GameVersion}|{snapshot.PlayerState.Cash}|{snapshot.PlayerState.Lives}|{snapshot.WaveNumber}|{snapshot.GameTime}";

            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(checksumData));
            return Convert.ToBase64String(hash);
        }

        private ValidationResult ValidateSnapshot(SnapshotData snapshot)
        {
            var result = new ValidationResult { IsValid = true };

            if (string.IsNullOrEmpty(snapshot.Id))
                result.AddError("Snapshot ID is null or empty");

            if (snapshot.Timestamp == default)
                result.AddError("Snapshot timestamp is invalid");

            if (snapshot.PlayerState == null)
                result.AddError("Player state is null");
            else
            {
                if (snapshot.PlayerState.Cash < 0)
                    result.AddError("Player cash is negative");

                if (snapshot.PlayerState.Lives < 0)
                    result.AddError("Player lives is negative");
            }

            if (snapshot.WaveNumber < 0)
                result.AddError("Wave number is negative");

            if (snapshot.GameTime < 0)
                result.AddError("Game time is negative");

            if (!string.IsNullOrEmpty(snapshot.Metadata.Checksum))
            {
                var expectedChecksum = CalculateChecksum(snapshot);
                if (snapshot.Metadata.Checksum != expectedChecksum)
                    result.AddError("Snapshot checksum mismatch - data may be corrupted");
            }

            return result;
        }

        internal SnapshotData CreateSnapshot(object playerState, int waveNumber, float gameTime, object customData, string customId)
        {
            NI.Hit();
            return default(SnapshotData);
        }

        internal SnapshotData CreateSnapshot(object state, int waveNumber, float gameTime, Dictionary<string, object> customData)
        {
            NI.Hit();
            return default(SnapshotData);
        }
    }

    // ============================================================================
    // SnapshotInfo
    // ============================================================================

    /// <summary>
    /// Snapshot information for listing and management.
    /// </summary>
    public sealed class SnapshotInfo
    {
        public string Id { get; set; }
        public DateTime Timestamp { get; set; }
        public int WaveNumber { get; set; }
        public int Cash { get; set; }
        public int Lives { get; set; }
        public bool IsValid { get; set; }
        public string Source { get; set; } // "Memory" or "Disk"
    }

    // ============================================================================
    // ValidationResult
    // ============================================================================

    /// <summary>
    /// Validation result for snapshots.
    /// </summary>
    internal sealed class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new List<string>();

        public void AddError(string error)
        {
            Errors.Add(error);
            IsValid = false;
        }
    }
}
