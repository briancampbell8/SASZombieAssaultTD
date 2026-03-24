/*
File:    PlayerLives.cs
Purpose: Player lives management system for SAS Zombie Assault TD.
Features: Lives tracking, game over detection, difficulty scaling.
*/

using System;
using SASZombieAssaultTD.Engine.Core;

namespace SASZombieAssaultTD.Engine.Economy
{
    /// <summary>
    /// Manages player lives and game over conditions.
    /// Handles lives tracking, enemy escapes, and difficulty-based life scaling.
    /// </summary>
    public class PlayerLives
    {
        private static PlayerLives _instance;
        public static PlayerLives Instance => _instance ??= new PlayerLives();

        private int _currentLives;
        private int _startingLives;
        private int _maxLives;

        private PlayerLives()
        {
            _startingLives = 20;
            _maxLives = 50;
            _currentLives = _startingLives;
            ModernLoggingSystem.Log("INFO", $"PlayerLives: Initialized with {_startingLives} starting lives");
        }

        /// <summary>
        /// Gets the current number of lives.
        /// </summary>
        public int CurrentLives => _currentLives;

        /// <summary>
        /// Gets the starting number of lives.
        /// </summary>
        public int StartingLives => _startingLives;

        /// <summary>
        /// Gets the maximum number of lives allowed.
        /// </summary>
        public int MaxLives => _maxLives;

        /// <summary>
        /// Gets the total number of lives lost.
        /// </summary>
        public int LivesLost => _startingLives - _currentLives;

        /// <summary>
        /// Event fired when lives change.
        /// </summary>
        public event Action<int> OnLivesChanged;

        /// <summary>
        /// Event fired when player runs out of lives (game over).
        /// </summary>
        public event Action OnGameOver;

        /// <summary>
        /// Sets the starting number of lives.
        /// </summary>
        /// <param name="lives">Starting lives amount</param>
        public void SetStartingLives(int lives)
        {
            _startingLives = System.Math.Max(1, System.Math.Min(lives, _maxLives));
            _currentLives = _startingLives;
            OnLivesChanged?.Invoke(_currentLives);
            ModernLoggingSystem.Log("INFO", $"PlayerLives: Set starting lives to {_startingLives}");
        }

        /// <summary>
        /// Removes a life when an enemy escapes.
        /// </summary>
        /// <param name="amount">Number of lives to remove (default: 1)</param>
        public void RemoveLife(int amount = 1)
        {
            _currentLives = System.Math.Max(0, _currentLives - amount);
            OnLivesChanged?.Invoke(_currentLives);
            ModernLoggingSystem.Log("INFO", $"PlayerLives: Removed {amount} life(s) - Remaining: {_currentLives}");

            if (_currentLives <= 0)
            {
                OnGameOver?.Invoke();
                ModernLoggingSystem.Log("WARNING", "PlayerLives: Game over - No lives remaining");
            }
        }

        /// <summary>
        /// Adds lives (bonus lives).
        /// </summary>
        /// <param name="amount">Number of lives to add</param>
        public void AddLife(int amount = 1)
        {
            _currentLives = System.Math.Min(_maxLives, _currentLives + amount);
            OnLivesChanged?.Invoke(_currentLives);
            ModernLoggingSystem.Log("INFO", $"PlayerLives: Added {amount} life(s) - Total: {_currentLives}");
        }

        /// <summary>
        /// Sets lives to a specific amount.
        /// </summary>
        /// <param name="lives">New lives amount</param>
        public void SetLives(int lives)
        {
            _currentLives = System.Math.Max(0, System.Math.Min(lives, _maxLives));
            OnLivesChanged?.Invoke(_currentLives);
            ModernLoggingSystem.Log("INFO", $"PlayerLives: Set lives to {_currentLives}");

            if (_currentLives <= 0)
            {
                OnGameOver?.Invoke();
                ModernLoggingSystem.Log("WARNING", "PlayerLives: Game over - No lives remaining");
            }
        }

        /// <summary>
        /// Resets lives to starting amount.
        /// </summary>
        public void Reset()
        {
            _currentLives = _startingLives;
            OnLivesChanged?.Invoke(_currentLives);
            ModernLoggingSystem.Log("INFO", $"PlayerLives: Reset to starting lives - {_currentLives}");
        }

        /// <summary>
        /// Checks if player has any lives remaining.
        /// </summary>
        /// <returns>True if player has lives</returns>
        public bool HasLives()
        {
            return _currentLives > 0;
        }

        /// <summary>
        /// Gets the percentage of lives remaining.
        /// </summary>
        /// <returns>Lives percentage (0-100)</returns>
        public float GetLivesPercentage()
        {
            return _startingLives > 0 ? (float)_currentLives / _startingLives * 100f : 0f;
        }
    }
}
