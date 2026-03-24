using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using SASZombieAssaultTD.Engine.Enemies;

namespace SASZombieAssaultTD.Engine.Waves
{
    /// <summary>
    /// Wave loader for SAS Zombie Assault TD.
    /// Loads wave scripts from JSON files and provides wave management.
    /// </summary>
    public class WaveLoader
    {
        private readonly Dictionary<int, WaveScript> _waveScripts;
        private readonly string _waveDataPath;
        private bool _isInitialized;
        private static WaveLoader _instance;

        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static WaveLoader Instance => _instance ??= new WaveLoader();

        private WaveLoader()
        {
            _waveScripts = new Dictionary<int, WaveScript>();
            _waveDataPath = Path.Combine("Data", "Waves");
        }

        /// <summary>
        /// Initialize the wave loader.
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized) return;

            Console.WriteLine("Initializing Wave Loader");

            try
            {
                // Create wave data directory if it doesn't exist
                Directory.CreateDirectory(_waveDataPath);

                // Load all wave scripts
                LoadAllWaveScripts();

                _isInitialized = true;
                Console.WriteLine($"Wave Loader initialized with {_waveScripts.Count} wave scripts");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to initialize Wave Loader: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Load all wave scripts from files.
        /// </summary>
        /// <returns>List of loaded wave scripts.</returns>
        public List<WaveScript> LoadAllWaveScripts()
        {
            _waveScripts.Clear();

            try
            {
                // Try to load from JSON files first
                var loadedFromFiles = LoadFromJsonFiles();

                if (loadedFromFiles.Count > 0)
                {
                    Console.WriteLine($"Loaded {loadedFromFiles.Count} wave scripts from JSON files");
                    return loadedFromFiles;
                }

                // Fallback to default wave scripts
                Console.WriteLine("No JSON files found, creating default wave scripts");
                var defaultWaves = CreateDefaultWaveScripts();

                foreach (var wave in defaultWaves)
                {
                    _waveScripts[wave.WaveNumber] = wave;
                }

                // Save default waves to JSON files
                SaveAllWaveScripts();

                return defaultWaves;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading wave scripts: {ex.Message}");
                return new List<WaveScript>();
            }
        }

        /// <summary>
        /// Load a specific wave script.
        /// </summary>
        /// <param name="waveNumber">Wave number to load.</param>
        /// <returns>Loaded wave script, or null if not found.</returns>
        public WaveScript LoadWaveScript(int waveNumber)
        {
            if (_waveScripts.TryGetValue(waveNumber, out var waveScript))
            {
                return waveScript;
            }

            // Try to load from file
            var filePath = GetWaveFilePath(waveNumber);
            if (File.Exists(filePath))
            {
                try
                {
                    var json = File.ReadAllText(filePath);
                    waveScript = JsonSerializer.Deserialize<WaveScript>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (waveScript != null)
                    {
                        _waveScripts[waveNumber] = waveScript;
                        Console.WriteLine($"Loaded wave {waveNumber} from file");
                        return waveScript;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading wave {waveNumber} from file: {ex.Message}");
                }
            }

            Console.WriteLine($"Wave {waveNumber} not found");
            return null;
        }

        /// <summary>
        /// Save a wave script to file.
        /// </summary>
        /// <param name="waveScript">Wave script to save.</param>
        /// <returns>True if saved successfully.</returns>
        public bool SaveWaveScript(WaveScript waveScript)
        {
            if (waveScript == null) return false;

            try
            {
                var filePath = GetWaveFilePath(waveScript.WaveNumber);

                var json = JsonSerializer.Serialize(waveScript, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                File.WriteAllText(filePath, json);

                // Update cache
                _waveScripts[waveScript.WaveNumber] = waveScript;

                Console.WriteLine($"Saved wave {waveScript.WaveNumber} to file");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving wave {waveScript.WaveNumber}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Save all wave scripts to files.
        /// </summary>
        /// <returns>True if all saved successfully.</returns>
        public bool SaveAllWaveScripts()
        {
            var success = true;

            foreach (var waveScript in _waveScripts.Values)
            {
                if (!SaveWaveScript(waveScript))
                {
                    success = false;
                }
            }

            Console.WriteLine($"Saved {_waveScripts.Count} wave scripts to files (Success: {success})");
            return success;
        }

        /// <summary>
        /// Delete a wave script file.
        /// </summary>
        /// <param name="waveNumber">Wave number to delete.</param>
        /// <returns>True if deleted successfully.</returns>
        public bool DeleteWaveScript(int waveNumber)
        {
            try
            {
                var filePath = GetWaveFilePath(waveNumber);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    _waveScripts.Remove(waveNumber);
                    Console.WriteLine($"Deleted wave {waveNumber} file");
                    return true;
                }

                Console.WriteLine($"Wave {waveNumber} file not found");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting wave {waveNumber}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Validate all wave scripts.
        /// </summary>
        /// <returns>Validation result.</returns>
        public ValidationResult ValidateAllWaveScripts()
        {
            var result = new ValidationResult { IsValid = true };

            foreach (var kvp in _waveScripts)
            {
                var waveNumber = kvp.Key;
                var waveScript = kvp.Value;

                var waveResult = waveScript.Validate();
                if (!waveResult.IsValid)
                {
                    result.IsValid = false;
                    result.AddError($"Wave {waveNumber}: {string.Join("; ", waveResult.Errors)}");
                }

                // Add warnings
                foreach (var warning in waveResult.Warnings)
                {
                    result.AddWarning($"Wave {waveNumber}: {warning}");
                }
            }

            // Check for missing wave numbers
            var maxWaveNumber = _waveScripts.Keys.Count > 0 ? _waveScripts.Keys.Max() : 0;
            for (int i = 1; i <= maxWaveNumber; i++)
            {
                if (!_waveScripts.ContainsKey(i))
                {
                    result.AddWarning($"Missing wave {i}");
                }
            }

            return result;
        }

        /// <summary>
        /// Get wave script by number.
        /// </summary>
        /// <param name="waveNumber">Wave number.</param>
        /// <returns>Wave script, or null if not found.</returns>
        public WaveScript GetWaveScript(int waveNumber)
        {
            return _waveScripts.TryGetValue(waveNumber, out var waveScript) ? waveScript : null;
        }

        /// <summary>
        /// Get all wave scripts.
        /// </summary>
        /// <returns>All loaded wave scripts.</returns>
        public IReadOnlyDictionary<int, WaveScript> GetAllWaveScripts()
        {
            return _waveScripts;
        }

        /// <summary>
        /// Get wave script count.
        /// </summary>
        /// <returns>Number of loaded wave scripts.</returns>
        public int GetWaveCount()
        {
            return _waveScripts.Count;
        }

        /// <summary>
        /// Get total enemy count across all waves.
        /// </summary>
        /// <returns>Total enemy count.</returns>
        public int GetTotalEnemyCount()
        {
            var total = 0;
            foreach (var waveScript in _waveScripts.Values)
            {
                total += waveScript.GetTotalEnemyCount();
            }
            return total;
        }

        /// <summary>
        /// Get wave statistics summary.
        /// </summary>
        /// <returns>Wave statistics.</returns>
        public WaveStatistics GetWaveStatistics()
        {
            var stats = new WaveStatistics
            {
                TotalWaves = _waveScripts.Count,
                TotalEnemies = GetTotalEnemyCount(),
                AverageEnemiesPerWave = _waveScripts.Count > 0 ? GetTotalEnemyCount() / _waveScripts.Count : 0,
                MostDifficultWave = GetMostDifficultWave(),
                LeastDifficultWave = GetLeastDifficultWave(),
                BossWaves = GetBossWavesCount(),
                SpecialEnemyTypes = GetSpecialEnemyTypes()
            };

            return stats;
        }

        /// <summary>
        /// Export wave scripts to JSON string.
        /// </summary>
        /// <returns>JSON string.</returns>
        public string ExportToJson()
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                return JsonSerializer.Serialize(_waveScripts.Values, options);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exporting wave scripts to JSON: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Import wave scripts from JSON string.
        /// </summary>
        /// <param name="json">JSON string to import.</param>
        /// <returns>True if imported successfully.</returns>
        public bool ImportFromJson(string json)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var waves = JsonSerializer.Deserialize<List<WaveScript>>(json, options);
                if (waves == null) return false;

                _waveScripts.Clear();
                foreach (var wave in waves)
                {
                    _waveScripts[wave.WaveNumber] = wave;
                }

                SaveAllWaveScripts();
                Console.WriteLine($"Imported {waves.Count} wave scripts from JSON");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error importing wave scripts from JSON: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Reload wave scripts from files.
        /// </summary>
        /// <returns>True if reloaded successfully.</returns>
        public bool ReloadWaveScripts()
        {
            try
            {
                _waveScripts.Clear();
                LoadAllWaveScripts();
                Console.WriteLine("Reloaded wave scripts from files");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reloading wave scripts: {ex.Message}");
                return false;
            }
        }

        #region Private Methods

        /// <summary>
        /// Load wave scripts from JSON files.
        /// </summary>
        private List<WaveScript> LoadFromJsonFiles()
        {
            var waves = new List<WaveScript>();

            if (!Directory.Exists(_waveDataPath))
            {
                Console.WriteLine($"Wave data directory not found: {_waveDataPath}");
                return waves;
            }

            var files = Directory.GetFiles(_waveDataPath, "*.json");
            Console.WriteLine($"Found {files.Length} wave JSON files");

            foreach (var file in files)
            {
                try
                {
                    var json = File.ReadAllText(file);
                    var waveScript = JsonSerializer.Deserialize<WaveScript>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (waveScript != null)
                    {
                        // Extract wave number from filename if not set
                        if (waveScript.WaveNumber == 0)
                        {
                            var fileName = Path.GetFileNameWithoutExtension(file);
                            if (int.TryParse(fileName, out var waveNumber))
                            {
                                waveScript.WaveNumber = waveNumber;
                            }
                        }

                        _waveScripts[waveScript.WaveNumber] = waveScript;
                        waves.Add(waveScript);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading wave from {file}: {ex.Message}");
                }
            }

            return waves;
        }

        /// <summary>
        /// Get file path for a wave number.
        /// </summary>
        private string GetWaveFilePath(int waveNumber)
        {
            return Path.Combine(_waveDataPath, $"wave_{waveNumber:D3}.json");
        }

        /// <summary>
        /// Create default wave scripts.
        /// </summary>
        /// <returns>List of default wave scripts.</returns>
        private List<WaveScript> CreateDefaultWaveScripts()
        {
            var waves = new List<WaveScript>();

            // Create 10 default waves with increasing difficulty
            for (int i = 1; i <= 10; i++)
            {
                var waveScript = CreateDefaultWaveScript(i);
                waves.Add(waveScript);
            }

            return waves;
        }

        /// <summary>
        /// Create a default wave script.
        /// </summary>
        /// <param name="waveNumber">Wave number.</param>
        /// <returns>Default wave script.</returns>
        private WaveScript CreateDefaultWaveScript(int waveNumber)
        {
            var waveScript = new WaveScript
            {
                WaveNumber = waveNumber,
                WaveName = $"Wave {waveNumber}",
                Description = $"Default wave {waveNumber}",
                InterWaveDelay = 10f,
                DifficultyMultiplier = new DifficultyMultiplier(),
                Modifiers = new WaveModifiers(),
                Rewards = new WaveRewards(),
                Environment = new WaveEnvironment()
            };

            // Add spawn groups based on wave number
            var enemyCount = 5 + (waveNumber * 2);
            var spawnGroup = new WaveSpawnGroup
            {
                EnemyType = (WaveSpawnGroup.ZombieType)GetDefaultEnemyType(waveNumber),
                Count = enemyCount,
                SpawnDelay = 0.5f,
                Pattern = GetDefaultSpawnPattern(waveNumber),
                DelayAfterGroup = 2f
            };

            waveScript.SpawnGroups.Add(spawnGroup);

            // Add boss every 5 waves
            if (waveNumber % 5 == 0)
            {
                var bossGroup = new WaveSpawnGroup
                {
                    EnemyType = WaveSpawnGroup.ZombieType.Swarm,
                    Count = 1,
                    SpawnDelay = 1f,
                    Pattern = SpawnPatternType.Line,
                    DelayAfterGroup = 3f,
                    IsBoss = true,
                    HealthMultiplier = 5f + (waveNumber * 0.5f),
                    SpeedMultiplier = 0.8f,
                    DamageMultiplier = 3f
                };

                waveScript.SpawnGroups.Add(bossGroup);
            }

            return waveScript;
        }

        /// <summary>
        /// Get default enemy type for wave number.
        /// </summary>
        private ZombieType GetDefaultEnemyType(int waveNumber)
        {
            return waveNumber switch
            {
                <= 2 => ZombieType.Swarm,
                <= 4 => ZombieType.Basic,
                <= 6 => ZombieType.Fast,
                <= 8 => ZombieType.Toxic,
                <= 10 => ZombieType.Armored,
                _ => ZombieType.Tank
            };
        }

        /// <summary>
        /// Get default spawn pattern for wave number.
        /// </summary>
        private SpawnPatternType GetDefaultSpawnPattern(int waveNumber)
        {
            var patterns = new[]
            {
                SpawnPatternType.Line,
                SpawnPatternType.Cluster,
                SpawnPatternType.Spread,
                SpawnPatternType.Wave,
                SpawnPatternType.Circle,
                SpawnPatternType.Flanking
            };

            return patterns[waveNumber % patterns.Length];
        }

        /// <summary>
        /// Get most difficult wave.
        /// </summary>
        private int GetMostDifficultWave()
        {
            var mostDifficult = 0;
            var maxDifficulty = 0f;

            foreach (var kvp in _waveScripts)
            {
                var difficulty = kvp.Value.GetDifficultyRating();
                if (difficulty > maxDifficulty)
                {
                    maxDifficulty = difficulty;
                    mostDifficult = kvp.Key;
                }
            }

            return mostDifficult;
        }

        /// <summary>
        /// Get least difficult wave.
        /// </summary>
        private int GetLeastDifficultWave()
        {
            var leastDifficult = 0;
            var minDifficulty = float.MaxValue;

            foreach (var kvp in _waveScripts)
            {
                var difficulty = kvp.Value.GetDifficultyRating();
                if (difficulty < minDifficulty)
                {
                    minDifficulty = difficulty;
                    leastDifficult = kvp.Key;
                }
            }

            return leastDifficult;
        }

        /// <summary>
        /// Get count of boss waves.
        /// </summary>
        private int GetBossWavesCount()
        {
            var count = 0;
            foreach (var waveScript in _waveScripts.Values)
            {
                if (waveScript.HasBossEnemies())
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Get special enemy types used across all waves.
        /// </summary>
        private List<ZombieType> GetSpecialEnemyTypes()
        {
            var types = new HashSet<ZombieType>();

            foreach (var waveScript in _waveScripts.Values)
            {
                foreach (var enemyType in waveScript.GetEnemyTypes())
                {
                    if (enemyType != ZombieType.Swarm)
                    {
                        types.Add(enemyType);
                    }
                }
            }

            return new List<ZombieType>(types);
        }

        #endregion
    }

    /// <summary>
    /// Wave statistics summary.
    /// </summary>
    public class WaveStatistics
    {
        public int TotalWaves { get; set; }
        public int TotalEnemies { get; set; }
        public float AverageEnemiesPerWave { get; set; }
        public int MostDifficultWave { get; set; }
        public int LeastDifficultWave { get; set; }
        public int BossWaves { get; set; }
        public List<ZombieType> SpecialEnemyTypes { get; set; }

        public override string ToString()
        {
            return $"Wave Statistics:\n" +
                   $"Total Waves: {TotalWaves}\n" +
                   $"Total Enemies: {TotalEnemies}\n" +
                   $"Average Enemies/Wave: {AverageEnemiesPerWave:F1}\n" +
                   $"Most Difficult: Wave {MostDifficultWave}\n" +
                   $"Least Difficult: Wave {LeastDifficultWave}\n" +
                   $"Boss Waves: {BossWaves}\n" +
                   $"Special Types: {string.Join(", ", SpecialEnemyTypes)}";
        }
    }
}
