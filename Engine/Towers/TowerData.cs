/*
File:    TowerData.cs
Purpose:   Core tower data structure for SAS Zombie Assault TD.
Features:  Tower properties, upgrade paths, and power requirements.
*/

using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.VectorMath;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Towers
{
    ///<summary>
    ///Terrain types for tower placement.
    ///</summary>
    public enum TerrainType
    {
        Invalid,
        Walkable,
        Blocked,
        Water,
        Mountain,
        Road,
        Any,
        Ground,
        Rooftop
    }

    ///<summary>
    ///Core data structure for tower definitions.
    ///Contains all tower properties required for gameplay and validation.
    ///</summary>
    public class TowerData
    {
        /// Basic Properties

        ///<summary>
        ///Unique identifier for the tower type.
        ///</summary>
        public string TowerId { get; set; } = string.Empty;

        ///<summary>
        ///Display name of the tower.
        ///</summary>
        public string Name { get; set; } = string.Empty;

        ///<summary>
        ///Description of the tower.
        ///</summary>
        public string Description { get; set; } = string.Empty;

        ///<summary>
        ///The tower's type.
        ///</summary>
        public TowerType Type { get; set; } = TowerType.Basic;

        ///<summary>
        ///Size of the tower (grid cells occupied).
        ///</summary>
        public Vector3Int Size { get; set; } = new Vector3Int(1, 1, 0);

        ///<summary>
        ///Sprite asset ID for tower rendering.
        ///</summary>
        public string Sprite { get; set; } = string.Empty;

        ///<summary>
        ///Base cost to build the tower.
        ///</summary>
        public int BaseCost { get; set; }

        ///<summary>
        ///Damage dealt by the tower.
        ///</summary>
        public float Damage { get; set; }

        ///<summary>
        ///Attack range of the tower.
        ///</summary>
        public float Range { get; set; }

        ///<summary>
        ///Attack speed (attacks per second).
        ///</summary>
        public float AttackSpeed { get; set; }

        ///<summary>
        ///Fire rate of the tower (time between attacks).
        ///</summary>
        public float FireRate { get; set; } = 1.0f;

        ///<summary>
        ///Accuracy of the tower attacks.
        ///</summary>
        public float Accuracy { get; set; } = 0.9f;

        ///<summary>
        ///Targeting mode of the tower.
        ///</summary>
        public string TargetingMode { get; set; } = "Closest";

        ///<summary>
        ///Available upgrades for this tower.
        ///</summary>
        public List<TowerUpgrade> AvailableUpgrades { get; set; } = new();

        ///<summary>
        ///Preview sprite for placement UI.
        ///</summary>
        public string PreviewSprite { get; set; } = string.Empty;

        ///<summary>
        ///Cost of the tower.
        ///</summary>
        public int Cost { get; set; }

        ///<summary>
        ///Grid size of the tower.
        ///</summary>
        public Vector3Int GridSize { get; set; } = new Vector3Int(1, 1, 0);

        ///

        /// Power Grid Requirements

        ///<summary>
        ///Whether the tower requires power grid connection.
        ///</summary>
        public bool RequiresPower { get; set; }

        ///<summary>
        ///Maximum number of towers allowed per map.
        ///</summary>
        public int MaxPerMap { get; set; }

        ///<summary>
        ///Required terrain type for placement.
        ///</summary>
        public TerrainType RequiredTerrain { get; set; }

        ///

        /// Upgrade Paths

        ///<summary>
        ///List of possible upgrade paths.
        ///</summary>
        public List<string> UpgradePaths { get; set; } = new();

        ///<summary>
        ///Current upgrade level of the tower.
        ///</summary>
        public int UpgradeLevel { get; set; }

        ///

        /// Constructor

        ///<summary>
        ///Creates a new tower data instance.
        ///</summary>
        ///<param name="towerId">Unique tower identifier.</param>
        public TowerData(string towerId)
        {
            TowerId = towerId ?? throw new ArgumentNullException(nameof(towerId));
        }

        ///

        /// Validation

        ///<summary>
        ///Validates the tower data.
        ///</summary>
        ///<returns>True if valid, false otherwise.</returns>
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(TowerId) && BaseCost > 0 && Damage > 0;
        }

        internal class Builder : TowerData
        {
            private TowerType towerType;

            public Builder(TowerType towerType) : base(towerType.ToString())
            {
                this.towerType = towerType;
            }

            public TowerType Type { get; set; }
            public string Name { get; set; }
            public int Damage { get; set; }
            public float Range { get; set; }
            public float FireRate { get; set; }
            public int Cost { get; set; }
        }

        ///
    }
}
