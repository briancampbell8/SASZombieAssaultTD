Rendering System
================

Overview
--------

The rendering system provides a framework-agnostic abstraction for drawing
frames, managing render surfaces, and coordinating batched sprite rendering.

Key Components
--------------

- IRenderContext: Minimal interface for issuing draw commands.
- RenderSurface: Optional off-screen target for advanced effects.
- RenderQueue / SpriteBatch: Ordered, batched draw submission.
