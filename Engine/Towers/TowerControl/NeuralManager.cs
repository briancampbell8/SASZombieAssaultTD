using SASZombieAssaultTD.Engine.Extensions;
using SASZombieAssaultTD.Engine.LevelUpControl;
using SASZombieAssaultTD.Engine.Managers;
using SASZombieAssaultTD.Engine.Towers;
using SASZombieAssaultTD.Engine.Towers.TowerControl;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SASZombieAssaultTD.Engine.Towers.TowerControl
{
    /// <summary>
    /// Tower upgrade database for managing upgrade data.
    /// </summary>
    public class TowerUpgradeDatabase
    {
        readonly Dictionary<TowerType, List<TowerUpgrade>> _upgrades;

        public TowerUpgradeDatabase() => _upgrades = new Dictionary<TowerType, List<TowerUpgrade>>();

        public List<TowerUpgrade> GetUpgrades(TowerType type)
        {
            return _upgrades.TryGetValue(type, out var upgrades) ? upgrades : new List<TowerUpgrade>();
        }

        public void AddUpgrade(TowerType type, TowerUpgrade upgrade)
        {
            if (!_upgrades.ContainsKey(type))
                _upgrades[type] = new List<TowerUpgrade>();

            _upgrades[type].Add(upgrade);
        }
    }

    /// <summary>
    /// Tower upgrade manager for managing upgrade operations.
    /// </summary>
    public class TowerUpgradeManager
    {
        private readonly Dictionary<Tower, List<TowerUpgrade>> _towerUpgrades = new();
        private Dictionary<string, TowerUpgrade> _upgrades;

        public TowerUpgradeManager() => _upgrades = new Dictionary<string, TowerUpgrade>();

        public TowerUpgrade GetUpgrade(string id)
        {
            return _upgrades.TryGetValue(id, out var upgrade) ? upgrade : null;
        }

        public void RegisterUpgrade(TowerUpgrade upgrade) => _upgrades[upgrade.Id] = upgrade;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal TowerUpgrade[] GetUpgrades(string id) => new[] { GetUpgrade(id) };

        internal object GetPurchasedUpgrades(Tower tower)
        {
            if (tower == null)
                return new List<TowerUpgrade>();

            // Check if we have upgrades recorded for this tower
            if (!_towerUpgrades.ContainsKey(tower))
            {
                // No upgrades found for this tower
                return new List<TowerUpgrade>();
            }

            return _towerUpgrades[tower];
        }
    }

    /// <summary>
    /// Neural manager for SAS Zombie Assault TD tower intelligence systems.
    /// Manages all tower enhancements, progression, and neural pathways.
    /// </summary>
    [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
    public class NeuralManager
    {
        readonly Dictionary<TowerType, List<TowerUpgrade>> _upgradePaths = new();
        readonly Dictionary<TowerType, TowerUpgradeDatabase> _upgradeDatabases = new();
        readonly Dictionary<string, TowerUpgrade> _purchasedUpgrades;
        readonly Dictionary<Tower, List<TowerUpgrade>> _towerUpgrades = new();
        bool _isInitialized;
        static NeuralManager _instance;

        // Events
        public event Action<TowerUpgrade> OnUpgradePurchased;
        public event Action<TowerUpgrade> OnUpgradeApplied;
        public event Action<TowerUpgrade> OnUpgradeRemoved;
        public event Action<Tower> OnTowerUpgraded;

        // Properties
        public bool IsInitialized => _isInitialized;
        public int TotalUpgradesPurchased => _purchasedUpgrades.Count;
        public int TotalUpgradeValue => _purchasedUpgrades.Values.Sum(u => u.Cost);

        // Singleton
        public static NeuralManager Instance => _instance ??= new NeuralManager();

        private NeuralManager()
        {
            _upgradePaths = new Dictionary<TowerType, List<TowerUpgrade>>();
            _upgradeDatabases = new Dictionary<TowerType, TowerUpgradeDatabase>();
            _purchasedUpgrades = new Dictionary<string, TowerUpgrade>();
            _towerUpgrades = new Dictionary<Tower, List<TowerUpgrade>>();
        }

        /// <summary>
        /// Initialize the upgrade manager.
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized) return;

            Console.WriteLine("Initializing Tower Upgrade Manager");

            try
            {
                // Initialize upgrade databases
                InitializeUpgradeDatabases();

                // Initialize upgrade paths
                InitializeUpgradePaths();

                _isInitialized = true;
                Console.WriteLine("Tower Upgrade Manager initialized successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to initialize Tower Upgrade Manager: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get available upgrades for a tower.
        /// </summary>
        /// <param name="tower">Tower to get upgrades for.</param>
        /// <returns>List of available upgrades.</returns>
        public List<TowerUpgrade> GetAvailableUpgrades(Tower tower)
        {
            if (tower == null) return new List<TowerUpgrade>();

            var availableUpgrades = new List<TowerUpgrade>();
            var towerType = tower.Type;
            var towerLevel = tower.Level;
            var playerLevel = PlayerLevel.Instance?.CurrentLevel ?? 1;

            if (_upgradePaths.TryGetValue(towerType, out var upgradePath))
            {
                foreach (var upgrade in upgradePath)
                {
                    if (upgrade.IsAvailable && !IsUpgradePurchased(tower, upgrade))
                    {
                        availableUpgrades.Add(upgrade);
                    }
                }
            }

            return availableUpgrades;
        }

        /// <summary>
        /// Get purchased upgrades for a tower.
        /// </summary>
        /// <param name="tower">Tower to get purchased upgrades for.</param>
        /// <returns>List of purchased upgrades.</returns>
        public List<TowerUpgrade> GetPurchasedUpgrades(Tower tower)
        {
            if (tower == null) return new List<TowerUpgrade>();

            return _towerUpgrades.TryGetValue(tower, out var upgrades) ? upgrades : new List<TowerUpgrade>();
        }

        /// <summary>
        /// Purchase an upgrade for a tower.
        /// </summary>
        /// <param name="tower">Tower to upgrade.</param>
        /// <param name="upgrade">Upgrade to purchase.</param>
        /// <returns>True if upgrade was purchased.</returns>
        public bool PurchaseUpgrade(Tower tower, TowerUpgrade upgrade)
        {
            if (tower == null || upgrade == null) return false;

            var playerCash = EconomyManager.Instance.CurrentCash;
            var towerLevel = tower.Level;

            if (!upgrade.CanPurchase(playerCash, towerLevel))
            {
                PlayErrorSound("PlayInsufficientFunds");
                return false;
            }

            // Purchase upgrade
            if (!upgrade.Purchase(ref playerCash))
            {
                return false;
            }

            // Apply upgrade to tower
            upgrade.ApplyToTower(tower);

            // Track purchased upgrade
            var upgradeKey = $"{tower.Type}_{upgrade.Level}_{upgrade.Name}";
            _purchasedUpgrades[upgradeKey] = upgrade;

            // Add to tower upgrades
            if (!_towerUpgrades.ContainsKey(tower))
            {
                _towerUpgrades[tower] = new List<TowerUpgrade>();
            }

            _towerUpgrades[tower].Add(upgrade);

            // Trigger events
            OnUpgradePurchased?.Invoke(upgrade);
            OnUpgradeApplied?.Invoke(upgrade);
            OnTowerUpgraded?.Invoke(tower);

            // Play success sound
            PlaySuccessSound("PlayUpgradePurchase");

            Console.WriteLine($"Purchased upgrade: {upgrade.Name} for {tower.Type}");
            return true;
        }

        /// <summary>
        /// Remove an upgrade from a tower.
        /// </summary>
        /// <param name="tower">Tower to remove upgrade from.</param>
        /// <param name="upgrade">Upgrade to remove.</param>
        /// <returns>True if upgrade was removed.</returns>
        public bool RemoveUpgrade(Tower tower, TowerUpgrade upgrade)
        {
            if (tower == null || upgrade == null) return false;

            // Remove upgrade from tower
            upgrade.RemoveFromTower(tower);

            // Remove from tracking
            var upgradeKey = $"{tower.Type}_{upgrade.Level}_{upgrade.Name}";
            _purchasedUpgrades.Remove(upgradeKey);

            if (_towerUpgrades.ContainsKey(tower))
            {
                _towerUpgrades[tower].Remove(upgrade);
            }

            // Trigger events
            OnUpgradeRemoved?.Invoke(upgrade);

            Console.WriteLine($"Removed upgrade: {upgrade.Name} from {tower.Type}");
            return true;
        }

        /// <summary>
        /// Check if an upgrade is purchased for a tower.
        /// </summary>
        /// <param name="tower">Tower to check.</param>
        /// <param name="upgrade">Upgrade to check.</param>
        /// <returns>True if upgrade is purchased.</returns>
        public bool IsUpgradePurchased(Tower tower, TowerUpgrade upgrade)
        {
            if (tower == null || upgrade == null) return false;

            var upgradeKey = $"{tower.Type}_{upgrade.Level}_{upgrade.Name}";
            return _purchasedUpgrades.ContainsKey(upgradeKey);
        }

        /// <summary>
        /// Get upgrade path for a tower type.
        /// </summary>
        /// <param name="towerType">Tower type.</param>
        /// <returns>Upgrade path for the tower type.</returns>
        public List<TowerUpgrade> GetUpgradePath(TowerType towerType)
        {
            return _upgradePaths.TryGetValue(towerType, out var path) ? path : new List<TowerUpgrade>();
        }

        /// <summary>
        /// Get upgrade database for a tower type.
        /// </summary>
        /// <param name="towerType">Tower type.</param>
        /// <returns>Upgrade database for the tower type.</returns>
        public TowerUpgradeDatabase GetUpgradeDatabase(TowerType towerType)
        {
            return _upgradeDatabases.TryGetValue(towerType, out var database) ? database : null;
        }

        /// <summary>
        /// Get all purchased upgrades.
        /// </summary>
        /// <returns>All purchased upgrades.</returns>
        public IReadOnlyDictionary<string, TowerUpgrade> GetAllPurchasedUpgrades()
        {
            return _purchasedUpgrades;
        }

        /// <summary>
        /// Get upgrade statistics.
        /// </summary>
        /// <returns>Upgrade statistics.</returns>
        public UpgradeStatistics GetStatistics()
        {
            var stats = new UpgradeStatistics
            {
                TotalUpgradesPurchased = TotalUpgradesPurchased,
                TotalUpgradeValue = TotalUpgradeValue,
                AverageUpgradeCost = TotalUpgradesPurchased > 0 ? TotalUpgradeValue / TotalUpgradesPurchased : 0,
                MostPurchasedUpgrade = GetMostPurchasedUpgrade(),
                MostValuableUpgrade = GetMostValuableUpgrade(),
                UpgradePathsCompleted = GetCompletedUpgradePaths(),
                TotalUpgradePaths = _upgradePaths.Count
            };

            return stats;
        }

        /// <summary>
        /// Reset all upgrades.
        /// </summary>
        public void ResetUpgrades()
        {
            Console.WriteLine("Resetting all tower upgrades");

            // Remove all upgrades from towers
            foreach (var kvp in _towerUpgrades)
            {
                var tower = kvp.Key;
                var upgrades = kvp.Value;

                foreach (var upgrade in upgrades)
                    upgrade.RemoveFromTower(tower);
                
            }

            // Clear tracking
            _purchasedUpgrades.Clear();
            _towerUpgrades.Clear();

            // Refund some cost (optional)
            var refundAmount = TotalUpgradeValue / 2; // Refund 50%

            EconomyManager.Instance.AddCash(refundAmount);

            Console.WriteLine($"Refunded ${refundAmount} from upgrade reset");
        }

        public bool SaveUpgrades(string saveName = "upgrades")
        {
            return SaveUpgrades(new UpgradeSaveData
            {
                PurchasedUpgrades = new Dictionary<string, string>(_purchasedUpgrades.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Name)),
                TowerBrains = _towerUpgrades.ToDictionary(static kvp => kvp.Key.Id, static kvp => kvp.Value.Select(static u => u.Name).ToList()),
                SaveTime = DateTime.Now
            }, saveName);
        }

        /// <summary>
        /// Save upgrade data.
        /// </summary>
        /// <param name="saveName">Save file name.</param>
        /// <returns>True if saved successfully.</returns>
        public bool SaveUpgrades(UpgradeSaveData saveData, string saveName = "upgrades")
        {
            try
            {
                // Save to file (implementation would go here)
                Console.WriteLine($"Saved upgrade data: {saveName}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving upgrades: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Load upgrade data.
        /// </summary>
        /// <param name="saveName">Save file name.</param>
        /// <returns>True if loaded successfully.</returns>
        public bool LoadUpgrades(string saveName = "upgrades")
        {
            try
            {
                // Load from file (implementation would go here)
                Console.WriteLine($"Loaded upgrade data: {saveName}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading upgrades: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Get upgrade recommendations for a tower.
        /// </summary>
        /// <param name="tower">Tower to get recommendations for.</param>
        /// <param name="count">Number of recommendations.</param>
        /// <returns>List of recommended upgrades.</returns>
        public List<TowerUpgrade> GetUpgradeRecommendations(Tower tower, int count = 3)
        {
            if (tower == null) return new List<TowerUpgrade>();

            var availableUpgrades = GetAvailableUpgrades(tower);
            if (availableUpgrades.Count == 0) return new List<TowerUpgrade>();

            // Sort by efficiency rating
            var sortedUpgrades = availableUpgrades.OrderByDescending(u => u.GetEfficiencyRating()).ToList();

            return sortedUpgrades.Take(count).ToList();
        }

        /// <summary>
        /// Get upgrade cost for a tower at specific level.
        /// </summary>
        /// <param name="towerType">Tower type.</param>
        /// <param name="level">Upgrade level.</param>
        /// <returns>Upgrade cost.</returns>
        public int GetUpgradeCost(TowerType towerType, int level)
        {
            if (_upgradePaths.TryGetValue(towerType, out var upgradePath))
            {
                var upgrade = upgradePath.FirstOrDefault(u => u.Level == level);
                return upgrade?.Cost ?? 0;
            }

            return 0;
        }

        /// <summary>
        /// Get max upgrade level for a tower type.
        /// </summary>
        /// <param name="towerType">Tower type.</param>
        /// <returns>Max upgrade level.</returns>
        public int GetMaxUpgradeLevel(TowerType towerType)
        {
            if (_upgradePaths.TryGetValue(towerType, out var upgradePath))
            {
                return upgradePath.Max(u => u.Level);
            }

            return 1;
        }

        #region Private Methods

        /// <summary>
        /// Initialize upgrade databases.
        /// </summary>
        void InitializeUpgradeDatabases()
        {
            // Create upgrade databases for each tower type
            foreach (TowerType towerType in Enum.GetValues<TowerType>())
            {
                var db = new TowerUpgradeDatabase();
                _upgradeDatabases[towerType] = db;
            }

            Console.WriteLine($"Initialized {_upgradeDatabases.Count} upgrade databases");
        }

        /// <summary>
        /// Initialize upgrade paths.
        /// </summary>
        void InitializeUpgradePaths()
        {
            // Create upgrade paths for each tower type
            foreach (var kvp in _upgradeDatabases)
            {
                var towerType = kvp.Key;
                var database = kvp.Value;
                
                // Get upgrades from database
                var upgradePath = database.GetUpgrades(towerType);
                _upgradePaths[towerType] = upgradePath;
            }

            Console.WriteLine($"Initialized {_upgradePaths.Count} upgrade paths");
        }

        /// <summary>
        /// Get most purchased upgrade.
        /// </summary>
        /// <returns>Most purchased upgrade.</returns>
        TowerUpgrade GetMostPurchasedUpgrade()
        {
            return _purchasedUpgrades.Values.GroupBy(u => u.Name)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault()?.FirstOrDefault();
        }

        /// <summary>
        /// Get most valuable upgrade.
        /// </summary>
        /// <returns>Most valuable upgrade.</returns>
        TowerUpgrade GetMostValuableUpgrade()
        {
            return _purchasedUpgrades.Values.OrderByDescending(u => u.Cost).FirstOrDefault();
        }

        /// <summary>
        /// Get number of completed upgrade paths.
        /// </summary>
        /// <returns>Number of completed upgrade paths.</returns>
        int GetCompletedUpgradePaths()
        {
            var completedPaths = 0;

            foreach (var kvp in _upgradePaths)
            {
                var towerType = kvp.Key;
                var upgradePath = kvp.Value;
                var maxLevel = upgradePath.Max(u => u.Level);

                var purchasedLevels = _purchasedUpgrades.Values
                    .Where(u => u.TowerType == towerType)
                    .Select(u => u.Level)
                    .Distinct()
                    .Count();

                if (purchasedLevels >= maxLevel) completedPaths++;
            }

            return completedPaths;
        }

        /// <summary>
        /// Play success sound.
        /// </summary>
        void PlaySuccessSound(string soundName)
        {
            // TODO: Implement ModernAudioSubsystem instance
            // ModernAudioSubsystem.PlaySound(soundName);
        }

        /// <summary>
        /// Play error sound.
        /// </summary>
        void PlayErrorSound(string soundName)
        {
            // TODO: Implement ModernAudioSubsystem instance
            // ModernAudioSubsystem.PlaySound(soundName);
        }

        string GetDebuggerDisplay() => ToString();

        #endregion
    }

    public class UpgradeSaveData
    {
        public Dictionary<string, string> PurchasedUpgrades { get; internal set; }
        public Dictionary<uint, List<string>> TowerBrains { get; internal set; }
        public DateTime SaveTime { get; internal set; }
    }

    /// <summary>
    /// Upgrade statistics container.
    /// </summary>
    public class UpgradeStatistics
    {
        public int TotalUpgradesPurchased { get; set; }
        public int TotalUpgradeValue { get; set; }
        public int AverageUpgradeCost { get; set; }
        public TowerUpgrade MostPurchasedUpgrade { get; set; }
        public TowerUpgrade MostValuableUpgrade { get; set; }
        public int UpgradePathsCompleted { get; set; }
        public int TotalUpgradePaths { get; set; }

        public override string ToString()
        {
            return $"Upgrade Statistics:\n" +
                   $"Total Upgrades Purchased: {TotalUpgradesPurchased}\n" +
                   $"Total Upgrade Value: ${TotalUpgradeValue}\n" +
                   $"Average Upgrade Cost: ${AverageUpgradeCost}\n" +
                   $"Most Purchased: {MostPurchasedUpgrade?.Name ?? "None"}\n" +
                   $"Most Valuable: {MostValuableUpgrade?.Name ?? "None"}\n" +
                   $"Completed Paths: {UpgradePathsCompleted}/{TotalUpgradePaths}";
        }
    }

    /// <summary>
    /// Upgrade save data container.
    /// </summary>
    ///    public class UpgradeSaveData (not Needed)
    ///   {
    ///       public Dictionary<string, string> PurchasedUpgrades { get; set; }
    ///       public Dictionary<string, List<string>> TowerBrains { get; set; }
    ///       public DateTime SaveTime { get; set; }

    ///      public static implicit operator UpgradeSaveData(UpgradeSaveData v)
    ///    {
    ///         throw new NotImplementedException();
}
///   }

/// <summary>
/// Extension methods for TowerUpgradeManager.
/// </summary>
public static class TowerUpgradeManagerExtensions
{
    /// <summary>
    /// Get upgrade completion percentage.
    /// </summary>
    public static float GetUpgradeCompletion(this NeuralManager manager, TowerType towerType)
    {
        var totalUpgrades = manager.GetUpgradePath(towerType).Count;

        var purchasedUpgrades = manager.GetAllPurchasedUpgrades()
            .Values.Count(u => u.TowerType == towerType);

        return totalUpgrades > 0 ? (float)purchasedUpgrades / totalUpgrades : 0f;
    }
}