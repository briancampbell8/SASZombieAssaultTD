/*
File:    LoadingScene.cs
Purpose: P11-11-07 - Simple loading scene for transitions between major scenes.
*/
using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.Core;
using System;
using SASZombieAssaultTD.Engine.Extensions;

namespace SASZombieAssaultTD.Engine.Scenes
{
    /// <summary>
    /// P11-11-07: Simple loading scene implementation for smooth transitions.
    /// </summary>
    public class LoadingScene : BaseScene
    {
        private BaseScene? _targetScene;
        private float _loadingTime;
        private float _minLoadingDuration;
        private bool _readyToTransition;

        private float _loadingTimer; // Tracks the elapsed loading time
        private float _loadingDuration; // Specifies the total loading duration

        /// <summary>
        /// Initializes a new instance of the LoadingScene class.
        /// </summary>
        /// <param name="targetScene">The scene to load after loading is complete.</param>
        /// <param name="minDuration">Minimum loading duration in seconds.</param>
        public LoadingScene(BaseScene? targetScene = null, float minDuration = 1.0f)
        {
            _targetScene = targetScene;
            _minLoadingDuration = minDuration;
            _loadingTime = 0f;
            _readyToTransition = false;

            _loadingTimer = 0f; // Initialize the loading timer
            _loadingDuration = minDuration; // Set the loading duration
        }

        /// <summary>
        /// Sets the target scene to transition to after loading.
        /// </summary>
        /// <param name="targetScene">The target scene.</param>
        public void SetTargetScene(BaseScene targetScene)
        {
            _targetScene = targetScene;
            _readyToTransition = false;
            _loadingTime = 0f;
        }

        /// <summary>
        /// P11-11-02: Called when the loading scene becomes active.
        /// </summary>
        public override void OnEnter()
        {
            Engine.Diagnostics.DebugLogger.LogDebug("INFO", "LoadingScene.OnEnter: Loading scene started");
            _loadingTime = 0f;
            _readyToTransition = false;

            // Simulate loading work - in a real implementation, this would
            // load assets, initialize systems, etc.
            SimulateLoadingWork();
        }

        /// <summary>
        /// P11-11-02: Called when the loading scene becomes inactive.
        /// </summary>
        public override void OnExit()
        {
            Engine.Diagnostics.DebugLogger.LogDebug("INFO", "LoadingScene.OnExit: Loading scene completed");
        }

        /// <summary>
        /// P11-11-02: Called every frame to update loading logic.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since last frame.</param>
        public override void OnUpdate(float deltaTime)
        {
            _loadingTime += deltaTime;

            // Check if minimum loading duration has passed and we're ready to transition
            if (_loadingTime >= _minLoadingDuration && _readyToTransition && _targetScene != null)
            {
                Engine.Diagnostics.DebugLogger.LogDebug("INFO", "LoadingScene.OnUpdate: Transitioning to target scene");
                SceneManager?.QueueScene(_targetScene.GetType().Name);
            }
        }

        /// <summary>
        /// P11-11-02: Called every frame to render the loading screen.
        /// </summary>
        /// <param name="context">The render context.</param>
        public override void OnRender(IRenderContext context)
        {
            if (context == null)
                return;

            // Simple loading screen rendering
            // In a real implementation, this would render a loading animation,
            // progress bar, loading tips, etc.

            // For now, we'll just clear to a dark color
            context.Clear(0.1f, 0.1f, 0.2f, 1.0f);

            // Render loading animation, progress bar, or loading text
            RenderLoadingUI(context);
        }

        /// <summary>
        /// Render loading UI elements.
        /// </summary>
        /// <param name="context">Render context.</param>
        private void RenderLoadingUI(IRenderContext context)
        {
            // Calculate loading progress based on time
            var progress = (float)(_loadingTimer / _loadingDuration);
            progress = System.Math.Clamp(progress, 0f, 1f);

            // Render loading text
            var loadingText = "Loading...";
            var textSize = context.MeasureText(loadingText, 24);
            var textX = (context.ScreenWidth - textSize.Width()) / 2;
            var textY = context.ScreenHeight / 2 - 50;

            context.DrawText(loadingText, textX, textY, 24, Color.White);

            // Render progress bar
            var barWidth = 300;
            var barHeight = 20;
            var barX = (context.ScreenWidth - barWidth) / 2;
            var barY = context.ScreenHeight / 2;

            // Progress bar background
            context.DrawRectangle(barX, barY, barWidth, barHeight, Color.Gray);

            // Progress bar fill
            var fillWidth = (int)(barWidth * progress);
            context.DrawRectangle(barX, barY, fillWidth, barHeight, Color.Green);

            // Render progress percentage
            var progressText = $"{(int)(progress * 100)}%";
            var progressSize = context.MeasureText(progressText, 18);
            var progressX = (context.ScreenWidth - progressSize.Width()) / 2;
            var progressY = barY + barHeight + 10;

            context.DrawText(progressText, progressX, progressY, 18, Color.White);
        }

        /// <summary>
        /// Legacy Update method for backward compatibility.
        /// </summary>
        public override void Update(float deltaTime)
        {
            OnUpdate(deltaTime);
        }

        /// <summary>
        /// Legacy Render method for backward compatibility.
        /// </summary>
        public override void Render(IRenderContext context)
        {
            OnRender(context);
        }

        /// <summary>
        /// Simulates loading work and marks the scene as ready for transition.
        /// </summary>
        private void SimulateLoadingWork()
        {
            // In a real implementation, this would:
            // - Load assets for the target scene
            // - Initialize scene-specific systems
            // - Prepare data structures
            // - Perform any required async operations

            // For now, we'll simulate a brief loading period
            Engine.Diagnostics.DebugLogger.LogDebug("INFO", "LoadingScene.SimulateLoadingWork: Simulating asset loading");

            // Mark as ready after a brief simulation
            // In a real async implementation, this would be called when loading actually completes
            System.Threading.Tasks.Task.Run(async () =>
            {
                await System.Threading.Tasks.Task.Delay(500); // 500ms simulation
                _readyToTransition = true;
                Engine.Diagnostics.DebugLogger.LogDebug("INFO", "LoadingScene.SimulateLoadingWork: Loading simulation complete");
            });
        }

        /// <summary>
        /// Gets the loading progress (0.0 to 1.0).
        /// </summary>
        public float LoadingProgress
        {
            get
            {
                if (_minLoadingDuration <= 0f)
                    return 1.0f;

                var progress = _loadingTime / _minLoadingDuration;
                return System.Math.Min(progress, 1.0f);
            }
        }

        /// <summary>
        /// Gets whether loading is complete.
        /// </summary>
        public bool IsLoadingComplete => _loadingTime >= _minLoadingDuration && _readyToTransition;
    }
}




