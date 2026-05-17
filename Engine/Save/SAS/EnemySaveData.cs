using System;
using System.Collections.Generic;
using System.Linq;
using SASZombieAssaultTD.Engine.VectorMath;
using SASZombieAssaultTD.Engine.Enemies;
using SASZombieAssaultTD.Engine.Gameplay.Enemies;
using SASZombieAssaultTD.Engine.Waves;
using EnemyType = SASZombieAssaultTD.Engine.Dictionary.EnemyType;
using ZombieType = SASZombieAssaultTD.Engine.Dictionary.ZombieType;

namespace SASZombieAssaultTD.Engine.Save.SAS
{
    /// <summary>
    /// Enemy save data container for SAS TD.
    /// Handles serialization and deserialization of enemy state.
    /// </summary>
    public class EnemySaveData
    {
        // Basic enemy information
        public int TotalSpawned { get; set; }
        public int TotalKilled { get; set; }
        public int TotalEscaped { get; set; }
        public int ActiveEnemies { get; set; }
        public DateTime LastUpdated { get; set; }

        // Enemy collection data
        public List<EnemySaveInfo> Enemies { get; set; }
        public Dictionary<uint, EnemyType> EnemyTypes { get; set; }
        public Dictionary<uint, Vector3> EnemyPositions { get; set; }
        public Dictionary<uint, float> EnemyHealth { get; set; }
        public Dictionary<uint, Vector3> EnemyVelocities { get; set; }
        public Dictionary<uint, object> EnemyWaves { get; set; }

        // Enemy statistics
        public Dictionary<uint, EnemyStatistics> EnemyStats { get; set; }
        public Dictionary<uint, int> EnemyKills { get; set; }
        public Dictionary<uint, float> EnemyDamage { get; set; }
        public Dictionary<uint, float> EnemyLifetime { get; set; }

        // Wave-specific data
        public Dictionary<int, List<uint>> WaveEnemies { get; set; }
        public Dictionary<int, Dictionary<uint, EnemyType>> WaveEnemyTypes { get; set; }
        public Dictionary<int, Dictionary<uint, Vector3>> WaveSpawnPositions { get; set; }

        // Custom enemy data
        public Dictionary<uint, Dictionary<string, object>> CustomData { get; set; }

        public EnemySaveData()
        {
            Enemies = new List<EnemySaveInfo>();
            EnemyTypes = new Dictionary<uint, EnemyType>();
            EnemyPositions = new Dictionary<uint, Vector3>();
            EnemyHealth = new Dictionary<uint, float>();
            EnemyVelocities = new Dictionary<uint, Vector3>();
            EnemyWaves = new Dictionary<uint, object>();
            EnemyStats = new Dictionary<uint, EnemyStatistics>();
            EnemyKills = new Dictionary<uint, int>();
            EnemyDamage = new Dictionary<uint, float>();
            EnemyLifetime = new Dictionary<uint, float>();
            WaveEnemies = new Dictionary<int, List<uint>>();
            WaveEnemyTypes = new Dictionary<int, Dictionary<uint, EnemyType>>();
            WaveSpawnPositions = new Dictionary<int, Dictionary<uint, Vector3>>();
            CustomData = new Dictionary<uint, Dictionary<string, object>>();
        }

        /// <summary>
        /// Validate enemy save data.
        /// </summary>
        /// <returns>True if data is valid.</returns>
        public bool Validate()
        {
            if (TotalSpawned < 0)
                return false;

            if (TotalKilled < 0)
                return false;

            if (TotalEscaped < 0)
                return false;

            if (ActiveEnemies < 0)
                return false;

            if (LastUpdated == default)
                return false;

            // Validate enemy collection
            if (Enemies == null)
                return false;

            // Validate consistency
            if (TotalSpawned != TotalKilled + TotalEscaped + ActiveEnemies)
                return false;

            return true;
        }

        /// <summary>
        /// Clone this enemy save data.
        /// </summary>
        /// <returns>Cloned data.</returns>
        public EnemySaveData Clone()
        {
            return new EnemySaveData
            {
                TotalSpawned = this.TotalSpawned,
                TotalKilled = this.TotalKilled,
                TotalEscaped = this.TotalEscaped,
                ActiveEnemies = this.ActiveEnemies,
                LastUpdated = this.LastUpdated,
                Enemies = new List<EnemySaveInfo>(this.Enemies),
                EnemyTypes = new Dictionary<uint, EnemyType>(this.EnemyTypes),
                EnemyPositions = new Dictionary<uint, Vector3>(this.EnemyPositions),
                EnemyHealth = new Dictionary<uint, float>(this.EnemyHealth),
                EnemyVelocities = new Dictionary<uint, Vector3>(this.EnemyVelocities),
                EnemyWaves = new Dictionary<uint, object>(this.EnemyWaves),
                EnemyStats = new Dictionary<uint, EnemyStatistics>(this.EnemyStats),
                EnemyKills = new Dictionary<uint, int>(this.EnemyKills),
                EnemyDamage = new Dictionary<uint, float>(this.EnemyDamage),
                EnemyLifetime = new Dictionary<uint, float>(this.EnemyLifetime),
                WaveEnemies = new Dictionary<int, List<uint>>(this.WaveEnemies),
                WaveEnemyTypes = new Dictionary<int, Dictionary<uint, EnemyType>>(this.WaveEnemyTypes),
                WaveSpawnPositions = new Dictionary<int, Dictionary<uint, Vector3>>(this.WaveSpawnPositions),
                CustomData = new Dictionary<uint, Dictionary<string, object>>(this.CustomData)
            };
        }

        /// <summary>
        /// Apply enemy save data to current game state.
        /// </summary>
        /// <returns>True if applied successfully.</returns>
        public bool ApplyToGame()
        {
            try
            {
                var enemyManager = EnemyManager.Instance;
                if (enemyManager == null)
                    return false;

                // Clear existing enemies
                enemyManager.ClearAllEnemies();

                // Restore enemies
                foreach (var enemyInfo in Enemies)
                {
                    var enemy = CreateEnemyFromSaveInfo(enemyInfo);
                    if (enemy != null)
                    {
                        enemyManager.AddEnemy(enemy);
                    }
                }

                // Restore enemy states
                foreach (var kvp in EnemyHealth)
                {
                    var enemyId = kvp.Key;
                    var health = kvp.Value;
                    var enemy = enemyManager.GetEnemy(enemyId);
                    if (enemy != null)
                    {
                        enemy.Health = health;
                    }
                }

                // Restore enemy velocities
                foreach (var kvp in EnemyVelocities)
                {
                    var enemyId = kvp.Key;
                    var velocity = kvp.Value;
                    var enemy = enemyManager.GetEnemy(enemyId);
                    if (enemy != null)
                    {
                        enemy.Velocity = velocity;
                    }
                }

                // Restore enemy waves
                foreach (var kvp in EnemyWaves)
                {
                    var enemyId = kvp.Key;
                    var wave = kvp.Value;
                    var enemy = enemyManager.GetEnemy(enemyId);
                    if (enemy != null)
                    {
                        enemy.SourceWave = wave is int w ? w : 0;
                    }
                }

                // Restore enemy statistics
                foreach (var kvp in EnemyStats)
                {
                    var enemyId = kvp.Key;
                    var stats = kvp.Value;
                    var enemy = enemyManager.GetEnemy(enemyId);
                    if (enemy != null)
                    {
                        enemy.TotalKills = stats.TotalKills;
                        enemy.DamageDealt = stats.DamageDealt;
                        enemy.Lifetime = stats.Lifetime;
                    }
                }

                Console.WriteLine($"Applied enemy save data: {TotalSpawned} enemies");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error applying enemy save data: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Capture current enemy state.
        /// </summary>
        /// <returns>Captured enemy save data.</returns>
        public static EnemySaveData CaptureCurrentState()
        {
            var enemyManager = EnemyManager.Instance;
            if (enemyManager == null)
                return new EnemySaveData();

            var saveData = new EnemySaveData
            {
                TotalSpawned = enemyManager.GetTotalSpawned(),
                TotalKilled = enemyManager.GetTotalKilled(),
                TotalEscaped = enemyManager.GetTotalEscaped(),
                ActiveEnemies = enemyManager.GetActiveEnemyCount(),
                LastUpdated = DateTime.Now
            };

            // Capture enemy information
            var enemies = enemyManager.GetAllEnemies();
            foreach (var enemy in enemies)
            {
                var enemyInfo = new EnemySaveInfo
                {
                    Id = enemy.Id.ToString(),
                    Type = enemy.Type.ToString(),
                    Name = enemy.Name,
                    Position = enemy.Position,
                    Velocity = enemy.Velocity,
                    Health = enemy.Health,
                    MaxHealth = enemy.MaxHealth,
                    Damage = enemy.Damage,
                    Speed = enemy.Speed,
                    Armor = enemy.Armor,
                    IsActive = enemy.IsActive,
                    SourceWave = enemy.SourceWave,
                    SpawnTime = DateTime.Now.AddSeconds(enemy.SpawnTime),
                    DeathTime = enemy.IsDead ? DateTime.Now.AddSeconds(enemy.DeathTime) : (DateTime?)null,
                    PathProgress = enemy.PathProgress,
                    Lifetime = (float)TimeSpan.FromSeconds(enemy.Lifetime).TotalSeconds
                };

                saveData.Enemies.Add(enemyInfo);

                // Add to collections
                saveData.EnemyTypes[enemy.Id] = (SASZombieAssaultTD.Engine.Dictionary.EnemyType)Enum.Parse<SASZombieAssaultTD.Engine.Dictionary.EnemyType>(enemy.Type.ToString());
                saveData.EnemyPositions[enemy.Id] = enemy.Position;
                saveData.EnemyHealth[enemy.Id] = enemy.Health;
                saveData.EnemyVelocities[enemy.Id] = enemy.Velocity;
                saveData.EnemyWaves[enemy.Id] = enemy.SourceWave;

                // Add to wave collections
                var wave = enemy.SourceWave;
                var waveId = wave is int w ? w : (int)wave;
                if (!saveData.WaveEnemies.ContainsKey(waveId))
                {
                    saveData.WaveEnemies[waveId] = new List<uint>();
                    saveData.WaveEnemyTypes[waveId] = new Dictionary<uint, EnemyType>();
                    saveData.WaveSpawnPositions[waveId] = new Dictionary<uint, Vector3>();
                }

                saveData.WaveEnemies[waveId].Add(enemy.Id);
                saveData.WaveEnemyTypes[waveId][enemy.Id] = (SASZombieAssaultTD.Engine.Dictionary.EnemyType)Enum.Parse<SASZombieAssaultTD.Engine.Dictionary.EnemyType>(enemy.Type.ToString());
                saveData.WaveSpawnPositions[waveId][enemy.Id] = enemy.Position;

                // Capture statistics
                saveData.EnemyStats[enemy.Id] = new EnemyStatistics
                {
                    TotalKills = enemy.TotalKills,
                    DamageDealt = enemy.DamageDealt,
                    Lifetime = TimeSpan.FromSeconds(enemy.Lifetime),
                    Accuracy = enemy.Accuracy,
                    Speed = enemy.Speed,
                    Armor = enemy.Armor
                };

                saveData.EnemyKills[enemy.Id] = enemy.TotalKills;
                saveData.EnemyDamage[enemy.Id] = enemy.DamageDealt;
                saveData.EnemyLifetime[enemy.Id] = enemy.Lifetime;
            }

            return saveData;
        }

        /// <summary>
        /// Get enemy save information by ID.
        /// </summary>
        /// <param name="enemyId">Enemy ID.</param>
        /// <returns>Enemy save info, or null if not found.</returns>
        public EnemySaveInfo GetEnemyInfo(string enemyId)
        {
            return Enemies.FirstOrDefault(e => e.Id == enemyId);
        }

        /// <summary>
        /// Get enemy type by ID.
        /// </summary>
        /// <param name="enemyId">Enemy ID.</param>
        /// <returns>Enemy type, or default if not found.</returns>
        public EnemyType GetEnemyType(uint enemyId)
        {
            if (EnemyTypes.TryGetValue(enemyId, out var type))
                return type;

            // Fallback conversion from ZombieType to EnemyType
            if (Enum.TryParse<ZombieType>(enemyId.ToString(), out var zombieType))
            {
                return zombieType switch
                {
                    ZombieType.Basic => EnemyType.BasicZombie,
                    ZombieType.Fast => EnemyType.FastZombie,
                    ZombieType.Tank => EnemyType.TankZombie,
                    ZombieType.Swift => EnemyType.SwiftZombie,
                    ZombieType.Heavy => EnemyType.HeavyZombie,
                    ZombieType.Swarm => EnemyType.SwarmZombie,
                    ZombieType.Spitter => EnemyType.SpitterZombie,
                    ZombieType.Bomber => EnemyType.BomberZombie,
                    _ => EnemyType.BasicZombie
                };
            }

            return EnemyType.BasicZombie;
        }

        /// <summary>
        /// Get enemy position by ID.
        /// </summary>
        /// <param name="enemyId">Enemy ID.</param>
        /// <returns>Enemy position, or Vector3.Zero if not found.</returns>
        public Vector3 GetEnemyPosition(uint enemyId)
        {
            return EnemyPositions.TryGetValue(enemyId, out var position) ? position : Vector3.Zero;
        }

        /// <summary>
        /// Get enemy health by ID.
        /// </summary>
        /// <param name="enemyId">Enemy ID.</param>
        /// <returns>Enemy health, or 0 if not found.</returns>
        public float GetEnemyHealth(uint enemyId)
        {
            return EnemyHealth.TryGetValue(enemyId, out var health) ? health : 0f;
        }

        /// <summary>
        /// Get enemy velocity by ID.
        /// </summary>
        /// <param name="enemyId">Enemy ID.</param>
        /// <returns>Enemy velocity, or Vector3.Zero if not found.</returns>
        public Vector3 GetEnemyVelocity(uint enemyId)
        {
            return EnemyVelocities.TryGetValue(enemyId, out var velocity) ? velocity : Vector3.Zero;
        }

        /// <summary>
        /// Get enemy wave by ID.
        /// </summary>
        /// <param name="enemyId">Enemy ID.</param>
        /// <returns>Enemy wave, or 0 if not found.</returns>
        public int GetEnemyWave(uint enemyId)
        {
            return EnemyWaves.TryGetValue(enemyId, out var wave) && wave is int w ? w : 0;
        }

        /// <summary>
        /// Get enemy statistics by ID.
        /// </summary>
        /// <param name="enemyId">Enemy ID.</param>
        /// <returns>Enemy statistics, or null if not found.</returns>
        public EnemyStatistics GetEnemyStatistics(uint enemyId)
        {
            return EnemyStats.TryGetValue(enemyId, out var stats) ? stats : null;
        }

        /// <summary>
        /// Add or update enemy statistics.
        /// </summary>
        /// <param name="enemyId">Enemy ID.</param>
        /// <param name="statistics">Enemy statistics.</param>
        public void SetEnemyStatistics(uint enemyId, EnemyStatistics statistics)
        {
            EnemyStats[enemyId] = statistics;
        }

        /// <summary>
        /// Add custom data for an enemy.
        /// </summary>
        /// <param name="enemyId">Enemy ID.</param>
        /// <param name="key">Data key.</param>
        /// <param name="value">Data value.</param>
        public void SetCustomData(uint enemyId, string key, object value)
        {
            if (!CustomData.ContainsKey(enemyId))
            {
                CustomData[enemyId] = new Dictionary<string, object>();
            }
            CustomData[enemyId][key] = value;
        }

        /// <summary>
        /// Get custom data for an enemy.
        /// </summary>
        /// <param name="enemyId">Enemy ID.</param>
        /// <param name="key">Data key.</param>
        /// <returns>Data value, or null if not found.</returns>
        public object GetCustomData(uint enemyId, string key)
        {
            return CustomData.TryGetValue(enemyId, out var data) && data.TryGetValue(key, out var value) ? value : null;
        }

        /// <summary>
        /// Get enemies for a specific wave.
        /// </summary>
        /// <param name="wave">Wave number.</param>
        /// <returns>List of enemy IDs in the wave.</returns>
        public List<string> GetWaveEnemies(int wave)
        {
            return WaveEnemies.TryGetValue(wave, out var enemies) ? enemies.Select(id => id.ToString()).ToList() : new List<string>();
        }

        /// <summary>
        /// Get enemy types for a specific wave.
        /// </summary>
        /// <param name="wave">Wave number.</param>
        /// <returns>Dictionary of enemy types by ID.</returns>
        public Dictionary<string, EnemyType> GetWaveEnemyTypes(int wave)
        {
            return WaveEnemyTypes.TryGetValue(wave, out var types) ? types.ToDictionary(innerKvp => innerKvp.Key.ToString(), innerKvp => innerKvp.Value) : new Dictionary<string, EnemyType>();
        }

        /// <summary>
        /// Get spawn positions for a specific wave.
        /// </summary>
        /// <param name="wave">Wave number.</param>
        /// <returns>Dictionary of spawn positions by ID.</returns>
        public Dictionary<string, Vector3> GetWaveSpawnPositions(int wave)
        {
            return WaveSpawnPositions.TryGetValue(wave, out var positions) ? positions.ToDictionary(innerKvp => innerKvp.Key.ToString(), innerKvp => innerKvp.Value) : new Dictionary<string, Vector3>();
        }

        /// <summary>
        /// Get save summary.
        /// </summary>
        /// <returns>Save summary string.</returns>
        public string GetSummary()
        {
            return $"Enemy Save Data:\n" +
                   $"Total Spawned: {TotalSpawned}\n" +
                   $"Total Killed: {TotalKilled}\n" +
                   $"Total Escaped: {TotalEscaped}\n" +
                   $"Active Enemies: {ActiveEnemies}\n" +
                   $"Enemy Types: {string.Join(", ", EnemyTypes.Values.Distinct())}\n" +
                   $"Average Health: {(EnemyHealth.Count > 0 ? EnemyHealth.Values.Average() : 0f):F1}\n" +
                   $"Average Speed: {(EnemyVelocities.Count > 0 ? EnemyVelocities.Values.Average(v => v.Length) : 0f):F1}\n" +
                   $"Average Lifetime: {(EnemyLifetime.Count > 0 ? EnemyLifetime.Values.Average() / 60 : 0f):F1}m\n" +
                   $"Last Updated: {LastUpdated:yyyy-MM-dd HH:mm:ss}";
        }

        /// <summary>
        /// Get wave summary.
        /// </summary>
        /// <param name="wave">Wave number.</param>
        /// <returns>Wave summary string.</returns>
        public string GetWaveSummary(int wave)
        {
            var enemies = GetWaveEnemies(wave);
            var types = GetWaveEnemyTypes(wave);
            var positions = GetWaveSpawnPositions(wave);

            return $"Wave {wave}:\n" +
                   $"Enemies: {enemies.Count}\n" +
                   $"Types: {string.Join(", ", types.Values.Distinct())}\n" +
                   $"Spawn Positions: {positions.Count}";
        }

        #region Private Methods

        /// <summary>
        /// Create an enemy from save information.
        /// </summary>
        private Enemy CreateEnemyFromSaveInfo(EnemySaveInfo enemyInfo)
        {
            try
            {
                // Create basic enemy using saved id so Entity/Id are preserved
                if (!uint.TryParse(enemyInfo.Id, out var parsedId))
                    return null;

                var enemy = Enemy.CreateEnemy(parsedId);
                if (enemy != null)
                {
                    enemy.Name = enemyInfo.Name;
                    enemy.Position = enemyInfo.Position;
                    enemy.Velocity = enemyInfo.Velocity;
                    enemy.Health = enemyInfo.Health;
                    enemy.MaxHealth = enemyInfo.MaxHealth;
                    enemy.Damage = enemyInfo.Damage;
                    enemy.Speed = enemyInfo.Speed;
                    enemy.Armor = enemyInfo.Armor;
                    enemy.IsActive = enemyInfo.IsActive;
                    enemy.SourceWave = enemyInfo.SourceWave;
                    enemy.SpawnTime = (float)(enemyInfo.SpawnTime - DateTime.Now).TotalSeconds;
                    // enemy.DeathTime = enemyInfo.DeathTime.HasValue ? (float?)(enemyInfo.DeathTime.Value - DateTime.Now).TotalSeconds : null; // Read-only property
                    enemy.PathProgress = enemyInfo.PathProgress;
                }

                return enemy;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating enemy from save info: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Create an enemy by type.
        /// </summary>
        private Enemy CreateEnemyByType(ZombieType enemyType)
        {
            // Implementation would create appropriate enemy based on type
            switch (enemyType)
            {
                case ZombieType.Swarm:
                    return new Enemy();
                case ZombieType.Sprinter:
                    return new Enemy();
                case ZombieType.Shadow:
                    return new Enemy();
                case ZombieType.Bloater:
                    return new Enemy();
                case ZombieType.Mamushka:
                    return new Enemy();
                case ZombieType.Ruin:
                    return new Enemy();
                case ZombieType.Devastator:
                    return new Enemy();
                case ZombieType.RobotClown:
                    return new Enemy(); // TODO: Implement RobotClownZombie
                default:
                    return new Enemy();
            }
        }

        #endregion
    }

    /// <summary>
    /// Individual enemy save information.
    /// </summary>
    public class EnemySaveInfo
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Velocity { get; set; }
        public float Health { get; set; }
        public float MaxHealth { get; set; }
        public float Damage { get; set; }
        public float Speed { get; set; }
        public float Armor { get; set; }
        public bool IsActive { get; set; }
        public int SourceWave { get; set; }
        public DateTime SpawnTime { get; set; }
        public DateTime? DeathTime { get; set; }
        public float Lifetime { get; set; }
        public float PathProgress { get; set; }
        public List<string> Path { get; set; }
        public Dictionary<string, object> CustomProperties { get; set; }

        public EnemySaveInfo()
        {
            CustomProperties = new Dictionary<string, object>();
            Path = new List<string>();
        }
    }

    /// <summary>
    /// Enemy statistics for save data.
    /// </summary>
    public class EnemyStatistics
    {
        public int TotalKills { get; set; }
        public float DamageDealt { get; set; }
        public TimeSpan Lifetime { get; set; }
        public float Accuracy { get; set; }
        public float Speed { get; set; }
        public float Armor { get; set; }
        public int ShotsFired { get; set; }
        public int ShotsHit { get; set; }
        public float CriticalHits { get; set; }
        public float DamageTaken { get; set; }
        public float RangeEfficiency { get; set; }
        public float CostEfficiency { get; set; }

        public EnemyStatistics()
        {
            CreatedTime = DateTime.Now;
        }

        public DateTime CreatedTime { get; set; }
    }

    /// <summary>
    /// Enemy manager for handling enemy operations.
    /// </summary>
    public class EnemyManager
    {
        private static EnemyManager _instance;
        public static EnemyManager Instance => _instance ??= new EnemyManager();

        private readonly List<Enemy> _enemies = new();

        public void ClearAllEnemies() => _enemies.Clear();
        public void AddEnemy(Enemy enemy) => _enemies.Add(enemy);
        public Enemy GetEnemy(string id) => _enemies.FirstOrDefault(e => e.Id.ToString() == id);
        public Enemy GetEnemy(uint id) => _enemies.FirstOrDefault(e => e.Id == id);
        public List<Enemy> GetAllEnemies() => new List<Enemy>(_enemies);
        public int GetTotalSpawned() => _enemies.Count;
        public int GetTotalKilled() => _enemies.Count(e => e.Health <= 0);
        public int GetTotalEscaped() => 0;
        public int GetActiveEnemyCount() => _enemies.Count(e => e.Health > 0);
    }
}
