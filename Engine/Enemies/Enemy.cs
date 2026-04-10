/*
File:    Enemy.cs
Purpose: Core enemy entity for SAS Zombie Assault TD.
Features: Enemy properties, behaviors, AI, and state management.
*/

using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.VectorMath;
using SASZombieAssaultTD.Engine.Waves;
using System;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Enemies
{
    /// <summary>
    /// Represents an enemy entity in the game world.
    /// Combines ECS entity with enemy-specific properties and behaviors.
    /// </summary>
    public class   Enemy
    {
        #region Constructors

        /// <summary>
        /// Creates a new enemy.
        /// </summary>
        public Enemy(Entity entity)
        {
            if (!entity.IsValid)
                throw new ArgumentException("Entity must be valid", nameof(entity));

            Entity = entity;
            SpawnTime = 0.0f; // Will be set by spawn system
        }

        /// <summary>
        /// Public parameterless constructor for ObjectPool compatibility.
        /// </summary>
        public Enemy() : this(new Entity(1))
        {
        }

        #endregion

        #region Static Factory

        /// <summary>
        /// Static factory to ensure Enemy class is properly accessible.
        /// </summary>
        public static Enemy CreateEnemy(uint entityId)
        {
            var entity = new Entity(entityId);
            return new Enemy(entity);
        }

        /// <summary>
        /// Static factory to ensure Enemy class is properly accessible.
        /// </summary>
        public static Enemy Create(Entity entity)
        {
            return new Enemy(entity);
        }

        #endregion

        #region Core Properties

        /// <summary>
        /// The underlying ECS entity.
        /// </summary>
        public Entity Entity { get; private set; }

        /// <summary>
        /// Unique enemy identifier.
        /// </summary>
        public uint Id => Entity.Id;

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

        #endregion

        #region Spawn Tracking

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

        #endregion

        #region Movement & Pathing

        /// <summary>
        /// Current velocity.
        /// </summary>
        public Vector3 Velocity { get; set; }

        /// <summary>
        /// Progress along the path (0.0 to 1.0).
        /// </summary>
        public float PathProgress { get; set; } = 0.0f;

        #endregion

        #region Behavior & AI

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

        #endregion

        #region Visual Effects

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

        #endregion

        #region Appearance

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

        #endregion

        #region Lifecycle

        /// <summary>
        /// Marks this enemy as dead.
        /// </summary>
        public void Kill()
        {
            if (!IsDead)
            {
                IsDead = true;
                DeathTime = 0.0f; // Will be set by game time
            }
        }

        /// <summary>
        /// Updates the enemy state.
        /// </summary>
        /// <param name="deltaTime">Time since last update.</param>
        public void Update(float deltaTime)
        {
            // Update position based on velocity
            if (!IsDead)
            {
                Position += Velocity * deltaTime;
            }

            // Update visual effects
            UpdateVisualEffects(deltaTime);
        }

        /// <summary>
        /// Updates visual effects on this enemy.
        /// </summary>
        /// <param name="deltaTime">Time since last update.</param>
        private void UpdateVisualEffects(float deltaTime)
        {
            // Update visual effects like acid, burn, freeze, etc.
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

        #endregion
    }
}
