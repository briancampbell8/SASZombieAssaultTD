# UpdateManager.cs Analysis and Build Mapping

## 📊 System Analysis Overview

**File:** `Engine/Core/Managers/UpdateManager.cs`  
**Purpose:** Concrete implementation of BaseManager for per-frame update orchestration  
**Role:** Manages update loop coordination for all engine systems requiring per-frame updates

---

## 🎯 **System Flow Analysis**

### **Integration Points in Engine Architecture:**

#### **1. GameRoot.cs UpdateManager Integration**
```csharp
// GameRoot constructor - UpdateManager injected
public GameRoot(
    ISystemRegistry systemRegistry,
    SystemManager systemManager,
    UpdateManager updateManager,    // : BaseManager
    RenderManager renderManager,
    InputManager inputManager)
{
    _updateManager = updateManager;
}

// GameRoot update loop
public void Update(float deltaTime)
{
    if (!_isInitialized)
        return;
        
    _stateMachine.Update(deltaTime);
    _updateManager.UpdateAll(deltaTime);    // Update all updatable systems
    _inputManager.Update(deltaTime);
}

// GameRoot initialization/shutdown
public void Initialize()
{
    _systemManager.Initialize();
    _updateManager.Initialize();    // Initialize update manager
    _renderManager.Initialize();
    _inputManager.Initialize();
}

public void Stop()
{
    _systemManager.Shutdown();
    _updateManager.Shutdown();      // Shutdown update manager
    _renderManager.Shutdown();
    _inputManager.Shutdown();
}
```

#### **2. Update Loop Orchestration**
```csharp
// UpdateManager responsibilities
- Discover all systems implementing IUpdatableSystem
- Coordinate per-frame updates in priority order
- Track update performance and metrics
- Handle update failures gracefully
- Provide update diagnostics and profiling
- Manage update enable/disable states
```

#### **3. SystemRegistry Integration**
```csharp
// UpdateManager uses SystemRegistry to find updatable systems
var systems = _registry.GetRegisteredTypes();
foreach (var systemType in systems)
{
    var system = _registry.GetSystem(systemType);
    if (system is IUpdatableSystem updatableSystem)
    {
        _updatableSystems.Add(updatableSystem);
    }
}
```

---

## 🏗 **Required Implementation Structure**

### **Core UpdateManager Design:**
```csharp
namespace SASZombieAssaultTD.Engine.Core.Managers
{
    /// <summary>
    /// Manages the per-frame update loop for all engine systems.
    /// Handles update orchestration, performance tracking, and
    /// error isolation for systems requiring per-frame updates.
    /// </summary>
    public class UpdateManager : BaseManager
    {
        private readonly List<IUpdatableSystem> _updatableSystems = new();
        private readonly Dictionary<Type, UpdateSystemInfo> _systemInfo = new();
        private readonly PriorityQueue<IUpdatableSystem, int> _updateQueue = new();
        
        // Performance tracking
        private float _totalUpdateTime = 0f;
        private int _frameCount = 0;
        private readonly float[] _updateTimeHistory = new float[60]; // 1 second at 60fps
        
        // Constructor
        public UpdateManager(ISystemRegistry registry) 
            : base("UpdateManager", registry)
        {
        }
        
        // BaseManager abstract method implementations
        protected override int GetSystemCount() => _updatableSystems.Count;
        
        protected override void DoInitialize()
        {
            // Discover all updatable systems
            DiscoverUpdatableSystems();
            
            // Sort systems by update priority
            SortSystemsByPriority();
            
            LogInfo($"Discovered {_updatableSystems.Count} updatable systems");
        }
        
        protected override void DoShutdown()
        {
            // Clear all updatable systems
            _updatableSystems.Clear();
            _systemInfo.Clear();
            _updateQueue.Clear();
            
            LogInfo("Shutdown update manager");
        }
        
        // Update-specific methods
        public void UpdateAll(float deltaTime)
        public void EnableSystem<T>() where T : class
        public void DisableSystem<T>() where T : class
        public UpdateSystemInfo? GetSystemInfo<T>() where T : class
        public IEnumerable<UpdateSystemInfo> GetAllSystemInfo()
        public UpdatePerformanceMetrics GetPerformanceMetrics()
    }
}
```

### **Supporting Types:**
```csharp
/// <summary>
/// Information about an updatable system.
/// </summary>
public class UpdateSystemInfo
{
    public Type SystemType { get; set; }
    public string Name { get; set; }
    public bool IsEnabled { get; set; }
    public int UpdatePriority { get; set; }
    public float AverageUpdateTime { get; set; }
    public float LastUpdateTime { get; set; }
    public int UpdateCount { get; set; }
    public int ErrorCount { get; set; }
    public string? LastError { get; set; }
    public DateTime LastUpdateTimeStamp { get; set; }
}

/// <summary>
/// Performance metrics for the update manager.
/// </summary>
public class UpdatePerformanceMetrics
{
    public float TotalUpdateTime { get; set; }
    public float AverageUpdateTime { get; set; }
    public float MaxUpdateTime { get; set; }
    public float MinUpdateTime { get; set; }
    public int FrameCount { get; set; }
    public float[] UpdateTimeHistory { get; set; }
    public int ActiveSystemCount { get; set; }
    public int DisabledSystemCount { get; set; }
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
- **IUpdatableSystem** - Interface for systems requiring updates
- **UpdateSystemInfo** - Class for update diagnostic information
- **UpdatePerformanceMetrics** - Class for performance tracking

---

## 🔧 **Implementation Requirements**

### **1. Updatable System Discovery**
```csharp
private void DiscoverUpdatableSystems()
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
                
                if (system is IUpdatableSystem updatableSystem)
                {
                    _systemInfo[systemType] = new UpdateSystemInfo
                    {
                        SystemType = systemType,
                        Name = systemType.Name,
                        IsEnabled = updatableSystem.IsEnabled,
                        UpdatePriority = updatableSystem.UpdatePriority,
                        LastUpdateTimeStamp = DateTime.Now
                    };
                    
                    _updatableSystems.Add(updatableSystem);
                    
                    LogInfo($"Discovered updatable system {systemType.Name} with priority {updatableSystem.UpdatePriority}");
                }
            }
            catch (Exception ex)
            {
                LogError($"Failed to discover system {systemType.Name}: {ex.Message}", ex);
            }
        }
        
        SetMetric("UpdatableSystemCount", _updatableSystems.Count);
    }
}
```

### **2. Priority-Based Update Orchestration**
```csharp
private void SortSystemsByPriority()
{
    lock (_lock)
    {
        // Sort by update priority (lower numbers update first)
        _updatableSystems.Sort((a, b) => a.UpdatePriority.CompareTo(b.UpdatePriority));
        
        // Rebuild priority queue
        _updateQueue.Clear();
        foreach (var system in _updatableSystems)
        {
            _updateQueue.Enqueue(system, system.UpdatePriority);
        }
        
        LogInfo("Sorted updatable systems by priority");
    }
}

public void UpdateAll(float deltaTime)
{
    if (Status != ManagerStatus.Running)
        return;
        
    lock (_lock)
    {
        var frameStartTime = DateTime.Now;
        
        // Update all enabled systems in priority order
        foreach (var system in _updatableSystems)
        {
            if (!system.IsEnabled)
                continue;
                
            var systemType = system.GetType();
            var systemInfo = _systemInfo[systemType];
            var updateStartTime = DateTime.Now;
            
            try
            {
                system.Update(deltaTime);
                
                var updateTime = (float)(DateTime.Now - updateStartTime).TotalMilliseconds;
                systemInfo.LastUpdateTime = updateTime;
                systemInfo.UpdateCount++;
                systemInfo.LastUpdateTimeStamp = DateTime.Now;
                
                // Update running average
                systemInfo.AverageUpdateTime = 
                    (systemInfo.AverageUpdateTime * (systemInfo.UpdateCount - 1) + updateTime) / systemInfo.UpdateCount;
            }
            catch (Exception ex)
            {
                systemInfo.ErrorCount++;
                systemInfo.LastError = ex.Message;
                
                LogError($"System {systemType.Name} update failed: {ex.Message}", ex);
                
                // Optionally disable system on repeated errors
                if (systemInfo.ErrorCount > 10)
                {
                    system.IsEnabled = false;
                    systemInfo.IsEnabled = false;
                    LogWarning($"Disabling system {systemType.Name} due to repeated errors");
                }
            }
        }
        
        // Track frame performance
        var frameUpdateTime = (float)(DateTime.Now - frameStartTime).TotalMilliseconds;
        UpdatePerformanceMetrics(frameUpdateTime);
        
        UpdateLastActivity();
    }
}
```

### **3. Performance Tracking and Metrics**
```csharp
private void UpdatePerformanceMetrics(float frameUpdateTime)
{
    _totalUpdateTime += frameUpdateTime;
    _frameCount++;
    
    // Update rolling history (last 60 frames)
    _updateTimeHistory[_frameCount % 60] = frameUpdateTime;
    
    // Update metrics every 60 frames (approximately 1 second)
    if (_frameCount % 60 == 0)
    {
        var avgUpdateTime = _totalUpdateTime / _frameCount;
        var maxUpdateTime = _updateTimeHistory.Max();
        var minUpdateTime = _updateTimeHistory.Where(t => t > 0).Min();
        
        SetMetric("AverageFrameUpdateTime", avgUpdateTime);
        SetMetric("MaxFrameUpdateTime", maxUpdateTime);
        SetMetric("MinFrameUpdateTime", minUpdateTime);
        SetMetric("FrameCount", _frameCount);
        
        // Reset counters
        _totalUpdateTime = 0f;
        _frameCount = 0;
    }
}

public UpdatePerformanceMetrics GetPerformanceMetrics()
{
    lock (_lock)
    {
        return new UpdatePerformanceMetrics
        {
            TotalUpdateTime = _totalUpdateTime,
            AverageUpdateTime = _updateTimeHistory.Where(t => t > 0).DefaultIfEmpty().Average(),
            MaxUpdateTime = _updateTimeHistory.Max(),
            MinUpdateTime = _updateTimeHistory.Where(t => t > 0).DefaultIfEmpty().Min(),
            FrameCount = _frameCount,
            UpdateTimeHistory = _updateTimeHistory.ToArray(),
            ActiveSystemCount = _updatableSystems.Count(s => s.IsEnabled),
            DisabledSystemCount = _updatableSystems.Count(s => !s.IsEnabled)
        };
    }
}
```

### **4. System Management Methods**
```csharp
public void EnableSystem<T>() where T : class
{
    lock (_lock)
    {
        var systemType = typeof(T);
        if (_systemInfo.TryGetValue(systemType, out var info))
        {
            var system = _updatableSystems.FirstOrDefault(s => s.GetType() == systemType);
            if (system != null)
            {
                system.IsEnabled = true;
                info.IsEnabled = true;
                LogInfo($"Enabled updatable system {systemType.Name}");
            }
        }
    }
}

public void DisableSystem<T>() where T : class
{
    lock (_lock)
    {
        var systemType = typeof(T);
        if (_systemInfo.TryGetValue(systemType, out var info))
        {
            var system = _updatableSystems.FirstOrDefault(s => s.GetType() == systemType);
            if (system != null)
            {
                system.IsEnabled = false;
                info.IsEnabled = false;
                LogInfo($"Disabled updatable system {systemType.Name}");
            }
        }
    }
}

public UpdateSystemInfo? GetSystemInfo<T>() where T : class
{
    lock (_lock)
    {
        var systemType = typeof(T);
        return _systemInfo.TryGetValue(systemType, out var info) ? info : null;
    }
}

public IEnumerable<UpdateSystemInfo> GetAllSystemInfo()
{
    lock (_lock)
    {
        return _systemInfo.Values.ToList();
    }
}
```

---

## 🎯 **Update Integration Patterns**

### **1. IUpdatableSystem Interface**
```csharp
/// <summary>
/// Interface for systems that require per-frame updates.
/// Used by UpdateManager to coordinate system updates.
/// </summary>
public interface IUpdatableSystem
{
    /// <summary>
    /// Updates the system for the current frame.
    /// Called every frame by UpdateManager.
    /// </summary>
    /// <param name="deltaTime">Time since last frame in seconds.</param>
    void Update(float deltaTime);
    
    /// <summary>
    /// Gets whether the system is enabled for updates.
    /// Disabled systems are skipped during UpdateAll().
    /// </summary>
    bool IsEnabled { get; set; }
    
    /// <summary>
    /// Gets the update priority (lower numbers update first).
    /// Used to determine update order.
    /// </summary>
    int UpdatePriority { get; }
}
```

### **2. System Update Pattern Examples**
```csharp
// Example: Physics System
public class PhysicsSystem : IUpdatableSystem
{
    public bool IsEnabled { get; set; } = true;
    public int UpdatePriority => 10; // Early in frame
    
    public void Update(float deltaTime)
    {
        // Update physics simulation
        _world.Step(deltaTime);
        ProcessCollisions();
        UpdateTransforms();
    }
}

// Example: Animation System
public class AnimationSystem : IUpdatableSystem
{
    public bool IsEnabled { get; set; } = true;
    public int UpdatePriority => 50; // Mid-frame
    
    public void Update(float deltaTime)
    {
        // Update animations
        foreach (var animator in _animators)
        {
            animator.Update(deltaTime);
        }
    }
}

// Example: Audio System
public class AudioSystem : IUpdatableSystem
{
    public bool IsEnabled { get; set; } = true;
    public int UpdatePriority => 90; // Late in frame
    
    public void Update(float deltaTime)
    {
        // Update audio playback
        _audioEngine.Update(deltaTime);
        ProcessAudioEvents();
    }
}
```

---

## 📁 **File Structure Requirements**

### **Create These Files:**
```
Engine/Core/Interfaces/
├── IUpdatableSystem.cs         (Interface for updatable systems)

Engine/Core/Managers/
├── BaseManager.cs              (Already created)
├── SystemManager.cs            (Already created)
├── UpdateManager.cs            (Update orchestration)
├── RenderManager.cs            (Render orchestration)
└── InputManager.cs             (Input processing)

Engine/Core/
├── Data/
    ├── UpdateSystemInfo.cs     (Update system information)
    └── UpdatePerformanceMetrics.cs (Performance metrics)
```

---

## 🔐 **Security and Performance Requirements**

### **Thread Safety:**
- **Update State:** Thread-safe system state tracking
- **Performance Metrics:** Thread-safe metric collection
- **System Management:** Thread-safe enable/disable operations
- **Error Isolation:** Prevent one system failure from affecting others

### **Performance Considerations:**
- **Update Order:** Optimize priority-based update sequence
- **Error Handling:** Minimal performance impact from error handling
- **Metrics Overhead:** Lightweight performance tracking
- **Memory Management:** Efficient data structures for update tracking

---

## 🚀 **Build Integration Requirements**

### **1. Namespace Consistency**
```csharp
namespace SASZombieAssaultTD.Engine.Core.Managers
{
    public class UpdateManager : BaseManager
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
- **Error Handling:** Graceful handling of update failures
- **Performance:** Efficient frame-by-frame execution

---

## 📊 **Testing Requirements**

### **Unit Tests Needed:**
1. **Constructor Tests:** Test UpdateManager initialization
2. **DoInitialize() Tests:** Test system discovery and sorting
3. **DoShutdown() Tests:** Test cleanup operations
4. **UpdateAll() Tests:** Test update orchestration
5. **Priority Ordering Tests:** Test system update order
6. **Error Handling Tests:** Test system update failures
7. **Performance Tests:** Test performance metrics collection

### **Integration Tests:**
1. **SystemRegistry Integration:** Test with actual SystemRegistry
2. **GameRoot Integration:** Test with actual GameRoot update loop
3. **System Integration:** Test with real updatable systems
4. **Performance Tests:** Benchmark update performance
5. **Error Recovery Tests:** Test error isolation and recovery

---

## 🎯 **Build Recommendations**

### **Phase 1: Core Implementation**
1. **Create IUpdatableSystem.cs** - Interface for updatable systems
2. **Create UpdateSystemInfo.cs** - Update system information class
3. **Create UpdatePerformanceMetrics.cs** - Performance metrics class
4. **Implement UpdateManager.cs** - Core functionality

### **Phase 2: Update Integration**
1. **Implement system discovery** - Find updatable systems
2. **Implement priority ordering** - Sort systems by update priority
3. **Implement update orchestration** - Coordinate per-frame updates
4. **Add performance tracking** - Monitor update performance

### **Phase 3: Testing and Optimization**
1. **Unit Tests** - Test all functionality
2. **Integration Tests** - Test with real systems
3. **Performance Testing** - Benchmark update performance
4. **Error Handling Tests** - Test failure scenarios

---

## 🏆 **Success Criteria**

### **Functional Requirements:**
- ✅ **System Discovery** - Find all updatable systems
- ✅ **Update Orchestration** - Coordinate per-frame updates
- ✅ **Priority Management** - Order systems by update priority
- ✅ **Error Isolation** - Handle system failures independently
- ✅ **Performance Tracking** - Monitor update performance

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
1. **UpdateManager class** - Core implementation
2. **IUpdatableSystem interface** - Contract for updatable systems
3. **System discovery** - Find and register updatable systems
4. **Update orchestration** - Coordinate per-frame updates

### **MEDIUM PRIORITY (Should Have):**
1. **Priority ordering** - Proper update sequence
2. **Performance tracking** - Monitor update performance
3. **Error isolation** - Handle system failures gracefully
4. **System management** - Enable/disable systems

### **LOW PRIORITY (Nice to Have):**
1. **Advanced diagnostics** - Detailed performance metrics
2. **Dynamic priority** - Runtime priority adjustment
3. **Update profiling** - Per-system performance profiling
4. **Configuration support** - Configurable update behavior

---

## 📝 **Next Steps**

1. **Create IUpdatableSystem.cs** - Interface for updatable systems
2. **Create supporting types** - UpdateSystemInfo, UpdatePerformanceMetrics
3. **Implement UpdateManager.cs** - Core functionality
4. **Test with mock systems** - Verify basic functionality
5. **Test with real systems** - Integration testing
6. **Test with GameRoot** - Full integration
7. **Run Build Worthiness Analysis** - Confirm production readiness

**This UpdateManager will provide robust update loop coordination for all engine systems and ensure proper per-frame updates, performance tracking, and error handling across your rebuilt architecture.**
