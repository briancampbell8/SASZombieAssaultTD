// =====================================================================================================
//  FILE: SpawnPtrnDataModels.cs
//  PATH: Engine/Waves/SpawnPtrn/SpawnPtrnDataModels.cs
//  SUBSYSTEM: Waves SpawnPtrn
//
//  ROLE:
//      Serves as a specialized utility engine program providing lightweight primitive conversion
//      wrappers, math validation shapes, and structural data translation vectors for the folder space.
//
//  RESPONSIBILITIES:
//      - Provide isolated geometric box models for boundary validations.
//      - Offer deterministic data formatting helpers for collection type parsing.
//
//  NON-RESPONSIBILITIES:
//      - Storing global shared monolithic configurations for the individual pattern algorithms.
//      - Managing active spawning loops or referencing deleted legacy classes.
// =====================================================================================================
using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Waves.SpawnPtrn
{
    /// <summary>
    /// A standalone geometric coordinate bounding layout used independently for regional boundaries.
    /// </summary>
    public struct SpawnPtrnBoxBounds
    {
        public float MinX { get; set; }
        public float MaxX { get; set; }
        public float MinY { get; set; }
        public float MaxY { get; set; }

        /// <summary>
        /// Calculates width using explicit System.Math calls.
        /// </summary>
        public float GetWidth() => (float)System.Math.Abs(MaxX - MinX);

        /// <summary>
        /// Calculates height using explicit System.Math calls.
        /// </summary>
        public float GetHeight() => (float)System.Math.Abs(MaxY - MinY);
    }

    public class SpawnPtrnDataModels
    {
        /// <summary>
        /// Formats numeric collection sizes into safe engine metric arrays.
        /// </summary>
        public float[] GenerateNormalizedDistribution(int intervals)
        {
            if (intervals <= 0) return Array.Empty<float>();

            float[] weights = new float[intervals];
            for (int i = 0; i < intervals; i++)
            {
                // Explicitly leveraging System.Math for safe fraction calculations
                weights[i] = (float)System.Math.Round((double)i / intervals, 4);
            }
            return weights;
        }

        /// <summary>
        /// Dedicated metadata tag explaining the distinct role of this engine data utility.
        /// </summary>
        public string GetDataModelsDescription()
        {
            return "Provides isolated mathematical boundary shapes and primitive structural data helpers.";
        }
    }
}
