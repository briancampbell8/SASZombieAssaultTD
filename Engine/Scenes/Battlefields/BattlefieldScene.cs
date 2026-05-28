/*
Program Name: SASZombieAssaultTD
File Path: Engine\Scenes\Battlefields\BattlefieldScene.cs
Purpose: Base class for all battlefield-specific scenes.
Features: Tilemap loading, spawn node placement, camera setup, wave progression, P100 integration.
*/

using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.Gameplay;
using SASZombieAssaultTD.Engine.VectorMath;
using SASZombieAssaultTD.Engine.Waves;
using IRenderContext = SASZombieAssaultTD.Engine.Rendering.IRenderContext;


namespace SASZombieAssaultTD.Engine.Scenes.Battlefields
{
    /// <summary>
    /// Base class for all battlefield-specific scenes.
    /// P120-08: Provides common functionality for battlefield scenes including tilemap loading,
    /// spawn node placement, camera setup, wave progression, and P100 system integration.
    /// </summary>
    public abstract class BattlefieldScene : BaseScene
    {
        protected BattlefieldType Type { get; private set; }
        protected string TilemapPath { get; private set; }
        protected List<Vector3> SpawnNodes { get; private set; }
        protected List<Vector3> ExitNodes { get; private set; }
        protected Vector3 CameraBoundsMin { get; private set; }
        protected Vector3 CameraBoundsMax { get; private set; }
        protected int CurrentWave { get; private set; }
        protected bool IsWaveActive { get; private set; }
        protected DifficultyScaling.DifficultyLevel CurrentDifficulty { get; private set; }

        /// <summary>
        /// Initializes a new battlefield scene.
        /// </summary>
        /// <param name="battlefieldType">The type of battlefield.</param>
        /// <param name="tilemapPath">The path to the tilemap asset.</param>
        protected BattlefieldScene(BattlefieldType battlefieldType, string tilemapPath)
        {
            Type = battlefieldType;
            TilemapPath = tilemapPath;
            SpawnNodes = new List<Vector3>();
            ExitNodes = new List<Vector3>();
            CameraBoundsMin = Vector3.Zero;
            CameraBoundsMax = Vector3.Zero;
            CurrentWave = 0;
            IsWaveActive = false;
            CurrentDifficulty = DifficultyScaling.DifficultyLevel.Normal;

            Engine.Diagnostics.DebugLogger.Log(DiagnosticLevel.Info,"INFO", $"BattlefieldScene: Created {battlefieldType.GetDisplayName()} scene");
        }

        /// <summary>
        /// Sets the camera bounds for this battlefield.
        /// </summary>
        /// <param name="min">Minimum camera bounds.</param>
        /// <param name="max">Maximum camera bounds.</param>
        protected void SetCameraBounds(Vector3 min, Vector3 max)
        {
            CameraBoundsMin = min;
            CameraBoundsMax = max;
        }

        /// <summary>
        /// P11-11-02: Called when the battlefield scene becomes active.
        /// </summary>
        public override void OnEnter()
        {
            // Call the static extension class directly
            Engine.Diagnostics.DebugLogger.Log(DiagnosticLevel.Info, "INFO",
                $"BattlefieldScene: Entering {BattlefieldTypeExtensions.GetDisplayName(BattlefieldType.SomeValue)}");

            // Load tilemap
            LoadTilemap();

            // Setup spawn nodes
            SetupSpawnNodes();

            // Setup exit nodes
            SetupExitNodes();

            // Setup camera bounds
            SetupCameraBounds();

            // Initialize wave progression
            CurrentWave = 0;
            IsWaveActive = false;


            Engine.Diagnostics.DebugLogger.Log(DiagnosticLevel.Info, "INFO",
                $"BattlefieldScene: {BattlefieldTypeExtensions.GetDisplayName(BattlefieldType.SomeValue)} initialization complete");
        }


        /// <summary>
        /// P11-11-02: Called when the battlefield scene becomes inactive.
        /// </summary>
        public override void OnExit()
        {
            Engine.Diagnostics.DebugLogger.Log(DiagnosticLevel.Info,"INFO", 
                $"BattlefieldScene: Exiting {BattlefieldTypeExtensions.GetDisplayName(BattlefieldType.SomeValue)}");

            // Stop any active wave
            if (IsWaveActive)
            {
                StopWave();
            }

            // Cleanup battlefield-specific resources
            CleanupBattlefield();
        }

        /// <summary>
        /// P11-11-02: Called during scene update loop.
        /// </summary>
        /// <param name="deltaTime">Time since last update.</param>
        public override void OnUpdate(float deltaTime)
        {
            // Update wave progression
            if (IsWaveActive)
            {
                UpdateWave(deltaTime);
            }

            // Update battlefield-specific logic
            UpdateBattlefield(deltaTime);
        }

        /// <summary>
        /// P11-11-02: Called during scene render loop.
        /// </summary>
        /// <param name="context">Render context.</param>
        public override void OnRender(IRenderContext context)
        {
            // Render tilemap layers
            if (context is IDrawingContext drawingContext)
                RenderTilemap(drawingContext);

            // Render battlefield-specific elements
            if (context is IDrawingContext drawingContext2)
                RenderBattlefield(drawingContext2);
        }

        /// <summary>
        /// Loads the tilemap for this battlefield.
        /// Override in derived classes for specific loading logic.
        /// </summary>
        protected virtual void LoadTilemap()
        {
            Engine.Diagnostics.DebugLogger.Log(DiagnosticLevel.Info,"INFO", $"BattlefieldScene: Loading tilemap from {TilemapPath}");
            // TODO: Implement tilemap loading through asset system
        }

        /// <summary>
        /// Sets up spawn nodes for this battlefield.
        /// Override in derived classes to define spawn positions.
        /// </summary>
        protected virtual void SetupSpawnNodes()
        {
            Engine.Diagnostics.DebugLogger.Log(DiagnosticLevel.Info,"INFO", $"BattlefieldScene: Setting up spawn nodes for {BattlefieldTypeExtensions.GetDisplayName(BattlefieldType.SomeValue)}");
            // Override in derived classes to add specific spawn nodes
        }

        /// <summary>
        /// Sets up exit nodes for this battlefield.
        /// Override in derived classes to define exit positions.
        /// </summary>
        protected virtual void SetupExitNodes()
        {
            Engine.Diagnostics.DebugLogger.Log(DiagnosticLevel.Info,"INFO", $"BattlefieldScene: Setting up exit nodes for {BattlefieldTypeExtensions.GetDisplayName(BattlefieldType.SomeValue)}");
            // Override in derived classes to add specific exit nodes
        }

        /// <summary>
        /// Sets up camera bounds for this battlefield.
        /// Override in derived classes to define camera limits.
        /// </summary>
        protected virtual void SetupCameraBounds()
        {
            Engine.Diagnostics.DebugLogger.Log(DiagnosticLevel.Info,"INFO", $"BattlefieldScene: Setting up camera bounds for {BattlefieldTypeExtensions.GetDisplayName(BattlefieldType.SomeValue    )}");
            // Override in derived classes to set specific camera bounds
        }

        /// <summary>
        /// Starts the next wave.
        /// P100 Integration: Uses DifficultyScaling system and ChampionVisuals.
        /// </summary>
        public virtual void StartNextWave()
        {
            if (IsWaveActive)
            {
                Engine.Diagnostics.DebugLogger.Log(DiagnosticLevel.Warning,"Warning", $"BattlefieldScene: Wave already active, cannot start new wave");
                return;
            }

            CurrentWave++;
            IsWaveActive = true;

            // P100 Integration: Get difficulty scaling for this wave
            var difficultyScaling = DifficultyScaling.GetForWave(CurrentWave, CurrentDifficulty);

            Engine.Diagnostics.DebugLogger.Log(DiagnosticLevel.Info,"INFO", $"BattlefieldScene: Starting wave {CurrentWave} on {BattlefieldTypeExtensions.GetDisplayName(BattlefieldType.SomeValue)} with difficulty {CurrentDifficulty}");
            Engine.Diagnostics.DebugLogger.Log(DiagnosticLevel.Info,"INFO", $"BattlefieldScene: Wave multipliers - Health: {difficultyScaling.HealthMultiplier:F2}, Speed: {difficultyScaling.SpeedMultiplier:F2}, Count: {difficultyScaling.CountMultiplier:F2}, ChampionSpawnRate: {difficultyScaling.ChampionSpawnRate:P2}");

            // P100 Integration: Apply champion spawn rate
            ApplyChampionSpawnRate(difficultyScaling.ChampionSpawnRate);

            // TODO: Spawn enemies using WaveSpawnGroup with difficulty scaling
            // TODO: Apply DifficultyScaling stat multipliers to spawned enemies
        }

        /// <summary>
        /// Sets the difficulty level for this battlefield.
        /// P100 Integration: Allows dynamic difficulty adjustment.
        /// </summary>
        /// <param name="difficulty">The difficulty level.</param>
        public void SetDifficulty(DifficultyScaling.DifficultyLevel difficulty)
        {
            CurrentDifficulty = difficulty;
            Engine.Diagnostics.DebugLogger.Log(DiagnosticLevel.Info,"INFO", $"BattlefieldScene: Difficulty set to {difficulty} for {BattlefieldTypeExtensions.GetDisplayName(BattlefieldType.SomeValue)}");
        }

        /// <summary>
        /// Applies champion spawn rate from difficulty scaling.
        /// P100 Integration: Uses champion spawn rate to determine champion enemy spawns.
        /// </summary>
        /// <param name="championSpawnRate">The champion spawn rate (0.0 to 1.0).</param>
        protected virtual void ApplyChampionSpawnRate(float championSpawnRate)
        {
            // P100 Integration: Randomly determine if enemies should be champions based on spawn rate
            // This would be called during enemy spawning in the actual implementation
            var random = new Random();
            var shouldSpawnChampion = random.NextDouble() < championSpawnRate;

            if (shouldSpawnChampion)
            {
                // Generate random champion level based on difficulty and wave
                var championLevel = GenerateChampionLevel();
                Engine.Diagnostics.DebugLogger.Log(DiagnosticLevel.Info,"INFO", $"BattlefieldScene: Champion enemy will spawn with level {championLevel}");

                // P100 Integration: Apply champion visuals when champion enemy is created
                // var visuals = ChampionVisuals.GenerateForLevel(championLevel);
                // visuals.ApplyTo(enemy);
            }
        }

        /// <summary>
        /// Generates a champion level based on current wave and difficulty.
        /// P100 Integration: Champion level scales with wave progression.
        /// </summary>
        /// <returns>Champion level (1-10).</returns>
        protected virtual int GenerateChampionLevel()
        {
            // Base level on wave number, capped at 10
            var baseLevel = System.Math.Min(CurrentWave, 10);

            // Adjust based on difficulty
            var difficultyBonus = CurrentDifficulty switch
            {
                DifficultyScaling.DifficultyLevel.Easy => 0,
                DifficultyScaling.DifficultyLevel.Normal => 0,
                DifficultyScaling.DifficultyLevel.Hard => 1,
                DifficultyScaling.DifficultyLevel.Elite => 2,
                _ => 0
            };

            var championLevel = System.Math.Min(baseLevel + difficultyBonus, 10);
            return System.Math.Max(championLevel, 1); // Ensure minimum level of 1
        }

        /// <summary>
        /// Stops the current wave.
        /// </summary>
        public virtual void StopWave()
        {
            if (!IsWaveActive)
            {
                return;
            }

            IsWaveActive = false;
            Engine.Diagnostics.DebugLogger.Log(DiagnosticLevel.Info,"INFO", 
                $"BattlefieldScene: Stopped wave {CurrentWave} on {BattlefieldTypeExtensions.GetDisplayName(BattlefieldType.SomeValue)}");
        }

        /// <summary>
        /// Updates the current wave.
        /// Override in derived classes for specific wave logic.
        /// </summary>
        /// <param name="deltaTime">Time since last update.</param>
        protected virtual void UpdateWave(float deltaTime)
        {
            // Override in derived classes for specific wave update logic
        }

        /// <summary>
        /// Updates battlefield-specific logic.
        /// Override in derived classes for specific update logic.
        /// </summary>
        /// <param name="deltaTime">Time since last update.</param>
        protected virtual void UpdateBattlefield(float deltaTime)
        {
            // Override in derived classes for specific battlefield update logic
        }

        /// <summary>
        /// Renders the tilemap layers.
        /// Override in derived classes for specific rendering logic.
        /// </summary>
        /// <param name="context">Render context.</param>
        protected virtual void RenderTilemap(IDrawingContext context)
        {
            // Override in derived classes for specific tilemap rendering
        }

        /// <summary>
        /// Renders battlefield-specific elements.
        /// Override in derived classes for specific rendering logic.
        /// </summary>
        /// <param name="context">Render context.</param>
        protected virtual void RenderBattlefield(IDrawingContext context)
        {
            // Override in derived classes for specific battlefield rendering
        }

        /// <summary>
        /// Cleans up battlefield-specific resources.
        /// Override in derived classes for specific cleanup logic.
        /// </summary>
        protected virtual void CleanupBattlefield()
        {
            SpawnNodes.Clear();
            ExitNodes.Clear();
            Engine.Diagnostics.DebugLogger.Log(DiagnosticLevel.Info,"INFO",
                $"BattlefieldScene: Cleaned up {BattlefieldTypeExtensions.GetDisplayName(BattlefieldType.SomeValue)}");
        }

        /// <summary>
        /// Gets the battlefield type.
        /// </summary>
        /// <returns>The battlefield type.</returns>
        // Replace lines 343-346 with this single line:
        public abstract BattlefieldType GetBattlefieldType();


        /// <summary>
        /// Gets the current wave number.
        /// </summary>
        /// <returns>The current wave number.</returns>
        public int GetCurrentWave()
        {
            return CurrentWave;
        }

        /// <summary>
        /// Gets whether a wave is currently active.
        /// </summary>
        /// <returns>True if a wave is active.</returns>
        public bool GetIsWaveActive()
        {
            return IsWaveActive;
        }

        /// <summary>
        /// Gets all spawn nodes.
        /// </summary>
        /// <returns>List of spawn node positions.</returns>
        public IReadOnlyList<Vector3> GetSpawnNodes()
        {
            return SpawnNodes.AsReadOnly();
        }

        /// <summary>
        /// Gets all exit nodes.
        /// </summary>
        /// <returns>List of exit node positions.</returns>
        public IReadOnlyList<Vector3> GetExitNodes()
        {
            return ExitNodes.AsReadOnly();
        }

        /// <summary>
        /// Gets the camera bounds.
        /// </summary>
        /// <returns>Tuple of min and max bounds.</returns>
        public (Vector3 min, Vector3 max) GetCameraBounds()
        {
            return (CameraBoundsMin, CameraBoundsMax);
        }

        private class DiagnosticLevel
        {
            internal static string Info;
            internal static string Warning;
        }
    }
}
