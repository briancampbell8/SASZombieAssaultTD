# P11-04-07 System Documentation

## Overview

This document provides comprehensive documentation for all systems and components modified or created under P11-04-07 requirements. These systems handle death animations, visual effects, and audio feedback for entity deaths in the SAS Zombie Assault TD game.

---

## AnimationTriggerSystem.cs

### Location
`Engine/Systems/Gameplay/AnimationTriggerSystem.cs`

### Purpose
Triggers death animations on entities when they die, providing visual feedback for player actions and game events.

### Subscribed Events
- **EntityDiedEvent**: Primary trigger for death animations
  - Receives death information including entity ID, death type, and death reason
  - Triggers appropriate death animation based on death type

### Triggered Effects
- **Death Animation Playback**: Sets entity animation to death state
  - Maps death types to specific animation names
  - Sets animation playback mode to "Once" (non-looping)
  - Resets animation state for proper playback

### Architectural Separation of Concerns
- **Event-Driven Design**: Subscribes to EntityDiedEvent for loose coupling
- **Component Access**: Uses EntityManager to access AnimationComponent
- **Animation Control**: Only controls animation state, does not handle rendering
- **Audit Logging**: Comprehensive logging for debugging and monitoring

### Key Methods

#### `Initialize()`
```csharp
public void Initialize()
```
- Subscribes to EntityDiedEvent
- Sets up system for operation
- Includes error handling and debug logging

#### `OnEntityDied(EntityDiedEvent deathEvent)`
```csharp
private void OnEntityDied(EntityDiedEvent deathEvent)
```
- Event handler for EntityDiedEvent
- Extracts entity's AnimationComponent
- Triggers death animation based on death type

#### `TriggerDeathAnimation(AnimationComponent animationComponent, EntityDiedEvent deathEvent)`
```csharp
private void TriggerDeathAnimation(AnimationComponent animationComponent, EntityDiedEvent deathEvent)
```
- Maps death types to animation names
- Sets animation to "Death" state
- Configures playback mode for non-looping death animations

#### `GetDeathAnimationName(DeathType deathType)`
```csharp
private string GetDeathAnimationName(DeathType deathType)
```
- Returns appropriate animation name for each death type:
  - `PlayerKill` → "Death_PlayerKill"
  - `EnemyKill` → "Death_EnemyKill"
  - `Environmental` → "Death_Environmental"
  - `HealthDepletion` → "Death_HealthDepletion"
  - `Scripted` → "Death_Scripted"
  - `Suicide` → "Death_Suicide"
  - `Unknown` → "Death_Generic"

### Dependencies
- `EntityManager`: For component access
- `EventBus`: For event subscription
- `AnimationComponent`: For animation control
- `EntityDiedEvent`: For death event handling

### XML Documentation
All public methods and properties include comprehensive XML documentation with:
- Purpose descriptions
- Parameter explanations
- Return value documentation
- Usage examples where applicable

---

## DeathEffectSystem.cs

### Location
`Engine/Systems/Gameplay/DeathEffectSystem.cs`

### Purpose
Spawns particle effects and plays death sound effects when entities die, providing comprehensive audiovisual feedback.

### Subscribed Events
- **EntityDiedEvent**: Primary trigger for death effects
  - Receives death information including entity position and death type
  - Triggers particle effects and sound effects based on death type

### Triggered Effects
- **Particle Effects**: Visual effects at death location
  - Blood splatter, explosions, debris, dust clouds
  - Different effects for different death types
  - Configurable particle count, velocity, lifetime, and color

- **Sound Effects**: Audio feedback at death location
  - Spatialized playback based on death position
  - Different sounds for different death types
  - Volume attenuation based on distance

### Architectural Separation of Concerns
- **Event-Driven Design**: Subscribes to EntityDiedEvent for loose coupling
- **Effect Coordination**: Orchestrates particle and audio systems
- **Configuration-Based**: Uses effect configurations for different death types
- **Spatial Audio**: Integrates with AudioSystem for 3D positioning
- **Audit Logging**: Comprehensive logging for debugging and monitoring

### Key Methods

#### `Initialize()`
```csharp
public void Initialize()
```
- Subscribes to EntityDiedEvent
- Initializes death effect configurations
- Sets up system for operation

#### `OnEntityDied(EntityDiedEvent deathEvent)`
```csharp
private void OnEntityDied(EntityDiedEvent deathEvent)
```
- Event handler for EntityDiedEvent
- Extracts entity's position from TransformComponent
- Coordinates particle and sound effect spawning

#### `SpawnDeathEffectsAtPosition(PointF position, EntityDiedEvent deathEvent)`
```csharp
private void SpawnDeathEffectsAtPosition(PointF position, EntityDiedEvent deathEvent)
```
- Gets effect configuration for death type
- Spawns particle effects at specified position
- Plays spatialized sound effects at specified position

#### `SpawnParticleEffects(PointF position, DeathEffectConfiguration effectConfig, EntityDiedEvent deathEvent)`
```csharp
private void SpawnParticleEffects(PointF position, DeathEffectConfiguration effectConfig, EntityDiedEvent deathEvent)
```
- Iterates through configured particle effects
- Calls ParticleSystem.SpawnEffect for each effect
- Handles errors gracefully

#### `PlaySoundEffects(PointF position, DeathEffectConfiguration effectConfig, EntityDiedEvent deathEvent)`
```csharp
private void PlaySoundEffects(PointF position, DeathEffectConfiguration effectConfig, EntityDiedEvent deathEvent)
```
- Iterates through configured sound effects
- Calls AudioSystem.PlaySound with spatialized playback
- Handles errors gracefully

### Death Effect Configurations

#### PlayerKill
- **Particles**: blood_splatter, death_explosion
- **Sounds**: sfx_enemy_death_playerkill, sfx_blood_splash

#### EnemyKill
- **Particles**: small_explosion, death_puff
- **Sounds**: sfx_enemy_death_enemykill

#### Environmental
- **Particles**: environmental_debris, dust_cloud
- **Sounds**: sfx_environmental_death, sfx_debris_fall

#### HealthDepletion
- **Particles**: fade_out_particles, spirit_rise
- **Sounds**: sfx_health_depletion_death

#### Scripted
- **Particles**: scripted_death_effect
- **Sounds**: sfx_scripted_death

#### Suicide
- **Particles**: self_destruct_explosion
- **Sounds**: sfx_self_destruct

#### Unknown
- **Particles**: generic_death_particles
- **Sounds**: sfx_generic_death

### Dependencies
- `EntityManager`: For component access
- `ParticleSystem`: For particle effect spawning
- `AudioSystem`: For spatialized sound playback
- `EventBus`: For event subscription
- `TransformComponent`: For position extraction
- `EntityDiedEvent`: For death event handling

---

## AnimationComponent.cs (Updated)

### Location
`Engine/Components/AnimationComponent.cs`

### Purpose
Enhanced animation component with support for named animations, completion events, and ECS-friendly properties.

### New Properties

#### `CurrentAnimation`
```csharp
public string CurrentAnimation { get; set; } = string.Empty;
```
- **Purpose**: Identifies which animation is currently active
- **Usage**: Set by PlayAnimation method for animation tracking
- **ECS-Friendly**: Simple string property for component queries

#### `IsLooping`
```csharp
public bool IsLooping => PlaybackMode == AnimationPlaybackMode.Loop;
```
- **Purpose**: Provides ECS-friendly access to looping state
- **Implementation**: Derived from PlaybackMode for consistency
- **Usage**: Read-only property for animation state queries

#### `IsPlaying`
```csharp
public bool IsPlaying => PlaybackState == AnimationPlaybackState.Playing;
```
- **Purpose**: Provides ECS-friendly access to playing state
- **Implementation**: Derived from PlaybackState for consistency
- **Usage**: Read-only property for animation state queries

#### `OnAnimationComplete`
```csharp
public event Action<string>? OnAnimationComplete;
```
- **Purpose**: Event triggered when non-looping animations complete
- **Parameter**: Animation name that completed
- **Usage**: Subscribe for animation completion callbacks

### New Methods

#### `PlayAnimation(string animationName)`
```csharp
public void PlayAnimation(string animationName)
```
- **Purpose**: Plays a specific animation by name
- **Parameters**: 
  - `animationName`: Name of animation to play
- **Behavior**:
  - Sets CurrentAnimation property
  - Resets animation state
  - Starts playback in Playing state
  - Validates input parameters

#### `Update(float deltaTime)`
```csharp
public void Update(float deltaTime)
```
- **Purpose**: Advances animation based on elapsed time
- **Parameters**:
  - `deltaTime`: Time elapsed since last update
- **Behavior**:
  - Updates elapsed time accumulator
  - Advances frames when frame duration is exceeded
  - Handles different playback modes (Loop, Once, PingPong)
  - Fires completion events for non-looping animations

#### `AdvanceFrame()`
```csharp
private void AdvanceFrame()
```
- **Purpose**: Advances to next frame based on playback mode
- **Behavior**:
  - **Loop**: Wraps around to first frame
  - **Once**: Stops at last frame and fires completion event
  - **PingPong**: Reverses direction at boundaries

#### `FireAnimationCompleteEvent()`
```csharp
private void FireAnimationCompleteEvent()
```
- **Purpose**: Fires animation completion event safely
- **Behavior**:
  - Prevents multiple event firings
  - Passes animation name to subscribers
  - Only fires for non-looping animations

### Architectural Separation of Concerns
- **Component-Based**: ECS-friendly design with simple properties
- **Event-Driven**: Uses completion events for loose coupling
- **State Management**: Clear separation of animation states
- **Time-Based**: Frame advancement based on delta time
- **Validation**: Input validation and error handling

---

## ParticleSystem.cs (Updated)

### Location
`Engine/Systems/ParticleSystem.cs`

### Purpose
Enhanced particle system with support for named effect spawning and comprehensive particle configuration.

### New Methods

#### `SpawnEffect(string effectName, PointF position)`
```csharp
public void SpawnEffect(string effectName, PointF position)
```
- **Purpose**: Spawns a particle effect at specified position
- **Parameters**:
  - `effectName`: Name of effect to spawn
  - `position`: World position for effect origin
- **Behavior**:
  - Validates system state and parameters
  - Gets effect configuration by name
  - Spawns particles with configured properties
  - Includes comprehensive error handling

#### `GetEffectConfiguration(string effectName)`
```csharp
private ParticleEffectConfiguration GetEffectConfiguration(string effectName)
```
- **Purpose**: Returns configuration for specified effect name
- **Supported Effects**:
  - `blood_splatter`: Red particles with downward velocity
  - `death_explosion`: Orange/yellow explosion particles
  - `small_explosion`: White/gray small explosion
  - `death_puff`: Gray dust puff effect
  - `environmental_debris`: Brown debris particles
  - `dust_cloud`: Gray dust cloud effect
  - `fade_out_particles`: White to blue fade effect
  - `spirit_rise`: Blue rising spirit effect
  - `scripted_death_effect`: Purple scripted death effect
  - `self_destruct_explosion`: Large orange explosion
  - `generic_death_particles`: Gray generic death effect

#### `SpawnEffectEmitter(ParticleEffectConfiguration effectConfig, PointF position, string effectName)`
```csharp
private void SpawnEffectEmitter(ParticleEffectConfiguration effectConfig, PointF position, string effectName)
```
- **Purpose**: Creates temporary emitter for effect spawning
- **Behavior**:
  - Creates emitter component with effect configuration
  - Creates transform component at specified position
  - Directly spawns particles (temporary implementation)

#### `SpawnParticleFromEffect(ParticleEmitterComponent emitter, TransformComponent transform)`
```csharp
private void SpawnParticleFromEffect(ParticleEmitterComponent emitter, TransformComponent transform)
```
- **Purpose**: Spawns single particle from effect configuration
- **Behavior**:
  - Gets particle from pool
  - Sets properties from configuration ranges
  - Adds particle to active list

### New Supporting Classes

#### `ParticleEffectConfiguration`
```csharp
public class ParticleEffectConfiguration
```
- **Purpose**: Configuration for particle effects
- **Properties**:
  - `ParticleCount`: Number of particles to emit
  - `EmissionRate`: Particles per second
  - `InitialVelocityRange`: Random velocity range
  - `LifetimeRange`: Random lifetime range
  - `ColorTintRange`: Random color range
  - `InitialScaleRange`: Random scale range

#### `Vector2Range`
```csharp
public class Vector2Range
```
- **Purpose**: Range for Vector2 values with random generation
- **Methods**:
  - `GetRandom()`: Returns random Vector2 within range

#### `FloatRange`
```csharp
public class FloatRange
```
- **Purpose**: Range for float values with random generation
- **Methods**:
  - `GetRandom()`: Returns random float within range

#### `ColorRange`
```csharp
public class ColorRange
```
- **Purpose**: Range for Color values with random generation
- **Methods**:
  - `GetRandom()`: Returns random Color within range

### Architectural Separation of Concerns
- **Effect-Based**: Named effects for easy usage
- **Configuration-Driven**: Effect properties defined in configurations
- **Randomization**: Built-in random value generation
- **Performance**: Particle pooling for efficiency
- **Extensibility**: Easy to add new effect types

---

## AudioSystem.cs (Updated)

### Location
`Engine/Systems/Audio/AudioSystem.cs`

### Purpose
Enhanced audio system with support for spatialized sound playback and 3D audio positioning.

### New Methods

#### `PlaySound(string soundId, PointF position, string category = "SFX", bool loop = false, float maxDistance = 1000.0f, float referenceDistance = 100.0f)`
```csharp
public void PlaySound(string soundId, PointF position, string category = "SFX", bool loop = false, float maxDistance = 1000.0f, float referenceDistance = 100.0f)
```
- **Purpose**: Plays sound effect with spatialized playback at specified position
- **Parameters**:
  - `soundId`: Identifier of sound to play
  - `position`: World position where sound should originate
  - `category`: Audio category for volume control (default: "SFX")
  - `loop`: Whether sound should loop (default: false)
  - `maxDistance`: Maximum hearing distance (default: 1000.0f)
  - `referenceDistance`: Reference distance for attenuation (default: 100.0f)
- **Behavior**:
  - Calculates spatialized volume based on distance
  - Applies volume attenuation for realistic 3D audio
  - Skips playback if out of hearing range
  - Publishes AudioPlayEvent for tracking

#### `CalculateSpatializedVolume(PointF soundPosition, float maxDistance, float referenceDistance)`
```csharp
private float CalculateSpatializedVolume(PointF soundPosition, float maxDistance, float referenceDistance)
```
- **Purpose**: Calculates volume based on distance from listener
- **Algorithm**: Inverse distance attenuation model
- **Behavior**:
  - Assumes listener at origin (0, 0) for current implementation
  - Returns 0.0 if beyond maximum distance
  - Applies reference distance for volume calculation
  - Clamps result to valid range (0.0 to 1.0)

#### `CalculateDistance(PointF point1, PointF point2)`
```csharp
private float CalculateDistance(PointF point1, PointF point2)
```
- **Purpose**: Calculates Euclidean distance between two points
- **Usage**: Helper method for spatialized audio calculations
- **Implementation**: Standard distance formula

### Spatial Audio Features

#### Distance Attenuation
- **Model**: Inverse distance attenuation
- **Formula**: `referenceDistance / (referenceDistance + distance)`
- **Behavior**: Volume decreases as distance increases

#### Maximum Hearing Distance
- **Purpose**: Performance optimization and realism
- **Default**: 1000.0 units
- **Behavior**: Sounds beyond this distance are not played

#### Reference Distance
- **Purpose**: Reference point for volume calculation
- **Default**: 100.0 units
- **Behavior**: Volume at this distance serves as reference point

### Future Enhancements (Documented)
- **Listener Position**: Dynamic listener position (camera/player)
- **Stereo Panning**: Left/right audio balance based on position
- **Occlusion**: Environmental obstacles affecting sound
- **Environmental Effects**: Reverb, echo, and other effects
- **Doppler Effect**: Pitch changes based on relative motion

### Architectural Separation of Concerns
- **Spatial Audio**: 3D positioning and distance calculation
- **Volume Control**: Integration with existing volume system
- **Event Publishing**: Maintains AudioPlayEvent publishing
- **Performance**: Distance culling for optimization
- **Extensibility**: Configurable parameters for different scenarios

---

## Integration and Usage

### System Initialization Order
1. **AudioSystem**: Initialize first for audio subsystem
2. **ParticleSystem**: Initialize for particle effects
3. **AnimationTriggerSystem**: Initialize for death animations
4. **DeathEffectSystem**: Initialize last (depends on others)

### Event Flow
1. Entity dies → `EntityDiedEvent` published
2. `AnimationTriggerSystem` receives event → Triggers death animation
3. `DeathEffectSystem` receives event → Spawns particles and sounds
4. Systems coordinate to provide comprehensive death feedback

### Component Requirements
- **AnimationComponent**: Required on entities for death animations
- **TransformComponent**: Required on entities for position-based effects
- **HealthComponent**: Required for death detection (external system)

### Configuration
- **Death Effects**: Configured in `DeathEffectSystem.InitializeDeathEffectConfigurations()`
- **Particle Effects**: Configured in `ParticleSystem.GetEffectConfiguration()`
- **Sound Effects**: Configured in `AudioSystem` sound registry

---

## Performance Considerations

### Particle System
- **Pooling**: Reuses particle objects to reduce garbage collection
- **Distance Culling**: Only processes particles within range
- **Batch Processing**: Efficient particle updates

### Audio System
- **Distance Culling**: Skips sounds beyond hearing distance
- **Volume Calculation**: Efficient distance-based attenuation
- **Event Publishing**: Minimal overhead for tracking

### Animation System
- **State Management**: Efficient animation state tracking
- **Event Prevention**: Guards against multiple completion events
- **Component Access**: Direct component access for performance

---

## Debugging and Monitoring

### Audit Logging
All systems include comprehensive debug logging:
- **Initialization**: System startup and configuration
- **Event Handling**: Event reception and processing
- **Effect Spawning**: Particle and sound effect creation
- **Errors**: Graceful error handling with detailed messages
- **Performance**: System statistics and monitoring

### Statistics
- **AnimationTriggerSystem**: Initialization and subscription status
- **DeathEffectSystem**: Configuration count and subscription status
- **ParticleSystem**: Active particles, pool size, and capacity
- **AudioSystem**: Sound loading and playback statistics

### Error Handling
- **Graceful Degradation**: Systems continue operating despite individual failures
- **Validation**: Input parameter validation
- **Resource Management**: Proper cleanup and disposal
- **Fallback Behavior**: Default configurations for missing effects

---

## Conclusion

The P11-04-07 systems provide comprehensive death feedback for the SAS Zombie Assault TD game:

1. **AnimationTriggerSystem**: Handles death animations with proper state management
2. **DeathEffectSystem**: Coordinates particle and audio effects
3. **AnimationComponent**: Enhanced with ECS-friendly properties and events
4. **ParticleSystem**: Extended with named effect spawning
5. **AudioSystem**: Enhanced with spatialized audio playback

All systems follow architectural best practices:
- **Event-Driven Design**: Loose coupling through EventBus
- **Component-Based**: ECS-friendly architecture
- **Separation of Concerns**: Clear responsibilities
- **Audit-Friendly**: Comprehensive logging and documentation
- **Performance-Optimized**: Efficient algorithms and resource management

The implementation provides a solid foundation for death feedback that can be easily extended and configured for different game scenarios.
