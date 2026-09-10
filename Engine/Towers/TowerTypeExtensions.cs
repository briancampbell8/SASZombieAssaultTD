// =====================================================================================================
//  FILE: TowerTypeExtensions.cs
//  PATH: Engine/Towers/TowerTypeExtensions.cs
//  SUBSYSTEM: Towers
//
//  ROLE:
//      Provides deterministic, type‑safe extension methods for TowerType, enabling metadata lookup,
//      category classification, minimum‑distance rules, and descriptive helpers.
//
//  RESPONSIBILITIES:
//      - Extend TowerType with helper methods for retrieving tower metadata.
//      - Provide deterministic utilities for minimum placement distance and category evaluation.
//      - Improve readability and maintainability of tower‑type logic without modifying core systems.
//      - Remain pure: no side effects outside TowerType’s own classification behavior.
//
//  NON-RESPONSIBILITIES:
//      - Performing rendering or GPU operations.
//      - Managing tower lifecycle, placement, or ECS integration.
//      - Allocating towers, destroying towers, or mutating TowerManager behavior.
//      - Replacing or overriding TowerType’s deterministic behavior.
//
//  ARCHITECTURAL NOTES:
//      - TowerType is intentionally minimal; extensions provide convenience without expanding subsystem
//        responsibilities.
//      - All extension methods must remain stateless and side‑effect free.
//      - Relocated from Engine/Extensions during subsystem cleanup.
// =====================================================================================================
