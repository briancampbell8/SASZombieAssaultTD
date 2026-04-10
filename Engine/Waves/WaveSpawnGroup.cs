// FILE PATH: Engine/Waves/WaveSpawnGroup.cs
// EXECUTION TRIGGER: Instantiated by WaveManager during wave initialization
// PROGRAM PURPOSE: Configures and controls enemy spawn groups with timing, positioning, stat modifiers, and visual effects
// PROGRAM CALLS: NavigationGrid, Enemy, IDifficultyService, WaveGameState, SpawnConditions, ThreadLocal<Random>
// PROGRAM CONTENTS: WaveSpawnGroup class with spawn configuration properties, spawn position methods, enemy modification methods, validation, cloning, and nested types (ZombieType enum, Color struct, VisualEffect class, EnemyBehaviorModifier class, plus SpawnPositionType, AggressionLevel, AIType, SpawnTrigger enums)

using SASZombieAssaultTD.Engine.Navigation;
using SASZombieAssaultTD.Engine.VectorMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

// PROGRAM CALLS: None (data container)
// PROGRAM CONTENTS: WaveGameState class with PlayerLevel, BuiltTowers, GameSpeed, IsPaused properties and default constructor

namespace SASZombieAssaultTD.Engine.Waves
{
    /// <summary>
    /// Enhanced wave spawn group with advanced configuration options.
    /// Provides detailed control over enemy spawning behavior.
    /// </summary>
    public class WaveSpawnGroup : IWaveSpawnGroup
    {
        #region
        /// <summary>
        /// Color for visual effects.
        /// </summary>
        public struct Color
        {
            public byte R { get; set; }
            public byte G { get; set; }
            public byte B { get; set; }
            public byte A { get; set; }

            public Color(byte r, byte g, byte b, byte a = 255)
            {
                R = r;
                G = g;
                B = b;
                A = a;
            }
        }

        /// <summary>
        /// Zombie type enumeration.
        /// </summary>
        public enum ZombieType
        {
            Basic,
            Fast,
            Tank,
            Spitter,
            Boss,
            Swarm,
            Armored,
            Suicide,
            Bloater,
            Mamushka
        }
        /// <summary>
        /// Thread-local random number generator for spawn variations.
        /// Each thread gets its own instance to ensure thread-safety.
        /// </summary>
        static readonly ThreadLocal<Random> _random = new ThreadLocal<Random>(() =>
            new Random(Guid.NewGuid().GetHashCode()));

        // Basic spawn properties
        public WaveSpawnGroup.ZombieType EnemyType { get; set; }
        public int Count { get; set; }
        public float SpawnDelay { get; set; }
        public SpawnPatternType Pattern { get; set; }
        public float DelayAfterGroup { get; set; }
        public bool IsBoss { get; set; }

        // Timing variations
        public float SpawnDelayVariation { get; set; } = 0f;
        public bool RandomizeDelay { get; set; } = false;
        public float InitialDelay { get; set; } = 0f;

        // Spawn position control
        public SpawnPositionType PositionType { get; set; } = SpawnPositionType.Random;
        public List<int> SpecificSpawnPoints { get; set; }
        public Vector3 CustomSpawnPosition { get; set; }
        public float SpawnRadius { get; set; } = 2f;

        // Enemy modifications
        public float? HealthMultiplier { get; set; }
        public float? SpeedMultiplier { get; set; }
        public float? DamageMultiplier { get; set; }
        public float? SizeMultiplier { get; set; }
        public float? ArmorMultiplier { get; set; }

        // Behavioral modifications
        public List<EnemyBehaviorModifier> BehaviorModifiers { get; set; }
        public AIType OverrideAI { get; set; }
        public AggressionLevel Aggression { get; set; } = AggressionLevel.Normal;

        // Visual modifications
        public string EnemySkin { get; set; }
        public WaveSpawnGroup.Color? TintColor { get; set; }
        public float? Scale { get; set; }
        public List<VisualEffect> VisualEffects { get; set; }

        // Spawn conditions
        public SpawnConditions Conditions { get; set; }
        public SpawnTrigger Trigger { get; set; }
        public bool ConditionalSpawn { get; set; }

        // Special properties
        public bool IsChampion { get; set; }
        public int ChampionLevel { get; set; } = 1;
        public List<string> SpecialAbilities { get; set; }
        public Dictionary<string, float> CustomProperties { get; set; }

        // Events and callbacks
        public Action<Enemy> OnEnemySpawned { get; set; }
        public Action<WaveSpawnGroup> OnGroupCompleted { get; set; }
        public Func<Enemy, bool> SpawnFilter { get; set; }

        // Cached spawn points to avoid repeated LINQ allocations in hot path
        Vector3[] _cachedSpawnPoints;
        bool _spawnPointsCached;

        public WaveSpawnGroup()
        {
            EnemyType = WaveSpawnGroup.ZombieType.Swarm;
            Count = 1;
            SpawnDelay = 0.5f;
            Pattern = SpawnPatternType.Line;
            DelayAfterGroup = 2f;
            IsBoss = false;

            SpecificSpawnPoints = new List<int>();
            BehaviorModifiers = new List<EnemyBehaviorModifier>();
            VisualEffects = new List<VisualEffect>();
            Conditions = new SpawnConditions();
            SpecialAbilities = new List<string>();
            CustomProperties = new Dictionary<string, float>();
        }

        /// <summary>
        /// Get effective spawn delay with variations.
        /// </summary>
        /// <param name="enemyIndex">Index of enemy in spawn sequence.</param>
        /// <returns>Effective spawn delay.</returns>
        public float GetEffectiveSpawnDelay(int enemyIndex = 0)
        {
            var baseDelay = SpawnDelay;

            // Add initial delay for first enemy
            if (enemyIndex == 0)
            {
                baseDelay += InitialDelay;
            }

            // Apply random variation if enabled
            if (RandomizeDelay && SpawnDelayVariation > 0)
            {
                var variation = ((float)_random.Value.NextDouble() - 0.5f) * 2f * SpawnDelayVariation;

                baseDelay += variation;
            }

            // Apply pattern-specific delays
            baseDelay = ApplyPatternDelay(baseDelay, enemyIndex);

            return System.Math.Max(0.1f, baseDelay); // Minimum delay
        }

        /// <summary>
        /// Get effective enemy count with modifiers.
        /// </summary>
        /// <param name="difficultyService">Difficulty service for getting current difficulty multiplier.</param>
        /// <returns>Effective enemy count.</returns>
        public int GetEffectiveCount(IDifficultyService difficultyService = null)
        {
            var count = Count;
            var multiplier = 1.0f;

            // Apply difficulty-based count increase
            if (difficultyService != null)
            {
                multiplier = difficultyService.GetEnemyCountMultiplier();
            }

            // Apply champion modifier (separate from stat scaling)
            if (IsChampion)
            {
                multiplier *= GetChampionCountMultiplier();
            }

            return (int)(count * multiplier);
        }

        /// <summary>
        /// Get the champion multiplier for enemy count (10% per level).
        /// </summary>
        public float GetChampionCountMultiplier() => 1f + (ChampionLevel * 0.1f);

        /// <summary>
        /// Get the champion multiplier for enemy stats (20% per level).
        /// </summary>
        public float GetChampionStatMultiplier() => 1f + (ChampionLevel * 0.2f);

        /// <summary>
        /// Get spawn position for specific enemy.
        /// </summary>
        /// <param name="enemyIndex">Index of enemy in spawn sequence.</param>
        /// <param name="totalEnemies">Total enemies in this group.</param>
        /// <returns>Spawn position.</returns>
        public Vector3 GetSpawnPosition(int enemyIndex, int totalEnemies)
        {
            return PositionType switch
            {
                SpawnPositionType.Random => GetRandomSpawnPosition(),
                SpawnPositionType.Specific => GetSpecificSpawnPosition(enemyIndex),
                SpawnPositionType.Custom => CustomSpawnPosition,
                SpawnPositionType.Pattern => GetPatternSpawnPosition(enemyIndex, totalEnemies),
                SpawnPositionType.Circle => GetCircleSpawnPosition(enemyIndex, totalEnemies),
                SpawnPositionType.Line => GetLineSpawnPosition(enemyIndex, totalEnemies),
                SpawnPositionType.Cluster => GetClusterSpawnPosition(),
                _ => GetRandomSpawnPosition()
            };
        }

        /// <summary>
        /// Apply modifications to spawned enemy.
        /// </summary>
        /// <param name="enemy">Enemy to modify.</param>
        /// <param name="enemyIndex">Index of enemy in spawn sequence.</param>
        public void ApplyEnemyModifications(Enemy enemy, int enemyIndex = 0)
        {
            if (enemy == null) return;

            // Apply stat multipliers
            if (HealthMultiplier.HasValue)
                enemy.MaxHealth = (int)(enemy.MaxHealth * HealthMultiplier.Value);

            if (SpeedMultiplier.HasValue)
                enemy.Speed *= SpeedMultiplier.Value;

            if (DamageMultiplier.HasValue)
                enemy.Damage = (int)(enemy.Damage * DamageMultiplier.Value);

            if (SizeMultiplier.HasValue)
                enemy.Size *= SizeMultiplier.Value;

            if (ArmorMultiplier.HasValue)
                enemy.Armor *= ArmorMultiplier.Value;

            // Apply behavior modifications
            foreach (var modifier in BehaviorModifiers)
                modifier.Apply(enemy);

            // Override AI if specified
            if (OverrideAI != AIType.Default)
            {
                enemy.SetAIType(OverrideAI.ToString());
            }

            // Set aggression level
            enemy.SetAggressionLevel((float)Aggression);

            // Apply visual modifications
            ApplyVisualModifications(enemy);

            // Apply champion properties
            if (IsChampion)
            {
                ApplyChampionProperties(enemy);
            }

            // Apply special abilities
            foreach (var ability in SpecialAbilities)
                enemy.AddSpecialAbility(ability);

            // Apply custom properties
            foreach (var property in CustomProperties)
                enemy.SetCustomProperty(property.Key, property.Value);

            // Set spawn group reference
            enemy.SourceSpawnGroup = this;
            enemy.SpawnIndex = enemyIndex;
        }

        /// <summary>
        /// Check if spawn conditions are met.
        /// </summary>
        /// <param name="currentWave">Current wave number.</param>
        /// <param name="gameState">Current game state.</param>
        /// <returns>True if conditions are met.</returns>
        public bool AreSpawnConditionsMet(int currentWave, WaveGameState gameState)
        {
            if (!ConditionalSpawn) return true;

            if (gameState == null) return true;

            return Conditions.AreConditionsMet(currentWave, gameState.PlayerLevel, new List<string>());
        }

        /// <summary>
        /// Check if enemy should be spawned based on filter.
        /// </summary>
        /// <param name="enemy">Enemy to check.</param>
        /// <returns>True if enemy should be spawned.</returns>
        public bool ShouldSpawnEnemy(Enemy enemy)
        {
            if (SpawnFilter == null) return true;

            return SpawnFilter(enemy);
        }

        /// <summary>
        /// Trigger spawn completion callback.
        /// </summary>
        public void OnGroupCompletedCallback() => OnGroupCompleted?.Invoke(this);

        /// <summary>
        /// Trigger enemy spawned callback.
        /// </summary>
        /// <param name="enemy">Spawned enemy.</param>
        public void OnEnemySpawnedCallback(Enemy enemy) => OnEnemySpawned?.Invoke(enemy);

        /// <summary>
        /// Get spawn group description.
        /// </summary>
        /// <returns>Description string.</returns>
        public string GetDescription()
        {
            var description = $"{EnemyType} x{Count}";

            if (IsBoss) description += " (BOSS)";

            if (IsChampion)
                description += $" (Champion Lv.{ChampionLevel})";

            if (Pattern != SpawnPatternType.Line)
                description += $" [{Pattern}]";

            var modifiers = new List<string>();

            if (HealthMultiplier.HasValue && HealthMultiplier.Value != 1f)
                modifiers.Add($"HP x{HealthMultiplier.Value:F1}");

            if (SpeedMultiplier.HasValue && SpeedMultiplier.Value != 1f)
                modifiers.Add($"Speed x{SpeedMultiplier.Value:F1}");

            if (DamageMultiplier.HasValue && DamageMultiplier.Value != 1f)
                modifiers.Add($"DMG x{DamageMultiplier.Value:F1}");

            if (modifiers.Count > 0)
                description += $" ({string.Join(", ", modifiers)})";

            return description;
        }

        /// <summary>
        /// Validate spawn group configuration.
        /// </summary>
        /// <returns>Validation result.</returns>
        public ValidationResult Validate()
        {
            var result = new ValidationResult { IsValid = true };

            if (Count <= 0)
            {
                result.IsValid = false;
                result.AddError("Enemy count must be positive");
            }

            if (SpawnDelay < 0)
            {
                result.IsValid = false;
                result.AddError("Spawn delay cannot be negative");
            }

            if (DelayAfterGroup < 0)
            {
                result.IsValid = false;
                result.AddError("Delay after group cannot be negative");
            }

            if (ChampionLevel < 1)
            {
                result.IsValid = false;
                result.AddError("Champion level must be at least 1");
            }

            if (SpawnRadius < 0)
            {
                result.IsValid = false;
                result.AddError("Spawn radius cannot be negative");
            }

            // Validate stat multipliers
            if (HealthMultiplier.HasValue && HealthMultiplier.Value < 0)
            {
                result.IsValid = false;
                result.AddError("Health multiplier cannot be negative");
            }

            if (SpeedMultiplier.HasValue && SpeedMultiplier.Value < 0)
            {
                result.IsValid = false;
                result.AddError("Speed multiplier cannot be negative");
            }

            if (DamageMultiplier.HasValue && DamageMultiplier.Value < 0)
            {
                result.IsValid = false;
                result.AddError("Damage multiplier cannot be negative");
            }

            if (SizeMultiplier.HasValue && SizeMultiplier.Value < 0)
            {
                result.IsValid = false;
                result.AddError("Size multiplier cannot be negative");
            }

            if (ArmorMultiplier.HasValue && ArmorMultiplier.Value < 0)
            {
                result.IsValid = false;
                result.AddError("Armor multiplier cannot be negative");
            }

            // Validate scale
            if (Scale.HasValue && Scale.Value <= 0)
            {
                result.IsValid = false;
                result.AddError("Scale must be positive");
            }

            // Validate spawn point indices
            if (SpecificSpawnPoints != null)
            {
                foreach (var point in SpecificSpawnPoints)
                {
                    if (point < 0)
                    {
                        result.IsValid = false;
                        result.AddError("Specific spawn point indices cannot be negative");
                        break;
                    }
                }
            }

            // Validate spawn delay variation
            if (SpawnDelayVariation < 0)
            {
                result.IsValid = false;
                result.AddError("Spawn delay variation cannot be negative");
            }

            return result;
        }

        /// <summary>
        /// Clone this spawn group.
        /// </summary>
        /// <remarks>
        /// Event callbacks (OnEnemySpawned, OnGroupCompleted, SpawnFilter) are intentionally NOT copied
        /// to prevent potential memory leaks and unintended side effects from shared delegate references.
        /// Use <see cref="Clone(bool)"/> with includeCallbacks=true if you need to copy callbacks.
        /// </remarks>
        /// <returns>Cloned spawn group.</returns>
        public WaveSpawnGroup Clone()
        {
            var clone = new WaveSpawnGroup
            {
                EnemyType = EnemyType,
                Count = Count,
                SpawnDelay = SpawnDelay,
                Pattern = Pattern,
                DelayAfterGroup = DelayAfterGroup,
                IsBoss = IsBoss,
                SpawnDelayVariation = SpawnDelayVariation,
                RandomizeDelay = RandomizeDelay,
                InitialDelay = InitialDelay,
                PositionType = PositionType,
                SpecificSpawnPoints = new List<int>(SpecificSpawnPoints),
                CustomSpawnPosition = CustomSpawnPosition,
                SpawnRadius = SpawnRadius,
                HealthMultiplier = HealthMultiplier,
                SpeedMultiplier = SpeedMultiplier,
                DamageMultiplier = DamageMultiplier,
                SizeMultiplier = SizeMultiplier,
                ArmorMultiplier = ArmorMultiplier,
                OverrideAI = OverrideAI,
                Aggression = Aggression,
                EnemySkin = EnemySkin,
                TintColor = TintColor,
                Scale = Scale,
                IsChampion = IsChampion,
                ChampionLevel = ChampionLevel,
                SpecialAbilities = new List<string>(SpecialAbilities),
                CustomProperties = new Dictionary<string, float>(CustomProperties),
                ConditionalSpawn = ConditionalSpawn
            };

            clone.BehaviorModifiers = BehaviorModifiers.Select(mod => mod.Clone()).ToList();
            clone.VisualEffects = VisualEffects.Select(effect => effect.Clone()).ToList();
            clone.Conditions = Conditions.Clone();

            return clone;
        }

        /// <summary>
        /// Clone this spawn group with option to copy event callbacks.
        /// </summary>
        /// <param name="includeCallbacks">If true, copies event delegates. Use with caution - can cause memory leaks if not managed properly.</param>
        /// <returns>Cloned spawn group.</returns>
        public WaveSpawnGroup Clone(bool includeCallbacks)
        {
            var clone = Clone();

            if (includeCallbacks)
            {
                clone.OnEnemySpawned = OnEnemySpawned;
                clone.OnGroupCompleted = OnGroupCompleted;
                clone.SpawnFilter = SpawnFilter;
            }

            return clone;
        }

        /// <summary>
        /// Initialize and cache spawn points for this wave.
        /// Call this at wave start to avoid LINQ allocations during spawning.
        /// </summary>
        public void InitializeSpawnPoints()
        {
            _cachedSpawnPoints = GetAvailableSpawnPoints()?.ToArray() ?? GetDefaultSpawnPoints();
            _spawnPointsCached = true;
        }

        /// <summary>
        /// Clear cached spawn points.
        /// Call this after wave completion to free memory.
        /// </summary>
        public void ClearCachedSpawnPoints()
        {
            _cachedSpawnPoints = null;
            _spawnPointsCached = false;
        }

        /// <summary>
        /// Get available spawn points from the navigation grid.
        /// </summary>
        List<Vector3> GetAvailableSpawnPoints()
        {
            var cells = NavigationGrid.Instance?.GetNeighbors(new Vector3Int(0, 0), false);
            if (cells == null) return null;

            var points = new List<Vector3>(cells.Count);

            foreach (var cell in cells)
            {
                points.Add(new Vector3(cell.GridPosition.X, cell.GridPosition.Y, cell.GridPosition.Z));
            }

            return points.Count > 0 ? points : null;
        }

        /// <summary>
        /// Get default fallback spawn points.
        /// </summary>
        Vector3[] GetDefaultSpawnPoints()
        {
            return new[] { Vector3.Zero, new Vector3(10, 0, 0), new Vector3(0, 10, 0) };
        }

        /// <summary>
        /// Get cached spawn points or fetch from grid if not cached.
        /// </summary>
        IReadOnlyList<Vector3> GetSpawnPoints()
        {
            if (_spawnPointsCached && _cachedSpawnPoints != null)
            {
                return _cachedSpawnPoints;
            }

            var points = GetAvailableSpawnPoints();

            if (points != null && points.Count > 0)
            {
                return points;
            }

            return GetDefaultSpawnPoints();
        }

        float ApplyPatternDelay(float baseDelay, int enemyIndex)
        {
            return Pattern switch
            {
                SpawnPatternType.Wave => baseDelay + (MathF.Sin(enemyIndex * 0.5f) * 0.2f),
                SpawnPatternType.Circle => baseDelay + (enemyIndex * 0.1f),
                _ => baseDelay
            };
        }

        Vector3 GetRandomSpawnPosition()
        {
            var spawnPoints = GetSpawnPoints();
            if (spawnPoints.Count == 0) return Vector3.Zero;

            return spawnPoints[_random.Value.Next(0, spawnPoints.Count)];
            
        }

        Vector3 GetSpecificSpawnPosition(int enemyIndex)
        {
            var spawnPoints = GetSpawnPoints();
            if (spawnPoints.Count == 0) return Vector3.Zero;

            if (SpecificSpawnPoints.Count > 0)
            {
                var pointIndex = SpecificSpawnPoints[enemyIndex % SpecificSpawnPoints.Count];

                if (pointIndex >= 0 && pointIndex < spawnPoints.Count)
                    return spawnPoints[pointIndex];
            }

            return spawnPoints[enemyIndex % spawnPoints.Count];
        }

        Vector3 GetPatternSpawnPosition(int enemyIndex, int totalEnemies)
        {
            var spawnPoints = GetSpawnPoints();
            if (spawnPoints.Count == 0) return Vector3.Zero;

            return Pattern switch
            {
                SpawnPatternType.Line => spawnPoints[enemyIndex % spawnPoints.Count],
                SpawnPatternType.Wave => spawnPoints[(int)(MathF.Sin(enemyIndex * 0.5f) * (spawnPoints.Count - 1))],
                SpawnPatternType.Flanking => enemyIndex < spawnPoints.Count / 2 ?
                    spawnPoints[enemyIndex] :
                    spawnPoints[spawnPoints.Count - 1 - (enemyIndex - spawnPoints.Count / 2)],
                SpawnPatternType.Pincer => enemyIndex % 2 == 0 ?
                    spawnPoints[0] :
                    spawnPoints[spawnPoints.Count - 1],
                _ => GetRandomSpawnPosition()
            };
        }

        Vector3 GetCircleSpawnPosition(int enemyIndex, int totalEnemies)
        {
            var center = GetRandomSpawnPosition();
            var angle = (2f * MathF.PI * enemyIndex) / totalEnemies;

            return new Vector3(
                center.X + MathF.Cos(angle) * SpawnRadius,
                center.Y + MathF.Sin(angle) * SpawnRadius,
                center.Z
            );
        }

        Vector3 GetLineSpawnPosition(int enemyIndex, int totalEnemies)
        {
            var basePosition = GetRandomSpawnPosition();
            var spacing = SpawnRadius / totalEnemies;

            return new Vector3(
                basePosition.X + (enemyIndex - totalEnemies / 2f) * spacing,
                basePosition.Y,
                basePosition.Z
            );
        }

        Vector3 GetClusterSpawnPosition()
        {
            var center = GetRandomSpawnPosition();

            var offset = new Vector3(
                 (float)_random.Value.NextDouble() * (SpawnRadius * 2) - SpawnRadius,
                 (float)_random.Value.NextDouble() * (SpawnRadius * 2) - SpawnRadius, 0
                );

            return center + offset;
        }

        void ApplyVisualModifications(Enemy enemy)
        {
            if (!string.IsNullOrEmpty(EnemySkin))
            {
                enemy.SetSkin(EnemySkin);
            }

            if (TintColor.HasValue)
            {
                enemy.SetTintColor(TintColor.Value.R / 255f, TintColor.Value.G / 255f, TintColor.Value.B / 255f, TintColor.Value.A / 255f);
            }

            if (Scale.HasValue) enemy.SetScale(Scale.Value);

            foreach (var effect in VisualEffects)
                enemy.AddVisualEffect(effect);
        }

        /// <summary>
        /// Apply champion properties to an enemy.
        /// Uses 20% stat multiplier per champion level for health and damage.
        /// </summary>
        void ApplyChampionProperties(Enemy enemy)
        {
            enemy.IsChampion = true;
            // TODO: Add ChampionLevel property to Enemy class
            // enemy.ChampionLevel = ChampionLevel;

            // Champion bonuses (20% per level for stats)
            var championBonus = GetChampionStatMultiplier();
            enemy.MaxHealth = (int)(enemy.MaxHealth * championBonus);
            enemy.Damage = (int)(enemy.Damage * championBonus);

            // Visual champion effects
            // enemy.SetChampionVisuals(); // TODO: implement champion visuals
        }

        /// <summary>
        /// Get enemy modifiers as a read-only list.
        /// </summary>
        /// <returns>Read-only list of behavior modifiers.</returns>
        internal IReadOnlyList<EnemyBehaviorModifier> GetEnemyModifiers()
            => BehaviorModifiers?.AsReadOnly();

        #endregion
    }

    /// <summary>
    /// Spawn position types.
    /// </summary>
    public enum SpawnPositionType
    {
        Random,
        Specific,
        Custom,
        Pattern,
        Circle,
        Line,
        Cluster
    }

    /// <summary>
    /// Aggression levels for enemies.
    /// </summary>
    public enum AggressionLevel
    {
        Passive,
        Normal,
        Aggressive,
        Berserk
    }

    /// <summary>
    /// AI types for enemy behavior override.
    /// </summary>
    public enum AIType
    {
        Default,
        Aggressive,
        Defensive,
        Smart,
        Kamikaze,
        Guardian
    }

    /// <summary>
    /// Spawn trigger types.
    /// </summary>
    public enum SpawnTrigger
    {
        Automatic,
        OnTimer,
        OnEnemyDeath,
        OnTowerDestroyed,
        OnPlayerAction,
        Custom
    }

    /// <summary>
    /// Enemy behavior modifier.
    /// </summary>
    public class EnemyBehaviorModifier
    {
        public string ModifierType { get; set; }
        public float Value { get; set; }
        public float Duration { get; set; }
        public Dictionary<string, object> Parameters { get; set; }

        public EnemyBehaviorModifier() => Parameters = new Dictionary<string, object>();

        public void Apply(Enemy enemy)
        {
            var enemyModifier = new Enemies.EnemyBehaviorModifier
            {
                ModifierType = ModifierType,
                Value = Value,
                Duration = Duration,
                Parameters = Parameters ?? new Dictionary<string, object>()
            };

            enemy.ApplyBehaviorModifier(enemyModifier);
        }

        public EnemyBehaviorModifier Clone()
        {
            return new EnemyBehaviorModifier
            {
                ModifierType = ModifierType,
                Value = Value,
                Duration = Duration,
                Parameters = new Dictionary<string, object>(Parameters)
            };
        }
    }

    /// <summary>
    /// Visual effect for enemies.
    /// </summary>
    public class VisualEffect
    {
        public string EffectType { get; set; }
        public WaveSpawnGroup.Color Color { get; set; }
        public float Intensity { get; set; }
        public float Duration { get; set; }
        public Dictionary<string, object> Parameters { get; set; }

        public VisualEffect() => Parameters = new Dictionary<string, object>();

        public VisualEffect Clone()
        {
            return new VisualEffect
            {
                EffectType = EffectType,
                Color = Color,
                Intensity = Intensity,
                Duration = Duration,
                Parameters = new Dictionary<string, object>(Parameters)
            };
        }
    }
}