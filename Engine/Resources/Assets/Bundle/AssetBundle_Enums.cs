/*
//============================================================================
//File:        AssetBundle_Enums.cs
//Path:        E:\BDC\Projects\SASZombieAssaultTD\Engine\Resources\Assets\Bundle\AssetBundle_Enums.cs
//Program:     AssetBundle (Enums)
//Subsystem:   Assets
//Author:      BDC
//Created:     2026-05-17
//
//Purpose:
//    Defines core enumerations used by the AssetBundle subsystem, including
//    bundle versioning and compression level selection. These enums form part
//    of the stable serialization contract for bundle metadata and processing.
//
//Responsibilities:
//    • Provide version identifiers for bundle format evolution.
//    • Define compression level options for bundle creation and loading.
//    • Maintain stable enum values for serialization and compatibility.
//    • Serve as shared constants for AssetBundle_Core, Loader, and Processors.
//
//Notes:
//    • These enums are engine‑owned and must remain stable for serialization.
//    • Do not reorder or renumber values without a formal migration plan.
//    • No BGFX, no legacy backend references.
//============================================================================
*/

//

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Resources
{
    ///<summary>
    ///Identifies the version of the asset bundle format.
    ///Used for compatibility checks and upgrade paths.
    ///</summary>
    public enum BundleVersion
    {
        ///<summary>
        ///Original bundle format. Basic metadata, no compression metadata block.
        ///</summary>
        V1 = 1,

        ///<summary>
        ///Updated bundle format with explicit compression metadata and
        ///improved header structure.
        ///</summary>
        V2 = 2
    }

    ///<summary>
    ///Defines compression levels used when packing asset bundles.
    ///These values map to engine-level compression behavior, not .NET's enum.
    ///</summary>
    public enum CompressionLevel
    {
        ///<summary>
        ///Fastest compression available. Prioritizes speed over size reduction.
        ///</summary>
        Fastest,

        ///<summary>
        ///No compression applied. Useful for debugging or rapid iteration.
        ///</summary>
        NoCompression,

        ///<summary>
        ///Balanced compression. Good default for most asset bundles.
        ///</summary>
        Optimal,

        ///<summary>
        ///Maximum compression ratio. Slowest but smallest output size.
        ///</summary>
        Maximum
    }
}
