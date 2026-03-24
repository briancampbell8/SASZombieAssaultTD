# P11-04-10 System Documentation

## Overview

P11-04-10 implements comprehensive game lifecycle management systems including respawn logic, round progression, and game over handling. This document provides detailed documentation of all new and modified systems, components, and events.

## Table of Contents

1. [New Systems](#new-systems)
2. [New Events](#new-events)
3. [Modified Components](#modified-components)
4. [Modified Systems](#modified-systems)
5. [Architecture and Integration](#architecture-and-integration)
6. [Usage Examples](#usage-examples)

---

## New Systems

### RespawnSystem.cs

**Location:** `Engine/Systems/Gameplay/RespawnSystem.cs`

**Purpose:** Handles entity respawn logic and timing for player entities.

#### Key Features
- Subscribes to `EntityDiedEvent` for automatic respawn detection
- Configurable respawn delay and health percentage
- Manual respawn request support
- Component reset functionality (Health, Transform, Stats)
- Auto-respawn enable/disable functionality

#### Public API
```csharp
public class RespawnSystem
{
    public float DefaultRespawnDelay { get; set; } = 5.0f;
    public float DefaultRespawnHealthPercentage { get; set; } = 1.0f;
    public bool AutoRespawnEnabled { get; set; } = true;
    
    public void Update(float deltaTime);
    public bool RequestRespawn(object entityId, PointF? respawnLocation = null, 
                             float delay = -1, float healthPercentage = -1);
}
```

#### Event Subscriptions
- **EntityDiedEvent:** Detects player deaths and queues auto-respawn

#### Event Publications
- **EntityRespawnedEvent:** Published when respawn completes

#### Component Integration
- **HealthComponent:** Calls `ResetHealth()` with specified percentage
- **TransformComponent:** Calls `ResetPosition()` with respawn location
- **StatsComponent:** Calls `ResetForRespawn()` for stat reset

#### Audit Logging
All respawn operations are logged with timestamps and entity IDs for audit purposes.

---

### RoundResetSystem.cs

**Location:** `Engine/Systems/Gameplay/RoundResetSystem.cs`

**Purpose:** Manages round completion detection and wave progression.

#### Key Features
- Enemy death tracking and round completion detection
- Configurable round transition delays
- Automatic next round spawning
- Player resource and stat reset options
- Round statistics collection

#### Public API
```csharp
public class RoundResetSystem
{
    public int CurrentRound { get; private set; }
    public bool IsTransitioning { get; private set; }
    public float RoundTransitionDelay { get; set; } = 10.0f;
    public bool AutoStartNextRound { get; set; } = true;
    public bool ResetPlayerResources { get; set; } = false;
    public bool ResetPlayerStats { get; set; } = false;
    
    public void Update(float deltaTime);
    public void StartRound(int roundNumber, int enemyCount, float difficultyMultiplier = 1.0f);
    public void CompleteRound(string completionReason = "Manual completion");
}
```

#### Event Subscriptions
- **EntityDiedEvent:** Tracks enemy and player deaths for round statistics

#### Event Publications
- **RoundCompletedEvent:** Published when round completes with full statistics

#### Round Progression Logic
1. Detects when all enemies in current wave are defeated
2. Calculates round statistics (score, resources, duration)
3. Publishes `RoundCompletedEvent`
4. Applies player resource/stat resets if configured
5. Schedules next round with increased difficulty

#### Audit Logging
All round transitions and statistics are logged for comprehensive game audit trails.

---

### GameOverSystem.cs

**Location:** `Engine/Systems/Gameplay/GameOverSystem.cs`

**Purpose:** Detects and handles game over conditions with state transitions.

#### Key Features
- Player death detection and game over triggering
- Configurable death limits and respawn policies
- Gameplay system freezing (UI remains active)
- Multiple game over types (death, victory, resource depletion, etc.)
- Final statistics collection

#### Public API
```csharp
public class GameOverSystem
{
    public bool IsGameOver { get; private set; }
    public bool WasVictory { get; private set; }
    public string GameOverReason { get; private set; }
    public bool AutoGameOverOnPlayerDeath { get; set; } = true;
    public bool AllowRespawnsAfterGameOver { get; set; } = false;
    public int MaxPlayerDeaths { get; set; } = -1;
    
    public void Update(float deltaTime);
    public void TriggerGameOver(string reason, GameOverType gameOverType, 
                             bool wasVictory = false, object? triggeringEntityId = null);
    public void ResetGameOver();
}
```

#### Event Subscriptions
- **EntityDiedEvent:** Detects player and critical entity deaths

#### Event Publications
- **GameOverEvent:** Published with comprehensive game statistics

#### Game Over Conditions
- **Player Death:** Configurable death limits
- **Base Destruction:** Critical entity protection
- **Resource Depletion:** Economic failure conditions
- **Time Limits:** Time-based game over
- **Victory:** Success condition achievement

#### System Freezing
When game over occurs:
- Gameplay systems are frozen (AI, physics, combat)
- UI system remains active for game over screen
- All further game logic processing is suspended

#### Audit Logging
Complete game over sequence is logged with timestamps and triggering conditions.

---

## New Events

### EntityRespawnedEvent.cs

**Location:** `Engine/Systems/Events/EntityRespawnedEvent.cs`

**Purpose:** Event published when an entity respawns in the game world.

#### Properties
```csharp
public class EntityRespawnedEvent
{
    public object EntityId { get; set; }
    public DateTime RespawnTimestamp { get; set; }
    public object RespawnLocation { get; set; }
    public string RespawnReason { get; set; }
    public float RespawnDelay { get; set; }
    public float RespawnHealthPercentage { get; set; } = 1.0f;
    public bool WasAutoRespawn { get; set; }
    public string? AdditionalContext { get; set; }
}
```

#### Usage Example
```csharp
var respawnEvent = new EntityRespawnedEvent
{
    EntityId = playerId,
    RespawnTimestamp = DateTime.UtcNow,
    RespawnLocation = new PointF(100, 200),
    RespawnReason = "Player death",
    RespawnDelay = 5.0f,
    RespawnHealthPercentage = 1.0f,
    WasAutoRespawn = true
};

eventBus.Publish(respawnEvent);
```

---

### RoundCompletedEvent.cs

**Location:** `Engine/Systems/Events/RoundCompletedEvent.cs`

**Purpose:** Event published when a game round is completed.

#### Properties
```csharp
public class RoundCompletedEvent
{
    public int RoundId { get; set; }
    public DateTime CompletionTimestamp { get; set; }
    public float RoundDuration { get; set; }
    public int TotalEnemiesSpawned { get; set; }
    public int TotalEnemiesKilled { get; set; }
    public long RoundScore { get; set; }
    public int ResourcesEarned { get; set; }
    public int PlayerDeaths { get; set; }
    public string CompletionReason { get; set; }
    public RoundInfo? NextRoundInfo { get; set; }
    public string? AdditionalContext { get; set; }
}
```

#### Supporting Classes
```csharp
public class RoundInfo
{
    public int RoundNumber { get; set; }
    public int ExpectedEnemies { get; set; }
    public float DifficultyMultiplier { get; set; } = 1.0f;
    public List<string> Modifiers { get; set; } = new List<string>();
    public float EstimatedDuration { get; set; }
}
```

---

### GameOverEvent.cs

**Location:** `Engine/Systems/Events/GameOverEvent.cs`

**Purpose:** Event published when the game ends.

#### Properties
```csharp
public class GameOverEvent
{
    public DateTime GameOverTimestamp { get; set; }
    public string GameOverReason { get; set; }
    public GameOverType GameOverType { get; set; }
    public long FinalScore { get; set; }
    public int FinalRound { get; set; }
    public float TotalPlayTime { get; set; }
    public int TotalEnemiesKilled { get; set; }
    public int TotalPlayerDeaths { get; set; }
    public int FinalResources { get; set; }
    public bool WasVictory { get; set; }
    public object? TriggeringEntityId { get; set; }
    public string? AdditionalContext { get; set; }
}
```

#### Game Over Types
```csharp
public enum GameOverType
{
    PlayerDeath,
    BaseDestroyed,
    TimeLimitExceeded,
    ResourceDepletion,
    Victory,
    ManualQuit,
    Unknown
}
```

---

## Modified Components

### HealthComponent.cs

**Location:** `Engine/Components/HealthComponent.cs`

**New Properties**
```csharp
public bool AutoHealOnRespawn { get; set; } = false;
public float AutoHealAmount { get; set; } = 1.0f;
```

**New Methods**
```csharp
/// <summary>
/// Resets health to the specified percentage of maximum health.
/// P11-04-10-F: Supports respawn health reset with optional auto-heal.
/// </summary>
/// <param name="healthPercentage">Health percentage to reset to (0.0 to 1.0).</param>
/// <returns>True if health was reset, false if parameters are invalid.</returns>
public bool ResetHealth(float healthPercentage = 1.0f)
```

#### Auto-Heal Logic
When `AutoHealOnRespawn` is enabled and `ResetHealth()` is called with less than 100% health:
- Calculates heal amount: `MaxHealth * AutoHealAmount`
- Applies heal: `CurrentHealth = Min(MaxHealth, CurrentHealth + HealAmount)`
- Ensures health never exceeds maximum

#### Integration Points
- **RespawnSystem:** Calls `ResetHealth()` during respawn process
- **Game Over Detection:** Health state affects game over conditions

---

### TransformComponent.cs

**Location:** `Engine/Components/TransformComponent.cs`

**New Properties**
```csharp
/// <summary>
/// Gets or sets the respawn position for the entity.
/// P11-04-10-G: Used by RespawnSystem to determine where to respawn entities.
/// </summary>
public PointF RespawnPosition { get; set; } = PointF.Empty;
```

**New Methods**
```csharp
/// <summary>
/// Resets the entity position to the specified location.
/// P11-04-10-G: Used by RespawnSystem to reposition entities during respawn.
/// </summary>
/// <param name="newPosition">The new position to set</param>
public void ResetPosition(PointF newPosition)

/// <summary>
/// Sets the respawn position for the entity.
/// P11-04-10-G: Stores the location where entity should respawn.
/// </summary>
/// <param name="respawnPosition">The respawn position to set</param>
public void SetRespawnPosition(PointF respawnPosition)
```

#### Respawn Logic
- `RespawnPosition` defaults to `PointF.Empty` if not set
- `ResetPosition()` immediately updates the entity's current position
- `SetRespawnPosition()` stores the location for future respawns

#### Integration Points
- **RespawnSystem:** Uses `RespawnPosition` for respawn location, calls `ResetPosition()`
- **Game Initialization:** Systems can set initial respawn positions

---

## Modified Systems

### UISystem.cs

**Location:** `Engine/Systems/UISystem.cs`

**New Subsystems**
```csharp
// P11-04-10-H: Game lifecycle UI subsystems
private readonly GameOverScreenRenderer _gameOverScreenRenderer;
private readonly RoundCompleteNotificationRenderer _roundCompleteRenderer;
private readonly RespawnCountdownRenderer _respawnCountdownRenderer;
```

**New Public Methods**
```csharp
// Game Over Screen
public void ShowGameOverScreen(bool wasVictory, long finalScore, int finalRound, float totalPlayTime);
public void HideGameOverScreen();

// Round Complete Notifications
public void ShowRoundCompleteNotification(int roundNumber, long roundScore, float nextRoundDelay);
public void HideRoundCompleteNotification();

// Respawn Countdown
public void ShowRespawnCountdown(object entityId, float respawnTime);
public void HideRespawnCountdown(object entityId);
public void UpdateRespawnCountdown(object entityId, float remainingTime);
```

#### UI Layering Order
1. Health bars (background layer)
2. Score display
3. Kill feed
4. Round complete notifications
5. Respawn countdown
6. Game over screen (foreground layer)

#### Integration Points
- **GameOverSystem:** Calls `ShowGameOverScreen()` on game over
- **RoundResetSystem:** Calls `ShowRoundCompleteNotification()` on round completion
- **RespawnSystem:** Calls respawn countdown methods during respawn process

#### Statistics Enhancement
```csharp
public class UISystemStatistics
{
    // Existing properties...
    
    // P11-04-10-H: Game lifecycle UI subsystem statistics
    public GameOverScreenStatistics GameOverScreenStats { get; set; }
    public RoundCompleteStatistics RoundCompleteStats { get; set; }
    public RespawnCountdownStatistics RespawnCountdownStats { get; set; }
}
```

---

## Architecture and Integration

### System Dependencies

```
RespawnSystem
├── EntityManager (component access)
├── EventBus (event subscription/publication)
└── Components: HealthComponent, TransformComponent, StatsComponent

RoundResetSystem
├── EntityManager (component access)
├── EventBus (event subscription/publication)
└── Components: PlayerComponent, EnemyComponent, StatsComponent

GameOverSystem
├── EntityManager (component access)
├── EventBus (event subscription/publication)
└── Components: PlayerComponent, BaseComponent

UISystem
├── EntityManager (UIComponent access)
├── AssetManager (UI assets)
├── EventBus (system communication)
└── TextRenderer (text rendering)
```

### Event Flow

```
EntityDiedEvent
├── RespawnSystem → EntityRespawnedEvent
├── RoundResetSystem → RoundCompletedEvent
└── GameOverSystem → GameOverEvent

RoundCompletedEvent
└── UISystem → ShowRoundCompleteNotification()

EntityRespawnedEvent
└── UISystem → HideRespawnCountdown()

GameOverEvent
└── UISystem → ShowGameOverScreen()
```

### Component Reset Sequence

During respawn:
1. **HealthComponent.ResetHealth()** - Sets health to specified percentage
2. **TransformComponent.ResetPosition()** - Moves entity to respawn location
3. **StatsComponent.ResetForRespawn()** - Clears respawn-specific stats

### System Initialization Order

1. **Core Systems** (EntityManager, EventBus, AssetManager)
2. **Component Systems** (Health, Transform, Stats management)
3. **Gameplay Systems** (RespawnSystem, RoundResetSystem, GameOverSystem)
4. **UI System** (UISystem with all subsystems)

---

## Usage Examples

### Basic Respawn Setup

```csharp
// Initialize respawn system
var respawnSystem = new RespawnSystem(entityManager, eventBus);
respawnSystem.DefaultRespawnDelay = 3.0f;
respawnSystem.DefaultRespawnHealthPercentage = 0.75f;
respawnSystem.AutoRespawnEnabled = true;

// Configure entity for respawn
var healthComp = entityManager.GetComponent<HealthComponent>(playerId);
healthComp.AutoHealOnRespawn = true;
healthComp.AutoHealAmount = 0.25f;

var transformComp = entityManager.GetComponent<TransformComponent>(playerId);
transformComp.SetRespawnPosition(new PointF(100, 100));
```

### Round Management

```csharp
// Initialize round system
var roundSystem = new RoundResetSystem(entityManager, eventBus);
roundSystem.AutoStartNextRound = true;
roundSystem.RoundTransitionDelay = 5.0f;

// Start first round
roundSystem.StartRound(1, 10, 1.0f);

// Manual round completion
roundSystem.CompleteRound("Objectives completed");
```

### Game Over Configuration

```csharp
// Initialize game over system
var gameOverSystem = new GameOverSystem(entityManager, eventBus);
gameOverSystem.AutoGameOverOnPlayerDeath = true;
gameOverSystem.MaxPlayerDeaths = 3;
gameOverSystem.AllowRespawnsAfterGameOver = false;

// Manual game over trigger
gameOverSystem.TriggerGameOver("Time limit exceeded", GameOverType.TimeLimitExceeded);
```

### UI Integration

```csharp
// Initialize UI system
var uiSystem = new UISystem(entityManager, assetManager, eventBus);

// Show game over screen
uiSystem.ShowGameOverScreen(false, 15000, 12, 3600.0f);

// Show round complete
uiSystem.ShowRoundCompleteNotification(5, 2500, 10.0f);

// Show respawn countdown
uiSystem.ShowRespawnCountdown(playerId, 5.0f);
```

---

## Audit and Compliance

### Logging Standards
All systems implement comprehensive audit logging with:
- Timestamps for all operations
- Entity IDs for tracking
- Operation context and reasons
- Error handling and exception logging

### Event Serialization
All events are marked with `[Serializable]` attribute for:
- Persistent storage capabilities
- Network synchronization support
- Debug and analysis tools

### Component Validation
All component methods include:
- Parameter validation
- Return value success indicators
- Exception handling
- XML documentation

---

## Performance Considerations

### Memory Management
- Event objects use value types where possible
- Component references are cached in systems
- UI subsystems implement object pooling for frequent updates

### Update Optimization
- Systems only process relevant entity types
- UI updates are batched by layer
- Event subscriptions are filtered by entity type

### Scalability
- Configurable entity limits per system
- Dynamic UI element management
- Event-driven architecture minimizes polling

---

## Future Enhancements

### Planned Features
- Network multiplayer respawn synchronization
- Advanced round progression algorithms
- Customizable game over conditions
- UI animation and transition effects

### Extension Points
- Custom respawn logic plugins
- Round modifier system
- Game over condition framework
- UI theme and skinning system

---

**Document Version:** 1.0  
**Last Updated:** 2025-02-14  
**P11-04-10 Implementation:** Complete
