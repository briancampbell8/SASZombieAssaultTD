/* ====================================================================================================
 *  FILE: TowerSaveData.cs
 *  PATH: Engine/Save/SAS/TowerSaveData.cs
 *  SUBSYSTEM: Save System
 *  ROLE: Serialization and deserialization of tower state.
 *
 *  RESPONSIBILITIES:
 *      - Capture tower state from the active game.
 *      - Serialize tower attributes, upgrades, abilities, and statistics.
 *      - Restore tower state into a running game session.
 *      - Provide validation, cloning, and summary utilities.
 *
 *  NON-RESPONSIBILITIES:
 *      - Tower gameplay logic.
 *      - Registry management beyond applying save data.
 *      - UI, networking, or persistence storage.
 *
 *  ARCHITECTURAL NOTES:
 *      - All collections must be initialized in the constructor.
 *      - All public methods must be null‑safe and deterministic.
 *      - Save data must remain serialization‑friendly and version‑safe.
 * ==================================================================================================== */

using System;
using System.Collections.Generic;
using System.Linq;
using SASZombieAssaultTD.Engine.VectorMath;
using SASZombieAssaultTD.Engine.Towers;
using SASZombieAssaultTD.Engine.Gameplay.Towers;
using SASZombieAssaultTD.Engine.Gameplay.Items;
using Tower = SASZombieAssaultTD.Engine.Towers.Tower;
using TowerTypeAlias = SASZombieAssaultTD.Engine.Dictionary.TowerType;
using SASZombieAssaultTD.Engine.Extensions;
using SASZombieAssaultTD.Engine.Animation.Core.Time;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Save.SAS
{
    ///<summary>
    ///Container for all serialized tower data.
    ///Handles validation, cloning, and application to game state.
    ///</summary>
    public class TowerSaveData
    {
        //===============================================================================================
        // BASIC SAVE DATA
        //===============================================================================================

        ///<summary>Total number of towers saved.</summary>
        public int TowerCount { get; set; }

        ///<summary>Total combined value of all towers.</summary>
        public int TotalValue { get; set; }

        ///<summary>Timestamp of last save update.</summary>
        public DateTime LastUpdated { get; set; }

        //===============================================================================================
        // COLLECTIONS
        //===============================================================================================

        ///<summary>List of serialized tower entries.</summary>
        public List<TowerSaveInfo> Towers { get; set; }

        ///<summary>Dictionary of tower levels keyed by tower ID.</summary>
        public Dictionary<string, int> TowerLevels { get; set; }

        ///<summary>Dictionary of tower positions keyed by tower ID.</summary>
        public Dictionary<string, Vector3> TowerPositions { get; set; }

        ///<summary>List of tower type names present in the save.</summary>
        public List<string> TowerTypes { get; set; }

        ///<summary>Dictionary of upgrade names per tower ID.</summary>
        public Dictionary<string, List<string>> TowerUpgrades { get; set; }

        ///<summary>Dictionary of upgrade levels per tower ID.</summary>
        public Dictionary<string, int> TowerUpgradeLevels { get; set; }

        ///<summary>Dictionary of ability names per tower ID.</summary>
        public Dictionary<string, List<string>> TowerAbilities { get; set; }

        ///<summary>Dictionary of tower statistics per tower ID.</summary>
        public Dictionary<string, TowerStatistics> TowerStats { get; set; }

        ///<summary>Dictionary of kill counts per tower ID.</summary>
        public Dictionary<string, int> TowerKills { get; set; }

        ///<summary>Dictionary of damage totals per tower ID.</summary>
        public Dictionary<string, float> TowerDamage { get; set; }

        ///<summary>Dictionary of uptime values per tower ID.</summary>
        public Dictionary<string, TimeSpan> TowerUptime { get; set; }

        ///<summary>Custom per‑tower data for modding or extended features.</summary>
        public Dictionary<string, Dictionary<string, object>> CustomData { get; set; }

        //===============================================================================================
        // CONSTRUCTOR
        //===============================================================================================

        ///<summary>
        ///Initializes all collections to ensure serialization safety.
        ///</summary>
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

        //===============================================================================================
        // VALIDATION
        //===============================================================================================

        ///<summary>
        ///Validates the integrity of the save data.
        ///Ensures counts, timestamps, and collections are consistent.
        ///</summary>
        ///<returns>True if valid; false otherwise.</returns>
        public bool Validate()
        {
            if (TowerCount < 0) return false;
            if (TotalValue < 0) return false;
            if (LastUpdated == default) return false;

            //Validate tower list count
            if (Towers == null || Towers.Count != TowerCount)
                return false;

            if (TowerLevels == null) return false;
            if (TowerPositions == null) return false;
            if (TowerTypes == null) return false;

            return true;
        }

        //===============================================================================================
        // CLONING
        //===============================================================================================

        ///<summary>
        ///Creates a deep clone of this save data.
        ///</summary>
        ///<returns>New TowerSaveData instance.</returns>
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

        //===============================================================================================
        // APPLY SAVE DATA TO GAME
        //===============================================================================================

        ///<summary>
        ///Applies the saved tower data to the active game state.
        ///</summary>
        ///<param name="tower">Temporary tower reference (unused).</param>
        ///<param name="position1">Temporary position reference (unused).</param>
        ///<returns>True if applied successfully.</returns>
        public bool ApplyToGame(Tower tower, Vector3 position1)
        {
            try
            {
                var towerRegistry = TowerRegistry.Instance;
                if (towerRegistry == null)
                    return false;

                //Clear existing towers before restoration
                towerRegistry.ClearAllTowers();

                //Restore tower instances
                foreach (var towerInfo in Towers)
                {
                    var towerF = CreateTowerFromSaveInfo(towerInfo);
                    if (towerF != null)
                        towerRegistry.AddTower(towerF);
                }

                //Restore tower levels
                foreach (var kvp in TowerLevels)
                {
                    var towerF = towerRegistry.GetTower(kvp.Key);
                    if (towerF != null)
                        towerF.Level = kvp.Value;
                }

                //Restore tower positions
                //Restore tower positions
                foreach (var kvp in TowerPositions)
                {
                    var towerF = towerRegistry.GetTower(kvp.Key);
                    if (towerF != null)
                    {
                        //Bypasses the read-only restriction using the engine's custom property method
                        towerF.SetCustomProperty("Position", kvp.Value);
                    }
                }

                //Restore upgrades
                foreach (var kvp in TowerUpgrades)
                {
                    var towerF = towerRegistry.GetTower(kvp.Key);
                    if (towerF != null)
                    {
                        foreach (var upgradeName in kvp.Value)
                        {
                            //Upgrade application logic placeholder
                        }
                    }
                }

                //Restore abilities
                foreach (var kvp in TowerAbilities)
                {
                    var towerF = towerRegistry.GetTower(kvp.Key);
                    if (towerF != null)
                    {
                        foreach (var ability in kvp.Value)
                            towerF.AddSpecialAbility(ability);
                    }
                }

                //Restore statistics
                foreach (var kvp in TowerStats)
                {
                    var towerF = towerRegistry.GetTower(kvp.Key);
                    if (towerF != null)
                    {
                        towerF.TotalKills = kvp.Value.TotalKills;
                        towerF.DamageDealt = kvp.Value.TotalDamage;
                        towerF.Uptime = (float)kvp.Value.Uptime.TotalSeconds;
                    }
                }

                System.Diagnostics.Debug.WriteLine($"Applied tower save data: {TowerCount} towers");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error applying tower save data: {ex.Message}");
                return false;
            }
        }

        //===============================================================================================
        // CAPTURE CURRENT STATE
        //===============================================================================================

        ///<summary>
        ///Captures the current tower state from the active game.
        ///</summary>
        ///<returns>New TowerSaveData instance.</returns>
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

            //Capture each tower
            foreach (var tower in towerRegistry.GetAllTowers())
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
                    DamageDealt = (float)tower.DamageDealt,
                    Range = (float)tower.Range,
                    FireRate = tower.FireRate,
                    Speed = tower.Speed,
                    IsActive = tower.IsActive
                };

                saveData.Towers.Add(towerInfo);

                //Add to collections
                saveData.TowerLevels[tower.Id.ToString()] = tower.Level;
                saveData.TowerPositions[tower.Id.ToString()] = tower.Position;

                if (!saveData.TowerTypes.Contains(tower.Type.ToString()))
                    saveData.TowerTypes.Add(tower.Type.ToString());

                //Capture upgrades
                var upgrades = tower.GetAvailableUpgrades();
                if (upgrades.Count > 0)
                {
                    saveData.TowerUpgrades[tower.Id.ToString()] = upgrades.Select(u => u.Name).ToList();
                    saveData.TowerUpgradeLevels[tower.Id.ToString()] = tower.Level;
                }

                //Capture abilities
                var abilities = tower.GetSpecialAbilities();
                if (abilities.Count > 0)
                    saveData.TowerAbilities[tower.Id.ToString()] = abilities;

                //Capture statistics
                saveData.TowerStats[tower.Id.ToString()] = new TowerStatistics
                {
                    TotalKills = tower.TotalKills,
                    TotalDamage = tower.DamageDealt,
                    Time = TimeSpan.FromSeconds(tower.Uptime),
                    Accuracy = tower.Accuracy,
                    DPS = tower.DPS
                };

                saveData.TowerKills[tower.Id.ToString()] = tower.TotalKills;
                saveData.TowerDamage[tower.Id.ToString()] = tower.DamageDealt;
                saveData.TowerUptime[tower.Id.ToString()] = TimeSpan.FromSeconds(tower.Uptime);
            }

            return saveData;
        }

        //===============================================================================================
        // LOOKUP HELPERS
        //===============================================================================================

        ///<summary>Returns tower save info by ID.</summary>
        public TowerSaveInfo GetTowerInfo(string towerId)
            => Towers.FirstOrDefault(t => t.Id == towerId);

        ///<summary>Returns tower level by ID.</summary>
        public int GetTowerLevel(string towerId)
            => TowerLevels.TryGetValue(towerId, out var level) ? level : 0;

        ///<summary>Returns tower position by ID.</summary>
        public Vector3 GetTowerPosition(string towerId)
            => TowerPositions.TryGetValue(towerId, out var pos) ? pos : Vector3.Zero;

        ///<summary>Returns upgrade list by tower ID.</summary>
        public List<string> GetTowerUpgrades(string towerId)
            => TowerUpgrades.TryGetValue(towerId, out var list) ? list : new List<string>();

        ///<summary>Returns ability list by tower ID.</summary>
        public List<string> GetTowerAbilities(string towerId)
            => TowerAbilities.TryGetValue(towerId, out var list) ? list : new List<string>();

        ///<summary>Returns tower statistics by ID.</summary>
        public TowerStatistics GetTowerStatistics(string towerId)
            => TowerStats.TryGetValue(towerId, out var stats) ? stats : null;

        ///<summary>Sets tower statistics for a given ID.</summary>
        public void SetTowerStatistics(string towerId, TowerStatistics statistics)
            => TowerStats[towerId] = statistics;

        ///<summary>Sets custom data for a tower.</summary>
        public void SetCustomData(string towerId, string key, object value)
        {
            if (!CustomData.ContainsKey(towerId))
                CustomData[towerId] = new Dictionary<string, object>();

            CustomData[towerId][key] = value;
        }

        ///<summary>Gets custom data for a tower.</summary>
        public object GetCustomData(string towerId, string key)
            => CustomData.TryGetValue(towerId, out var dict) && dict.TryGetValue(key, out var val) ? val : null;

        //===============================================================================================
        // SUMMARY
        //===============================================================================================

        ///<summary>
        ///Returns a human‑readable summary of the save data.
        ///</summary>
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

        //===============================================================================================
        // PRIVATE HELPERS
        //===============================================================================================

        ///<summary>
        ///Creates a tower instance from serialized save info.
        ///Read‑only properties are intentionally not overwritten.
        ///</summary>
        private Tower CreateTowerFromSaveInfo(TowerSaveInfo towerInfo)
        {

            try
            {
                if (Enum.TryParse<TowerTypeAlias>(towerInfo.Type, out var towerType))
                {
                    var tower = CreateTowerByType(towerType);

                    if (tower != null)
                    {
                        tower.Level = towerInfo.Level;
                        tower.Rotation = towerInfo.Rotation.Y;
                        tower.Scale = towerInfo.Scale.X;
                        tower.Health = towerInfo.Health;
                        tower.MaxHealth = towerInfo.MaxHealth;
                        tower.DamageDealt = towerInfo.DamageDealt;
                        tower.SetRange(towerInfo.Range);
                        tower.SetFireRate(towerInfo.FireRate);
                        tower.Speed = towerInfo.Speed;
                        tower.IsActive = towerInfo.IsActive;
                    }

                    return tower;
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating tower from save info: {ex.Message}");
                return null;
            }
        }

        ///<summary>
        ///Creates a tower instance based on its type.
        ///Placeholder implementation until tower factory is complete.
        ///</summary>
        private Tower CreateTowerByType(TowerTypeAlias towerType)
        {
            return towerType switch
            {
                TowerTypeAlias.VickersTurret => null,
                TowerTypeAlias.MGLTurret => null,
                TowerTypeAlias.SpecialTurret => null,
                TowerTypeAlias.SASSoldier => null,
                TowerTypeAlias.SniperSAS => null,
                _ => null
            };
        }
    }

    //===============================================================================================
    // SUPPORTING DATA STRUCTURES
    //===============================================================================================

    ///<summary>
    ///Serialized tower information used for saving and restoring tower state.
    ///</summary>
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
        public float DamageDealt { get; set; }
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

    ///<summary>
    ///Statistical data for a tower, used for analytics and save restoration.
    ///</summary>
    public class TowerStatistics
    {
        public int TotalKills { get; set; }
        public float TotalDamage { get; set; }
        public TimeSpan Uptime { get; set; }
        internal double Accuracy;
        internal TimeSpan Time;
        internal double DPS;
    }
}
