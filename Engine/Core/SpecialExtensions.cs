// =====================================================================================================
//  FILE: SpecialExtensions.cs
//  PATH: Engine/Core/SpecialExtensions.cs
//  MODULE: Core
//
//  ROLE:
//      Provide deterministic, subsystem‑agnostic utility helpers for lightweight math operations,
//      safe casting, clamping, normalization, and common deterministic behaviors used across the engine.
//
//  RESPONSIBILITIES:
//      - Provide pure, stateless helper functions for common deterministic operations.
//      - Support safe casting and fallback behavior for mixed‑type engine data.
//      - Provide lightweight math helpers (clamp, lerp, normalize, distance) without introducing dependencies.
//      - Remain fully deterministic and side‑effect free.
//
//  NON-RESPONSIBILITIES:
//      - Managing engine lifecycle or orchestrating subsystem behavior.
//      - Performing rendering, spawning, ECS operations, or state transitions.
//      - Allocating engine resources or mutating engine state.
//      - Replacing or overriding subsystem‑specific extension modules.
//
//  ARCHITECTURAL NOTES:
//      - This module is intentionally generic and subsystem‑agnostic.
//      - All helpers must remain pure and deterministic.
//      - Relocated from Engine/Extensions during subsystem cleanup.
// =====================================================================================================
