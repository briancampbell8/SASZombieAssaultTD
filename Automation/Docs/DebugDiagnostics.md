# Debug & Diagnostics

## Purpose
Provide visibility into engine behavior during development.

## Components
- FrameStats: Tracks FPS, UPS, and delta time.
- HeartbeatMonitor: Simple health indicator for the main loop.
- DebugLogger: Lightweight logging abstraction.

## Usage
- FrameStats updated each frame and rendered via DebugOverlay.
- HeartbeatMonitor can be used to detect stalls or hangs.
- DebugLogger writes to console or future log targets.

## Visual Studio Workflow
- Run in Debug Mode.
- Use breakpoints in scenes and rendering.
- Compare behavior against reference footage and documentation.