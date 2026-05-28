/*
Program Name: SASZombieAssaultTD
File Path: Engine\Scenes\Battlefields\ShopTilYouDropScene.cs
Purpose: Shop Til You Drop battlefield scene implementation.
Features: Open floor plan, player-created pathing, high turret placement freedom, killbox gameplay.
*/

using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Gameplay;
using SASZombieAssaultTD.Engine.VectorMath;
using System;

namespace SASZombieAssaultTD.Engine.Scenes.Battlefields
{
    /// <summary>
    /// Shop Til You Drop battlefield scene.
    /// P120-09: Implements the Shop Til You Drop battlefield with open floor plan and player-created pathing.
    /// </summary>
    public class ShopTilYouDropScene : BattlefieldScene
    {
        public ShopTilYouDropScene() 
            : base(BattlefieldType.ShopTilYouDrop, "Assets/Maps/ShopTilYouDrop.json")
        {
        }

        /// <summary>
        /// Sets up spawn nodes for Shop Til You Drop.
        /// Open floor with flexible spawn points for player-created pathing.
        /// </summary>
        protected override void SetupSpawnNodes()
        {
            base.SetupSpawnNodes();

            // Western wall spawn (single dominant direction)
            SpawnNodes.Add(new Vector3(50f, 150f, 0f));
            SpawnNodes.Add(new Vector3(50f, 200f, 0f));
            SpawnNodes.Add(new Vector3(50f, 250f, 0f));
            SpawnNodes.Add(new Vector3(50f, 300f, 0f));
            SpawnNodes.Add(new Vector3(50f, 350f, 0f));

            // Flexible spawn points for killbox creation
            SpawnNodes.Add(new Vector3(200f, 150f, 0f));
            SpawnNodes.Add(new Vector3(200f, 350f, 0f));
            SpawnNodes.Add(new Vector3(400f, 150f, 0f));
            SpawnNodes.Add(new Vector3(400f, 350f, 0f));

            Engine.Diagnostics.DebugLogger.Log("Info", $"ShopTilYouDropScene: Setup {SpawnNodes.Count} spawn nodes");
        }

        /// <summary>
        /// Sets up exit nodes for Shop Til You Drop.
        /// Eastern exit for player-defined pathing.
        /// </summary>
        protected override void SetupExitNodes()
        {
            base.SetupExitNodes();

            ExitNodes.Add(new Vector3(900f, 200f, 0f));
            ExitNodes.Add(new Vector3(900f, 300f, 0f));

            Engine.Diagnostics.DebugLogger.Log("Info", $"ShopTilYouDropScene: Setup {ExitNodes.Count} exit nodes");
        }

        /// <summary>
        /// Sets up camera bounds for Shop Til You Drop.
        /// Open retail floor plan.
        /// </summary>
        protected override void SetupCameraBounds()
        {
            base.SetupCameraBounds();

            SetCameraBounds(new Vector3(0f, 0f, 0f), new Vector3(1000f, 500f, 0f));

            Engine.Diagnostics.DebugLogger.Log("Info", $"ShopTilYouDropScene: Camera bounds set to {CameraBoundsMin} to {CameraBoundsMax}");
        }

        public override BattlefieldType GetBattlefieldType()
        {
            NotImplementedGuard.Hit("NOT_IMPLEMENTED");

            throw new System.NotImplementedException();
        }

        private object TheContainingMember()
        {
            NotImplementedGuard.Hit("NOT_IMPLEMENTED");

            throw new NotImplementedException();
        }

        private object TheContainingType()
        {
            NotImplementedGuard.Hit("NOT_IMPLEMENTED");

            throw new NotImplementedException();
        }
    }
}
