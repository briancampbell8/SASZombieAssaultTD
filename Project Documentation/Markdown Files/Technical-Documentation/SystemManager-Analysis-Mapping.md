# SystemManager.cs Analysis and Build Mapping

## 📊 System Analysis Overview

**File:** `Engine/Core/Managers/SystemManager.cs`  
**Purpose:** Concrete implementation of BaseManager for system lifecycle management  
**Role:** Manages initialization, shutdown, and lifecycle of all engine systems

---

## 🎯 **System Flow Analysis**

### **Integration Points in Engine Architecture:**

#### **1. GameRoot.cs SystemManager Integration**
```csharp
// GameRoot constructor - SystemManager injected
public GameRoot(
    ISystemRegistry systemRegistry,
    SystemManager systemManager,    // : BaseManager
    UpdateManager updateManager,
    RenderManager renderManager,
    InputManager inputManager)
{
    _systemManager = systemManager;
}

// GameRoot initialization
public void Initialize()
{
    _systemManager.Initialize();    // Initialize all systems
    _updateManager.Initialize();
    _renderManager.Initialize();
    _inputManager.Initialize();
}

// GameRoot shutdown
public void Stop()
{
    _systemManager.Shutdown();      // Shutdown all systems
    _updateManager.Shutdown();
    _renderManager.Shutdown();
    _inputManager.Shutdown();
}
```

#### **2. System Lifecycle Management**
```csharp
// SystemManager responsibilities
- Initialize all registered systems in correct order
- Shutdown all systems in reverse order
- Track system states and dependencies
- Handle system initialization failures gracefully
- Provide system diagnostics and metrics
```

#### **3. SystemRegistry Integration**
```csharp
// SystemManager uses SystemRegistry to find and manage systems
var systems = _registry.GetRegisteredTypes();
foreach (var systemType in systems)
{
    var system = _registry.GetSystem(systemType);
    if (system is IManagedSystem managedSystem)
    {
        managedSystem.Initialize();
        _managedSystems.Add(managedSystem);
    }
}
```

---

## 🏗 **Required Implementation Structure**

### **Core SystemManager Design:**
```csharp
namespace SASZombieAssaultTD.Engine.Core.Managers
{
    /// <summary>
    /// Manages the lifecycle of all engine systems.
    /// Handles initialization, shutdown, and state tracking
    /// for systems registered in the SystemRegistry.
    /// </summary>
    public class SystemManager : BaseManager
    {
        private readonly List<IManagedSystem> _managedSystems = new();
        private readonly Dictionary<Type, SystemInfo> _systemInfo = new();
        
        // Constructor
        public SystemManager(ISystemRegistry registry) 
            : base("SystemManager", registry)
        {
        }
        
        // BaseManager abstract method implementations
        protected override int GetSystemCount() => _managedSystems.Count;
        
        protected override void DoInitialize()
        {
            // Initialize all registered systems
            InitializeAllSystems();
        }
        
        protected override void DoShutdown()
        {
            // Shutdown all managed systems
            ShutdownAllSystems();
        }
        
        // System-specific methods
        public void InitializeAllSystems()
        public void ShutdownAllSystems()
        public SystemInfo? GetSystemInfo<T>() where T : class
        public IEnumerable<SystemInfo> GetAllSystemInfo()
        public bool IsSystemInitialized<T>() where T : class
    }
}
```

### **Supporting Types:**
```csharp
/// <summary>
/// Information about a managed system.
/// </summary>
public class SystemInfo
{
    public Type SystemType { get; set; }
    public string Name { get; set; }
    public SystemStatus Status { get; set; }
    public DateTime InitializationTime { get; set; }
    public TimeSpan InitializationDuration { get; set; }
    public string? ErrorMessage { get; set; }
    public Dictionary<string, object> Metrics { get; set; }
}

/// <summary>
/// Status of a managed system.
/// </summary>
public enum SystemStatus
{
    Uninitialized,
    Initializing,
    Initialized,
    ShuttingDown,
    Shutdown,
    Error
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
- **ISystemRegistry** - For system resolution and registration
- **IManagedSystem** - Interface for systems that can be managed
- **SystemInfo** - Class for system diagnostic information
- **SystemStatus** - Enum for system state tracking

---

## 🔧 **Implementation Requirements**

### **1. System Discovery and Registration**
```csharp
protected override void DoInitialize()
{
    lock (_lock)
    {
        LogInfo("Starting system initialization...");
        
        // Discover all registered systems
        var registeredTypes = _registry.GetRegisteredTypes().ToList();
        SetMetric("RegisteredSystemCount", registeredTypes.Count);
        
        // Initialize systems in dependency order
        var initializationOrder = DetermineInitializationOrder(registeredTypes);
        
        foreach (var systemType in initializationOrder)
        {
            InitializeSystem(systemType);
        }
        
        LogInfo($"Initialized {_managedSystems.Count} systems successfully.");
        SetMetric("InitializedSystemCount", _managedSystems.Count);
    }
}

private void InitializeSystem(Type systemType)
{
    try
    {
        var system = _registry.GetSystem(systemType);
        if (system == null)
        {
            LogWarning($"System {systemType.Name} not found in registry.");
            return;
        }
        
        if (system is IManagedSystem managedSystem)
        {
            var startTime = DateTime.Now;
            
            _systemInfo[systemType] = new SystemInfo
            {
                SystemType = systemType,
                Name = systemType.Name,
                Status = SystemStatus.Initializing,
                InitializationTime = startTime
            };
            
            managedSystem.Initialize();
            
            var duration = DateTime.Now - startTime;
            _systemInfo[systemType].Status = SystemStatus.Initialized;
            _systemInfo[systemType].InitializationDuration = duration;
            
            _managedSystems.Add(managedSystem);
            
            LogInfo($"Initialized system {systemType.Name} in {duration.TotalMilliseconds:F2}ms");
        }
        else
        {
            LogInfo($"System {systemType.Name} does not implement IManagedSystem - skipping lifecycle management.");
        }
    }
    catch (Exception ex)
    {
        _systemInfo[systemType].Status = SystemStatus.Error;
        _systemInfo[systemType].ErrorMessage = ex.Message;
        
        LogError($"Failed to initialize system {systemType.Name}: {ex.Message}", ex);
        throw new InvalidOperationException($"System initialization failed: {systemType.Name}", ex);
    }
}
```

### **2. System Shutdown Management**
```csharp
protected override void DoShutdown()
{
    lock (_lock)
    {
        LogInfo("Starting system shutdown...");
        
        // Shutdown in reverse initialization order
        for (int i = _managedSystems.Count - 1; i >= 0; i--)
        {
            try
            {
                var system = _managedSystems[i];
                var systemType = system.GetType();
                
                _systemInfo[systemType].Status = SystemStatus.ShuttingDown;
                
                var startTime = DateTime.Now;
                system.Shutdown();
                var duration = DateTime.Now - startTime;
                
                _systemInfo[systemType].Status = SystemStatus.Shutdown;
                
                LogInfo($"Shutdown system {systemType.Name} in {duration.TotalMilliseconds:F2}ms");
            }
            catch (Exception ex)
            {
                var systemType = _managedSystems[i].GetType();
                _systemInfo[systemType].Status = SystemStatus.Error;
                _systemInfo[systemType].ErrorMessage = ex.Message;
                
                LogError($"Failed to shutdown system {systemType.Name}: {ex.Message}", ex);
            }
        }
        
        _managedSystems.Clear();
        LogInfo("Shutdown all systems completed.");
    }
}
```

### **3. Dependency Order Resolution**
```csharp
private List<Type> DetermineInitializationOrder(List<Type> systemTypes)
{
    // Simple dependency resolution - can be enhanced with actual dependency attributes
    var ordered = new List<Type>();
    var remaining = new HashSet<Type>(systemTypes);
    
    // Core systems first
    var coreOrder = new[]
    {
        typeof(EventBus),
        typeof(AssetManager),
        typeof(SaveManager),
        typeof(ECSWorld)
    };
    
    foreach (var coreType in coreOrder)
    {
        if (remaining.Contains(coreType))
        {
            ordered.Add(coreType);
            remaining.Remove(coreType);
        }
    }
    
    // Add remaining systems in alphabetical order (can be improved)
    ordered.AddRange(remaining.OrderBy(t => t.Name));
    
    return ordered;
}
```

### **4. System Diagnostics and Monitoring**
```csharp
public SystemInfo? GetSystemInfo<T>() where T : class
{
    lock (_lock)
    {
        return _systemInfo.TryGetValue(typeof(T), out var info) ? info : null;
    }
}

public IEnumerable<SystemInfo> GetAllSystemInfo()
{
    lock (_lock)
    {
        return _systemInfo.Values.ToList();
    }
}

public bool IsSystemInitialized<T>() where T : class
{
    lock (_lock)
    {
        return _systemInfo.TryGetValue(typeof(T), out var info) && 
               info.Status == SystemStatus.Initialized;
    }
}

public override ManagerDiagnostics GetDiagnostics()
{
    lock (_lock)
    {
        var baseDiagnostics = base.GetDiagnostics();
        
        // Add system-specific metrics
        baseDiagnostics.Metrics["InitializedSystems"] = _managedSystems.Count;
        baseDiagnostics.Metrics["FailedSystems"] = _systemInfo.Values.Count(s => s.Status == SystemStatus.Error);
        baseDiagnostics.Metrics["AverageInitTime"] = _systemInfo.Values
            .Where(s => s.Status == SystemStatus.Initialized)
            .DefaultIfEmpty()
            .Average(s => s.InitializationDuration.TotalMilliseconds);
        
        return baseDiagnostics;
    }
}
```

---

## 🎯 **System Integration Patterns**

### **1. IManagedSystem Interface**
```csharp
/// <summary>
/// Interface for systems that can be managed by SystemManager.
/// </summary>
public interface IManagedSystem
{
    /// <summary>
    /// Initializes the system.
    /// Called once during engine startup.
    /// </summary>
    void Initialize();
    
    /// <summary>
    /// Shuts down the system.
    /// Called once during engine shutdown.
    /// </summary>
    void Shutdown();
    
    /// <summary>
    /// Gets the current status of the system.
    /// </summary>
    SystemStatus Status { get; }
}
```

### **2. System Registration Pattern**
```csharp
// During engine setup
var registry = new SystemRegistry();
registry.RegisterSystem(new EventBus());
registry.RegisterSystem(new AssetManager());
registry.RegisterSystem(new SaveManager());
registry.RegisterSystem(new ECSWorld());
registry.RegisterSystem(new PhysicsSystem());
// ... all 221+ systems

// SystemManager will discover and manage all registered systems
var systemManager = new SystemManager(registry);
systemManager.Initialize(); // Initializes all systems
```

---

## 📁 **File Structure Requirements**

### **Create These Files:**
```
Engine/Core/Interfaces/
├── IManagedSystem.cs           (Interface for managed systems)

Engine/Core/Managers/
├── BaseManager.cs              (Already created)
├── SystemManager.cs            (System lifecycle management)
├── UpdateManager.cs            (Update orchestration)
├── RenderManager.cs            (Render orchestration)
└── InputManager.cs             (Input processing)

Engine/Core/
├── Enums/
    ├── SystemStatus.cs         (System status enum)
    └── SystemInfo.cs            (System information class)
```

---

## 🔐 **Security and Performance Requirements**

### **Thread Safety:**
- **System State:** Thread-safe system status tracking
- **Initialization:** Atomic system initialization
- **Shutdown:** Graceful shutdown with error isolation
- **Diagnostics:** Thread-safe diagnostic information

### **Performance Considerations:**
- **Initialization Order:** Optimize for system dependencies
- **Error Isolation:** Prevent one system failure from affecting others
- **Metrics Overhead:** Minimal performance impact from monitoring
- **Memory Management:** Proper cleanup during shutdown

---

## 🚀 **Build Integration Requirements**

### **1. Namespace Consistency**
```csharp
namespace SASZombieAssaultTD.Engine.Core.Managers
{
    public class SystemManager : BaseManager
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

### **3. SystemRegistry Integration:**
- **System Discovery:** Use GetRegisteredTypes() to find systems
- **System Resolution:** Use GetSystem<T>() to get system instances
- **Type Safety:** Proper generic type handling
- **Error Handling:** Handle missing systems gracefully

---

## 📊 **Testing Requirements**

### **Unit Tests Needed:**
1. **Constructor Tests:** Test SystemManager initialization
2. **DoInitialize() Tests:** Test system initialization sequence
3. **DoShutdown() Tests:** Test system shutdown sequence
4. **SystemInfo Tests:** Test system information tracking
5. **Error Handling Tests:** Test system failure scenarios
6. **Dependency Order Tests:** Test initialization order
7. **Diagnostics Tests:** Test diagnostic information accuracy

### **Integration Tests:**
1. **SystemRegistry Integration:** Test with actual SystemRegistry
2. **GameRoot Integration:** Test with actual GameRoot
3. **System Integration:** Test with real system implementations
4. **Performance Tests:** Benchmark initialization performance
5. **Error Recovery Tests:** Test error isolation and recovery

---

## 🎯 **Build Recommendations**

### **Phase 1: Core Implementation**
1. **Create IManagedSystem.cs** - Interface for managed systems
2. **Create SystemInfo.cs** - System information class
3. **Create SystemStatus.cs** - System status enum
4. **Implement SystemManager.cs** - Core functionality

### **Phase 2: System Integration**
1. **Implement system discovery** - Find registered systems
2. **Implement initialization order** - Dependency resolution
3. **Implement shutdown sequence** - Reverse order shutdown
4. **Add diagnostics support** - System tracking and metrics

### **Phase 3: Testing and Optimization**
1. **Unit Tests** - Test all functionality
2. **Integration Tests** - Test with real systems
3. **Performance Testing** - Benchmark initialization
4. **Error Handling Tests** - Test failure scenarios

---

## 🏆 **Success Criteria**

### **Functional Requirements:**
- ✅ **System Discovery** - Find all registered systems
- ✅ **Initialization Management** - Initialize systems in correct order
- ✅ **Shutdown Management** - Shutdown systems gracefully
- ✅ **Error Isolation** - Handle system failures independently
- ✅ **Diagnostics** - Track system states and performance

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
- ✅ **Extensibility** - Easy to add new system types

---

## 🎉 **Implementation Priority**

### **HIGH PRIORITY (Must Have):**
1. **SystemManager class** - Core implementation
2. **IManagedSystem interface** - Contract for managed systems
3. **System discovery** - Find and register systems
4. **Initialization/Shutdown** - Core lifecycle management

### **MEDIUM PRIORITY (Should Have):**
1. **SystemInfo tracking** - Diagnostic information
2. **Dependency order** - Proper initialization sequence
3. **Error isolation** - Handle system failures gracefully
4. **Performance metrics** - Track initialization performance

### **LOW PRIORITY (Nice to Have):**
1. **Advanced diagnostics** - Detailed system information
2. **Dependency attributes** - Declarative dependency specification
3. **System events** - Publish system lifecycle events
4. **Configuration support** - Configurable system behavior

---

## 📝 **Next Steps**

1. **Create IManagedSystem.cs** - Interface for managed systems
2. **Create supporting types** - SystemInfo, SystemStatus
3. **Implement SystemManager.cs** - Core functionality
4. **Test with mock systems** - Verify basic functionality
5. **Test with real systems** - Integration testing
6. **Test with GameRoot** - Full integration
7. **Run Build Worthiness Analysis** - Confirm production readiness

**This SystemManager will provide robust lifecycle management for all engine systems and ensure proper initialization, shutdown, and error handling across your rebuilt architecture.**
