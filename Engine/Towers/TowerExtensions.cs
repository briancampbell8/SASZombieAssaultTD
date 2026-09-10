// =====================================================================================================
//  FILE: TowerExtensions.cs
//  PATH: Engine/Towers/TowerExtensions.cs
//  MODULE: Towers
//
//  ROLE:
//      Provide deterministic, type‑safe helper methods for TowerManager and Tower instances, supporting
//      lookup helpers, placement validation, upgrade checks, and tower‑math utilities without modifying
//      core TowerManager or Tower behavior.
//
//  RESPONSIBILITIES:
//      - Provide nearest‑tower lookup helpers.
//      - Provide deterministic tower‑math utilities (distance, range checks).
//      - Provide placement validation and affordability helpers.
//      - Provide upgrade‑related helpers (cost, eligibility).
//      - Remain pure and side‑effect free.
//
//  NON-RESPONSIBILITIES:
//      - Performing rendering or GPU operations.
//      - Managing ECS entities or world‑grid occupancy.
//      - Allocating or destroying towers.
//      - Mutating TowerManager lifecycle behavior.
//
//  ARCHITECTURAL NOTES:
//      - TowerManager is intentionally minimal; extensions provide convenience without expanding subsystem
//        responsibilities.
//      - Relocated from Engine/Extensions during subsystem cleanup.
//      - Must not introduce cross‑subsystem coupling.
// =====================================================================================================
