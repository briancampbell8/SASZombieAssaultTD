# AnimationController.cs - Complete Specification

## Purpose and Responsibilities

**Primary Purpose**: P11-16-03 - Main animation controller that integrates state machine, blend trees, and animation events. Provides deterministic animation control with state machine integration, blend tree support, and event dispatching.

**Core Responsibilities**:
- Manage animation clip playback with deterministic timing control
- Integrate with AnimationStateMachine for state-based animation control
- Support blend trees for complex animation blending
- Dispatch animation events through AnimationEventDispatcher
- Provide crossfade functionality between animations
- Manage runtime parameters for animation behavior control
- Offer comprehensive statistics and validation capabilities

## Public API

### Properties
- **AnimationStateMachine StateMachine** - Gets the state machine for this animation controller
- **BlendParameters CurrentBlendParameters** - Gets the current blend parameters
- **AnimationEventDispatcher EventDispatcher** - Gets the event dispatcher for this animation controller
- **AnimationClip? CurrentClip** - Gets the currently playing animation clip
- **float CurrentTime** - Gets the current playback time in seconds
- **float PlaybackSpeed** - Gets the playback speed multiplier
- **bool IsPlaying** - Gets whether the animation is currently playing
- **bool IsPaused** - Gets whether the animation is currently paused
- **bool IsLooping** - Gets whether the current animation should loop
- **int LoopCount** - Gets the number of times the current animation has looped
- **bool IsCrossfading** - Gets whether a crossfade is currently in progress
- **float CrossfadeProgress** - Gets the crossfade progress as a value between 0 and 1
- **IReadOnlyDictionary<string, float> Parameters** - Gets read-only access to animation parameters

### Events
- **Action<string>? OnClipStarted** - Fired when an animation clip starts playing
- **Action<string>? OnClipCompleted** - Fired when an animation clip completes
- **Action<string, string>? OnClipChanged** - Fired when an animation clip changes (previous, current)
- **Action<string, string>? OnStateChanged** - Fired when a state machine transition occurs (previous, current)
- **Action<AnimationEvent>? OnAnimationEvent** - Fired when an animation event is triggered

### Methods
- **AnimationController()** - Constructor initializing all systems with deterministic defaults
- **void Update(float deltaTime)** - Updates animation controller with deterministic timing
- **void Play(string clipName, bool loop = false, float crossfadeDuration = 0.0f)** - Plays an animation clip with optional looping and crossfade
- **void Stop()** - Stops current animation with deterministic state reset
- **void Pause()** - Pauses current animation with deterministic state preservation
- **void Resume()** - Resumes current animation with deterministic state restoration
- **void SetPlaybackSpeed(float speed)** - Sets playback speed with deterministic rate control
- **void AddClip(AnimationClip clip)** - Adds an animation clip with deterministic clip management
- **void RemoveClip(string clipName)** - Removes an animation clip with deterministic clip management
- **void SetStateBlendTree(string stateName, BlendTree? blendTree)** - Sets a blend tree for a specific state
- **void SetParameter(string parameterName, float value)** - Sets an animation parameter with deterministic parameter management
- **float GetParameter(string parameterName)** - Gets an animation parameter with deterministic parameter access
- **void ForceTransition(string stateName)** - Forces a state machine transition with deterministic state control
- **Dictionary<string, object> GetStatistics()** - Gets comprehensive statistics about animation controller
- **bool ValidateConfiguration()** - Validates animation controller configuration

## Internal Helpers

### Private Fields
- **AnimationStateMachine _stateMachine** - State machine for managing animation states and transitions
- **Dictionary<string, AnimationClip> _clips** - Dictionary of animation clips keyed by name
- **Dictionary<string, BlendTree> _stateToBlendTreeMap** - Dictionary of blend trees keyed by state name
- **BlendParameters _currentBlendParameters** - Current blend parameters for blend tree evaluation
- **AnimationEventDispatcher _eventDispatcher** - Event dispatcher for animation events
- **AnimationClip? _currentClip** - Currently playing animation clip
- **float _currentTime** - Current playback time in seconds
- **float _playbackSpeed** - Playback speed multiplier
- **bool _isPlaying** - Whether the animation is currently playing
- **bool _isPaused** - Whether the animation is currently paused
- **bool _isLooping** - Whether the current animation should loop
- **int _loopCount** - Number of times the current animation has looped
- **float _crossfadeDuration** - Crossfade duration in seconds
- **float _crossfadeTime** - Current crossfade time in seconds
- **AnimationClip? _previousClip** - Previous animation clip for crossfading
- **Dictionary<string, float> _parameters** - Animation parameters for runtime control

### Private Methods
- **void StartCrossfade(AnimationClip newClip, float duration)** - Starts a crossfade between animations
- **void CompleteCrossfade()** - Completes a crossfade with deterministic finalization
- **void ProcessAnimationEvents(float deltaTime)** - Processes animation events with deterministic event handling
- **void SynchronizeStateWithAnimation()** - Synchronizes state machine with current animation

## Data Structures

### Core Data Types
- **Dictionary<string, AnimationClip>** - Clip storage with string key lookup
- **Dictionary<string, BlendTree>** - Blend tree storage with state-based lookup
- **Dictionary<string, float>** - Parameter storage for runtime control
- **BlendParameters** - Structured parameter set for blend tree evaluation

### State Management
- **Deterministic state tracking** with explicit CurrentState and PreviousState
- **Crossfade state management** with duration and progress tracking
- **Parameter synchronization** between animation controller and blend trees
- **Event queue management** for deterministic event processing

## State Flow

### Initialization Flow
1. Create AnimationStateMachine with deterministic state management
2. Initialize clip and parameter dictionaries with StringComparer.Ordinal
3. Create BlendParameters and AnimationEventDispatcher
4. Set all state variables to deterministic defaults
5. Log initialization completion

### Update Flow
1. Validate deltaTime (reject non-positive values)
2. Early return if not playing or paused
3. Calculate adjustedDeltaTime = deltaTime * playbackSpeed
4. Update state machine with adjusted time
5. Handle crossfade progress if active
6. Update current animation timing
7. Check for animation completion and handle looping
8. Process animation events for current time range
9. Synchronize state machine with animation parameters

### Play Flow
1. Validate clip name (reject null/empty)
2. Look up clip in dictionary (reject if not found)
3. Store previous clip name for change events
4. Handle crossfade setup if duration > 0
5. Set new clip and reset timing state
6. Update playback state (playing, not paused, loop flag)
7. Fire appropriate events (ClipChanged, ClipStarted)

### Crossfade Flow
1. Validate new clip and duration
2. Store current clip as previous
3. Set new clip as current
4. Reset timing and loop state
5. Set crossfade duration and start time
6. Log crossfade initiation

## Integration Points

### ECS Integration
- **AnimationControllerComponent**: Uses AnimationController as reference for entity-level animation control
- **AnimationSystem**: Updates multiple AnimationController instances per frame
- **AnimationECSIntegration**: Provides statistics and validation for ECS integration
- **AnimationDebugTools**: Inspects AnimationController state for debugging

### Animation System Integration
- **AnimationStateMachine**: Direct integration for state-based animation control
- **AnimationClip**: Storage and playback management
- **BlendTree**: Integration for complex animation blending
- **AnimationEventDispatcher**: Event processing and dispatching
- **BlendParameters**: Parameter management for blend tree evaluation

### Tools Integration
- **AnimationVerificationSuite**: Tests AnimationController functionality
- **AnimationStateMachineValidationReport**: Validates AnimationController integration
- **AnimationSystemTest**: Integration testing for AnimationController

### Navigation Integration
- **AnimationController** referenced in NavigationVerificationSuite for animation-state synchronization testing

### Input Integration
- **AnimationController** parameters can be controlled through input systems for interactive animation control

## Expected Behavior

### Deterministic Behavior
- All state changes are explicit and trackable
- No random or non-deterministic behavior in timing or state management
- Consistent parameter handling and validation
- Predictable event firing order

### Error Handling
- Comprehensive null checking and validation
- Graceful degradation for invalid operations
- Detailed error logging through DebugLogger
- State consistency maintenance during errors

### Performance Characteristics
- Efficient dictionary lookups with StringComparer.Ordinal
- Minimal allocations during update loops
- Optimized event processing with early returns
- Deterministic memory usage patterns

### Thread Safety
- Not inherently thread-safe (designed for single-threaded game loop)
- All operations assume single-threaded access
- State changes are atomic within single frame updates

## Validation Requirements

### Configuration Validation
- Must have at least one animation clip registered
- Current clip must exist in clip collection
- Playback speed cannot be negative
- Crossfade duration cannot be negative
- All blend trees must have valid state names

### Runtime Validation
- Clip names cannot be null or empty
- Parameters must have valid names
- State transitions must be to valid states
- Crossfade operations must have valid clips

## Statistics and Monitoring

### Runtime Statistics
- Current clip and timing information
- Playback state and loop counts
- Crossfade progress and state
- Parameter and blend tree counts
- State machine integration status

### Performance Metrics
- Entity processing counts (via AnimationSystem)
- Animation update timing
- Event dispatch statistics
- Memory usage patterns

This specification provides the complete foundation for implementing AnimationController.cs with full integration across the project's animation, ECS, and tooling systems.
