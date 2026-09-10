// =====================================================================================================
//  FILE: SpawnPatternExtensions.cs
//  PATH: Engine/Navigation/SpawnPatternExtensions.cs
//  MODULE: Navigation
//
//  ROLE:
//      Provide deterministic, stateless helper methods for spawn‑point enumeration, safe fallback
//      origin selection, and summary generation using NavigationGrid’s public deterministic API.
//
//  RESPONSIBILITIES:
//      - Provide pure, stateless utilities for safe spawn‑point enumeration.
//      - Provide deterministic fallback origin selection when spawn points are missing.
//      - Provide readable summaries of registered spawn points.
//      - Remain fully deterministic and side‑effect free.
//
//  NON-RESPONSIBILITIES:
//      - Performing spawning, unit creation, or game‑state mutation.
//      - Referencing legacy SpawnPattern or deprecated pattern executor systems.
//      - Allocating or mutating NavigationGrid internals.
//
//  ARCHITECTURAL NOTES:
//      - This module operates exclusively on NavigationGrid’s public deterministic API.
//      - Relocated from Engine/Extensions during subsystem cleanup.
//      - Must not introduce cross‑subsystem coupling or hidden dependencies.
// =====================================================================================================
