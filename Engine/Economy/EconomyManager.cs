/*
File:    EconomyManager.cs
Purpose: Central economy system for SAS Zombie Assault TD.
Features: Cash management, resource tracking, difficulty scaling.
*/

using System;
using SASZombieAssaultTD.Engine.Core;

namespace SASZombieAssaultTD.Engine.Economy
{
    /// <summary>
    /// Central economy manager for handling all financial aspects of the game.
    /// Manages cash, rewards, purchases, and difficulty-based scaling.
    /// </summary>
    public static class EconomyManager
    {
        private static int _currentCash = 500;
        private static int _startingCash = 500;
        private static float _cashRewardMultiplier = 1.0f;
        private static float _towerCostMultiplier = 1.0f;
        private static float _upgradeCostMultiplier = 1.0f;
        private static int _totalEarned = 0;
        private static int _totalSpent = 0;
        private static int _towerPurchases = 0;
        private static int _upgradePurchases = 0;

        public static int CurrentCash => _currentCash;
        public static int StartingCash => _startingCash;
        public static int TotalEarned => _totalEarned;
        public static int TotalSpent => _totalSpent;
        public static int TowerPurchases => _towerPurchases;
        public static int UpgradePurchases => _upgradePurchases;
        
        public static event Action<int> OnCashChanged;

        public static void SetDifficultyMultiplier(float cashRewardMultiplier, float towerCostMultiplier, float upgradeCostMultiplier)
        {
            _cashRewardMultiplier = cashRewardMultiplier;
            _towerCostMultiplier = towerCostMultiplier;
            _upgradeCostMultiplier = upgradeCostMultiplier;

            Engine.Diagnostics.DebugLogger.LogDebug("INFO", $"EconomyManager: Set difficulty multipliers - Cash: {cashRewardMultiplier:F2}, Tower: {towerCostMultiplier:F2}, Upgrade: {upgradeCostMultiplier:F2}");
        }

        public static void AddCash(int amount)
        {
            _currentCash += amount;
            _totalEarned += amount;
            OnCashChanged?.Invoke(_currentCash);
            Engine.Diagnostics.DebugLogger.LogDebug("INFO", $"EconomyManager: Added {amount} cash - Total: {_currentCash}");
        }

        public static void RemoveCash(int amount)
        {
            if (_currentCash >= amount)
            {
                _currentCash -= amount;
                _totalSpent += amount;
                OnCashChanged?.Invoke(_currentCash);
                Engine.Diagnostics.DebugLogger.LogDebug("INFO", $"EconomyManager: Removed {amount} cash - Total: {_currentCash}");
            }
            else
            {
                Engine.Diagnostics.DebugLogger.LogDebug("WARNING", "EconomyManager: Insufficient cash to remove " + amount);
            }
        }

        public static void RecordTowerPurchase(int cost)
        {
            _towerPurchases++;
            RemoveCash(cost);
            Engine.Diagnostics.DebugLogger.LogDebug("INFO", $"EconomyManager: Tower purchase recorded - Total: {_towerPurchases}");
        }

        public static void RecordUpgradePurchase(int cost)
        {
            _upgradePurchases++;
            RemoveCash(cost);
            Engine.Diagnostics.DebugLogger.LogDebug("INFO", $"EconomyManager: Upgrade purchase recorded - Total: {_upgradePurchases}");
        }

        public static void Reset()
        {
            _currentCash = _startingCash;
            _totalEarned = 0;
            _totalSpent = 0;
            _towerPurchases = 0;
            _upgradePurchases = 0;
            OnCashChanged?.Invoke(_currentCash);
            Engine.Diagnostics.DebugLogger.LogDebug("INFO", "EconomyManager: Economy reset to starting cash: " + _startingCash);
        }

        public static (float cashRewardMultiplier, float towerCostMultiplier, float upgradeCostMultiplier) GetDifficultyMultipliers()
        {
            return (_cashRewardMultiplier, _towerCostMultiplier, _upgradeCostMultiplier);
        }

        public static bool CanAfford(int amount)
        {
            return _currentCash >= amount;
        }

        public static void Spend(int amount)
        {
            RemoveCash(amount);
        }

        public static void Earn(int amount)
        {
            AddCash(amount);
        }

        public static int GetFinalCost(int baseCost)
        {
            return (int)(baseCost * _towerCostMultiplier);
        }

        public static bool HasEnoughCash(int cost)
        {
            return _currentCash >= cost;
        }
    }
}
