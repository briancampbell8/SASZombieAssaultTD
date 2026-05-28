# SAS Zombie Assault TD - Execution Tree Map

## Visual Overview of Project Execution Flow

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                           ENTRY POINT                                        │
│                    ┌─────────────────────┐                                   │
│                    │   Program.Main()    │                                   │
│                    │   [STAThread]       │                                   │
│                    └──────────┬──────────┘                                   │
└───────────────────────────────┼─────────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                      ENGINE INITIALIZATION                                   │
│              ┌────────────────────────────────┐                            │
│              │    EngineCore.InitializeAsync() │                            │
│              └───────────────┬────────────────┘                            │
│                              │                                              │
│              ┌───────────────┴───────────────┐                             │
│              │ InitializeCoreSystemsAsync()   │                             │
│              │ • Asset System                │                             │
│              │ • Logging System              │                             │
│              │ • Config Loading              │                             │
│              └───────────────┬───────────────┘                              │
└──────────────────────────────┼──────────────────────────────────────────────┘
                               │
                               ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                       GAME ROOT INITIALIZATION                               │
│         ┌────────────────────────────────────────────────┐                   │
│         │           GameRoot.Initialize()               │                   │
│         │     ┌────────────────────────────────────┐   │                   │
│         │     │    PerformInitialization()          │   │                   │
│         │     │  (via Initialization.cs partial)   │   │                   │
│         │     └───────────────┬──────────────────────┘   │                   │
│         └─────────────────────┼──────────────────────────┘                   │
│                               │                                              │
│     ┌───────────────────────────┼───────────────────────────┐                 │
│     │          PHASE 1          │          PHASE 2          │                 │
│     │   SystemManager Init      │   UpdateManager Init      │                 │
│     │   • System Registry       │   • Update Systems        │                 │
│     │   • Service Locator       │   • IUpdateSystem list    │                 │
│     │                           │                           │                 │
│     │          PHASE 3          │          PHASE 4          │                 │
│     │   RenderManager Init      │   GameStateMachine Init   │                 │
│     │   • IRenderContext        │   • State transitions     │                 │
│     │   • Render Systems        │   • Initial state         │                 │
│     │                           │                           │                 │
│     │          PHASE 5          │                           │                 │
│     │   RenderContext Init      │                           │                 │
│     │   • Graphics API setup    │                           │                 │
│     └───────────────────────────┴───────────────────────────┘                 │
└─────────────────────────────────────────────────────────────────────────────┘
                               │
                               ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                          MAIN GAME LOOP                                      │
│              ┌────────────────────────────────────┐                        │
│              │       GameRoot.RunAsync()          │                        │
│              │   ┌────────────────────────────┐   │                        │
│              │   │    GameLoop Execution      │   │                        │
│              │   └───────────┬────────────────┘   │                        │
│              └───────────────┼────────────────────┘                        │
└──────────────────────────────┼──────────────────────────────────────────────┘
                               │
              ┌────────────────┼────────────────┐
              │                │                │
              ▼                ▼                ▼
┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐
│  UPDATE PHASE   │  │  RENDER PHASE   │  │   INPUT PHASE   │
│                 │  │                 │  │                 │
│ UpdateManager   │  │ RenderManager   │  │ UIInputRouter   │
│ .UpdateAll()    │  │ .RenderAll()    │  │ .ProcessInput() │
│                 │  │                 │  │                 │
│ ├─ AI Systems   │  │ ├─ 2D Sprites   │  │ ├─ Keyboard     │
│ ├─ Animation    │  │ ├─ UI Elements  │  │ ├─ Mouse        │
│ ├─ Physics      │  │ ├─ Effects      │  │ ├─ Gamepad      │
│ ├─ Game Logic   │  │ └─ Debug Viz    │  │ └─ Touch        │
│ └─ Achievements │  │                 │  │                 │
└─────────────────┘  └─────────────────┘  └─────────────────┘
                               │
                               ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                        SYSTEM ARCHITECTURE                                   │
│                                                                              │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐        │
│  │   ECSWorld  │  │ SystemReg.  │  │   Systems   │  │    UI       │        │
│  │  (Entities) │  │ (Services)  │  │  (Managers) │  │  (Input/    │        │
│  │             │  │             │  │             │  │  Rendering) │        │
│  └──────┬──────┘  └──────┬──────┘  └──────┬──────┘  └──────┬──────┘        │
│         │                │                │                │               │
│         └────────────────┴────────────────┴────────────────┘               │
│                          │                                                  │
│                          ▼                                                  │
│              ┌─────────────────────┐                                       │
│              │     GameRoot        │                                       │
│              │  (Central Hub)      │                                       │
│              └─────────────────────┘                                       │
│                          │                                                  │
│              ┌───────────┴───────────┐                                     │
│              ▼                       ▼                                      │
│  ┌──────────────────┐  ┌──────────────────┐                                │
│  │  State Machine   │  │  Game Loop       │                                │
│  │  (IGameState)    │  │  (GameLoop)      │                                │
│  └──────────────────┘  └──────────────────┘                                │
└─────────────────────────────────────────────────────────────────────────────┘


## Detailed Execution Flow

### 1. Application Entry
```
Program.Main()
    ├── Console output startup messages
    ├── Display version info
    ├── Call EngineCore.InitializeAsync()
    │   └── Returns bool (success/failure)
    ├── IF initialized:
    │   └── Call EngineCore.RunAsync()
    ├── Call EngineCore.ShutdownAsync()
    └── Return exit code (0=success, 1=failure)
```

### 2. Engine Initialization
```
EngineCore.InitializeAsync()
    ├── Check _isInitialized flag (thread-safe lock)
    ├── IF already initialized: return true
    ├── Call InitializeCoreSystemsAsync()
    │   ├── Initialize Asset System
    │   ├── Initialize Logging System
    │   └── Return success/failure
    ├── Set _isInitialized = true
    └── Return true on success
```

### 3. GameRoot Initialization
```
GameRoot.Initialize()  (via GameRootMain.cs)
    ├── Check _isInitialized flag (thread-safe _stateLock)
    ├── IF already initialized: return
    ├── Call PerformInitialization() (via Initialization.cs)
    │   ├── PHASE 1: SystemManager.Initialize()
    │   ├── PHASE 2: UpdateManager.Initialize()
    │   ├── PHASE 3: RenderManager.Initialize()
    │   ├── PHASE 4: GameStateMachine.Initialize()
    │   └── PHASE 5: RenderContext.Initialize()
    ├── Set _isInitialized = true
    └── Log success
```

### 4. Main Game Loop
```
EngineCore.RunAsync() / GameLoop.Run()
    ├── Validate initialized state
    ├── WHILE running:
    │   ├── Update Phase
    │   │   └── UpdateManager.UpdateAll(deltaTime)
    │   │       └── For each IUpdateSystem:
    │   │           └── system.Update(deltaTime)
    │   ├── Render Phase
    │   │   └── RenderManager.RenderAll()
    │   │       └── For each render system:
    │   │           └── system.Render()
    │   └── Input Phase
    │       └── UIInputRouter.ProcessInput()
    └── Handle exceptions
```

### 5. System Dependencies
```
┌─────────────────────────────────────────────────────────┐
│              SYSTEM DEPENDENCY GRAPH                     │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  EngineCore (bootstrap)                                 │
│       │                                                 │
│       ├──► SystemRegistry (service locator)            │
│       │       ├──► SystemManager                       │
│       │       ├──► UpdateManager                       │
│       │       ├──► RenderManager                       │
│       │       └──► ECSWorld                            │
│       │                                                 │
│       ├──► GameRoot (central coordinator)              │
│       │       ├──► GameStateMachine                    │
│       │       ├──► UIInputRouter                       │
│       │       └──► IRenderContext                      │
│       │                                                 │
│       └──► ModernLoggingSystem (global logging)        │
│                                                         │
└─────────────────────────────────────────────────────────┘
```


## Animation System Execution

```
Animation Subsystem Flow
    │
    ├──► AnimationClip
    │       ├── AnimationTrack[] (per-property)
    │       │       ├── AnimationFrame[]
    │       │       └── Keyframe interpolation
    │       └── Transition data
    │
    ├──► AnimationPlayer
    │       ├── Play(clip, loop)
    │       ├── Stop()
    │       └── Sample(time) → pose
    │
    ├──► AnimationStateMachine
    │       ├── IAnimationState[] states
    │       │       ├── IdleState
    │       │       ├── MoveState
    │       │       ├── AttackState
    │       │       └── JumpState
    │       └── AnimationTransition[]
    │
    └──► BlendTree (runtime blending)
            ├── IBlendNode[]
            │       ├── SingleClipNode
            │       ├── LinearBlendNode
            │       └── TwoDBlendNode
            └── BlendParameters
```


## AI System Execution

```
AI Subsystem Flow
    │
    ├──► AIController (entity AI component)
    │       ├── IAIBehavior[] behaviors
    │       └── Update() → behavior.Tick()
    │
    ├──► Blackboard (shared memory)
    │       ├── Dictionary<string, object> data
    │       ├── SetValue<T>() / GetValue<T>()
    │       └── Event notification
    │
    └──► BasicChaseBehavior (example)
            ├── Target acquisition
            ├── Pathfinding
            └── Movement execution
```


## Achievement System Execution

```
Achievement Subsystem Flow
    │
    ├──► AchievementManager (singleton)
    │       ├── AchievementDefinition[] (static data)
    │       ├── AchievementInstance[] (runtime state)
    │       └── ChallengeInstance[] (progress tracking)
    │
    ├──► AchievementDefinition
    │       ├── Id, Name, Description
    │       ├── AchievementCategory, AchievementRarity
    │       ├── RequirementType, RequirementTarget
    │       └── PointValue, IconId
    │
    ├──► AchievementInstance
    │       ├── CurrentProgress
    │       ├── IsUnlocked, UnlockDate
    │       └── UpdateProgress()
    │
    └──► AchievementListRenderer / AchievementPopupRenderer
            ├── Render achievement UI
            ├── Progress bars
            └── Unlock notifications
```


## Shutdown Flow

```
EngineCore.ShutdownAsync() / GameRoot.Shutdown()
    ├── IF not initialized: return
    ├── Set _isRunning = false (stop game loop)
    ├── PerformShutdown() (reverse order)
    │   ├── PHASE 1: GameStateMachine.Shutdown()
    │   ├── PHASE 2: InputManager shutdown
    │   ├── PHASE 3: RenderManager.Shutdown()
    │   ├── PHASE 4: UpdateManager.Shutdown()
    │   ├── PHASE 5: SystemManager.Shutdown()
    │   └── PHASE 6: RenderContext.Shutdown()
    ├── Set _isInitialized = false
    └── Log completion
```


## File Structure Map

```
SASZombieAssaultTD/
├── Entry Point
│   └── Engine/Animation/Core/EngineCore.cs  (Program.Main, EngineCore)
│
├── Core Engine
│   ├── Engine/EngineBootstrap.cs          (DI, factory methods)
│   ├── Engine/GameRoot/GameRootMain.cs     (central hub)
│   ├── Engine/GameRoot/Initialization.cs   (startup sequence)
│   ├── Engine/Systems/SystemManager.cs     (system registry)
│   ├── Engine/Systems/UpdateManager.cs     (update scheduling)
│   ├── Engine/Systems/RenderManager.cs     (render scheduling)
│   └── Engine/GameLoop/GameLoopMain.cs     (main loop)
│
├── Logging Infrastructure
│   ├── Engine/Animation/Core/ModernLoggingSystem.cs
│   ├── Engine/Animation/Core/ModernLoggingSystemExtensions.cs
│   └── Engine/Animation/Core/LoggingSystemMonitor.cs
│
├── AI System
│   ├── Engine/AI/AIController.cs
│   ├── Engine/AI/Blackboard/Blackboard.cs
│   └── Engine/AI/Behaviors/BasicChaseBehavior.cs
│
├── Achievement System
│   ├── Engine/Achievements/AchievementDefinition.cs
│   ├── Engine/Achievements/AchievementInstance.cs
│   ├── Engine/Achievements/AchievementCategory.cs
│   ├── Engine/Achievements/AchievementRarity.cs
│   ├── Engine/Achievements/ChallengeDefinition.cs
│   ├── Engine/Achievements/ChallengeInstance.cs
│   └── Engine/Achievements/*Renderer.cs    (UI)
│
└── Animation System
    ├── BlendTree/
    │   ├── BlendTree.cs, BlendTreeNode.cs, IBlendNode.cs
    │   ├── LinearBlendNode.cs, TwoDBlendNode.cs, SingleClipNode.cs
    │   ├── BlendParameters.cs, BlendTreeSerializer.cs
    │   └── BlendTreeValidator.cs, ValidationReport.cs
    ├── Components/
    │   ├── AnimationControllerComponent.cs, AnimationPlayer.cs
    │   ├── AnimationStateMachine.cs, AnimationStateMachineComponent.cs
    │   ├── IAnimationState.cs, IdleState.cs, MoveState.cs
    │   ├── AttackState.cs, JumpState.cs, AnimationFrame.cs
    │   └── AnimationTypes.cs, AnimationStateInspector.cs
    └── Core/
        ├── AnimationClip.cs, AnimationTrack.cs
        ├── AnimationTransition.cs, AnimationTransitionDebug.cs
        └── AnimationConditionOperator.cs
```


## Execution Order Summary

| Phase | Component | Action |
|-------|-----------|--------|
| 1 | `Program.Main` | Entry point, display startup info |
| 2 | `EngineCore.InitializeAsync` | Bootstrap core systems |
| 3 | `SystemManager` | Initialize system registry |
| 4 | `UpdateManager` | Initialize update systems |
| 5 | `RenderManager` | Initialize render systems |
| 6 | `GameStateMachine` | Initialize game states |
| 7 | `RenderContext` | Initialize graphics |
| 8 | `GameLoop.Run` | Begin main loop |
| 9 | Update Phase | All `IUpdateSystem.Update()` |
| 10 | Render Phase | All render systems execute |
| 11 | Input Phase | Process user input |
| 12 | Repeat 9-11 | Until shutdown signal |
| 13 | `GameRoot.Shutdown` | Reverse initialization order |
| 14 | Cleanup | Release resources |


## Key Interfaces

```csharp
// Core update contract
public interface IUpdateSystem
{
    void Update(float deltaTime);
}

// Game state contract
public interface IGameStateMachine
{
    void Initialize();
    void Shutdown();
    void ChangeState(IGameState newState);
}

// Render context contract
public interface IRenderContext
{
    void Initialize();
    void Shutdown();
    // ... render methods
}

// AI behavior contract
public interface IAIBehavior
{
    void Tick(Blackboard blackboard, float deltaTime);
}

// Animation state contract
public interface IAnimationState
{
    void Enter();
    void Update(float deltaTime);
    void Exit();
    string CheckTransitions();
}

// Blend node contract
public interface IBlendNode
{
    BlendTreeNode Evaluate(BlendParameters parameters);
}
```


## Threading Model

```
┌─────────────────────────────────────────────────────────┐
│                   THREAD ARCHITECTURE                  │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  Main Thread                                            │
│  ├── Program.Main()                                     │
│  ├── Engine initialization (synchronous)                │
│  ├── Game loop (blocking)                               │
│  │   ├── Update (single-threaded)                      │
│  │   ├── Render (single-threaded)                      │
│  │   └── Input (single-threaded)                       │
│  └── Shutdown (synchronous)                             │
│                                                         │
│  Background Threads                                     │
│  ├── ModernLoggingSystem (async queue processing)      │
│  └── Asset loading (async I/O)                         │
│                                                         │
│  Thread Safety                                          │
│  ├── _initLock (EngineCore initialization)             │
│  ├── _stateLock (GameRoot state changes)               │
│  ├── _transitionLock (AnimationTransitionDebug)        │
│  └── ConcurrentQueue<> (logging queue)                 │
│                                                         │
└─────────────────────────────────────────────────────────┘
```
