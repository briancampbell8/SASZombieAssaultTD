/*
Program Name: SASZombieAssaultTD
File Path: Engine\Scenes\Battlefields\CleanupScene.cs
Purpose: Cleanup On Aisle 13 battlefield scene implementation.
Features: Grocery store biome, single dominant spawn direction, aisle-based choke points, strong funneling.
*/

using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Gameplay;
using SASZombieAssaultTD.Engine.VectorMath;
using System;

namespace SASZombieAssaultTD.Engine.Scenes.Battlefields
{
    /// <summary>
    /// Cleanup On Aisle 13 battlefield scene.
    /// P120-09: Implements the Cleanup On Aisle 13 battlefield with aisle-based choke points.
    /// </summary>
    public class CleanupScene : BattlefieldScene
    {
        public CleanupScene() 
            : base(BattlefieldType.Cleanup, "Assets/Maps/Cleanup.json")
        {
        }

        /// <summary>
        /// Sets up spawn nodes for Cleanup On Aisle 13.
        /// Single dominant spawn direction from western wall.
        /// </summary>
        protected override void SetupSpawnNodes()
        {
            base.SetupSpawnNodes();

            // Western wall dominant spawn
            SpawnNodes.Add(new Vector3(50f, 150f, 0f));
            SpawnNodes.Add(new Vector3(50f, 200f, 0f));
            SpawnNodes.Add(new Vector3(50f, 250f, 0f));
            SpawnNodes.Add(new Vector3(50f, 300f, 0f));
            SpawnNodes.Add(new Vector3(50f, 350f, 0f));

            // Aisle-based choke point spawns
            SpawnNodes.Add(new Vector3(200f, 180f, 0f));
            SpawnNodes.Add(new Vector3(200f, 220f, 0f));
            SpawnNodes.Add(new Vector3(200f, 280f, 0f));
            SpawnNodes.Add(new Vector3(200f, 320f, 0f));

            Engine.Diagnostics.DebugLogger.Log("Info", $"CleanupScene: Setup {SpawnNodes.Count} spawn nodes");
        }

        /// <summary>
        /// Sets up exit nodes for Cleanup On Aisle 13.
        /// Eastern mall exit.
        /// </summary>
        protected override void SetupExitNodes()
        {
            base.SetupExitNodes();

            ExitNodes.Add(new Vector3(850f, 250f, 0f));

            Engine.Diagnostics.DebugLogger.Log("Info", $"CleanupScene: Setup {ExitNodes.Count} exit nodes");
        }

        /// <summary>
        /// Sets up camera bounds for Cleanup On Aisle 13.
        /// Grocery store layout.
        /// </summary>
        protected override void SetupCameraBounds()
        {
            base.SetupCameraBounds();

            SetCameraBounds(new Vector3(0f, 0f, 0f), new Vector3(900f, 500f, 0f));

            Engine.Diagnostics.DebugLogger.Log("Info", $"CleanupScene: Camera bounds set to {CameraBoundsMin} to {CameraBoundsMax}");
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
