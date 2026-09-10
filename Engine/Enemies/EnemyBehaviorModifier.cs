// ====================================================================================================
//  FILE: EnemyBehaviorModifier.cs
//  PATH: ./Engine/Enemies/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the EnemyBehaviorModifier module.
//
//  RESPONSIBILITIES:
//      - Provide CreateChampion() behavior for the Core subsystem.
//      - Provide CreateSpeed() behavior for the Core subsystem.
//      - Provide CreateHealth() behavior for the Core subsystem.
//      - Provide CreateArmor() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
using SASZombieAssaultTD.Engine.VectorMath;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Enemies
{
    ///<summary>
    ///Enemy behavior modifier for special abilities and effects.
    ///</summary>
    public class EnemyBehaviorModifier
    {
        public string Name { get; set; }
        public float SpeedMultiplier { get; set; } = 1.0f;
        public float HealthMultiplier { get; set; } = 1.0f;
        public float DamageMultiplier { get; set; } = 1.0f;
        public float ArmorMultiplier { get; set; } = 1.0f;
        public Vector3 SizeMultiplier { get; set; } = Vector3.One;
        public bool IsChampion { get; set; } = false;
        public int ChampionLevel { get; set; } = 1;
        public float Duration { get; set; } = 0f; //0 = permanent
        public string ModifierType { get; set; }
        public float Value { get; set; }
        public Dictionary<string, object> Parameters { get; set; }

        public EnemyBehaviorModifier() => Name = "None";

        public EnemyBehaviorModifier(string name) => Name = name;

        ///<summary>
        ///Creates a champion modifier.
        ///</summary>
        public static EnemyBehaviorModifier CreateChampion(int level = 1)
        {
            return new EnemyBehaviorModifier("Champion")
            {
                IsChampion = true,
                ChampionLevel = level,
                HealthMultiplier = 1.0f + (level * 0.5f),
                DamageMultiplier = 1.0f + (level * 0.3f),
                SpeedMultiplier = 1.0f + (level * 0.2f),
                SizeMultiplier = new Vector3(1.2f, 1.2f, 1.2f)
            };
        }

        ///<summary>
        ///Creates a speed modifier.
        ///</summary>
        public static EnemyBehaviorModifier CreateSpeed(float multiplier)
        {
            return new EnemyBehaviorModifier("Speed Boost")
            {
                SpeedMultiplier = multiplier
            };
        }

        ///<summary>
        ///Creates a health modifier.
        ///</summary>
        public static EnemyBehaviorModifier CreateHealth(float multiplier)
        {
            return new EnemyBehaviorModifier("Health Boost")
            {
                HealthMultiplier = multiplier
            };
        }

        ///<summary>
        ///Creates an armor modifier.
        ///</summary>
        public static EnemyBehaviorModifier CreateArmor(float multiplier)
        {
            return new EnemyBehaviorModifier("Armor Boost")
            {
                ArmorMultiplier = multiplier
            };
        }
    }
}

