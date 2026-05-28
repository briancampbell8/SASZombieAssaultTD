/*
// File: BundleReports.cs
// Purpose: Render static layout images using texture handles provided by StaticLayoutLoader.
// Notes:   This renderer draws only pre-defined static UI elements. No animation, no logic.
//          All visibility, ordering, and asset resolution is handled externally.
*/

using System;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Resources
{
    public partial class BundleValidationResult
    {
        // IsValid is already defined in the other file, so we keep the missing ones here:
        public long ExpectedSize { get; set; }
        public long ActualSize { get; set; }
        public string ExpectedChecksum { get; set; } = string.Empty;
        public string ActualChecksum { get; set; } = string.Empty;

        public List<string> Issues { get; set; } = new List<string>();
        public List<string> CorruptedAssets { get; set; } = new List<string>();
        public List<string> Errors { get; set; } = new List<string>();
    }

    public partial class BundleHealthReport
    {
        // Timestamp, BundlePath, and IsLoaded are already defined in the other file:
        public int TotalAssets { get; set; }
        public int LoadedAssets { get; set; }
        public long MemoryUsage { get; set; }
        public float CompressionRatio { get; set; }

        public List<string> Warnings { get; set; } = new List<string>();
        public List<string> Recommendations { get; set; } = new List<string>();
    }

    public partial class BundleRepairReport
    {
        // Timestamp and ActionsTaken are already defined in the other file.
        // If this class becomes empty, that is completely fine!
    }

    public partial class BundleOptimizationReport
    {
        // Timestamp, ActionsTaken, AssetsOptimized, and MemoryFreed are already defined in the other file.
        // If this class becomes empty, that is completely fine!
    }
}
