/*
File:    BackgroundRenderer.cs
File Path: Engine\Rendering\BackgroundRenderer.cs
Purpose: Background rendering system for SAS Zombie Assault TD.
Features: Animated backgrounds, parallax effects, and transitions.
*/

using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.VectorMath;
using System;

namespace SASZombieAssaultTD.Engine.Rendering
{
    /// <summary>
    /// Background renderer for main menu and game states.
    /// Handles animated backgrounds with parallax scrolling and transitions.
    /// </summary>
    public class BackgroundRenderer
    {
        #region Properties

        /// <summary>
        /// Gets or sets whether the background renderer is active.
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// Gets or sets the background name/identifier.
        /// </summary>
        public string BackgroundName { get; private set; } = string.Empty;

        /// <summary>
        /// Gets or sets the animation time.
        /// </summary>
        public float AnimationTime { get; private set; }

        /// <summary>
        /// Gets or sets the background color.
        /// </summary>
        public Color BackgroundColor { get; set; } = new Color(20, 20, 40);

        /// <summary>
        /// Gets or sets the parallax speed.
        /// </summary>
        public Vector3 ParallaxSpeed { get; set; } = new Vector3(10.0f, 5.0f, 0f);

        #endregion

        #region Fields

        private bool _isInitialized = false;
        private float _transitionTime = 0f;
        private float _transitionDuration = 1.0f;
        private Color _startColor;
        private Color _targetColor;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes the background renderer with the specified background.
        /// </summary>
        /// <param name="backgroundName">The name of the background to load.</param>
        public void Initialize(string backgroundName)
        {
            if (string.IsNullOrEmpty(backgroundName))
            {
                ModernLoggingSystem.Log("ERROR", "BackgroundRenderer: Background name cannot be null or empty");
                return;
            }

            BackgroundName = backgroundName;
            _isInitialized = true;
            IsActive = false;
            AnimationTime = 0f;

            // Set default colors based on background name
            switch (backgroundName.ToLower())
            {
                case "main_menu_background":
                    BackgroundColor = new Color(20, 20, 40);
                    break;
                case "game_background":
                    BackgroundColor = new Color(10, 30, 10);
                    break;
                default:
                    BackgroundColor = new Color(30, 30, 30);
                    break;
            }

            _startColor = BackgroundColor;
            _targetColor = BackgroundColor;

            ModernLoggingSystem.Log("DEBUG", $"BackgroundRenderer: Initialized background '{backgroundName}'");
        }

        #endregion

        #region Control Methods

        /// <summary>
        /// Starts the background animation.
        /// </summary>
        public void Start()
        {
            if (!_isInitialized)
            {
                ModernLoggingSystem.Log("WARNING", "BackgroundRenderer: Cannot start - not initialized");
                return;
            }

            IsActive = true;
            AnimationTime = 0f;

            ModernLoggingSystem.Log("DEBUG", $"BackgroundRenderer: Started background '{BackgroundName}'");
        }

        /// <summary>
        /// Stops the background animation.
        /// </summary>
        public void Stop()
        {
            if (!_isInitialized)
                return;

            IsActive = false;
            AnimationTime = 0f;

            ModernLoggingSystem.Log("DEBUG", $"BackgroundRenderer: Stopped background '{BackgroundName}'");
        }

        #endregion

        #region Update Methods

        /// <summary>
        /// Updates the background animation.
        /// </summary>
        /// <param name="deltaTime">Time since last update.</param>
        public void Update(float deltaTime)
        {
            if (!_isInitialized || !IsActive)
                return;

            AnimationTime += deltaTime;

            // Update color transition if active
            if (_transitionTime < _transitionDuration)
            {
                _transitionTime += deltaTime;
                float progress = System.Math.Min(_transitionTime / _transitionDuration, 1.0f);
                BackgroundColor = Color.Lerp(_startColor, _targetColor, progress);
            }

            // Add subtle animation effects
            UpdateBackgroundAnimation(deltaTime);
        }

        /// <summary>
        /// Updates background animation effects.
        /// </summary>
        /// <param name="deltaTime">Time since last update.</param>
        private void UpdateBackgroundAnimation(float deltaTime)
        {
            // Add subtle pulsing effect
            float pulse = (float)System.Math.Sin(AnimationTime * 0.5f) * 0.1f + 1.0f;
            
            // Modulate background color slightly
            var baseColor = _targetColor;
            BackgroundColor = new Color(
                (int)(baseColor.R * pulse),
                (int)(baseColor.G * pulse),
                (int)(baseColor.B * pulse)
            );
        }

        #endregion

        #region Rendering

        /// <summary>
        /// Renders the background.
        /// </summary>
        public void Render()
        {
            if (!_isInitialized)
                return;

            try
            {
                // Clear the screen with background color
                // This would integrate with the actual rendering system
                RenderBackground();
            }
            catch (Exception ex)
            {
                ModernLoggingSystem.Log("ERROR", $"BackgroundRenderer: Failed to render background '{BackgroundName}': {ex.Message}");
            }
        }

        /// <summary>
        /// Performs the actual background rendering.
        /// </summary>
        private void RenderBackground()
        {
            // This would integrate with the actual rendering context
            // For now, we'll just log the rendering attempt
            ModernLoggingSystem.Log("DEBUG", $"BackgroundRenderer: Rendering background '{BackgroundName}' with color {BackgroundColor}");
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Transitions the background color to a new color.
        /// </summary>
        /// <param name="targetColor">The target color.</param>
        /// <param name="duration">Transition duration in seconds.</param>
        public void TransitionToColor(Color targetColor, float duration = 1.0f)
        {
            if (!_isInitialized)
                return;

            _startColor = BackgroundColor;
            _targetColor = targetColor;
            _transitionDuration = duration;
            _transitionTime = 0f;

            ModernLoggingSystem.Log("DEBUG", $"BackgroundRenderer: Starting color transition to {targetColor} over {duration}s");
        }

        /// <summary>
        /// Gets debug information about the background renderer.
        /// </summary>
        /// <returns>Debug information string.</returns>
        public string GetDebugInfo()
        {
            var info = $"BackgroundRenderer Debug Info:\n";
            info += $"  Background Name: {BackgroundName}\n";
            info += $"  Is Initialized: {_isInitialized}\n";
            info += $"  Is Active: {IsActive}\n";
            info += $"  Animation Time: {AnimationTime:F3}s\n";
            info += $"  Background Color: {BackgroundColor}\n";
            info += $"  Transition Progress: {_transitionTime:F3}s / {_transitionDuration:F3}s\n";

            return info;
        }

        #endregion
    }
}
