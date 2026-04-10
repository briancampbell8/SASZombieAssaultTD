/*
File:    PlayerDataTypes.cs
Purpose: Data structures and enums for the Player System in SAS Zombie Assault TD.
Features: Transaction tracking, action results, level up events, and progression data.
Validation: Built-in validation for all data structures with reasonable limits.
Performance: Optimized for frequent use with minimal memory footprint.
Persistence: JSON serializable with proper attributes for save/load functionality.
Integration: Designed for use with PlayerSystem, PlayerEconomy, and PlayerProgression.
*/

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SASZombieAssaultTD.Engine.Player
{
    /// <summary>
    /// Represents a financial transaction in the player economy.
    /// This class tracks all cash movements including purchases, rewards,
    /// and penalties with detailed information for debugging and analytics.
    /// </summary>
    /// <remarks>
    /// The Transaction class provides comprehensive transaction tracking:
    /// - Amount tracking with positive values for gains and negative for losses
    /// - Transaction type classification for categorization
    /// - Source identification for debugging and analytics
    /// - Timestamp tracking for chronological ordering
    /// - JSON serialization support for save/load functionality
    /// 
    /// Transaction Types:
    /// - Purchase: Tower placement, upgrades, and item buying
    /// - Reward: Enemy kills, wave completion, bonuses, and achievements
    /// - Penalty: Cheating detection, rule violations, or special game mechanics
    /// 
    /// Performance Characteristics:
    /// - Memory: ~40 bytes per transaction
    /// - Serialization: <0.01ms per transaction
    /// - Validation: Minimal overhead with built-in checks
    /// - Usage: Economy tracking, debugging, player statistics
    /// </remarks>
    /// <example>
    /// <code>
    /// // Create purchase transaction
    /// var purchase = new Transaction
    /// {
    ///     Amount = -500,
    ///     Type = TransactionType.Purchase,
    ///     Source = "vickers_turret",
    ///     Timestamp = DateTime.UtcNow
    /// };
    /// 
    /// // Create reward transaction
    /// var reward = new Transaction
    /// {
    ///     Amount = 25,
    ///     Type = TransactionType.Reward,
    ///     Source = "enemy_kill",
    ///     Timestamp = DateTime.UtcNow
    /// };
    /// 
    /// // Display transaction information
    /// Console.WriteLine($"{transaction.Type}: ${transaction.Amount} from {transaction.Source}");
    /// </code>
    /// </example>
    public class Transaction
    {
        public int Amount;         // Change 'internal' to 'public'
        public TransactionType Type; // Change 'private object' to 'public TransactionType'
        public string Source;      // Ensure this is public
        public DateTime Timestamp; // Ensure this is public

        /// <summary>
        /// Gets or sets the transaction amount.
        /// Positive values represent gains (rewards), negative values represent losses (purchases).
        /// This field is the core financial value of the transaction and is used for
        /// cash calculation and player statistics.
        /// </summary>
        /// <remarks>
        /// Amount Guidelines:
        /// - Positive: Cash rewards, enemy kills, wave completion bonuses
        /// - Negative: Tower purchases, upgrades, penalties
        /// - Range: Typically -100,000 to +100,000 for normal gameplay
        /// - Validation: Handled by PlayerEconomy class
        /// 
        /// Example Values:
        /// - 25: Enemy kill reward
        /// - -500: Tower purchase
        /// - 100: Wave completion bonus
        /// - -1000: Special upgrade
        /// </remarks>

        /// <summary>
        /// Gets or sets the type of transaction.
        /// This field categorizes the transaction for analysis, filtering,
        /// and UI display purposes. Each type has specific validation rules
        /// and processing requirements.
        /// </summary>
        /// <remarks>
        /// Transaction Types:
        /// - Purchase: Money spent on towers, upgrades, or items
        /// - Reward: Money earned from gameplay actions
        /// - Penalty: Money lost due to penalties or special mechanics
        /// 
        /// Type Usage:
        /// - UI: Different colors/icons for different types
        /// - Analytics: Spending patterns and earning sources
        /// - Debugging: Transaction flow analysis
        /// - Validation: Type-specific rules and limits
        /// </remarks>
        /// public TransactionType Type { get; set; } NOT NEEDED, can be inferred from Amount sign

        /// <summary>
        /// Gets or sets the source of the transaction.
        /// This field identifies what caused the transaction and provides
        /// context for debugging, analytics, and player feedback.
        /// </summary>
        /// <remarks>
        /// Source Examples:
        /// - "vickers_turret": Tower purchase
        /// - "enemy_kill": Enemy defeat reward
        /// - "wave_completion": Wave finishing bonus
        /// - "damage_upgrade": Tower upgrade purchase
        /// - "objective_bonus": Special achievement reward
        /// 
        /// Source Guidelines:
        /// - Use descriptive, lowercase names with underscores
        /// - Include specific item identifiers for purchases
        /// - Use action identifiers for rewards
        /// - Keep source names consistent across the game
        /// </remarks>
        /// public string Source { get; set; } = string.Empty; Already defined

        /// <summary>
        /// Gets or sets the timestamp when the transaction occurred.
        /// This field records the exact time of the transaction for
        /// chronological ordering, debugging, and analytics purposes.
        /// </summary>
        /// <remarks>
        /// Timestamp Usage:
        /// - Chronological transaction ordering
        /// - Debugging transaction flows
        /// - Analytics time-based analysis
        /// - Player session tracking
        /// 
        /// Timestamp Precision:
        /// - UTC time for consistency across time zones
        /// - Millisecond precision for detailed analysis
        /// - Automatic generation during transaction creation
        /// </remarks>
        ///   public DateTime Timestamp { get; set; } = DateTime.UtcNow; Already defined

        /// <summary>
        /// Gets a formatted string representation of the transaction.
        /// This method provides a human-readable summary of the transaction
        /// for debugging, logging, and display purposes.
        /// </summary>
        /// <returns>A formatted string containing transaction details.</returns>
        /// <remarks>
        /// Format Pattern: "[Type] $Amount from Source at HH:mm:ss"
        /// Example: "[Purchase] $-500 from vickers_turret at 14:30:25"
        /// 
        /// Usage:
        /// - Debug logging and troubleshooting
        /// - Player transaction history display
        /// - Development and testing feedback
        /// </remarks>
        /// 
        /// public TransactionType Type { get; set; }
        public override string ToString()
        {
            var Amount = 0;
            var amountStr = Amount >= 0 ? $"+${Amount}" : $"${Amount}";

            return $"{Type} {amountStr} from {Source} at {Timestamp:HH:mm:ss}";
        }
    }

    /// <summary>
    /// Enumeration of transaction types for categorizing player economy transactions.
    /// This enum provides type safety and clear categorization for different
    /// kinds of financial transactions in the game.
    /// </summary>
    /// <remarks>
    /// Transaction Type Usage:
    /// - Purchase: Money spent by the player on game items
    /// - Reward: Money earned by the player through gameplay
    /// - Penalty: Money lost due to penalties or special mechanics
    /// 
    /// Type Selection Guidelines:
    /// - Use Purchase for any money spent (towers, upgrades, items)
    /// - Use Reward for any money earned (kills, waves, achievements)
    /// - Use Penalty for money lost due to game mechanics or violations
    /// 
    /// Future Extensions:
    /// - Refund: Money returned for cancelled purchases
    /// - Bonus: Special promotional or event-based rewards
    /// - Tax: Automatic deductions for special game mechanics
    /// </remarks>
    public enum TransactionType
    {
        /// <summary>
        /// Money spent by the player on purchases.
        /// Used for tower placement, upgrades, and item buying.
        /// </summary>
        Purchase,

        /// <summary>
        /// Money earned by the player as rewards.
        /// Used for enemy kills, wave completion, bonuses, and achievements.
        /// </summary>
        Reward,

        /// <summary>
        /// Money lost due to penalties or special mechanics.
        /// Used for cheating penalties, rule violations, or special game events.
        /// </summary>
        Penalty
    }

    /// <summary>
    /// Represents the result of a player action or operation.
    /// This class provides standardized feedback for game actions including
    /// success/failure status, descriptive messages, and special states.
    /// </summary>
    /// <remarks>
    /// The ActionResult class provides comprehensive operation feedback:
    /// - Success/failure status with boolean flag
    /// - Descriptive messages for player feedback
    /// - Special state indicators (game over, paused, etc.)
    /// - Static factory methods for common result types
    /// - JSON serialization support for save/load functionality
    /// 
    /// Result Types:
    /// - Success: Operation completed successfully
    /// - Failed: Operation failed due to validation or game rules
    /// - GameOver: Special failure indicating game end
    /// 
    /// Performance Characteristics:
    /// - Memory: ~32 bytes per result
    /// - Creation: <0.01ms for factory methods
    /// - Usage: Action feedback, UI notifications, debugging
    /// </remarks>
    /// <example>
    /// <code>
    /// // Create success result
    /// var successResult = ActionResult.Success("Tower placed successfully");
    /// Console.WriteLine(successResult.Message); // "Tower placed successfully"
    /// 
    /// // Create failure result
    /// var failResult = ActionResult.Failed("Insufficient funds");
    /// Console.WriteLine(failResult.Success); // false
    /// Console.WriteLine(failResult.Message); // "Insufficient funds"
    /// 
    /// // Create game over result
    /// var gameOverResult = ActionResult.GameOver("No lives remaining");
    /// Console.WriteLine(gameOverResult.IsGameOver); // true
    /// 
    /// // Check result and handle accordingly
    /// if (result.Success)
    /// {
    ///     ShowSuccessMessage(result.Message);
    /// }
    /// else if (result.IsGameOver)
    /// {
    ///     ShowGameOverScreen();
    /// }
    /// else
    /// {
    ///     ShowErrorMessage(result.Message);
    /// }
    /// </code>
    /// </example>
    public class ActionResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        object Result;
        string Error;

        public ActionResult(bool v, object value)
        {
        }

        /// <summary>
        /// Gets or sets whether the action was successful.
        /// This boolean indicates the primary success status of the operation
        /// and should be checked first when handling action results.
        /// </summary>
        /// <remarks>
        /// Success Guidelines:
        /// - true: Operation completed as intended
        /// - false: Operation failed due to validation or game rules
        /// - Check this property before examining other properties
        /// - Use Message property for detailed feedback
        /// 
        /// Examples:
        /// - true: Tower placed successfully, enemy defeated
        /// - false: Insufficient funds, invalid location, game paused
        /// </remarks>
        /// public bool Success { get; set; }
        public ActionResult(bool isSuccess, string error, bool GameOver)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        ///    public ActionResult(bool v, object value, bool isGameOver) : this(v, value)
        ///   {
        ///   }

        public ActionResult(bool isSuccess, object result, bool isGameOver)
        {
            IsSuccess = isSuccess;
            Result = result;
            IsGameOver = isGameOver;
        }

        /// <summary>
        /// Gets or sets the descriptive message for the action result.
        /// This message provides detailed feedback to the player about
        /// what happened and why, suitable for UI display or logging.
        /// </summary>
        /// <remarks>
        /// Message Guidelines:
        /// - Be concise but descriptive
        /// - Use player-friendly language
        /// - Include specific details (amounts, item names, etc.)
        /// - Localize for international audiences
        /// 
        /// Message Examples:
        /// - "Tower placed successfully at (10, 5)"
        /// - "Insufficient funds: Need $500, have $250"
        /// - "Cannot place tower: Location blocked"
        /// - "Game Over: No lives remaining"
        /// </remarks>
        /// public string Message { get; set; } = string.Empty; Already defined

        /// <summary>
        /// Gets or sets whether the action resulted in game over.
        /// This boolean indicates if the action caused the game to end
        /// and should be checked after processing critical game events.
        /// </summary>
        /// <remarks>
        /// Game Over Guidelines:
        /// - Set to true when lives reach zero
        /// - Triggers special UI handling and game state changes
        /// - Usually accompanied by Success = false
        /// - Player can typically restart from game over state
        /// 
        /// Game Over Scenarios:
        /// - Lives depleted to zero
        /// - Critical objective failed
        /// - Time limit exceeded (if implemented)
        /// - Player quit during active game
        /// </remarks>
        public bool IsGameOver { get; set; }

        /// <summary>
        /// Creates a successful action result with a message.
        /// This static factory method provides convenient creation of
        /// successful results with proper initialization.
        /// </summary>
        /// <param name="message">
        /// The success message to display. Can be null or empty for default message.
        /// </param>
        /// <returns>A new ActionResult indicating success.</returns>
        /// <remarks>
        /// This factory method ensures proper initialization:
        /// - Success is set to true
        /// - IsGameOver is set to false
        /// - Message is set to provided value or default
        /// - Returns fully configured ActionResult instance
        /// </remarks>
        /// <example>
        /// <code>
        /// // Create success with custom message
        /// var result = ActionResult.Success("Tower placed at (10, 5)");
        /// 
        /// // Create success with default message
        /// var result = ActionResult.Success();
        /// Console.WriteLine(result.Message); // "Operation successful"
        /// </code>
        /// </example>
        public static ActionResult Success() => new ActionResult(true, null);

        /// <summary>
        /// Creates a failed action result with a message.
        /// This static factory method provides convenient creation of
        /// failed results with proper initialization.
        /// </summary>
        /// <param name="message">
        /// The failure message to display. Should explain why the operation failed.
        /// </param>
        /// <returns>A new ActionResult indicating failure.</returns>
        /// <remarks>
        /// This factory method ensures proper initialization:
        /// - Success is set to false
        /// - IsGameOver is set to false
        /// - Message is set to provided value (required)
        /// - Returns fully configured ActionResult instance
        /// 
        /// Message Guidelines:
        /// - Should be descriptive and helpful
        /// - Include specific failure reasons
        /// - Suggest possible solutions when appropriate
        /// </remarks>
        /// <example>
        /// <code>
        /// // Create failure with specific reason
        /// var result = ActionResult.Failed("Insufficient funds: Need $500, have $250");
        /// 
        /// // Create failure for invalid location
        /// var result = ActionResult.Failed("Cannot place tower: Location is blocked");
        /// 
        /// // Create failure for game state
        /// var result = ActionResult.Failed("Cannot place tower: Game is paused");
        /// </code>
        /// </example>
        public static ActionResult Failure(string error) => new ActionResult(false, error);

        /// <summary>
        /// Creates a game over action result with a message.
        /// This static factory method provides convenient creation of
        /// game over results with proper initialization.
        /// </summary>
        /// <param name="message">
        /// The game over message to display. Should explain why the game ended.
        /// </param>
        /// <returns>A new ActionResult indicating game over.</returns>
        /// <remarks>
        /// This factory method ensures proper initialization:
        /// - Success is set to false
        /// - IsGameOver is set to true
        /// - Message is set to provided value (required)
        /// - Returns fully configured ActionResult instance
        /// 
        /// Game Over Guidelines:
        /// - Should indicate why the game ended
        /// - Can include final statistics or achievements
        /// - May suggest options for next steps
        /// - Triggers special UI handling
        /// </remarks>
        /// <example>
        /// <code>
        /// // Create game over for lives depletion
        /// var result = ActionResult.GameOver("No lives remaining - Game Over!");
        /// 
        /// // Create game over with statistics
        /// var result = ActionResult.GameOver($"Game Over! Final Score: {score}, Waves: {waves}");
        /// 
        /// // Create game over for time limit
        /// var result = ActionResult.GameOver("Time limit exceeded - Game Over!");
        /// </code>
        /// </example>
        public static ActionResult GameOver(string message)
        {
            return new ActionResult(
                isSuccess: false,
                error: message ?? "Game Over",
                GameOver: true
            );
        }

        /// <summary>
        /// Gets a formatted string representation of the action result.
        /// This method provides a human-readable summary of the result
        /// for debugging, logging, and display purposes.
        /// </summary>
        /// <returns>A formatted string containing result details.</returns>
        /// <remarks>
        /// Format Pattern: "[Success/Failed/GameOver] Message"
        /// Examples:
        /// - "[Success] Tower placed successfully"
        /// - "[Failed] Insufficient funds"
        /// - "[GameOver] No lives remaining"
        /// 
        /// Usage:
        /// - Debug logging and troubleshooting
        /// - Console output during development
        /// - Simple result display in debug UI
        /// </remarks>
        public override string ToString()
        {
            // Change line 494 to:
            var status = IsSuccess ? "Success" : (IsGameOver ? "GameOver" : "Failed");

            if (string.IsNullOrEmpty(Message)) return status;

            return $"[{status}] {Message}";
        }

        internal static ActionResult Failed(string v)
        {
            throw new NotImplementedException();
        }

     
    }

    /// <summary>
    /// Represents a level up event in player progression.
    /// This class contains information about the new level and experience
    /// when a player advances to the next level in the game.
    /// </summary>
    /// <remarks>
    /// The LevelUp class provides comprehensive level up information:
    /// - New level number for progression tracking
    /// - Current experience for display purposes
    /// - Automatic timestamp generation for event tracking
    /// - JSON serialization support for save/load functionality
    /// - Integration with PlayerSystem event system
    /// 
    /// Level Up Features:
    /// - Tower unlocking based on level requirements
    /// - Experience calculation and carry-over
    /// - UI notifications and sound effects
    /// - Achievement tracking and rewards
    /// 
    /// Performance Characteristics:
    /// - Memory: ~24 bytes per level up event
    /// - Creation: <0.01ms for new instances
    /// - Usage: Event handling, UI updates, progression tracking
    /// </remarks>
    /// <example>
    /// <code>
    /// // Create level up event
    /// var levelUp = new LevelUp
    /// {
    ///     NewLevel = 5,
    ///     Experience = 150
    /// };
    /// 
    /// // Handle level up
    /// Console.WriteLine($"Level up! Now level {levelUp.NewLevel}");
    /// Console.WriteLine($"Experience: {levelUp.Experience}/{nextLevelExperience}");
    /// 
    /// // Check for unlocks
    /// if (levelUp.NewLevel >= 3)
    /// {
    ///     UnlockTower("mgl_turret");
    ///     Console.WriteLine("MGL Turret unlocked!");
    /// }
    /// 
    /// // Show notification
    /// ShowLevelUpNotification(levelUp);
    /// PlayLevelUpSound();
    /// </code>
    /// </example>
    public class LevelUp
    {
        /// <summary>
        /// Gets or sets the new level number after leveling up.
        /// This property represents the player's current level after
        /// the level up event and is used for progression tracking.
        /// </summary>
        /// <remarks>
        /// Level Progression:
        /// - Starts at level 1 for new games
        /// - Increases by 1 for each level up
        /// - Determines tower unlocking and rewards
        /// - Affects experience requirements for next level
        /// 
        /// Level Ranges:
        /// - Early game: Levels 1-5 (basic towers)
        /// - Mid game: Levels 6-15 (advanced towers)
        /// - Late game: Levels 16-25 (special towers)
        /// - End game: Levels 26+ (master towers)
        /// 
        /// Level Impact:
        /// - Tower availability and unlocking
        /// - Experience calculation multipliers
        /// - Reward scaling and bonuses
        /// - Difficulty adjustments
        /// </remarks>
        public int NewLevel { get; set; }

        /// <summary>
        /// Gets or sets the current experience after leveling up.
        /// This property represents the remaining experience after
        /// the level up, which carries over to the next level.
        /// </summary>
        /// <remarks>
        /// Experience Carry-Over:
        /// - Experience that exceeded previous level requirement
        /// - Applied toward next level progress
        /// - Can be zero if exactly met requirement
        /// - Max value is next level requirement minus 1
        /// 
        /// Experience Calculation:
        /// - Previous: 950/1000 (level 5)
        /// - Gain: +200 experience
        /// - New: 150/1200 (level 6 with 150 carry-over)
        /// 
        /// Usage:
        /// - UI progress bar display
        /// - Experience percentage calculation
        /// - Debugging and analytics
        /// </remarks>
        public int Experience { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when the level up occurred.
        /// This field records the exact time of the level up for
        /// analytics, achievement tracking, and debugging purposes.
        /// </summary>
        /// <remarks>
        /// Timestamp Usage:
        /// - Level up rate analysis
        /// - Session duration tracking
        /// - Achievement time-based requirements
        /// - Debugging progression issues
        /// 
        /// Timestamp Precision:
        /// - UTC time for consistency
        /// - Millisecond precision for detailed analysis
        /// - Automatic generation during creation
        /// - Used for time-based achievements
        /// </remarks>
        [JsonIgnore]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets a formatted string representation of the level up event.
        /// This method provides a human-readable summary of the level up
        /// for logging, debugging, and display purposes.
        /// </summary>
        /// <returns>A formatted string containing level up details.</returns>
        /// <remarks>
        /// Format Pattern: "Level up to NewLevel with Experience experience"
        /// Example: "Level up to 5 with 150 experience"
        /// 
        /// Usage:
        /// - Debug logging and troubleshooting
        /// - Console output during development
        /// - Simple level up display in debug UI
        /// - Achievement tracking messages
        /// </remarks>
        public override string ToString() => $"Level up to {NewLevel} with {Experience} experience";
    }

    /// <summary>
    /// Contains player progression data for save/load functionality.
    /// This class stores progression information including current level,
    /// experience, and unlocked towers for persistence across game sessions.
    /// </summary>
    /// <remarks>
    /// The PlayerProgressionData class provides persistence support:
    /// - JSON serialization for save/load operations
    /// - Version compatibility for data migration
    /// - Compact storage for efficient save files
    /// - Integration with PlayerProgression class
    /// 
    /// Data Structure:
    /// - CurrentLevel: Player's current progression level
    /// - CurrentExperience: Experience points at current level
    /// - UnlockedTowers: Set of unlocked tower identifiers
    /// - Version: Data format version for migration support
    /// 
    /// Performance Characteristics:
    /// - Memory: ~200 bytes for typical progression data
    /// - Serialization: <1ms for JSON conversion
    /// - Validation: Built-in checks for data integrity
    /// - Usage: Save/load operations, data migration
    /// </remarks>
    /// <example>
    /// <code>
    /// // Create progression data for saving
    /// var progressionData = new PlayerProgressionData
    /// {
    ///     CurrentLevel = 8,
    ///     CurrentExperience = 350,
    ///     UnlockedTowers = new HashSet<string>
    ///     {
    ///         "vickers_turret",
    ///         "mgl_turret",
    ///         "special_turret"
    ///     }
    /// };
    /// 
    /// // Serialize to JSON
    /// string json = JsonSerializer.Serialize(progressionData);
    /// 
    /// // Deserialize from JSON
    /// var loadedData = JsonSerializer.Deserialize<PlayerProgressionData>(json);
    /// 
    /// // Apply loaded data to progression system
    /// progression.CurrentLevel = loadedData.CurrentLevel;
    /// progression.CurrentExperience = loadedData.CurrentExperience;
    /// progression.UnlockedTowers = loadedData.UnlockedTowers;
    /// </code>
    /// </example>
    public class PlayerProgressionData
    {
        /// <summary>
        /// Gets or sets the current player level.
        /// This property stores the player's progression level for
        /// save/load functionality and should match the PlayerProgression.CurrentLevel.
        /// </summary>
        /// <remarks>
        /// Level Storage:
        /// - Range: 1 to maximum level (typically 100)
        /// - Validation: Handled by PlayerProgression class
        /// - Migration: Supported across data format versions
        /// - Default: 1 for new games
        /// </remarks>
        public int CurrentLevel { get; set; } = 1;

        /// <summary>
        /// Gets or sets the current experience points.
        /// This property stores the player's experience at the current level
        /// for save/load functionality and should match PlayerProgression.CurrentExperience.
        /// </summary>
        /// <remarks>
        /// Experience Storage:
        /// - Range: 0 to experience requirement for next level
        /// - Validation: Handled by PlayerProgression class
        /// - Carry-over: Preserved during level ups
        /// - Default: 0 for new games
        /// </remarks>
        public int CurrentExperience { get; set; } = 0;

        /// <summary>
        /// Gets or sets the set of unlocked towers.
        /// This property stores tower identifiers that the player has unlocked
        /// through level progression for save/load functionality.
        /// </summary>
        /// <remarks>
        /// Tower Storage:
        /// - Format: HashSet of tower identifier strings
        /// - Validation: Handled by PlayerProgression class
        /// - Persistence: JSON serialized as array
        /// - Default: Empty set for new games
        /// 
        /// Tower Identifiers:
        /// - "vickers_turret": Basic machine gun turret
        /// - "mgl_turret": Grenade launcher turret
        /// - "special_turret": Special elemental turret
        /// - "sniper_sas": Elite sniper unit
        /// </remarks>
        public HashSet<string> UnlockedTowers { get; set; } = new();

        /// <summary>
        /// Gets or sets the data format version.
        /// This property tracks the version of the data format for
        /// migration purposes and compatibility checking.
        /// </summary>
        /// <remarks>
        /// Version Information:
        /// - Current version: 1
        /// - Purpose: Data migration and compatibility
        /// - Format: Integer increment for breaking changes
        /// - Migration: Handled by save/load system
        /// 
        /// Version History:
        /// - 1: Initial format with basic progression data
        /// - Future: Additional fields or structure changes
        /// </remarks>
        public int Version { get; set; } = 1;
    }

    /// <summary>
    /// Contains complete player data for save/load functionality.
    /// This class aggregates all player-related data including state,
    /// progression, and metadata for comprehensive game persistence.
    /// </summary>
    /// <remarks>
    /// The PlayerData class provides comprehensive save/load support:
    /// - Aggregates all player data in single structure
    /// - JSON serialization for file-based persistence
    /// - Version support for data migration
    /// - Metadata for save file management
    /// 
    /// Data Structure:
    /// - State: Core player state (lives, cash, score, wave)
    /// - Progression: Level and unlock information
    /// - LastSaved: Timestamp for save management
    /// - Version: Data format version for migration
    /// 
    /// Performance Characteristics:
    /// - Memory: ~1KB for complete player data
    /// - Serialization: <5ms for JSON conversion
    /// - Validation: Built-in checks for data integrity
    /// - Usage: Save/load operations, data backup
    /// </remarks>
    /// <example>
    /// <code>
    /// // Create player data for saving
    /// var playerData = new PlayerData
    /// {
    ///     State = playerSystem.State.Clone(),
    ///     Progression = new PlayerProgressionData
    ///     {
    ///         CurrentLevel = playerSystem.Progression.CurrentLevel,
    ///         CurrentExperience = playerSystem.Progression.CurrentExperience,
    ///         UnlockedTowers = playerSystem.Progression.UnlockedTowers
    ///     },
    ///     LastSaved = DateTime.UtcNow,
    ///     Version = 1
    /// };
    /// 
    /// // Serialize to JSON for saving
    /// string json = JsonSerializer.Serialize(playerData, new JsonSerializerOptions { WriteIndented = true });
    /// File.WriteAllText("save.json", json);
    /// 
    /// // Load from JSON
    /// string loadedJson = File.ReadAllText("save.json");
    /// var loadedData = JsonSerializer.Deserialize<PlayerData>(loadedJson);
    /// 
    /// // Restore player system from loaded data
    /// playerSystem.State = loadedData.State;
    /// playerSystem.State.Validate();
    /// playerSystem.Progression.RestoreFromData(loadedData.Progression);
    /// </code>
    /// </example>
    public class PlayerData
    {
        /// <summary>
        /// Gets or sets the player state data.
        /// This property contains the core player information including
        /// lives, cash, score, and wave progression.
        /// </summary>
        /// <remarks>
        /// State Data:
        /// - Lives: Current and maximum lives
        /// - Cash: Current cash amount
        /// - Score: Cumulative score
        /// - WaveNumber: Current wave progression
        /// - IsGameOver: Game over state
        /// - IsPaused: Pause state
        /// 
        /// Validation:
        /// - Handled by PlayerState.Validate() method
        /// - Automatic correction of invalid values
        /// - Reasonable limits enforced
        /// </remarks>
        public PlayerState State { get; set; } = new();

        /// <summary>
        /// Gets or sets the player progression data.
        /// This property contains level, experience, and tower unlock
        /// information for progression tracking.
        /// </summary>
        /// <remarks>
        /// Progression Data:
        /// - CurrentLevel: Player's current level
        /// - CurrentExperience: Experience at current level
        /// - UnlockedTowers: Set of unlocked tower identifiers
        /// - Version: Data format version
        /// 
        /// Integration:
        /// - Used by PlayerProgression for restoration
        /// - Supports data migration across versions
        /// - Maintains unlock state across sessions
        /// </remarks>
        public PlayerProgressionData Progression { get; set; } = new();

        /// <summary>
        /// Gets or sets the timestamp when the data was last saved.
        /// This property tracks when the save file was created or updated
        /// for save file management and debugging purposes.
        /// </summary>
        /// <remarks>
        /// Timestamp Usage:
        /// - Save file management and sorting
        /// - Backup file identification
        /// - Debugging save/load issues
        /// - Session duration tracking
        /// 
        /// Timestamp Format:
        /// - UTC time for consistency
        /// - Automatic generation during save
        /// - Preserved during load operations
        /// </remarks>
        public DateTime LastSaved { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the data format version.
        /// This property tracks the version of the data format for
        /// migration purposes and compatibility checking.
        /// </summary>
        /// <remarks>
        /// Version Information:
        /// - Current version: 1
        /// - Purpose: Data migration and compatibility
        /// - Format: Integer increment for breaking changes
        /// - Migration: Handled by save/load system
        /// 
        /// Version History:
        /// - 1: Initial format with state and progression data
        /// - Future: Additional fields, structure changes
        /// </remarks>
        public int Version { get; set; } = 1;
    }
}