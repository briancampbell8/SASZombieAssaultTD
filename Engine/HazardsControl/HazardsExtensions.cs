// =====================================================================================================
//  FILE: HazardsExtensions.cs
//  PATH: Engine/HazardsControl/HazardsExtensions.cs
//  MODULE: HazardsControl
//
//  ROLE:
//      Provide deterministic, type‑safe extension methods for HazardsMain and Hazard instances,
//      enabling hazard lookup helpers, state evaluation, lifetime checks, intensity math utilities,
//      and radius containment helpers.
//
//  RESPONSIBILITIES:
//      - Provide GetNearestHazard() behavior for the HazardsControl subsystem.
//      - Provide GetHazardsByType() behavior for the HazardsControl subsystem.
//      - Provide IsExpired(), IsActive(), IsWithinLifetime() behavior for Hazard instances.
//      - Provide GetIntensityPercent() and IsDecaying() behavior for Hazard instances.
//      - Provide IsInsideRadius() and DistanceTo() behavior for Hazard instances.
//      - Provide IsType() and GetTypeSummary() behavior for Hazard instances.
//
//  NON-RESPONSIBILITIES:
//      - Performing rendering or GPU operations directly.
//      - Managing ECS entities or world‑grid occupancy directly.
//      - Allocating hazards, destroying hazards, or mutating core lifecycle behavior.
//      - Replacing or overriding HazardsMain’s deterministic behavior.
//
//  NOTES:
//      Reflection removed; deterministic public accessors used exclusively.
//      Relocated from Engine/Extensions to Engine/HazardsControl.
// =====================================================================================================

using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.HazardsControl
{
    /// <summary>
    /// Deterministic helper extensions for HazardsMain and Hazard.
    /// </summary>
    public static class HazardsExtensions
    {
        // ----------------------------------------------------------------------------------------------
        //  INTERNAL ACCESS (NO REFLECTION)
        // ----------------------------------------------------------------------------------------------

        private static IEnumerable<Hazard> EnumerateActiveHazards(this HazardsMain manager)
        {
            return manager.ActiveHazards;   // Your modern deterministic API
        }

        // ----------------------------------------------------------------------------------------------
        //  HAZARD LOOKUP HELPERS
        // ----------------------------------------------------------------------------------------------

        public static Hazard GetNearestHazard(this HazardsMain manager, Vector3 position)
        {
            if (manager == null)
                return null;

            Hazard nearest = null;
            float bestDist = float.MaxValue;

            foreach (Hazard hazard in manager.EnumerateActiveHazards())
            {
                float dist = (hazard.Position - position).Length;
                if (dist < bestDist)
                {
                    bestDist = dist;
                    nearest = hazard;
                }
            }

            return nearest;
        }

        public static IEnumerable<Hazard> GetHazardsByType(this HazardsMain manager, string type)
        {
            if (manager == null || string.IsNullOrWhiteSpace(type))
                yield break;

            foreach (Hazard hazard in manager.EnumerateActiveHazards())
            {
                if (hazard.Type.Equals(type, System.StringComparison.OrdinalIgnoreCase))
                    yield return hazard;
            }
        }

        // ----------------------------------------------------------------------------------------------
        //  STATE / LIFETIME HELPERS
        // ----------------------------------------------------------------------------------------------

        public static bool IsExpired(this Hazard hazard)
        {
            if (hazard == null)
                return true;

            return hazard.State == HazardState.Expired || hazard.CurrentIntensity <= 0f;
        }

        public static bool IsActive(this Hazard hazard)
        {
            return hazard != null && hazard.State == HazardState.Active;
        }

        public static bool IsWithinLifetime(this Hazard hazard)
        {
            return hazard != null && hazard.Lifetime <= hazard.MaxLifetime;
        }

        // ----------------------------------------------------------------------------------------------
        //  INTENSITY / DECAY HELPERS
        // ----------------------------------------------------------------------------------------------

        public static float GetIntensityPercent(this Hazard hazard)
        {
            if (hazard == null)
                return 0f;

            return hazard.CurrentIntensity * 100f;
        }

        public static bool IsDecaying(this Hazard hazard)
        {
            return hazard != null &&
                   hazard.HasDecay &&
                   hazard.State == HazardState.Active;
        }

        // ----------------------------------------------------------------------------------------------
        //  RADIUS / RANGE HELPERS
        // ----------------------------------------------------------------------------------------------

        public static bool IsInsideRadius(this Hazard hazard, Vector3 position)
        {
            if (hazard == null)
                return false;

            float dist = (hazard.Position - position).Length;
            return dist <= hazard.Radius;
        }

        public static float DistanceTo(this Hazard hazard, Vector3 position)
        {
            if (hazard == null)
                return float.MaxValue;

            return (hazard.Position - position).Length;
        }

        // ----------------------------------------------------------------------------------------------
        //  TYPE / CATEGORY HELPERS
        // ----------------------------------------------------------------------------------------------

        public static bool IsType(this Hazard hazard, string keyword)
        {
            if (hazard == null || string.IsNullOrWhiteSpace(keyword))
                return false;

            return hazard.Type.IndexOf(keyword, System.StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static string GetTypeSummary(this Hazard hazard)
        {
            if (hazard == null)
                return "No hazard";

            return $"{hazard.Type} (Intensity {hazard.CurrentIntensity:F2})";
        }
    }
}
