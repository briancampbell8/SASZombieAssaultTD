using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.UI;
using SASZombieAssaultTD.Engine.VectorMath;
using SASZombieAssaultTD.Engine.UI.Components;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace SASZombieAssaultTD.Engine.Scenes
{
    /// <summary>
    /// Main menu scene for the game.
    /// Implements main menu UI and navigation.
    /// </summary>
    public class MainMenuScene : BaseScene
    {
        readonly List<UIElementBase> _menuElements = new();
        int _selectedOption;
        bool _isInitialized;

        public override void Initialize()
        {
            if (_isInitialized) return;

            ModernLoggingSystem.Log("INFO", "MainMenuScene: Initializing main menu");
            CreateMenuElements();
            _isInitialized = true;
        }

        public override void Update(float deltaTime)
        {
            if (!_isInitialized) return;

            UpdateMenuAnimations(deltaTime);
            ProcessMenuInput();
        }

        public override void Render(IRenderContext renderContext)
        {
            if (!_isInitialized || renderContext == null) return;

            RenderMenuBackground(renderContext);
            RenderMenuElements(renderContext);
        }

        void CreateMenuElements()
        {
            _menuElements.Clear();
            _selectedOption = 0;

            _menuElements.AddRange(new[]
            {
                new Button
                {
                    Text = "Start Game",
                    Position = new Point((int)new Vector3(400, 200, 0f).X, (int)new Vector3(400, 200, 0f).Y),
                    Size = new Core.Size((int)new Vector3(200, 50, 0f).X, (int)new Vector3(200, 50, 0f).Y),
                    OnClick = OnStartGameClick,
                    BackgroundColor = GetFromArgb(System.Drawing.Color.FromArgb(50, 50, 100, 255))

                },
                new Button
                {
                    Text = "Options",
                    Position = new Point((int)new Vector3(400, 270, 0f).X, (int)new Vector3(400, 270, 0f).Y),
                    Size = new Core.Size((int)new Vector3(200, 50, 0f).X, (int)new Vector3(200, 50, 0f).Y),
                    OnClick = OnOptionsClick,
                    BackgroundColor = GetFromArgb(System.Drawing.Color.FromArgb(50, 50, 100, 255))
                },
                new Button
                {
                    Text = "Quit",
                    Position = new Point((int)new Vector3(400, 340, 0f).X, (int)new Vector3(400, 340, 0f).Y),
                    Size = new Core.Size((int)new Vector3(200, 50, 0f).X, (int)new Vector3(200, 50, 0f).Y),
                    OnClick = OnQuitClick,
                    BackgroundColor = GetFromArgb(System.Drawing.Color.FromArgb(50, 50, 100, 255))
                }
            });

            ModernLoggingSystem.Log("INFO", $"MainMenuScene: Created {_menuElements.Count} menu elements");
        }

           static Color GetFromArgb (System.Drawing.Color color) => new Color(color.R, color.G, color.B, color.A);


        void UpdateMenuAnimations(float deltaTime)
        {
            foreach (var element in _menuElements)
            {
                if (element is Button button)
                {
                    button.Update(deltaTime);
                }
            }
        }

        void ProcessMenuInput()
        {
            if (InputRouter == null || _menuElements.Count == 0) return;

            try
            {
                var inputObj = InputRouter.GetMenuInput();
                if (inputObj == null) return;

                // Check if input has the expected properties
                var upPressed = false;
                var downPressed = false;
                var selectPressed = false;

                // Try to get input properties using reflection or duck typing
                var inputType = inputObj.GetType();
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
                    ModernLoggingSystem.Log("DEBUG", $"MainMenuScene: Selected option {_selectedOption}");
                    UpdateMenuSelection();
                }
                else if (downPressed)
                {
                    _selectedOption = (_selectedOption + 1) % _menuElements.Count;
                    ModernLoggingSystem.Log("DEBUG", $"MainMenuScene: Selected option {_selectedOption}");
                    UpdateMenuSelection();
                }
                else if (selectPressed && _selectedOption >= 0 && _selectedOption < _menuElements.Count)
                {
                    if (_menuElements[_selectedOption] is Button button)
                    {
                        button.OnClick?.Invoke();
                        ModernLoggingSystem.Log("DEBUG", $"MainMenuScene: Activated menu option {_selectedOption}");
                    }
                }
            }
            catch (Exception ex)
            {
                ModernLoggingSystem.Log("ERROR", $"MainMenuScene: Error processing menu input: {ex.Message}");
                // Fallback: keep current selection
            }
        }

        /// <summary>
        /// Updates the visual selection state of menu elements.
        /// </summary>
        void UpdateMenuSelection()
        {
            for (int i = 0; i < _menuElements.Count; i++)
            {
                if (_menuElements[i] is Button button)
                {
                    // Update button appearance based on selection
                    button.BackgroundColor = i == _selectedOption ?
                        System.Drawing.Color.FromArgb(100, 100, 200, 255) :
                        System.Drawing.Color.FromArgb(50, 50, 100, 255);
                }
            }
        }

        void RenderMenuBackground(IRenderContext renderContext)
        {
            renderContext.Clear(new Color(20, 20, 40));
        }

        void RenderMenuElements(IRenderContext renderContext)
        {
            foreach (var element in _menuElements)
                element.Render(renderContext);
            
        }

        void OnStartGameClick()
        {
            ModernLoggingSystem.Log("INFO", "MainMenuScene: Start Game clicked - transitioning to Gameplay");
        }

        void OnOptionsClick()
        {
            ModernLoggingSystem.Log("INFO", "MainMenuScene: Options clicked - opening options menu");
        }

        void OnQuitClick()
        {
            ModernLoggingSystem.Log("INFO", "MainMenuScene: Quit clicked - initiating shutdown");
        }

        public override void Cleanup()
        {
            ModernLoggingSystem.Log("INFO", "MainMenuScene: Cleaning up main menu");
            _menuElements.Clear();
            _selectedOption = 0;
            _isInitialized = false;
        }
    }
}