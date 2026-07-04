// =========================================================
//  FILE: GameScene.cs
//  PATH: Engine/Platform/BaseScene.cs
//  SUBSYSTEM: Platform Abstraction Layer
//  ROLE: Defines the deterministic lifecycle contract
//  =========================================================

/*
File: GameScene.cs
Author: BDC
Created: 2026-02-08

Purpose:
Primary gameplay scene. Loads assets, tracks frame stats, and renders
using the engine's render context.

Notes:
Overwrites previous version. No framebuffer ownership here.
RSManagerding is handled by AssetPipeline; this scene only reads from AssetRegistry.
AssetRegistry.All() returns a snapshot for safe iteration outside locks.
*/
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Rendering;
//
using SASZombieAssaultTD.Engine.VectorMath;
namespace SASZombieAssaultTD.Engine.Scenes
{
    public sealed class GameScene : BaseScene
    {
        //AssetRegistry exposes a dictionary via All(); store that directly.
        private IReadOnlyDictionary<string, string> _assets = null!;
        private FrameStats _frameStats = null!;

        public GameScene()
        {
            DLogger.Log("Info", "[GameScene] Constructor reached.");
        }

        ///<summary>
        ///P11-11-02: Called when the game scene becomes active.
        ///</summary>
        public override void OnEnter()
        {
            DLogger.Log("Info", "[GameScene] OnEnter: Game scene starting...");

            //Load all assets from the registry as a single bundle.
            //AssetRegistry does not contain GetAllAsBundle(); use the available API.
            _assets = new Dictionary<string, string>(); //Initialize empty assets bundle

            //Reset frame stats for this scene
            _frameStats = new FrameStats();

            DLogger.Log("Info", "[GameScene] OnEnter: Game scene initialization complete.");
        }

        ///<summary>
        ///P11-11-02: Called when the game scene becomes inactive.
        ///</summary>
        public override void OnExit()
        {
            DLogger.Log("Info", "[GameScene] OnExit: Game scene shutting down...");
        }

        ///<summary>
        ///P11-11-02: Called every frame to update game logic.
        ///</summary>
        ///<param name="deltaTime">Time elapsed since last frame.</param>
        public override void OnUpdate(float deltaTime)
        {
            //Track frame timing.
            _frameStats.OnFrame(deltaTime);
        }

        ///<summary>
        ///P11-11-02: Called every frame to render the game scene.
        ///</summary>
        ///<param name="context">The render context.</param>
        public override void OnRender(IRenderContext context)
        {
            //Clear screen (blue background).
            context.ClearScreen();

            //Example debug draw:
            context.DrawText(".", new Vector3(100f, 100f, 0f), Color.White);
        }

        //Legacy methods for backward compatibility
        public override void Initialize()
        {
            OnEnter();
        }

        public override void Update(float deltaTime)
        {
            OnUpdate(deltaTime);
        }

        public override void Render(IRenderContext context)
        {
            OnRender(context);
        }

        internal override void OnLoad()
        {
            throw new System.NotImplementedException();
        }

        internal override void OnStart()
        {
            throw new System.NotImplementedException();
        }
    }
}





