/*
File:    Tower.cs
Folder:  Engine/Towers/
Purpose: Basic tower entity for SAS Zombie Assault TD.
Features: Tower identification, position, and data reference.
*/

using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.VectorMath;
using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.Projectiles;
using SASZombieAssaultTD.Engine.Extensions;

namespace SASZombieAssaultTD.Engine.Towers
{
    /// <summary>
    /// Represents a placed tower in the game world, managing its properties, upgrades, and stats.
    /// </summary>
    public class Tower
    {
        #region Properties

        /// <summary>
        /// Unique identifier for this tower instance.
        /// </summary>
        public uint Id { get; }

        /// <summary>
        /// Position of the tower in world space.
        /// </summary>
        public Vector3 Position { get; private set; }

        /// <summary>
        /// Grid position of the tower.
        /// </summary>
        public Vector3Int GridPosition { get; private set; }

        /// <summary>
        /// Tower data containing stats and properties.
        /// </summary>
        public TowerData Data { get; }

        /// <summary>
        /// Whether the tower is currently active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Current upgrade level of the tower.
        /// </summary>
        public int UpgradeLevel { get; private set; } = 0;

        /// <summary>
        /// Entity reference for ECS integration.
        /// </summary>
        public Entity Entity { get; set; }

        /// <summary>
        /// Tower name for display.
        /// </summary>
        public string Name => Data?.Name ?? "Unknown Tower";

        /// <summary>
        /// Tower type for display.
        /// </summary>
        public TowerType Type => Data?.Type ?? TowerType.Basic;

        /// <summary>
        /// Current damage based on upgrade level.
        /// </summary>
        public double Damage => Data?.Damage ?? 0.0;

        /// <summary>
        /// Current range based on upgrade level.
        /// </summary>
        public double Range => Data?.Range ?? 0.0;

        /// <summary>
        /// Total kills by this tower.
        /// </summary>
        public int TotalKills { get; set; }

        /// <summary>
        /// Tower accuracy percentage.
        /// </summary>
        public double Accuracy => Data?.Accuracy ?? 1.0;

        /// <summary>
        /// Damage per second.
        /// </summary>
        public double DPS => Damage > 0 ? Damage / (Data?.FireRate ?? 1.0) : 0.0;

        /// <summary>
        /// Tower uptime in seconds.
        /// </summary>
        public float Uptime { get; set; }

        /// <summary>
        /// Available upgrades for this tower.
        /// </summary>
        public List<TowerUpgrade> AvailableUpgrades => Data?.AvailableUpgrades ?? new();

        /// <summary>
        /// Current targeting mode.
        /// </summary>
        public string TargetingMode => Data?.TargetingMode ?? "Closest";

        /// <summary>
        /// Whether the tower can be upgraded.
        /// </summary>
        public bool CanUpgrade => UpgradeLevel < 3 && (Data?.AvailableUpgrades?.Count ?? 0) > UpgradeLevel;

        /// <summary>
        /// Current level of the tower.
        /// </summary>
        public int Level { get; set; } = 1;

        /// <summary>
        /// Current health of the tower.
        /// </summary>
        public float Health { get; set; } = 100.0f;

        /// <summary>
        /// Maximum health of the tower.
        /// </summary>
        public float MaxHealth { get; set; } = 100.0f;

        /// <summary>
        /// Cost of the tower.
        /// </summary>
        public int Cost => Data?.Cost ?? 0;

        /// <summary>
        /// Scale of the tower.
        /// </summary>
        public float Scale { get; set; } = 1.0f;

        /// <summary>
        /// Rotation of the tower.
        /// </summary>
        public float Rotation { get; set; } = 0.0f;

        /// <summary>
        /// Speed of the tower.
        /// </summary>
        public float Speed { get; set; } = 1.0f;

        /// <summary>
        /// Fire rate of the tower.
        /// </summary>
        public float FireRate => Data?.FireRate ?? 1.0f;

        /// <summary>
        /// Total damage dealt by this tower.
        /// </summary>
        public float DamageDealt { get; set; } = 0.0f;

        #endregion

        #region Notifications

        /// <summary>
        /// Notification hook called when a projectile is fired from this tower.
        /// Kept as a no-op default to avoid widespread code changes; override in
        /// derived tower types if specific behavior is required.
        /// </summary>
        public virtual void OnProjectileFired(Projectile projectile)
        {
            // Default: do nothing
        }

        /// <summary>
        /// Notification hook called when multiple projectiles are fired.
        /// </summary>
        public virtual void OnProjectilesFired(System.Collections.Generic.List<Projectile> projectiles)
        {
            // Default: do nothing
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new tower instance with a world position.
        /// </summary>
        /// <param name="id">Unique tower identifier.</param>
        /// <param name="data">Tower data reference.</param>
        /// <param name="position">World position.</param>
        public Tower(uint id, TowerData data, Vector3 position)
        {
            Id = id;
            Data = data ?? throw new ArgumentNullException(nameof(data));
            Position = position;
            GridPosition = new Vector3Int((int)position.X, (int)position.Y, 0);
        }

        /// <summary>
        /// Creates a new tower instance with a tower type and world position.
        /// </summary>
        /// <param name="id">Unique tower identifier.</param>
        /// <param name="towerType">Type of tower.</param>
        /// <param name="position">World position.</param>
        public Tower(uint id, TowerType towerType, Vector3 position)
        {
            Id = id;
            // Create basic tower data from type
            TowerData.Builder builder = new(towerType)
            {
                Type = towerType,
                Name = towerType.ToString(),
                Damage = GetDefaultDamage(towerType),
                Range = GetDefaultRange(towerType),
                FireRate = GetDefaultFireRate(towerType),
                Cost = GetDefaultCost(towerType)
            };
            Data = builder;
            Position = position;
            GridPosition = new Vector3Int((int)position.X, (int)position.Y, 0);
        }

        /// <summary>
        /// Creates a new tower instance with a grid position.
        /// </summary>
        /// <param name="id">Unique tower identifier.</param>
        /// <param name="data">Tower data reference.</param>
        /// <param name="gridPosition">Grid position.</param>
        public Tower(uint id, TowerData data, Vector3Int gridPosition)
        {
            Id = id;
            Data = data ?? throw new ArgumentNullException(nameof(data));
            GridPosition = gridPosition;
            Position = new Vector3(gridPosition.X, gridPosition.Y, 0);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Upgrades the tower to the next level.
        /// </summary>
        /// <returns>True if the upgrade was successful; otherwise, false.</returns>
        public bool Upgrade()
        {
            if (!CanUpgrade) return false;

            UpgradeLevel++;
            return true;
        }

        /// <summary>
        /// Gets the current damage based on upgrade level.
        /// </summary>
        /// <returns>Current damage value.</returns>
        public float GetCurrentDamage() => Data.Damage * (1 + (UpgradeLevel * 0.5f)); // 50% damage increase per level

        /// <summary>
        /// Gets the current range based on upgrade level.
        /// </summary>
        /// <returns>Current range value.</returns>
        public float GetCurrentRange() => Data.Range * (1 + (UpgradeLevel * 0.25f)); // 25% range increase per level

        /// <summary>
        /// Applies an upgrade to the tower.
        /// </summary>
        /// <param name="upgrade">The upgrade to apply.</param>
        public void ApplyUpgrade(TowerUpgrade upgrade)
        {
            if (!CanUpgrade) throw new InvalidOperationException("Cannot apply upgrade. Maximum upgrade level reached.");
            UpgradeLevel++;
            // Apply upgrade effects (implementation depends on TowerUpgrade details)
        }

        /// <summary>
        /// Gets the special abilities of this tower.
        /// </summary>
        /// <returns>List of special abilities.</returns>
        public List<string> GetSpecialAbilities()
        {
            return new List<string>(); // Default implementation
        }

        /// <summary>
        /// Gets the available upgrades for this tower.
        /// </summary>
        /// <returns>List of available upgrades.</returns>
        public List<TowerUpgrade> GetAvailableUpgrades() =>
     Data?.AvailableUpgrades?.ConvertAll(static u => new TowerUpgrade(u)) ?? [];

        /// <summary>
        /// Gets the default damage for a tower type.
        /// </summary>
        /// <param name="towerType">Type of tower.</param>
        /// <returns>Default damage value.</returns>
        private static int GetDefaultDamage(TowerType towerType)
        {
            return towerType switch
            {
                TowerType.Basic => 10,
                TowerType.Sniper => 25,
                TowerType.Splash => 15,
                TowerType.Freeze => 5,
                TowerType.Rapid => 8,
                TowerType.Poison => 12,
                TowerType.Laser => 20,
                TowerType.Tesla => 18,
                _ => 10
            };
        }

        /// <summary>
        /// Gets the default range for a tower type.
        /// </summary>
        /// <param name="towerType">Type of tower.</param>
        /// <returns>Default range value.</returns>
        private static float GetDefaultRange(TowerType towerType)
        {
            return towerType switch
            {
                TowerType.Basic => 3.0f,
                TowerType.Sniper => 6.0f,
                TowerType.Splash => 2.5f,
                TowerType.Freeze => 3.5f,
                TowerType.Rapid => 2.8f,
                TowerType.Poison => 3.2f,
                TowerType.Laser => 5.5f,
                TowerType.Tesla => 4.0f,
                _ => 3.0f
            };
        }

        /// <summary>
        /// Gets the default fire rate for a tower type.
        /// </summary>
        /// <param name="towerType">Type of tower.</param>
        /// <returns>Default fire rate value.</returns>
        private static float GetDefaultFireRate(TowerType towerType)
        {
            return towerType switch
            {
                TowerType.Basic => 1.0f,
                TowerType.Sniper => 0.5f,
                TowerType.Splash => 0.8f,
                TowerType.Freeze => 1.2f,
                TowerType.Rapid => 2.5f,
                TowerType.Poison => 0.9f,
                TowerType.Laser => 1.5f,
                TowerType.Tesla => 1.0f,
                _ => 1.0f
            };
        }

        /// <summary>
        /// Gets the default cost for a tower type.
        /// </summary>
        /// <param name="towerType">Type of tower.</param>
        /// <returns>Default cost value.</returns>
        private static int GetDefaultCost(TowerType towerType)
        {
            return towerType switch
            {
                TowerType.Basic => 100,
                TowerType.Sniper => 200,
                TowerType.Splash => 150,
                TowerType.Freeze => 175,
                TowerType.Rapid => 125,
                TowerType.Poison => 160,
                TowerType.Laser => 250,
                TowerType.Tesla => 225,
                _ => 100
            };
        }

        #endregion
    }
}
