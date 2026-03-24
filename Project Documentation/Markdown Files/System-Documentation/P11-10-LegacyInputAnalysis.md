# P11-10 Legacy Input Code Analysis

## Overview
This document identifies legacy input code that needs to be migrated to the new InputModule system as part of P11-10-08.

## Legacy Systems Identified

### 1. InputSystem.cs (Engine/Systems/Input/InputSystem.cs)
- **Status**: Legacy - Should be replaced by new InputModule
- **Location**: `Engine/Systems/Input/InputSystem.cs`
- **Features**: Basic keyboard/mouse state tracking
- **API**: `GetKeyDown()`, `GetKeyPressed()`, `GetMousePosition()`, `SetKeyState()`, `SetMousePosition()`
- **Issues**: 
  - Limited key enumeration (only Enter, Escape, Space)
  - No event capture layer
  - No mouse button tracking
  - No scroll wheel support
  - No delta tracking

### 2. Legacy InputSystem in Compatibility Layer
- **Location**: `Engine/Compatibility/LegacyCompatibilityLayer.cs`
- **Lines**: 326-329
- **Purpose**: Fallback input system
- **Status**: Should be removed or updated

## Current Dependencies

### GameRoot.cs Dependencies
- **Line 32**: `private readonly InputSystem _inputSystem = new InputSystem();`
- **Line 78**: `public InputSystem InputSystem => _inputSystem;`
- **Line 236**: `_inputSystem.Shutdown();`

### BaseScene.cs Dependencies
- **Line 23**: `protected InputSystem? InputSystem => _gameRoot?.InputSystem;`

### TestScenes Dependencies
- **Line 16**: `public InputSystem? InputSystem { get; set; }`

## Migration Strategy

### Phase 1: Immediate (P11-10-08 Completion)
- **Document legacy dependencies** ✅
- **Create migration plan** ✅
- **Update GameRoot to use InputModule** 
- **Update BaseScene to use InputModule**
- **Remove legacy InputSystem.cs**

### Phase 2: API Compatibility
- **Create adapter methods** if needed for backward compatibility
- **Update enum mappings** (Key enum vs Keys enum)
- **Update method signatures** (PointF vs Point)

### Phase 3: Cleanup
- **Remove LegacyCompatibilityLayer InputSystem**
- **Update all references to use new InputModule**
- **Remove using statements for legacy namespace**

## API Mapping

### Legacy InputSystem → New InputModule

| Legacy API | New InputModule API | Notes |
|------------|-------------------|-------|
| `GetKeyDown(Key key)` | `IsKeyHeld(Keys key)` | Different enum type |
| `GetKeyPressed(Key key)` | `IsKeyPressed(Keys key)` | Different enum type |
| `GetMousePosition()` | `GetMousePosition()` | PointF vs Point |
| `SetKeyState(Key key, bool down)` | Event-based system | Use OnKeyDown/OnKeyUp |
| `SetMousePosition(float x, float y)` | Event-based system | Use OnMouseMove |

### Enum Mapping

| Legacy Key | New Keys |
|------------|----------|
| Key.Enter | Keys.Enter |
| Key.Escape | Keys.Escape |
| Key.Space | Keys.Space |

## Integration Points

### GameRoot Integration Required
- Replace `InputSystem` field with `InputModule` reference
- Update constructor to accept InputModule from GameLoop
- Update InputSystem property to return InputModule
- Remove legacy InputSystem initialization

### BaseScene Integration Required
- Update InputSystem property to use new InputModule
- Update any input queries to use new API

## Benefits of Migration

### New InputModule Advantages
- **Event Capture Layer**: Proper OS event handling
- **Per-Frame Polling**: Deterministic state updates
- **Comprehensive Key Support**: Full keyboard coverage
- **Mouse Button Tracking**: All mouse buttons supported
- **Scroll Wheel Support**: Delta tracking included
- **Debug Overlay**: Built-in debugging capabilities
- **Thread-Safe**: Proper event buffering
- **Better Performance**: Optimized state management

### Legacy InputSystem Limitations
- **Limited Keys**: Only 4 keys supported
- **No Mouse Buttons**: Only position tracking
- **No Events**: Direct state setting only
- **No Scroll Wheel**: Missing common input
- **No Debug Support**: No diagnostic capabilities

## Migration Priority

### High Priority
1. Update GameRoot to use InputModule
2. Update BaseScene to use InputModule
3. Remove legacy InputSystem.cs

### Medium Priority
4. Update TestScenes to use InputModule
5. Remove LegacyCompatibilityLayer InputSystem

### Low Priority
6. Create compatibility adapters if needed
7. Update documentation references

## Risk Assessment

### Low Risk
- New InputModule is fully functional
- API is more comprehensive than legacy
- No breaking changes to core game logic

### Medium Risk
- Enum type changes (Key vs Keys)
- Point vs PointF type differences
- Method signature changes

### Mitigation
- Create adapter methods if needed
- Update all references systematically
- Test input functionality thoroughly

## Next Steps

1. **Update GameRoot.cs** to integrate InputModule
2. **Update BaseScene.cs** to use InputModule
3. **Remove legacy InputSystem.cs**
4. **Test input functionality** with new system
5. **Update documentation** to reflect changes

The new InputModule provides a significant improvement over the legacy system and should be adopted as the standard input handling solution.
