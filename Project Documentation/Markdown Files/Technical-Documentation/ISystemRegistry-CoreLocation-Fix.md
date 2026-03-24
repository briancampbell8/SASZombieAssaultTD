# ISystemRegistry.cs Core Location Fix

## 📁 **DIRECTORY STRUCTURE CORRECTION**

**Target Location:** `Engine/Core/Interfaces/ISystemRegistry.cs`  
**Current Status:** File needs to be moved and namespace updated

---

## 🔧 **REQUIRED FIXES:**

### **1. Move File Location**
**From:** `Engine/Interfaces/ISystemRegistry.cs`  
**To:** `Engine/Core/Interfaces/ISystemRegistry.cs`

### **2. Update Namespace**
**Current Namespace:** `SASZombieAssaultTD.Engine.Interfaces`  
**Correct Namespace:** `SASZombieAssaultTD.Engine.Core.Interfaces`

### **3. Update Header Comment**
**Current PATH:** `Engine/Core/Interfaces/ISystemRegistry.cs`  
**Should match:** `Engine/Core/Interfaces/ISystemRegistry.cs` ✅ (already correct)

---

## 📝 **NAMESPACE FIX:**

```csharp
// Line 30 - Update namespace to match Core structure
namespace SASZombieAssaultTD.Engine.Core.Interfaces
{
    /// <summary>
    /// Provides a centralized registry for engine systems.
    /// Enables type-safe registration and resolution of systems
    /// used throughout the engine (ECS, Rendering, Input, Audio, etc.).
    /// </summary>
    public interface ISystemRegistry
    {
        // ... rest of interface remains the same
    }
}
```

---

## 🎯 **EXPECTED STRUCTURE:**

```
Engine/Core/
├── Interfaces/
│   ├── ISystemRegistry.cs          ✅ (Fixed namespace)
│   └── [Other core interfaces]
├── SystemRegistry.cs              (Implementation)
└── [Other core files]
```

---

## 🚀 **NEXT STEPS:**

1. **Move file** from `Engine/Interfaces/` to `Engine/Core/Interfaces/`
2. **Update namespace** to `SASZombieAssaultTD.Engine.Core.Interfaces`
3. **Verify compilation** after namespace fix
4. **Run Build Worthiness Analysis** to confirm it's production-ready

---

## 🎉 **RESULT:**

After these fixes, ISystemRegistry.cs will be:
- ✅ **Correctly located** in `Engine/Core/Interfaces/`
- ✅ **Properly namespaced** as `Engine.Core.Interfaces`
- ✅ **Production-ready** with perfect interface design
- ✅ **Consistent** with your Core architecture structure

**Ready for Build Worthiness Analysis once the fixes are applied!**
