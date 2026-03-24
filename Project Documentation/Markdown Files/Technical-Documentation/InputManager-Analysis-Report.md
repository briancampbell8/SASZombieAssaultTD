# InputManager.cs Project Analysis and Mapping Report

## **File Information**
- **File Path**: `Engine/Input/InputManager.cs`
- **Current Namespace**: `SASZombieAssaultTD.Engine.Input`
- **Purpose**: Central input system hub for all user interaction
- **Dependencies**: Multiple input source classes, event system

---

## **1. System Architecture Analysis**

### **Core Purpose**
InputManager serves as the central orchestrator for all input processing in the SASZombieAssaultTD engine. It manages multiple input devices, routes input events to appropriate handlers, and provides input recording/playback capabilities for debugging and testing.

### **Key Responsibilities**
- **Input Source Management**: Register and manage keyboard, mouse, and gamepad input sources
- **Event Routing**: Direct input events to appropriate game systems and handlers
- **Input Mapping**: Translate raw input events to gameplay actions
- **Recording System**: Capture input sequences for playback and debugging
- **State Management**: Track active input handlers and device states
- **Performance Optimization**: Efficient event processing and buffering

### **Integration Points**
- **ECS System**: Feeds input events to entity behavior components
- **Event System**: Publishes input events for system-wide consumption
- **Animation System**: Triggers animation state changes based on input
- **UI System**: Handles UI-specific input interactions
- **Physics System**: Directs physics-affecting input (movement, forces)

---

## **2. Build Standards Specification**

### **Required Using Directives**
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using SASZombieAssaultTD.Engine.Events;
using SASZombieAssaultTD.Engine.Input;
```

### **Namespace Requirements**
- **Primary Namespace**: `SASZombieAssaultTD.Engine.Input`
- **No nested namespaces** within this file
- **All input sources** must use the same namespace

### **Critical Dependencies That Must Exist**
- **IInputSource interface**: Contract for all input devices
- **InputEvent struct**: Standardized input event data structure
- **InputAction enum**: Gameplay action mappings
- **InputEventType enum**: Input event categorization
- **EventBus class**: Event publishing and subscription
- **InputBuffer class**: Input event buffering and management

### **Missing Classes That Must Be Defined**
1. **MouseInputSource**: Mouse input device implementation
2. **InputRecorder**: Input sequence recording functionality
3. **InputPlayback**: Recorded input sequence playback
4. **InputEvent**: Standardized input event structure
5. **KeyCode enum**: Keyboard key definitions
6. **InputState enum**: Input state values (Pressed, Released, Held)

---

## **3. Integration Mapping**

### **Systems That Depend on InputManager**
- **AnimationController**: Triggers animation state changes
- **PhysicsSystem**: Receives movement and force input
- **CollisionSystem**: Handles collision-related input
- **RenderingSystem**: Processes camera and rendering input
- **UISystem**: Manages UI interaction input
- **ECS Systems**: Directs input to entity components

### **Systems That InputManager Depends On**
- **EventBus**: For publishing input events
- **Input Sources**: KeyboardInputSource, MouseInputSource, GamepadInputSource
- **Component System**: For entity-specific input routing
- **Time System**: For input timing and delta calculations

### **Data Flow Architecture**
```
Input Devices → InputManager → EventBus → Game Systems
                    ↓
              Input Recording/Playback
                    ↓
              Input Action Mapping
```

---

## **4. Implementation Guidelines**

### **Design Patterns to Follow**
- **Observer Pattern**: Event-driven input notification
- **Strategy Pattern**: Different input source implementations
- **Command Pattern**: Input action mapping and execution
- **Singleton Pattern**: Single input manager instance
- **Factory Pattern**: Input source creation and management

### **Performance Requirements**
- **Real-time Processing**: Input must be processed within single frame
- **Low Latency**: Minimal delay between input and system response
- **Memory Efficiency**: Object pooling for frequent input events
- **Thread Safety**: Safe concurrent access to input state

### **Memory Management Considerations**
- **Object Pooling**: Reuse InputEvent instances
- **Buffer Management**: Fixed-size input buffers to prevent memory growth
- **Garbage Collection**: Minimize allocations in hot paths
- **Resource Cleanup**: Proper disposal of input sources

### **Thread Safety Requirements**
- **Concurrent Access**: Multiple systems may query input state simultaneously
- **Lock-Free Operations**: Where possible, avoid locks in performance-critical paths
- **Atomic Operations**: For simple state changes and queries
- **Event Queue Safety**: Thread-safe event publishing and consumption

---

## **5. Build Readiness Checklist**

### **Syntax Requirements**
- ✅ All class definitions complete with proper access modifiers
- ✅ All method implementations present (no empty methods unless intentional)
- ✅ Proper using statements for all referenced types
- ✅ Correct namespace declarations and organization

### **Reference Completeness**
- ✅ All input source classes properly defined and accessible
- ✅ Event system integration points established
- ✅ Enum definitions complete and consistent
- ✅ Interface implementations properly declared

### **Interface Implementation Status**
- ✅ IInputSource implementations for all device types
- ✅ IInputHandler implementations for game systems
- ✅ Event subscription and unsubscription mechanisms
- ✅ Input mapping and action resolution interfaces

### **Documentation Standards**
- ✅ XML documentation on all public APIs
- ✅ Parameter descriptions for all methods
- ✅ Usage examples for complex operations
- ✅ Performance characteristics documentation

---

## **6. Critical Success Factors**

### **Must-Have Features**
1. **Multi-Device Support**: Keyboard, mouse, gamepad input handling
2. **Event-Driven Architecture**: Efficient event publishing and subscription
3. **Input Recording**: Complete input capture and playback system
4. **Action Mapping**: Raw input to gameplay action translation
5. **Performance Optimization**: Sub-frame input processing

### **Integration Requirements**
1. **ECS Compatibility**: Seamless integration with entity-component system
2. **Event System Integration**: Proper use of existing EventBus infrastructure
3. **Animation System Triggers**: Input-driven animation state changes
4. **Physics System Input**: Movement and force application through input
5. **UI System Interaction**: Separate handling for UI vs gameplay input

### **Quality Standards**
1. **Zero Compilation Errors**: All references and types properly resolved
2. **Consistent Naming**: Following established engine conventions
3. **Memory Efficiency**: Minimal allocations in hot paths
4. **Thread Safety**: Safe concurrent access patterns
5. **Comprehensive Testing**: All input scenarios covered

---

## **7. Expected Deliverables**

### **Core Classes to Implement**
- **InputManager**: Main orchestrator class
- **MouseInputSource**: Mouse device implementation
- **InputRecorder**: Recording functionality
- **InputPlayback**: Playback functionality
- **InputEvent**: Event data structure
- **Supporting Enums**: KeyCode, InputState, InputAction, InputEventType

### **Key Methods to Implement**
- **Input Processing**: Update loops and event handling
- **Device Management**: Registration and lifecycle management
- **Recording/Playback**: Capture and replay functionality
- **Action Mapping**: Input to gameplay action translation
- **Event Routing**: Proper event distribution to handlers

---

## **8. File Organization Requirements**

### **Directory Structure**
```
Engine/Input/
├── InputManager.cs (Primary file)
├── KeyboardInputSource.cs (Exists)
├── MouseInputSource.cs (To be created/fixed)
├── InputRecorder.cs (To be created/fixed)
├── InputPlayback.cs (To be created/fixed)
└── InputTypes.cs (Supporting types and enums)
```

### **Namespace Consistency**
- All files must use `SASZombieAssaultTD.Engine.Input`
- No cross-namespace dependencies within input system
- Clear separation between device implementations and management

---

## **9. Pre-Implementation Cleanup Required**

### **Files to Delete Before Implementation**
1. **`Engine/Core/Managers/InputManager.cs`** - Duplicate file causing namespace conflicts
2. **`Engine/Core/Input/InputEvent.cs`** - Duplicate file causing ambiguous references

### **Missing Files to Create**
1. **`Engine/Input/InputBuffer.cs`** - Input event buffering functionality
2. **`Engine/Input/InputTypes.cs`** - Supporting enums and type definitions

### **Why Cleanup Is Critical**
- **Prevents Build Errors**: Duplicate files cause "ambiguous reference" compilation errors
- **Ensures Single Source of Truth**: One definitive implementation per type
- **Maintains Clean Architecture**: Proper separation of concerns
- **Avoids Copilot Confusion**: Clear file structure for AI implementation

---

## **10. Implementation Priority**

### **Phase 1: Foundation**
1. **InputManager.cs** - Main orchestrator
2. **InputEvent.cs** - Event data structure
3. **IInputSource.cs** - Interface contracts

### **Phase 2: Device Support**
4. **MouseInputSource.cs** - Mouse device implementation
5. **InputBuffer.cs** - Event buffering
6. **InputTypes.cs** - Supporting enums

### **Phase 3: Advanced Features**
7. **InputRecorder.cs** - Recording functionality
8. **InputPlayback.cs** - Playback functionality

---

**This comprehensive analysis provides Copilot with the complete specification needed to rebuild InputManager.cs and its dependencies for a clean, maintainable, and performant input system.**
