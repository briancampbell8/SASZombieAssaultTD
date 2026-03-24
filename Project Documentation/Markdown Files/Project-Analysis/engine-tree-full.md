Folder PATH listing
Volume serial number is 1096-EFE4
E:\BDC\PROJECTS\SASZOMBIEASSAULTTD\ENGINE
¦   engine-tree.txt
¦   GameRoot.cs
¦   
+---Animation
¦   ¦   AnimationClip.cs
¦   ¦   AnimationController.cs
¦   ¦   AnimationDebugTools.cs
¦   ¦   AnimationParameters.cs
¦   ¦   AnimationPerformanceAnalyzer.cs
¦   ¦   AnimationStateInspector.cs
¦   ¦   AnimationStateMachine.cs
¦   ¦   AnimationTrack.cs
¦   ¦   AnimationTransitionDebug.cs
¦   ¦   
¦   +---BlendTrees
¦   ¦   ¦   BlendParameters.cs
¦   ¦   ¦   BlendTree.cs
¦   ¦   ¦   BlendTreeSerializer.cs
¦   ¦   ¦   BlendTreeValidator.cs
¦   ¦   ¦   IBlendNode.cs
¦   ¦   ¦   
¦   ¦   +---Nodes
¦   ¦           LinearBlendNode.cs
¦   ¦           SingleClipNode.cs
¦   ¦           
¦   +---Diagnostics
¦   ¦       AnimationDiagnostics.cs
¦   ¦       
¦   +---Events
¦   ¦       AnimationEvent.cs
¦   ¦       AnimationEventContext.cs
¦   ¦       AnimationEventDispatcher.cs
¦   ¦       AnimationEventECSIntegration.cs
¦   ¦       AnimationEventSerializer.cs
¦   ¦       AnimationEventTrack.cs
¦   ¦       AnimationEventValidator.cs
¦   ¦       IAnimationEventReceiver.cs
¦   ¦       
¦   +---States
¦   ¦       AttackState.cs
¦   ¦       IAnimationState.cs
¦   ¦       IdleState.cs
¦   ¦       JumpState.cs
¦   ¦       MoveState.cs
¦   ¦       
¦   +---Visualization
¦           AnimationStateVisualization.cs
¦           
+---Audio
¦       AudioEngine.cs
¦       MusicTrack.cs
¦       SoundEffect.cs
¦       
+---Compatibility
+---Components
¦       ColliderShapes.cs
¦       CollisionComponent.cs
¦       InventoryComponent.cs
¦       ParticleEmitterComponent.cs
¦       PhysicsComponent.cs
¦       SpriteComponent.cs
¦       StatsComponent.cs
¦       TransformComponent.cs
¦       TriggerComponent.cs
¦       UIComponent.cs
¦       
+---Core
¦   ¦   GameLoop.cs
¦   ¦   IProgram.cs
¦   ¦   TimingController.cs
¦   ¦   
¦   +---Diagnostics
¦   ¦       ManagerDiagnostics.cs
¦   ¦       
¦   +---Input
¦   ¦       InputModule.cs
¦   ¦       InputModuleTest.cs
¦   ¦       
¦   +---Interfaces
¦   ¦       IDebugRenderer.cs
¦   ¦       IGameStateMachine.cs
¦   ¦       IManager.cs
¦   ¦       ISystemRegistry.cs
¦   ¦       
¦   +---Logging
¦   ¦       DebugLogger.cs
¦   ¦       ILogger.cs
¦   ¦       
¦   +---Managers
¦   ¦       BaseManager.cs
¦   ¦       InputManager.cs
¦   ¦       RenderManager.cs
¦   ¦       SystemManager.cs
¦   ¦       UpdateManager.cs
¦   ¦       
¦   +---Math
¦   ¦       VectorTypes.cs
¦   ¦       
¦   +---Registry
¦   ¦       SystemRegistry.cs
¦   ¦       
¦   +---Timing
¦           FrameDiagnostics.cs
¦           TimingModule.cs
¦           
+---ECS
¦   ¦   BaseComponent.cs
¦   ¦   ECSDebugInspector.cs
¦   ¦   ECSVerificationReport.cs
¦   ¦   ECSVerificationSuite.cs
¦   ¦   ECSWorld.cs
¦   ¦   Entity.cs
¦   ¦   EntityFactory.cs
¦   ¦   EntityManager.cs
¦   ¦   IComponent.cs
¦   ¦   IEntityComponent.cs
¦   ¦   IGameSystem.cs
¦   ¦   
¦   +---Components
¦   ¦       ActiveComponent.cs
¦   ¦       AnimationControllerComponent.cs
¦   ¦       DamageComponent.cs
¦   ¦       EnemyTypeComponent.cs
¦   ¦       HealthComponent.cs
¦   ¦       MovementComponent.cs
¦   ¦       NavAgentComponent.cs
¦   ¦       RenderableComponent.cs
¦   ¦       ScoreComponent.cs
¦   ¦       TransformComponent.cs
¦   ¦       
¦   +---Systems
¦   ¦       AISystem.cs
¦   ¦       AnimationSystem.cs
¦   ¦       CollisionSystem.cs
¦   ¦       CombatSystem.cs
¦   ¦       ISystem.cs
¦   ¦       NavigationSystem.cs
¦   ¦       RenderSystem.cs
¦   ¦       ScoringSystem.cs
¦   ¦       
¦   +---Testing
¦           ECSTestSuite.cs
¦           
+---Entities
¦       Enemy.cs
¦       EnemyData.cs
¦       Entity.cs
¦       Projectile.cs
¦       Soldier.cs
¦       Turret.cs
¦       
+---Input
¦       IInputSource.cs
¦       InputEvent.cs
¦       InputEventTypes.cs
¦       InputManager.cs
¦       InputPlayback.cs
¦       InputRecorder.cs
¦       KeyboardInputSource.cs
¦       MouseInputSource.cs
¦       
+---Managers
¦       EnemyManager.cs
¦       EntityManager.cs
¦       ProjectileManager.cs
¦       WaveManager.cs
¦       
+---Memory
¦       MemoryTracker.cs
¦       ObjectPool.cs
¦       
+---Navigation
¦       AStarPathfinder.cs
¦       FlowField.cs
¦       NavigationDebugRenderer.cs
¦       NavigationGrid.cs
¦       NavigationMigrationHelper.cs
¦       NavigationVerificationSuite.cs
¦       
+---Pathfinding
¦       PathfindingOptimizer.cs
¦       
+---Performance
¦       DebugOverlay.cs
¦       FramePacer.cs
¦       PerformanceProfiler.cs
¦       
+---Physics
¦       AABBShape.cs
¦       CapsuleShape.cs
¦       CircleShape.cs
¦       ColliderComponent.cs
¦       CollisionDebugRenderer.cs
¦       CollisionEvent.cs
¦       CollisionShape.cs
¦       CollisionVerificationSuite.cs
¦       PhysicsTuning.cs
¦       SpatialPartitionGrid.cs
¦       
+---Platform
¦       Win32Window.cs
¦       
+---Rendering
¦   ¦   DebugOverlay.cs
¦   ¦   Framebuffer.cs
¦   ¦   IRenderContext.cs
¦   ¦   RenderCommandQueue.cs
¦   ¦   RenderDiagnostics.cs
¦   ¦   Renderer.cs
¦   ¦   RenderInitValidator.cs
¦   ¦   RenderPipelineConfig.cs
¦   ¦   RenderQueue.cs
¦   ¦   RenderSurface.cs
¦   ¦   Sprite.cs
¦   ¦   SpriteBatch.cs
¦   ¦   SpriteBatchOptimizer.cs
¦   ¦   SpriteBatchRenderer.cs
¦   ¦   TextRenderer.cs
¦   ¦   Texture2D.cs
¦   ¦   TextureCache.cs
¦   ¦   WindowHost.cs
¦   ¦   
¦   +---Debug
¦   ¦       PathDebugRenderer.cs
¦   ¦       
¦   +---Zombies
¦           ZombieRenderer.cs
¦           
+---Save
¦       SaveData.cs
¦       SaveSystem.cs
¦       
+---Scene
¦       Entity.cs
¦       GameplayScene.cs
¦       MainMenuScene.cs
¦       Scene.cs
¦       SceneManager.cs
¦       
+---Scenes
¦   ¦   BaseScene.cs
¦   ¦   GameScene.cs
¦   ¦   LoadingScene.cs
¦   ¦   MainMenuScene.cs
¦   ¦   PauseScene.cs
¦   ¦   SceneManager.cs
¦   ¦   SceneTransitionTest.cs
¦   ¦   
¦   +---TestScenes
¦           MeanStreetsTest.cs
¦           
+---State
¦       AdvancedStateMachine.cs
¦       BootState.cs
¦       EnhancedStateMachine.cs
¦       GameEvent.cs
¦       GameplayState.cs
¦       GameStateType.cs
¦       IGameState.cs
¦       MainMenuState.cs
¦       PausedState.cs
¦       README.md
¦       StateBuilder.cs
¦       StateDebugger.cs
¦       StateFactory.cs
¦       StateMachine.cs
¦       StateMachineExtensions.cs
¦       StateMachineIntegration.cs
¦       StateMachineProfiler.cs
¦       StateMachineTest.cs
¦       StateTransition.cs
¦       
+---Systems
¦   ¦   AnimationSystem.cs
¦   ¦   CameraSystem.cs
¦   ¦   CollisionSystem.cs
¦   ¦   DefaultPlatformRenderer.cs
¦   ¦   EventLogSystem.cs
¦   ¦   GameStateManager.cs
¦   ¦   InputRouter.cs
¦   ¦   LoggingSystem.cs
¦   ¦   ParticleSystem.cs
¦   ¦   PathfindingSystem.cs
¦   ¦   PhysicsSystem.cs
¦   ¦   RenderingSystem.cs
¦   ¦   RenderQueue.cs
¦   ¦   TriggerSystem.cs
¦   ¦   UISystem.cs
¦   ¦   WaveAnalyticsSummary.cs
¦   ¦   WaveSystem.cs
¦   ¦   
¦   +---Achievements
¦   ¦   ¦   AchievementDefinition.cs
¦   ¦   ¦   AchievementInstance.cs
¦   ¦   ¦   AchievementListRenderer.cs
¦   ¦   ¦   AchievementPopupRenderer.cs
¦   ¦   ¦   AchievementRarity.cs
¦   ¦   ¦   ChallengeDefinition.cs
¦   ¦   ¦   ChallengeInstance.cs
¦   ¦   ¦   
¦   ¦   +---UI
¦   ¦           AchievementListRenderer.cs
¦   ¦           AchievementPopupRenderer.cs
¦   ¦           
¦   +---AI
¦   ¦   ¦   AIController.cs
¦   ¦   ¦   
¦   ¦   +---Behaviors
¦   ¦   ¦       BasicChaseBehavior.cs
¦   ¦   ¦       
¦   ¦   +---Blackboard
¦   ¦           Blackboard.cs
¦   ¦           
¦   +---Assets
¦   ¦       AssetBatchLoadResult.cs
¦   ¦       AssetBundle.cs
¦   ¦       AssetDiscovery.cs
¦   ¦       AssetHandle.cs
¦   ¦       AssetInitializer.cs
¦   ¦       AssetKey.cs
¦   ¦       AssetLoadContext.cs
¦   ¦       AssetLoadResult.cs
¦   ¦       AssetManager.cs
¦   ¦       AssetMetadata.cs
¦   ¦       AssetPipeline.cs
¦   ¦       AssetRegistry.cs
¦   ¦       AssetSource.cs
¦   ¦       AssetType.cs
¦   ¦       AssetUtils.cs
¦   ¦       AssetValidation.cs
¦   ¦       DataLoader.cs
¦   ¦       TextureLoader.cs
¦   ¦       
¦   +---Audio
¦   ¦       AudioSystem.cs
¦   ¦       MusicTrack.cs
¦   ¦       SoundEffect.cs
¦   ¦       
¦   +---Challenges
¦   ¦       ChallengeDifficulty.cs
¦   ¦       ChallengeListRenderer.cs
¦   ¦       ChallengeTrackerRenderer.cs
¦   ¦       
¦   +---Combat
¦   ¦   ¦   KillFeedSystem.cs
¦   ¦   ¦   
¦   ¦   +---Statistics
¦   ¦           KillFeedStatistics.cs
¦   ¦           
¦   +---Diagnostics
¦   ¦       DebugLogger.cs
¦   ¦       FrameStats.cs
¦   ¦       HeartbeatMonitor.cs
¦   ¦       Timing.cs
¦   ¦       
¦   +---Enemies
¦   ¦   ¦   EnemyDefinition.cs
¦   ¦   ¦   EnemyDefinitionRegistry.cs
¦   ¦   ¦   EnemyDefinitionValidator.cs
¦   ¦   ¦   EnemySystem.cs
¦   ¦   ¦   ZombieAI.cs
¦   ¦   ¦   ZombieMovement.cs
¦   ¦   ¦   ZombieSpawner.cs
¦   ¦   ¦   
¦   ¦   +---Tests
¦   ¦       ¦   EnemyDefinitionRegistryTests.cs
¦   ¦       ¦   EnemyDefinitionValidatorTests.cs
¦   ¦       ¦   SASZombieAssaultTD.code-workspace
¦   ¦       ¦   SASZombieAssaultTD.Engine.Systems.Enemies.Tests.csproj
¦   ¦       ¦   
¦   ¦       +---bin
¦   ¦       ¦   +---Debug
¦   ¦       ¦       +---net10.0-windows
¦   ¦       ¦           ¦   Microsoft.TestPlatform.CommunicationUtilities.dll
¦   ¦       ¦           ¦   Microsoft.TestPlatform.CoreUtilities.dll
¦   ¦       ¦           ¦   Microsoft.TestPlatform.CrossPlatEngine.dll
¦   ¦       ¦           ¦   Microsoft.TestPlatform.PlatformAbstractions.dll
¦   ¦       ¦           ¦   Microsoft.TestPlatform.Utilities.dll
¦   ¦       ¦           ¦   Microsoft.VisualStudio.CodeCoverage.Shim.dll
¦   ¦       ¦           ¦   Microsoft.VisualStudio.TestPlatform.Common.dll
¦   ¦       ¦           ¦   Microsoft.VisualStudio.TestPlatform.ObjectModel.dll
¦   ¦       ¦           ¦   Newtonsoft.Json.dll
¦   ¦       ¦           ¦   NuGet.Frameworks.dll
¦   ¦       ¦           ¦   SASZombieAssaultTD.deps.json
¦   ¦       ¦           ¦   SASZombieAssaultTD.dll
¦   ¦       ¦           ¦   SASZombieAssaultTD.Engine.Systems.Enemies.Tests.deps.json
¦   ¦       ¦           ¦   SASZombieAssaultTD.Engine.Systems.Enemies.Tests.dll
¦   ¦       ¦           ¦   SASZombieAssaultTD.Engine.Systems.Enemies.Tests.pdb
¦   ¦       ¦           ¦   SASZombieAssaultTD.Engine.Systems.Enemies.Tests.runtimeconfig.json
¦   ¦       ¦           ¦   SASZombieAssaultTD.exe
¦   ¦       ¦           ¦   SASZombieAssaultTD.pdb
¦   ¦       ¦           ¦   SASZombieAssaultTD.runtimeconfig.json
¦   ¦       ¦           ¦   testhost.dll
¦   ¦       ¦           ¦   testhost.exe
¦   ¦       ¦           ¦   xunit.abstractions.dll
¦   ¦       ¦           ¦   xunit.assert.dll
¦   ¦       ¦           ¦   xunit.core.dll
¦   ¦       ¦           ¦   xunit.execution.dotnet.dll
¦   ¦       ¦           ¦   xunit.runner.visualstudio.testadapter.dll
¦   ¦       ¦           ¦   
¦   ¦       ¦           +---Assets
¦   ¦       ¦           ¦   ¦   logo.png
¦   ¦       ¦           ¦   ¦   
¦   ¦       ¦           ¦   +---AtlasSource
¦   ¦       ¦           ¦   ¦   +---PremiumItems
¦   ¦       ¦           ¦   ¦           cryo.png
¦   ¦       ¦           ¦   ¦           healthup.png
¦   ¦       ¦           ¦   ¦           incendiary.png
¦   ¦       ¦           ¦   ¦           longbow.png
¦   ¦       ¦           ¦   ¦           mines.png
¦   ¦       ¦           ¦   ¦           necro.png
¦   ¦       ¦           ¦   ¦           nuke.png
¦   ¦       ¦           ¦   ¦           typhoon.png
¦   ¦       ¦           ¦   ¦           
¦   ¦       ¦           ¦   +---Battlefields
¦   ¦       ¦           ¦   ¦       CleanupOnAisle13.png
¦   ¦       ¦           ¦   ¦       DeadWarehouse.png
¦   ¦       ¦           ¦   ¦       Killtop.png
¦   ¦       ¦           ¦   ¦       MeanStreets.png
¦   ¦       ¦           ¦   ¦       OutbreakMansion.png
¦   ¦       ¦           ¦   ¦       ShopTilYouDrop.png
¦   ¦       ¦           ¦   ¦       SubZero.png
¦   ¦       ¦           ¦   ¦       Touchdown.png
¦   ¦       ¦           ¦   ¦       
¦   ¦       ¦           ¦   +---FlashExtract
¦   ¦       ¦           ¦   ¦   +---PremiumItems
¦   ¦       ¦           ¦   ¦           cryo.png
¦   ¦       ¦           ¦   ¦           healthup.png
¦   ¦       ¦           ¦   ¦           incendiary.png
¦   ¦       ¦           ¦   ¦           longbow.png
¦   ¦       ¦           ¦   ¦           mines.png
¦   ¦       ¦           ¦   ¦           necro.png
¦   ¦       ¦           ¦   ¦           nuke.png
¦   ¦       ¦           ¦   ¦           typhoon.png
¦   ¦       ¦           ¦   ¦           
¦   ¦       ¦           ¦   +---Sprites
¦   ¦       ¦           ¦       +---PremiumItems
¦   ¦       ¦           ¦       ¦       B-52.png
¦   ¦       ¦           ¦       ¦       cryo.png
¦   ¦       ¦           ¦       ¦       healthup.png
¦   ¦       ¦           ¦       ¦       incendiary.png
¦   ¦       ¦           ¦       ¦       longbow.png
¦   ¦       ¦           ¦       ¦       mines.png
¦   ¦       ¦           ¦       ¦       necro.png
¦   ¦       ¦           ¦       ¦       nuke.png
¦   ¦       ¦           ¦       ¦       typhoon.png
¦   ¦       ¦           ¦       ¦       
¦   ¦       ¦           ¦       +---Soldiers
¦   ¦       ¦           ¦               grenadier.png
¦   ¦       ¦           ¦               medic.png
¦   ¦       ¦           ¦               rifleman.png
¦   ¦       ¦           ¦               sawgunner.png
¦   ¦       ¦           ¦               sniper.png
¦   ¦       ¦           ¦               
¦   ¦       ¦           +---cs
¦   ¦       ¦           ¦       Microsoft.TestPlatform.CommunicationUtilities.resources.dll
¦   ¦       ¦           ¦       Microsoft.TestPlatform.CoreUtilities.resources.dll
¦   ¦       ¦           ¦       Microsoft.TestPlatform.CrossPlatEngine.resources.dll
¦   ¦       ¦           ¦       Microsoft.VisualStudio.TestPlatform.Common.resources.dll
¦   ¦       ¦           ¦       Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll
¦   ¦       ¦           ¦       
¦   ¦       ¦           +---de
¦   ¦       ¦           ¦       Microsoft.TestPlatform.CommunicationUtilities.resources.dll
¦   ¦       ¦           ¦       Microsoft.TestPlatform.CoreUtilities.resources.dll
¦   ¦       ¦           ¦       Microsoft.TestPlatform.CrossPlatEngine.resources.dll
¦   ¦       ¦           ¦       Microsoft.VisualStudio.TestPlatform.Common.resources.dll
¦   ¦       ¦           ¦       Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll
¦   ¦       ¦           ¦       
¦   ¦       ¦           +---es
¦   ¦       ¦           ¦       Microsoft.TestPlatform.CommunicationUtilities.resources.dll
¦   ¦       ¦           ¦       Microsoft.TestPlatform.CoreUtilities.resources.dll
¦   ¦       ¦           ¦       Microsoft.TestPlatform.CrossPlatEngine.resources.dll
¦   ¦       ¦           ¦       Microsoft.VisualStudio.TestPlatform.Common.resources.dll
¦   ¦       ¦           ¦       Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll
¦   ¦       ¦           ¦       
¦   ¦       ¦           +---fr
¦   ¦       ¦           ¦       Microsoft.TestPlatform.CommunicationUtilities.resources.dll
¦   ¦       ¦           ¦       Microsoft.TestPlatform.CoreUtilities.resources.dll
¦   ¦       ¦           ¦       Microsoft.TestPlatform.CrossPlatEngine.resources.dll
¦   ¦       ¦           ¦       Microsoft.VisualStudio.TestPlatform.Common.resources.dll
¦   ¦       ¦           ¦       Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll
¦   ¦       ¦           ¦       
¦   ¦       ¦           +---it
¦   ¦       ¦           ¦       Microsoft.TestPlatform.CommunicationUtilities.resources.dll
¦   ¦       ¦           ¦       Microsoft.TestPlatform.CoreUtilities.resources.dll
¦   ¦       ¦           ¦       Microsoft.TestPlatform.CrossPlatEngine.resources.dll
¦   ¦       ¦           ¦       Microsoft.VisualStudio.TestPlatform.Common.resources.dll
¦   ¦       ¦           ¦       Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll
¦   ¦       ¦           ¦       
¦   ¦       ¦           +---ja
¦   ¦       ¦           ¦       Microsoft.TestPlatform.CommunicationUtilities.resources.dll
¦   ¦       ¦           ¦       Microsoft.TestPlatform.CoreUtilities.resources.dll
¦   ¦       ¦           ¦       Microsoft.TestPlatform.CrossPlatEngine.resources.dll
¦   ¦       ¦           ¦       Microsoft.VisualStudio.TestPlatform.Common.resources.dll
¦   ¦       ¦           ¦       Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll
¦   ¦       ¦           ¦       
¦   ¦       ¦           +---ko
¦   ¦       ¦           ¦       Microsoft.TestPlatform.CommunicationUtilities.resources.dll
¦   ¦       ¦           ¦       Microsoft.TestPlatform.CoreUtilities.resources.dll
¦   ¦       ¦           ¦       Microsoft.TestPlatform.CrossPlatEngine.resources.dll
¦   ¦       ¦           ¦       Microsoft.VisualStudio.TestPlatform.Common.resources.dll
¦   ¦       ¦           ¦       Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll
¦   ¦       ¦           ¦       
¦   ¦       ¦           +---pl
¦   ¦       ¦           ¦       Microsoft.TestPlatform.CommunicationUtilities.resources.dll
¦   ¦       ¦           ¦       Microsoft.TestPlatform.CoreUtilities.resources.dll
¦   ¦       ¦           ¦       Microsoft.TestPlatform.CrossPlatEngine.resources.dll
¦   ¦       ¦           ¦       Microsoft.VisualStudio.TestPlatform.Common.resources.dll
¦   ¦       ¦           ¦       Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll
¦   ¦       ¦           ¦       
¦   ¦       ¦           +---pt-BR
¦   ¦       ¦           ¦       Microsoft.TestPlatform.CommunicationUtilities.resources.dll
¦   ¦       ¦           ¦       Microsoft.TestPlatform.CoreUtilities.resources.dll
¦   ¦       ¦           ¦       Microsoft.TestPlatform.CrossPlatEngine.resources.dll
¦   ¦       ¦           ¦       Microsoft.VisualStudio.TestPlatform.Common.resources.dll
¦   ¦       ¦           ¦       Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll
¦   ¦       ¦           ¦       
¦   ¦       ¦           +---ru
¦   ¦       ¦           ¦       Microsoft.TestPlatform.CommunicationUtilities.resources.dll
¦   ¦       ¦           ¦       Microsoft.TestPlatform.CoreUtilities.resources.dll
¦   ¦       ¦           ¦       Microsoft.TestPlatform.CrossPlatEngine.resources.dll
¦   ¦       ¦           ¦       Microsoft.VisualStudio.TestPlatform.Common.resources.dll
¦   ¦       ¦           ¦       Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll
¦   ¦       ¦           ¦       
¦   ¦       ¦           +---tr
¦   ¦       ¦           ¦       Microsoft.TestPlatform.CommunicationUtilities.resources.dll
¦   ¦       ¦           ¦       Microsoft.TestPlatform.CoreUtilities.resources.dll
¦   ¦       ¦           ¦       Microsoft.TestPlatform.CrossPlatEngine.resources.dll
¦   ¦       ¦           ¦       Microsoft.VisualStudio.TestPlatform.Common.resources.dll
¦   ¦       ¦           ¦       Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll
¦   ¦       ¦           ¦       
¦   ¦       ¦           +---zh-Hans
¦   ¦       ¦           ¦       Microsoft.TestPlatform.CommunicationUtilities.resources.dll
¦   ¦       ¦           ¦       Microsoft.TestPlatform.CoreUtilities.resources.dll
¦   ¦       ¦           ¦       Microsoft.TestPlatform.CrossPlatEngine.resources.dll
¦   ¦       ¦           ¦       Microsoft.VisualStudio.TestPlatform.Common.resources.dll
¦   ¦       ¦           ¦       Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll
¦   ¦       ¦           ¦       
¦   ¦       ¦           +---zh-Hant
¦   ¦       ¦                   Microsoft.TestPlatform.CommunicationUtilities.resources.dll
¦   ¦       ¦                   Microsoft.TestPlatform.CoreUtilities.resources.dll
¦   ¦       ¦                   Microsoft.TestPlatform.CrossPlatEngine.resources.dll
¦   ¦       ¦                   Microsoft.VisualStudio.TestPlatform.Common.resources.dll
¦   ¦       ¦                   Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll
¦   ¦       ¦                   
¦   ¦       +---obj
¦   ¦       ¦   ¦   project.assets.json
¦   ¦       ¦   ¦   project.nuget.cache
¦   ¦       ¦   ¦   SASZombieAssaultTD.Engine.Systems.Enemies.Tests.csproj.nuget.dgspec.json
¦   ¦       ¦   ¦   SASZombieAssaultTD.Engine.Systems.Enemies.Tests.csproj.nuget.g.props
¦   ¦       ¦   ¦   SASZombieAssaultTD.Engine.Systems.Enemies.Tests.csproj.nuget.g.targets
¦   ¦       ¦   ¦   
¦   ¦       ¦   +---Debug
¦   ¦       ¦       +---net10.0-windows
¦   ¦       ¦           ¦   .NETCoreApp,Version=v10.0.AssemblyAttributes.cs
¦   ¦       ¦           ¦   SASZombi.CE30576C.Up2Date
¦   ¦       ¦           ¦   SASZombieAssaultTD.Engine.Systems.Enemies.Tests.AssemblyInfo.cs
¦   ¦       ¦           ¦   SASZombieAssaultTD.Engine.Systems.Enemies.Tests.AssemblyInfoInputs.cache
¦   ¦       ¦           ¦   SASZombieAssaultTD.Engine.Systems.Enemies.Tests.assets.cache
¦   ¦       ¦           ¦   SASZombieAssaultTD.Engine.Systems.Enemies.Tests.csproj.AssemblyReference.cache
¦   ¦       ¦           ¦   SASZombieAssaultTD.Engine.Systems.Enemies.Tests.csproj.CoreCompileInputs.cache
¦   ¦       ¦           ¦   SASZombieAssaultTD.Engine.Systems.Enemies.Tests.csproj.FileListAbsolute.txt
¦   ¦       ¦           ¦   SASZombieAssaultTD.Engine.Systems.Enemies.Tests.dll
¦   ¦       ¦           ¦   SASZombieAssaultTD.Engine.Systems.Enemies.Tests.GeneratedMSBuildEditorConfig.editorconfig
¦   ¦       ¦           ¦   SASZombieAssaultTD.Engine.Systems.Enemies.Tests.genruntimeconfig.cache
¦   ¦       ¦           ¦   SASZombieAssaultTD.Engine.Systems.Enemies.Tests.GlobalUsings.g.cs
¦   ¦       ¦           ¦   SASZombieAssaultTD.Engine.Systems.Enemies.Tests.pdb
¦   ¦       ¦           ¦   SASZombieAssaultTD.Engine.Systems.Enemies.Tests.sourcelink.json
¦   ¦       ¦           ¦   
¦   ¦       ¦           +---ref
¦   ¦       ¦           ¦       SASZombieAssaultTD.Engine.Systems.Enemies.Tests.dll
¦   ¦       ¦           ¦       
¦   ¦       ¦           +---refint
¦   ¦       ¦                   SASZombieAssaultTD.Engine.Systems.Enemies.Tests.dll
¦   ¦       ¦                   
¦   ¦       +---SASZombieAssaultTD.Engine.Systems.Enemies.Tests
¦   +---Events
¦   ¦       AchievementUnlockedEvent.cs
¦   ¦       ChallengeCompletedEvent.cs
¦   ¦       EntityDiedEvent.cs
¦   ¦       EntityRespawnedEvent.cs
¦   ¦       EventBus.cs
¦   ¦       EventManager.cs
¦   ¦       GameLoadedEvent.cs
¦   ¦       GameOverEvent.cs
¦   ¦       IEventBus.cs
¦   ¦       IEventListener.cs
¦   ¦       IGameEvent.cs
¦   ¦       ItemAddedEvent.cs
¦   ¦       ItemPickupEvent.cs
¦   ¦       ItemRemovedEvent.cs
¦   ¦       ItemUseEvent.cs
¦   ¦       KillAttributedEvent.cs
¦   ¦       LevelUpEvent.cs
¦   ¦       ResourceChangedEvent.cs
¦   ¦       RoundCompletedEvent.cs
¦   ¦       ScoreUpdatedEvent.cs
¦   ¦       TriggerEnterEvent.cs
¦   ¦       TriggerExitEvent.cs
¦   ¦       ZoneEnteredEvent.cs
¦   ¦       ZoneExitedEvent.cs
¦   ¦       
¦   +---Gameplay
¦   ¦   ¦   AchievementSystem.cs
¦   ¦   ¦   AnimationSystem.cs
¦   ¦   ¦   AnimationTriggerSystem.cs
¦   ¦   ¦   CameraSystem.cs
¦   ¦   ¦   ChallengeSystem.cs
¦   ¦   ¦   DamageSystem.cs
¦   ¦   ¦   DeathEffectSystem.cs
¦   ¦   ¦   DeathSystem.cs
¦   ¦   ¦   EnemyBase.cs
¦   ¦   ¦   EntityRemovalSystem.cs
¦   ¦   ¦   GameOverSystem.cs
¦   ¦   ¦   InventorySystem.cs
¦   ¦   ¦   KillAttributionSystem.cs
¦   ¦   ¦   MetaProgressionSystem.cs
¦   ¦   ¦   PathfindingSystem.cs
¦   ¦   ¦   PickupSystem.cs
¦   ¦   ¦   PlayerMovementConfig.cs
¦   ¦   ¦   PlayerSystem.cs
¦   ¦   ¦   ProjectileSystem.cs
¦   ¦   ¦   ResourceSystem.cs
¦   ¦   ¦   ResourceType.cs
¦   ¦   ¦   RespawnSystem.cs
¦   ¦   ¦   RoundResetSystem.cs
¦   ¦   ¦   ScoreSystem.cs
¦   ¦   ¦   StatsTrackerSystem.cs
¦   ¦   ¦   TowerBase.cs
¦   ¦   ¦   TowerSystem.cs
¦   ¦   ¦   WaveController.cs
¦   ¦   ¦   ZoneTriggerSystem.cs
¦   ¦   ¦   
¦   ¦   +---Animation
¦   ¦   ¦       AnimationClip.cs
¦   ¦   ¦       AnimationFrame.cs
¦   ¦   ¦       AnimationPlayer.cs
¦   ¦   ¦       
¦   ¦   +---Events
¦   ¦   ¦       GameplayEventRouter.cs
¦   ¦   ¦       GameplayEventType.cs
¦   ¦   ¦       
¦   ¦   +---Interaction
¦   ¦   ¦       IInteractable.cs
¦   ¦   ¦       InteractionSystem.cs
¦   ¦   ¦       InteractionType.cs
¦   ¦   ¦       
¦   ¦   +---Inventory
¦   ¦   ¦       InventorySystem.cs
¦   ¦   ¦       
¦   ¦   +---Weapons
¦   ¦   ¦       WeaponSystem.cs
¦   ¦   ¦       
¦   ¦   +---Zombies
¦   ¦           BasicZombie.cs
¦   ¦           ZombieBase.cs
¦   ¦           ZombieController.cs
¦   ¦           
¦   +---Hazards
¦   ¦   +---Analytics
¦   ¦   ¦       HazardAnalytics.cs
¦   ¦   ¦       HazardAnalyticsValidator.cs
¦   ¦   ¦       HazardDebugOverlay.cs
¦   ¦   ¦       HazardEffectResult.cs
¦   ¦   ¦       
¦   ¦   +---Core
¦   ¦   ¦       HazardEffect.cs
¦   ¦   ¦       HazardManager.cs
¦   ¦   ¦       HazardZone.cs
¦   ¦   ¦       StandardHazardZone.cs
¦   ¦   ¦       
¦   ¦   +---Nuke
¦   ¦   ¦       NukeBlast.cs
¦   ¦   ¦       NukeCloud.cs
¦   ¦   ¦       RadiationExposure.cs
¦   ¦   ¦       
¦   ¦   +---Visual
¦   ¦           HazardVisualSystem.cs
¦   ¦           
¦   +---Inventory
¦   ¦       ItemDatabase.cs
¦   ¦       ItemDefinition.cs
¦   ¦       ItemInstance.cs
¦   ¦       
¦   +---Meta
¦   ¦       MetaProgressionRenderer.cs
¦   ¦       
¦   +---Pathfinding
¦   ¦       Pathfinder.cs
¦   ¦       PathLoader.cs
¦   ¦       PathNetwork.cs
¦   ¦       PathNode.cs
¦   ¦       
¦   +---Persistence
¦   ¦       LoadSystem.cs
¦   ¦       SaveGameData.cs
¦   ¦       SaveManager.cs
¦   ¦       SaveSerializer.cs
¦   ¦       
¦   +---Player
¦   ¦       PlayerStatsData.cs
¦   ¦       
¦   +---Rendering
¦   ¦       RenderContext.cs
¦   ¦       RenderSettings.cs
¦   ¦       RenderSystem.cs
¦   ¦       
¦   +---Resources
¦   ¦       ResourceSystem.cs
¦   ¦       
¦   +---Save
¦   ¦       SaveFileInfo.cs
¦   ¦       
¦   +---SaveLoad
¦   ¦       FileUtils.cs
¦   ¦       JsonUtils.cs
¦   ¦       SaveData.cs
¦   ¦       SaveManager.cs
¦   ¦       SaveVersioning.cs
¦   ¦       
¦   +---Settings
¦   ¦       SettingsSystem.cs
¦   ¦       
¦   +---UI
¦   ¦   ¦   AchievementListRenderer.cs
¦   ¦   ¦   AchievementPopupRenderer.cs
¦   ¦   ¦   Button.cs
¦   ¦   ¦   ChallengeListRenderer.cs
¦   ¦   ¦   ChallengeTrackerRenderer.cs
¦   ¦   ¦   GameOverScreenRenderer.cs
¦   ¦   ¦   GameOverScreenStatistics.cs
¦   ¦   ¦   HealthBarRenderer.cs
¦   ¦   ¦   HUD.cs
¦   ¦   ¦   InventoryPanelRenderer.cs
¦   ¦   ¦   ItemTooltipRenderer.cs
¦   ¦   ¦   KillFeedSystem.cs
¦   ¦   ¦   LayoutSystem.cs
¦   ¦   ¦   Menus.cs
¦   ¦   ¦   MetaProgressionRenderer.cs
¦   ¦   ¦   Panel.cs
¦   ¦   ¦   PointF.cs
¦   ¦   ¦   ResourceDisplayRenderer.cs
¦   ¦   ¦   RespawnCountdownRenderer.cs
¦   ¦   ¦   RespawnCountdownStatistics.cs
¦   ¦   ¦   RoundCompleteNotificationRenderer.cs
¦   ¦   ¦   RoundCompleteStatistics.cs
¦   ¦   ¦   ScoreDisplayStatistics.cs
¦   ¦   ¦   ScoreDisplaySystem.cs
¦   ¦   ¦   UIElement.cs
¦   ¦   ¦   UIElementBase.cs
¦   ¦   ¦   UIRoot.cs
¦   ¦   ¦   UISystem.cs
¦   ¦   ¦   
¦   ¦   +---Assets
¦   ¦   ¦       UIAssetLoader.cs
¦   ¦   ¦       UIFont.cs
¦   ¦   ¦       UISprite.cs
¦   ¦   ¦       
¦   ¦   +---Debug
¦   ¦   ¦       UIDebugInspector.cs
¦   ¦   ¦       UIDebugOverlay.cs
¦   ¦   ¦       
¦   ¦   +---Input
¦   ¦   ¦       UIFocusManager.cs
¦   ¦   ¦       UIInputRouter.cs
¦   ¦   ¦       UIInputState.cs
¦   ¦   ¦       
¦   ¦   +---Layout
¦   ¦   ¦       UILayoutSystem.cs
¦   ¦   ¦       UILayoutTypes.cs
¦   ¦   ¦       
¦   ¦   +---Rendering
¦   ¦   ¦       UIBatcher.cs
¦   ¦   ¦       UIRenderContext.cs
¦   ¦   ¦       UIRenderer.cs
¦   ¦   ¦       
¦   ¦   +---Statistics
¦   ¦   ¦       HealthBarRendererStatistics.cs
¦   ¦   ¦       KillFeedStatistics.cs
¦   ¦   ¦       
¦   ¦   +---Styles
¦   ¦   ¦       UIStyle.cs
¦   ¦   ¦       UIStyleResolver.cs
¦   ¦   ¦       UIStyleSheet.cs
¦   ¦   ¦       
¦   ¦   +---Widgets
¦   ¦           UIButton.cs
¦   ¦           UIPanel.cs
¦   ¦           UIText.cs
¦   ¦           UIWidgetBase.cs
¦   ¦           
¦   +---World
¦           WorldStateSystem.cs
¦           
+---Tools
¦       AppendixScripts.cs
¦       AutomationEnhancements.cs
¦       CleanupScripts.cs
¦       StructureGenerator.cs
¦       
+---UI
¦       Label.cs
¦       Panel.cs
¦       UIElement.cs
¦       UIManager.cs
¦       
+---Utility
¦       Logger.cs
¦       MathHelper.cs
¦       Randomizer.cs
¦       Time.cs
¦       
+---Window
        Window.cs
        
