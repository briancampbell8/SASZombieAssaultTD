/*
File:    GameOverUI.cs
Folder:  Engine/UI/
Purpose:  Game over UI management for SAS Zombie Assault TD.
Features: Display game over screen, statistics, and navigation options.
*/

using System;
using SASZombieAssaultTD.Engine.VectorMath;
using SASZombieAssaultTD.Engine.Rendering;

namespace SASZombieAssaultTD.Engine.UI
{
    /// <summary>
    /// Game over UI component for SAS Zombie Assault TD.
    /// Manages game over screen display and user interactions.
    /// </summary>
    public class GameOverUI
    {
        ///  Properties

        /// <summary>
        /// Whether the game over UI is visible.
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// Current game over message.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// 

        ///  Events

        /// <summary>
        /// Event triggered when retry is selected.
        /// </summary>
        public event Action OnRetry;

        /// <summary>
        /// Event triggered when main menu is selected.
        /// </summary>
        public event Action OnMainMenu;

        /// <summary>
        /// Event triggered when high scores is selected.
        /// </summary>
        public event Action OnHighScores;

        /// 

        ///  Constructor

        /// <summary>
        /// Creates a new GameOverUI instance.
        /// </summary>
        public GameOverUI()
        {
            IsVisible = false;
        }

        /// 

        ///  Public Methods

        /// <summary>
        /// Shows the game over screen with specified message.
        /// </summary>
        /// <param name="message">The game over message.</param>
        public void Show(string message)
        {
            Message = message ?? "Game Over";
            IsVisible = true;
        }

        /// <summary>
        /// Hides the game over screen.
        /// </summary>
        public void Hide()
        {
            IsVisible = false;
        }

        /// <summary>
        /// Updates the game over UI.
        /// </summary>
        /// <param name="deltaTime">Time since last update.</param>
        public void Update(float deltaTime)
        {
            // Handle input and animations
        }

        /// <summary>
        /// Renders the game over UI.
        /// </summary>
        /// <param name="context">Render context.</param>
        public void Render(IRenderContext context)
        {
            if (!IsVisible) return;

            // Render game over screen
        }

        /// <summary>
        /// Initializes the game over UI.
        /// </summary>
        public void Initialize()
        {
            IsVisible = false;
            Message = string.Empty;
        }

        /// <summary>
        /// Shows an error message.
        /// </summary>
        /// <param name="error">The error message.</param>
        public void ShowError(string error)
        {
            Message = $"ERROR: {error}";
            IsVisible = true;
        }

        /// <summary>
        /// Shows the high scores screen.
        /// </summary>
        public void ShowHighScores()
        {
            Message = "High Scores";
            IsVisible = true;
        }

        /// 
    }
}
