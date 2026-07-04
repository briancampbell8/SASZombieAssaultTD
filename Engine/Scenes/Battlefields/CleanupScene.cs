// =========================================================
//  FILE: CleanupScene.cs
//  PATH: Engine/Platform/BaseScene.cs
//  SUBSYSTEM: Platform Abstraction Layer
//  ROLE: Defines the deterministic lifecycle contract
//  =========================================================

/*
Program Name: SASZombieAssaultTD
File Path: Engine\Scenes\Battlefields\CleanupScene.cs
Purpose: Cleanup On Aisle 13 battlefield scene implementation.
Features: Grocery store biome, single dominant spawn direction, aisle-based choke points, strong funneling.
*/

//
using System;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Gameplay;
using SASZombieAssaultTD.Engine.VectorMath;
namespace SASZombieAssaultTD.Engine.Scenes.Battlefields

{
    ///<summary>
    ///Cleanup On Aisle 13 battlefield scene.
    ///P120-09: Implements the Cleanup On Aisle 13 battlefield with aisle-based choke points.
    ///</summary>
    public class CleanupScene : BattlefieldScene
    {
        public CleanupScene()
            : base(BattlefieldType.Cleanup, "Assets/Maps/Cleanup.json")
        {
        }

        ///<summary>
        ///Sets up spawn nodes for Cleanup On Aisle 13.
        ///Single dominant spawn direction from western wall.
        ///</summary>
        protected override void SetupSpawnNodes(object dlogger)
        {
            base.SetupSpawnNodes();

            //Western wall dominant spawn
            SpawnNodes.Add(new Vector3(50f, 150f, 0f));
            SpawnNodes.Add(new Vector3(50f, 200f, 0f));
            SpawnNodes.Add(new Vector3(50f, 250f, 0f));
            SpawnNodes.Add(new Vector3(50f, 300f, 0f));
            SpawnNodes.Add(new Vector3(50f, 350f, 0f));

            //Aisle-based choke point spawns
            SpawnNodes.Add(new Vector3(200f, 180f, 0f));
            SpawnNodes.Add(new Vector3(200f, 220f, 0f));
            SpawnNodes.Add(new Vector3(200f, 280f, 0f));
            SpawnNodes.Add(new Vector3(200f, 320f, 0f));

            Dlogger.Log(
                "Info", $"CleanupScene: Setup {SpawnNodes.Count} spawn nodes");
        }

        ///<summary>
        ///Sets up exit nodes for Cleanup On Aisle 13.
        ///Eastern mall exit.
        ///</summary>
        protected override void SetupExitNodes()
        {
            base.SetupExitNodes();

            ExitNodes.Add(new Vector3(850f, 250f, 0f));

            Dlogger.Log(LogSubsystems.Scenes, LogSubsystems.Scenes, LogLevel.Info,
                "Info", $"CleanupScene: Setup {ExitNodes.Count} exit nodes");
        }

        ///<summary>
        ///Sets up camera bounds for Cleanup On Aisle 13.
        ///Grocery store layout.
        ///</summary>
        protected override void SetupCameraBounds()
        {
            base.SetupCameraBounds();

            SetCameraBounds(new Vector3(0f, 0f, 0f), new Vector3(900f, 500f, 0f));

            Dlogger.Log("Info", $"CleanupScene: Camera bounds set to {CameraBoundsMin} to {CameraBoundsMax}");
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

        internal override void OnUnload()
        {
            throw new NotImplementedException();
        }

        internal override void OnLoad()
        {
            throw new NotImplementedException();
        }

        internal override void OnStart()
        {
            throw new NotImplementedException();
        }
    }

    internal class Dlogger
    {
        internal static void Log(LogSubsystems scenes, string v1, string v2)
        {
            NotImplementedGuard.Hit("NOT_IMPLEMENTED");
            throw new NotImplementedException();
        }

        internal static void Log(LogSubsystems scenes1, LogSubsystems scenes2, LogLevel info, string v1, string v2)
        {
            NotImplementedGuard.Hit("NOT_IMPLEMENTED");
            throw new NotImplementedException();
        }

        internal static void Log(string v1, string v2, string v)
        {
            NotImplementedGuard.Hit("NOT_IMPLEMENTED");
            throw new NotImplementedException();
        }

        internal static void Log(string v, string v1)
        {
            throw new NotImplementedException();
        }

        internal static void Log(LogSubsystems eCS, LogLevel iNFO, string v)
        {
            throw new NotImplementedException();
        }

        internal static void Log(Exception ex, string v)
        {
            throw new NotImplementedException();
        }

        internal static void Log(LogLevel warning, string v)
        {
            throw new NotImplementedException();
        }

        private object TheContainingType()
        {
            NotImplementedGuard.Hit("NOT_IMPLEMENTED");
            throw new NotImplementedException();
        }
    }
}
