// =====================================================================================================
//  FILE: TowerUpgradeExtensions.cs
//  PATH: Engine/Towers/TowerUpgradeExtensions.cs
//  SUBSYSTEM: Towers
//
//  ROLE:
//      Provides deterministic, type‑safe extension methods for TowerUpgrade, enabling metadata lookup,
//      tier evaluation, special‑ability helpers, and upgrade comparison utilities.
//
//  RESPONSIBILITIES:
//      - Extend TowerUpgrade with helper methods for retrieving upgrade metadata.
//      - Provide deterministic utilities for tier evaluation and special‑ability inspection.
//      - Improve readability and maintainability of upgrade logic without modifying core systems.
//      - Remain pure: no side effects outside TowerUpgrade’s own stat and metadata fields.
//
//  NON-RESPONSIBILITIES:
//      - Performing rendering or GPU operations directly.
//      - Managing tower lifecycle, placement, or ECS integration.
//      - Allocating upgrades, destroying upgrades, or mutating TowerUpgradeManager behavior.
//      - Replacing or overriding TowerUpgrade’s deterministic behavior.
//
//  ARCHITECTURAL NOTES:
//      - TowerUpgrade is intentionally minimal; extensions provide convenience without expanding subsystem
//        responsibilities.
//      - All extension methods must remain stateless and side‑effect free.
//      - Relocated from Engine/Extensions during subsystem cleanup.
// =====================================================================================================
