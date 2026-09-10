// =====================================================================================================
//  FILE: UpgradePanel-Logic.cs
//  PATH: Engine/UI/HUD/PanelsLegacy/UpgradePanel-Logic.cs
//  SUBSYSTEM: UI / HUD (Legacy Panel Partials)
// =====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Linq;
using SASZombieAssaultTD.Engine.Economy;
using SASZombieAssaultTD.Engine.Towers;

namespace SASZombieAssaultTD.Engine.UI.HUD
{
    public partial class UpgradePanel
    {
        // ---------------------------------------------------------------------------------------------
        // Upgrade Selection Logic
        // ---------------------------------------------------------------------------------------------

        private void SelectUpgrade(int index)
        {
            if (index < 0 || index >= _availableUpgrades.Count)
                return;

            _selectedUpgradeIndex = index;
            _selectedUpgrade = _availableUpgrades[index];

            UpdateCanAffordStatus();

            OnUpgradeSelected?.Invoke(_selectedUpgrade);
        }

        private void UpdateCanAffordStatus()
        {
            if (_selectedUpgrade == null)
            {
                _canAffordUpgrade = false;
                return;
            }

            _canAffordUpgrade = EconomyManager.CanAfford(_selectedUpgrade.Cost);
        }

        // ---------------------------------------------------------------------------------------------
        // Upgrade Availability Queries
        // ---------------------------------------------------------------------------------------------

        public TowerUpgrade GetSelectedUpgrade()
        {
            return _selectedUpgrade;
        }

        public int GetAvailableUpgradeCount()
        {
            return _availableUpgrades.Count;
        }

        public List<TowerUpgrade> GetAllAvailableUpgrades()
        {
            return new List<TowerUpgrade>(_availableUpgrades);
        }

        public bool HasAvailableUpgrades()
        {
            return _availableUpgrades.Count > 0;
        }

        public TowerUpgrade GetUpgradeAt(int index)
        {
            if (index < 0 || index >= _availableUpgrades.Count)
                return null;

            return _availableUpgrades[index];
        }

        public int GetTotalUpgradeCost()
        {
            return _availableUpgrades.Sum(u => u.Cost);
        }

        // ---------------------------------------------------------------------------------------------
        // Sorting Helpers
        // ---------------------------------------------------------------------------------------------

        public void SortUpgradesByCost()
        {
            _availableUpgrades = _availableUpgrades
                .OrderBy(u => u.Cost)
                .ToList();
        }

        public void SortUpgradesByLevel()
        {
            _availableUpgrades = _availableUpgrades
                .OrderBy(u => u.Level)
                .ToList();
        }

        public void SortUpgradesByDamage()
        {
            // Explicitly cast the object property to float for sorting operations
            _availableUpgrades = _availableUpgrades
                .OrderByDescending(u => (float)u.DamageIncrease)
                .ToList();
        }

        public List<TowerUpgrade> GetUpgradesByType(string type)
        {
            // Fixed CS0019: Compare string representations to resolve Enum vs String mismatch
            return _availableUpgrades
                .Where(u => u.Type.ToString().Equals(type, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public List<TowerUpgrade> GetUpgradesByDamageIncrease(float minDamage)
        {
            // Fixed CS0019: Unbox object value to float before applying relational check
            return _availableUpgrades
                .Where(u => (float)u.DamageIncrease >= minDamage)
                .ToList();
        }

        // ---------------------------------------------------------------------------------------------
        // Upgrade Property Helpers
        // ---------------------------------------------------------------------------------------------

        public bool IsMaxLevel(TowerUpgrade upgrade)
        {
            return upgrade.Level >= upgrade.MaxLevel;
        }

        public bool IsAvailable(TowerUpgrade upgrade)
        {
            return _availableUpgrades.Contains(upgrade);
        }

        public int GetCost(TowerUpgrade upgrade)
        {
            return upgrade.Cost;
        }

        // Fixed CS0266: Explicitly unbox object properties to float return parameters
        public float GetDamageIncrease(TowerUpgrade upgrade)
        {
            return (float)upgrade.DamageIncrease;
        }

        public float GetRangeIncrease(TowerUpgrade upgrade)
        {
            return (float)upgrade.RangeIncrease;
        }

        public float GetFireRateIncrease(TowerUpgrade upgrade)
        {
            return (float)upgrade.FireRateIncrease;
        }

        public float GetSpeedIncrease(TowerUpgrade upgrade)
        {
            return (float)upgrade.SpeedIncrease;
        }
    }
}
