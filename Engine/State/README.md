# State Machine System

This directory contains the complete state machine system for SAS Zombie Assault TD, including both the basic P20-02 implementation and enhanced features.

## Core Files

### Basic Implementation (P20-02)
- `IGameState.cs` - Base interface for all game states
- `GameStateType.cs` - Enumeration of game state types
- `GameEvent.cs` - Base class for state machine events
- `StateMachine.cs` - Core state machine controller
- `BootState.cs` - Initial boot state implementation
- `MainMenuState.cs` - Main menu state with navigation
- `GameplayState.cs` - Active gameplay state
- `PausedState.cs` - Game pause state
- `StateMachineTest.cs` - Verification tests

### Enhanced Features
- `StateTransition.cs` - Enhanced transition tracking and validation
- `StateMachineExtensions.cs` - Extension methods for additional functionality
- `StateFactory.cs` - Factory pattern for state creation and configuration
- `AdvancedStateMachine.cs` - State machine with history and advanced features
- `StateDebugger.cs` - Comprehensive debugging and monitoring utilities
- `StateMachineProfiler.cs` - Real-time performance monitoring
- `EnhancedStateMachine.cs` - Production-ready state machine with full monitoring
- `StateBuilder.cs` - Fluent builder pattern for configuration
- `StateMachineIntegration.cs` - Integration utilities for engine systems

## Quick Start

### Basic Usage
```csharp
// Create a basic state machine
var stateMachine = new StateMachine();
stateMachine.RegisterState(GameStateType.Boot, new BootState(stateMachine));
stateMachine.RegisterState(GameStateType.MainMenu, new MainMenuState(stateMachine));
stateMachine.ChangeState(GameStateType.Boot);
```

### Enhanced Usage
```csharp
// Create an enhanced state machine with all features
var stateMachine = StateMachineBuilder.Create()
    .WithDefaultStates()
    .WithProfiling()
    .WithDebugging()
    .BuildEnhanced();
```

### Environment-Specific
```csharp
// Create for different environments
var devMachine = StateMachineIntegration.CreateEnvironmentStateMachine(StateMachineEnvironment.Development);
var prodMachine = StateMachineIntegration.CreateEnvironmentStateMachine(StateMachineEnvironment.Production);
```

## State Flow

```
Boot → MainMenu → Gameplay → Paused
  ↑       ↓         ↓       ↓
  └───────┴─────────┴───────┘
```

## Key Features

### Basic Features
- ✅ State registration and management
- ✅ State transitions with Enter/Exit lifecycle
- ✅ Event handling and forwarding
- ✅ Input event processing
- ✅ Engine integration (GameRoot)

### Enhanced Features
- ✅ Performance profiling and monitoring
- ✅ Comprehensive debugging and logging
- ✅ Transition history tracking
- ✅ State configuration management
- ✅ Factory pattern for state creation
- ✅ Builder pattern for configuration
- ✅ Environment-specific configurations
- ✅ Validation and issue detection
- ✅ Integration utilities

## Documentation

- `../Docs/P20-02-StateMachineIntegration-Documentation.md` - Basic implementation documentation
- `../Docs/P20-02-EnhancedStateMachine-Documentation.md` - Enhanced features documentation

## Integration

The state machine system is designed to integrate seamlessly with:
- GameRoot (engine core)
- Input system (P20-01)
- Audio system
- UI system
- Event system (EventBus)

## Performance

- Minimal overhead for basic operations (~0.01ms)
- Configurable profiling overhead (~0.1ms when enabled)
- Bounded memory usage with configurable limits
- Production-ready with essential monitoring

## Best Practices

1. **Development**: Use enhanced state machine with full debugging
2. **Testing**: Use enhanced state machine with moderate debugging
3. **Production**: Use enhanced state machine with minimal overhead
4. **Custom States**: Use StateFactory for consistent state creation
5. **Configuration**: Use StateBuilder for flexible setup
6. **Monitoring**: Set up automatic performance monitoring in production

## Examples

See the documentation files for comprehensive usage examples and integration patterns.
