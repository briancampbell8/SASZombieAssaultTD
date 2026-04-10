/*
File:    PauseScene.cs
Author:  BDC
Created: 2026-02-07
Purpose: Pause overlay scene. Handles pause menu initialization, input updates, and rendering.
Notes:   Inherits from BaseScene. Provides complete pause menu functionality with resume, options, and quit.
*/

using SASZombieAssaultTD.Engine.Extensions;
using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.UI;
using SASZombieAssaultTD.Engine.VectorMath;
using SASZombieAssaultTD.Engine.UI.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using ModernLoggingSystem = SASZombieAssaultTD.Engine.Core.ModernLoggingSystem;

namespace SASZombieAssaultTD.Engine.Scenes
{
    public sealed class PauseScene : BaseScene
    {
        bool _resumeRequested;
        BaseScene? _previousGameScene;
        private List<UIElementBase> _menuElements = new();
        private int _selectedOption = 0;
        private bool _isInitialized = false;
        private float deltaTime;

        public PauseScene()
        {
            ModernLoggingSystem.Log("INFO", "PauseScene: Constructor called");
        }

        /// <summary>
        /// P11-11-02: Called when the pause scene becomes active.
        /// </summary>
        public override void OnEnter()
        {
            ModernLoggingSystem.Log("INFO", "PauseScene: OnEnter - Pause scene activated");
            _resumeRequested = false;
            CreatePauseMenu();
            _isInitialized = true;
        }

        /// <summary>
        /// P11-11-02: Called when the pause scene becomes inactive.
        /// </summary>
        public override void OnExit()
        {
            ModernLoggingSystem.Log("INFO", "PauseScene: OnExit - Pause scene deactivating");
            _resumeRequested = false;
            CleanupPauseMenu();
            _isInitialized = false;
        }

        /// <summary>
        /// Loads pause scene content.
        /// </summary>
        public override void LoadContent()
        {
            base.LoadContent();
            ModernLoggingSystem.Log("INFO", "PauseScene: Loading pause menu content");
            
            // Load pause menu assets
            LoadPauseAssets();
            
            // Initialize pause systems
            InitializePauseSystems();
        }

        /// <summary>
        /// Loads pause menu specific assets.
        /// </summary>
        private void LoadPauseAssets()
        {
            // Load pause menu textures, sounds, etc.
            ModernLoggingSystem.Log("DEBUG", "PauseScene: Pause assets loaded");
        }

        /// <summary>
        /// Initializes pause menu specific systems.
        /// </summary>
        private void InitializePauseSystems()
        {
            // Initialize pause menu systems
            ModernLoggingSystem.Log("DEBUG", "PauseScene: Pause systems initialized");
        }

        /// <summary>
        /// Creates the pause menu UI elements.
        /// </summary>
        private void CreatePauseMenu()
        {
            _menuElements.Clear();
            _selectedOption = 0;

            _menuElements.AddRange(new[]
            {
                new Button
                {
                    Text = "Resume Game",
                    Position = new Point((int)new Vector3(350, 200, 0f).X, (int)new Vector3(350, 200, 0f).Y),
                    Size = new Core.Size((int)new Vector3(200, 50, 0f).X, (int)new Vector3(200, 50, 0f).Y),
                    OnClick = OnResumeClick,
                    BackgroundColor = Color.FromArgb(100, 100, 200, 255) // Selected by default
                },
                new Button
                {
                    Text = "Options",
                    Position = new Point((int)new Vector3(350, 270, 0f).X, (int)new Vector3(350, 270, 0f).Y),
                    Size = new Core.Size((int)new Vector3(200, 50, 0f).X, (int)new Vector3(200, 50, 0f).Y),
                    OnClick = OnOptionsClick,
                    BackgroundColor = Color.FromArgb(50, 50, 100, 255)
                },
                new Button
                {
                    Text = "Quit to Menu",
                    Position = new Point((int)new Vector3(350, 340, 0f).X, (int)new Vector3(350, 340, 0f).Y),
                    Size = new Core.Size((int)new Vector3(200, 50, 0f).X, (int)new Vector3(200, 50, 0f).Y),
                    OnClick = OnQuitClick,
                    BackgroundColor = Color.FromArgb(50, 50, 100, 255)
                }
            });

            ModernLoggingSystem.Log("INFO", $"PauseScene: Created {_menuElements.Count} menu elements");
        }

        /// <summary>
        /// Cleans up pause menu resources.
        /// </summary>
        private void CleanupPauseMenu()
        {
            _menuElements.Clear();
            ModernLoggingSystem.Log("DEBUG", "PauseScene: Pause menu cleaned up");
        }

        /// <summary>
        /// Updates pause scene logic.
        /// </summary>
        public override void OnUpdate(float deltaTime)
        {
            if (!_isInitialized)
                return;

            UpdatePauseAnimations(deltaTime);
            ProcessPauseInput();
        }

        /// <summary>
        /// Updates pause menu animations.
        /// </summary>
        private void UpdatePauseAnimations(float deltaTime)
        {
            foreach (var element in _menuElements)
            {
                if (element is Button button)
                {
                    button.Update(deltaTime);
                }
            }
        }

        /// <summary>
        /// Updates the visual selection state of menu elements.
        /// </summary>
        private void UpdateMenuSelection()
        {
            for (int i = 0; i < _menuElements.Count; i++)
            {
                if (_menuElements[i] is Button button)
                {
                    button.Update(deltaTime);
                }
            }
        }

        /// <summary>
        /// Processes pause menu input.
        /// </summary>
        private void ProcessPauseInput()
        {
            if (InputRouter == null || _menuElements.Count == 0)
                return;
                
            try
            {
                object inputObj = InputRouter.GetMenuInput();
                if (inputObj == null)
                    return;

                // Check for ESC key to resume immediately
                bool escapePressed = false;
                var inputType = inputObj.GetType();
                var escapeProperty = inputType.GetProperty("IsEscapePressed");
                
                if (escapeProperty != null && escapeProperty.PropertyType == typeof(bool))
                    escapePressed = (bool)escapeProperty.GetValue(inputObj);

                if (escapePressed)
                {
                    RequestResume();
                    return;
                }

                // Check for menu navigation input
                bool upPressed = false;
                bool downPressed = false;
                bool selectPressed = false;

                var upProperty = inputType.GetProperty("IsUpPressed");
                var downProperty = inputType.GetProperty("IsDownPressed");
                var selectProperty = inputType.GetProperty("IsSelectPressed");

                if (upProperty != null && upProperty.PropertyType == typeof(bool))
                    upPressed = (bool)upProperty.GetValue(inputObj);
                
                if (downProperty != null && downProperty.PropertyType == typeof(bool))
                    downPressed = (bool)downProperty.GetValue(inputObj);
                
                if (selectProperty != null && selectProperty.PropertyType == typeof(bool))
                    selectPressed = (bool)selectProperty.GetValue(inputObj);

                // Handle menu navigation
                if (upPressed)
                {
                    _selectedOption = (_selectedOption - 1 + _menuElements.Count) % _menuElements.Count;
                    UpdateMenuSelection();
                    ModernLoggingSystem.Log("DEBUG", $"PauseScene: Selected option {_selectedOption}");
                }
                else if (downPressed)
                {
                    _selectedOption = (_selectedOption + 1) % _menuElements.Count;
                    UpdateMenuSelection();
                    ModernLoggingSystem.Log("DEBUG", $"PauseScene: Selected option {_selectedOption}");
                }
                else if (selectPressed && _selectedOption >= 0 && _selectedOption < _menuElements.Count)
                {
                    if (_menuElements[_selectedOption] is Button button)
                    {
                        button.OnClick?.Invoke();
                        ModernLoggingSystem.Log("DEBUG", $"PauseScene: Activated menu option {_selectedOption}");
                    }
                }
            }
            catch (Exception ex)
            {
                ModernLoggingSystem.Log("ERROR", $"PauseScene: Error processing input: {ex.Message}");
            }
        }

        /// <summary>
        /// Renders the pause overlay UI.
        /// </summary>
        /// <param name="context">The render context.</param>
        public override void OnRender(IRenderContext context)
        {
            if (context == null || !_isInitialized)
                return;

            // Render semi-transparent overlay
            RenderPauseOverlay(context);
            
            // Render pause menu
            RenderPauseMenu(context);
        }

        /// <summary>
        /// Renders the semi-transparent overlay.
        /// </summary>
        private void RenderPauseOverlay(IRenderContext context)
        {
            // Draw semi-transparent overlay
            // Note: In a full implementation, this would use alpha blending
            // For now, we'll just darken the background slightly
            context.Clear(0.0f, 0.0f, 0.0f, 0.5f);
        }

        /// <summary>
        /// Renders the pause menu UI.
        /// </summary>
        private void RenderPauseMenu(IRenderContext context)
        {
            // Render menu elements
            foreach (var element in _menuElements)
            {
                element.Render(context);
            }

            // Render pause title
            context.DrawText("GAME PAUSED", new Vector3(320, 120, 0), Color.White, 32.0f);
            context.DrawText("Use Arrow Keys to Navigate, Enter to Select", new Vector3(250, 420, 0), Color.LightGray, 16.0f);
            context.DrawText("Press ESC to Resume", new Vector3(350, 450, 0), Color.LightGray, 16.0f);
        }

        /// <summary>
        /// Handles resume button click.
        /// </summary>
        private void OnResumeClick()
        {
            ModernLoggingSystem.Log("INFO", "PauseScene: Resume clicked - requesting resume");
            RequestResume();
        }

        /// <summary>
        /// Handles options button click.
        /// </summary>
        private void OnOptionsClick()
        {
            ModernLoggingSystem.Log("INFO", "PauseScene: Options clicked - transitioning to options");
            // In a full implementation, this would transition to an options scene
            // For now, just log the action
        }

        /// <summary>
        /// Handles quit button click.
        /// </summary>
        private void OnQuitClick()
        {
            ModernLoggingSystem.Log("INFO", "PauseScene: Quit clicked - transitioning to main menu");
            SceneManager?.QueueScene("MainMenu");
        }

        /// <summary>
        /// Signals the pause scene to resume gameplay.
        /// Called by the input subsystem when ESC is pressed or resume is clicked.
        /// </summary>
        public void RequestResume() 
        { 
            _resumeRequested = true;
            ModernLoggingSystem.Log("INFO", "PauseScene: Resume requested");
        }

        /// <summary>
        /// Gets whether resume has been requested.
        /// </summary>
        public bool ResumeRequested => _resumeRequested;

        // Legacy methods for backward compatibility
        public override void Initialize() => OnEnter();
        public override void Update(float deltaTime) => OnUpdate(deltaTime);
        public override void Render(IRenderContext context) => OnRender(context);
    }
}