// =====================================================================================================
//  FILE: AssetTypeExtensions.cs
//  PATH: Engine/Resources/Assets/AssetTypeExtensions.cs
//  MODULE: Resources / Assets
//
//  ROLE:
//      Provide deterministic, type-safe comparison helpers for AssetType values used by the asset
//      loading and resource management subsystems.
//
//  RESPONSIBILITIES:
//      - Provide EqualsAssetType() behavior for the Resources / Assets subsystem.
//      - Ensure AssetType comparisons remain deterministic and string-format agnostic.
//      - Improve readability of AssetType equality checks without modifying core asset logic.
//
//  NON-RESPONSIBILITIES:
//      - Performing asset loading, caching, or lifetime management.
//      - Managing file I/O, serialization, or persistence.
//      - Mutating global resource registries or asset catalogs.
//
//  ARCHITECTURAL NOTES:
//      - This module is scoped strictly to AssetType comparison helpers.
//      - Extracted from OperatorExtensions.cs during subsystem breakup and relocated to the
//        Resources / Assets pipeline.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.Resources.Assets
{
    /// <summary>
    /// Deterministic comparison helpers for AssetType.
    /// </summary>
    public static class AssetTypeExtensions
    {
        /// <summary>
        /// Compares two AssetType values for equality using their canonical string representation.
        /// </summary>
        public static bool EqualsAssetType(this AssetType type1, AssetType type2)
        {
            return type1.ToString() == type2.ToString();
        }

        public class DataLoader
        {
            public static object Load(string path)
            {
                return null;
            }
        }
    }
}
