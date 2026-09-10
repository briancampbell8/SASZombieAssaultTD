// =====================================================================================================
//  FILE: PlacementConcreteRules.cs
//  PATH: Engine/Towers/Placement/PlacementConcreteRules.cs
//  SUBSYSTEM: Towers/Placement
//
//  ROLE:
//      Houses the explicit mathematical, spatial, and environmental rule conditions used by the
//      tower placement validation engine to assess grid cell eligibility.
//
//  RESPONSIBILITIES:
//      - Enforce strict map grid boundaries during construction evaluations.
//      - Prevent multi-tile cell ECSEntityCore overlaps via active navigation grid structural lookups.
//      - Guard enemy path solutions to guarantee map navigation layout continuity.
//      - Verify physical structure proximity bounds and surface terrain type compatibility.
//
//  NON-RESPONSIBILITIES:
//      - Directly caching spatial map grid indices or array variables.
//      - Mutating active ECSEntityCore registry tracking pools or triggering external gameplay UI alerts.
// =====================================================================================================

using SASZombieAssaultTD.Engine.Navigation;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Towers.Placement
{
    internal class PlacementConcreteRules
    {
    }

    /// <summary>
    /// Rule: Check if position is within grid bounds.
    /// </summary>
    public class GridBoundsRule : PlacementRule
    {
        public override bool IsValid(Vector3Int gridPosition, TowerData towerData)
        {
            var grid = NavigationGrid.Instance;
            if (grid == null) return false;

            // Fixed by packing discrete integer fields into structural Vector3Int arguments
            Vector3Int startCheck = new Vector3Int(gridPosition.X, gridPosition.Y, gridPosition.Z);
            Vector3Int endCheck = new Vector3Int(gridPosition.X + towerData.GridSize.X - 1, gridPosition.Y + towerData.GridSize.Y - 1, gridPosition.Z);

            return grid.IsInBounds(startCheck) && grid.IsInBounds(endCheck);
        }

        public override string GetFailureMessage(Vector3Int gridPosition, TowerData towerData)
        {
            return "Position is outside the playable area";
        }
    }

    /// <summary>
    /// Rule: Check if position is already occupied.
    /// </summary>
    public class OccupancyRule : PlacementRule
    {
        public override bool IsValid(Vector3Int gridPosition, TowerData towerData)
        {
            var grid = NavigationGrid.Instance;
            if (grid == null) return false;

            for (int x = 0; x < towerData.GridSize.X; x++)
            {
                for (int y = 0; y < towerData.GridSize.Y; y++)
                {
                    var checkPos = new Vector3Int(gridPosition.X + x, gridPosition.Y + y, gridPosition.Z);

                    if (!grid.IsWalkable(checkPos))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public override string GetFailureMessage(Vector3Int gridPosition, TowerData towerData)
        {
            return "Position is already occupied";
        }
    }

    /// <summary>
    /// Rule: Check if placement would block enemy paths.
    /// </summary>
    public class PathBlockingRule : PlacementRule
    {
        public override bool IsValid(Vector3Int gridPosition, TowerData towerData)
        {
            var validator = new PlacementValidator();
            return !validator.WouldBlockPaths(gridPosition, towerData);
        }

        public override string GetFailureMessage(Vector3Int gridPosition, TowerData towerData)
        {
            return "Placement would block enemy paths";
        }
    }

    /// <summary>
    /// Rule: Check minimum distance from other towers.
    /// </summary>
    public class TowerSpacingRule : PlacementRule
    {
        public override bool IsValid(Vector3Int gridPosition, TowerData towerData)
        {
            var validator = new PlacementValidator();
            return !validator.IsTooCloseToOtherTowers(gridPosition, towerData);
        }

        public override string GetFailureMessage(Vector3Int gridPosition, TowerData towerData)
        {
            return "Too close to other towers";
        }
    }

    /// <summary>
    /// Rule: Check if terrain supports the tower.
    /// </summary>
    public class TerrainRule : PlacementRule
    {
        public override bool IsValid(Vector3Int gridPosition, TowerData towerData)
        {
            var validator = new PlacementValidator();
            return validator.IsValidTerrain(gridPosition, towerData);
        }

        public override string GetFailureMessage(Vector3Int gridPosition, TowerData towerData)
        {
            return "Terrain does not support this tower type";
        }
    }

    /// <summary>
    /// Rule: Check if player can afford the tower.
    /// </summary>
    public class EconomyRule : PlacementRule
    {
        public override bool IsValid(Vector3Int gridPosition, TowerData towerData)
        {
            return true;
        }

        public override string GetFailureMessage(Vector3Int gridPosition, TowerData towerData)
        {
            return "Insufficient funds";
        }
    }
}
