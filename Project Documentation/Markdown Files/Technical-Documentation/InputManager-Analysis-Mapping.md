# InputManager.cs Analysis and Build Mapping

## 📊 System Analysis Overview

**File:** `Engine/Core/Managers/InputManager.cs`  
**Purpose:** Concrete implementation of BaseManager for input processing and management  
**Role:** Manages input device coordination, event processing, and input state tracking

---

## 🎯 **System Flow Analysis**

### **Integration Points in Engine Architecture:**

#### **1. GameRoot.cs InputManager Integration**
```csharp
// GameRoot constructor - InputManager injected
public GameRoot(
    ISystemRegistry systemRegistry,
    SystemManager systemManager,
    UpdateManager updateManager,
    RenderManager renderManager,
    InputManager inputManager)      // : BaseManager
{
    _inputManager = inputManager;
}

// GameRoot update loop
public void Update(float deltaTime)
{
    if (!_isInitialized)
        return;
        
    _stateMachine.Update(deltaTime);
    _updateManager.UpdateAll(deltaTime);
    _inputManager.Update(deltaTime);    // Process input events
}

// GameRoot initialization/shutdown
public void Initialize()
{
    _systemManager.Initialize();
    _updateManager.Initialize();
    _renderManager.Initialize();
    _inputManager.Initialize();        // Initialize input manager
}

public void Stop()
{
    _systemManager.Shutdown();
    _updateManager.Shutdown();
    _renderManager.Shutdown();
    _inputManager.Shutdown();          // Shutdown input manager
}
```

#### **2. Input Processing Orchestration**
```csharp
// InputManager responsibilities
- Discover all systems implementing IInputSystem
- Coordinate input device processing
- Track input state and events
- Handle input failures gracefully
- Provide input diagnostics and profiling
- Manage input device connections/disconnections
- Route input events to appropriate systems
```

#### **3. SystemRegistry Integration**
```csharp
// InputManager uses SystemRegistry to find input systems
var systems = _registry.GetRegisteredTypes();
foreach (var systemType in systems)
{
    var system = _registry.GetSystem(systemType);
    if (system is IInputSystem inputSystem)
    {
        _inputSystems.Add(inputSystem);
    }
}
```

---

## 🏗 **Required Implementation Structure**

### **Core InputManager Design:**
```csharp
namespace SASZombieAssaultTD.Engine.Core.Managers
{
    /// <summary>
    /// Manages input processing for all engine systems.
    /// Handles input device coordination, event processing, and
    /// error isolation for systems requiring input management.
    /// </summary>
    public class InputManager : BaseManager
    {
        private readonly List<IInputSystem> _inputSystems = new();
        private readonly Dictionary<Type, InputSystemInfo> _systemInfo = new();
        
        // Input device management
        private readonly Dictionary<InputDeviceType, IInputDevice> _devices = new();
        private readonly InputState _inputState = new();
        
        // Performance tracking
        private float _totalInputTime = 0f;
        private int _frameCount = 0;
        private readonly float[] _inputTimeHistory = new float[60]; // 1 second at 60fps
        
        // Event management
        private readonly Queue<InputEvent> _eventQueue = new();
        private readonly Dictionary<InputEventType, List<IInputHandler>> _handlers = new();
        
        // Constructor
        public InputManager(ISystemRegistry registry) 
            : base("InputManager", registry)
        {
        }
        
        // BaseManager abstract method implementations
        protected override int GetSystemCount() => _inputSystems.Count;
        
        protected override void DoInitialize()
        {
            // Discover all input systems
            DiscoverInputSystems();
            
            // Initialize input devices
            InitializeInputDevices();
            
            // Setup event handlers
            SetupEventHandlers();
            
            LogInfo($"Discovered {_inputSystems.Count} input systems");
        }
        
        protected override void DoShutdown()
        {
            // Shutdown input devices
            ShutdownInputDevices();
            
            // Clear all input systems
            _inputSystems.Clear();
            _systemInfo.Clear();
            _devices.Clear();
            _handlers.Clear();
            
            // Clear event queue
            while (_eventQueue.Count > 0)
                _eventQueue.Dequeue();
            
            LogInfo("Shutdown input manager");
        }
        
        // Input-specific methods
        public void Update(float deltaTime)
        public void RegisterDevice(IInputDevice device)
        public void UnregisterDevice(InputDeviceType deviceType)
        public void RegisterHandler(InputEventType eventType, IInputHandler handler)
        public void UnregisterHandler(InputEventType eventType, IInputHandler handler)
        public InputSystemInfo? GetSystemInfo<T>() where T : class
        public IEnumerable<InputSystemInfo> GetAllSystemInfo()
        public InputPerformanceMetrics GetPerformanceMetrics()
        public InputState GetInputState()
    }
}
```

### **Supporting Types:**
```csharp
/// <summary>
/// Information about an input system.
/// </summary>
public class InputSystemInfo
{
    public Type SystemType { get; set; }
    public string Name { get; set; }
    public bool IsEnabled { get; set; }
    public int InputPriority { get; set; }
    public float AverageInputTime { get; set; }
    public float LastInputTime { get; set; }
    public int InputCount { get; set; }
    public int EventCount { get; set; }
    public int ErrorCount { get; set; }
    public string? LastError { get; set; }
    public DateTime LastInputTimeStamp { get; set; }
    public List<InputDeviceType> SupportedDevices { get; set; }
}

/// <summary>
/// Performance metrics for the input manager.
/// </summary>
public class InputPerformanceMetrics
{
    public float TotalInputTime { get; set; }
    public float AverageInputTime { get; set; }
    public float MaxInputTime { get; set; }
    public float MinInputTime { get; set; }
    public int FrameCount { get; set; }
    public float[] InputTimeHistory { get; set; }
    public int ActiveSystemCount { get; set; }
    public int DisabledSystemCount { get; set; }
    public int EventsProcessed { get; set; }
    public int DevicesConnected { get; set; }
    public float EventsPerSecond { get; set; }
}
```

---

## 📋 **Required Dependencies**

### **Using Statements:**
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using SASZombieAssaultTD.Engine.Core.Interfaces;
```

### **Dependencies:**
- **BaseManager** - Abstract base class to inherit from
- **ISystemRegistry** - For system resolution and discovery
- **IInputSystem** - Interface for systems requiring input processing
- **IInputDevice** - Interface for input devices
- **IInputHandler** - Interface for input event handlers
- **InputSystemInfo** - Class for input diagnostic information
- **InputPerformanceMetrics** - Class for input performance tracking

---

## 🔧 **Implementation Requirements**

### **1. Input System Discovery**
```csharp
private void DiscoverInputSystems()
{
    lock (_lock)
    {
        var registeredTypes = _registry.GetRegisteredTypes().ToList();
        SetMetric("RegisteredSystemCount", registeredTypes.Count);
        
        foreach (var systemType in registeredTypes)
        {
            try
            {
                var system = _registry.GetSystem(systemType);
                if (system == null)
                {
                    LogWarning($"System {systemType.Name} not found in registry.");
                    continue;
                }
                
                if (system is IInputSystem inputSystem)
                {
                    _systemInfo[systemType] = new InputSystemInfo
                    {
                        SystemType = systemType,
                        Name = systemType.Name,
                        IsEnabled = inputSystem.IsEnabled,
                        InputPriority = inputSystem.InputPriority,
                        LastInputTimeStamp = DateTime.Now,
                        SupportedDevices = inputSystem.SupportedDevices.ToList()
                    };
                    
                    _inputSystems.Add(inputSystem);
                    
                    LogInfo($"Discovered input system {systemType.Name} with priority {inputSystem.InputPriority}.");
                }
            }
            catch (Exception ex)
            {
                LogError($"Failed to discover system {systemType.Name}: {ex.Message}", ex);
            }
        }
        
        SetMetric("InputSystemCount", _inputSystems.Count);
    }
}
```

### **2. Priority-Based Input Processing**
```csharp
private void SortSystemsByPriority()
{
    lock (_lock)
    {
        // Sort by input priority (lower numbers process first)
        _inputSystems.Sort((a, b) => a.InputPriority.CompareTo(b.InputPriority));
        
        LogInfo("Sorted input systems by priority.");
    }
}

public void Update(float deltaTime)
{
    if (Status != ManagerStatus.Running)
        return;
        
    lock (_lock)
    {
        var frameStartTime = DateTime.Now;
        
        try
        {
            // Update input devices
            UpdateInputDevices(deltaTime);
            
            // Process input events
            ProcessInputEvents(deltaTime);
            
            // Update all enabled systems in priority order
            foreach (var system in _inputSystems)
            {
                if (!system.IsEnabled)
                    continue;
                    
                var systemType = system.GetType();
                var info = _systemInfo[systemType];
                
                var inputStartTime = DateTime.Now;
                
                try
                {
                    system.ProcessInput(_inputState, deltaTime);
                    
                    var inputTime = (float)(DateTime.Now - inputStartTime).TotalMilliseconds;
                    
                    info.LastInputTime = inputTime;
                    info.InputCount++;
                    info.LastInputTimeStamp = DateTime.Now;
                    
                    // Update running average
                    info.AverageInputTime = 
                        (info.AverageInputTime * (info.InputCount - 1) + inputTime) / info.InputCount;
                }
                catch (Exception ex)
                {
                    info.ErrorCount++;
                    info.LastError = ex.Message;
                    
                    LogError($"System {systemType.Name} input processing failed: {ex.Message}", ex);
                    
                    // Optionally disable system on repeated errors
                    if (info.ErrorCount > 10)
                    {
                        system.IsEnabled = false;
                        info.IsEnabled = false;
                        LogWarning($"Disabling system {systemType.Name} due to repeated input errors.");
                    }
                }
            }
        }
        finally
        {
            // Track frame performance
            var frameInputTime = (float)(DateTime.Now - frameStartTime).TotalMilliseconds;
            UpdateInputMetrics(frameInputTime);
            
            UpdateLastActivity();
        }
    }
}
```

### **3. Input Device Management**
```csharp
private void InitializeInputDevices()
{
    lock (_lock)
    {
        try
        {
            // Initialize standard input devices
            RegisterDevice(new KeyboardDevice());
            RegisterDevice(new MouseDevice());
            RegisterDevice(new GamepadDevice());
            
            LogInfo("Initialized input devices");
        }
        catch (Exception ex)
        {
            LogError("Failed to initialize input devices", ex);
            throw;
        }
    }
}

private void UpdateInputDevices(float deltaTime)
{
    lock (_lock)
    {
        foreach (var device in _devices.Values)
        {
            try
            {
                device.Update(deltaTime);
                
                // Collect input state
                _inputState.UpdateFromDevice(device);
            }
            catch (Exception ex)
            {
                LogError($"Failed to update device {device.DeviceType}: {ex.Message}", ex);
            }
        }
    }
}

public void RegisterDevice(IInputDevice device)
{
    lock (_lock)
    {
        if (_devices.ContainsKey(device.DeviceType))
        {
            LogWarning($"Device {device.DeviceType} already registered. Replacing...");
            _devices[device.DeviceType].Dispose();
        }
        
        _devices[device.DeviceType] = device;
        device.Initialize();
        
        LogInfo($"Registered input device {device.DeviceType}");
    }
}
```

### **4. Input Event Processing**
```csharp
private void ProcessInputEvents(float deltaTime)
{
    lock (_lock)
    {
        // Process queued events
        while (_eventQueue.Count > 0)
        {
            var inputEvent = _eventQueue.Dequeue();
            ProcessEvent(inputEvent);
        }
        
        // Generate events from input state changes
        var newEvents = _inputState.GenerateEvents();
        foreach (var newEvent in newEvents)
        {
            ProcessEvent(newEvent);
        }
    }
}

private void ProcessEvent(InputEvent inputEvent)
{
    try
    {
        // Route event to appropriate handlers
        if (_handlers.TryGetValue(inputEvent.EventType, out var handlers))
        {
            foreach (var handler in handlers)
            {
                try
                {
                    handler.HandleInput(inputEvent);
                }
                catch (Exception ex)
                {
                    LogError($"Input handler failed for event {inputEvent.EventType}: {ex.Message}", ex);
                }
            }
        }
        
        // Update system event counts
        foreach (var info in _systemInfo.Values)
        {
            info.EventCount++;
        }
    }
    catch (Exception ex)
    {
        LogError($"Failed to process input event {inputEvent.EventType}: {ex.Message}", ex);
    }
}

public void RegisterHandler(InputEventType eventType, IInputHandler handler)
{
    lock (_lock)
    {
        if (!_handlers.ContainsKey(eventType))
        {
            _handlers[eventType] = new List<IInputHandler>();
        }
        
        _handlers[eventType].Add(handler);
        LogInfo($"Registered handler for event type {eventType}");
    }
}
```

### **5. Performance Tracking and Metrics**
```csharp
private void UpdateInputMetrics(float frameInputTime)
{
    _totalInputTime += frameInputTime;
    _frameCount++;
    
    // Update rolling history (last 60 frames)
    _inputTimeHistory[_frameCount % 60] = frameInputTime;
    
    // Update metrics every 60 frames (approximately 1 second)
    if (_frameCount % 60 == 0)
    {
        var avgInputTime = _totalInputTime / _frameCount;
        var maxInputTime = _inputTimeHistory.Max();
        var minInputTime = _inputTimeHistory.Where(t => t > 0).DefaultIfEmpty().Min();
        var eventsPerSec = _systemInfo.Values.Sum(s => s.EventCount) / (_frameCount / 60f);
        
        SetMetric("AverageFrameInputTime", avgInputTime);
        SetMetric("MaxFrameInputTime", maxInputTime);
        SetMetric("MinFrameInputTime", minInputTime);
        SetMetric("EventsPerSecond", eventsPerSec);
        SetMetric("FrameCount", _frameCount);
        
        // Reset counters
        _totalInputTime = 0f;
        _frameCount = 0;
        
        // Reset event counts
        foreach (var info in _systemInfo.Values)
        {
            info.EventCount = 0;
        }
    }
}

public InputPerformanceMetrics GetPerformanceMetrics()
{
    lock (_lock)
    {
        var totalEvents = _systemInfo.Values.Sum(s => s.EventCount);
        var avgInputTime = _inputTimeHistory.Where(t => t > 0).DefaultIfEmpty().Average();
        var eventsPerSec = avgInputTime > 0 ? totalEvents / (_frameCount / 60f) : 0f;
        
        return new InputPerformanceMetrics
        {
            TotalInputTime = _totalInputTime,
            AverageInputTime = avgInputTime,
            MaxInputTime = _inputTimeHistory.Max(),
            MinInputTime = _inputTimeHistory.Where(t => t > 0).DefaultIfEmpty().Min(),
            FrameCount = _frameCount,
            InputTimeHistory = _inputTimeHistory.ToArray(),
            ActiveSystemCount = _inputSystems.Count(s => s.IsEnabled),
            DisabledSystemCount = _inputSystems.Count(s => !s.IsEnabled),
            EventsProcessed = totalEvents,
            DevicesConnected = _devices.Count,
            EventsPerSecond = eventsPerSec
        };
    }
}
```

---

## 🎯 **Input Integration Patterns**

### **1. IInputSystem Interface**
```csharp
/// <summary>
/// Interface for systems that require input processing.
/// Used by InputManager to coordinate input processing.
/// </summary>
public interface IInputSystem
{
    /// <summary>
    /// Processes input for the current frame.
    /// Called every frame by InputManager.
    /// </summary>
    /// <param name="inputState">The current input state.</param>
    /// <param name="deltaTime">Time since last frame in seconds.</param>
    void ProcessInput(InputState inputState, float deltaTime);
    
    /// <summary>
    /// Gets whether the system is enabled for input processing.
    /// Disabled systems are skipped during Update().
    /// </summary>
    bool IsEnabled { get; set; }
    
    /// <summary>
    /// Gets the input processing priority (lower numbers process first).
    /// Used to determine processing order.
    /// </summary>
    int InputPriority { get; }
    
    /// <summary>
    /// Gets the input devices supported by this system.
    /// </summary>
    IEnumerable<InputDeviceType> SupportedDevices { get; }
}
```

### **2. Input Device and Event Interfaces**
```csharp
/// <summary>
/// Interface for input devices.
/// </summary>
public interface IInputDevice : IDisposable
{
    InputDeviceType DeviceType { get; }
    bool IsConnected { get; }
    
    void Initialize();
    void Update(float deltaTime);
    InputState GetState();
}

/// <summary>
/// Interface for input event handlers.
/// </summary>
public interface IInputHandler
{
    void HandleInput(InputEvent inputEvent);
}

/// <summary>
/// Represents an input event.
/// </summary>
public class InputEvent
{
    public InputEventType EventType { get; set; }
    public InputDeviceType DeviceType { get; set; }
    public float Timestamp { get; set; }
    public Dictionary<string, object> Data { get; set; }
}
```

### **3. System Input Pattern Examples**
```csharp
// Example: Player Input System
public class PlayerInputSystem : IInputSystem
{
    public bool IsEnabled { get; set; } = true;
    public int InputPriority => 10; // Early in frame
    
    public IEnumerable<InputDeviceType> SupportedDevices => 
        new[] { InputDeviceType.Keyboard, InputDeviceType.Gamepad };
    
    public void ProcessInput(InputState inputState, float deltaTime)
    {
        // Process player movement
        var moveVector = inputState.GetVector(InputAction.Move);
        _playerController.SetMovement(moveVector);
        
        // Process actions
        if (inputState.IsPressed(InputAction.Jump))
            _playerController.Jump();
            
        if (inputState.IsPressed(InputAction.Fire))
            _playerController.Fire();
    }
}

// Example: UI Input System
public class UIInputSystem : IInputSystem
{
    public bool IsEnabled { get; set; } = true;
    public int InputPriority => 90; // Late in frame
    
    public IEnumerable<InputDeviceType> SupportedDevices => 
        new[] { InputDeviceType.Mouse, InputDeviceType.Keyboard };
    
    public void ProcessInput(InputState inputState, float deltaTime)
    {
        // Process UI interactions
        var mousePosition = inputState.GetVector(InputAction.MousePosition);
        var clickState = inputState.GetButtonState(InputAction.MouseClick);
        
        _uiSystem.ProcessInput(mousePosition, clickState);
    }
}
```

---

## 📁 **File Structure Requirements**

### **Create These Files:**
```
Engine/Core/Interfaces/
├── IInputSystem.cs              (Interface for input systems)
├── IInputDevice.cs             (Input device interface)
├── IInputHandler.cs            (Input event handler interface)

Engine/Core/Managers/
├── BaseManager.cs              (Already created)
├── SystemManager.cs            (Already created)
├── UpdateManager.cs            (Already created)
├── RenderManager.cs            (Already created)
└── InputManager.cs             (Input processing)

Engine/Core/
├── Input/
│   ├── InputState.cs           (Input state management)
│   ├── InputEvent.cs           (Input event definition)
│   ├── InputDevice.cs          (Base input device)
│   ├── KeyboardDevice.cs       (Keyboard implementation)
│   ├── MouseDevice.cs          (Mouse implementation)
│   └── GamepadDevice.cs        (Gamepad implementation)
├── Data/
    ├── InputSystemInfo.cs      (Input system information)
    └── InputPerformanceMetrics.cs (Input performance metrics)
└── Enums/
    ├── InputDeviceType.cs       (Input device types)
    ├── InputEventType.cs        (Input event types)
    └── InputAction.cs           (Input actions)
```

---

## 🔐 **Security and Performance Requirements**

### **Thread Safety:**
- **Input State:** Thread-safe input state tracking
- **Performance Metrics:** Thread-safe metric collection
- **System Management:** Thread-safe enable/disable operations
- **Event Processing:** Thread-safe event queue processing

### **Performance Considerations:**
- **Input Order:** Optimize priority-based input processing sequence
- **Event Processing:** Efficient event queue and routing
- **Error Handling:** Minimal performance impact from error handling
- **Memory Management:** Efficient data structures for input tracking

---

## 🚀 **Build Integration Requirements**

### **1. Namespace Consistency**
```csharp
namespace SASZombieAssaultTD.Engine.Core.Managers
{
    public class InputManager : BaseManager
    {
        // Implementation
    }
}
```

### **2. BaseManager Integration:**
- **Abstract Methods:** Implement GetSystemCount(), DoInitialize(), DoShutdown()
- **Logging:** Use BaseManager logging methods
- **Metrics:** Use BaseManager metric collection
- **Thread Safety:** Inherit BaseManager thread safety patterns

### **3. GameRoot Integration:**
- **Update Loop:** Integrate with GameRoot.Update() method
- **Delta Time:** Proper deltaTime parameter handling
- **Error Handling:** Graceful handling of input failures
- **Performance:** Efficient frame-by-frame input processing

---

## 📊 **Testing Requirements**

### **Unit Tests Needed:**
1. **Constructor Tests:** Test InputManager initialization
2. **DoInitialize() Tests:** Test system discovery and device setup
3. **DoShutdown() Tests:** Test cleanup operations
4. **Update() Tests:** Test input processing orchestration
5. **Priority Ordering Tests:** Test system input processing order
6. **Error Handling Tests:** Test system input processing failures
7. **Performance Tests:** Test input performance metrics collection

### **Integration Tests:**
1. **SystemRegistry Integration:** Test with actual SystemRegistry
2. **GameRoot Integration:** Test with actual GameRoot update loop
3. **System Integration:** Test with real input systems
4. **Device Integration:** Test with actual input devices
5. **Performance Tests:** Benchmark input performance

---

## 🎯 **Build Recommendations**

### **Phase 1: Core Implementation**
1. **Create IInputSystem.cs** - Interface for input systems
2. **Create IInputDevice.cs** - Input device interface
3. **Create InputState.cs** - Input state management
4. **Create InputSystemInfo.cs** - Input system information class
5. **Implement InputManager.cs** - Core functionality

### **Phase 2: Input Integration**
1. **Implement system discovery** - Find input systems
2. **Implement device management** - Input device coordination
3. **Implement event processing** - Input event routing
4. **Add performance tracking** - Monitor input performance

### **Phase 3: Testing and Optimization**
1. **Unit Tests** - Test all functionality
2. **Integration Tests** - Test with real systems and devices
3. **Performance Testing** - Benchmark input performance
4. **Error Handling Tests** - Test failure scenarios

---

## 🏆 **Success Criteria**

### **Functional Requirements:**
- ✅ **System Discovery** - Find all input systems
- **Input Orchestration** - Coordinate input processing
- **Priority Management** - Order systems by input priority
- **Device Management** - Manage input device connections
- **Event Processing** - Route input events to handlers
- **Error Isolation** - Handle system input failures independently
- **Performance Tracking** - Monitor input performance

### **Quality Requirements:**
- ✅ **Comprehensive documentation**
- ✅ **Unit test coverage**
- ✅ **Integration test validation**
- ✅ **Performance benchmarking**
- ✅ **Error handling robustness**

### **Architecture Requirements:**
- ✅ **BaseManager inheritance** - Follow established patterns
- ✅ **SystemRegistry integration** - Work with system registry
- ✅ **Thread safety** - Safe for concurrent access
- ✅ **GameRoot integration** - Perfect update loop integration

---

## 🎉 **Implementation Priority**

### **HIGH PRIORITY (Must Have):**
1. **InputManager class** - Core implementation
2. **IInputSystem interface** - Contract for input systems
3. **IInputDevice interface** - Input device contract
4. **InputState class** - Input state management
5. **System discovery** - Find and register input systems

### **MEDIUM PRIORITY (Should Have):**
1. **Priority ordering** - Proper input processing sequence
2. **Device management** - Input device coordination
3. **Event processing** - Input event routing
4. **Performance tracking** - Monitor input performance

### **LOW PRIORITY (Nice to Have):**
1. **Advanced device support** - Multiple device types
2. **Input mapping** - Configurable input mappings
3. **Input recording** - Input recording and playback
4. **Configuration support** - Configurable input behavior

---

## 📝 **Next Steps**

1. **Create IInputSystem.cs** - Interface for input systems
2. **Create IInputDevice.cs** - Input device interface
3. **Create InputState.cs** - Input state management
4. **Create supporting types** - InputSystemInfo, InputPerformanceMetrics
5. **Implement InputManager.cs** - Core functionality
6. **Test with mock systems** - Verify basic functionality
7. **Test with real systems** - Integration testing
8. **Test with GameRoot** - Full integration
9. **Run Build Worthiness Analysis** - Confirm production readiness

**This InputManager will provide robust input processing coordination for all engine systems and ensure proper input device management, event processing, and error handling across your rebuilt architecture.**
