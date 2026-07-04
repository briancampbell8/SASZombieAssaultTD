using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.Utility;
using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.Audio;
using SASZombieAssaultTD.Engine.Resources;
using SASZombieAssaultTD.Engine.Towers;
using SASZombieAssaultTD.Engine.Extensions;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Towers.TowerControl
{
    ///<summary>
    ///Neural database for SAS Zombie Assault TD tower intelligence systems.
    ///Manages enhancement data, neural pathways, and persistence.
    ///</summary>
    public class NeuralDatabase
    {
        private readonly TowerType _towerType;
        private readonly List<TowerUpgrade> _upgrades;
        private readonly Dictionary<int, TowerUpgrade> _upgradeLevels;
        private readonly Dictionary<SASZombieAssaultTD.Engine.Towers.UpgradeType, List<TowerUpgrade>> _upgradeTypes;
        private readonly string _dataPath;
        private bool _isLoaded;

        //Properties
        public TowerType TowerType => _towerType;
        public int TotalUpgrades => _upgrades.Count;
        public int MaxLevel => _upgradeLevels.Keys.Count > 0 ? _upgradeLevels.Keys.Max() : 1;
        public bool IsLoaded => _isLoaded;

        public NeuralDatabase(TowerType towerType)
        {
            _towerType = towerType;
            _upgrades = new List<TowerUpgrade>();
            _upgradeLevels = new Dictionary<int, TowerUpgrade>();
            _upgradeTypes = new Dictionary<SASZombieAssaultTD.Engine.Towers.UpgradeType, List<TowerUpgrade>>();
            _dataPath = Path.Combine("Data", "Upgrades", $"{towerType}.json");
        }

        ///<summary>
        ///Initialize the upgrade database.
        ///</summary>
        public void Initialize()
        {
            if (_isLoaded) return;

            System.Diagnostics.Debug.WriteLine($"Initializing upgrade database for {_towerType}");

            try
            {
                //Load upgrades from file or create defaults
                LoadUpgrades();

                //Build lookup tables
                BuildLookupTables();

                _isLoaded = true;
                System.Diagnostics.Debug.WriteLine($"Upgrade database initialized for {_towerType} with {_upgrades.Count} upgrades");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to initialize upgrade database for {_towerType}: {ex.Message}");
                throw;
            }
        }

        ///<summary>
        ///Get upgrade path for the tower.
        ///</summary>
        ///<returns>Upgrade path.</returns>
        public List<TowerUpgrade> GetUpgradePath()
        {
            return new List<TowerUpgrade>(_upgrades);
        }

        ///<summary>
        ///Get upgrade by level.
        ///</summary>
        ///<param name="level">Upgrade level.</param>
        ///<returns>Upgrade at specified level.</returns>
        public TowerUpgrade GetUpgradeByLevel(int level)
        {
            return _upgradeLevels.TryGetValue(level, out var upgrade) ? upgrade : null;
        }

        ///<summary>
        ///Get upgrades by type.
        ///</summary>
        ///<param name="upgradeType">Upgrade type.</param>
        ///<returns>List of upgrades of specified type.</returns>
        public List<TowerUpgrade> GetUpgradesByType(SASZombieAssaultTD.Engine.Towers.UpgradeType upgradeType)
        {
            return _upgradeTypes.TryGetValue(upgradeType, out var upgrades) ? upgrades : new List<TowerUpgrade>();
        }

        ///<summary>
        ///Get upgrade by name.
        ///</summary>
        ///<param name="name">Upgrade name.</param>
        ///<returns>Upgrade with specified name.</returns>
        public TowerUpgrade GetUpgradeByName(string name)
        {
            return _upgrades.FirstOrDefault(u => u.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        ///<summary>
        ///Get available upgrades for a tower level.
        ///</summary>
        ///<param name="towerLevel">Current tower level.</param>
        ///<param name="playerLevel">Current player level.</param>
        ///<returns>List of available upgrades.</returns>
        public List<TowerUpgrade> GetAvailableUpgrades(int towerLevel, int playerLevel)
        {
            return _upgrades.Where(u => u.IsAvailable).ToList();
        }

        ///<summary>
        ///Get next upgrade in the path.
        ///</summary>
        ///<param name="currentLevel">Current upgrade level.</param>
        ///<returns>Next upgrade or null if at max level.</returns>
        public TowerUpgrade GetNextUpgrade(int currentLevel)
        {
            return _upgradeLevels.TryGetValue(currentLevel + 1, out var upgrade) ? upgrade : null;
        }

        ///<summary>
        ///Get upgrade cost for a level.
        ///</summary>
        ///<param name="level">Upgrade level.</param>
        ///<returns>Upgrade cost.</returns>
        public int GetUpgradeCost(int level)
        {
            var upgrade = GetUpgradeByLevel(level);
            return upgrade?.Cost ?? 0;
        }

        ///<summary>
        ///Get total cost for all upgrades.
        ///</summary>
        ///<returns>Total cost of all upgrades.</returns>
        public int GetTotalUpgradeCost()
        {
            return _upgrades.Sum(u => u.Cost);
        }

        ///<summary>
        ///Get upgrade statistics.
        ///</summary>
        ///<returns>Upgrade statistics.</returns>
        public UpgradeDatabaseStatistics GetStatistics()
        {
            return new UpgradeDatabaseStatistics
            {
                TowerType = _towerType,
                TotalUpgrades = TotalUpgrades,
                MaxLevel = MaxLevel,
                TotalCost = GetTotalUpgradeCost(),
                AverageCost = TotalUpgrades > 0 ? GetTotalUpgradeCost() / TotalUpgrades : 0,
                UpgradeTypes = _upgradeTypes.Keys.ToList(),
                MostExpensiveUpgrade = _upgrades.OrderByDescending(u => u.Cost).FirstOrDefault(),
                CheapestUpgrade = _upgrades.OrderBy(u => u.Cost).FirstOrDefault(),
                MostPowerfulUpgrade = _upgrades.OrderByDescending(u => u.GetPowerRating()).FirstOrDefault()
            };
        }

        ///<summary>
        ///Add an upgrade to the database.
        ///</summary>
        ///<param name="upgrade">Upgrade to add.</param>
        ///<returns>True if upgrade was added.</returns>
        public bool AddUpgrade(TowerUpgrade upgrade)
        {
            if (upgrade == null) return false;
            if (upgrade.TowerType != _towerType) return false;
            if (_upgradeLevels.ContainsKey(upgrade.Level)) return false;

            _upgrades.Add(upgrade);
            _upgradeLevels[upgrade.Level] = upgrade;

            //Add to type lookup
            if (!_upgradeTypes.ContainsKey(upgrade.Type))
            {
                _upgradeTypes[upgrade.Type] = new List<TowerUpgrade>();
            }
            _upgradeTypes[upgrade.Type].Add(upgrade);

            System.Diagnostics.Debug.WriteLine($"Added upgrade: {upgrade.Name} (Level {upgrade.Level}) to {_towerType}");
            return true;
        }

        ///<summary>
        ///Remove an upgrade from the database.
        ///</summary>
        ///<param name="upgrade">Upgrade to remove.</param>
        ///<returns>True if upgrade was removed.</returns>
        public bool RemoveUpgrade(TowerUpgrade upgrade)
        {
            if (upgrade == null) return false;

            var removed = _upgrades.Remove(upgrade);
            if (removed)
            {
                _upgradeLevels.Remove(upgrade.Level);

                if (_upgradeTypes.ContainsKey(upgrade.Type))
                {
                    _upgradeTypes[upgrade.Type].Remove(upgrade);
                    if (_upgradeTypes[upgrade.Type].Count == 0)
                    {
                        _upgradeTypes.Remove(upgrade.Type);
                    }
                }

                System.Diagnostics.Debug.WriteLine($"Removed upgrade: {upgrade.Name} from {_towerType}");
            }

            return removed;
        }

        ///<summary>
        ///Update an upgrade in the database.
        ///</summary>
        ///<param name="upgrade">Upgrade to update.</param>
        ///<returns>True if upgrade was updated.</returns>
        public bool UpdateUpgrade(TowerUpgrade upgrade)
        {
            if (upgrade == null) return false;
            if (upgrade.TowerType != _towerType) return false;

            var existingUpgrade = GetUpgradeByLevel(upgrade.Level);
            if (existingUpgrade == null) return false;

            //Remove old upgrade
            RemoveUpgrade(existingUpgrade);

            //Add updated upgrade
            return AddUpgrade(upgrade);
        }

        ///<summary>
        ///Validate upgrade database.
        ///</summary>
        ///<returns>Validation result.</returns>
        public ValidationResult ValidateDatabase()
        {
            var result = new ValidationResult { IsValid = true };

            //Check for duplicate levels
            var levelCounts = _upgrades.GroupBy(u => u.Level).ToDictionary(g => g.Key, g => g.Count());
            foreach (var kvp in levelCounts)
            {
                if (kvp.Value > 1)
                {
                    result.IsValid = false;
                    result.AddError($"Duplicate upgrade level: {kvp.Key} ({kvp.Value} upgrades)");
                }
            }

            //Check for missing levels
            var maxLevel = MaxLevel;
            for (int i = 1; i <= maxLevel; i++)
            {
                if (!_upgradeLevels.ContainsKey(i))
                {
                    result.AddWarning($"Missing upgrade level: {i}");
                }
            }

            //Validate each upgrade
            foreach (var upgrade in _upgrades)
            {
                var upgradeResult = ValidateUpgrade(upgrade);
                if (!upgradeResult.IsValid)
                {
                    result.IsValid = false;
                    result.AddError($"Upgrade {upgrade.Name}: {string.Join("; ", upgradeResult.Errors)}");
                }
            }

            return result;
        }

        ///<summary>
        ///Save upgrade database to file.
        ///</summary>
        ///<returns>True if saved successfully.</returns>
        public bool SaveDatabase()
        {
            try
            {
                //Create directory if it doesn't exist
                Directory.CreateDirectory(Path.GetDirectoryName(_dataPath));

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var json = JsonSerializer.Serialize(_upgrades, options);
                File.WriteAllText(_dataPath, json);

                System.Diagnostics.Debug.WriteLine($"Saved upgrade database for {_towerType} to {_dataPath}");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving upgrade database for {_towerType}: {ex.Message}");
                return false;
            }
        }

        ///<summary>
        ///Load upgrade database from file.
        ///</summary>
        ///<returns>True if loaded successfully.</returns>
        public bool LoadDatabase()
        {
            try
            {
                if (!File.Exists(_dataPath))
                {
                    System.Diagnostics.Debug.WriteLine($"Upgrade database file not found for {_towerType}, creating defaults");
                    CreateDefaultUpgrades();
                    return SaveDatabase();
                }

                var json = File.ReadAllText(_dataPath);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var upgrades = JsonSerializer.Deserialize<List<TowerUpgrade>>(json, options);
                if (upgrades == null) return false;

                _upgrades.Clear();
                _upgradeLevels.Clear();
                _upgradeTypes.Clear();

                foreach (var upgrade in upgrades)
                {
                    AddUpgrade(upgrade);
                }

                _isLoaded = true;
                System.Diagnostics.Debug.WriteLine($"Loaded upgrade database for {_towerType} from {_dataPath}");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading upgrade database for {_towerType}: {ex.Message}");
                return false;
            }
        }

        ///<summary>
        ///Export upgrade database to JSON.
        ///</summary>
        ///<returns>JSON string.</returns>
        public string ExportToJson()
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                return JsonSerializer.Serialize(_upgrades, options);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error exporting upgrade database for {_towerType}: {ex.Message}");
                return string.Empty;
            }
        }

        ///<summary>
        ///Import upgrade database from JSON.
        ///</summary>
        ///<param name="json">JSON string to import.</param>
        ///<returns>True if imported successfully.</returns>
        public bool ImportFromJson(string json)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var upgrades = JsonSerializer.Deserialize<List<TowerUpgrade>>(json, options);
                if (upgrades == null) return false;

                _upgrades.Clear();
                _upgradeLevels.Clear();
                _upgradeTypes.Clear();

                foreach (var upgrade in upgrades)
                {
                    AddUpgrade(upgrade);
                }

                _isLoaded = true;
                System.Diagnostics.Debug.WriteLine($"Imported upgrade database for {_towerType} from JSON");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error importing upgrade database for {_towerType}: {ex.Message}");
                return false;
            }
        }

        ///<summary>
        ///Clear all upgrades.
        ///</summary>
        public void ClearUpgrades()
        {
            _upgrades.Clear();
            _upgradeLevels.Clear();
            _upgradeTypes.Clear();
            _isLoaded = false;

            System.Diagnostics.Debug.WriteLine($"Cleared all upgrades for {_towerType}");
        }

        /// Private Methods

        ///<summary>
        ///Load upgrades from file or create defaults.
        ///</summary>
        private void LoadUpgrades()
        {
            if (!LoadDatabase())
            {
                CreateDefaultUpgrades();
            }
        }

        ///<summary>
        ///Create default upgrades for the tower type.
        ///</summary>
        private void CreateDefaultUpgrades()
        {
            CreateGenericUpgrades();
        }

        ///<summary>
        ///Create generic upgrades for unknown tower types.
        ///</summary>
        private void CreateGenericUpgrades()
        {
            for (int i = 1; i <= 5; i++)
            {
                var upgrade = NeuralTowerUpgrade.Create(i, $"Upgrade {i}", $"Generic upgrade level {i}", 100 * i, _towerType, UpgradeType.Damage);
                upgrade.SetVisualProperties(null, Color.White, null, null);
                AddUpgrade(upgrade);
            }
        }

        ///<summary>
        ///Build lookup tables for faster access.
        ///</summary>
        private void BuildLookupTables()
        {
            _upgradeLevels.Clear();
            _upgradeTypes.Clear();

            foreach (var upgrade in _upgrades)
            {
                _upgradeLevels[upgrade.Level] = upgrade;

                if (!_upgradeTypes.ContainsKey(upgrade.Type))
                {
                    _upgradeTypes[upgrade.Type] = new List<TowerUpgrade>();
                }
                _upgradeTypes[upgrade.Type].Add(upgrade);
            }
        }

        ///<summary>
        ///Validate a single upgrade.
        ///</summary>
        private ValidationResult ValidateUpgrade(TowerUpgrade upgrade)
        {
            var result = new ValidationResult { IsValid = true };

            if (upgrade.Level <= 0)
            {
                result.IsValid = false;
                result.AddError("Upgrade level must be positive");
            }

            if (upgrade.Cost < 0)
            {
                result.IsValid = false;
                result.AddError("Upgrade cost cannot be negative");
            }

            if (string.IsNullOrEmpty(upgrade.Name))
            {
                result.IsValid = false;
                result.AddError("Upgrade name cannot be empty");
            }

            if (upgrade.TowerType != _towerType)
            {
                result.IsValid = false;
                result.AddError("Upgrade tower type mismatch");
            }

            return result;
        }

        ///
    }

    ///<summary>
    ///Upgrade database statistics.
    ///</summary>
    public class UpgradeDatabaseStatistics
    {
        public TowerType TowerType { get; set; }
        public int TotalUpgrades { get; set; }
        public int MaxLevel { get; set; }
        public int TotalCost { get; set; }
        public int AverageCost { get; set; }
        public List<SASZombieAssaultTD.Engine.Towers.UpgradeType> UpgradeTypes { get; set; }
        public TowerUpgrade MostExpensiveUpgrade { get; set; }
        public TowerUpgrade CheapestUpgrade { get; set; }
        public TowerUpgrade MostPowerfulUpgrade { get; set; }

        public override string ToString()
        {
            return $"Upgrade Database Statistics for {TowerType}:\n" +
                   $"Total Upgrades: {TotalUpgrades}\n" +
                   $"Max Level: {MaxLevel}\n" +
                   $"Total Cost: ${TotalCost}\n" +
                   $"Average Cost: ${AverageCost}\n" +
                   $"Upgrade Types: {string.Join(", ", UpgradeTypes)}\n" +
                   $"Most Expensive: {MostExpensiveUpgrade?.Name ?? "None"}\n" +
                   $"Cheapest: {CheapestUpgrade?.Name ?? "None"}\n" +
                   $"Most Powerful: {MostPowerfulUpgrade?.Name ?? "None"}";
        }
    }
}
