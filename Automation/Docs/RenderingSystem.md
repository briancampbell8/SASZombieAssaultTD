# Rendering System (WinForms Surface)

## Purpose
Provide a WinForms-based rendering surface for the engine, suitable for 2D tower-defense visuals and debug overlays.

## Components
- WindowHost: Owns the WinForms Form and message loop.
- RenderSurface: Custom control used as the drawing surface.
- IRenderContext: Abstraction over drawing operations.
- DebugOverlay: Renders frame stats and engine heartbeat.

## Integration
- GameRoot constructs WindowHost and passes engine callbacks.
- GameLoop drives Update/Render; RenderSurface invalidates and repaints.
- DebugOverlay draws last to ensure visibility.

## Future Evolution
- Swap GDI+ drawing with GPU-backed renderer if needed.
- Add camera, layers, and sprite batching on top of this surface.