/*
Program Name: SASZombieAssaultTD
File Path: Engine\Gameplay\BattlefieldType.cs
Purpose: Enumeration of all battlefield types in the game.
Features: 8 battlefields with unique characteristics and biomes.
*/

namespace SASZombieAssaultTD.Engine.Gameplay
{
    /// <summary>
    /// Enumeration of all battlefield types in SAS Zombie Assault TD.
    /// P120-07: Defines the 8 battlefields from MapFlow documentation.
    /// </summary>
    public enum BattlefieldType
    {
        /// <summary>
        /// Mean Street - Narrow urban street with rooftop positions.
        /// Biome: Urban street
        /// Pathing: Linear street path
        /// Features: Rooftop tiles for SAS soldiers
        /// Difficulty: Early-game pacing
        /// </summary>
        MeanStreet,

        /// <summary>
        /// Sub-Zero - Mountain refuge camp in snow biome.
        /// Biome: Snow/mountain refuge
        /// Pathing: Wide early defense, multi-lane later
        /// Features: Slow-movement tiles (snow)
        /// Difficulty: Mid-game progression
        /// </summary>
        SubZero,

        /// <summary>
        /// Dead Warehouse - Cramped indoor warehouse.
        /// Biome: Indoor warehouse
        /// Pathing: Narrow corridors
        /// Features: High-density waves
        /// Difficulty: Path manipulation focus
        /// </summary>
        DeadWarehouse,

        /// <summary>
        /// Shop Til You Drop - Open floor retail store.
        /// Biome: Open floor retail
        /// Pathing: Player-created pathing
        /// Features: High turret placement freedom
        /// Difficulty: Killbox-focused gameplay
        /// </summary>
        ShopTilYouDrop,

        /// <summary>
        /// Killtop - Vegetated hilltop with multi-directional spawns.
        /// Biome: Vegetated hilltop
        /// Pathing: Multi-directional spawns
        /// Features: Elevation props
        /// Difficulty: Chaotic wave pacing
        /// </summary>
        Killtop,

        /// <summary>
        /// Touchdown - Stadium with curved paths.
        /// Biome: Stadium
        /// Pathing: Curved paths
        /// Features: Barrier props
        /// Difficulty: Mid-game spike
        /// </summary>
        Touchdown,

        /// <summary>
        /// Cleanup On Aisle 13 - Grocery store with aisle-based choke points.
        /// Biome: Grocery store
        /// Pathing: Single dominant spawn direction
        /// Features: Aisle-based choke points
        /// Difficulty: Strong funneling potential
        /// </summary>
        Cleanup,

        /// <summary>
        /// Outbreak Mansion - Mansion and garden with final boss.
        /// Biome: Mansion + garden
        /// Pathing: Multiple natural choke points
        /// Features: Final boss map (Ruin)
        /// Difficulty: High-intensity late waves
        /// </summary>
        OutbreakMansion,
        SomeValue
    }

    /// <summary>
    /// Extension methods for BattlefieldType.
    /// </summary>
    public static class BattlefieldTypeExtensions
    {
        /// <summary>
        /// Gets the display name for a battlefield type.
        /// </summary>
        /// <param name="battlefield">The battlefield type.</param>
        /// <returns>The display name.</returns>
        public static string GetDisplayName(this BattlefieldType battlefield)
        {
            return battlefield switch
            {
                BattlefieldType.MeanStreet => "Mean Street",
                BattlefieldType.SubZero => "Sub-Zero",
                BattlefieldType.DeadWarehouse => "Dead Warehouse",
                BattlefieldType.ShopTilYouDrop => "Shop Til You Drop",
                BattlefieldType.Killtop => "Killtop",
                BattlefieldType.Touchdown => "Touchdown",
                BattlefieldType.Cleanup => "Cleanup On Aisle 13",
                BattlefieldType.OutbreakMansion => "Outbreak Mansion",
                _ => battlefield.ToString()
            };
        }

        /// <summary>
        /// Gets the biome description for a battlefield type.
        /// </summary>
        /// <param name="battlefield">The battlefield type.</param>
        /// <returns>The biome description.</returns>
        public static string GetBiome(this BattlefieldType battlefield)
        {
            return battlefield switch
            {
                BattlefieldType.MeanStreet => "Urban Street",
                BattlefieldType.SubZero => "Snow/Mountain Refuge",
                BattlefieldType.DeadWarehouse => "Indoor Warehouse",
                BattlefieldType.ShopTilYouDrop => "Open Floor Retail",
                BattlefieldType.Killtop => "Vegetated Hilltop",
                BattlefieldType.Touchdown => "Stadium",
                BattlefieldType.Cleanup => "Grocery Store",
                BattlefieldType.OutbreakMansion => "Mansion + Garden",
                _ => "Unknown"
            };
        }

        /// <summary>
        /// Gets the difficulty tier for a battlefield type.
        /// </summary>
        /// <param name="battlefield">The battlefield type.</param>
        /// <returns>The difficulty tier (1-8).</returns>
        public static int GetDifficultyTier(this BattlefieldType battlefield)
        {
            return battlefield switch
            {
                BattlefieldType.MeanStreet => 1,
                BattlefieldType.SubZero => 2,
                BattlefieldType.DeadWarehouse => 3,
                BattlefieldType.ShopTilYouDrop => 4,
                BattlefieldType.Killtop => 5,
                BattlefieldType.Touchdown => 6,
                BattlefieldType.Cleanup => 7,
                BattlefieldType.OutbreakMansion => 8,
                _ => 1
            };
        }

        /// <summary>
        /// Checks if a battlefield is a final boss map.
        /// </summary>
        /// <param name="battlefield">The battlefield type.</param>
        /// <returns>True if this is a final boss map.</returns>
        public static bool IsFinalBossMap(this BattlefieldType battlefield)
        {
            return battlefield == BattlefieldType.OutbreakMansion;
        }
    }
}
