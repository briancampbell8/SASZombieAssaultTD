// ====================================================================================================
//  FILE: RSBatchLoadResult.cs
//  PATH: ./Engine/Resources/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the RSBatchLoadResult module.
//
//  RESPONSIBILITIES:
//      - Provide ToString() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
/*
File:    AssetBatchLoadResult.cs
Author:  BDC
Created: 2026-02-08
Purpose: Represents the result of a batch asset loading operation.
Notes:   Contains success and failure counts for pipeline operations.
*/

using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Assets
{
    ///<summary>
    ///Represents the outcome of loading multiple assets in a batch.
    ///</summary>
    public sealed class AssetBatchLoadResult
    {
        ///<summary>
        ///Number of assets successfully loaded.
        ///</summary>
        public int SuccessCount { get; }

        ///<summary>
        ///Number of assets that failed to load.
        ///</summary>
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



