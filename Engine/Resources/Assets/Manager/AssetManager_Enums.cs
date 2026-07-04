//============================================================================
//File:        AssetManager_Enums.cs
//Path:        E:\BDC\Projects\SASZombieAssaultTD\Engine\Resources\Assets\Manager\AssetManager_Enums.cs
//Program:     AssetManager (Enums)
//Subsystem:   Assets
//Author:      BDC
//Created:     2026-05-29
//
//Purpose:
//    Defines enumeration types used throughout the new Asset subsystem,
//    including asset loading priority levels and other classification enums
//    required by loader, registry, pipeline, and validation components.
//
//Responsibilities:
//    • Provide strongly‑typed priority levels for asset loading.
//    • Support scheduling, caching, and async loading decisions.
//    • Ensure consistent priority semantics across all AssetManager partials.
//    • Serve as a stable contract for the Asset subsystem.
//
//Architecture:
//    • Pure enum definitions (no logic).
//    • Consumed by AssetManager_Loader, AssetManager_Registry, and AssetPipeline.
//    • Stable contract: changes may affect scheduling and caching behavior.
//
//Integration Points:
//    • Asset loading queues.
//    • Preload systems.
//    • Memory optimization routines.
//    • AssetBundle and AssetPipeline priority mapping.
//
//Notes:
//    • Priority values are ordered from highest urgency (0) to lowest (4).
//    • Additional enums may be added here as the subsystem expands.
//============================================================================


//

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Resources
{
    ///<summary>
    ///Asset loading priority levels.
    ///Determines scheduling order for async and background loading.
    ///</summary>
    public enum AssetPriority
    {
        ///<summary>
        ///Must be loaded immediately; required for startup or critical systems.
        ///</summary>
        Critical = 0,

        ///<summary>
        ///High‑importance assets needed soon after startup.
        ///</summary>
        High = 1,

        ///<summary>
        ///Default priority for most assets.
        ///</summary>
        Normal = 2,

        ///<summary>
        ///Low‑importance assets that can be deferred.
        ///</summary>
        Low = 3,

        ///<summary>
        ///Background loading; lowest urgency.
        ///</summary>
        Background = 4
    }
}
