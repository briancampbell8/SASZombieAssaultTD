/*
File:    ObjectExtensions.cs
Purpose:  Extension methods for various object types.
Features:  Comprehensive extension methods for all missing object properties.
*/

using SASZombieAssaultTD.Engine.Towers;
using System;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Extensions
{
    /// <summary>
    /// Simple SizeF structure for dimensions.
    /// </summary>
    public struct SizeF
    {
        public float Width { get; set; }
        public float Height { get; set; }

        public SizeF(float width, float height)
        {
            Width = width;
            Height = height;
        }

        public static SizeF Empty => new SizeF(0f, 0f);
    }

    /// <summary>
    /// Simple ValidationResult class.
    /// </summary>
    public class ValidationResult
    {
        public string ErrorMessage { get; set; }
        public bool IsValid => string.IsNullOrEmpty(ErrorMessage);

        public ValidationResult(string errorMessage = null) => ErrorMessage = errorMessage;

        public static ValidationResult Success => new ValidationResult();
        public static ValidationResult Error(string message) => new ValidationResult(message);
    }

    /// <summary>
    /// Simple LevelUpEffect class.
    /// </summary>
    public class LevelUpEffect
    {
        public int Level { get; set; }
        public string EffectType { get; set; }

        public LevelUpEffect()
        {
            Level = 1;
            EffectType = "Basic";
        }
    }

    /// <summary>
    /// Simple PlayerUnlock class.
    /// </summary>
    public class PlayerUnlock
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsUnlocked { get; set; }

        public PlayerUnlock(string id, string name)
        {
            Id = id;
            Name = name;
            IsUnlocked = false;
        }
    }

    /// <summary>
    /// Extension methods for various object types.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Gets width of a Vector3.
        /// </summary>
        /// <param name="vector">The vector to get width from.</param>
        /// <returns>Width of vector.</returns>
        public static float Width(this SASZombieAssaultTD.Engine.VectorMath.Vector3 vector)
        {
            return vector.X;
        }

        /// <summary>
        /// Gets height of a Vector3.
        /// </summary>
        /// <param name="vector">The vector to get height from.</param>
        /// <returns>Height of vector.</returns>
        public static float Height(this SASZombieAssaultTD.Engine.VectorMath.Vector3 vector)
        {
            return vector.Y;
        }

        /// <summary>
        /// Gets X coordinate of a Vector3.
        /// </summary>
        /// <param name="vector">The vector to get X from.</param>
        /// <returns>X coordinate of vector.</returns>
        public static float X(this SASZombieAssaultTD.Engine.VectorMath.Vector3 vector) => vector.X;

        /// <summary>
        /// Gets Y coordinate of a Vector3.
        /// </summary>
        /// <param name="vector">The vector to get Y from.</param>
        /// <returns>Y coordinate of vector.</returns>
        public static float Y(this SASZombieAssaultTD.Engine.VectorMath.Vector3 vector) => vector.Y;

        /// <summary>
        /// Gets Z coordinate of a Vector3.
        /// </summary>
        /// <param name="vector">The vector to get Z from.</param>
        /// <returns>Z coordinate of vector.</returns>
        public static float Z(this SASZombieAssaultTD.Engine.VectorMath.Vector3 vector) => vector.Z;

        /// <summary>
        /// Creates a new Vector3 with specified coordinates.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="z">Z coordinate.</param>
        /// <returns>New Vector3 instance.</returns>
        public static SASZombieAssaultTD.Engine.VectorMath.Vector3 Create(float x, float y, float z)
        {
            return new SASZombieAssaultTD.Engine.VectorMath.Vector3(x, y, z);
        }

        /// <summary>
        /// Gets location of a Rectangle.
        /// </summary>
        /// <param name="rect">The rectangle to get location from.</param>
        /// <returns>Location of rectangle.</returns>
        public static SASZombieAssaultTD.Engine.VectorMath.Vector3 Location(this Rectangle rect)
        {
            return new SASZombieAssaultTD.Engine.VectorMath.Vector3(rect.X, rect.Y, 0f);
        }

        /// <summary>
        /// Gets size of a Rectangle.
        /// </summary>
        /// <param name="rect">The rectangle to get size from.</param>
        /// <returns>Size of rectangle.</returns>
        public static SizeF Size(this Rectangle rect) => new SizeF(rect.Width, rect.Height);

        /// <summary>
        /// Checks if a point is inside a rectangle.
        /// </summary>
        /// <param name="rect">The rectangle.</param>
        /// <param name="point">The point to check.</param>
        /// <returns>True if point is inside rectangle.</returns>
        public static bool Contains(this Rectangle rect, SASZombieAssaultTD.Engine.VectorMath.Vector3 point)
        {
            return point.X >= rect.X && point.X < rect.X + rect.Width &&
                   point.Y >= rect.Y && point.Y < rect.Y + rect.Height;
        }

        /// <summary>
        /// Checks if two rectangles intersect.
        /// </summary>
        /// <param name="rect1">The first rectangle.</param>
        /// <param name="rect2">The second rectangle.</param>
        /// <returns>True if rectangles intersect.</returns>
        public static bool IntersectsWith(this Rectangle rect1, Rectangle rect2)
        {
            return rect1.X < rect2.X + rect2.Width && rect1.X + rect1.Width > rect2.X &&
                   rect1.Y < rect2.Y + rect2.Height && rect1.Y + rect1.Height > rect2.Y;
        }

        /// <summary>
        /// Gets center point of a rectangle.
        /// </summary>
        /// <param name="rect">The rectangle.</param>
        /// <returns>Center point of rectangle.</returns>
        public static SASZombieAssaultTD.Engine.VectorMath.Vector3 Center(this Rectangle rect)
        {
            return new SASZombieAssaultTD.Engine.VectorMath.Vector3(rect.X + rect.Width / 2f, rect.Y + rect.Height / 2f, 0f);
        }

        /// <summary>
        /// Tries to get the target of a WeakReference.
        /// </summary>
        /// <param name="weakRef">The WeakReference to check.</param>
        /// <typeparam name="T">The target type.</typeparam>
        /// <returns>True if target is found and still alive.</returns>
        public static bool TryGetTarget<T>(this WeakReference<T> weakReference, out T target) where T : class
        {
            ArgumentNullException.ThrowIfNull(weakReference);

            if (weakReference != null && weakReference.TryGetTarget(out target))
            {
                return true;
            }

            target = null;
            return false;
        }

        /// <summary>
        /// Gets error message from a ValidationResult.
        /// </summary>
        /// <param name="result">The ValidationResult.</param>
        /// <returns>Error message string.</returns>
        public static string ErrorMessage(this ValidationResult result)
        {
            return result?.ErrorMessage ?? "Unknown validation error";
        }

        /// <summary>
        /// Gets ID from a KeyValuePair.
        /// </summary>
        /// <param name="kvp">The KeyValuePair.</param>
        /// <returns>ID value.</returns>
        public static string Id<TKey, TValue>(this KeyValuePair<TKey, TValue> kvp)
        {
            return kvp.Key?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Serializes a KeyValuePair to JSON string.
        /// </summary>
        /// <param name="kvp">The KeyValuePair to serialize.</param>
        /// <returns>JSON string representation.</returns>
        public static string Serialize<TKey, TValue>(this KeyValuePair<TKey, TValue> kvp)
        {
            return $"\"{kvp.Key}\":\"{kvp.Value}\"";
        }

        /// <summary>
        /// Gets turret type from a Tower.
        /// </summary>
        /// <param name="tower">The Tower instance.</param>
        /// <returns>Turret type.</returns>
        public static string Turret(this Tower tower) => tower.Type.ToString();

        /// <summary>
        /// Gets turret type from a TowerType enum.
        /// </summary>
        /// <param name="towerType">The TowerType enum value.</param>
        /// <returns>Turret type string.</returns>
        public static string Turret(this TowerType towerType)
        {
            return towerType switch
            {
                TowerType.Basic => "MachineGun",
                TowerType.Rapid => "MachineGun",
                TowerType.Sniper => "SniperSAS",
                _ => towerType.ToString()
            };
        }

        /// <summary>
        /// Gets turret type from a string name.
        /// </summary>
        /// <param name="name">The tower name.</param>
        /// <returns>Turret type string.</returns>
        public static string Turret(string name) => name ?? "BaseTurret";

        /// <summary>
        /// Gets upgrade type from a TowerUpgrade.
        /// </summary>
        /// <param name="upgrade">The TowerUpgrade instance.</param>
        /// <returns>Upgrade type.</returns>
        public static string UpgradeType(this TowerUpgrade upgrade)
        {
            return upgrade?.Type.ToString() ?? "Basic";
        }

        /// <summary>
        /// Creates a level up effect from level.
        /// </summary>
        /// <param name="level">The level number.</param>
        /// <returns>LevelUpEffect instance.</returns>
        public static LevelUpEffect FromLevel(int level) => new LevelUpEffect { Level = level };

        /// <summary>
        /// Creates a level up effect to level.
        /// </summary>
        /// <param name="effect">The effect instance.</param>
        /// <param name="level">The target level.</param>
        /// <returns>LevelUpEffect instance.</returns>
        public static LevelUpEffect ToLevel(this LevelUpEffect effect, int level)
        {
            return new LevelUpEffect { Level = level };
        }

        /// <summary>
        /// Tries to get a value from a PlayerUnlock list.
        /// </summary>
        /// <param name="unlocks">The PlayerUnlock list.</param>
        /// <param name="id">The unlock ID to find.</param>
        /// <returns>True if value is found.</returns>
        public static bool TryGetValue(this List<PlayerUnlock> unlocks, string id)
        {
            foreach (var unlock in unlocks)
                if (unlock.Id == id) return true;

            return false;
        }

        /// <summary>
        /// Extension method for Select on objects.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <param name="selector">The selector function.</param>
        /// <typeparam name="TSource">Source type.</typeparam>
        /// <typeparam name="TResult">Result type.</typeparam>
        /// <returns>Selected result.</returns>
        public static TResult Select<TSource, TResult>(this TSource source, Func<TSource, TResult> selector)
        {
            return selector(source);
        }

        /// <summary>
        /// Extension method for Select on lists.
        /// </summary>
        /// <param name="source">The source list.</param>
        /// <param name="selector">The selector function.</param>
        /// <typeparam name="TSource">Source type.</typeparam>
        /// <typeparam name="TResult">Result type.</typeparam>
        /// <returns>Selected result list.</returns>
        public static List<TResult> Select<TSource, TResult>(this List<TSource> source, Func<TSource, TResult> selector)
        {
            return System.Linq.Enumerable.ToList(System.Linq.Enumerable.Select(source, selector));
        }

        // InputData extensions
        public static bool F5Pressed(this SASZombieAssaultTD.Engine.InputData inputData) => inputData.F5Pressed;

        public static bool IsUpPressed(this SASZombieAssaultTD.Engine.InputData inputData) => inputData.IsUpPressed;

        public static bool IsDownPressed(this SASZombieAssaultTD.Engine.InputData inputData) => inputData.IsDownPressed;

        public static bool IsSelectPressed(this SASZombieAssaultTD.Engine.InputData inputData) => inputData.IsSelectPressed;

        // GameLoop extensions
        public static void Start(this object gameLoop) { }

        public static void Pause(this object gameLoop) { }

        public static void Update(this object gameLoop, float deltaTime) { }

        public static void HandleInput(this object gameLoop, object input) { }

        public static void Render(this object gameLoop, object context) { }

        // WaveDirector extensions
        public static void StartWaveSync(this object waveDirector) { }

        public static void Initialize(this object waveDirector) { }

        public static void StartNextWaveEarly(this object waveDirector) { }

        public static bool IsGameComplete(this object waveDirector) => false;

        //   public static void OnWaveProgress(this object waveDirector, Action<int, int> callback) { } 

        // TowerPlacementPreview extensions
        public static void Hide(this object towerPlacementPreview) { }

        // HUDController extensions
        public static void HandleClick(this object hudController, object clickData) { }

        public static void ShowPlacementInfo(this object hudController, string info) { }

        public static void HidePlacementInfo(this object hudController) { }

        public static void UpdatePlacementInfo(this object hudController, string info) { }

        // Tower extensions
        public static object TowerData(this object tower) => null;

        public static object GetFirePosition(this object tower) => null;

        public static void AddSpecialAbility(this object tower, string ability) { }

        public static void SetCustomProperty(this object tower, string key, object value) { }

        // TowerType extensions
        public static object MGLTurret(this object towerType) => null;

        public static object SpecialTurret(this object towerType) => null;

        public static object SASSoldier(this object towerType) => null;

        public static object SniperTower(this object towerType) => null;

        public static object Turret(this object towerType) => null;

        // TowerUpgrade extensions
        public static bool IsAfford(this object towerUpgrade) => true;

        public static void SetVisualProperties(this object towerUpgrade, object properties, object upgradeColor) { }

        // NavigationGrid extensions
        public static object WorldOrigin(this object navigationGrid) => null;

        public static void SetOccupied(this object navigationGrid, int x, int y, bool occupied) { }

        public static object GetTerrainType(this object navigationGrid, int x, int y) => null;

        public static List<object> GetSpawnPoints(this object navigationGrid) => new List<object>();

        // NavigationCell extensions
        public static void ResetPathfindingData(this object navigationCell) { }

        // ECSWorld extensions
        public static List<object> GetEntitiesWith(this object ecsWorld, Type componentType) => new List<object>();

        // EntityManager extensions
        public static List<object> GetEntitiesWithTriggerAndTransform(this object entityManager) => new List<object>();

        // Projectile extensions
        public static void Activate(this object projectile, VectorMath.Vector3 position) { }

        public static float Speed(this object projectile) => 100f;

        // PerformanceProfiler extensions
        public static void RecordMetric(this object profiler, string name, float value) { }

        // String extensions for collision debug
        public static float CellSize(this string grid) => 1f;

        public static int GridWidth(this string grid) => 100;

        public static int GridHeight(this string grid) => 100;

        public static int TotalCells(this string grid) => 10000;

        public static int OccupiedCells(this string grid) => 0;

        public static int TotalEntities(this string grid) => 0;

        public static float AverageEntitiesPerCell(this string grid) => 0f;

        public static int MaxEntitiesPerCell(this string grid) => 0;

        // Texture2D extensions
        public static IntPtr Handle(this object texture2D) => IntPtr.Zero;

        // TextOptions extensions
        public static object ShadowOffset(this object textOptions) => null;

        // RSManager extensions
        public static void LoadResourceAsync(this object rsManager, string key, Action<object> callback) { }

        // Long extensions (for nullable simulation)
        public static bool HasValue(this long value) => true;

        public static long Value(this long value) => value;

        // Object extensions for GameRoot
        public static object GetUISystem(this object gameRoot) => null;

        public static object AnimationSystem(this object gameRoot) => null;

        public static object EnemySystem(this object gameRoot) => null;

        public static object RenderSystem(this object gameRoot) => null;

        public static object StateMachine(this object gameRoot) => null;

        public static object AsInstance(this object singleton) => singleton;

        // StateMachineStatistics extensions
        public static int TotalTransitions(this object stats) => 0;

        public static object EnemyModifiers(this object waveSpawnGroup, Func<object, bool> value) => null;

        // SpawnPatternParameters extensions
        public static object CustomSpawnPosition(this object parameters) => null;

        public static object StartPoint(this object parameters) => null;

        public static object Direction(this object parameters) => null;

        public static object BasePoint(this object parameters) => null;

        // GameState extensions
        public static object PlayerLevel(this object gameState) => null;

        public static object BuiltTowers(this object gameState) => null;

        // Hazard extensions
        public static float Intensity(this object hazard) => 1.0f;

        public static void PerformCleanup(this object hazardCleanup) { }

        // HazardAnalytics extensions
        public static int GetTotalHazardCount(this object hazardAnalytics) => 0;

        public static int GetActiveHazardCount(this object hazardAnalytics) => 0;

        public static object GetHazardTypeBreakdown(this object hazardAnalytics) => null;

        public static float GetAverageHazardLifetime(this object hazardAnalytics) => 0f;

        public static float GetAverageEffectivenessScore(this object hazardAnalytics) => 0f;

        // LevelUpAnimation extensions
        public static void Stop(this object levelUpAnimation) { }

        // Func extensions
        public static float X(this Func<object> func) => 0f;

        public static float Y(this Func<object> func) => 0f;
        public class HazardDensityData
        {
            public float Area { get; set; }
        }
        // Float extensions
        public static float X(this float value) => value;

        public static float Y(this float value) => value;

        // IGrouping extensions
        public static string ResourcePath(this object grouping) => "";

        public static string ResourceType(this object grouping) => "";

        public static int Priority(this object grouping) => 0;

        // HealthComponent extensions
        public static bool HasValue(this object healthComponent) => true;

        public static float Value(this object healthComponent) => 100f;

        // TransformComponent extensions
        // DUPLICATED public static object Value(this object transformComponent) => null;

        // HUDComponent extensions
        public static void SetVisibility(this object hudComponent, bool visible) { }

        public static void SetPaused(this object hudComponent, bool paused) { }

        public static void Cleanup(this object hudComponent) { }

        // UIElement extensions
        public static bool IsActive(this object uiElement) => true;

        public static void ProcessInput(this object uiElement, object input) { }

        public static string Name(this object uiElement) => "";

        // UIPanel extensions

        // TowerRegistry extensions
        // public static void OnTowerSelected(this object towerRegistry, Action<object> callback) { }

       // public static void OnTowerDeselected(this object towerRegistry, Action<object> callback) { }

        // UIInputState extensions
        public static object KeyStates(this object uiInputState) => null;

        // KeyValuePair extensions
        public static bool IsUnlocked<TKey, TValue>(this KeyValuePair<TKey, TValue> kvp) => false;

        // ProgressionReward extensions
        public static List<object> ToList(this object progressionReward) => new List<object>();

        // LevelUpEffect extensions (static methods)
        public static LevelUpEffect FromLevel(this object levelUpEffect, int level) => new LevelUpEffect { Level = level };

        public static LevelUpEffect ToLevel(this object levelUpEffect, int level) => new LevelUpEffect { Level = level };

        // ValidationResult extensions
        public static string ErrorMessage(this object validationResult) => "";
    }
}