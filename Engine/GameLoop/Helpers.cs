/*
File:    Helpers.cs
Path:    Engine/GameLoop/Helpers.cs
Purpose: P11-09-01 - Contains all helper classes for GameLoop.
         Extracted nested classes and supporting structures.

Role:     Game loop helper specialist.
         - All nested helper classes
         - Supporting data structures
         - Utility methods
         - Extension methods
         - Common interfaces

Notes:    Contains all helper classes and supporting structures that
         were previously nested in the main GameLoop class.
         Provides clean separation of helper concerns.
*/

using System;
using SASZombieAssaultTD.Engine.Timing;
using SASZombieAssaultTD.Engine.UI.Input;

namespace SASZombieAssaultTD.Engine.Systems
{
    /// <summary>
    /// Partial class containing helper logic for GameLoop.
    /// </summary>
    public partial class GameLoop
    {
        /// <summary>
        /// Creates a new frame diagnostics instance.
        /// </summary>
        /// <returns>A new FrameDiagnostics instance.</returns>
        private FrameDiagnostics CreateFrameDiagnostics()
        {
            return new FrameDiagnostics
            {
                FrameTimer = new System.Diagnostics.Stopwatch(),
                FrameCount = 0,
                AverageFrameTime = 0f,
                MinFrameTime = float.MaxValue,
                MaxFrameTime = float.MinValue,
                FramesPerSecond = 0f,
                ErrorCount = 0
            };
        }

        /// <summary>
        /// Validates game loop configuration.
        /// </summary>
        /// <returns>True if configuration is valid.</returns>
        private bool ValidateConfiguration()
        {
            return _gameRoot != null &&
                   _timing != null &&
                   _diagnostics != null &&
                   TargetFrameTime > 0;
        }

        /// <summary>
        /// Gets configuration summary.
        /// </summary>
        /// <returns>Configuration summary.</returns>
        public ConfigurationSummary GetConfigurationSummary()
        {
            return new ConfigurationSummary
            {
                HasGameRoot = _gameRoot != null,
                HasTimingModule = _timing != null,
                HasDiagnostics = _diagnostics != null,
                HasInputRouter = _input != null,
                TargetFrameTime = TargetFrameTime,
                TargetFPS = 1f / TargetFrameTime,
                IsValid = ValidateConfiguration()
            };
        }
    }

    /// <summary>
    /// Configuration summary for the game loop.
    /// </summary>
    public class ConfigurationSummary
    {
        public bool HasGameRoot { get; set; }
        public bool HasTimingModule { get; set; }
        public bool HasDiagnostics { get; set; }
        public bool HasInputRouter { get; set; }
        public float TargetFrameTime { get; set; }
        public float TargetFPS { get; set; }
        public bool IsValid { get; set; }

        public override string ToString()
        {
            return $"Config: GameRoot={HasGameRoot}, Timing={HasTimingModule}, Diagnostics={HasDiagnostics}, Input={HasInputRouter}, TargetFPS={TargetFPS:F1}, Valid={IsValid}";
        }
    }

    /// <summary>
    /// Game loop factory for creating configured instances.
    /// </summary>
    public static class GameLoopFactory
    {
        /// <summary>
        /// Creates a new game loop with default configuration.
        /// </summary>
        /// <param name="gameRoot">The game root instance.</param>
        /// <param name="timing">The timing module.</param>
        /// <param name="diagnostics">The frame diagnostics.</param>
        /// <param name="input">The input router.</param>
        /// <returns>A configured game loop instance.</returns>
        public static GameLoop CreateDefault(
            GameRoot gameRoot,
            TimingModule timing,
            FrameDiagnostics diagnostics,
            UIInputRouter input)
        {
            return new GameLoop(gameRoot, timing, diagnostics, input);
        }

        /// <summary>
        /// Creates a new game loop with custom configuration.
        /// </summary>
        /// <param name="gameRoot">The game root instance.</param>
        /// <param name="timing">The timing module.</param>
        /// <param name="diagnostics">The frame diagnostics.</param>
        /// <param name="input">The input router.</param>
        /// <param name="targetFPS">The target frames per second.</param>
        /// <returns>A configured game loop instance.</returns>
        public static GameLoop CreateCustom(
            GameRoot gameRoot,
            TimingModule timing,
            FrameDiagnostics diagnostics,
            UIInputRouter input,
            float targetFPS)
        {
            var gameLoop = new GameLoop(gameRoot, timing, diagnostics, input);

            // Apply custom configuration
            // This would set custom target FPS and other parameters

            return gameLoop;
        }
    }

    /// <summary>
    /// Game loop builder for fluent configuration.
    /// </summary>
    public class GameLoopBuilder
    {
        private GameRoot _gameRoot;
        private TimingModule _timing;
        private FrameDiagnostics _diagnostics;
        private UIInputRouter _input;
        private float _targetFPS = 60f;

        /// <summary>
        /// Sets the game root.
        /// </summary>
        /// <param name="gameRoot">The game root instance.</param>
        /// <returns>The builder for chaining.</returns>
        public GameLoopBuilder WithGameRoot(GameRoot gameRoot)
        {
            _gameRoot = gameRoot;
            return this;
        }

        /// <summary>
        /// Sets the timing module.
        /// </summary>
        /// <param name="timing">The timing module.</param>
        /// <returns>The builder for chaining.</returns>
        public GameLoopBuilder WithTiming(TimingModule timing)
        {
            _timing = timing;
            return this;
        }

        /// <summary>
        /// Sets the frame diagnostics.
        /// </summary>
        /// <param name="diagnostics">The frame diagnostics.</param>
        /// <returns>The builder for chaining.</returns>
        public GameLoopBuilder WithDiagnostics(FrameDiagnostics diagnostics)
        {
            _diagnostics = diagnostics;
            return this;
        }

        /// <summary>
        /// Sets the input router.
        /// </summary>
        /// <param name="input">The input router.</param>
        /// <returns>The builder for chaining.</returns>
        public GameLoopBuilder WithInput(UIInputRouter input)
        {
            _input = input;
            return this;
        }

        /// <summary>
        /// Sets the target FPS.
        /// </summary>
        /// <param name="targetFPS">The target frames per second.</param>
        /// <returns>The builder for chaining.</returns>
        public GameLoopBuilder WithTargetFPS(float targetFPS)
        {
            _targetFPS = targetFPS;
            return this;
        }

        /// <summary>
        /// Builds the configured game loop.
        /// </summary>
        /// <returns>The configured game loop instance.</returns>
        public GameLoop Build()
        {
            if (_gameRoot == null)
                throw new InvalidOperationException("GameRoot is required");
            if (_timing == null)
                throw new InvalidOperationException("TimingModule is required");
            if (_diagnostics == null)
                throw new InvalidOperationException("FrameDiagnostics is required");

            return GameLoopFactory.CreateCustom(_gameRoot, _timing, _diagnostics, _input, _targetFPS);
        }
    }

    /// <summary>
    /// Extension methods for game loop operations.
    /// </summary>
    public static class GameLoopExtensions
    {
        /// <summary>
        /// Starts the game loop if it's initialized.
        /// </summary>
        /// <param name="gameLoop">The game loop.</param>
        /// <returns>True if the game loop was started.</returns>
        public static bool TryStart(this GameLoop gameLoop)
        {
            if (gameLoop == null || !gameLoop.IsInitialized)
                return false;

            try
            {
                gameLoop.Run();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Stops the game loop if it's running.
        /// </summary>
        /// <param name="gameLoop">The game loop.</param>
        /// <returns>True if the game loop was stopped.</returns>
        public static bool TryStop(this GameLoop gameLoop)
        {
            if (gameLoop == null || !gameLoop.IsRunning)
                return false;

            try
            {
                gameLoop.Stop();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a formatted performance summary.
        /// </summary>
        /// <param name="gameLoop">The game loop.</param>
        /// <returns>Formatted performance summary.</returns>
        public static string GetPerformanceSummary(this GameLoop gameLoop)
        {
            if (gameLoop == null)
                return "GameLoop is null";

            var metrics = gameLoop.GetPerformanceMetrics();
            return $"FPS: {metrics.FramesPerSecond:F1}, Frames: {metrics.FrameCount}, Errors: {metrics.ErrorCount}, Memory: {metrics.MemoryUsage / 1024 / 1024}MB";
        }
    }
}
