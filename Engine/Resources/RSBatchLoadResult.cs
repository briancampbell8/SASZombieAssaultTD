/*
File:    AssetBatchLoadResult.cs
Author:  BDC
Created: 2026-02-08
Purpose: Represents the result of a batch asset loading operation.
Notes:   Contains success and failure counts for pipeline operations.
*/

namespace SASZombieAssaultTD.Engine.Assets
{
    /// <summary>
    /// Represents the outcome of loading multiple assets in a batch.
    /// </summary>
    public sealed class AssetBatchLoadResult
    {
        /// <summary>
        /// Number of assets successfully loaded.
        /// </summary>
        public int SuccessCount { get; }

        /// <summary>
        /// Number of assets that failed to load.
        /// </summary>
        public int FailureCount { get; }

        public AssetBatchLoadResult(int successCount, int failureCount)
        {
            SuccessCount = successCount;
            FailureCount = failureCount;
        }

        public override string ToString()
        {
            return $"AssetBatchLoadResult(Success={SuccessCount}, Failed={FailureCount})";
        }
    }
}


