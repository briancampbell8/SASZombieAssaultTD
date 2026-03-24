# P11-09 Legacy Code Analysis

## Overview
This document identifies legacy timing and loop code that needs to be updated to work with the new GameLoop system.

## Legacy Systems Identified

### 1. TimingController.cs (Engine/Core/TimingController.cs)
- **Status**: Legacy - Should be replaced by new TimingModule
- **Usage**: Currently used by WaveSystem.cs
- **Issues**: 
  - Uses old timing architecture
  - Conflicts with new TimingModule
  - Has different API (GameTime vs DeltaTime)

### 2. WaveSystem.cs Dependencies
- **File**: Engine/Systems/WaveSystem.cs
- **Lines**: 1027, 1331, 1334, 1366, 1368, 1382, 1725, 1728, 1736, 1739, 1813, 4190, 4538, 5672, 7531
- **Dependencies**: 
  - TimingController constructor parameter
  - _timingController.GameTime usage
  - _timingController.GetGameTime() usage
- **Required Changes**: 
  - Replace TimingController with new timing system
  - Update time tracking to use delta time accumulation
  - Modify constructor to accept timing data differently

## Recommended Migration Strategy

### Phase 1: Immediate (P11-09 Completion)
- Keep legacy TimingController for now to avoid breaking existing systems
- Document the legacy dependencies
- Ensure new GameLoop works independently

### Phase 2: Future Migration
- Update WaveSystem to use new timing architecture
- Remove TimingController.cs
- Update all timing references throughout the codebase

## New GameLoop Architecture

### Authoritative Loop Location
- **File**: Engine/Core/GameLoop.cs
- **Method**: Start() - contains the single authoritative while loop
- **Order**: Update → Render → Present

### Timing System
- **File**: Engine/Core/Timing/TimingModule.cs
- **Features**: High-precision timing, frame pacing, delta time calculation
- **Replaces**: TimingController.cs functionality

### Frame Diagnostics
- **File**: Engine/Core/Timing/FrameDiagnostics.cs
- **Features**: FPS counting, frame time analysis, performance metrics

## Integration Points

### GameRoot Integration
- **File**: Engine/GameRoot.cs
- **Changes**: Updated Run() method to use new GameLoop
- **Render Context**: Creates Framebuffer for rendering

### Engine State Management
- **Enum**: EngineState (Initialized, Running, Paused, Shutdown)
- **Methods**: SetPaused(), RequestShutdown()
- **Integration**: Built into GameLoop state machine

## Verification Status

### Completed Components
- ✅ P11-09-01: Authoritative game loop created
- ✅ P11-09-02: High-precision timing module implemented
- ✅ P11-09-03: Rendering pipeline integrated
- ✅ P11-09-04: Update phase hooks added
- ✅ P11-09-05: Engine state management implemented
- ✅ P11-09-06: Frame diagnostics added

### Legacy Code Status
- ⚠️ TimingController.cs still exists (legacy)
- ⚠️ WaveSystem.cs uses legacy timing
- ⚠️ No immediate removal to avoid breaking changes

## Next Steps

1. **Short Term**: Document legacy dependencies and ensure new system works
2. **Medium Term**: Migrate WaveSystem to new timing architecture
3. **Long Term**: Remove all legacy timing code

## Notes

The new GameLoop system is designed to be backward compatible where possible. Legacy systems like WaveSystem can continue to function while migration is planned. The new architecture provides a clean separation between the authoritative game loop and system-specific timing requirements.
