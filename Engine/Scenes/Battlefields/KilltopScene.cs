/*
Program Name: SASZombieAssaultTD
File Path: Engine\Scenes\Battlefields\KilltopScene.cs
Purpose: Killtop battlefield scene implementation.
Features: Multi-directional spawns, vegetated hilltop biome, elevation props, chaotic wave pacing.
*/

using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Gameplay;
using SASZombieAssaultTD.Engine.VectorMath;
using System.Security.AccessControl;

namespace SASZombieAssaultTD.Engine.Scenes.Battlefields
{
    /// <summary>
    /// Killtop battlefield scene.
    /// P120-09: Implements the Killtop battlefield with multi-directional spawns and elevation.
    /// </summary>
    public class KilltopScene : BattlefieldScene
    {
        private object TheType;
        private object TheMember;

        public KilltopScene() 
            : base(BattlefieldType.Killtop, "Assets/Maps/Killtop.json")
        {
        }

        /// <summary>
        /// Sets up spawn nodes for Killtop.
        /// Multi-directional spawns from hilltop approaches.
        /// </summary>
        protected override void SetupSpawnNodes()
        {
            base.SetupSpawnNodes();

            // Northern approach
            SpawnNodes.Add(new Vector3(400f, 50f, 0f));
            SpawnNodes.Add(new Vector3(450f, 50f, 0f));
            SpawnNodes.Add(new Vector3(500f, 50f, 0f));

            // Eastern approach
            SpawnNodes.Add(new Vector3(850f, 200f, 0f));
            SpawnNodes.Add(new Vector3(850f, 250f, 0f));
            SpawnNodes.Add(new Vector3(850f, 300f, 0f));

            // Southern approach
            SpawnNodes.Add(new Vector3(400f, 450f, 0f));
            SpawnNodes.Add(new Vector3(450f, 450f, 0f));
            SpawnNodes.Add(new Vector3(500f, 450f, 0f));

            // Western approach
            SpawnNodes.Add(new Vector3(50f, 200f, 0f));
            SpawnNodes.Add(new Vector3(50f, 250f, 0f));
            SpawnNodes.Add(new Vector3(50f, 300f, 0f));

            Engine.Diagnostics.DebugLogger.Log("Info", $"KilltopScene: Setup {SpawnNodes.Count} spawn nodes (multi-directional)");
        }

        /// <summary>
        /// Sets up exit nodes for Killtop.
        /// Hilltop center exits.
        /// </summary>
        protected override void SetupExitNodes()
        {
            base.SetupExitNodes();

            ExitNodes.Add(new Vector3(450f, 240f, 0f));
            ExitNodes.Add(new Vector3(450f, 260f, 0f));

            Engine.Diagnostics.DebugLogger.Log("Info", $"KilltopScene: Setup {ExitNodes.Count} exit nodes");
        }

        /// <summary>
        /// Sets up camera bounds for Killtop.
        /// Vegetated hilltop area.
        /// </summary>
        protected override void SetupCameraBounds()
        {
            base.SetupCameraBounds();

            SetCameraBounds(new Vector3(0f, 0f, 0f), new Vector3(900f, 500f, 0f));

            Engine.Diagnostics.DebugLogger.Log("Info", $"KilltopScene: Camera bounds set to {CameraBoundsMin} to {CameraBoundsMax}");
        }

        public override BattlefieldType GetBattlefieldType()
        {
            NotImplementedGuard.Hit("NOT_IMPLEMENTED");

            throw new System.NotImplementedException();
        }
    }
}
