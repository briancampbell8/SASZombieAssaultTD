# ✅ UpdateLoop.cs Refactoring Summary

**Target File:** `Engine/GameRoot/UpdateLoop.cs`  
**Date:** April 14, 2026  
**Scope:** Local refactor + bugfix + diagnostics + validation hooks

---

## Changes Applied

### Step 1 — Added New Private Fields
```csharp
private int _frameIndex = 0;
private Vector3 _lastViewportSize = new Vector3(-1, -1, 0);
private IDiagnosticOverlay _diagnosticOverlay;
```

### Step 2 — Hardened RenderContext Usage
- Added null check at the top of `PerformRender()`
- Logs error and returns early if `_renderContext` is null

### Step 3 — Made Framebuffer Clear Deterministic
- Changed from silent cast-and-clear to explicit pattern matching
- Logs debug message if clear is skipped

### Step 4 — Made Map Rendering Failure Modes Explicit
- Replaced combined null check with separate checks for `_staticLayout` and `_staticLayoutRenderer`
- Each case logs specific debug message explaining why map was skipped

### Step 5 — Corrected Render Order
**Previous Order:**
1. Map
2. HUD
3. State machine
4. RenderManager

**New Order:**
1. Map
2. State machine
3. HUD
4. RenderManager

### Step 6 — Fixed HUD Scaling Jitter and Null Logging
- HUD scale now only updates when viewport size changes (no per-frame jitter)
- Uses `_lastViewportSize` to track changes
- Logs exactly one scale update per resolution change
- Added null check for `_hudManager` with debug log

### Step 7 — Guarded RenderManager Usage
- Added null check for `_renderManager` before calling `RenderAll()`
- Logs debug message if RenderManager is null

### Step 8 — Improved Update Loop Logging
- Added frame index to update completion log
- Frame index increments after each update

### Step 9 — Injected Diagnostic Overlay Hook
- Added `_diagnosticOverlay?.Draw(_renderContext)` at end of `PerformRender()`
- Diagnostic overlay renders on top of everything

### Step 10 — Replaced Console.WriteLine
- Removed all `Console.WriteLine` calls from `PerformRender()`
- All logging now uses `ModernLoggingSystem`

---

## Expected Log Patterns

### Normal Frame
```
[PerformRender] Drawing Map...
Frame 0 update completed in 0.0167s
[PerformRender] HUD scale updated for viewport 1920x1080
Frame render completed
```

### Viewport Resize
```
[PerformRender] HUD scale updated for viewport 2560x1440
```

### Missing Components
```
[PerformRender] HUDManager is null — HUD skipped
[PerformRender] RenderManager is null — skipping system rendering
[PerformRender] Map rendering skipped — static layout is null
[PerformRender] Map rendering skipped — static layout renderer is null
```

---

## Validation Checklist

- [ ] HUD elements remain anchored correctly at different resolutions
- [ ] Diagnostic overlay (if wired) renders on top of HUD and game content
- [ ] Log shows exactly one `HUD scale updated` per resolution change
- [ ] No `Console.WriteLine` output from UpdateLoop
- [ ] Frame index increments correctly in logs
