# Engine Build Report
Generated: 03/02/2026 16:29:14

## Directory Structure

Folder PATH listing
Volume serial number is 1096-EFE4
E:\BDC\PROJECTS\SASZOMBIEASSAULTTD\ENGINE
ª   engine-tree.txt
ª   EngineBootstrap.cs
ª   EngineMath.cs
ª   Input
ª   PATCHES.md
ª   temp_dirs.txt
ª   
+---Achievements
ª   ª   AchievementCategory.cs
ª   ª   AchievementDefinition.cs
ª   ª   AchievementInstance.cs
ª   ª   AchievementListRenderer.cs
ª   ª   AchievementPopupRenderer.cs
ª   ª   AchievementRarity.cs
ª   ª   ChallengeDefinition.cs
ª   ª   ChallengeInstance.cs
ª   ª   
ª   +---UI
ª           AchievementListRenderer.cs
ª           AchievementPopupRenderer.cs
ª           
+---AI
ª   ª   AIController.cs
ª   ª   
ª   +---Behaviors
ª   ª       BasicChaseBehavior.cs
ª   ª       
ª   +---Blackboard
ª           Blackboard.cs
ª           
+---Animation
ª   ª   AnimationClip.cs
ª   ª   AnimationConditionOperator.cs
ª   ª   AnimationController.cs
ª   ª   AnimationStateMachine.cs
ª   ª   AnimationTrack.cs
ª   ª   AnimationTransition.cs
ª   ª   AnimationTransitionDebug.cs
ª   ª   
ª   +---AnimationComponents
ª   ª       AnimationComponent.cs
ª   ª       AnimationControllerComponent.cs
ª   ª       AnimationFrame.cs
ª   ª       AnimationPlayer.cs
ª   ª       AnimationStateInspector.cs
ª   ª       AnimationStateSnapshot.cs
ª   ª       AnimationStateVisualization.cs
ª   ª       AnimationTypes.cs
ª   ª       AttackState.cs
ª   ª       IAnimationState.cs
ª   ª       IdleState.cs
ª   ª       JumpState.cs
ª   ª       MoveState.cs
ª   ª       
ª   +---AnimationEvents
ª   ª       AnimationEvent.cs
ª   ª       AnimationEventContext.cs
ª   ª       AnimationEventDispatcher.cs
ª   ª       AnimationEventTrack.cs
ª   ª       IAnimationEventReceiver.cs
ª   ª       
ª   +---AnimationSystems
ª   ª       AnimationSystem.cs
ª   ª       AnimationTriggerSystem.cs
ª   ª       
ª   +---BlendTree
ª   ª       BlendParameters.cs
ª   ª       BlendTree.cs
ª   ª       BlendTreeNode.cs
ª   ª       BlendTreeSerializer.cs
ª   ª       BlendTreeValidationResult.cs
ª   ª       BlendTreeValidator.cs
ª   ª       IBlendNode.cs
ª   ª       LinearBlendNode.cs
ª   ª       SingleClipNode.cs
ª   ª       TwoDBlendNode.cs
ª   ª       
ª   +---Integration
ª           AnimationECSIntegration.cs
ª           AnimationEventECSIntegration.cs
ª           AnimationEventTypes.cs
ª           
+---Audio
ª       CoreAudioEngine.cs
ª       CoreSoundEffect.cs
ª       
+---Camera
ª       CameraSystem.cs
ª       
+---Challenges
ª       ChallengeCategory.cs
ª       ChallengeDifficulty.cs
ª       ChallengeListRenderer.cs
ª       ChallengeTrackerRenderer.cs
ª       ChallengeType.cs
ª       
+---Combat
ª   ª   KillFeedSystem.cs
ª   ª   
ª   +---Statistics
ª           KillFeedStatistics.cs
ª           
+---Components
ª       ActiveComponent.cs
ª       CollisionComponent.cs
ª       ComponentTypes.cs
ª       DamageComponent.cs
ª       EnemyTypeComponent.cs
ª       HealthComponent.cs
ª       InventoryComponent.cs
ª       LifetimeComponent.cs
ª       MovementComponent.cs
ª       ParticleEmitterComponent.cs
ª       RenderableComponent.cs
ª       ScoreComponent.cs
ª       SpriteComponent.cs
ª       StatsComponent.cs
ª       TransformComponent.cs
ª       TriggerComponent.cs
ª       UIComponent.cs
ª       
+---Core
ª       CoreColor.cs
ª       CoreRectangle.cs
ª       CoreTimeStep.cs
ª       CoreTransform.cs
ª       
+---Diagnostics
ª       DebugLogger.cs
ª       DebugOverlay.cs
ª       FrameDiagnostics.cs
ª       FrameStats.cs
ª       HeartbeatMonitor.cs
ª       ManagerDiagnostics.cs
ª       Timing.cs
ª       
+---ECS
ª   ª   BaseComponent.cs
ª   ª   CollisionTypes.cs
ª   ª   ECSComponent.cs
ª   ª   ECSDebugInspector.cs
ª   ª   ECSEntity.cs
ª   ª   ECSManager.cs
ª   ª   ECSSystem.cs
ª   ª   ECSVerificationReport.cs
ª   ª   ECSWorld.cs
ª   ª   EntityFactory.cs
ª   ª   EntityManager.cs
ª   ª   IComponent.cs
ª   ª   IEntityComponent.cs
ª   ª   IGameSystem.cs
ª   ª   ISystem.cs
ª   ª   
ª   +---Components
ª           Components.cs
ª           
+---Enemies
ª   ª   EnemyDefinition.cs
ª   ª   EnemyDefinitionRegistry.cs
ª   ª   EnemyDefinitionValidator.cs
ª   ª   EnemySystem.cs
ª   ª   ZombieAI.cs
ª   ª   ZombieMovement.cs
ª   ª   ZombieSpawner.cs
ª   ª   
ª   +---Tests
ª       ª   EnemyDefinitionRegistryTests.cs
ª       ª   EnemyDefinitionValidatorTests.cs
ª       ª   SASZombieAssaultTD.code-workspace
ª       ª   SASZombieAssaultTD.Engine.Systems.Enemies.Tests.csproj
ª       ª   
ª       +---bin
ª       ª   +---Debug
ª       ª   ª   +---net10.0-windows
ª       ª   ª   ª   ª   Microsoft.TestPlatform.CommunicationUtilities.dll
ª       ª   ª   ª   ª   Microsoft.TestPlatform.CoreUtilities.dll
ª       ª   ª   ª   ª   Microsoft.TestPlatform.CrossPlatEngine.dll
ª       ª   ª   ª   ª   Microsoft.TestPlatform.PlatformAbstractions.dll
ª       ª   ª   ª   ª   Microsoft.TestPlatform.Utilities.dll
ª       ª   ª   ª   ª   Microsoft.VisualStudio.CodeCoverage.Shim.dll
ª       ª   ª   ª   ª   Microsoft.VisualStudio.TestPlatform.Common.dll
ª       ª   ª   ª   ª   Microsoft.VisualStudio.TestPlatform.ObjectModel.dll
ª       ª   ª   ª   ª   Newtonsoft.Json.dll
ª       ª   ª   ª   ª   NuGet.Frameworks.dll
ª       ª   ª   ª   ª   SASZombieAssaultTD.deps.json
ª       ª   ª   ª   ª   SASZombieAssaultTD.dll
ª       ª   ª   ª   ª   SASZombieAssaultTD.Engine.Systems.Enemies.Tests.deps.json
ª       ª   ª   ª   ª   SASZombieAssaultTD.Engine.Systems.Enemies.Tests.dll
ª       ª   ª   ª   ª   SASZombieAssaultTD.Engine.Systems.Enemies.Tests.pdb
ª       ª   ª   ª   ª   SASZombieAssaultTD.Engine.Systems.Enemies.Tests.runtimeconfig.json
ª       ª   ª   ª   ª   SASZombieAssaultTD.exe
ª       ª   ª   ª   ª   SASZombieAssaultTD.pdb
ª       ª   ª   ª   ª   SASZombieAssaultTD.runtimeconfig.json
ª       ª   ª   ª   ª   testhost.dll
ª       ª   ª   ª   ª   testhost.exe
ª       ª   ª   ª   ª   xunit.abstractions.dll
ª       ª   ª   ª   ª   xunit.assert.dll
ª       ª   ª   ª   ª   xunit.core.dll
ª       ª   ª   ª   ª   xunit.execution.dotnet.dll
ª       ª   ª   ª   ª   xunit.runner.visualstudio.testadapter.dll
ª       ª   ª   ª   ª   
ª       ª   ª   ª   +---Assets
ª       ª   ª   ª   ª   ª   logo.png
ª       ª   ª   ª   ª   ª   
ª       ª   ª   ª   ª   +---AtlasSource
ª       ª   ª   ª   ª   ª   +---PremiumItems
ª       ª   ª   ª   ª   ª           cryo.png
ª       ª   ª   ª   ª   ª           healthup.png
ª       ª   ª   ª   ª   ª           incendiary.png
ª       ª   ª   ª   ª   ª           longbow.png
ª       ª   ª   ª   ª   ª           mines.png
ª       ª   ª   ª   ª   ª           necro.png
ª       ª   ª   ª   ª   ª           nuke.png
ª       ª   ª   ª   ª   ª           typhoon.png
ª       ª   ª   ª   ª   ª           
ª       ª   ª   ª   ª   +---Battlefields
ª       ª   ª   ª   ª   ª       CleanupOnAisle13.png
ª       ª   ª   ª   ª   ª       DeadWarehouse.png
ª       ª   ª   ª   ª   ª       Killtop.png
ª       ª   ª   ª   ª   ª       MeanStreets.png
ª       ª   ª   ª   ª   ª       OutbreakMansion.png
ª       ª   ª   ª   ª   ª       ShopTilYouDrop.png
ª       ª   ª   ª   ª   ª       SubZero.png
ª       ª   ª   ª   ª   ª       Touchdown.png
ª       ª   ª   ª   ª   ª       
ª       ª   ª   ª   ª   +---FlashExtract
ª       ª   ª   ª   ª   ª   +---PremiumItems
ª       ª   ª   ª   ª   ª           cryo.png
ª       ª   ª   ª   ª   ª           healthup.png
ª       ª   ª   ª   ª   ª           incendiary.png
ª       ª   ª   ª   ª   ª           longbow.png
ª       ª   ª   ª   ª   ª           mines.png
ª       ª   ª   ª   ª   ª           necro.png
ª       ª   ª   ª   ª   ª           nuke.png
ª       ª   ª   ª   ª   ª           typhoon.png
ª       ª   ª   ª   ª   ª           
ª       ª   ª   ª   ª   +---Sprites
ª       ª   ª   ª   ª       +---PremiumItems
ª       ª   ª   ª   ª       ª       B-52.png
ª       ª   ª   ª   ª       ª       cryo.png
ª       ª   ª   ª   ª       ª       healthup.png
ª       ª   ª   ª   ª       ª       incendiary.png
ª       ª   ª   ª   ª       ª       longbow.png
ª       ª   ª   ª   ª       ª       mines.png
ª       ª   ª   ª   ª       ª       necro.png
ª       ª   ª   ª   ª       ª       nuke.png
ª       ª   ª   ª   ª       ª       typhoon.png
ª       ª   ª   ª   ª       ª       
ª       ª   ª   ª   ª       +---Soldiers
ª       ª   ª   ª   ª               grenadier.png
ª       ª   ª   ª   ª               medic.png
ª       ª   ª   ª   ª               rifleman.png
ª       ª   ª   ª   ª               sawgunner.png
ª       ª   ª   ª   ª               sniper.png
ª       ª   ª   ª   ª               
ª       ª   ª   ª   +---cs
ª       ª   ª   ª   ª       Microsoft.TestPlatform.CommunicationUtilities.resources.dll
ª       ª   ª   ª   ª       Microsoft.TestPlatform.CoreUtilities.resources.dll
ª       ª   ª   ª   ª       Microsoft.TestPlatform.CrossPlatEngine.resources.dll
ª       ª   ª   ª   ª       Microsoft.VisualStudio.TestPlatform.Common.resources.dll
ª       ª   ª   ª   ª       Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll
ª       ª   ª   ª   ª       
ª       ª   ª   ª   +---de
ª       ª   ª   ª   ª       Microsoft.TestPlatform.CommunicationUtilities.resources.dll
ª       ª   ª   ª   ª       Microsoft.TestPlatform.CoreUtilities.resources.dll
ª       ª   ª   ª   ª       Microsoft.TestPlatform.CrossPlatEngine.resources.dll
ª       ª   ª   ª   ª       Microsoft.VisualStudio.TestPlatform.Common.resources.dll
ª       ª   ª   ª   ª       Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll
ª       ª   ª   ª   ª       
ª       ª   ª   ª   +---es
ª       ª   ª   ª   ª       Microsoft.TestPlatform.CommunicationUtilities.resources.dll
ª       ª   ª   ª   ª       Microsoft.TestPlatform.CoreUtilities.resources.dll
ª       ª   ª   ª   ª       Microsoft.TestPlatform.CrossPlatEngine.resources.dll
ª       ª   ª   ª   ª       Microsoft.VisualStudio.TestPlatform.Common.resources.dll
ª       ª   ª   ª   ª       Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll
ª       ª   ª   ª   ª       
ª       ª   ª   ª   +---fr
ª       ª   ª   ª   ª       Microsoft.TestPlatform.CommunicationUtilities.resources.dll
ª       ª   ª   ª   ª       Microsoft.TestPlatform.CoreUtilities.resources.dll
ª       ª   ª   ª   ª       Microsoft.TestPlatform.CrossPlatEngine.resources.dll
ª       ª   ª   ª   ª       Microsoft.VisualStudio.TestPlatform.Common.resources.dll
ª       ª   ª   ª   ª       Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll
ª       ª   ª   ª   ª       
ª       ª   ª   ª   +---it
ª       ª   ª   ª   ª       Microsoft.TestPlatform.CommunicationUtilities.resources.dll
ª       ª   ª   ª   ª       Microsoft.TestPlatform.CoreUtilities.resources.dll
ª       ª   ª   ª   ª       Microsoft.TestPlatform.CrossPlatEngine.resources.dll
ª       ª   ª   ª   ª       Microsoft.VisualStudio.TestPlatform.Common.resources.dll
ª       ª   ª   ª   ª       Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll
ª       ª   ª   ª   ª       
ª       ª   ª   ª   +---ja
ª       ª   ª   ª   ª       Microsoft.TestPlatform.CommunicationUtilities.resources.dll
ª       ª   ª   ª   ª       Microsoft.TestPlatform.CoreUtilities.resources.dll
ª       ª   ª   ª   ª       Microsoft.TestPlatform.CrossPlatEngine.resources.dll
ª       ª   ª   ª   ª       Microsoft.VisualStudio.TestPlatform.Common.resources.dll
ª       ª   ª   ª   ª       Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll
ª       ª   ª   ª   ª       
ª       ª   ª   ª   +---ko
ª       ª   ª   ª   ª       Microsoft.TestPlatform.CommunicationUtilities.resources.dll
ª       ª   ª   ª   ª       Microsoft.TestPlatform.CoreUtilities.resources.dll
ª       ª   ª   ª   ª       Microsoft.TestPlatform.CrossPlatEngine.resources.dll
ª       ª   ª   ª   ª       Microsoft.VisualStudio.TestPlatform.Common.resources.dll
ª       ª   ª   ª   ª       Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll
ª       ª   ª   ª   ª       
ª       ª   ª   ª   +---pl
ª       ª   ª   ª   ª       Microsoft.TestPlatform.CommunicationUtilities.resources.dll
ª       ª   ª   ª   ª       Microsoft.TestPlatform.CoreUtilities.resources.dll
ª       ª   ª   ª   ª       Microsoft.TestPlatform.CrossPlatEngine.resources.dll
ª       ª   ª   ª   ª       Microsoft.VisualStudio.TestPlatform.Common.resources.dll
ª       ª   ª   ª   ª       Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll
ª       ª   ª   ª   ª       
ª       ª   ª   ª   +---pt-BR
ª       ª   ª   ª   ª       Microsoft.TestPlatform.CommunicationUtilities.resources.dll
ª       ª   ª   ª   ª       Microsoft.TestPlatform.CoreUtilities.resources.dll
ª       ª   ª   ª   ª       Microsoft.TestPlatform.CrossPlatEngine.resources.dll
ª       ª   ª   ª   ª       Microsoft.VisualStudio.TestPlatform.Common.resources.dll
ª       ª   ª   ª   ª       Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll
ª       ª   ª   ª   ª       
ª       ª   ª   ª   +---ru
ª       ª   ª   ª   ª       Microsoft.TestPlatform.CommunicationUtilities.resources.dll
ª       ª   ª   ª   ª       Microsoft.TestPlatform.CoreUtilities.resources.dll
ª       ª   ª   ª   ª       Microsoft.TestPlatform.CrossPlatEngine.resources.dll
ª       ª   ª   ª   ª       Microsoft.VisualStudio.TestPlatform.Common.resources.dll
ª       ª   ª   ª   ª       Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll
ª       ª   ª   ª   ª       
ª       ª   ª   ª   +---tr
ª       ª   ª   ª   ª       Microsoft.TestPlatform.CommunicationUtilities.resources.dll
ª       ª   ª   ª   ª       Microsoft.TestPlatform.CoreUtilities.resources.dll
ª       ª   ª   ª   ª       Microsoft.TestPlatform.CrossPlatEngine.resources.dll
ª       ª   ª   ª   ª       Microsoft.VisualStudio.TestPlatform.Common.resources.dll
ª       ª   ª   ª   ª       Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll
ª       ª   ª   ª   ª       
ª       ª   ª   ª   +---zh-Hans
ª       ª   ª   ª   ª       Microsoft.TestPlatform.CommunicationUtilities.resources.dll
ª       ª   ª   ª   ª       Microsoft.TestPlatform.CoreUtilities.resources.dll
ª       ª   ª   ª   ª       Microsoft.TestPlatform.CrossPlatEngine.resources.dll
ª       ª   ª   ª   ª       Microsoft.VisualStudio.TestPlatform.Common.resources.dll
ª       ª   ª   ª   ª       Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll
ª       ª   ª   ª   ª       
ª       ª   ª   ª   +---zh-Hant
ª       ª   ª   ª           Microsoft.TestPlatform.CommunicationUtilities.resources.dll
ª       ª   ª   ª           Microsoft.TestPlatform.CoreUtilities.resources.dll
ª       ª   ª   ª           Microsoft.TestPlatform.CrossPlatEngine.resources.dll
ª       ª   ª   ª           Microsoft.VisualStudio.TestPlatform.Common.resources.dll
ª       ª   ª   ª           Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll
ª       ª   ª   ª           
ª       ª   ª   +---net8.0
ª       ª   +---Release
ª       ª       +---net8.0
ª       +---obj
ª           ª   project.assets.json
ª           ª   project.nuget.cache
ª           ª   SASZombieAssaultTD.Engine.Systems.Enemies.Tests.csproj.nuget.dgspec.json
ª           ª   SASZombieAssaultTD.Engine.Systems.Enemies.Tests.csproj.nuget.g.props
ª           ª   SASZombieAssaultTD.Engine.Systems.Enemies.Tests.csproj.nuget.g.targets
ª           ª   
ª           +---Debug
ª           ª   +---net10.0-windows
ª           ª   ª   ª   .NETCoreApp,Version=v10.0.AssemblyAttributes.cs
ª           ª   ª   ª   SASZombi.CE30576C.Up2Date
ª           ª   ª   ª   SASZombieAssaultTD.Engine.Systems.Enemies.Tests.AssemblyInfo.cs
ª           ª   ª   ª   SASZombieAssaultTD.Engine.Systems.Enemies.Tests.AssemblyInfoInputs.cache
ª           ª   ª   ª   SASZombieAssaultTD.Engine.Systems.Enemies.Tests.assets.cache
ª           ª   ª   ª   SASZombieAssaultTD.Engine.Systems.Enemies.Tests.csproj.AssemblyReference.cache
ª           ª   ª   ª   SASZombieAssaultTD.Engine.Systems.Enemies.Tests.csproj.CoreCompileInputs.cache
ª           ª   ª   ª   SASZombieAssaultTD.Engine.Systems.Enemies.Tests.csproj.FileListAbsolute.txt
ª           ª   ª   ª   SASZombieAssaultTD.Engine.Systems.Enemies.Tests.dll
ª           ª   ª   ª   SASZombieAssaultTD.Engine.Systems.Enemies.Tests.GeneratedMSBuildEditorConfig.editorconfig
ª           ª   ª   ª   SASZombieAssaultTD.Engine.Systems.Enemies.Tests.genruntimeconfig.cache
ª           ª   ª   ª   SASZombieAssaultTD.Engine.Systems.Enemies.Tests.GlobalUsings.g.cs
ª           ª   ª   ª   SASZombieAssaultTD.Engine.Systems.Enemies.Tests.pdb
ª           ª   ª   ª   SASZombieAssaultTD.Engine.Systems.Enemies.Tests.sourcelink.json
ª           ª   ª   ª   
ª           ª   ª   +---ref
ª           ª   ª   ª       SASZombieAssaultTD.Engine.Systems.Enemies.Tests.dll
ª           ª   ª   ª       
ª           ª   ª   +---refint
ª           ª   ª           SASZombieAssaultTD.Engine.Systems.Enemies.Tests.dll
ª           ª   ª           
ª           ª   +---net8.0
ª           ª       ª   .NETCoreApp,Version=v8.0.AssemblyAttributes.cs
ª           ª       ª   SASZombieAssaultTD.Engine.Systems.Enemies.Tests.AssemblyInfo.cs
ª           ª       ª   SASZombieAssaultTD.Engine.Systems.Enemies.Tests.AssemblyInfoInputs.cache
ª           ª       ª   SASZombieAssaultTD.Engine.Systems.Enemies.Tests.GeneratedMSBuildEditorConfig.editorconfig
ª           ª       ª   SASZombieAssaultTD.Engine.Systems.Enemies.Tests.GlobalUsings.g.cs
ª           ª       ª   
ª           ª       +---ref
ª           ª       +---refint
ª           +---Release
ª               +---net8.0
ª                   ª   .NETCoreApp,Version=v8.0.AssemblyAttributes.cs
ª                   ª   SASZombieAssaultTD.Engine.Systems.Enemies.Tests.AssemblyInfo.cs
ª                   ª   SASZombieAssaultTD.Engine.Systems.Enemies.Tests.AssemblyInfoInputs.cache
ª                   ª   SASZombieAssaultTD.Engine.Systems.Enemies.Tests.GeneratedMSBuildEditorConfig.editorconfig
ª                   ª   SASZombieAssaultTD.Engine.Systems.Enemies.Tests.GlobalUsings.g.cs
ª                   ª   
ª                   +---ref
ª                   +---refint
+---EngineMath
ª       EngineMath.cs
ª       
+---GameLoop
ª       Diagnostics.cs
ª       ErrorHandling.cs
ª       GameLoopMain.cs
ª       Helpers.cs
ª       Input.cs
ª       State.cs
ª       Timing.cs
ª       
+---GameRoot
ª       Debug.cs
ª       EventRouting.cs
ª       GameRootMain.cs
ª       Initialization.cs
ª       SceneFlow.cs
ª       State.cs
ª       SystemRegistration.cs
ª       UpdateLoop.cs
ª       
+---HazardsControl
ª       Hazard.cs
ª       HazardAnalytics.cs
ª       HazardCleanup.cs
ª       HazardDensity.cs
ª       HazardKillAttribution.cs
ª       HazardLaneInteraction.cs
ª       HazardLifecycle.cs
ª       HazardOccupancy.cs
ª       HazardRegistration.cs
ª       HazardsMain.cs
ª       HazardTypes.cs
ª       HazardVisuals.cs
ª       
+---Interfaces
ª       IDebugRenderer.cs
ª       IGameStateMachine.cs
ª       IManager.cs
ª       ISystemRegistry.cs
ª       SystemInterfaces.cs
ª       
+---Managers
ª       RenderManager.cs
ª       SystemManager.cs
ª       UpdateManager.cs
ª       
+---Memory
ª       MemoryTracker.cs
ª       ObjectPool.cs
ª       
+---Navigation
ª       AStarPathfinder.cs
ª       FlowField.cs
ª       NavigationDebugRenderer.cs
ª       NavigationGrid.cs
ª       NavigationMigrationHelper.cs
ª       
+---Performance
ª       DebugOverlay.cs
ª       FramePacer.cs
ª       PerformanceProfiler.cs
ª       
+---Persistence
ª       SaveDataTypes.cs
ª       
+---Physics
ª   ª   AABBShape.cs
ª   ª   CapsuleShape.cs
ª   ª   CircleShape.cs
ª   ª   ColliderComponent.cs
ª   ª   CollisionDebugRenderer.cs
ª   ª   CollisionEvent.cs
ª   ª   CollisionShape.cs
ª   ª   CollisionSystem.cs
ª   ª   PhysicsSystem.cs
ª   ª   PhysicsTuning.cs
ª   ª   SpatialPartitionGrid.cs
ª   ª   TriggerSystem.cs
ª   ª   
ª   +---Components
ª           ColliderShapes.cs
ª           PhysicsComponent.cs
ª           
+---Platform
ª       IProgram.cs
ª       Win32Window.cs
ª       
+---Registry
ª       SystemRegistry.cs
ª       
+---Rendering
ª   ª   Color.cs
ª   ª   Framebuffer.cs
ª   ª   IPlatformRenderer.cs
ª   ª   IRenderContext.cs
ª   ª   ParticleSystem.cs
ª   ª   Rectangle.cs
ª   ª   RenderCommandQueue.cs
ª   ª   RenderDiagnostics.cs
ª   ª   Renderer.cs
ª   ª   RenderInitValidator.cs
ª   ª   RenderPipelineConfig.cs
ª   ª   RenderQueue.cs
ª   ª   RenderSurface.cs
ª   ª   Sprite.cs
ª   ª   SpriteBatch.cs
ª   ª   SpriteBatchOptimizer.cs
ª   ª   SpriteBatchRenderer.cs
ª   ª   TextRenderer.cs
ª   ª   Texture2D.cs
ª   ª   TextureCache.cs
ª   ª   WindowHost.cs
ª   ª   
ª   +---Debug
ª   ª       PathDebugRenderer.cs
ª   ª       
ª   +---Systems
ª   ª       RenderingSystem.cs
ª   ª       
ª   +---Zombies
ª           ZombieRenderer.cs
ª           
+---Resources
ª       DataLoader.cs
ª       RSBatchLoadResult.cs
ª       RSBundle.cs
ª       RSDiscovery.cs
ª       RSHandle.cs
ª       RSInitializer.cs
ª       RSKey.cs
ª       RSLoadContext.cs
ª       RSLoadResult.cs
ª       RSManager.cs
ª       RSMetadata.cs
ª       RSPipeline.cs
ª       RSRegistry.cs
ª       RSSource.cs
ª       RSType.cs
ª       RSUtils.cs
ª       RSValidation.cs
ª       TextureLoader.cs
ª       
+---Save
ª       SaveData.cs
ª       SaveSystem.cs
ª       
+---Scenes
ª   ª   BaseScene.cs
ª   ª   GameScene.cs
ª   ª   LoadingScene.cs
ª   ª   MainMenuScene.cs
ª   ª   PauseScene.cs
ª   ª   Scene.cs
ª   ª   SceneManager.cs
ª   ª   SceneTransitionTest.cs
ª   ª   
ª   +---TestScenes
ª           MeanStreetsTest.cs
ª           
+---State
ª       AdvancedStateMachine.cs
ª       BootState.cs
ª       EnhancedStateMachine.cs
ª       GameEvent.cs
ª       GameplayState.cs
ª       GameStateType.cs
ª       IGameState.cs
ª       MainMenuState.cs
ª       PausedState.cs
ª       README.md
ª       StateDebugger.cs
ª       StateFactory.cs
ª       StateMachine.cs
ª       StateMachineExtensions.cs
ª       StateMachineIntegration.cs
ª       StateMachineProfiler.cs
ª       StateMachineTest.cs
ª       StateTransition.cs
ª       
+---Systems
ª   ª   EventRouter.cs
ª   ª   
ª   +---Gameplay
ª           NavigationGrid.cs
ª           PathfindingSystem.cs
ª           PriorityQueue.cs
ª           
+---Timing
ª       TimingController.cs
ª       TimingModule.cs
ª       
+---UI
ª   ª   AchievementListRenderer.cs
ª   ª   AchievementPopupRenderer.cs
ª   ª   Button.cs
ª   ª   ChallengeListRenderer.cs
ª   ª   ChallengeTrackerRenderer.cs
ª   ª   GameOverScreenRenderer.cs
ª   ª   GameOverScreenStatistics.cs
ª   ª   HealthBarRenderer.cs
ª   ª   HUD.cs
ª   ª   InventoryPanelRenderer.cs
ª   ª   ItemTooltipRenderer.cs
ª   ª   KillFeedSystem.cs
ª   ª   LayoutSystem.cs
ª   ª   Menus.cs
ª   ª   MetaProgressionRenderer.cs
ª   ª   Panel.cs
ª   ª   PointF.cs
ª   ª   ResourceDisplayRenderer.cs
ª   ª   RespawnCountdownRenderer.cs
ª   ª   RespawnCountdownStatistics.cs
ª   ª   RoundCompleteNotificationRenderer.cs
ª   ª   RoundCompleteStatistics.cs
ª   ª   ScoreDisplayStatistics.cs
ª   ª   ScoreDisplaySystem.cs
ª   ª   UIElement.cs
ª   ª   UIElementBase.cs
ª   ª   UIRoot.cs
ª   ª   UISystem.cs
ª   ª   
ª   +---Assets
ª   ª       UIAssetLoader.cs
ª   ª       UIFont.cs
ª   ª       UISprite.cs
ª   ª       
ª   +---Components
ª   ª       Label.cs
ª   ª       
ª   +---Debug
ª   ª       UIDebugInspector.cs
ª   ª       UIDebugOverlay.cs
ª   ª       
ª   +---Input
ª   ª       UIFocusManager.cs
ª   ª       UIInputRouter.cs
ª   ª       UIInputState.cs
ª   ª       
ª   +---Layout
ª   ª       UILayoutSystem.cs
ª   ª       UILayoutTypes.cs
ª   ª       
ª   +---Rendering
ª   ª       UIBatcher.cs
ª   ª       UIRenderContext.cs
ª   ª       UIRenderer.cs
ª   ª       
ª   +---Statistics
ª   ª       HealthBarRendererStatistics.cs
ª   ª       KillFeedStatistics.cs
ª   ª       
ª   +---Styles
ª   ª       UIStyle.cs
ª   ª       UIStyleResolver.cs
ª   ª       UIStyleSheet.cs
ª   ª       
ª   +---Systems
ª   ª       UIManager.cs
ª   ª       
ª   +---Widgets
ª           UIButton.cs
ª           UIPanel.cs
ª           UIText.cs
ª           UIWidgetBase.cs
ª           
+---Utility
ª       DebugLogger.cs
ª       ILogger.cs
ª       Logger.cs
ª       MathHelper.cs
ª       Randomizer.cs
ª       Time.cs
ª       
+---VectorMath
ª       Vector3Math.cs
ª       
+---Window
        Window.cs
        

## Source Files

- EngineBootstrap.cs
- EngineMath.cs
- Achievements\AchievementCategory.cs
- Achievements\AchievementDefinition.cs
- Achievements\AchievementInstance.cs
- Achievements\AchievementListRenderer.cs
- Achievements\AchievementPopupRenderer.cs
- Achievements\AchievementRarity.cs
- Achievements\ChallengeDefinition.cs
- Achievements\ChallengeInstance.cs
- Achievements\UI\AchievementListRenderer.cs
- Achievements\UI\AchievementPopupRenderer.cs
- AI\AIController.cs
- AI\Behaviors\BasicChaseBehavior.cs
- AI\Blackboard\Blackboard.cs
- Animation\AnimationClip.cs
- Animation\AnimationConditionOperator.cs
- Animation\AnimationController.cs
- Animation\AnimationStateMachine.cs
- Animation\AnimationTrack.cs
- Animation\AnimationTransition.cs
- Animation\AnimationTransitionDebug.cs
- Animation\AnimationComponents\AnimationComponent.cs
- Animation\AnimationComponents\AnimationControllerComponent.cs
- Animation\AnimationComponents\AnimationFrame.cs
- Animation\AnimationComponents\AnimationPlayer.cs
- Animation\AnimationComponents\AnimationStateInspector.cs
- Animation\AnimationComponents\AnimationStateSnapshot.cs
- Animation\AnimationComponents\AnimationStateVisualization.cs
- Animation\AnimationComponents\AnimationTypes.cs
- Animation\AnimationComponents\AttackState.cs
- Animation\AnimationComponents\IAnimationState.cs
- Animation\AnimationComponents\IdleState.cs
- Animation\AnimationComponents\JumpState.cs
- Animation\AnimationComponents\MoveState.cs
- Animation\AnimationEvents\AnimationEvent.cs
- Animation\AnimationEvents\AnimationEventContext.cs
- Animation\AnimationEvents\AnimationEventDispatcher.cs
- Animation\AnimationEvents\AnimationEventTrack.cs
- Animation\AnimationEvents\IAnimationEventReceiver.cs
- Animation\AnimationSystems\AnimationSystem.cs
- Animation\AnimationSystems\AnimationTriggerSystem.cs
- Animation\BlendTree\BlendParameters.cs
- Animation\BlendTree\BlendTree.cs
- Animation\BlendTree\BlendTreeNode.cs
- Animation\BlendTree\BlendTreeSerializer.cs
- Animation\BlendTree\BlendTreeValidationResult.cs
- Animation\BlendTree\BlendTreeValidator.cs
- Animation\BlendTree\IBlendNode.cs
- Animation\BlendTree\LinearBlendNode.cs
- Animation\BlendTree\SingleClipNode.cs
- Animation\BlendTree\TwoDBlendNode.cs
- Animation\Integration\AnimationECSIntegration.cs
- Animation\Integration\AnimationEventECSIntegration.cs
- Animation\Integration\AnimationEventTypes.cs
- Audio\CoreAudioEngine.cs
- Audio\CoreSoundEffect.cs
- Camera\CameraSystem.cs
- Challenges\ChallengeCategory.cs
- Challenges\ChallengeDifficulty.cs
- Challenges\ChallengeListRenderer.cs
- Challenges\ChallengeTrackerRenderer.cs
- Challenges\ChallengeType.cs
- Combat\KillFeedSystem.cs
- Combat\Statistics\KillFeedStatistics.cs
- Components\ActiveComponent.cs
- Components\CollisionComponent.cs
- Components\ComponentTypes.cs
- Components\DamageComponent.cs
- Components\EnemyTypeComponent.cs
- Components\HealthComponent.cs
- Components\InventoryComponent.cs
- Components\LifetimeComponent.cs
- Components\MovementComponent.cs
- Components\ParticleEmitterComponent.cs
- Components\RenderableComponent.cs
- Components\ScoreComponent.cs
- Components\SpriteComponent.cs
- Components\StatsComponent.cs
- Components\TransformComponent.cs
- Components\TriggerComponent.cs
- Components\UIComponent.cs
- Core\CoreColor.cs
- Core\CoreRectangle.cs
- Core\CoreTimeStep.cs
- Core\CoreTransform.cs
- Diagnostics\DebugLogger.cs
- Diagnostics\DebugOverlay.cs
- Diagnostics\FrameDiagnostics.cs
- Diagnostics\FrameStats.cs
- Diagnostics\HeartbeatMonitor.cs
- Diagnostics\ManagerDiagnostics.cs
- Diagnostics\Timing.cs
- ECS\BaseComponent.cs
- ECS\CollisionTypes.cs
- ECS\ECSComponent.cs
- ECS\ECSDebugInspector.cs
- ECS\ECSEntity.cs
- ECS\ECSManager.cs
- ECS\ECSSystem.cs
- ECS\ECSVerificationReport.cs
- ECS\ECSWorld.cs
- ECS\EntityFactory.cs
- ECS\EntityManager.cs
- ECS\IComponent.cs
- ECS\IEntityComponent.cs
- ECS\IGameSystem.cs
- ECS\ISystem.cs
- ECS\Components\Components.cs
- Enemies\EnemyDefinition.cs
- Enemies\EnemyDefinitionRegistry.cs
- Enemies\EnemyDefinitionValidator.cs
- Enemies\EnemySystem.cs
- Enemies\ZombieAI.cs
- Enemies\ZombieMovement.cs
- Enemies\ZombieSpawner.cs
- Enemies\Tests\EnemyDefinitionRegistryTests.cs
- Enemies\Tests\EnemyDefinitionValidatorTests.cs
- Enemies\Tests\obj\Debug\net10.0-windows\.NETCoreApp,Version=v10.0.AssemblyAttributes.cs
- Enemies\Tests\obj\Debug\net10.0-windows\SASZombieAssaultTD.Engine.Systems.Enemies.Tests.AssemblyInfo.cs
- Enemies\Tests\obj\Debug\net10.0-windows\SASZombieAssaultTD.Engine.Systems.Enemies.Tests.GlobalUsings.g.cs
- Enemies\Tests\obj\Debug\net8.0\.NETCoreApp,Version=v8.0.AssemblyAttributes.cs
- Enemies\Tests\obj\Debug\net8.0\SASZombieAssaultTD.Engine.Systems.Enemies.Tests.AssemblyInfo.cs
- Enemies\Tests\obj\Debug\net8.0\SASZombieAssaultTD.Engine.Systems.Enemies.Tests.GlobalUsings.g.cs
- Enemies\Tests\obj\Release\net8.0\.NETCoreApp,Version=v8.0.AssemblyAttributes.cs
- Enemies\Tests\obj\Release\net8.0\SASZombieAssaultTD.Engine.Systems.Enemies.Tests.AssemblyInfo.cs
- Enemies\Tests\obj\Release\net8.0\SASZombieAssaultTD.Engine.Systems.Enemies.Tests.GlobalUsings.g.cs
- EngineMath\EngineMath.cs
- GameLoop\Diagnostics.cs
- GameLoop\ErrorHandling.cs
- GameLoop\GameLoopMain.cs
- GameLoop\Helpers.cs
- GameLoop\Input.cs
- GameLoop\State.cs
- GameLoop\Timing.cs
- GameRoot\Debug.cs
- GameRoot\EventRouting.cs
- GameRoot\GameRootMain.cs
- GameRoot\Initialization.cs
- GameRoot\SceneFlow.cs
- GameRoot\State.cs
- GameRoot\SystemRegistration.cs
- GameRoot\UpdateLoop.cs
- HazardsControl\Hazard.cs
- HazardsControl\HazardAnalytics.cs
- HazardsControl\HazardCleanup.cs
- HazardsControl\HazardDensity.cs
- HazardsControl\HazardKillAttribution.cs
- HazardsControl\HazardLaneInteraction.cs
- HazardsControl\HazardLifecycle.cs
- HazardsControl\HazardOccupancy.cs
- HazardsControl\HazardRegistration.cs
- HazardsControl\HazardsMain.cs
- HazardsControl\HazardTypes.cs
- HazardsControl\HazardVisuals.cs
- Interfaces\IDebugRenderer.cs
- Interfaces\IGameStateMachine.cs
- Interfaces\IManager.cs
- Interfaces\ISystemRegistry.cs
- Interfaces\SystemInterfaces.cs
- Managers\RenderManager.cs
- Managers\SystemManager.cs
- Managers\UpdateManager.cs
- Memory\MemoryTracker.cs
- Memory\ObjectPool.cs
- Navigation\AStarPathfinder.cs
- Navigation\FlowField.cs
- Navigation\NavigationDebugRenderer.cs
- Navigation\NavigationGrid.cs
- Navigation\NavigationMigrationHelper.cs
- Performance\DebugOverlay.cs
- Performance\FramePacer.cs
- Performance\PerformanceProfiler.cs
- Persistence\SaveDataTypes.cs
- Physics\AABBShape.cs
- Physics\CapsuleShape.cs
- Physics\CircleShape.cs
- Physics\ColliderComponent.cs
- Physics\CollisionDebugRenderer.cs
- Physics\CollisionEvent.cs
- Physics\CollisionShape.cs
- Physics\CollisionSystem.cs
- Physics\PhysicsSystem.cs
- Physics\PhysicsTuning.cs
- Physics\SpatialPartitionGrid.cs
- Physics\TriggerSystem.cs
- Physics\Components\ColliderShapes.cs
- Physics\Components\PhysicsComponent.cs
- Platform\IProgram.cs
- Platform\Win32Window.cs
- Registry\SystemRegistry.cs
- Rendering\Color.cs
- Rendering\Framebuffer.cs
- Rendering\IPlatformRenderer.cs
- Rendering\IRenderContext.cs
- Rendering\ParticleSystem.cs
- Rendering\Rectangle.cs
- Rendering\RenderCommandQueue.cs
- Rendering\RenderDiagnostics.cs
- Rendering\Renderer.cs
- Rendering\RenderInitValidator.cs
- Rendering\RenderPipelineConfig.cs
- Rendering\RenderQueue.cs
- Rendering\RenderSurface.cs
- Rendering\Sprite.cs
- Rendering\SpriteBatch.cs
- Rendering\SpriteBatchOptimizer.cs
- Rendering\SpriteBatchRenderer.cs
- Rendering\TextRenderer.cs
- Rendering\Texture2D.cs
- Rendering\TextureCache.cs
- Rendering\WindowHost.cs
- Rendering\Debug\PathDebugRenderer.cs
- Rendering\Systems\RenderingSystem.cs
- Rendering\Zombies\ZombieRenderer.cs
- Resources\DataLoader.cs
- Resources\RSBatchLoadResult.cs
- Resources\RSBundle.cs
- Resources\RSDiscovery.cs
- Resources\RSHandle.cs
- Resources\RSInitializer.cs
- Resources\RSKey.cs
- Resources\RSLoadContext.cs
- Resources\RSLoadResult.cs
- Resources\RSManager.cs
- Resources\RSMetadata.cs
- Resources\RSPipeline.cs
- Resources\RSRegistry.cs
- Resources\RSSource.cs
- Resources\RSType.cs
- Resources\RSUtils.cs
- Resources\RSValidation.cs
- Resources\TextureLoader.cs
- Save\SaveData.cs
- Save\SaveSystem.cs
- Scenes\BaseScene.cs
- Scenes\GameScene.cs
- Scenes\LoadingScene.cs
- Scenes\MainMenuScene.cs
- Scenes\PauseScene.cs
- Scenes\Scene.cs
- Scenes\SceneManager.cs
- Scenes\SceneTransitionTest.cs
- Scenes\TestScenes\MeanStreetsTest.cs
- State\AdvancedStateMachine.cs
- State\BootState.cs
- State\EnhancedStateMachine.cs
- State\GameEvent.cs
- State\GameplayState.cs
- State\GameStateType.cs
- State\IGameState.cs
- State\MainMenuState.cs
- State\PausedState.cs
- State\StateDebugger.cs
- State\StateFactory.cs
- State\StateMachine.cs
- State\StateMachineExtensions.cs
- State\StateMachineIntegration.cs
- State\StateMachineProfiler.cs
- State\StateMachineTest.cs
- State\StateTransition.cs
- Systems\EventRouter.cs
- Systems\Gameplay\NavigationGrid.cs
- Systems\Gameplay\PathfindingSystem.cs
- Systems\Gameplay\PriorityQueue.cs
- Timing\TimingController.cs
- Timing\TimingModule.cs
- UI\AchievementListRenderer.cs
- UI\AchievementPopupRenderer.cs
- UI\Button.cs
- UI\ChallengeListRenderer.cs
- UI\ChallengeTrackerRenderer.cs
- UI\GameOverScreenRenderer.cs
- UI\GameOverScreenStatistics.cs
- UI\HealthBarRenderer.cs
- UI\HUD.cs
- UI\InventoryPanelRenderer.cs
- UI\ItemTooltipRenderer.cs
- UI\KillFeedSystem.cs
- UI\LayoutSystem.cs
- UI\Menus.cs
- UI\MetaProgressionRenderer.cs
- UI\Panel.cs
- UI\PointF.cs
- UI\ResourceDisplayRenderer.cs
- UI\RespawnCountdownRenderer.cs
- UI\RespawnCountdownStatistics.cs
- UI\RoundCompleteNotificationRenderer.cs
- UI\RoundCompleteStatistics.cs
- UI\ScoreDisplayStatistics.cs
- UI\ScoreDisplaySystem.cs
- UI\UIElement.cs
- UI\UIElementBase.cs
- UI\UIRoot.cs
- UI\UISystem.cs
- UI\Assets\UIAssetLoader.cs
- UI\Assets\UIFont.cs
- UI\Assets\UISprite.cs
- UI\Components\Label.cs
- UI\Debug\UIDebugInspector.cs
- UI\Debug\UIDebugOverlay.cs
- UI\Input\UIFocusManager.cs
- UI\Input\UIInputRouter.cs
- UI\Input\UIInputState.cs
- UI\Layout\UILayoutSystem.cs
- UI\Layout\UILayoutTypes.cs
- UI\Rendering\UIBatcher.cs
- UI\Rendering\UIRenderContext.cs
- UI\Rendering\UIRenderer.cs
- UI\Statistics\HealthBarRendererStatistics.cs
- UI\Statistics\KillFeedStatistics.cs
- UI\Styles\UIStyle.cs
- UI\Styles\UIStyleResolver.cs
- UI\Styles\UIStyleSheet.cs
- UI\Systems\UIManager.cs
- UI\Widgets\UIButton.cs
- UI\Widgets\UIPanel.cs
- UI\Widgets\UIText.cs
- UI\Widgets\UIWidgetBase.cs
- Utility\DebugLogger.cs
- Utility\ILogger.cs
- Utility\Logger.cs
- Utility\MathHelper.cs
- Utility\Randomizer.cs
- Utility\Time.cs
- VectorMath\Vector3Math.cs
- Window\Window.cs

## Summary

Total C# files: 327
