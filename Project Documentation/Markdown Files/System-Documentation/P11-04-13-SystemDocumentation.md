# P11-04-13 Achievement, Challenge, and Meta Progression Systems Documentation

## Overview

This document provides comprehensive documentation for all achievement, challenge, and meta progression systems implemented in P11-04-13. These systems provide long-term player progression, daily/weekly challenges, and achievement tracking with full save/load support and UI integration.

## System Architecture

### Core Components

1. **AchievementDefinition.cs** - Defines achievement metadata and requirements
2. **AchievementInstance.cs** - Tracks per-player achievement progress
3. **AchievementSystem.cs** - Manages achievement evaluation and unlocking
4. **ChallengeDefinition.cs** - Defines challenge metadata and rewards
5. **ChallengeInstance.cs** - Tracks per-player challenge progress with expiration
6. **ChallengeSystem.cs** - Manages daily/weekly challenge rotation and completion
7. **MetaProgressionSystem.cs** - Manages long-term player progression (XP, levels, tokens)

### Event System

1. **AchievementUnlockedEvent.cs** - Published when achievements are completed
2. **ChallengeCompletedEvent.cs** - Published when challenges are completed
3. **LevelUpEvent.cs** - Published when player levels up

### Save/Load Integration

1. **SaveGameData.cs** - Extended with achievement, challenge, and meta progression data
2. **SaveManager.cs** - Extended to collect new progression data
3. **LoadSystem.cs** - Extended to restore new progression data

### UI Integration

1. **UISystem.cs** - Extended with achievement and challenge UI subsystems

## Detailed System Documentation

### AchievementDefinition.cs

**Purpose**: Defines achievement metadata including requirements, rewards, and presentation data.

**Key Features**:
- Achievement categories (Combat, Survival, Collection, Economy, Special)
- Rarity tiers (Common, Uncommon, Rare, Epic, Legendary)
- Requirement types (KillCount, EntityTypeKills, DeathTypeKills, ScoreThreshold, etc.)
- Entity and death type filtering for specific achievements
- Serialization support with DataContract attributes
- Validation methods for data integrity

**Key Methods**:
- `IsValid()` - Validates achievement definition
- `MatchesCriteria()` - Checks if achievement matches specific criteria

**Usage Example**:
```csharp
var achievement = new AchievementDefinition
{
    Id = "first_blood",
    Name = "First Blood",
    Description = "Get your first kill",
    Category = AchievementCategory.Combat,
    Rarity = AchievementRarity.Common,
    RequirementType = AchievementRequirementType.KillCount,
    RequirementTarget = 1,
    PointValue = 10,
    IsActive = true
};
```

### AchievementInstance.cs

**Purpose**: Tracks per-player achievement progress and completion state.

**Key Features**:
- Progress tracking with current and target values
- Completion state with timestamps
- Update count for analytics
- Custom progress data for complex achievements
- Reset functionality for debugging
- Clone support for data copying

**Key Methods**:
- `UpdateProgress(amount, target)` - Increments progress and checks completion
- `SetProgress(progress, target)` - Sets absolute progress value
- `MarkCompleted()` - Marks achievement as completed
- `ResetProgress()` - Resets all progress data
- `GetCompletionPercentage(target)` - Returns 0.0-1.0 progress
- `GetRemainingProgress(target)` - Returns remaining progress needed

**Usage Example**:
```csharp
var instance = new AchievementInstance("first_blood");
var newlyCompleted = instance.UpdateProgress(1, 1);
if (newlyCompleted)
{
    // Handle achievement completion
}
```

### AchievementSystem.cs

**Purpose**: Manages achievement tracking, evaluation, and unlocking through event-driven architecture.

**Key Features**:
- Event-driven progress evaluation
- Real-time achievement checking
- Achievement publishing via AchievementUnlockedEvent
- Support for multiple requirement types
- Audit-friendly logging
- ECS-friendly design

**Event Subscriptions**:
- `KillAttributedEvent` - For kill-based achievements
- `RoundCompletedEvent` - For round-based achievements
- `ResourceChangedEvent` - For economy achievements
- `ItemAddedEvent` - For collection achievements
- `ScoreUpdatedEvent` - For score-based achievements

**Key Methods**:
- `RegisterAchievement()` - Registers achievement definitions
- `GetAchievementInstance()` - Retrieves achievement progress
- `GetCompletedAchievements()` - Gets all completed achievements
- `ForceCompleteAchievement()` - Debug method to complete achievements

**Usage Example**:
```csharp
var system = new AchievementSystem(eventBus, logger);
system.Initialize();

// Register achievements
system.RegisterAchievement(achievementDefinition);

// System automatically processes events and updates achievements
```

### ChallengeDefinition.cs

**Purpose**: Defines daily/weekly challenge metadata including objectives, rewards, and expiration.

**Key Features**:
- Challenge types (Daily, Weekly, Special)
- Difficulty tiers (Easy, Normal, Hard, Extreme)
- Time-based expiration support
- Reward definitions (XP, Currency)
- Entity and death type filtering
- Serialization support

**Key Methods**:
- `IsValid()` - Validates challenge definition
- `MatchesCriteria()` - Checks if challenge matches specific criteria
- `GetTotalRewardValue()` - Calculates total reward value

**Usage Example**:
```csharp
var challenge = new ChallengeDefinition
{
    Id = "daily_kills_10",
    Name = "Daily Killer",
    Description = "Get 10 kills today",
    ChallengeType = ChallengeType.Daily,
    Category = ChallengeCategory.Combat,
    RequirementType = ChallengeRequirementType.KillCount,
    RequirementTarget = 10,
    XPReward = 200,
    CurrencyReward = 100,
    Difficulty = ChallengeDifficulty.Normal
};
```

### ChallengeInstance.cs

**Purpose**: Tracks per-player challenge progress with time-based expiration and reward claiming.

**Key Features**:
- Progress tracking with expiration timestamps
- Reward claiming state management
- Time remaining calculations
- Custom progress data support
- Reset functionality with new expiration times

**Key Methods**:
- `UpdateProgress(amount, target)` - Updates progress if not expired
- `SetProgress(progress, target)` - Sets absolute progress value
- `ClaimRewards()` - Marks rewards as claimed
- `IsExpired()` - Checks if challenge has expired
- `GetTimeRemaining()` - Returns time until expiration

**Usage Example**:
```csharp
var instance = new ChallengeInstance("daily_kills_10", DateTime.UtcNow.AddDays(1));
var completed = instance.UpdateProgress(1, 10);
if (completed)
{
    var claimed = instance.ClaimRewards();
    // Award rewards to player
}
```

### ChallengeSystem.cs

**Purpose**: Manages daily and weekly challenge generation, rotation, and completion tracking.

**Key Features**:
- Automatic daily/weekly challenge rotation
- Weighted random challenge selection
- Expiration handling and cleanup
- Challenge completion publishing
- Progress tracking for active challenges

**Rotation Logic**:
- Daily challenges reset at midnight UTC
- Weekly challenges reset on Monday midnight UTC
- Difficulty-weighted selection (easier challenges more likely)
- 3 daily challenges, 5 weekly challenges

**Event Subscriptions**:
- Same as AchievementSystem for progress evaluation

**Key Methods**:
- `RegisterChallenge()` - Registers challenge definitions
- `GetActiveDailyChallenges()` - Gets current daily challenges
- `GetActiveWeeklyChallenges()` - Gets current weekly challenges
- `ClaimChallengeRewards()` - Claims rewards for completed challenges
- `ForceCompleteChallenge()` - Debug method to complete challenges

### MetaProgressionSystem.cs

**Purpose**: Manages long-term player progression including XP, levels, and unlock tokens.

**Key Features**:
- Exponential XP progression formula
- Level-up detection and token awarding
- XP awarding from multiple sources
- Save/load data conversion
- Progress percentage calculations

**XP Formula**:
```
XP Required = BaseXP * (Level - 1) ^ 1.5
BaseXP = 100
```

**XP Sources**:
- Kills (scaled by entity type and death type)
- Round completion (scaled by round number)
- Achievement completion (scaled by rarity)
- Challenge completion (scaled by difficulty)

**Key Methods**:
- `AwardXP(amount, source)` - Awards XP and checks level-ups
- `SpendUnlockTokens(amount)` - Spends tokens for unlocks
- `GetXPRequiredForLevel(level)` - Calculates XP needed for level
- `GetLevelProgressPercentage()` - Returns current level progress
- `ToSaveData()` / `FromSaveData()` - Serialization support

**Usage Example**:
```csharp
var system = new MetaProgressionSystem(eventBus, logger);
system.Initialize();

// Award XP from various sources
system.AwardXP(50, XPSource.Kill);
system.AwardXP(200, XPSource.Achievement);

// Check progression
var currentLevel = system.CurrentLevel;
var xpToNext = system.XPToNextLevel;
```

## Event System Documentation

### AchievementUnlockedEvent.cs

**Purpose**: Published when an achievement is completed by the player.

**Key Properties**:
- `AchievementId` - Unique achievement identifier
- `AchievementName` - Display name
- `AchievementDescription` - Detailed description
- `Category` - Achievement category
- `Rarity` - Achievement rarity tier
- `PointValue` - Points awarded
- `CompletionTime` - When achievement was completed
- `ProgressUpdates` - Number of updates made
- `TriggeringEntityId` - Entity that triggered completion (optional)

**Usage**:
```csharp
eventBus.Subscribe<AchievementUnlockedEvent>(OnAchievementUnlocked);

void OnAchievementUnlocked(AchievementUnlockedEvent evt)
{
    // Show achievement popup
    // Update UI
    // Log completion
}
```

### ChallengeCompletedEvent.cs

**Purpose**: Published when a challenge is completed by the player.

**Key Properties**:
- `ChallengeId` - Unique challenge identifier
- `ChallengeName` - Display name
- `ChallengeType` - Daily/Weekly/Special
- `Category` - Challenge category
- `Difficulty` - Challenge difficulty
- `XPReward` / `CurrencyReward` - Rewards awarded
- `CompletionTime` - When challenge was completed
- `TimeRemaining` - Time left until expiration
- `RewardsAutoClaimed` - Whether rewards were auto-claimed

**Usage**:
```csharp
eventBus.Subscribe<ChallengeCompletedEvent>(OnChallengeCompleted);

void OnChallengeCompleted(ChallengeCompletedEvent evt)
{
    // Show challenge completion notification
    // Award rewards
    // Update challenge tracker UI
}
```

### LevelUpEvent.cs

**Purpose**: Published when the player levels up in the meta progression system.

**Key Properties**:
- `PreviousLevel` - Level before level-up
- `NewLevel` - Level after level-up
- `LevelsGained` - Number of levels gained (multi-level ups)
- `TotalXP` - Total XP after level-up
- `XPRequiredForNewLevel` - XP needed for new level
- `UnlockTokensAwarded` - Tokens awarded from level-up
- `TotalUnlockTokens` - Total tokens after level-up
- `XPSource` - Source of XP that triggered level-up
- `IsMultiLevelUp` - Whether multiple levels were gained
- `UnlocksGranted` - Array of unlocks granted at this level

**Usage**:
```csharp
eventBus.Subscribe<LevelUpEvent>(OnLevelUp);

void OnLevelUp(LevelUpEvent evt)
{
    // Show level-up notification
    // Update progression bar
    // Handle unlocks
}
```

## Save/Load Integration Documentation

### SaveGameData Extensions

**New Properties**:
- `AchievementProgress` - AchievementProgressData with completed/in-progress achievements
- `ChallengeProgress` - ChallengeProgressData with active/completed challenges
- `MetaProgression` - MetaProgressionSaveData with XP, level, tokens

**AchievementProgressData**:
- `CompletedAchievements` - List of completed achievement instances
- `InProgressAchievements` - List of in-progress achievement instances
- `TotalAchievementPoints` - Total points earned
- Validation and clone methods

**ChallengeProgressData**:
- `ActiveChallenges` - Currently active challenge instances
- `CompletedChallenges` - Completed challenge instances
- `TotalChallengesCompleted` - Lifetime completion count
- `LastDailyReset` / `LastWeeklyReset` - Reset timestamps

**MetaProgressionSaveData**:
- `Level` - Current player level
- `CurrentXP` - Current XP amount
- `TotalXPEarned` - Lifetime XP earned
- `UnlockTokens` - Available unlock tokens
- `LastSavedTime` - Serialization timestamp

### SaveManager Extensions

**New Collection Methods**:
- `CollectAchievementProgressData()` - Gathers achievement progress from AchievementSystem
- `CollectChallengeProgressData()` - Gathers challenge progress from ChallengeSystem
- `CollectMetaProgressionData()` - Gathers meta progression from MetaProgressionSystem

**Integration Points**:
- All new data types are included in save validation
- Summary includes achievement count and player level
- Version-safe serialization with backward compatibility

### LoadSystem Extensions

**New Load Methods**:
- `LoadAchievementProgress()` - Restores achievement progress to AchievementSystem
- `LoadChallengeProgress()` - Restores challenge progress to ChallengeSystem
- `LoadMetaProgression()` - Restores meta progression to MetaProgressionSystem

**Load Phases**:
- Phase 4: Load achievement progress (60% complete)
- Phase 5: Load challenge progress (70% complete)
- Phase 6: Load meta progression (80% complete)
- Phase 7: Load persistent entities (90% complete)

**Validation**:
- All new data types are validated before loading
- Partial restoration support for corrupted data
- Audit logging for all load operations

## UI Integration Documentation

### UISystem Extensions

**New UI Subsystems**:
- `AchievementPopupRenderer` - Shows achievement unlock notifications
- `ChallengeTrackerRenderer` - Displays active challenge progress
- `MetaProgressionRenderer` - Shows XP/level progress bar
- `AchievementListRenderer` - Full achievement list screen
- `ChallengeListRenderer` - Full challenge list screen

**Render Order**:
1. Health bars (background layer)
2. Score display, Kill feed, Resource display
3. Inventory panel, Challenge tracker, Meta progression bar
4. Achievement list, Challenge list
5. Game over screen (modal layer)
6. Achievement popups (popup layer - above all)
7. Tooltips (tooltip layer - highest)

**UIElementBase Usage**:
All new UI components extend UIElementBase for:
- Consistent positioning and scaling
- Visibility management
- Event handling
- Render state management

**Event Integration**:
- AchievementPopupRenderer subscribes to AchievementUnlockedEvent
- ChallengeTrackerRenderer subscribes to ChallengeCompletedEvent
- MetaProgressionRenderer subscribes to LevelUpEvent
- Real-time UI updates without polling

## Progression Formulas

### Achievement Point Values

| Rarity | Points | XP Reward |
|---------|--------|-----------|
| Common | 10 | 50 |
| Uncommon | 25 | 100 |
| Rare | 50 | 200 |
| Epic | 100 | 500 |
| Legendary | 200 | 1000 |

### Challenge Reward Values

| Difficulty | XP Reward | Currency Reward |
|-----------|------------|----------------|
| Easy | 100 | 50 |
| Normal | 200 | 100 |
| Hard | 400 | 200 |
| Extreme | 800 | 400 |

### XP Calculation Examples

**Kill XP**:
- Base zombie: 10 XP
- Boss zombie: 100 XP
- Special zombie: 25 XP
- Elite zombie: 50 XP
- Headshot multiplier: 2.0x
- Explosion multiplier: 1.5x
- Burn multiplier: 1.2x

**Round XP**:
- Formula: 50 * 1.1^(round-1)
- Round 1: 50 XP
- Round 5: 80 XP
- Round 10: 129 XP

**Level Requirements**:
- Level 1: 0 XP
- Level 2: 100 XP
- Level 5: 665 XP
- Level 10: 2,847 XP
- Level 20: 11,357 XP

## Best Practices

### Achievement Design
- Use clear, achievable requirements
- Provide varied difficulty across categories
- Include both short-term and long-term goals
- Reward progression, not just completion
- Use descriptive names and helpful descriptions

### Challenge Design
- Daily challenges should be completable in 1-2 hours
- Weekly challenges should be completable in 5-10 hours
- Include variety across all game systems
- Balance rewards with difficulty
- Reset times should be predictable (daily/weekly)

### Meta Progression Design
- XP gains should feel meaningful but not overwhelming
- Level-ups should provide tangible benefits
- Unlock tokens should be valuable but not required
- Progress should be visible and understandable

### Save/Load Design
- Always validate data before saving/loading
- Provide graceful degradation for missing data
- Use versioning for forward compatibility
- Include audit trails for debugging

### UI Design
- Use consistent visual language
- Provide clear feedback for all actions
- Layer UI elements logically
- Ensure accessibility and readability
- Support both mouse and gamepad navigation

## Testing Considerations

### Unit Testing
- Test all achievement requirement types
- Verify challenge rotation logic
- Test XP calculation edge cases
- Validate save/load data integrity

### Integration Testing
- Test event flow between systems
- Verify UI updates in real-time
- Test save/load with progression data
- Validate challenge expiration handling

### Performance Testing
- Monitor achievement evaluation performance
- Test challenge rotation overhead
- Profile save/load with large datasets
- Optimize UI rendering for many elements

## Troubleshooting

### Common Issues

**Achievements Not Unlocking**:
- Check event subscriptions in AchievementSystem
- Verify achievement definition IsActive flag
- Confirm requirement target values are correct
- Check entity type filtering criteria

**Challenges Not Rotating**:
- Verify system time and timezone handling
- Check challenge pool population
- Confirm reset time calculations
- Validate challenge definition IsActive flags

**Save/Load Issues**:
- Check data validation methods
- Verify serialization attributes
- Confirm version compatibility
- Check for null reference exceptions

**UI Not Updating**:
- Verify event subscriptions in UI renderers
- Check UIElementBase inheritance
- Confirm render order and layering
- Validate visibility state management

### Debug Tools

**AchievementSystem**:
- `ForceCompleteAchievement()` for testing
- Achievement count logging
- Progress percentage validation

**ChallengeSystem**:
- `ForceCompleteChallenge()` for testing
- Challenge rotation logging
- Expiration time debugging

**MetaProgressionSystem**:
- XP award logging by source
- Level-up validation
- Progress percentage verification

## Conclusion

The P11-04-13 implementation provides a comprehensive achievement, challenge, and meta progression system with:

- **Full Event-Driven Architecture**: All systems communicate through events
- **Complete Save/Load Support**: All progression data persists correctly
- **Rich UI Integration**: Multiple UI components for different aspects
- **Scalable Design**: Easy to add new achievements and challenges
- **Audit-Friendly Logging**: Comprehensive logging for debugging
- **ECS-Friendly Design**: Compatible with existing entity system
- **XML Documentation**: Complete API documentation for all systems

The systems are ready for integration with existing game systems and provide a solid foundation for long-term player engagement and progression.
