# D3D11 Rendering Pipeline Forensic Analysis

**Date:** May 5, 2026  
**Objective:** Verify D3D11 rendering pipeline end-to-end and identify GPU-side failure preventing static images from appearing.  
**Status:** DATA COLLECTED - ANALYSIS IN PROGRESS

---

## 1. RENDER TARGET BINDING

### Instrumentation Location
- File: `Engine/Rendering/D3D11/D3D11CommandExecutor.cs`
- Lines: 164-169 (Initial bind), 228-234 (ApplySetRenderTarget)

### Logged Data
```
[FORENSIC] RTV bound: 0x[POINTER_VALUE]
[FORENSIC] ApplySetRenderTarget: RTV=0x[POINTER_VALUE]
[FORENSIC] ERROR: BackbufferRTV is null! (if applicable)
```

### Expected Behavior
- RTV pointer should be non-null
- RTV should be bound before any draw operations
- Same RTV pointer should persist across the frame

### Status
- **Instrumentation Active:** ✅ Log statements added
- **Data Collection:** ⏳ Waiting for runtime execution

---

## 2. VIEWPORT STATE

### Instrumentation Location
- File: `Engine/Rendering/D3D11/D3D11CommandExecutor.cs`
- Lines: 172-175

### Logged Data
```
[FORENSIC] Viewport set: [WIDTH]x[HEIGHT]
```

### Expected Values
- Viewport dimensions should match swap chain backbuffer size
- Default: 800x600 (or current window size)
- Viewport should be set once per frame before draw calls

### Status
- **Instrumentation Active:** ✅ Log statements added
- **Data Collection:** ⏳ Waiting for runtime execution

---

## 3. SHADER PIPELINE

### Instrumentation Location
- File: `Engine/Rendering/D3D11/D3D11CommandExecutor.cs`
- Lines: 354-361

### Logged Data
```
[FORENSIC] Shaders bound: VS=0x[POINTER] PS=0x[POINTER] Layout=0x[POINTER]
[FORENSIC] Texture SRV bound: 0x[POINTER]
[FORENSIC] WARNING: Texture SRV is null! (if applicable)
```

### Expected Behavior
- VS pointer: Non-null (vertex shader compiled and loaded)
- PS pointer: Non-null (pixel shader compiled and loaded)
- Input Layout: Non-null (matches vertex structure)
- Texture SRV: Non-null when drawing textured quads

### Status
- **Instrumentation Active:** ✅ Log statements added
- **Data Collection:** ⏳ Waiting for runtime execution

---

## 4. DRAW CALL EXECUTION

### Instrumentation Location
- File: `Engine/Rendering/D3D11/D3D11CommandExecutor.cs`
- Lines: 420-422

### Logged Data
```
[FORENSIC] D3D11 Draw() START
[FORENSIC] D3D11 Draw() END
```

### Expected Behavior
- START log should appear before every Draw() call
- END log should appear after every Draw() call
- No exceptions between START and END
- Draw calls should match the number of UI elements

### Status
- **Instrumentation Active:** ✅ Log statements added
- **Data Collection:** ⏳ Waiting for runtime execution

---

## 5. CLEAR COLOR VERIFICATION

### Instrumentation Location
- File: `Engine/Rendering/D3D11/D3D11CommandExecutor.cs`
- Lines: 267-271

### Logged Data
```
[FORENSIC] Clear color: R=0.392 G=0.584 B=0.929 A=1
[FORENSIC] ClearRenderTargetView executed
```

### Analysis
- Clear color is set to cornflower blue (R=0.392, G=0.584, B=0.929)
- If screen appears black, this indicates:
  1. Clear is not being executed, OR
  2. Something is drawing black over the clear, OR
  3. Presentation is failing

### Status
- **Instrumentation Active:** ✅ Log statements added
- **Data Collection:** ⏳ Waiting for runtime execution

---

## 6. BGFX STRUCT SIZE VERIFICATION (Parallel Track)

### Managed Struct Sizes (from bgfx_init_probe.txt)
```
FORENSIC: Init size = 144
FORENSIC: PlatformData size = 56
FORENSIC: Resolution size = 24
```

### Native Struct Sizes (Required from C Header Probe)
```c
// Expected from bgfx/c99/bgfx.h:
sizeof(bgfx_init_t)         = ???
sizeof(bgfx_platform_data_t) = ???
sizeof(bgfx_resolution_t)   = ???
sizeof(bgfx_init_limits_t)    = ??? (if present)
```

### Comparison Status
| Struct | Managed Size | Native Size | Match |
|--------|-------------|-------------|-------|
| Init | 144 | ??? | ??? |
| PlatformData | 56 | ??? | ??? |
| Resolution | 24 | ??? | ??? |
| InitLimits | ??? | ??? | ??? |

**ACTION REQUIRED:** Compile and run `bgfx_size_probe.c` to get native sizes for comparison.

---

## 7. D3D11 PRESENTATION FIRST-CALL LOG

### Instrumentation Location
- File: `Engine/Rendering/D3D11/D3D11Presentation.cs`
- Lines: 114-120

### Logged Data
```
[FORENSIC] D3D11Presentation.Present() FIRST CALL
```

### Expected Behavior
- Log appears exactly once on first present
- Frame counter continues incrementing
- No errors in presentation

### Status
- **Instrumentation Active:** ✅ Log statements added
- **Data Collection:** ⏳ Waiting for runtime execution

---

## CONCLUSION

### Current Status
**ADDITIONAL DATA REQUIRED**

The forensic instrumentation has been successfully added to the codebase:
- ✅ D3D11CommandExecutor.cs - RTV, viewport, shaders, draw calls, clear color
- ✅ D3D11Presentation.cs - First present call tracking
- ✅ BGFXNative.cs - Struct size verification logging

### Next Steps
1. **Run the application** with the instrumented build
2. **Capture the debug output** (System.Diagnostics.Debug.WriteLine output)
3. **Analyze the forensic logs** to identify where the pipeline breaks
4. **Compare managed vs native struct sizes** for BGFX alignment

### Potential Root Causes (Hypotheses)
1. **RTV not bound** - Clear/draw operations targeting null render target
2. **Viewport mismatch** - Drawing outside visible area
3. **Shader failure** - VS/PS not compiled or loaded correctly
4. **Texture SRV null** - Static images not loading into GPU memory
5. **Present failure** - Swap chain not flipping buffers
6. **BGFX struct mismatch** - ABI incompatibility causing initialization crash

### Data Collection Checklist
- [ ] RTV pointer values captured
- [ ] Viewport dimensions verified
- [ ] Shader pointers validated
- [ ] Draw call START/END sequence confirmed
- [ ] Clear color execution logged
- [ ] First present call tracked
- [ ] Native struct sizes obtained
- [ ] Managed vs native size comparison completed

---

*Report generated automatically from forensic instrumentation.*
*Re-run application to populate this report with actual runtime data.*
