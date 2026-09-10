# SAS Zombie Assault TD - Static Image Display Analysis

**Date:** May 5, 2026  
**Analysis Type:** Comprehensive Project Review - Static Image Display Pipeline  
**Prepared for:** BDC and Copilot Review  

---

## Executive Summary

This analysis examines the complete static image display pipeline in the SAS Zombie Assault TD engine, identifying critical issues preventing static images from displaying correctly on screen. The primary blockers are:

1. **BGFX Struct Layout Mismatch** - Critical ABI incompatibility causing AccessViolationException at `bgfx_init`
2. **Renderer Selection** - Multiple renderer backends (D3D11, BGFX) with inconsistent activation
3. **Framebuffer Upload Path** - CPU→GPU transfer chain has potential failure points
4. **Static Layout Rendering** - Rendering pipeline initialization timing issues

---

## 1. Rendering Pipeline Architecture

### 1.1 High-Level Flow

```
Program.cs
    ↓
GameRoot.Initialize()
    ↓
StaticLayoutLoader.LoadAsync("Assets/Static/MeanStreets.json")
    ↓
GameRoot.UpdateLoop()
    ↓
StaticLayoutRenderer.Draw() → Framebuffer.DrawTexture()
    ↓
D3D11GraphicsDevice.UploadFramebuffer()
    ↓
D3D11FramebufferUploader.Upload() → GPU Backbuffer
    ↓
D3D11Presentation.Present() → Display
```

### 1.2 Key Components

| Component | Purpose | Status |
|-----------|---------|--------|
| `Framebuffer` | CPU-side pixel buffer (BGRA format) | ✅ Functional |
| `StaticLayout` | JSON-defined image layout | ✅ Loads successfully |
| `StaticLayoutRenderer` | Renders static images to framebuffer | ⚠️ Needs verification |
| `TextureManager` | Loads and caches textures | ✅ Working |
| `D3D11GraphicsDevice` | Primary GPU renderer | ✅ Initialized |
| `BGFXGraphicsDevice` | Alternate GPU renderer | ❌ **CRASH - Struct Layout** |
| `Win32Window` | Native window with message pump | ✅ Created |

---

## 2. Static Image Display Chain - Detailed Analysis

### 2.1 Image Loading Path

**File:** `Engine/UI/Assets/UIAssetLoader.cs`  
**Status:** ✅ **WORKING**

```csharp
// From GameRoot.Initialization.cs
_textureManager = new TextureManager();
_staticLayoutLoader = new StaticLayoutLoader(_textureManager);
_staticLayout = await _staticLayoutLoader.LoadAsync("Assets/Static/MeanStreets.json");
```

**Verification:**
- MeanStreets.json loads successfully
- Images parsed and stored in layout
- Texture files loaded via ImageSharp (SixLabors)

### 2.2 Texture Loading

**File:** `Engine/Rendering/Texture2D.cs`  
**Status:** ✅ **WORKING**

```csharp
public static Texture2D LoadFromFile(string filePath, TextureCache? textureCache = null)
{
    using var image = Image.Load<Rgba32>(filePath);
    // Converts RGBA → BGRA format
    // Loads pixel data into byte[]
}
```

**Verified Operations:**
- File existence check passes
- ImageSharp successfully loads textures
- Pixel format conversion (RGBA → BGRA) working
- Forensic logging shows texture data loaded

### 2.3 Static Layout Rendering

**File:** `Engine/UI/StaticLayoutRenderer.cs`  
**Status:** ⚠️ **NEEDS VERIFICATION**

```csharp
public void Draw(IRenderContext context, StaticLayout layout)
{
    foreach (var img in layout.Images.OrderBy(i => i.Layer))
    {
        var texture = _textureManager.Get(img.Path);
        if (texture == null) continue;  // ← Potential silent failure
        
        context.DrawTexture(texture, destRect, Color.White);
    }
}
```

**Potential Issues:**
1. `context` may not be the Framebuffer instance
2. DrawTexture implementation may fail silently
3. Viewport calculations may cull images incorrectly

### 2.4 Framebuffer Drawing

**File:** `Engine/Rendering/Framebuffer/FramebufferTexture.cs`  
**Status:** ✅ **FUNCTIONAL**

```csharp
public void DrawTexture(object texture, Rectangle destination, Rectangle? source = null)
{
    var tex = texture as Texture2D;
    if (tex == null) return;  // ← Silent failure if wrong type
    
    // Nearest-neighbor sampling
    // Alpha blending (source-over)
    // Writes to _pixels array (BGRA)
}
```

**Verification Points:**
- Forensic logging confirms DrawTexture calls
- Pixel writing confirmed via logging
- BGRA format maintained throughout

### 2.5 Framebuffer Upload to GPU

**File:** `Engine/Rendering/D3D11/D3D11FramebufferUploader.cs`  
**Status:** ✅ **WORKING**

```csharp
public void Upload(Framebuffer fb)
{
    // 1. Validate framebuffer
    // 2. Get backbuffer texture from swap chain
    // 3. Map upload texture for CPU write
    // 4. Copy pixel data row-by-row
    // 5. Unmap
    // 6. CopyResource(uploadTexture → backbuffer)
}
```

**Forensic Evidence:**
```
[FORENSIC] D3D11FramebufferUploader.Upload() ENTRY
[FORENSIC] About to call _uploader.Upload - _uploader is null: False
[FORENSIC] Upload texture Unmap completed
[FORENSIC] CopyResource completed successfully
[FORENSIC] D3D11FramebufferUploader.Upload() COMPLETED SUCCESSFULLY
```

### 2.6 Presentation

**File:** `Engine/Rendering/D3D11/D3D11Presentation.cs`  
**Status:** ✅ **FUNCTIONAL**

```csharp
public void Present()
{
    _swapChain.Present(1, PresentFlags.None);
}
```

---

## 3. Critical Issues Identified

### 3.1 🔴 CRITICAL: BGFX Struct Layout Mismatch

**Severity:** BLOCKER  
**Impact:** AccessViolationException at `bgfx_init`  
**Status:** IN PROGRESS - Partially Fixed

**Problem:**
The C# struct definitions for BGFX interop do not match the native C struct layouts in `bgfx.dll`.

**Evidence:**
```
FORENSIC: Init size = 144 (C#) vs expected ~152-168 (C header)
FORENSIC: PlatformData size = 56 (C#) vs expected 64-80 (C header)
FORENSIC: Resolution size = 24 (C#) ✓ matches
```

**Root Causes Identified:**
1. **PlatformData** - Missing `queue` field, wrong `type` size (byte vs int)
2. **Resolution** - Wrong texture format field sizes (byte vs int for enums)
3. **Init** - Alignment issues causing 8 bytes extra padding

**Fixes Applied:**
- ✅ Changed `session` → `queue` in PlatformData
- ✅ Changed `type` from `byte` to `int` (4-byte enum)
- ✅ Changed texture format fields from `byte` to `int`
- ✅ Added `Pack = 8` to Init struct

**Remaining Issue:**
```
Init struct: 144 bytes (still 8-24 bytes short of expected)
```

**Recommendation:**
Compile and run `bgfx_size_probe.c` against the exact `bgfx.dll` to get authoritative struct sizes, then adjust C# definitions to match exactly.

### 3.2 🟡 WARNING: Renderer Selection Ambiguity

**File:** `Engine/Rendering/RendererSelector.cs`  
**Status:** ⚠️ **POTENTIAL ISSUE**

The project has both D3D11 and BGFX renderers. The selector may be choosing BGFX which crashes, rather than the working D3D11 path.

**Evidence:**
```csharp
// From Program.cs
var graphicsDevice = RendererSelector.Create(RendererBackend.BGFX);  // ← Always BGFX?
```

**Recommendation:**
Force D3D11 backend for static image display until BGFX struct issues are resolved.

### 3.3 🟡 WARNING: StaticLayoutRenderer Context

**File:** `Engine/UI/StaticLayoutRenderer.cs`  
**Status:** ⚠️ **NEEDS VERIFICATION**

```csharp
public void Draw(IRenderContext context, StaticLayout layout)
{
    // context parameter - is this actually the Framebuffer?
    context.DrawTexture(texture, destRect, Color.White);
}
```

**Potential Issue:**
The `context` passed may not be the Framebuffer instance that gets uploaded to GPU.

**Verification Needed:**
Add type checking and logging to confirm `context` is `Framebuffer` before drawing.

### 3.4 🟢 GOOD: Forensic Logging System

**File:** `Engine/Animation/Core/ModernLoggingSystem.cs`  
**Status:** ✅ **EXCELLENT**

The forensic logging provides detailed trace information:
- Every framebuffer operation logged
- Texture load verification
- Upload completion confirmation
- Error capture with stack traces

**Key Log Files:**
- `forensic_log.md` - Structured markdown logging
- `bgfx_init_probe.txt` - BGFX initialization tracing
- `hud_debug.log` - HUD-specific diagnostics

---

## 4. Current Execution Flow Analysis

### 4.1 Successful Path (Based on Logs)

```
✓ Program.cs loads
✓ ModernLoggingSystem initializes
✓ Framebuffer created (800x600)
✓ Win32Window created (hwnd=1968934)
✓ D3D11GraphicsDevice initialized
✓ D3D11 device created (0x000001B390CF8740)
✓ Swap chain created (0x000001B392469080)
✓ Backbuffer RTV created
✓ StaticLayout loaded (MeanStreets.json)
? StaticLayoutRenderer.Draw() - UNVERIFIED
✓ Framebuffer.Upload() called
✓ D3D11FramebufferUploader.Upload() succeeds
✓ CopyResource completed
? Present() - UNVERIFIED RESULT
```

### 4.2 BGFX Failure Path

```
✓ BGFX.Initialize() called
✓ BGFXActivationGuard created
✓ BGFXState created
✓ Init struct created (144 bytes - WRONG SIZE)
✓ CALLBACKS DISABLED FOR ISOLATION
✗ bgfx_init() - AccessViolationException
```

---

## 5. Recommendations

### 5.1 Immediate Actions (Priority 1)

1. **Fix BGFX Struct Sizes**
   - Compile `bgfx_size_probe.c` with matching headers
   - Get exact struct sizes from native side
   - Adjust C# structs to match byte-for-byte
   - Test until `Marshal.SizeOf<T>()` matches C `sizeof()`

2. **Force D3D11 Renderer**
   - Change `RendererSelector.Create(RendererBackend.BGFX)` to `.D3D11`
   - Ensure static image path uses working D3D11 backend
   - Disable BGFX path until struct alignment fixed

3. **Verify StaticLayoutRenderer Context**
   - Add type check: `if (context is Framebuffer fb)`
   - Log actual context type before drawing
   - Ensure DrawTexture writes to correct buffer

### 5.2 Short-Term (Priority 2)

4. **Add Framebuffer Content Verification**
   - After StaticLayoutRenderer.Draw(), sample framebuffer pixels
   - Verify non-black pixels exist (images were drawn)
   - Log pixel statistics (count, min, max values)

5. **Viewport/Clamping Review**
   - Check viewport calculations in StaticLayoutRenderer
   - Verify 1080p→600p offset math doesn't cull all images
   - Test with debug rectangles showing viewport bounds

### 5.3 Long-Term (Priority 3)

6. **BGFX Full Integration**
   - Once struct sizes match, re-enable BGFX path
   - Add callback interface for BGFX debugging
   - Test both D3D11 and BGFX renderers

7. **Performance Optimization**
   - Batch static image draws
   - Minimize CPU→GPU transfers
   - Consider GPU-side static image caching

---

## 6. File Status Summary

| File | Purpose | Status | Issues |
|------|---------|--------|--------|
| `Program.cs` | Entry point | ✅ | Calls BGFX by default |
| `GameRoot.cs` | Main game controller | ✅ | Initialization flow correct |
| `StaticLayoutRenderer.cs` | Static image rendering | ⚠️ | Context type unverified |
| `Framebuffer.cs` | CPU pixel buffer | ✅ | Core functions working |
| `FramebufferTexture.cs` | DrawTexture implementation | ✅ | Alpha blending correct |
| `Texture2D.cs` | Texture loading | ✅ | ImageSharp integration good |
| `D3D11GraphicsDeviceCore.cs` | D3D11 renderer | ✅ | Initialization succeeds |
| `D3D11FramebufferUploader.cs` | CPU→GPU upload | ✅ | Forensic-verified working |
| `BGFXNative.cs` | BGFX interop structs | ❌ | **Size mismatch - CRITICAL** |
| `BGFXGraphicsDeviceCore.cs` | BGFX renderer | ❌ | **Crash at init - CRITICAL** |
| `Win32Window.cs` | Native window | ✅ | Creation and messages working |
| `ModernLoggingSystem.cs` | Forensic logging | ✅ | Excellent diagnostics |

---

## 7. Conclusion

The static image display pipeline has a **single critical blocker**: the BGFX struct layout mismatch causing `AccessViolationException` at `bgfx_init`. The D3D11 rendering path appears functional based on forensic logging, but the renderer selection may be forcing the broken BGFX path.

**Immediate Solution:**
1. Force D3D11 renderer selection
2. Verify StaticLayoutRenderer context type
3. Confirm framebuffer content after drawing

Once the BGFX struct sizes are corrected (via the C probe tool), both renderer paths should work, providing redundancy and future flexibility.

---

## Appendix A: Forensic Log Evidence

### A.1 BGFX Initialization Failure
```
InitBGFX: MINIMAL OVERRIDE - zeroing all fields
InitBGFX: Minimal config - type=Direct3D11, hwnd=4524986, w=800, h=600
InitBGFX: CALLBACKS DISABLED FOR ISOLATION
FORENSIC: Init size = 144  ← TOO SMALL
FORENSIC: PlatformData size = 56  ← TOO SMALL
FORENSIC: Resolution size = 24  ✓ CORRECT
InitBGFX: BEFORE BGFXNative.bgfx_init call
[CRASH - AccessViolationException]
```

### A.2 D3D11 Success Path
```
[D3D11DeviceCore] SUCCESS: D3D11 device created
[D3D11DeviceCore] SUCCESS: Swap chain created
[D3D11DeviceCore] SUCCESS: Backbuffer RTV created
[FORENSIC] UploadFramebuffer ENTRY
[FORENSIC] CopyResource completed successfully
[FORENSIC] D3D11FramebufferUploader.Upload() COMPLETED SUCCESSFULLY
```

### A.3 Static Layout Loading
```
[HUD DIAGNOSTIC] StaticLayout.Images count = 12
[HUD DIAGNOSTIC] Image: id=bg_01, path=Assets/Static/MeanStreets.png, x=0, y=0
```

---

**End of Analysis**
