// =====================================================================================================
//  FILE: TC_Manager.cs
//  PATH: Engine/Towers/TowerControl/TC_Manager.cs
//  SUBSYSTEM: Tower TowerControl Subsystems
//
//  ROLE:
//      Root coordinator for the TowerControl subsystem. This program wires together all
//      controllers, queries, databases, and dispatchers into a single deterministic façade.
//      Extracted and rebuilt from NeuralManager.cs.
//
//  RESPONSIBILITIES:
//      - Construct and expose all TowerControl subsystem controllers.
//      - Maintain shared dictionaries used across controllers.
//      - Provide unified initialization sequencing.
//      - Provide a clean API surface for GameRootMain.
//
//  NON-RESPONSIBILITIES:
//      - Performing upgrade logic directly (delegated to controllers).
//      - Rendering, frame updates, or hardware boundaries.
//      - Managing game state or assets.
//
//  ARCHITECTURAL NOTES:
//      - All controllers are public and exposed through public properties.
//      - Initialization follows the deterministic order required by NeuralManager.cs.
// =====================================================================================================

using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Towers.TowerControl.Controllers;
using SASZombieAssaultTD.Engine.Towers.TowerControl.Database;
using SASZombieAssaultTD.Engine.Towers.TowerControl.Queries;
using static SASZombieAssaultTD.Engine.Towers.TowerEnums;

namespace SASZombieAssaultTD.Engine.Towers.TowerControl
{
    public class TC_Manager
    {
        // Shared subsystem dictionaries
        private readonly Dictionary<TowerType, TC_UpgradeDatabase> _upgradeDatabases;
        private readonly Dictionary<TowerType, List<TowerUpgrade>> _upgradePaths;
        private readonly Dictionary<string, TowerUpgrade> _purchasedUpgrades;
        private readonly Dictionary<Tower, List<TowerUpgrade>> _towerUpgrades;

        // Controllers
        public TC_PathControl PathControl { get; }
        public TC_AvailabilityControl AvailabilityControl { get; }
        public TC_PurchaseControl PurchaseControl { get; }
        public TC_SaveControl SaveControl { get; }
        public TC_LoadControl LoadControl { get; }
        public TC_InitializationControl InitializationControl { get; }
        public TC_RecommendationControl RecommendationControl { get; }
        public TC_EventDispatching EventDispatching { get; }

        // Queries
        public TC_CostQuery CostQuery { get; }
        public TC_LevelQuery LevelQuery { get; }

        // Additional public classes required by TC_Manager
        public TC_UpgradeCostCalculator UpgradeCostCalculator { get; }
        public TC_UpgradeStatistics UpgradeStatistics { get; }
        public TC_UpgradeValidator UpgradeValidator { get; }

        public TC_Manager()
        {
            // Shared dictionaries
            _upgradeDatabases = new Dictionary<TowerType, TC_UpgradeDatabase>();
            _upgradePaths = new Dictionary<TowerType, List<TowerUpgrade>>();
            _purchasedUpgrades = new Dictionary<string, TowerUpgrade>();
            _towerUpgrades = new Dictionary<Tower, List<TowerUpgrade>>();

            // Controllers
            PathControl = new TC_PathControl(_upgradePaths);
            AvailabilityControl = new TC_AvailabilityControl(_upgradePaths, _towerUpgrades);
            PurchaseControl = new TC_PurchaseControl(_purchasedUpgrades, _towerUpgrades);
            SaveControl = new TC_SaveControl(_purchasedUpgrades, _towerUpgrades);
            LoadControl = new TC_LoadControl();
            InitializationControl = new TC_InitializationControl(_upgradeDatabases, _upgradePaths);
            RecommendationControl = new TC_RecommendationControl(AvailabilityControl);
            EventDispatching = new TC_EventDispatching();

            // Queries
            CostQuery = new TC_CostQuery(_upgradePaths);
            LevelQuery = new TC_LevelQuery(_upgradePaths);

            // Additional required public classes
            UpgradeCostCalculator = new TC_UpgradeCostCalculator();
            UpgradeStatistics = new TC_UpgradeStatistics();
            UpgradeValidator = new TC_UpgradeValidator();
        }

        public void Initialize()
        {
            InitializationControl.InitializeAllControllers();
            EventDispatching.Initialize();
        }
    }

    // =====================================================================================================
    // REQUIRED PUBLIC CLASSES (Fixes CS0053 accessibility errors)
    // =====================================================================================================

    public class TC_UpgradeCostCalculator
    {
        public int Calculate(TowerUpgrade upgrade) => upgrade?.Cost ?? 0;
    }

    public class TC_UpgradeStatistics
    {
        public float GetEfficiency(TowerUpgrade upgrade) => (float)(upgrade?.GetEfficiencyRating() ?? 0f);
    }

    public class TC_UpgradeValidator
    {
        public bool Validate(Tower tower, TowerUpgrade upgrade)
        {
            if (tower == null || upgrade == null)
                return false;

            return tower.Level >= upgrade.RequiredLevel;
        }
    }
}
