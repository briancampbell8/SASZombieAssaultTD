# SASZombieAssaultTD Project - C# File Structure

## Project Overview
- **Total C# Files**: 436 files
- **Project Type**: .NET 8.0 Windows Game Engine
- **Target Framework**: net8.0-windows
- **Main Assembly**: SASZombieAssaultTD.Engine

## Root Level
```
SASZombieAssaultTD/
├── SASZombieAssaultTD.csproj
├── App.config
├── [Various documentation files]
└── Engine/ (436 C# files)
```

## Engine Core Structure
```
Engine/
├── Core/ (7 files)
│   ├── Color.cs
│   ├── CoreMath.cs
│   ├── IInitializable.cs
│   ├── IUpdatable.cs
│   ├── Rectangle.cs
│   ├── TimeStep.cs
│   └── Transform.cs
├── ECS/ (11 files)
│   ├── BaseComponent.cs
│   ├── CollisionTypes.cs
│   ├── ECSDebugInspector.cs
│   ├── ECSVerificationReport.cs
│   ├── ECSWorld.cs
│   ├── Entity.cs
│   ├── EntityFactory.cs
│   ├── EntityManager.cs
│   ├── IComponent.cs
│   ├── IEntityComponent.cs
│   └── IGameSystem.cs
├── Events/ (29 files)
│   ├── EventBus.cs
│   ├── EventManager.cs
│   ├── IEventBus.cs
│   ├── AchievementUnlockedEvent.cs
│   ├── ChallengeCompletedEvent.cs
│   ├── EntityDiedEvent.cs
│   ├── KillAttributedEvent.cs
│   ├── LevelUpEvent.cs
│   └── [21 more event files...]
├── Interfaces/ (5 files)
│   ├── IAnimationState.cs
│   ├── IDebugRenderer.cs
│   ├── IGameStateMachine.cs
│   ├── IManager.cs
│   └── SystemInterfaces.cs
└── Utility/ (6 files)
    ├── DebugLogger.cs
    ├── ILogger.cs
    ├── Logger.cs
    ├── MathHelper.cs
    ├── Randomizer.cs
    └── Time.cs
```

## Engine Systems
```
Engine/
├── Animation/ (14 files + subfolders)
│   ├── AnimationClip.cs
│   ├── AnimationController.cs
│   ├── AnimationStateMachine.cs
│   ├── Components/ (2 files)
│   ├── Events/ (9 files)
│   ├── Systems/ (2 files)
│   └── [other subfolders...]
├── Physics/ (12 files)
│   ├── CollisionSystem.cs
│   ├── PhysicsSystem.cs
│   ├── TriggerSystem.cs
│   ├── Components/ (2 files)
│   └── [7 more files...]
├── Rendering/ (21 files)
│   ├── ParticleSystem.cs
│   ├── Renderer.cs
│   ├── TextRenderer.cs
│   ├── Systems/ (1 file)
│   ├── Zombies/ (1 file)
│   └── [18 more files...]
├── UI/ (28 files + subfolders)
│   ├── HUD.cs
│   ├── UISystem.cs
│   ├── Components/ (1 file)
│   ├── Debug/ (2 files)
│   ├── Input/ (3 files)
│   ├── Rendering/ (3 files)
│   └── [18 more files...]
├── Audio/ (5 files)
├── Navigation/ (5 files)
├── Input/ (3 files)
└── Memory/ (2 files)
```

## Gameplay Systems
```
Engine/
├── Gameplay/ (29 files)
│   ├── AchievementSystem.cs
│   ├── ChallengeSystem.cs
│   ├── DamageSystem.cs
│   ├── DeathEffectSystem.cs
│   ├── EnemySystem.cs
│   ├── PlayerSystem.cs
│   ├── ProjectileSystem.cs
│   ├── ScoreSystem.cs
│   ├── TowerSystem.cs
│   ├── WaveController.cs
│   ├── Events/ (2 files)
│   ├── Interaction/ (3 files)
│   ├── Inventory/ (1 file)
│   ├── Weapons/ (1 file)
│   └── Zombies/ (3 files)
├── Enemies/ (7 files)
│   ├── EnemySystem.cs
│   ├── ZombieAI.cs
│   ├── ZombieSpawner.cs
│   └── [4 more files...]
├── HazardsControl/ (11 files)
│   ├── Hazard.cs
│   ├── HazardAnalytics.cs
│   ├── HazardsMain.cs
│   └── [8 more files...]
└── Waves/ (0 files - empty)
```

## Resource Management
```
Engine/
├── Resources/ (18 files)
│   ├── RSManager.cs
│   ├── RSType.cs
│   ├── RSMetadata.cs
│   ├── RSKey.cs
│   ├── RSDiscovery.cs
│   ├── RSPipeline.cs
│   ├── RSRegistry.cs
│   ├── RSValidation.cs
│   └── [9 more files...]
├── Achievements/ (8 files)
│   ├── AchievementDefinition.cs
│   ├── AchievementRarity.cs
│   ├── AchievementCategory.cs
│   ├── UI/ (2 files)
│   └── [4 more files...]
└── Challenges/ (5 files)
    ├── ChallengeDifficulty.cs
    ├── ChallengeType.cs
    ├── ChallengeCategory.cs
    └── [2 more files...]
```

## Scene & State Management
```
Engine/
├── Scenes/ (8 files)
│   ├── BaseScene.cs
│   ├── GameScene.cs
│   ├── SceneManager.cs
│   ├── LoadingScene.cs
│   ├── MainMenuScene.cs
│   ├── PauseScene.cs
│   ├── Scene.cs
│   └── TestScenes/ (1 file)
├── State/ (17 files)
│   ├── StateMachine.cs
│   ├── EnhancedStateMachine.cs
│   ├── StateDebugger.cs
│   ├── GameStateType.cs
│   ├── IGameState.cs
│   └── [11 more files...]
└── GameRoot/ (8 files)
    ├── GameRootMain.cs
    ├── Initialization.cs
    ├── SystemRegistration.cs
    └── [5 more files...]
```

## Components
```
Engine/
├── Components/ (9 files)
│   ├── CollisionComponent.cs
│   ├── ParticleEmitterComponent.cs
│   ├── SpriteComponent.cs
│   ├── StatsComponent.cs
│   ├── TransformComponent.cs
│   ├── TriggerComponent.cs
│   ├── UIComponent.cs
│   └── [2 more files...]
└── ECS/Components/ (9 files)
    ├── AnimationComponent.cs
    ├── HealthComponent.cs
    ├── MovementComponent.cs
    └── [6 more files...]
```

## Support Systems
```
Engine/
├── Diagnostics/ (7 files)
│   ├── DebugLogger.cs
│   ├── FrameStats.cs
│   ├── HeartbeatMonitor.cs
│   └── [4 more files...]
├── Timing/ (2 files)
├── Save/ (2 files)
├── Tools/ (7 files)
├── VectorMath/ (2 files)
├── Window/ (1 file)
└── Platform/ (2 files)
```

## Summary
- **Total C# Files**: 436 files
- **Main Categories**: 25+ major folders
- **Key Systems**: ECS, Events, Animation, Physics, Rendering, UI, Gameplay
- **Resource System**: RSManager with 18 supporting files
- **Component Architecture**: Split between Components/ and ECS/Components/
- **Scene Management**: 8 scene files with state machine support
- **Build Status**: 948 compilation errors remaining (as of latest analysis)

**Note**: Some folders like `Waves/` appear to be empty or excluded from the current build configuration.
