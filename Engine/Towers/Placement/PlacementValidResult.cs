// =====================================================================================================
//  FILE: PlacementValidResult.cs
//  PATH: Engine/Towers/Placement/PlacementValidResult.cs
//  SUBSYSTEM: Towers/Placement
//
//  ROLE:
//      Serves as the structural data container encapsulating the success status, 
//      diagnostic error messages, and tracking warnings for a tower placement validation operation.
//
//  RESPONSIBILITIES:
//      - Track overall validity state of a placement request.
//      - Collect and expose structured diagnostic error string logs.
//      - Collect and expose non-breaking environmental layout warning logs.
//
//  NON-RESPONSIBILITIES:
//      - Evaluating game rules, player currency pools, or grid coordinates directly.
//      - Executing fallback engine UI popups or rendering failure text blocks.
// =====================================================================================================

using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Towers.Placement
{
    /// <summary>
    /// Validation result containing errors and warnings for tower placement requests.
    /// </summary>
    public class PlacementValidResult
    {
        /// <summary>
        /// Gets or sets whether the placement execution constraints were successfully satisfied.
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Gets the accumulated collection of structural breaking failure error messages.
        /// </summary>
        public List<string> Errors { get; } = new();

        /// <summary>
        /// Gets the accumulated collection of non-breaking environmental layout notifications.
        /// </summary>
        public List<string> Warnings { get; } = new();

        /// <summary>
        /// appends a deterministic error message log entry and updates validity status to false.
        /// </summary>
        /// <param name="error">The diagnostic failure description string segment.</param>
        public void AddError(string error)
        {
            if (IsValid)
            {
                IsValid = false;
            }
            Errors.Add(error);
        }

        /// <summary>
        /// Appends a non-breaking environmental notification message entry to the warning collection.
        /// </summary>
        /// <param name="warning">The conditional system warning description string segment.</param>
        public void AddWarning(string warning)
        {
            Warnings.Add(warning);
        }
    }
}
