// ====================================================================================================
//  FILE: Enemy.cs
//  PATH: ./Engine/Enemies/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the Enemy module.
//
//  RESPONSIBILITIES:
//      - Provide CreateEnemy() behavior for the Core subsystem.
//      - Provide Create() behavior for the Core subsystem.
//      - Provide SetAIType() behavior for the Core subsystem.
//      - Provide SetAggressionLevel() behavior for the Core subsystem.
//      - Provide ApplyBehaviorModifier() behavior for the Core subsystem.
//      - Provide AddSpecialAbility() behavior for the Core subsystem.
//      - Provide SetCustomProperty() behavior for the Core subsystem.
//      - Provide AddVisualEffect() behavior for the Core subsystem.
//      - Provide ApplyAcidEffect() behavior for the Core subsystem.
//      - Provide ApplyChainLightningEffect() behavior for the Core subsystem.
//      - Provide SetScale() behavior for the Core subsystem.
//      - Provide SetSkin() behavior for the Core subsystem.
//      - Provide SetTintColor() behavior for the Core subsystem.
//      - Provide Kill() behavior for the Core subsystem.
//      - Provide Update() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
using System;
using System.Collections.Generic;

using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.VectorMath;
using SASZombieAssaultTD.Engine.Waves;

namespace SASZombieAssaultTD.Engine.Enemies
{
    /// <summary>
    /// Represents an enemy ECSEntityCore in the game world. Combines ECS ECSEntityCore with enemy-specific properties
    /// and behaviors.
    /// </summary>
    public class Enemy
    {
        /// Constructors

        /// <summary>
        /// Creates a new enemy.
        /// </summary>
        public Enemy(ECSEntityCore ECSEntityCore)
        {
            if (!ECSEntityCore.IsValid)
                throw new ArgumentException("Entity must be valid", nameof(ECSEntityCore));

            ECSEntityCore = ECSEntityCore;
            SpawnTime = 0.0f; //Will be set by spawn system
        }

        /// <summary>
        /// Public parameterless constructor for ObjectPool compatibility.
        /// </summary>
        public Enemy() : this(new ECSEntityCore(1))
        {
        }

        ///

        /// Static Factory

        /// <summary>
        /// Static factory to ensure Enemy class is properly accessible.
        /// </summary>
        public static Enemy CreateEnemy(uint ECSEntityCoreId)
        {
            var ECSEntityCore = new ECSEntityCore(ECSEntityCoreId);
            return new Enemy(ECSEntityCore);
        }

        /// <summary>
        /// Static factory to ensure Enemy class is properly accessible.
        /// </summary>
        public static Enemy Create(ECSEntityCore ECSEntityCore)
        {
            return new Enemy(ECSEntityCore);
        }

        ///

        /// Core Properties

        /// <summary>
        /// The underlying ECS ECSEntityCore.
        /// </summary>
        public ECSEntityCore ECSEntityCore { get; private set; }

        /// <summary>
        /// Unique enemy identifier.
        /// </summary>
        public uint Id => ECSEntityCore.Id;

        /// <summary>
        /// Enemy name/type.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Enemy size.
        /// </summary>
        public float Size { get; set; } = 1.0f;

        /// <summary>
        /// Enemy armor value.
        /// </summary>
        public float Armor { get; set; } = 0.0f;

        /// <summary>
        /// Whether this enemy is a champion variant.
        /// </summary>
        public bool IsChampion { get; set; } = false;

        /// <summary>
        /// Champion level for this enemy (0 if not a champion).
        /// </summary>
        public int ChampionLevel { get; set; } = 0;

        /// <summary>
        /// Whether the enemy is dead.
        /// </summary>
        public bool IsDead { get; private set; } = false;

        /// <summary>
        /// Whether this enemy is currently active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Current health of the enemy.
        /// </summary>
        public float Health { get; set; } = 100.0f;

        /// <summary>
        /// Maximum health of the enemy.
        /// </summary>
        public float MaxHealth { get; set; } = 100.0f;

        /// <summary>
        /// Damage dealt by this enemy.
        /// </summary>
        public float Damage { get; set; } = 10.0f;

        /// <summary>
        /// Movement speed of the enemy.
        /// </summary>
        public float Speed { get; set; } = 1.0f;

        /// <summary>
        /// Accuracy of the enemy.
        /// </summary>
        public float Accuracy { get; set; } = 0.8f;

        /// <summary>
        /// Enemy type identifier.
        /// </summary>
        public string Type { get; set; } = "Basic";

        /// <summary>
        /// Current position in world space.
        /// </summary>
        public Vector3 Position { get; set; } = Vector3.Zero;

        /// <summary>
        /// Total damage dealt by this enemy.
        /// </summary>
        public float DamageDealt { get; set; } = 0.0f;

        /// <summary>
        /// Total kills made by this enemy.
        /// </summary>
        public int TotalKills { get; set; } = 0;

        ///

        /// Spawn Tracking

        /// <summary>
        /// The spawn group that created this enemy.
        /// </summary>
        public IWaveSpawnGroup? SourceSpawnGroup { get; set; }

        /// <summary>
        /// The wave that spawned this enemy.
        /// </summary>
        public int SourceWave { get; set; }

        /// <summary>
        /// Index within the spawn group.
        /// </summary>
        public int SpawnIndex { get; set; } = -1;

        /// <summary>
        /// Time when this enemy was spawned.
        /// </summary>
        public float SpawnTime { get; set; } = 0.0f;

        /// <summary>
        /// Time when this enemy died.
        /// </summary>
        public float DeathTime { get; private set; } = -1.0f;

        /// <summary>
        /// How long this enemy has been alive.
        /// </summary>
        public float Lifetime => IsDead ? DeathTime - SpawnTime : 0.0f;

        ///

        /// Movement & Pathing

        /// <summary>
        /// Current velocity.
        /// </summary>
        public Vector3 Velocity { get; set; }

        /// <summary>
        /// Progress along the path (0.0 to 1.0).
        /// </summary>
        public float PathProgress { get; set; } = 0.0f;

        ///

        /// Behavior & AI

        private Dictionary<string, object> _customProperties = new();
        private List<EnemyBehaviorModifier> _behaviorModifiers = new();
        private List<string> _specialAbilities = new();

        /// <summary>
        /// Sets the AI type for this enemy.
        /// </summary>
        public void SetAIType(string aiType)
        {
            SetCustomProperty("AIType", aiType);
        }

        /// <summary>
        /// Sets the aggression level.
        /// </summary>
        public void SetAggressionLevel(float level)
        {
            SetCustomProperty("AggressionLevel", level);
        }

        /// <summary>
        /// Applies a behavior modifier to this enemy.
        /// </summary>
        public void ApplyBehaviorModifier(EnemyBehaviorModifier modifier)
        {
            _behaviorModifiers.Add(modifier);
        }

        /// <summary>
        /// Adds a special ability to this enemy.
        /// </summary>
        public void AddSpecialAbility(string ability)
        {
            _specialAbilities.Add(ability);
        }

        /// <summary>
        /// Sets a custom property.
        /// </summary>
        public void SetCustomProperty(string key, object value)
        {
            _customProperties[key] = value;
        }

        /// <summary>
        /// Gets a custom property.
        /// </summary>
        public T? GetCustomProperty<T>(string key)
        {
            if (_customProperties.TryGetValue(key, out var value) && value is T typedValue)
                return typedValue;
            return default;
        }

        ///

        /// Visual Effects

        private List<object> _visualEffects = new();

        /// <summary>
        /// Adds a visual effect to this enemy.
        /// </summary>
        public void AddVisualEffect(object effect)
        {
            _visualEffects.Add(effect);
        }

        /// <summary>
        /// Applies acid effect.
        /// </summary>
        public void ApplyAcidEffect(float duration, float damagePerSecond)
        {
            SetCustomProperty("AcidDuration", duration);
            SetCustomProperty("AcidDamagePerSecond", damagePerSecond);
        }

        /// <summary>
        /// Applies chain lightning effect.
        /// </summary>
        public void ApplyChainLightningEffect(float range)
        {
            SetCustomProperty("ChainLightningRange", range);
        }

        ///

        /// Appearance

        /// <summary>
        /// Sets the enemy scale.
        /// </summary>
        public void SetScale(float scale)
        {
            Size = scale;
            SetCustomProperty("Scale", scale);
        }

        /// <summary>
        /// Sets the enemy skin.
        /// </summary>
        public void SetSkin(string skinName)
        {
            SetCustomProperty("Skin", skinName);
        }

        /// <summary>
        /// Sets the enemy tint color.
        /// </summary>
        public void SetTintColor(float r, float g, float b, float a = 1.0f)
        {
            SetCustomProperty("TintColor", new Vector4(r, g, b, a));
        }

        ///

        /// Lifecycle

        /// <summary>
        /// Marks this enemy as dead.
        /// </summary>
        public void Kill()
        {
            if (!IsDead)
            {
                IsDead = true;
                DeathTime = 0.0f; //Will be set by game time
            }
        }

        /// <summary>
        /// Updates the enemy state.
        /// </summary>
        /// <param name="deltaTime">Time since last update.</param>
        public void Update(float deltaTime)
        {
            //Update position based on velocity
            if (!IsDead)
            {
                Position += Velocity * deltaTime;
            }

            //Update visual effects
            UpdateVisualEffects(deltaTime);
        }

        /// <summary>
        /// Updates visual effects on this enemy.
        /// </summary>
        /// <param name="deltaTime">Time since last update.</param>
        private void UpdateVisualEffects(float deltaTime)
        {
            //Update visual effects like acid, burn, freeze, etc.
            var acidDuration = GetCustomProperty<float?>("AcidDuration") ?? 0f;
            if (acidDuration > 0)
            {
                acidDuration -= deltaTime;
                if (acidDuration <= 0f)
                {
                    SetCustomProperty("AcidDuration", 0f);
                    SetCustomProperty("AcidDamagePerSecond", 0f);
                }
                else
                {
                    SetCustomProperty("AcidDuration", acidDuration);
                }
            }

            var burnDuration = GetCustomProperty<float?>("BurnDuration") ?? 0f;
            if (burnDuration > 0)
            {
                burnDuration -= deltaTime;
                if (burnDuration <= 0f)
                {
                    SetCustomProperty("BurnDuration", 0f);
                    SetCustomProperty("BurnDamagePerSecond", 0f);
                }
                else
                {
                    SetCustomProperty("BurnDuration", burnDuration);
                }
            }

            var freezeDuration = GetCustomProperty<float?>("FreezeDuration") ?? 0f;
            if (freezeDuration > 0)
            {
                freezeDuration -= deltaTime;
                if (freezeDuration <= 0f)
                {
                    SetCustomProperty("FreezeDuration", 0f);
                    SetCustomProperty("FreezeSlowFactor", 1.0f);
                }
                else
                {
                    SetCustomProperty("FreezeDuration", freezeDuration);
                }
            }
        }

        ///
    }
}