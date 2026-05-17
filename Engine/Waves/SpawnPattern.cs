using System;
using System.Collections.Generic;
using System.Linq;
using SASZombieAssaultTD.Engine.Extensions;
using SASZombieAssaultTD.Engine.VectorMath;
using SASZombieAssaultTD.Engine.Navigation;
using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.Dictionary;
using SASZombieAssaultTD.Engine.Core.Random;

namespace SASZombieAssaultTD.Engine.Waves
{
    /// <summary>
    /// Spawn pattern parameters for configuration.
    /// </summary>
    public sealed class SpawnPatternParameterss
    {
        public Vector3 Center { get; set; }
        public float? Radius { get; set; }
        public int Count { get; set; }
        public SpawnPatternType Type { get; set; }
        public float? Spread { get; set; }
        public float? Depth { get; set; }
        public float? RadiusEnd { get; set; }
        public float? RadiusStart { get; set; }
        public Vector3? CenterPoint { get; set; }
        public Vector3Int? GridSize { get; set; }
        public float? Spacing { get; set; }
        public Rectangle? Bounds { get; set; }
        public float? Amplitude { get; set; }
        public float? Frequency { get; set; }
        public Dictionary<string, object> CustomProperties { get; set; }

        public SpawnPatternParameterss()
        {
            CustomProperties = new Dictionary<string, object>();
        }
    }

    /// <summary>
    /// Spawn pattern description for UI display.
    /// </summary>
    public sealed class SpawnPatternDescription
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int MinEnemies { get; set; }
        public int MaxEnemies { get; set; }
        public List<SpawnPatternParameters> Parameters { get; set; }

        public SpawnPatternDescription()
        {
            Parameters = new List<SpawnPatternParameters>();
        }
    }

    /// <summary>
    /// Spawn pattern parameter for UI display.
    /// </summary>
    public sealed class SpawnPatternParameters
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public object DefaultValue { get; set; }
        public float MinValue { get; set; }
        public float MaxValue { get; set; }
        public ParameterType Type { get; set; }
        public bool Required { get; set; }
    }

    /// <summary>
    /// Spawn pattern validation result for spawn pattern parameters.
    /// </summary>
    public sealed class SpawnPatternValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; }

        public SpawnPatternValidationResult()
        {
            Errors = new List<string>();
        }

        public void AddError(string error)
        {
            Errors.Add(error);
        }
    }

    /// <summary>
    /// Spawn pattern system for SAS Zombie Assault TD.
    /// Defines how enemies are positioned and arranged during spawning.
    /// </summary>
    public class SpawnPattern
    {
        private readonly Dictionary<SpawnPatternType, ISpawnPatternStrategy> _strategies;
        private static SpawnPattern _instance;

        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static SpawnPattern Instance => _instance ??= new SpawnPattern();

        private SpawnPattern()
        {
            _strategies = new Dictionary<SpawnPatternType, ISpawnPatternStrategy>();
            InitializeStrategies();
        }

        /// <summary>
        /// Create a SpawnPattern instance from a SpawnPatternType.
        /// </summary>
        /// <param name="type">The pattern type.</param>
        /// <returns>A SpawnPattern instance configured for the specified type.</returns>
        public static SpawnPattern FromType(SpawnPatternType type)
        {
            var pattern = new SpawnPattern();
            // The pattern is already configured with all strategies via InitializeStrategies()
            return pattern;
        }

        /// <summary>
        /// Get spawn positions for a specific pattern.
        /// </summary>
        /// <param name="patternType">Type of spawn pattern.</param>
        /// <param name="enemyCount">Number of enemies to spawn.</param>
        /// <param name="parameters">Pattern-specific parameters.</param>
        /// <returns>List of spawn positions.</returns>
        public List<Vector3> GetSpawnPositions(SpawnPatternType patternType, int enemyCount, SpawnPatternParameterss parameters = null)
        {
            if (!_strategies.TryGetValue(patternType, out var strategy))
            {
                Console.WriteLine($"No strategy found for pattern type: {patternType}");
                return GetDefaultPositions(enemyCount);
            }

            try
            {
                return strategy.GeneratePositions(enemyCount, parameters ?? new SpawnPatternParameterss());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating spawn positions for {patternType}: {ex.Message}");
                return GetDefaultPositions(enemyCount);
            }
        }

        /// <summary>
        /// Register a custom spawn pattern strategy.
        /// </summary>
        /// <param name="patternType">Pattern type to register.</param>
        /// <param name="strategy">Strategy implementation.</param>
        public void RegisterStrategy(SpawnPatternType patternType, ISpawnPatternStrategy strategy)
        {
            _strategies[patternType] = strategy;
            Console.WriteLine($"Registered custom strategy for {patternType}");
        }

        /// <summary>
        /// Get pattern description and requirements.
        /// </summary>
        /// <param name="patternType">Pattern type to describe.</param>
        /// <returns>Pattern description.</returns>
        public SpawnPatternDescription GetPatternDescription(SpawnPatternType patternType)
        {
            if (!_strategies.TryGetValue(patternType, out var strategy))
            {
                return new SpawnPatternDescription
                {
                    Name = patternType.ToString(),
                    Description = "Unknown pattern",
                    MinEnemies = 1,
                    MaxEnemies = 100,
                    Parameters = new List<SpawnPatternParameters>()
                };
            }

            return strategy.GetDescription();
        }

        /// <summary>
        /// Validate pattern parameters.
        /// </summary>
        /// <param name="patternType">Pattern type to validate.</param>
        /// <param name="parameters">Parameters to validate.</param>
        /// <returns>Validation result.</returns>
        public ValidationResult ValidateParameters(SpawnPatternType patternType, SpawnPatternParameterss parameters)
        {
            if (!_strategies.TryGetValue(patternType, out var strategy))
            {
                var result = new ValidationResult { IsValid = false };
                result.AddError($"Unknown pattern type: {patternType}");
                return result;
            }

            return strategy.ValidateParameters(parameters);
        }

        /// <summary>
        /// Get default spawn positions (fallback).
        /// </summary>
        /// <param name="enemyCount">Number of enemies.</param>
        /// <returns>List of default positions.</returns>
        private List<Vector3> GetDefaultPositions(int enemyCount)
        {
            var positions = new List<Vector3>();
            var spawnPoints = NavigationGrid.Instance?.GetSpawnPoints();

            if (spawnPoints == null || spawnPoints.Count == 0)
            {
                // Fallback to origin
                for (int i = 0; i < enemyCount; i++)
                {
                    positions.Add(new Vector3(0, i * 1f, 0));
                }
                return positions;
            }

            // Distribute across spawn points
            for (int i = 0; i < enemyCount; i++)
            {
                var spawnPoint = spawnPoints[i % spawnPoints.Count];
                var position = spawnPoint is Vector3 vec ? vec : (Vector3)spawnPoint;
                positions.Add(position);
            }

            return positions;
        }

        /// <summary>
        /// Initialize default spawn pattern strategies.
        /// </summary>
        private void InitializeStrategies()
        {
            // Register all built-in strategies
            _strategies[SpawnPatternType.Line] = new SingleSpawnStrategy();
            _strategies[SpawnPatternType.Line] = new LineSpawnStrategy();
            _strategies[SpawnPatternType.Cluster] = new ClusterSpawnStrategy();
            _strategies[SpawnPatternType.Spread] = new SpreadSpawnStrategy();
            _strategies[SpawnPatternType.Wave] = new WaveSpawnStrategy();
            _strategies[SpawnPatternType.Circle] = new CircleSpawnStrategy();
            _strategies[SpawnPatternType.Random] = new RandomSpawnStrategy();
            _strategies[SpawnPatternType.Flanking] = new FlankingSpawnStrategy();
            _strategies[SpawnPatternType.Pincer] = new PincerSpawnStrategy();
            _strategies[SpawnPatternType.Spiral] = new SpiralSpawnStrategy();
            _strategies[SpawnPatternType.Cluster] = new GridSpawnStrategy();
            _strategies[SpawnPatternType.VFormation] = new VFormationSpawnStrategy();

            Console.WriteLine($"Initialized {_strategies.Count} spawn pattern strategies");
        }
    }

    /// <summary>
    /// Interface for spawn pattern strategies.
    /// </summary>
    public interface ISpawnPatternStrategy
    {
        /// <summary>
        /// Generate spawn positions for this pattern.
        /// </summary>
        /// <param name="enemyCount">Number of enemies to spawn.</param>
        /// <param name="parameters">Pattern parameters.</param>
        /// <returns>List of spawn positions.</returns>
        List<Vector3> GeneratePositions(int enemyCount, SpawnPatternParameterss parameters);

        /// <summary>
        /// Get pattern description.
        /// </summary>
        /// <returns>Pattern description.</returns>
        SpawnPatternDescription GetDescription();

        /// <summary>
        /// Validate pattern parameters.
        /// </summary>
        /// <param name="parameters">Parameters to validate.</param>
        /// <returns>Validation result.</returns>
        ValidationResult ValidateParameters(SpawnPatternParameterss parameters);
    }

    /// <summary>
    /// Base class for spawn pattern strategies.
    /// </summary>
    public abstract class BaseSpawnStrategy : ISpawnPatternStrategy
    {
        protected List<Vector3> GetSpawnPoints()
        {
            var spawnPoints = NavigationGrid.Instance?.GetSpawnPoints();
            return spawnPoints?.Cast<Vector3>().ToList() ?? new List<Vector3> { Vector3.Zero };
        }

        protected Vector3 GetRandomSpawnPoint()
        {
            var spawnPoints = GetSpawnPoints();
            if (spawnPoints.Count == 0)
                return Vector3.Zero;
            return spawnPoints[EngineRandom.Range(0, spawnPoints.Count)];
        }

        protected Vector3 GetSpawnPoint(int index)
        {
            var spawnPoints = GetSpawnPoints();
            if (spawnPoints.Count == 0)
                return Vector3.Zero;

            return spawnPoints[index % spawnPoints.Count];
        }

        public abstract List<Vector3> GeneratePositions(int enemyCount, SpawnPatternParameterss parameters);
        public abstract SpawnPatternDescription GetDescription();
        public abstract ValidationResult ValidateParameters(SpawnPatternParameterss parameters);
    }

    /// <summary>
    /// Single spawn strategy - all enemies from one point.
    /// </summary>
    public class SingleSpawnStrategy : BaseSpawnStrategy
    {
        public override List<Vector3> GeneratePositions(int enemyCount, SpawnPatternParameterss parameters)
        {
            var positions = new List<Vector3>();
            var spawnPoint = GetRandomSpawnPoint();

            for (int i = 0; i < enemyCount; i++)
            {
                // Add small offset to prevent exact overlap
                var offset = new Vector3(
                    EngineRandom.Range(-0.2f, 0.2f),
                    EngineRandom.Range(-0.2f, 0.2f),
                    0
                );
                positions.Add(spawnPoint + offset);
            }

            return positions;
        }

        public override SpawnPatternDescription GetDescription()
        {
            return new SpawnPatternDescription
            {
                Name = "Single",
                Description = "All enemies spawn from a single point",
                MinEnemies = 1,
                MaxEnemies = 50,
                Parameters = new List<SpawnPatternParameters>
                {
                    new SpawnPatternParameters
                    {
                        Name = "CustomSpawnPosition",
                        Type = ParameterType.Vector3,
                        Description = "Custom spawn position override",
                        Required = false
                    }
                }
            };
        }

        public override ValidationResult ValidateParameters(SpawnPatternParameterss parameters)
        {
            return new ValidationResult { IsValid = true };
        }
    }

    /// <summary>
    /// Line spawn strategy - enemies in a line formation.
    /// </summary>
    public class LineSpawnStrategy : BaseSpawnStrategy
    {
        public override List<Vector3> GeneratePositions(int enemyCount, SpawnPatternParameterss parameters)
        {
            var positions = new List<Vector3>();
            var spawnPoints = GetSpawnPoints();

            if (spawnPoints.Count == 0)
                return positions;

            var spacing = parameters.Spacing ?? 1f;
            var startPoint = Vector3.Zero;
            var direction = Vector3.Right;

            for (int i = 0; i < enemyCount; i++)
            {
                var position = startPoint + (direction * i * spacing);
                positions.Add(position);
            }

            return positions;
        }

        public override SpawnPatternDescription GetDescription()
        {
            return new SpawnPatternDescription
            {
                Name = "Line",
                Description = "Enemies spawn in a straight line formation",
                MinEnemies = 2,
                MaxEnemies = 20,
                Parameters = new List<SpawnPatternParameters>
                {
                    new SpawnPatternParameters
                    {
                        Name = "Spacing",
                        Type = ParameterType.Float,
                        Description = "Distance between enemies",
                        DefaultValue = 1f,
                        MinValue = 0.5f,
                        MaxValue = 3f
                    },
                    new SpawnPatternParameters
                    {
                        Name = "Direction",
                        Type = ParameterType.Vector3,
                        Description = "Direction of line formation",
                        DefaultValue = Vector3.Right
                    },
                    new SpawnPatternParameters
                    {
                        Name = "StartPoint",
                        Type = ParameterType.Vector3,
                        Description = "Starting position for line",
                        Required = false
                    }
                }
            };
        }

        public override ValidationResult ValidateParameters(SpawnPatternParameterss parameters)
        {
            var result = new ValidationResult { IsValid = true };

            if (parameters.Spacing.HasValue && parameters.Spacing.Value <= 0)
            {
                result.IsValid = false;
                result.AddError("Spacing must be positive");
            }

            return result;
        }
    }

    /// <summary>
    /// Cluster spawn strategy - enemies grouped together.
    /// </summary>
    public class ClusterSpawnStrategy : BaseSpawnStrategy
    {
        public override List<Vector3> GeneratePositions(int enemyCount, SpawnPatternParameterss parameters)
        {
            var positions = new List<Vector3>();
            var centerPoint = parameters.CenterPoint ?? GetRandomSpawnPoint();
            var radius = parameters.Radius.HasValue ? parameters.Radius.Value : 2f;

            for (int i = 0; i < enemyCount; i++)
            {
                // Generate random position within cluster radius
                var angle = System.Random.value * 2f * System.MathF.PI;
                var distance = System.Random.value * radius;

                var position = new Vector3(
                    centerPoint.X + MathF.Cos(angle) * distance,
                    centerPoint.Y + MathF.Sin(angle) * distance,
                    centerPoint.Z
                );

                positions.Add(position);
            }

            return positions;
        }

        public override SpawnPatternDescription GetDescription()
        {
            return new SpawnPatternDescription
            {
                Name = "Cluster",
                Description = "Enemies spawn in a clustered group",
                MinEnemies = 3,
                MaxEnemies = 30,
                Parameters = new List<SpawnPatternParameters>
                {
                    new SpawnPatternParameters
                    {
                        Name = "Radius",
                        Type = ParameterType.Float,
                        Description = "Radius of cluster area",
                        DefaultValue = 2f,
                        MinValue = 0.5f,
                        MaxValue = 5f
                    },
                    new SpawnPatternParameters
                    {
                        Name = "CenterPoint",
                        Type = ParameterType.Vector3,
                        Description = "Center of cluster",
                        Required = false
                    }
                }
            };
        }

        public override ValidationResult ValidateParameters(SpawnPatternParameterss parameters)
        {
            var result = new ValidationResult { IsValid = true };

            if (parameters.Radius.HasValue && parameters.Radius.Value <= 0)
            {
                result.IsValid = false;
                result.AddError("Radius must be positive");
            }

            return result;
        }
    }

    /// <summary>
    /// Spread spawn strategy - enemies distributed across spawn points.
    /// </summary>
    public class SpreadSpawnStrategy : BaseSpawnStrategy
    {
        public override List<Vector3> GeneratePositions(int enemyCount, SpawnPatternParameterss parameters)
        {
            var positions = new List<Vector3>();
            var spawnPoints = GetSpawnPoints();

            if (spawnPoints.Count == 0)
                return positions;

            // Distribute enemies across all spawn points
            for (int i = 0; i < enemyCount; i++)
            {
                var spawnPoint = spawnPoints[i % spawnPoints.Count];

                // Add small random offset
                var offset = new Vector3(
                    EngineRandom.Range(-0.5f, 0.5f),
                    EngineRandom.Range(-0.5f, 0.5f),
                    0
                );

                positions.Add(spawnPoint + offset);
            }

            return positions;
        }

        public override SpawnPatternDescription GetDescription()
        {
            return new SpawnPatternDescription
            {
                Name = "Spread",
                Description = "Enemies distributed across all spawn points",
                MinEnemies = 2,
                MaxEnemies = 100,
                Parameters = new List<SpawnPatternParameters>()
            };
        }

        public override ValidationResult ValidateParameters(SpawnPatternParameterss parameters)
        {
            return new ValidationResult { IsValid = true };
        }
    }

    /// <summary>
    /// Wave spawn strategy - enemies in wave pattern.
    /// </summary>
    public class WaveSpawnStrategy : BaseSpawnStrategy
    {
        public override List<Vector3> GeneratePositions(int enemyCount, SpawnPatternParameterss parameters)
        {
            var positions = new List<Vector3>();
            var spawnPoints = GetSpawnPoints();

            if (spawnPoints.Count == 0)
                return positions;

            var amplitude = parameters.Amplitude ?? 2f;
            var frequency = parameters.Frequency ?? 0.5f;
            var basePoint = GetSpawnPoints().Count > 0 ? GetSpawnPoints()[0] : Vector3.Zero;

            for (int i = 0; i < enemyCount; i++)
            {
                var waveOffset = MathF.Sin(i * frequency) * amplitude;
                var position = new Vector3(
                    basePoint.X + i * 1f,
                    basePoint.Y + waveOffset,
                    basePoint.Z
                );

                positions.Add(position);
            }

            return positions;
        }

        public override SpawnPatternDescription GetDescription()
        {
            return new SpawnPatternDescription
            {
                Name = "Wave",
                Description = "Enemies spawn in a wave pattern",
                MinEnemies = 3,
                MaxEnemies = 50,
                Parameters = new List<SpawnPatternParameters>
                {
                    new SpawnPatternParameters
                    {
                        Name = "Amplitude",
                        Type = ParameterType.Float,
                        Description = "Wave amplitude",
                        DefaultValue = 2f,
                        MinValue = 0.5f,
                        MaxValue = 5f
                    },
                    new SpawnPatternParameters
                    {
                        Name = "Frequency",
                        Type = ParameterType.Float,
                        Description = "Wave frequency",
                        DefaultValue = 0.5f,
                        MinValue = 0.1f,
                        MaxValue = 2f
                    },
                    new SpawnPatternParameters
                    {
                        Name = "BasePoint",
                        Type = ParameterType.Vector3,
                        Description = "Base point for wave",
                        Required = false
                    }
                }
            };
        }

        public override ValidationResult ValidateParameters(SpawnPatternParameterss parameters)
        {
            var result = new ValidationResult { IsValid = true };

            if (parameters.Amplitude.HasValue && parameters.Amplitude.Value < 0)
            {
                result.IsValid = false;
                result.AddError("Amplitude cannot be negative");
            }

            if (parameters.Frequency.HasValue && parameters.Frequency.Value <= 0)
            {
                result.IsValid = false;
                result.AddError("Frequency must be positive");
            }

            return result;
        }
    }

    /// <summary>
    /// Circle spawn strategy - enemies in circular formation.
    /// </summary>
    public class CircleSpawnStrategy : BaseSpawnStrategy
    {
        public override List<Vector3> GeneratePositions(int enemyCount, SpawnPatternParameterss parameters)
        {
            var positions = new List<Vector3>();
            var centerPoint = parameters.CenterPoint ?? GetRandomSpawnPoint();
            var radius = parameters.Radius.HasValue ? parameters.Radius.Value : 3f;

            for (int i = 0; i < enemyCount; i++)
            {
                var angle = (2f * MathF.PI * i) / enemyCount;
                var position = new Vector3(
                    centerPoint.X + MathF.Cos(angle) * radius,
                    centerPoint.Y + MathF.Sin(angle) * radius,
                    centerPoint.Z
                );

                positions.Add(position);
            }

            return positions;
        }

        public override SpawnPatternDescription GetDescription()
        {
            return new SpawnPatternDescription
            {
                Name = "Circle",
                Description = "Enemies spawn in a circular formation",
                MinEnemies = 3,
                MaxEnemies = 20,
                Parameters = new List<SpawnPatternParameters>
                {
                    new SpawnPatternParameters
                    {
                        Name = "Radius",
                        Type = ParameterType.Float,
                        Description = "Circle radius",
                        DefaultValue = 3f,
                        MinValue = 1f,
                        MaxValue = 8f
                    },
                    new SpawnPatternParameters
                    {
                        Name = "CenterPoint",
                        Type = ParameterType.Vector3,
                        Description = "Center of circle",
                        Required = false
                    }
                }
            };
        }

        public override ValidationResult ValidateParameters(SpawnPatternParameterss parameters)
        {
            var result = new ValidationResult { IsValid = true };

            if (parameters.Radius.HasValue && parameters.Radius.Value <= 0)
            {
                result.IsValid = false;
                result.AddError("Radius must be positive");
            }

            return result;
        }
    }

    /// <summary>
    /// Random spawn strategy - enemies at random positions.
    /// </summary>
    public class RandomSpawnStrategy : BaseSpawnStrategy
    {
        public override List<Vector3> GeneratePositions(int enemyCount, SpawnPatternParameterss parameters)
        {
            var positions = new List<Vector3>();
            var spawnPoints = GetSpawnPoints();

            if (spawnPoints.Count == 0)
                return positions;

            // TODO: Fix type mismatch - Bounds is Rect?, can't use ?? with Vector3
            // var bounds = parameters.Bounds ?? new Vector3(10f, 10f, 0f);
            var bounds = new Vector3(10f, 10f, 0f);
            var center = parameters.CenterPoint ?? spawnPoints[0];

            for (int i = 0; i < enemyCount; i++)
            {
                var position = new Vector3(
                    center.X + EngineRandom.Range(-bounds.X / 2f, bounds.X / 2f),
                    center.Y + EngineRandom.Range(-bounds.Y / 2f, bounds.Y / 2f),
                    center.Z
                );

                positions.Add(position);
            }

            return positions;
        }

        public override SpawnPatternDescription GetDescription()
        {
            return new SpawnPatternDescription
            {
                Name = "Random",
                Description = "Enemies spawn at random positions",
                MinEnemies = 1,
                MaxEnemies = 100,
                Parameters = new List<SpawnPatternParameters>
                {
                    new SpawnPatternParameters
                    {
                        Name = "Bounds",
                        Type = ParameterType.Vector3,
                        Description = "Random spawn boundaries",
                        DefaultValue = new Vector3(10f, 10f, 0f)
                    },
                    new SpawnPatternParameters
                    {
                        Name = "CenterPoint",
                        Type = ParameterType.Vector3,
                        Description = "Center of random area",
                        Required = false
                    }
                }
            };
        }

        public override ValidationResult ValidateParameters(SpawnPatternParameterss parameters)
        {
            return new ValidationResult { IsValid = true };
        }
    }

    /// <summary>
    /// Flanking spawn strategy - enemies from multiple sides.
    /// </summary>
    public class FlankingSpawnStrategy : BaseSpawnStrategy
    {
        public override List<Vector3> GeneratePositions(int enemyCount, SpawnPatternParameterss parameters)
        {
            var positions = new List<Vector3>();
            var spawnPoints = GetSpawnPoints();

            if (spawnPoints.Count < 2)
                return positions;

            var leftSide = enemyCount / 2;
            var rightSide = enemyCount - leftSide;

            // Left side enemies
            for (int i = 0; i < leftSide; i++)
            {
                var spawnPoint = spawnPoints[i % (spawnPoints.Count / 2)];
                positions.Add(spawnPoint);
            }

            // Right side enemies
            for (int i = 0; i < rightSide; i++)
            {
                var spawnPoint = spawnPoints[(spawnPoints.Count / 2) + (i % (spawnPoints.Count / 2))];
                positions.Add(spawnPoint);
            }

            return positions;
        }

        public override SpawnPatternDescription GetDescription()
        {
            return new SpawnPatternDescription
            {
                Name = "Flanking",
                Description = "Enemies flank from multiple sides",
                MinEnemies = 4,
                MaxEnemies = 50,
                Parameters = new List<SpawnPatternParameters>()
            };
        }

        public override ValidationResult ValidateParameters(SpawnPatternParameterss parameters)
        {
            return new ValidationResult { IsValid = true };
        }
    }

    /// <summary>
    /// Pincer spawn strategy - enemies attack from two opposite sides.
    /// </summary>
    public class PincerSpawnStrategy : BaseSpawnStrategy
    {
        public override List<Vector3> GeneratePositions(int enemyCount, SpawnPatternParameterss parameters)
        {
            var positions = new List<Vector3>();
            var spawnPoints = GetSpawnPoints();

            if (spawnPoints.Count < 2)
                return positions;

            var leftPoint = spawnPoints[0];
            var rightPoint = spawnPoints[spawnPoints.Count - 1];

            for (int i = 0; i < enemyCount; i++)
            {
                var position = (i % 2 == 0) ? leftPoint : rightPoint;
                positions.Add(position);
            }

            return positions;
        }

        public override SpawnPatternDescription GetDescription()
        {
            return new SpawnPatternDescription
            {
                Name = "Pincer",
                Description = "Enemies attack in pincer formation",
                MinEnemies = 2,
                MaxEnemies = 20,
                Parameters = new List<SpawnPatternParameters>()
            };
        }

        public override ValidationResult ValidateParameters(SpawnPatternParameterss parameters)
        {
            return new ValidationResult { IsValid = true };
        }
    }

    /// <summary>
    /// Spiral spawn strategy - enemies in spiral pattern.
    /// </summary>
    public class SpiralSpawnStrategy : BaseSpawnStrategy
    {
        public override List<Vector3> GeneratePositions(int enemyCount, SpawnPatternParameterss parameters)
        {
            var positions = new List<Vector3>();
            var centerPoint = parameters.CenterPoint ?? GetRandomSpawnPoint();
            var radiusStart = parameters.RadiusStart ?? 1f;
            var radiusEnd = parameters.RadiusEnd ?? 5f;

            for (int i = 0; i < enemyCount; i++)
            {
                var progress = (float)i / (enemyCount - 1);
                var radius = radiusStart + (radiusEnd - radiusStart) * progress;
                var angle = i * 0.5f; // Spiral rotation

                var position = new Vector3(
                    centerPoint.X + MathF.Cos(angle) * radius,
                    centerPoint.Y + MathF.Sin(angle) * radius,
                    centerPoint.Z
                );

                positions.Add(position);
            }

            return positions;
        }

        public override SpawnPatternDescription GetDescription()
        {
            return new SpawnPatternDescription
            {
                Name = "Spiral",
                Description = "Enemies spawn in spiral pattern",
                MinEnemies = 3,
                MaxEnemies = 30,
                Parameters = new List<SpawnPatternParameters>
                {
                    new SpawnPatternParameters
                    {
                        Name = "RadiusStart",
                        Type = ParameterType.Float,
                        Description = "Starting radius",
                        DefaultValue = 1f,
                        MinValue = 0.5f,
                        MaxValue = 3f
                    },
                    new SpawnPatternParameters
                    {
                        Name = "RadiusEnd",
                        Type = ParameterType.Float,
                        Description = "Ending radius",
                        DefaultValue = 5f,
                        MinValue = 2f,
                        MaxValue = 10f
                    }
                }
            };
        }

        public override ValidationResult ValidateParameters(SpawnPatternParameterss parameters)
        {
            var result = new ValidationResult { IsValid = true };

            if (parameters.RadiusStart.HasValue && parameters.RadiusStart.Value <= 0)
            {
                result.IsValid = false;
                result.AddError("RadiusStart must be positive");
            }

            if (parameters.RadiusEnd.HasValue && parameters.RadiusEnd.Value <= 0)
            {
                result.IsValid = false;
                result.AddError("RadiusEnd must be positive");
            }

            if (parameters.RadiusStart.HasValue && parameters.RadiusEnd.HasValue &&
                parameters.RadiusStart.Value >= parameters.RadiusEnd.Value)
            {
                result.IsValid = false;
                result.AddError("RadiusStart must be less than RadiusEnd");
            }

            return result;
        }
    }

    /// <summary>
    /// Grid spawn strategy - enemies in grid formation.
    /// </summary>
    public class GridSpawnStrategy : BaseSpawnStrategy
    {
        public override List<Vector3> GeneratePositions(int enemyCount, SpawnPatternParameterss parameters)
        {
            var positions = new List<Vector3>();
            var centerPoint = parameters.CenterPoint ?? GetRandomSpawnPoint();
            var gridSize = parameters.GridSize?.X ?? 3;
            var spacing = parameters.Spacing ?? 1f;

            var index = 0;
            for (int x = 0; x < gridSize && index < enemyCount; x++)
            {
                for (int y = 0; y < gridSize && index < enemyCount; y++)
                {
                    var position = new Vector3(
                        centerPoint.X + (x - gridSize / 2f) * spacing,
                        centerPoint.Y + (y - gridSize / 2f) * spacing,
                        centerPoint.Z
                    );

                    positions.Add(position);
                    index++;
                }
            }

            return positions;
        }

        public override SpawnPatternDescription GetDescription()
        {
            return new SpawnPatternDescription
            {
                Name = "Grid",
                Description = "Enemies spawn in grid formation",
                MinEnemies = 4,
                MaxEnemies = 25,
                Parameters = new List<SpawnPatternParameters>
                {
                    new SpawnPatternParameters
                    {
                        Name = "GridSize",
                        Type = ParameterType.Int,
                        Description = "Size of grid (NxN)",
                        DefaultValue = 3,
                        MinValue = 2,
                        MaxValue = 5
                    },
                    new SpawnPatternParameters
                    {
                        Name = "Spacing",
                        Type = ParameterType.Float,
                        Description = "Spacing between grid points",
                        DefaultValue = 1f,
                        MinValue = 0.5f,
                        MaxValue = 2f
                    }
                }
            };
        }

        public override ValidationResult ValidateParameters(SpawnPatternParameterss parameters)
        {
            var result = new ValidationResult { IsValid = true };

            if (parameters.GridSize.HasValue && (parameters.GridSize.Value.X < 2 || parameters.GridSize.Value.X > 5))
            {
                result.IsValid = false;
                result.AddError("GridSize must be between 2 and 5");
            }

            if (parameters.Spacing.HasValue && parameters.Spacing.Value <= 0)
            {
                result.IsValid = false;
                result.AddError("Spacing must be positive");
            }

            return result;
        }
    }

    /// <summary>
    /// V-formation spawn strategy - enemies in V formation.
    /// </summary>
    public class VFormationSpawnStrategy : BaseSpawnStrategy
    {
        public override List<Vector3> GeneratePositions(int enemyCount, SpawnPatternParameterss parameters)
        {
            var positions = new List<Vector3>();
            var centerPoint = parameters.CenterPoint ?? GetRandomSpawnPoint();
            var spread = parameters.Spread ?? 2f;
            var depth = parameters.Depth ?? 3f;

            // Tip of V (first enemy)
            positions.Add(centerPoint);

            // Left and right arms of V
            var armSize = (enemyCount - 1) / 2;
            for (int i = 1; i <= armSize; i++)
            {
                var leftOffset = new Vector3(-i * spread / armSize, -i * depth / armSize, 0);
                var rightOffset = new Vector3(i * spread / armSize, -i * depth / armSize, 0);

                positions.Add(centerPoint + leftOffset);
                positions.Add(centerPoint + rightOffset);
            }

            // Handle odd number (extra enemy on right arm)
            if (enemyCount % 2 == 0)
            {
                var extraOffset = new Vector3(0, -depth, 0);
                positions.Add(centerPoint + extraOffset);
            }

            return positions;
        }

        public override SpawnPatternDescription GetDescription()
        {
            return new SpawnPatternDescription
            {
                Name = "V Formation",
                Description = "Enemies spawn in V formation",
                MinEnemies = 3,
                MaxEnemies = 15,
                Parameters = new List<SpawnPatternParameters>
                {
                    new SpawnPatternParameters
                    {
                        Name = "Spread",
                        Type = ParameterType.Float,
                        Description = "Width of V formation",
                        DefaultValue = 2f,
                        MinValue = 1f,
                        MaxValue = 5f
                    },
                    new SpawnPatternParameters
                    {
                        Name = "Depth",
                        Type = ParameterType.Float,
                        Description = "Depth of V formation",
                        DefaultValue = 3f,
                        MinValue = 1f,
                        MaxValue = 6f
                    }
                }
            };
        }

        public override ValidationResult ValidateParameters(SpawnPatternParameterss parameters)
        {
            var result = new ValidationResult { IsValid = true };

            if (parameters.Spread.HasValue && parameters.Spread.Value <= 0)
            {
                result.IsValid = false;
                result.AddError("Spread must be positive");
            }

            if (parameters.Depth.HasValue && parameters.Depth.Value <= 0)
            {
                result.IsValid = false;
                result.AddError("Depth must be positive");
            }

            return result;
        }
    }
}
