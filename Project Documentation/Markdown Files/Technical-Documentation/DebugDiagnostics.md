Debug Diagnostics
=================

Overview
--------

The diagnostics subsystem provides lightweight runtime insight into engine
health, frame timing, and logging.

Components
----------

- FrameStats: Tracks delta time and FPS.
- HeartbeatMonitor: Detects stalls in the main loop.
- DebugLogger: Centralized, buffered logging surface.
