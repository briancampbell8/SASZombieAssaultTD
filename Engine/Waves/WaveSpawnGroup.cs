using System;
using System.Collections.Generic;
using System.Linq;
using SASZombieAssaultTD.Engine.VectorMath;
using SASZombieAssaultTD.Engine.Extensions;
using SASZombieAssaultTD.Engine.Dictionary;
using SASZombieAssaultTD.Engine.State;
using SASZombieAssaultTD.Engine.Navigation;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Waves
{
    ///<summary>
    ///Enhanced wave spawn group with advanced configuration options.
    ///Provides detailed control over enemy spawning behavior.
    ///</summary>
    public class WaveSpawnGroup : IWaveSpawnGroup
    {
        ///<summary>
        ///Color for visual effects.
        ///</summary>
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

        ///<summary>
        ///Zombie type enumeration.
        ///</summary>
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
        private static readonly System.Random _random = new System.Random();

        //Basic spawn properties
        public WaveSpawnGroup.ZombieType EnemyType { get; set; }
        public int Count { get; set; }
        public float SpawnDelay { get; set; }
        public SpawnPatternType Pattern { get; set; }
        public float DelayAfterGroup { get; set; }
        public bool IsBoss { get; set; }

        //Timing variations
        public float SpawnDelayVariation { get; set; } = 0f;
        public bool RandomizeDelay { get; set; } = false;
        public float InitialDelay { get; set; } = 0f;

        //Spawn position control
        public SpawnPositionType PositionType { get; set; } = SpawnPositionType.Random;
        public List<int> SpecificSpawnPoints { get; set; }
        public Vector3 CustomSpawnPosition { get; set; }
        public float SpawnRadius { get; set; } = 2f;

        //Enemy modifications
        public float? HealthMultiplier { get; set; }
        public float? SpeedMultiplier { get; set; }
        public float? DamageMultiplier { get; set; }
        public float? SizeMultiplier { get; set; }
        public float? ArmorMultiplier { get; set; }

        //Behavioral modifications
        public List<EnemyBehaviorModifier> BehaviorModifiers { get; set; }
        public AIType OverrideAI { get; set; }
        public AggressionLevel Aggression { get; set; } = AggressionLevel.Normal;

        //Visual modifications
        public string EnemySkin { get; set; }
        public WaveSpawnGroup.Color? TintColor { get; set; }
        public float? Scale { get; set; }
        public List<VisualEffect> VisualEffects { get; set; }

        //Spawn conditions
        public SpawnConditions Conditions { get; set; }
        public SpawnTrigger Trigger { get; set; }
        public bool ConditionalSpawn { get; set; }

        //Special properties
        public bool IsChampion { get; set; }
        public int ChampionLevel { get; set; } = 1;
        public List<string> SpecialAbilities { get; set; }
        public Dictionary<string, float> CustomProperties { get; set; }

        //Events and callbacks
        public Action<Enemy> OnEnemySpawned { get; set; }
        public Action<WaveSpawnGroup> OnGroupCompleted { get; set; }
        public Func<Enemy, bool> SpawnFilter { get; set; }

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

        ///<summary>
        ///Get effective spawn delay with variations.
        ///</summary>
        ///<param name="enemyIndex">Index of enemy in spawn sequence.</param>
        ///<returns>Effective spawn delay.</returns>
        public float GetEffectiveSpawnDelay(int enemyIndex = 0)
        {
            var baseDelay = SpawnDelay;

            //Add initial delay for first enemy
            if (enemyIndex == 0)
            {
                baseDelay += InitialDelay;
            }

            //Apply random variation if enabled
            if (RandomizeDelay && SpawnDelayVariation > 0)
            {
                var variation = ((float)_random.NextDouble() - 0.5f) * 2f * SpawnDelayVariation;
                baseDelay += variation;
            }

            //Apply pattern-specific delays
            baseDelay = ApplyPatternDelay(baseDelay, enemyIndex);

            return System.Math.Max(0.1f, baseDelay); //Minimum delay
        }

        ///<summary>
        ///Get effective enemy count with modifiers.
        ///</summary>
        ///<returns>Effective enemy count.</returns>
        public int GetEffectiveCount()
        {
            var count = Count;

            //Apply difficulty-based count increase
            var difficulty = "Normal"; //TODO: Implement proper difficulty system
            var multiplier = difficulty switch
            {
                "Hard" => 1.2f,
                "Elite" => 1.5f,
                _ => 1.0f
            };

            //Apply champion modifier
            if (IsChampion)
            {
                multiplier *= (1f + (ChampionLevel * 0.1f));
            }

            return (int)(count * multiplier);
        }

        ///<summary>
        ///Get spawn position for specific enemy.
        ///</summary>
        ///<param name="enemyIndex">Index of enemy in spawn sequence.</param>
        ///<param name="totalEnemies">Total enemies in this group.</param>
        ///<returns>Spawn position.</returns>
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

        ///<summary>
        ///Apply modifications to spawned enemy.
        ///</summary>
        ///<param name="enemy">Enemy to modify.</param>
        ///<param name="enemyIndex">Index of enemy in spawn sequence.</param>
        public void ApplyEnemyModifications(Enemy enemy, int enemyIndex = 0)
        {
            if (enemy == null) return;

            //Apply stat multipliers
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

            //Apply behavior modifications
            foreach (var modifier in BehaviorModifiers)
            {
                modifier.Apply(enemy);
            }

            //Override AI if specified
            if (OverrideAI != AIType.Default)
            {
                enemy.SetAIType(OverrideAI.ToString());
            }

            //Set aggression level
            enemy.SetAggressionLevel((float)Aggression);

            //Apply visual modifications
            ApplyVisualModifications(enemy);

            //Apply champion properties
            if (IsChampion)
            {
                ApplyChampionProperties(enemy);
            }

            //Apply special abilities
            foreach (var ability in SpecialAbilities)
            {
                enemy.AddSpecialAbility(ability);
            }

            //Apply custom properties
            foreach (var property in CustomProperties)
            {
                enemy.SetCustomProperty(property.Key, property.Value);
            }

            //Set spawn group reference
            enemy.SourceSpawnGroup = this;
            enemy.SpawnIndex = enemyIndex;
        }

        ///<summary>
        ///Check if spawn conditions are met.
        ///</summary>
        ///<param name="currentWave">Current wave number.</param>
        ///<param name="gameState">Current game state.</param>
        ///<returns>True if conditions are met.</returns>
        public bool AreSpawnConditionsMet(int currentWave, VisualEffect.GameState gameState)
        {
            if (!ConditionalSpawn)
                return true;

            if (gameState == null)
                return true;

            return Conditions.AreConditionsMet(currentWave, gameState.PlayerLevel, new List<string>());
        }

        ///<summary>
        ///Check if enemy should be spawned based on filter.
        ///</summary>
        ///<param name="enemy">Enemy to check.</param>
        ///<returns>True if enemy should be spawned.</returns>
        public bool ShouldSpawnEnemy(Enemy enemy)
        {
            if (SpawnFilter == null)
                return true;

            return SpawnFilter(enemy);
        }

        ///<summary>
        ///Trigger spawn completion callback.
        ///</summary>
        public void OnGroupCompletedCallback()
        {
            OnGroupCompleted?.Invoke(this);
        }

        ///<summary>
        ///Trigger enemy spawned callback.
        ///</summary>
        ///<param name="enemy">Spawned enemy.</param>
        public void OnEnemySpawnedCallback(Enemy enemy)
        {
            OnEnemySpawned?.Invoke(enemy);
        }

        ///<summary>
        ///Get spawn group description.
        ///</summary>
        ///<returns>Description string.</returns>
        public string GetDescription()
        {
            var description = $"{EnemyType} x{Count}";

            if (IsBoss)
                description += " (BOSS)";

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

        ///<summary>
        ///Validate spawn group configuration.
        ///</summary>
        ///<returns>Validation result.</returns>
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

            return result;
        }

        ///<summary>
        ///Clone this spawn group.
        ///</summary>
        ///<returns>Cloned spawn group.</returns>
        public WaveSpawnGroup Clone()
        {
            var clone = new WaveSpawnGroup
            {
                EnemyType = this.EnemyType,
                Count = this.Count,
                SpawnDelay = this.SpawnDelay,
                Pattern = this.Pattern,
                DelayAfterGroup = this.DelayAfterGroup,
                IsBoss = this.IsBoss,
                SpawnDelayVariation = this.SpawnDelayVariation,
                RandomizeDelay = this.RandomizeDelay,
                InitialDelay = this.InitialDelay,
                PositionType = this.PositionType,
                SpecificSpawnPoints = new List<int>(this.SpecificSpawnPoints),
                CustomSpawnPosition = this.CustomSpawnPosition,
                SpawnRadius = this.SpawnRadius,
                HealthMultiplier = this.HealthMultiplier,
                SpeedMultiplier = this.SpeedMultiplier,
                DamageMultiplier = this.DamageMultiplier,
                SizeMultiplier = this.SizeMultiplier,
                ArmorMultiplier = this.ArmorMultiplier,
                OverrideAI = this.OverrideAI,
                Aggression = this.Aggression,
                EnemySkin = this.EnemySkin,
                TintColor = this.TintColor,
                Scale = this.Scale,
                IsChampion = this.IsChampion,
                ChampionLevel = this.ChampionLevel,
                SpecialAbilities = new List<string>(this.SpecialAbilities),
                CustomProperties = new Dictionary<string, float>(this.CustomProperties),
                ConditionalSpawn = this.ConditionalSpawn
            };

            clone.BehaviorModifiers = this.BehaviorModifiers.Select(mod => mod.Clone()).ToList();
            clone.VisualEffects = this.VisualEffects.Select(effect => effect.Clone()).ToList();
            clone.Conditions = this.Conditions.Clone();

            return clone;
        }

        /// Private Helper Methods

        private float ApplyPatternDelay(float baseDelay, int enemyIndex)
        {
            return Pattern switch
            {
                SpawnPatternType.Wave => baseDelay + (MathF.Sin(enemyIndex * 0.5f) * 0.2f),
                SpawnPatternType.Circle => baseDelay + (enemyIndex * 0.1f),
                _ => baseDelay
            };
        }

        private Vector3 GetRandomSpawnPosition()
        {
            var spawnPoints = NavigationGrid.Instance?.GetNeighbors(new Vector3Int(0, 0), false)?.Select(cell =>
                new Vector3(cell.GridPosition.X, cell.GridPosition.Y, cell.GridPosition.Z)).ToList() ?? 
                new List<Vector3> { new Vector3(0, 0, 0), new Vector3(10, 0, 0), new Vector3(0, 10, 0) };
            
            if (spawnPoints == null || spawnPoints.Count == 0)
                return Vector3.Zero;

            return spawnPoints[_random.Next(0, spawnPoints.Count)];
        }

        private Vector3 GetSpecificSpawnPosition(int enemyIndex)
        {
            var spawnPoints = NavigationGrid.Instance?.GetNeighbors(new Vector3Int(0, 0), false)?.Select(cell =>
                new Vector3(cell.GridPosition.X, cell.GridPosition.Y, cell.GridPosition.Z)).ToList() ?? 
                new List<Vector3> { new Vector3(0, 0, 0), new Vector3(10, 0, 0), new Vector3(0, 10, 0) };
            if (spawnPoints == null || spawnPoints.Count == 0)
                return Vector3.Zero;

            if (SpecificSpawnPoints.Count > 0)
            {
                var pointIndex = SpecificSpawnPoints[enemyIndex % SpecificSpawnPoints.Count];
                if (pointIndex >= 0 && pointIndex < spawnPoints.Count)
                    return spawnPoints[pointIndex];
            }

            return spawnPoints[enemyIndex % spawnPoints.Count];
        }

        private Vector3 GetPatternSpawnPosition(int enemyIndex, int totalEnemies)
        {
            var spawnPoints = NavigationGrid.Instance?.GetNeighbors(new Vector3Int(0, 0), false)?.Select(cell =>
                new Vector3(cell.GridPosition.X, cell.GridPosition.Y, cell.GridPosition.Z)).ToList() ?? 
                new List<Vector3> { new Vector3(0, 0, 0), new Vector3(10, 0, 0), new Vector3(0, 10, 0) };
            if (spawnPoints == null || spawnPoints.Count == 0)
                return Vector3.Zero;

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

        private Vector3 GetCircleSpawnPosition(int enemyIndex, int totalEnemies)
        {
            var center = GetRandomSpawnPosition();
            var angle = (2f * MathF.PI * enemyIndex) / totalEnemies;

            return new Vector3(
                center.X + MathF.Cos(angle) * SpawnRadius,
                center.Y + MathF.Sin(angle) * SpawnRadius,
                center.Z
            );
        }

        private Vector3 GetLineSpawnPosition(int enemyIndex, int totalEnemies)
        {
            var basePosition = GetRandomSpawnPosition();
            var spacing = SpawnRadius / totalEnemies;

            return new Vector3(
                basePosition.X + (enemyIndex - totalEnemies / 2f) * spacing,
                basePosition.Y,
                basePosition.Z
            );
        }

        private Vector3 GetClusterSpawnPosition()
        {
            var center = GetRandomSpawnPosition();
            var offset = new Vector3(
                (float)_random.NextDouble() * (SpawnRadius * 2) - SpawnRadius,
                (float)_random.NextDouble() * (SpawnRadius * 2) - SpawnRadius,
                0
            );

            return center + offset;
        }

        private void ApplyVisualModifications(Enemy enemy)
        {
            if (!string.IsNullOrEmpty(EnemySkin))
            {
                enemy.SetSkin(EnemySkin);
            }

            if (TintColor.HasValue)
            {
                enemy.SetTintColor(TintColor.Value.R / 255f, TintColor.Value.G / 255f, TintColor.Value.B / 255f, TintColor.Value.A / 255f);
            }

            if (Scale.HasValue)
            {
                enemy.SetScale(Scale.Value);
            }

            foreach (var effect in VisualEffects)
            {
                enemy.AddVisualEffect(effect);
            }
        }

        private void ApplyChampionProperties(Enemy enemy)
        {
            enemy.IsChampion = true;
            //TODO: Add ChampionLevel property to Enemy class
            //enemy.ChampionLevel = ChampionLevel;

            //Champion bonuses
            var championBonus = 1f + (ChampionLevel * 0.2f);
            enemy.MaxHealth = (int)(enemy.MaxHealth * championBonus);
            enemy.Damage = (int)(enemy.Damage * championBonus);

            //Visual champion effects
            //enemy.SetChampionVisuals(); //TODO: implement champion visuals
        }

        ///
    }

    ///<summary>
    ///Spawn position types.
    ///</summary>
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

    ///<summary>
    ///Aggression levels for enemies.
    ///</summary>
    public enum AggressionLevel
    {
        Passive,
        Normal,
        Aggressive,
        Berserk
    }

    ///<summary>
    ///AI types for enemy behavior override.
    ///</summary>
    public enum AIType
    {
        Default,
        Aggressive,
        Defensive,
        Smart,
        Kamikaze,
        Guardian
    }

    ///<summary>
    ///Spawn trigger types.
    ///</summary>
    public enum SpawnTrigger
    {
        Automatic,
        OnTimer,
        OnEnemyDeath,
        OnTowerDestroyed,
        OnPlayerAction,
        Custom
    }

    ///<summary>
    ///Enemy behavior modifier.
    ///</summary>
    public class EnemyBehaviorModifier
    {
        public string ModifierType { get; set; }
        public float Value { get; set; }
        public float Duration { get; set; }
        public Dictionary<string, object> Parameters { get; set; }

        public EnemyBehaviorModifier()
        {
            Parameters = new Dictionary<string, object>();
        }

        public void Apply(Enemy enemy)
        {
            var enemyModifier = new Enemies.EnemyBehaviorModifier
            {
                ModifierType = this.ModifierType,
                Value = this.Value,
                Duration = this.Duration,
                Parameters = this.Parameters ?? new Dictionary<string, object>()
            };
            enemy.ApplyBehaviorModifier(enemyModifier);
        }

        public EnemyBehaviorModifier Clone()
        {
            return new EnemyBehaviorModifier
            {
                ModifierType = this.ModifierType,
                Value = this.Value,
                Duration = this.Duration,
                Parameters = new Dictionary<string, object>(this.Parameters)
            };
        }
    }

    ///<summary>
    ///Visual effect for enemies.
    ///</summary>
    public class VisualEffect
    {
        public string EffectType { get; set; }
        public WaveSpawnGroup.Color Color { get; set; }
        public float Intensity { get; set; }
        public float Duration { get; set; }
        public Dictionary<string, object> Parameters { get; set; }

        public VisualEffect()
        {
            Parameters = new Dictionary<string, object>();
        }

        public VisualEffect Clone()
        {
            return new VisualEffect
            {
                EffectType = this.EffectType,
                Color = this.Color,
                Intensity = this.Intensity,
                Duration = this.Duration,
                Parameters = new Dictionary<string, object>(this.Parameters)
            };
        }

        ///<summary>
        ///Game state for wave system.
        ///</summary>
        public class GameState
        {
            public int PlayerLevel { get; set; }
            public int BuiltTowers { get; set; }
            public float GameSpeed { get; set; }
            public bool IsPaused { get; set; }

            public GameState()
            {
                PlayerLevel = 1;
                BuiltTowers = 0;
                GameSpeed = 1f;
                IsPaused = false;
            }
        }
    }
}
