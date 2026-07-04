// =========================================================
//  FILE: SubZeroScene.cs
//  PATH: Engine/Platform/BaseScene.cs
//  SUBSYSTEM: Platform Abstraction Layer
//  ROLE: Defines the deterministic lifecycle contract
//  =========================================================

/*
Program Name: SASZombieAssaultTD
File Path: Engine\Scenes\Battlefields\SubZeroScene.cs
Purpose: Sub-Zero battlefield scene implementation.
Features: Snow biome, wide early defense area, multi-lane pressure, slow-movement tiles.
*/

using System;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Gameplay;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Scenes.Battlefields
{
    ///<summary>
    ///Sub-Zero battlefield scene.
    ///P120-09: Implements the Sub-Zero battlefield with snow biome and multi-lane pathing.
    ///</summary>
    public class SubZeroScene : BattlefieldScene
    {
        public SubZeroScene()
            : base(BattlefieldType.SubZero, "Assets/Maps/SubZero.json")
        {
        }

        ///<summary>
        ///Sets up spawn nodes for Sub-Zero.
        ///Wide early defense area with multi-lane pressure.
        ///</summary>
        protected override void SetupSpawnNodes(object dlogger)
        {
            base.SetupSpawnNodes();

            //Early wide spawn area
            SpawnNodes.Add(new Vector3(50f, 150f, 0f));
            SpawnNodes.Add(new Vector3(50f, 250f, 0f));
            SpawnNodes.Add(new Vector3(50f, 350f, 0f));

            //Multi-lane pressure points
            SpawnNodes.Add(new Vector3(200f, 150f, 0f));
            SpawnNodes.Add(new Vector3(200f, 250f, 0f));
            SpawnNodes.Add(new Vector3(200f, 350f, 0f));

            DLogger.Log("Info", $"SubZeroScene: Setup {SpawnNodes.Count} spawn nodes");
        }

        ///<summary>
        ///Sets up exit nodes for Sub-Zero.
        ///</summary>
        protected override void SetupExitNodes()
        {
            base.SetupExitNodes();

            ExitNodes.Add(new Vector3(850f, 200f, 0f));
            ExitNodes.Add(new Vector3(850f, 300f, 0f));

            DLogger.Log("Info", $"SubZeroScene: Setup {ExitNodes.Count} exit nodes");
        }

        ///<summary>
        ///Sets up camera bounds for Sub-Zero.
        ///Wide mountain refuge area.
        ///</summary>
        protected override void SetupCameraBounds()
        {
            base.SetupCameraBounds();

            SetCameraBounds(new Vector3(0f, 0f, 0f), new Vector3(950f, 500f, 0f));

            DLogger.Log("Info", $"SubZeroScene: Camera bounds set to {CameraBoundsMin} to {CameraBoundsMax}");
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

        internal override void OnLoad()
        {
            throw new NotImplementedException();
        }

        internal override void OnStart()
        {
            throw new NotImplementedException();
        }
    }
}
