# P11-04-08 System Documentation

## Overview

This document provides comprehensive documentation for all systems and components created under P11-04-08 requirements. These systems handle scoring, statistics tracking, and kill attribution for entity deaths in SAS Zombie Assault TD game.

---

## ScoreSystem.cs

### Location
`Engine/Systems/Gameplay/ScoreSystem.cs`

### Purpose
Manages game scoring by awarding points to entities based on kills and death types. Supports score multipliers and per-entity score tracking.

### Subscribed Events
- **EntityDiedEvent**: Primary trigger for score awarding
  - Receives death information including killer and victim IDs
  - Calculates and awards score based on death type and entity characteristics
  - Tracks total score and per-entity scores

### Actions Performed
- **Score Calculation**: Computes score based on base value, death type multipliers, and entity type multipliers
- **Score Awarding**: Awards calculated score to killer entity
- **Score Tracking**: Maintains total score and per-entity score dictionary
- **Multiplier Application**: Applies different multipliers for various scenarios

### Interactions with Other Systems
- **EntityDiedEvent**: Receives death events from game systems
- **EntityManager**: Accesses components for entity type detection
- **EventBus**: Subscribes to death events and could publish score events
- **Future Integration**: Works with KillAttributedEvent for enhanced scoring

### Architectural Separation of Concerns
- **Scoring Logic**: Pure score calculation and awarding
- **Multiplier System**: Configurable multipliers for different scenarios
- **Entity Detection**: Heuristic-based entity type identification
- **Audit Logging**: Comprehensive logging for debugging and monitoring
- **State Management**: Score tracking and statistics

### Key Methods

#### `Initialize()`
```csharp
public void Initialize()
```
- Subscribes to EntityDiedEvent
- Initializes score multipliers
- Sets up system for operation

#### `OnEntityDied(EntityDiedEvent deathEvent)`
```csharp
private void OnEntityDied(EntityDiedEvent deathEvent)
```
- Event handler for EntityDiedEvent
- Validates death event and killer presence
- Calculates and awards score to killer entity

#### `CalculateScore(EntityDiedEvent deathEvent)`
```csharp
private int CalculateScore(EntityDiedEvent deathEvent)
```
- Calculates base score using entity ID heuristics
- Applies death type multipliers
- Applies entity type multipliers
- Returns final rounded score value

#### `GetBaseScoreForEntity(object entityId)`
```csharp
private int GetBaseScoreForEntity(object entityId)
```
- Uses entity ID patterns to determine base score:
  - Enemy/Zombie: 100 points
  - Boss/Elite: 500 points
  - Player: -50 points (friendly fire penalty)
  - Default: 50 points

#### `GetDeathTypeMultiplier(DeathType deathType)`
```csharp
private float GetDeathTypeMultiplier(DeathType deathType)
```
- Returns multiplier based on death type:
  - PlayerKill: 1.0x
  - EnemyKill: 0.8x
  - Environmental: 0.5x
  - HealthDepletion: 0.3x
  - Scripted: 1.0x
  - Suicide: 0.0x
  - Unknown: 0.5x

### Score Multipliers

#### Death Type Multipliers
- **PlayerKill**: 1.0x (normal scoring)
- **EnemyKill**: 0.8x (reduced for enemy kills)
- **Environmental**: 0.5x (half for environmental kills)
- **HealthDepletion**: 0.3x (reduced for passive deaths)
- **Scripted**: 1.0x (full score for scripted events)
- **Suicide**: 0.0x (no score for self-inflicted deaths)
- **Unknown**: 0.5x (default for unknown deaths)

#### Entity Type Multipliers
- **Boss**: 2.0x (double score for bosses)
- **Elite**: 1.5x (50% bonus for elites)
- **Player**: 0.5x (half score for friendly fire)
- **Enemy/Zombie**: 1.0x (normal multiplier)
- **Default**: 1.0x (standard scoring)

### Dependencies
- `EntityManager`: For component access and entity queries
- `EventBus`: For EntityDiedEvent subscription
- `EntityDiedEvent`: For death event handling
- `DeathType`: For multiplier calculations

---

## StatsTrackerSystem.cs

### Location
`Engine/Systems/Gameplay/StatsTrackerSystem.cs`

### Purpose
Tracks and updates comprehensive statistics for entities including kills, deaths, kill streaks, and performance metrics.

### Subscribed Events
- **EntityDiedEvent**: Primary trigger for stat updates
  - Updates victim's death statistics
  - Updates killer's kill statistics
  - Manages kill streaks and timeouts

### Actions Performed
- **Stat Updates**: Updates Kills, Deaths, CurrentKillStreak, LongestKillStreak
- **Kill By Type Tracking**: Maintains dictionary of kills by DeathType
- **Kill Streak Management**: Tracks current streaks and handles timeouts
- **Component Management**: Creates StatsComponent for entities without them
- **Stat Resetting**: Provides methods for individual and global stat resets

### Interactions with Other Systems
- **EntityDiedEvent**: Receives death events from game systems
- **EntityManager**: Accesses and creates StatsComponent instances
- **StatsComponent**: Stores and manages per-entity statistics
- **Time System**: Uses DateTime for streak timeout calculations
- **Future Integration**: Works with KillAttributedEvent for enhanced tracking

### Architectural Separation of Concerns
- **Stat Tracking**: Pure statistical data management
- **Component Management**: ECS-friendly component creation and access
- **Streak Logic**: Kill streak calculation and timeout handling
- **Query Operations**: Provides methods for stat queries and rankings
- **Audit Logging**: Comprehensive logging for debugging and monitoring

### Key Methods

#### `Initialize()`
```csharp
public void Initialize()
```
- Subscribes to EntityDiedEvent
- Sets up system for operation
- Configures kill streak timeout

#### `OnEntityDied(EntityDiedEvent deathEvent)`
```csharp
private void OnEntityDied(EntityDiedEvent deathEvent)
```
- Event handler for EntityDiedEvent
- Updates victim's death statistics
- Updates killer's kill statistics
- Handles both sides of the death event

#### `UpdateVictimStats(object victimEntityId)`
```csharp
private void UpdateVictimStats(object victimEntityId)
```
- Gets or creates StatsComponent for victim
- Calls AddDeath() method
- Resets kill streak for victim

#### `UpdateKillerStats(object killerEntityId, DeathType deathType)`
```csharp
private void UpdateKillerStats(object killerEntityId, DeathType deathType)
```
- Gets or creates StatsComponent for killer
- Calls AddKill(deathType) method
- Updates kill streak and longest streak

#### `GetOrCreateStatsComponent(object entityId)`
```csharp
private StatsComponent GetOrCreateStatsComponent(object entityId)
```
- Retrieves existing StatsComponent
- Creates new StatsComponent if none exists
- Adds component to entity via EntityManager

#### `UpdateKillStreaks()`
```csharp
public void UpdateKillStreaks()
```
- Handles kill streak timeouts
- Uses 30-second timeout threshold
- Resets streaks for inactive entities

### Query Methods

#### `GetTopKillers(int topCount = 10)`
```csharp
public object[] GetTopKillers(int topCount = 10)
```
- Returns top N entities by total kills
- Sorted in descending order
- Useful for leaderboards and rankings

#### `GetEntitiesWithKillStreaks(int minStreakLength = 3)`
```csharp
public object[] GetEntitiesWithKillStreaks(int minStreakLength = 3)
```
- Returns entities with active kill streaks
- Filters by minimum streak length
- Useful for gameplay notifications

### Kill Streak System
- **Timeout**: 30 seconds without kill resets streak
- **Tracking**: Automatic streak management
- **Persistence**: Longest streak preserved across timeouts
- **Notifications**: System provides query methods for UI updates

### Dependencies
- `EntityManager`: For component access and entity queries
- `EventBus`: For EntityDiedEvent subscription
- `StatsComponent`: For storing per-entity statistics
- `EntityDiedEvent`: For death event handling
- `DeathType`: For kill categorization

---

## StatsComponent.cs

### Location
`Engine/Components/StatsComponent.cs`

### Purpose
ECS-friendly pure data component for storing entity statistics including kills, deaths, and kill streaks.

### Component Properties

#### `Kills`
```csharp
public int Kills { get; set; } = 0;
```
- Total number of kills this entity has scored
- Incremented by StatsTrackerSystem on kills
- Used for scoring and performance metrics

#### `Deaths`
```csharp
public int Deaths { get; set; } = 0;
```
- Total number of times this entity has died
- Incremented by StatsTrackerSystem on deaths
- Used for K/D ratio calculations

#### `CurrentKillStreak`
```csharp
public int CurrentKillStreak { get; set; } = 0;
```
- Current consecutive kills without dying
- Reset to 0 on death
- Used for gameplay bonuses and notifications

#### `LongestKillStreak`
```csharp
public int LongestKillStreak { get; set; } = 0;
```
- Longest kill streak this entity has achieved
- Updated when current streak exceeds previous best
- Used for achievement tracking

#### `KillByType`
```csharp
public Dictionary<DeathType, int> KillByType { get; set; } = new Dictionary<DeathType, int>();
```
- Dictionary tracking kills by death type
- Initialized with all DeathType values set to 0
- Used for detailed statistics and analysis

#### `LastKillTime`
```csharp
public DateTime LastKillTime { get; set; } = DateTime.MinValue;
```
- Timestamp of last kill this entity scored
- Used for kill streak timing and timeout calculations
- Updated by AddKill method

#### `LastDeathTime`
```csharp
public DateTime LastDeathTime { get; set; } = DateTime.MinValue;
```
- Timestamp of last time this entity died
- Used for death tracking and respawn timing
- Updated by AddDeath method

### Key Methods

#### `AddKill(DeathType deathType)`
```csharp
public void AddKill(DeathType deathType)
```
- Increments total kill count
- Updates current kill streak
- Updates longest kill streak if needed
- Increments KillByType dictionary
- Updates LastKillTime timestamp

#### `AddDeath()`
```csharp
public void AddDeath()
```
- Increments total death count
- Resets current kill streak to 0
- Updates LastDeathTime timestamp

#### `ResetStats()`
```csharp
public void ResetStats()
```
- Resets all statistics to default values
- Clears KillByType dictionary
- Resets timestamps to MinValue
- Used for new game sessions

#### `GetKillDeathRatio()`
```csharp
public float GetKillDeathRatio()
```
- Calculates kill-to-death ratio
- Returns 0 if deaths is 0 to avoid division
- Returns float.MaxValue if kills > 0 but deaths is 0

#### `GetKillsByType(DeathType deathType)`
```csharp
public int GetKillsByType(DeathType deathType)
```
- Returns number of kills for specific death type
- Safe dictionary access with default 0
- Used for detailed stat queries

### ECS Design
- **Pure Data**: No logic, only data storage
- **Component-Friendly**: Simple properties for ECS queries
- **Serializable**: All properties are serializable
- **Default Values**: Sensible defaults for all properties
- **Initialization**: Automatic dictionary initialization

---

## KillAttributionSystem.cs

### Location
`Engine/Systems/Gameplay/KillAttributionSystem.cs`

### Purpose
Determines kill attribution including team relationships, friendly fire detection, and self-inflicted death identification.

### Subscribed Events
- **EntityDiedEvent**: Primary trigger for attribution analysis
  - Analyzes death events for team relationships
  - Determines killer team and victim team
  - Publishes KillAttributedEvent with attribution metadata

### Published Events
- **KillAttributedEvent**: Attribution-enhanced death event
  - Contains team information and context
  - Used by other systems for enhanced processing
  - Includes friendly fire and self-inflicted flags

### Actions Performed
- **Team Determination**: Identifies teams for killer and victim
- **Friendly Fire Detection**: Identifies same-team kills
- **Self-Inflicted Detection**: Identifies suicide deaths
- **Environmental Attribution**: Handles deaths with no killer
- **Context Creation**: Generates additional context information

### Interactions with Other Systems
- **EntityDiedEvent**: Receives death events from game systems
- **KillAttributedEvent**: Publishes enhanced attribution events
- **EntityManager**: Accesses components for entity identification
- **Team System**: Uses heuristics for team determination
- **Future Integration**: Works with TeamComponent for enhanced accuracy

### Architectural Separation of Concerns
- **Attribution Logic**: Pure team and relationship analysis
- **Event Enhancement**: Enriches death events with metadata
- **Team Detection**: Heuristic-based team identification
- **Context Generation**: Creates additional context for other systems
- **Audit Logging**: Comprehensive logging for debugging and monitoring

### Key Methods

#### `Initialize()`
```csharp
public void Initialize()
```
- Subscribes to EntityDiedEvent
- Sets up system for operation
- Configures team detection heuristics

#### `OnEntityDied(EntityDiedEvent deathEvent)`
```csharp
private void OnEntityDied(EntityDiedEvent deathEvent)
```
- Event handler for EntityDiedEvent
- Determines attribution information
- Creates and publishes KillAttributedEvent

#### `DetermineKillAttribution(EntityDiedEvent deathEvent)`
```csharp
private KillAttribution DetermineKillAttribution(EntityDiedEvent deathEvent)
```
- Analyzes death event for attribution
- Handles environmental deaths (no killer)
- Detects self-inflicted deaths
- Detects friendly fire situations

#### `DetermineEntityTeam(object entityId)`
```csharp
private Team DetermineEntityTeam(object entityId)
```
- Uses entity ID heuristics for team detection:
  - Player/Hero/Ally → Team.Player
  - Enemy/Zombie/Monster → Team.Enemy
  - Environment/Trap/Hazard → Team.Neutral
  - Default → Team.Unknown

#### `CreateKillAttributedEvent(EntityDiedEvent deathEvent, KillAttribution attribution)`
```csharp
private KillAttributedEvent CreateKillAttributedEvent(EntityDiedEvent deathEvent, KillAttribution attribution)
```
- Creates fully populated KillAttributedEvent
- Combines original death event with attribution data
- Calculates base score value for scoring system

### Team Detection Logic

#### Entity ID Patterns
- **Player Team**: Contains "player", "hero", or "ally"
- **Enemy Team**: Contains "enemy", "zombie", "monster", or "hostile"
- **Neutral Team**: Contains "environment", "trap", "hazard", or "neutral"
- **Unknown Team**: Default for unrecognized patterns

#### Attribution Rules
- **Environmental Death**: No killer entity → Team.Neutral
- **Self-Inflicted**: Killer equals victim → IsSelfInflicted = true
- **Friendly Fire**: Same team, different entities → IsFriendlyFire = true
- **Normal Kill**: Different teams → Standard attribution

### Dependencies
- `EntityManager`: For component access and entity queries
- `EventBus`: For EntityDiedEvent subscription and KillAttributedEvent publishing
- `EntityDiedEvent`: For death event handling
- `KillAttributedEvent`: For enhanced event publishing
- `Team`: For team identification and relationships

---

## KillAttributedEvent.cs

### Location
`Engine/Systems/Events/KillAttributedEvent.cs`

### Purpose
Enhanced death event with comprehensive attribution information including team relationships and context.

### Event Properties

#### `VictimEntityId`
```csharp
[Required]
public object VictimEntityId { get; set; }
```
- Unique identifier of entity that died (victim)
- Required field for event validation
- Used for stat updates and notifications

#### `KillerEntityId`
```csharp
public object? KillerEntityId { get; set; }
```
- Identifier of entity that caused the death (killer)
- Can be null for environmental deaths
- Used for score awarding and stat tracking

#### `KillerTeam`
```csharp
[Required]
public Team KillerTeam { get; set; }
```
- Team affiliation of the killer entity
- Used for friendly fire detection
- Supports team-based scoring and logic

#### `VictimTeam`
```csharp
public Team VictimTeam { get; set; }
```
- Team affiliation of the victim entity
- Used for team relationship analysis
- Supports team-based statistics

#### `IsFriendlyFire`
```csharp
public bool IsFriendlyFire { get; set; }
```
- Whether the kill was friendly fire (same team)
- True when killer and victim are on same team
- Used for scoring adjustments and penalties

#### `IsSelfInflicted`
```csharp
public bool IsSelfInflicted { get; set; }
```
- Whether the death was self-inflicted
- True when entity caused its own death
- Used for special scoring rules

#### `DeathType`
```csharp
[Required]
public DeathType DeathType { get; set; }
```
- Death type category from original event
- Used for effect selection and scoring
- Maintains compatibility with EntityDiedEvent

#### `Timestamp`
```csharp
[Required]
public DateTime Timestamp { get; set; }
```
- Timestamp when the kill was attributed
- Used for timing, streaks, and analysis
- Copied from original death event

#### `AdditionalContext`
```csharp
[StringLength(200)]
public string? AdditionalContext { get; set; }
```
- Additional context about the kill
- Examples: weapon used, distance, environmental factors
- Limited to 200 characters for storage efficiency

#### `BaseScoreValue`
```csharp
public int BaseScoreValue { get; set; }
```
- Base score value before multipliers
- Calculated by attribution system
- Used as input for ScoreSystem calculations

#### `FinalScoreValue`
```csharp
public int FinalScoreValue { get; set; }
```
- Final score after all calculations
- Populated by ScoreSystem
- Represents actual points awarded

### Team Enum

#### `Team` Values
- **Player**: Player-controlled entities
- **Enemy**: Enemy entities hostile to players
- **Neutral**: Neutral entities (environment, civilians, etc.)
- **Unknown**: Unknown or unassigned team

### Event Features
- **Serializable**: Full binary serialization support
- **Validation**: Data annotations for required fields
- **Audit-Friendly**: Comprehensive ToString() method
- **Extensible**: Additional context for future enhancements

---

## Integration and Usage

### System Initialization Order
1. **StatsComponent**: Component definition (no initialization needed)
2. **KillAttributedEvent**: Event class (no initialization needed)
3. **KillAttributionSystem**: Initialize first (creates attribution events)
4. **ScoreSystem**: Initialize for scoring (consumes attribution events)
5. **StatsTrackerSystem**: Initialize for stat tracking (consumes death events)

### Event Flow
1. Entity dies → `EntityDiedEvent` published
2. `KillAttributionSystem` receives event → Analyzes attribution
3. `KillAttributionSystem` publishes `KillAttributedEvent`
4. `ScoreSystem` receives `KillAttributedEvent` → Awards score
5. `StatsTrackerSystem` receives `EntityDiedEvent` → Updates stats

### Component Requirements
- **StatsComponent**: Required on entities for stat tracking
- **TransformComponent**: Required for position-based systems
- **HealthComponent**: Required for death detection (external system)

### Configuration
- **Score Multipliers**: Configured in `ScoreSystem.InitializeScoreMultipliers()`
- **Team Detection**: Configured in `KillAttributionSystem.DetermineEntityTeam()`
- **Kill Streak Timeout**: Configured as constant (30 seconds)

---

## Performance Considerations

### Score System
- **Efficient Calculation**: Simple arithmetic operations
- **Dictionary Access**: O(1) score lookups
- **Event-Driven**: No polling required
- **Memory Usage**: Lightweight tracking structures

### Stats Tracker System
- **Component Caching**: Efficient component access
- **Batch Operations**: Efficient stat queries
- **Timeout Management**: Periodic streak updates
- **Query Optimization**: Sorted queries for rankings

### Kill Attribution System
- **Heuristic Detection**: Fast team identification
- **Event Enhancement**: Minimal overhead for metadata
- **String Operations**: Optimized pattern matching
- **Context Generation**: Efficient string building

---

## Debugging and Monitoring

### Audit Logging
All systems include comprehensive debug logging:
- **Initialization**: System startup and configuration
- **Event Handling**: Event reception and processing
- **Stat Updates**: Component creation and modifications
- **Attribution**: Team detection and relationship analysis
- **Errors**: Graceful error handling with detailed messages

### Statistics
- **ScoreSystem**: Total score, entity count, average score
- **StatsTrackerSystem**: Tracked entities, total kills/deaths, active streaks
- **KillAttributionSystem**: Initialization and subscription status

### Error Handling
- **Graceful Degradation**: Systems continue operating despite individual failures
- **Validation**: Input parameter validation
- **Null Checking**: Comprehensive null reference handling
- **Fallback Behavior**: Default values for missing data

---

## Extension Points

### Future Enhancements

#### Team Component System
- Replace heuristic team detection with dedicated TeamComponent
- Support dynamic team changes during gameplay
- Enable complex team relationships and alliances

#### Entity Type System
- Replace entity ID heuristics with EntityTypeComponent
- Support complex entity hierarchies and inheritance
- Enable dynamic entity properties and behaviors

#### Advanced Scoring
- Combo multipliers for rapid successive kills
- Team-based scoring bonuses
- Environmental and situational modifiers
- Achievement and milestone rewards

#### Enhanced Statistics
- Performance metrics (accuracy, efficiency)
- Time-based statistics (kills per minute)
- Weapon and ability usage tracking
- Historical data and trends

---

## Conclusion

The P11-04-08 systems provide comprehensive scoring, statistics, and attribution for SAS Zombie Assault TD game:

1. **ScoreSystem**: Flexible scoring with multipliers and per-entity tracking
2. **StatsTrackerSystem**: Comprehensive stat tracking with streak management
3. **StatsComponent**: ECS-friendly pure data component
4. **KillAttributionSystem**: Team detection and attribution analysis
5. **KillAttributedEvent**: Enhanced death event with metadata

All systems follow architectural best practices:
- **Event-Driven Design**: Loose coupling through EventBus
- **Component-Based**: ECS-friendly architecture
- **Separation of Concerns**: Clear responsibilities and boundaries
- **Audit-Friendly**: Comprehensive logging and documentation
- **Performance-Optimized**: Efficient algorithms and data structures

The implementation provides a solid foundation for game scoring and statistics that can be easily extended and configured for different gameplay scenarios.
