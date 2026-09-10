// ====================================================================================================
//  File: TowerEnums.cs
//  Subsystem: Towers
//  Description:
//      Centralized enumeration definitions for all TowerControl TC_ programs.
//      This file contains ONLY enum declarations. No classes, no logic, no methods.
//      All TC_ programs must reference enums from this file to maintain deterministic
//      subsystem boundaries and prevent enum scattering across the engine.
//
//  Rules:
//      - Enums must be grouped by functional category.
//      - Enums must be appended or updated only; header format must remain unchanged.
//      - No legacy naming (Neural, AI, etc.).
//      - No dependencies on other engine subsystems.
//      - All enums referenced by TC_ programs must be defined here.
//
//  Author: BDC
//
//  Change Log:
//      2026-08-11: Initial creation.
//      2026-08-24: Added additional enums for Towers complete pipeline
//  ====================================================================================================
namespace SASZombieAssaultTD.Engine.Towers
{
    public class TowerEnums
    {
        //==================================================
        // Upgrade Requirement Types
        //==================================================
        public enum UpgradeRequirementType
        {
            None,
            TowerLevel,
            PlayerLevel,
            ResourceThreshold,
            AchievementUnlock
        }

        //==================================================
        // Tower Upgrade Types
        //==================================================
        public enum UpgradeType
        {
            Damage,
            Range,
            FireRate,
            Speed,
            Armor,
            Accuracy,
            Special,
            Complete,
            Elemental
        }

        //==================================================
        // Requirement Types
        //==================================================
        public enum RequirementType
        {
            TowerLevel,
            PlayerLevel,
            WaveComplete,
            EnemiesKilled,
            TowerCount
        }

        //==================================================
        // Reward Types
        //==================================================
        public enum RewardType
        {
            Cash,
            Experience,
            Unlock
        }

        //==================================================
        // Terrain Type
        //==================================================
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

        //==================================================
        // Tower Type
        //==================================================
        public enum TowerType
        {
            Basic,
            Sniper,
            Splash,
            Freeze,
            Rapid,
            Poison,
            Laser,
            Tesla
        }
    }
}
