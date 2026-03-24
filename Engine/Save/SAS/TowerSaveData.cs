using System;
using System.Collections.Generic;
using System.Linq;
using SASZombieAssaultTD.Engine.VectorMath;
using SASZombieAssaultTD.Engine.Towers;
using SASZombieAssaultTD.Engine.Gameplay.Towers;
using SASZombieAssaultTD.Engine.Gameplay.Items;
using Tower = SASZombieAssaultTD.Engine.Towers.Tower;
// Alias to disambiguate TowerType between different namespaces
using TowerTypeAlias = SASZombieAssaultTD.Engine.Dictionary.TowerType;
using SASZombieAssaultTD.Engine.Extensions;

namespace SASZombieAssaultTD.Engine.Save.SAS
{
    /// <summary>
    /// Tower save data container for SAS TD.
    /// Handles serialization and deserialization of tower state.
    /// </summary>
    public class TowerSaveData
    {
        // Basic tower information
        public int TowerCount { get; set; }
        public int TotalValue { get; set; }
        public DateTime LastUpdated { get; set; }

        // Tower collection data
        public List<TowerSaveInfo> Towers { get; set; }
        public Dictionary<string, int> TowerLevels { get; set; }
        public Dictionary<string, Vector3> TowerPositions { get; set; }
        public List<string> TowerTypes { get; set; }

        // Tower upgrade data
        public Dictionary<string, List<string>> TowerUpgrades { get; set; }
        public Dictionary<string, int> TowerUpgradeLevels { get; set; }
        public Dictionary<string, List<string>> TowerAbilities { get; set; }

        // Tower statistics
        public Dictionary<string, TowerStatistics> TowerStats { get; set; }
        public Dictionary<string, int> TowerKills { get; set; }
        public Dictionary<string, float> TowerDamage { get; set; }
        public Dictionary<string, TimeSpan> TowerUptime { get; set; }

        // Custom tower data
        public Dictionary<string, Dictionary<string, object>> CustomData { get; set; }

        public TowerSaveData()
        {
            Towers = new List<TowerSaveInfo>();
            TowerLevels = new Dictionary<string, int>();
            TowerPositions = new Dictionary<string, Vector3>();
            TowerTypes = new List<string>();
            TowerUpgrades = new Dictionary<string, List<string>>();
            TowerUpgradeLevels = new Dictionary<string, int>();
            TowerAbilities = new Dictionary<string, List<string>>();
            TowerStats = new Dictionary<string, TowerStatistics>();
            TowerKills = new Dictionary<string, int>();
            TowerDamage = new Dictionary<string, float>();
            TowerUptime = new Dictionary<string, TimeSpan>();
            CustomData = new Dictionary<string, Dictionary<string, object>>();
        }

        /// <summary>
        /// Validate tower save data.
        /// </summary>
        /// <returns>True if data is valid.</returns>
        public bool Validate()
        {
            if (TowerCount < 0)
                return false;

            if (TotalValue < 0)
                return false;

            if (LastUpdated == default)
                return false;

            // Validate tower collection
            if (Towers == null || Towers.Count != TowerCount)
                return false;

            // Validate tower levels
            if (TowerLevels == null)
                return false;

            // Validate tower positions
            if (TowerPositions == null)
                return false;

            // Validate tower types
            if (TowerTypes == null)
                return false;

            return true;
        }

        /// <summary>
        /// Clone this tower save data.
        /// </summary>
        /// <returns>Cloned data.</returns>
        public TowerSaveData Clone()
        {
            return new TowerSaveData
            {
                TowerCount = this.TowerCount,
                TotalValue = this.TotalValue,
                LastUpdated = this.LastUpdated,
                Towers = new List<TowerSaveInfo>(this.Towers),
                TowerLevels = new Dictionary<string, int>(this.TowerLevels),
                TowerPositions = new Dictionary<string, Vector3>(this.TowerPositions),
                TowerTypes = new List<string>(this.TowerTypes),
                TowerUpgrades = new Dictionary<string, List<string>>(this.TowerUpgrades),
                TowerUpgradeLevels = new Dictionary<string, int>(this.TowerUpgradeLevels),
                TowerAbilities = new Dictionary<string, List<string>>(this.TowerAbilities),
                TowerStats = new Dictionary<string, TowerStatistics>(this.TowerStats),
                TowerKills = new Dictionary<string, int>(this.TowerKills),
                TowerDamage = new Dictionary<string, float>(this.TowerDamage),
                TowerUptime = new Dictionary<string, TimeSpan>(this.TowerUptime),
                CustomData = new Dictionary<string, Dictionary<string, object>>(this.CustomData)
            };
        }

        /// <summary>
        /// Apply tower save data to current game state.
        /// </summary>
        /// <returns>True if applied successfully.</returns>
        public bool ApplyToGame(Tower tower, Vector3 position1)
        {
            try
            {
                var towerRegistry = TowerRegistry.Instance;
                if (towerRegistry == null)
                    return false;

                // Clear existing towers
                towerRegistry.ClearAllTowers();

                // Restore towers
                foreach (var towerInfo in Towers)
                {
                    var towerF = CreateTowerFromSaveInfo(towerInfo);
                    if (tower != null)
                    {
                        towerRegistry.AddTower(tower);
                    }
                }

                // Restore tower levels
                foreach (var kvp in TowerLevels)
                {
                    var towerId = kvp.Key;
                    var level = kvp.Value;
                    var towerF = towerRegistry.GetTower(towerId);
                    if (tower != null)
                    {
                        tower.Level = level;
                    }
                }

                // Restore tower positions
                foreach (var kvp in TowerPositions)
                {
                    var towerId = kvp.Key;
                    var position = kvp.Value;
                    var towerF = towerRegistry.GetTower(towerId);
                    if (tower != null)
                    {
                        position1 = position;
                    }
                }

                // Restore tower upgrades
                foreach (var kvp in TowerUpgrades)
                {
                    var towerId = kvp.Key;
                    var upgrades = kvp.Value;
                    var towerF = towerRegistry.GetTower(towerId);
                    if (tower != null)
                    {
                        foreach (var upgradeName in upgrades)
                        {
                            // Apply upgrade logic here
                        }
                    }
                }

                // Restore tower abilities
                foreach (var kvp in TowerAbilities)
                {
                    var towerId = kvp.Key;
                    var abilities = kvp.Value;
                    var towerF = towerRegistry.GetTower(towerId);
                    if (tower != null)
                    {
                        foreach (var ability in abilities)
                        {
                            tower.AddSpecialAbility(ability);
                        }
                    }
                }

                // Restore tower statistics
                foreach (var kvp in TowerStats)
                {
                    var towerId = kvp.Key;
                    var stats = kvp.Value;
                    var towerF = towerRegistry.GetTower(towerId);
                    if (tower != null)
                    {
                        tower.TotalKills = stats.TotalKills;
                        tower.DamageDealt = stats.TotalDamage;
                        tower.Uptime = (float)stats.Uptime.TotalSeconds;
                    }
                }

                Console.WriteLine($"Applied tower save data: {TowerCount} towers");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error applying tower save data: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Capture current tower state.
        /// </summary>
        /// <returns>Captured tower save data.</returns>
        public static TowerSaveData CaptureCurrentState()
        {
            var towerRegistry = TowerRegistry.Instance;
            if (towerRegistry == null)
                return new TowerSaveData();

            var saveData = new TowerSaveData
            {
                TowerCount = towerRegistry.GetTowerCount(),
                TotalValue = towerRegistry.GetTotalTowerValue(),
                LastUpdated = DateTime.Now
            };

            // Capture tower information
            var towers = towerRegistry.GetAllTowers();
            foreach (var tower in towers)
            {
                var towerInfo = new TowerSaveInfo
                {
                    Id = tower.Id.ToString(),
                    Type = tower.Type.ToString(),
                    Name = tower.Name,
                    Level = tower.Level,
                    Position = tower.Position,
                    Rotation = new Vector3(0f, tower.Rotation, 0f),
                    Scale = new Vector3(tower.Scale, tower.Scale, 1f),
                    Cost = tower.Cost,
                    Health = tower.Health,
                    MaxHealth = tower.MaxHealth,
                    Damage = (float)tower.Damage,
                    Range = (float)tower.Range,
                    FireRate = tower.FireRate,
                    Speed = tower.Speed,
                    IsActive = tower.IsActive
                };

                saveData.Towers.Add(towerInfo);

                // Add to collections
                saveData.TowerLevels[tower.Id.ToString()] = tower.Level;
                saveData.TowerPositions[tower.Id.ToString()] = tower.Position;

                if (!saveData.TowerTypes.Contains(tower.Type.ToString()))
                {
                    saveData.TowerTypes.Add(tower.Type.ToString());
                }

                // Capture upgrades
                var upgrades = tower.GetAvailableUpgrades();
                if (upgrades.Count > 0)
                {
                    saveData.TowerUpgrades[tower.Id.ToString()] = upgrades.Select(u => u.Name).ToList();
                    saveData.TowerUpgradeLevels[tower.Id.ToString()] = tower.Level;
                }

                // Capture abilities
                var abilities = tower.GetSpecialAbilities();
                if (abilities.Count > 0)
                {
                    saveData.TowerAbilities[tower.Id.ToString()] = abilities;
                }

                // Capture statistics
                saveData.TowerStats[tower.Id.ToString()] = new TowerStatistics
                {
                    TotalKills = tower.TotalKills,
                    TotalDamage = tower.DamageDealt,
                    Uptime = TimeSpan.FromSeconds(tower.Uptime),
                    Accuracy = tower.Accuracy,
                    DPS = tower.DPS
                };

                saveData.TowerKills[tower.Id.ToString()] = tower.TotalKills;
                saveData.TowerDamage[tower.Id.ToString()] = tower.DamageDealt;
                saveData.TowerUptime[tower.Id.ToString()] = TimeSpan.FromSeconds(tower.Uptime);
            }

            return saveData;
        }

        /// <summary>
        /// Get tower save information by ID.
        /// </summary>
        /// <param name="towerId">Tower ID.</param>
        /// <returns>Tower save info, or null if not found.</returns>
        public TowerSaveInfo GetTowerInfo(string towerId)
        {
            return Towers.FirstOrDefault(t => t.Id == towerId);
        }

        /// <summary>
        /// Get tower level by ID.
        /// </summary>
        /// <param name="towerId">Tower ID.</param>
        /// <returns>Tower level, or 0 if not found.</returns>
        public int GetTowerLevel(string towerId)
        {
            return TowerLevels.TryGetValue(towerId, out var level) ? level : 0;
        }

        /// <summary>
        /// Get tower position by ID.
        /// </summary>
        /// <param name="towerId">Tower ID.</param>
        /// <returns>Tower position, or Vector3.Zero if not found.</returns>
        public Vector3 GetTowerPosition(string towerId)
        {
            return TowerPositions.TryGetValue(towerId, out var position) ? position : Vector3.Zero;
        }

        /// <summary>
        /// Get tower upgrades by ID.
        /// </summary>
        /// <param name="towerId">Tower ID.</param>
        /// <returns>List of upgrade names.</returns>
        public List<string> GetTowerUpgrades(string towerId)
        {
            return TowerUpgrades.TryGetValue(towerId, out var upgrades) ? upgrades : new List<string>();
        }

        /// <summary>
        /// Get tower abilities by ID.
        /// </summary>
        /// <param name="towerId">Tower ID.</param>
        /// <returns>List of ability names.</returns>
        public List<string> GetTowerAbilities(string towerId)
        {
            return TowerAbilities.TryGetValue(towerId, out var abilities) ? abilities : new List<string>();
        }

        /// <summary>
        /// Get tower statistics by ID.
        /// </summary>
        /// <param name="towerId">Tower ID.</param>
        /// <returns>Tower statistics, or null if not found.</returns>
        public TowerStatistics GetTowerStatistics(string towerId)
        {
            return TowerStats.TryGetValue(towerId, out var stats) ? stats : null;
        }

        /// <summary>
        /// Add or update tower statistics.
        /// </summary>
        /// <param name="towerId">Tower ID.</param>
        /// <param name="statistics">Tower statistics.</param>
        public void SetTowerStatistics(string towerId, TowerStatistics statistics)
        {
            TowerStats[towerId] = statistics;
        }

        /// <summary>
        /// Add custom data for a tower.
        /// </summary>
        /// <param name="towerId">Tower ID.</param>
        /// <param name="key">Data key.</param>
        /// <param name="value">Data value.</param>
        public void SetCustomData(string towerId, string key, object value)
        {
            if (!CustomData.ContainsKey(towerId))
            {
                CustomData[towerId] = new Dictionary<string, object>();
            }
            CustomData[towerId][key] = value;
        }

        /// <summary>
        /// Get custom data for a tower.
        /// </summary>
        /// <param name="towerId">Tower ID.</param>
        /// <param name="key">Data key.</param>
        /// <returns>Data value, or null if not found.</returns>
        public object GetCustomData(string towerId, string key)
        {
            return CustomData.TryGetValue(towerId, out var data) && data.TryGetValue(key, out var value) ? value : null;
        }

        /// <summary>
        /// Get save summary.
        /// </summary>
        /// <returns>Save summary string.</returns>
        public string GetSummary()
        {
            return $"Tower Save Data:\n" +
                   $"Total Towers: {TowerCount}\n" +
                   $"Total Value: ${TotalValue}\n" +
                   $"Tower Types: {string.Join(", ", TowerTypes)}\n" +
                   $"Last Updated: {LastUpdated:yyyy-MM-dd HH:mm:ss}\n" +
                   $"Average Level: {(TowerLevels.Count > 0 ? TowerLevels.Values.Average() : 0):F1}\n" +
                   $"Total Upgrades: {TowerUpgrades.Values.Sum(u => u.Count)}\n" +
                   $"Total Abilities: {TowerAbilities.Values.Sum(a => a.Count)}";
        }

        #region Private Methods

        /// <summary>
        /// Create a tower from save information.
        /// </summary>
        private Tower CreateTowerFromSaveInfo(TowerSaveInfo towerInfo)
        {
            try
            {
                // Parse tower type (use alias to avoid ambiguous reference)
                if (Enum.TryParse<TowerTypeAlias>(towerInfo.Type, out var towerType))
                {
                    // Create tower based on type
                    var tower = CreateTowerByType(towerType);

                    if (tower != null)
                    {
                        // tower.Id = towerInfo.Id; // Read-only property
                        // tower.Name = towerInfo.Name; // Read-only property
                        tower.Level = towerInfo.Level;
                        // tower.Position = towerInfo.Position; // Read-only property
                        tower.Rotation = towerInfo.Rotation.Y;
                        tower.Scale = towerInfo.Scale.X;
                        // tower.Cost = towerInfo.Cost; // Read-only property
                        tower.Health = towerInfo.Health;
                        tower.MaxHealth = towerInfo.MaxHealth;
                        tower.Damage = towerInfo.Damage;
                        tower.Range = towerInfo.Range;
                        tower.FireRate = towerInfo.FireRate;
                        tower.Speed = towerInfo.Speed;
                        tower.IsActive = towerInfo.IsActive;
                    }

                    return tower;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating tower from save info: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Create a tower by type.
        /// </summary>
        private Tower CreateTowerByType(TowerTypeAlias towerType)
        {
            return NewMethod(towerType);

            static Tower NewMethod(TowerTypeAlias towerType)
            {
                // Implementation would create appropriate tower based on type
                switch (towerType)
                {
                    case TowerTypeAlias.VickersTurret:
                        return null;
                    case TowerTypeAlias.MGLTurret:
                        return null;
                    case TowerTypeAlias.SpecialTurret:
                        return null;
                    case TowerTypeAlias.SASSoldier:
                        return null;
                    case TowerTypeAlias.SniperSAS:
                        return null;
                    default:
                        return null;
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// Individual tower save information.
    /// </summary>
    public class TowerSaveInfo
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; }
        public int Cost { get; set; }
        public float Health { get; set; }
        public float MaxHealth { get; set; }
        public float Damage { get; set; }
        public float Range { get; set; }
        public float FireRate { get; set; }
        public float Speed { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime LastModified { get; set; }

        public TowerSaveInfo()
        {
            CreatedTime = DateTime.Now;
            LastModified = DateTime.Now;
        }
    }

    /// <summary>
    /// Tower statistics for save data.
    /// </summary>
    public class TowerStatistics
    {
        public int TotalKills { get; set; }
        public float TotalDamage { get; set; }
        public TimeSpan Uptime { get; set; }
        public float Accuracy { get; set; }
        public float DPS { get; set; }
        public int ShotsFired { get; set; }
        public int ShotsHit { get; set; }
        public float CriticalHits { get; set; }
        public float DamageDealt { get; set; }
        public float DamageTaken { get; set; }
        public float RangeEfficiency { get; set; }
        public float CostEfficiency { get; set; }

        public TowerStatistics()
        {
            CreatedTime = DateTime.Now;
        }

        public DateTime CreatedTime { get; set; }
    }
}
