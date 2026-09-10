// ====================================================================================================
// FILE: WaveLoader.cs
// PATH: Engine/Waves/WaveLoader.cs
// SUBSYSTEM: Waves
//
// ROLE:
//     Wave definition loader for the Waves subsystem.
//     Loads, validates, constructs, imports, exports, and manages WaveScript assets.
//
// RESPONSIBILITIES:
//     - Initialize the wave loader and ensure wave data directory exists.
//     - Load all wave scripts from JSON files or generate defaults when missing.
//     - Load individual wave scripts on demand.
//     - Save individual or all wave scripts back to JSON.
//     - Delete wave script files and update internal cache.
//     - Validate all loaded wave scripts and report errors/warnings.
//     - Provide accessors for wave scripts, counts, statistics, and enemy totals.
//     - Export wave scripts to JSON and import them from JSON.
//     - Reload wave scripts from disk.
//
// NON-RESPONSIBILITIES:
//     - Difficulty scaling or progression logic (handled by DifficultyScaler).
//     - Wave spawning, director control, or runtime wave execution.
//     - Low-level serialization beyond JSON read/write.
//
// NOTES:
//     - Fallback default wave generation is used when no JSON files exist.
//     - All wave scripts are cached in-memory for fast access.
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Render.Validation;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using static SASZombieAssaultTD.Engine.Enemies.EnemiesEnums;

namespace SASZombieAssaultTD.Engine.Waves
{
    public class WaveLoader
    {
        private readonly Dictionary<int, WaveScript> _waveScripts;
        private readonly string _waveDataPath;
        private bool _isInitialized;
        private static WaveLoader _instance;

        public static WaveLoader Instance => _instance ??= new WaveLoader();

        private WaveLoader()
        {
            _waveScripts = new Dictionary<int, WaveScript>();
            _waveDataPath = Path.Combine("Data", "Waves");
        }

        public void Initialize()
        {
            if (_isInitialized) return;

            DLogger.Log(LogSubsystems.ResourcesPipeline, "Initializing Wave Loader");

            try
            {
                Directory.CreateDirectory(_waveDataPath);
                LoadAllWaveScripts();

                _isInitialized = true;
                DLogger.Log($"Wave Loader initialized with {_waveScripts.Count} wave scripts");
            }
            catch (Exception ex)
            {
                DLogger.Log($"Failed to initialize Wave Loader: {ex.Message}");
                throw;
            }
        }

        public List<WaveScript> LoadAllWaveScripts()
        {
            _waveScripts.Clear();

            try
            {
                var loadedFromFiles = LoadFromJsonFiles();

                if (loadedFromFiles.Count > 0)
                {
                    DLogger.Log($"Loaded {loadedFromFiles.Count} wave scripts from JSON files");
                    return loadedFromFiles;
                }

                DLogger.Log(LogSubsystems.ResourcesPipeline, "No JSON files found, creating default wave scripts");
                var defaultWaves = CreateDefaultWaveScripts();

                foreach (var wave in defaultWaves)
                    _waveScripts[wave.WaveNumber] = wave;

                SaveAllWaveScripts();
                return defaultWaves;
            }
            catch (Exception ex)
            {
                DLogger.Log($"Error loading wave scripts: {ex.Message}");
                return new List<WaveScript>();
            }
        }

        public WaveScript LoadWaveScript(int waveNumber)
        {
            if (_waveScripts.TryGetValue(waveNumber, out var waveScript))
                return waveScript;

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
                        DLogger.Log($"Loaded wave {waveNumber} from file");
                        return waveScript;
                    }
                }
                catch (Exception ex)
                {
                    DLogger.Log($"Error loading wave {waveNumber} from file: {ex.Message}");
                }
            }

            DLogger.Log($"Wave {waveNumber} not found");
            return null;
        }

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
                _waveScripts[waveScript.WaveNumber] = waveScript;

                DLogger.Log($"Saved wave {waveScript.WaveNumber} to file");
                return true;
            }
            catch (Exception ex)
            {
                DLogger.Log($"Error saving wave {waveScript.WaveNumber}: {ex.Message}");
                return false;
            }
        }

        public bool SaveAllWaveScripts()
        {
            var success = true;

            foreach (var waveScript in _waveScripts.Values)
                if (!SaveWaveScript(waveScript))
                    success = false;

            DLogger.Log($"Saved {_waveScripts.Count} wave scripts to files (Success: {success})");
            return success;
        }

        public bool DeleteWaveScript(int waveNumber)
        {
            try
            {
                var filePath = GetWaveFilePath(waveNumber);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    _waveScripts.Remove(waveNumber);
                    DLogger.Log($"Deleted wave {waveNumber} file");
                    return true;
                }

                DLogger.Log($"Wave {waveNumber} file not found");
                return false;
            }
            catch (Exception ex)
            {
                DLogger.Log($"Error deleting wave {waveNumber}: {ex.Message}");
                return false;
            }
        }

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

                foreach (var warning in waveResult.Warnings)
                    result.AddWarning($"Wave {waveNumber}: {warning}");
            }

            var maxWaveNumber = _waveScripts.Keys.Count > 0 ? _waveScripts.Keys.Max() : 0;
            for (int i = 1; i <= maxWaveNumber; i++)
                if (!_waveScripts.ContainsKey(i))
                    result.AddWarning($"Missing wave {i}");

            return result;
        }

        public WaveScript GetWaveScript(int waveNumber)
            => _waveScripts.TryGetValue(waveNumber, out var waveScript) ? waveScript : null;

        public IReadOnlyDictionary<int, WaveScript> GetAllWaveScripts()
            => _waveScripts;

        public int GetWaveCount()
            => _waveScripts.Count;

        public int GetTotalEnemyCount()
        {
            var total = 0;
            foreach (var waveScript in _waveScripts.Values)
                total += waveScript.GetTotalEnemyCount();
            return total;
        }

        public WaveStatistics GetWaveStatistics()
        {
            return new WaveStatistics
            {
                TotalWaves = _waveScripts.Count,
                TotalEnemies = GetTotalEnemyCount(),
                AverageEnemiesPerWave = _waveScripts.Count > 0 ? GetTotalEnemyCount() / _waveScripts.Count : 0,
                MostDifficultWave = GetMostDifficultWave(),
                LeastDifficultWave = GetLeastDifficultWave(),
                BossWaves = GetBossWavesCount(),
                SpecialEnemyTypes = GetSpecialEnemyTypes()
            };
        }

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
                DLogger.Log($"Error exporting wave scripts to JSON: {ex.Message}");
                return string.Empty;
            }
        }

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
                    _waveScripts[wave.WaveNumber] = wave;

                SaveAllWaveScripts();
                DLogger.Log($"Imported {waves.Count} wave scripts from JSON");
                return true;
            }
            catch (Exception ex)
            {
                DLogger.Log($"Error importing wave scripts from JSON: {ex.Message}");
                return false;
            }
        }

        public bool ReloadWaveScripts()
        {
            try
            {
                _waveScripts.Clear();
                LoadAllWaveScripts();
                DLogger.Log(LogSubsystems.ResourcesPipeline, "Reloaded wave scripts from files");
                return true;
            }
            catch (Exception ex)
            {
                DLogger.Log($"Error reloading wave scripts: {ex.Message}");
                return false;
            }
        }

        private List<WaveScript> LoadFromJsonFiles()
        {
            var waves = new List<WaveScript>();

            if (!Directory.Exists(_waveDataPath))
            {
                DLogger.Log($"Wave data directory not found: {_waveDataPath}");
                return waves;
            }

            var files = Directory.GetFiles(_waveDataPath, "*.json");
            DLogger.Log($"Found {files.Length} wave JSON files");

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
                        if (waveScript.WaveNumber == 0)
                        {
                            var fileName = Path.GetFileNameWithoutExtension(file);
                            if (int.TryParse(fileName, out var waveNumber))
                                waveScript.WaveNumber = waveNumber;
                        }

                        _waveScripts[waveScript.WaveNumber] = waveScript;
                        waves.Add(waveScript);
                    }
                }
                catch (Exception ex)
                {
                    DLogger.Log($"Error loading wave from {file}: {ex.Message}");
                }
            }

            return waves;
        }

        private string GetWaveFilePath(int waveNumber)
            => Path.Combine(_waveDataPath, $"wave_{waveNumber:D3}.json");

        private List<WaveScript> CreateDefaultWaveScripts()
        {
            var waves = new List<WaveScript>();

            for (int i = 1; i <= 10; i++)
                waves.Add(CreateDefaultWaveScript(i));

            return waves;
        }

        public WaveScript CreateDefaultWaveScript(int waveNumber)
        {
            var waveScript = new WaveScript
            {
                WaveNumber = waveNumber,
                WaveName = $"Wave {waveNumber}",
                Description = $"Default wave {waveNumber}",
                InterWaveDelay = 10f,
                Modifiers = new WaveModifiers(),
                Rewards = new WaveRewards(),
                Environment = new WaveEnvironment()
            };

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

        private int GetBossWavesCount()
        {
            var count = 0;
            foreach (var waveScript in _waveScripts.Values)
                if (waveScript.HasBossEnemies())
                    count++;
            return count;
        }

        private List<ZombieType> GetSpecialEnemyTypes()
        {
            var types = new HashSet<ZombieType>();

            foreach (var waveScript in _waveScripts.Values)
                foreach (var enemyType in waveScript.GetEnemyTypes())
                    if (enemyType != ZombieType.Swarm)
                        types.Add(enemyType);

            return new List<ZombieType>(types);
        }
    }

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
