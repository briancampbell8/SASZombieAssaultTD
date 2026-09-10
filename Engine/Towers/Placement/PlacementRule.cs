// =====================================================================================================
//  FILE: PlacementRule.cs
//  PATH: Engine/Towers/Placement/PlacementRule.cs
//  SUBSYSTEM: Towers/Placement
//
//  ROLE:
//      Defines the validation contract for tower placement rules. Concrete implementations
//      check specific environmental constraints (e.g., grid bounds, blockages, occupancy).
//
//  RESPONSIBILITIES:
//      - Provide an abstract evaluation surface for single-rule validation.
//      - Enforce a standardized result container generation routine.
//      - Standardize the retrieval of formatted diagnostic error descriptions.
//
//  NON-RESPONSIBILITIES:
//      - Directly caching grid spatial state variables.
//      - Altering real-world engine tower allocation tracking tables.
// =====================================================================================================

using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Towers.Placement
{
    /// <summary>
    /// Base class for all tower placement rule checks.
    /// </summary>
    public abstract class PlacementRule
    {
        /// <summary>
        /// Evaluates whether the rule criteria are met at the given grid position.
        /// </summary>
        /// <param name="gridPosition">The targeted grid coordinate placement vector.</param>
        /// <param name="towerData">The structural configuration definitions of the tower ECSEntityCore.</param>
        /// <returns>True if the grid cell context satisfies rule criteria; otherwise, false.</returns>
        public abstract bool IsValid(Vector3Int gridPosition, TowerData towerData);

        /// <summary>
        /// Runs full rule evaluations and constructs a structured result payload.
        /// </summary>
        /// <param name="gridPosition">The targeted grid coordinate placement vector.</param>
        /// <param name="towerData">The structural configuration definitions of the tower ECSEntityCore.</param>
        /// <returns>A populated verification record indicating success or failure reasons.</returns>
        public virtual PlacementValidResult Validate(Vector3Int gridPosition, TowerData towerData)
        {
            var result = new PlacementValidResult { IsValid = IsValid(gridPosition, towerData) };

            if (!result.IsValid)
            {
                result.AddError(GetFailureMessage(gridPosition, towerData));
            }

            return result;
        }

        /// <summary>
        /// Compiles a descriptive fallback failure message if validation returns false.
        /// </summary>
        /// <param name="gridPosition">The targeted grid coordinate placement vector.</param>
        /// <param name="towerData">The structural configuration definitions of the tower ECSEntityCore.</param>
        /// <returns>A human-readable diagnostic error text fragment.</returns>
        public abstract string GetFailureMessage(Vector3Int gridPosition, TowerData towerData);
    }
}
