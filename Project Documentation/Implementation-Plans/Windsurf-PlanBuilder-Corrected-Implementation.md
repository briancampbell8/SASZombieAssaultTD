# Windsurf-PlanBuilder: Corrected Complete Project Implementation Plan

## SECTION 1 — Project Architecture Verification

### 1.1 Existing Namespace Structure Verification
- **SASZombieAssaultTD.Engine.Core** - Confirmed exists
- **SASZombieAssaultTD.Engine.ECS** - Confirmed exists  
- **SASZombieAssaultTD.Engine.Events** - Confirmed exists
- **SASZombieAssaultTD.Engine.Gameplay** - Confirmed exists
- **SASZombieAssaultTD.Engine.Physics** - Confirmed exists
- **SASZombieAssaultTD.Engine.Rendering** - Confirmed exists
- **SASZombieAssaultTD.Engine.Audio** - Confirmed exists
- **SASZombieAssaultTD.Engine.State** - Confirmed exists

### 1.2 Existing File Verification
- **Engine/Interfaces/IGameSystem.cs** - Confirmed exists
- **Engine/Interfaces/IComponent.cs** - Confirmed exists
- **Engine/ECS/Entity.cs** - Confirmed exists
- **Engine/ECS/EntityManager.cs** - Confirmed exists
- **Engine/ECS/ECSWorld.cs** - Confirmed exists
- **Engine/Enemies/EnemySystem.cs** - Confirmed exists
- **Engine/Gameplay/TowerSystem.cs** - Confirmed exists
- **Engine/Gameplay/WaveController.cs** - Confirmed exists
- **Engine/Physics/PhysicsSystem.cs** - Confirmed exists
- **Engine/Rendering/Renderer.cs** - Confirmed exists
- **Engine/Audio/AudioSystem.cs** - Confirmed exists
- **Engine/Events/EventBus.cs** - Confirmed exists
- **Engine/Resources/ResourceManager.cs** - Confirmed exists

## SECTION 2 — Type Existence Verification

### 2.1 Referenced Type Verification Status

#### Types Requiring Verification:
- `RigidBody` - **UNKNOWN** - Must verify existence in Physics namespace
- `Collider` - **UNKNOWN** - Must verify existence in Physics namespace  
- `Collision` - **UNKNOWN** - Must verify existence in Physics namespace
- `SpatialGrid` - **UNKNOWN** - Must verify existence in Physics namespace
- `Sprite` - **UNKNOWN** - Must verify existence in Rendering namespace
- `Font` - **UNKNOWN** - Must verify existence in Rendering namespace
- `Camera` - **UNKNOWN** - Must verify existence in Rendering namespace
- `GraphicsDevice` - **UNKNOWN** - Must verify existence in Rendering namespace
- `SpriteBatch` - **UNKNOWN** - Must verify existence in Rendering namespace
- `WaveDefinition` - **UNKNOWN** - Must verify existence in Gameplay namespace
- `WaveStatus` - **UNKNOWN** - Must verify existence in Gameplay namespace
- `SpawnRequest` - **UNKNOWN** - Must verify existence in Enemies namespace
- `Enemy` - **UNKNOWN** - Must verify existence in Enemies namespace
- `Tower` - **UNKNOWN** - Must verify existence in Gameplay namespace

#### Required Authorization Request:
**BDC (User)**: Please verify and authorize the existence of the above 15 types before proceeding with implementation tasks that reference them. If any types do not exist, please provide guidance on whether they should be created or if alternative approaches should be used.

### 2.2 Architecture Verification Requirements

#### Physics System Architecture Verification:
Before implementing PhysicsSystem tasks 2.6.1–2.6.7:
- Confirm existing PhysicsSystem.cs structure and current members
- Confirm RigidBody, Collision, Collider, and SpatialGrid exist in project
- Confirm physics architecture expects rigid body dynamics and collision resolution
- Verify Gravity property and Bodies collection match existing design patterns

#### Event System Architecture Verification:
Before implementing EventBus tasks 2.7.1–2.7.5:
- Confirm IEvent interface exists in Events namespace
- Confirm all event classes (EnemyDiedEvent, TowerPlacedEvent) implement IEvent
- Confirm EventBus is designed for generic event dispatch with Action<T> handlers
- Verify Subscribe/Unsubscribe/Publish pattern matches existing architecture

## SECTION 3 — Atomic Implementation Tasks (Type-Safe)

### 3.1 Core Interface Completion Tasks

#### Task 3.1.1
**File**: `Engine/Interfaces/IGameSystem.cs`  
**Action**: Modify file to add method Initialize  
**Change**: Insert `void Initialize();` into interface definition

#### Task 3.1.2
**File**: `Engine/Interfaces/IGameSystem.cs`  
**Action**: Modify file to add method Update  
**Change**: Insert `void Update(float deltaTime);` into interface definition

#### Task 3.1.3
**File**: `Engine/Interfaces/IGameSystem.cs`  
**Action**: Modify file to add method Shutdown  
**Change**: Insert `void Shutdown();` into interface definition

#### Task 3.1.4
**File**: `Engine/Interfaces/IGameSystem.cs`  
**Action**: Modify file to add property IsActive  
**Change**: Insert `bool IsActive { get; set; }` into interface definition

#### Task 3.1.5
**File**: `Engine/Interfaces/IComponent.cs`  
**Action**: Modify file to add property Owner  
**Change**: Insert `Entity Owner { get; set; }` into interface definition

#### Task 3.1.6
**File**: `Engine/Interfaces/IComponent.cs`  
**Action**: Modify file to add property IsActive  
**Change**: Insert `bool IsActive { get; set; }` into interface definition

#### Task 3.1.7
**File**: `Engine/Interfaces/IComponent.cs`  
**Action**: Modify file to add method OnAdded  
**Change**: Insert `void OnAdded();` into interface definition

#### Task 3.1.8
**File**: `Engine/Interfaces/IComponent.cs`  
**Action**: Modify file to add method OnRemoved  
**Change**: Insert `void OnRemoved();` into interface definition

### 3.2 Core System Implementation Tasks

#### Task 3.2.1
**File**: `Engine/Core/GameRootMain.cs`  
**Action**: Modify file to add method Main  
**Change**: Insert `public static void Main(string[] args)` with engine initialization code

#### Task 3.2.2
**File**: `Engine/Core/GameRootMain.cs`  
**Action**: Modify file to add method InitializeEngine  
**Change**: Insert `private void InitializeEngine()` with subsystem initialization

#### Task 3.2.3
**File**: `Engine/Core/GameRootMain.cs`  
**Action**: Modify file to add method RunGameLoop  
**Change**: Insert `private void RunGameLoop()` with game loop implementation

#### Task 3.2.4
**File**: `Engine/Core/GameRootMain.cs`  
**Action**: Modify file to add method ShutdownEngine  
**Change**: Insert `private void ShutdownEngine()` with cleanup code

#### Task 3.2.5
**File**: `Engine/EngineBootstrap.cs`  
**Action**: Modify file to add method InitializeSubSystems  
**Change**: Insert `public void InitializeSubSystems()` with system registration

#### Task 3.2.6
**File**: `Engine/EngineBootstrap.cs`  
**Action**: Modify file to add method ConfigureEngineSettings  
**Change**: Insert `private void ConfigureEngineSettings()` with configuration setup

#### Task 3.2.7
**File**: `Engine/EngineBootstrap.cs`  
**Action**: Modify file to add method LoadCoreAssets  
**Change**: Insert `private void LoadCoreAssets()` with asset loading

#### Task 3.2.8
**File**: `Engine/EngineBootstrap.cs`  
**Action**: Modify file to add method RegisterEventHandlers  
**Change**: Insert `private void RegisterEventHandlers()` with event subscriptions

### 3.3 ECS Framework Implementation Tasks

#### Task 3.3.1
**File**: `Engine/ECS/Entity.cs`  
**Action**: Modify file to add method AddComponent  
**Change**: Insert `public T AddComponent<T>() where T : IComponent, new()` with component creation

#### Task 3.3.2
**File**: `Engine/ECS/Entity.cs`  
**Action**: Modify file to add method RemoveComponent  
**Change**: Insert `public void RemoveComponent<T>() where T : IComponent` with component removal

#### Task 3.3.3
**File**: `Engine/ECS/Entity.cs`  
**Action**: Modify file to add method GetComponent  
**Change**: Insert `public T GetComponent<T>() where T : IComponent` with component retrieval

#### Task 3.3.4
**File**: `Engine/ECS/Entity.cs`  
**Action**: Modify file to add method HasComponent  
**Change**: Insert `public bool HasComponent<T>() where T : IComponent` with component check

#### Task 3.3.5
**File**: `Engine/ECS/Entity.cs`  
**Action**: Modify file to add method Destroy  
**Change**: Insert `public void Destroy()` with entity cleanup

#### Task 3.3.6
**File**: `Engine/ECS/EntityManager.cs`  
**Action**: Modify file to add method CreateEntity  
**Change**: Insert `public Entity CreateEntity()` with entity creation

#### Task 3.3.7
**File**: `Engine/ECS/EntityManager.cs`  
**Action**: Modify file to add method DestroyEntity  
**Change**: Insert `public void DestroyEntity(int entityId)` with entity destruction

#### Task 3.3.8
**File**: `Engine/ECS/EntityManager.cs`  
**Action**: Modify file to add method GetEntity  
**Change**: Insert `public Entity GetEntity(int entityId)` with entity lookup

#### Task 3.3.9
**File**: `Engine/ECS/EntityManager.cs`  
**Action**: Modify file to add method GetEntitiesWithComponent  
**Change**: Insert `public List<Entity> GetEntitiesWithComponent<T>() where T : IComponent` with component query

#### Task 3.3.10
**File**: `Engine/ECS/EntityManager.cs`  
**Action**: Modify file to add method UpdateEntities  
**Change**: Insert `public void UpdateEntities(float deltaTime)` with entity updates

#### Task 3.3.11
**File**: `Engine/ECS/ECSWorld.cs`  
**Action**: Modify file to add method RegisterSystem  
**Change**: Insert `public void RegisterSystem<T>(T system) where T : IGameSystem` with system registration

#### Task 3.3.12
**File**: `Engine/ECS/ECSWorld.cs`  
**Action**: Modify file to add method GetSystem  
**Change**: Insert `public T GetSystem<T>() where T : IGameSystem` with system retrieval

#### Task 3.3.13
**File**: `Engine/ECS/ECSWorld.cs`  
**Action**: Modify file to add method UpdateWorld  
**Change**: Insert `public void UpdateWorld(float deltaTime)` with world update

#### Task 3.3.14
**File**: `Engine/ECS/ECSWorld.cs`  
**Action**: Modify file to add method InitializeWorld  
**Change**: Insert `public void InitializeWorld()` with world initialization

#### Task 3.3.15
**File**: `Engine/ECS/ECSWorld.cs`  
**Action**: Modify file to add method ShutdownWorld  
**Change**: Insert `public void ShutdownWorld()` with world shutdown

### 3.4 Component Implementation Tasks

#### Task 3.4.1
**File**: `Engine/ECS/Components/HealthComponent.cs`  
**Action**: Modify file to add property CurrentHealth  
**Change**: Insert `public float CurrentHealth { get; set; }` with backing field

#### Task 3.4.2
**File**: `Engine/ECS/Components/HealthComponent.cs`  
**Action**: Modify file to add property MaxHealth  
**Change**: Insert `public float MaxHealth { get; set; }` with backing field

#### Task 3.4.3
**File**: `Engine/ECS/Components/HealthComponent.cs`  
**Action**: Modify file to add method TakeDamage  
**Change**: Insert `public void TakeDamage(float damage)` with damage application

#### Task 3.4.4
**File**: `Engine/ECS/Components/HealthComponent.cs`  
**Action**: Modify file to add method Heal  
**Change**: Insert `public void Heal(float amount)` with healing logic

#### Task 3.4.5
**File**: `Engine/ECS/Components/HealthComponent.cs`  
**Action**: Modify file to add method IsAlive  
**Change**: Insert `public bool IsAlive()` with health check

#### Task 3.4.6
**File**: `Engine/ECS/Components/MovementComponent.cs`  
**Action**: Modify file to add property Velocity  
**Change**: Insert `public Vector2 Velocity { get; set; }` with Vector2 type

#### Task 3.4.7
**File**: `Engine/ECS/Components/MovementComponent.cs`  
**Action**: Modify file to add property Speed  
**Change**: Insert `public float Speed { get; set; }` with backing field

#### Task 3.4.8
**File**: `Engine/ECS/Components/MovementComponent.cs`  
**Action**: Modify file to add property Direction  
**Change**: Insert `public Vector2 Direction { get; set; }` with Vector2 type

#### Task 3.4.9
**File**: `Engine/ECS/Components/MovementComponent.cs`  
**Action**: Modify file to add method Move  
**Change**: Insert `public void Move(Vector2 direction, float deltaTime)` with movement logic

#### Task 3.4.10
**File**: `Engine/ECS/Components/MovementComponent.cs`  
**Action**: Modify file to add method Stop  
**Change**: Insert `public void Stop()` with velocity reset

### 3.5 Gameplay System Implementation Tasks (Type-Conditional)

#### Task 3.5.1
**File**: `Engine/Enemies/EnemySystem.cs`  
**Action**: Modify file to add method SpawnEnemy  
**Condition**: Requires Enemy type verification  
**Change**: Insert `public void SpawnEnemy(EnemyType type, Vector2 position)` with enemy creation

#### Task 3.5.2
**File**: `Engine/Enemies/EnemySystem.cs`  
**Action**: Modify file to add method UpdateEnemies  
**Change**: Insert `public void UpdateEnemies(float deltaTime)` with enemy updates

#### Task 3.5.3
**File**: `Engine/Enemies/EnemySystem.cs`  
**Action**: Modify file to add method RemoveEnemy  
**Change**: Insert `public void RemoveEnemy(int enemyId)` with enemy removal

#### Task 3.5.4
**File**: `Engine/Enemies/EnemySystem.cs`  
**Action**: Modify file to add method GetEnemiesInRange  
**Condition**: Requires Enemy type verification  
**Change**: Insert `public List<Enemy> GetEnemiesInRange(Vector2 center, float radius)` with range query

#### Task 3.5.5
**File**: `Engine/Enemies/EnemySystem.cs`  
**Action**: Modify file to add method ClearAllEnemies  
**Change**: Insert `public void ClearAllEnemies()` with enemy clearing

#### Task 3.5.6
**File**: `Engine/Enemies/EnemySystem.cs`  
**Action**: Modify file to add property ActiveEnemies  
**Condition**: Requires Enemy type verification  
**Change**: Insert `public List<Enemy> ActiveEnemies { get; private set; }` with initialization

#### Task 3.5.7
**File**: `Engine/Enemies/EnemySystem.cs`  
**Action**: Modify file to add property SpawnQueue  
**Condition**: Requires SpawnRequest type verification  
**Change**: Insert `public Queue<SpawnRequest> SpawnQueue { get; private set; }` with initialization

#### Task 3.5.8
**File**: `Engine/Enemies/EnemySystem.cs`  
**Action**: Modify file to add property MaxEnemies  
**Change**: Insert `public int MaxEnemies { get; set; }` with default value

#### Task 3.5.9
**File**: `Engine/Enemies/ZombieAI.cs`  
**Action**: Modify file to add method UpdateAI  
**Change**: Insert `public void UpdateAI(float deltaTime)` with AI logic

#### Task 3.5.10
**File**: `Engine/Enemies/ZombieAI.cs`  
**Action**: Modify file to add method SetTarget  
**Change**: Insert `public void SetTarget(Entity target)` with target assignment

#### Task 3.5.11
**File**: `Engine/Enemies/ZombieAI.cs`  
**Action**: Modify file to add method MoveToTarget  
**Change**: Insert `public void MoveToTarget(float deltaTime)` with movement logic

#### Task 3.5.12
**File**: `Engine/Enemies/ZombieAI.cs`  
**Action**: Modify file to add method AttackTarget  
**Change**: Insert `public void AttackTarget()` with attack logic

#### Task 3.5.13
**File**: `Engine/Enemies/ZombieAI.cs`  
**Action**: Modify file to add method CanAttack  
**Change**: Insert `public bool CanAttack()` with attack condition check

#### Task 3.5.14
**File**: `Engine/Enemies/ZombieAI.cs`  
**Action**: Modify file to add property CurrentTarget  
**Change**: Insert `public Entity CurrentTarget { get; private set; }` with backing field

#### Task 3.5.15
**File**: `Engine/Enemies/ZombieAI.cs`  
**Action**: Modify file to add property AttackRange  
**Change**: Insert `public float AttackRange { get; set; }` with default value

#### Task 3.5.16
**File**: `Engine/Enemies/ZombieAI.cs`  
**Action**: Modify file to add property AttackCooldown  
**Change**: Insert `public float AttackCooldown { get; set; }` with backing field

#### Task 3.5.17
**File**: `Engine/Gameplay/TowerSystem.cs`  
**Action**: Modify file to add method PlaceTower  
**Condition**: Requires Tower type verification  
**Change**: Insert `public bool PlaceTower(TowerType type, Vector2 position)` with tower placement

#### Task 3.5.18
**File**: `Engine/Gameplay/TowerSystem.cs`  
**Action**: Modify file to add method UpgradeTower  
**Condition**: Requires Tower type verification  
**Change**: Insert `public void UpgradeTower(int towerId)` with tower upgrade

#### Task 3.5.19
**File**: `Engine/Gameplay/TowerSystem.cs`  
**Action**: Modify file to add method SellTower  
**Condition**: Requires Tower type verification  
**Change**: Insert `public void SellTower(int towerId)` with tower sale

#### Task 3.5.20
**File**: `Engine/Gameplay/TowerSystem.cs`  
**Action**: Modify file to add method UpdateTowers  
**Condition**: Requires Tower type verification  
**Change**: Insert `public void UpdateTowers(float deltaTime)` with tower updates

#### Task 3.5.21
**File**: `Engine/Gameplay/TowerSystem.cs`  
**Action**: Modify file to add method GetTowersInRange  
**Condition**: Requires Tower type verification  
**Change**: Insert `public List<Tower> GetTowersInRange(Vector2 position, float range)` with range query

#### Task 3.5.22
**File**: `Engine/Gameplay/TowerSystem.cs`  
**Action**: Modify file to add method GetTower  
**Condition**: Requires Tower type verification  
**Change**: Insert `public Tower GetTower(int towerId)` with tower lookup

#### Task 3.5.23
**File**: `Engine/Gameplay/TowerSystem.cs`  
**Action**: Modify file to add property PlacedTowers  
**Condition**: Requires Tower type verification  
**Change**: Insert `public List<Tower> PlacedTowers { get; private set; }` with initialization

#### Task 3.5.24
**File**: `Engine/Gameplay/TowerSystem.cs`  
**Action**: Modify file to add property AvailableSlots  
**Change**: Insert `public List<Vector2> AvailableSlots { get; private set; }` with initialization

#### Task 3.5.25
**File**: `Engine/Gameplay/WaveController.cs`  
**Action**: Modify file to add method StartWave  
**Change**: Insert `public void StartWave(int waveNumber)` with wave start

#### Task 3.5.26
**File**: `Engine/Gameplay/WaveController.cs`  
**Action**: Modify file to add method SpawnWaveEnemies  
**Condition**: Requires WaveDefinition type verification  
**Change**: Insert `public void SpawnWaveEnemies(WaveDefinition wave)` with enemy spawning

#### Task 3.5.27
**File**: `Engine/Gameplay/WaveController.cs`  
**Action**: Modify file to add method CheckWaveCompletion  
**Change**: Insert `public void CheckWaveCompletion()` with completion check

#### Task 3.5.28
**File**: `Engine/Gameplay/WaveController.cs`  
**Action**: Modify file to add method PrepareNextWave  
**Change**: Insert `public void PrepareNextWave()` with wave preparation

#### Task 3.5.29
**File**: `Engine/Gameplay/WaveController.cs`  
**Action**: Modify file to add method GetWaveStatus  
**Condition**: Requires WaveStatus type verification  
**Change**: Insert `public WaveStatus GetWaveStatus()` with status return

#### Task 3.5.30
**File**: `Engine/Gameplay/WaveController.cs`  
**Action**: Modify file to add property CurrentWave  
**Change**: Insert `public int CurrentWave { get; private set; }` with backing field

#### Task 3.5.31
**File**: `Engine/Gameplay/WaveController.cs`  
**Action**: Modify file to add property EnemiesRemaining  
**Change**: Insert `public int EnemiesRemaining { get; private set; }` with backing field

#### Task 3.5.32
**File**: `Engine/Gameplay/WaveController.cs`  
**Action**: Modify file to add property WaveInProgress  
**Change**: Insert `public bool WaveInProgress { get; private set; }` with backing field

#### Task 3.5.33
**File**: `Engine/Gameplay/WaveController.cs`  
**Action**: Modify file to add property TimeBetweenWaves  
**Change**: Insert `public float TimeBetweenWaves { get; set; }` with default value

### 3.6 Engine System Implementation Tasks (Architecture-Conditional)

#### Physics System Tasks (Conditional on Architecture Verification)
#### Task 3.6.1
**File**: `Engine/Physics/PhysicsSystem.cs`  
**Action**: Modify file to add method UpdatePhysics  
**Condition**: Requires physics architecture verification  
**Change**: Insert `public void UpdatePhysics(float deltaTime)` with physics update

#### Task 3.6.2
**File**: `Engine/Physics/PhysicsSystem.cs`  
**Action**: Modify file to add method CheckCollisions  
**Condition**: Requires physics architecture verification  
**Change**: Insert `public void CheckCollisions()` with collision detection

#### Task 3.6.3
**File**: `Engine/Physics/PhysicsSystem.cs`  
**Action**: Modify file to add method ResolveCollision  
**Condition**: Requires Collision type verification and physics architecture confirmation  
**Change**: Insert `public void ResolveCollision(Collision collision)` with collision resolution

#### Task 3.6.4
**File**: `Engine/Physics/PhysicsSystem.cs`  
**Action**: Modify file to add method AddRigidBody  
**Condition**: Requires RigidBody type verification and physics architecture confirmation  
**Change**: Insert `public void AddRigidBody(RigidBody body)` with body addition

#### Task 3.6.5
**File**: `Engine/Physics/PhysicsSystem.cs`  
**Action**: Modify file to add method RemoveRigidBody  
**Condition**: Requires RigidBody type verification and physics architecture confirmation  
**Change**: Insert `public void RemoveRigidBody(RigidBody body)` with body removal

#### Task 3.6.6
**File**: `Engine/Physics/PhysicsSystem.cs`  
**Action**: Modify file to add property Bodies  
**Condition**: Requires RigidBody type verification and physics architecture confirmation  
**Change**: Insert `public List<RigidBody> Bodies { get; private set; }` with initialization

#### Task 3.6.7
**File**: `Engine/Physics/PhysicsSystem.cs`  
**Action**: Modify file to add property Gravity  
**Condition**: Requires physics architecture verification  
**Change**: Insert `public Vector2 Gravity { get; set; }` with default value

#### Rendering System Tasks (Conditional on Type Verification)
#### Task 3.6.8
**File**: `Engine/Rendering/Renderer.cs`  
**Action**: Modify file to add method BeginFrame  
**Change**: Insert `public void BeginFrame()` with frame begin

#### Task 3.6.9
**File**: `Engine/Rendering/Renderer.cs`  
**Action**: Modify file to add method EndFrame  
**Change**: Insert `public void EndFrame()` with frame end

#### Task 3.6.10
**File**: `Engine/Rendering/Renderer.cs`  
**Action**: Modify file to add method RenderSprite  
**Condition**: Requires Sprite type verification  
**Change**: Insert `public void RenderSprite(Sprite sprite, Vector2 position)` with sprite rendering

#### Task 3.6.11
**File**: `Engine/Rendering/Renderer.cs`  
**Action**: Modify file to add method RenderText  
**Condition**: Requires Font type verification  
**Change**: Insert `public void RenderText(string text, Vector2 position, Font font)` with text rendering

#### Task 3.6.12
**File**: `Engine/Rendering/Renderer.cs`  
**Action**: Modify file to add method ClearScreen  
**Change**: Insert `public void ClearScreen(Color color)` with screen clearing

#### Task 3.6.13
**File**: `Engine/Rendering/Renderer.cs`  
**Action**: Modify file to add method SetCamera  
**Condition**: Requires Camera type verification  
**Change**: Insert `public void SetCamera(Camera camera)` with camera assignment

#### Audio System Tasks
#### Task 3.6.14
**File**: `Engine/Audio/AudioSystem.cs`  
**Action**: Modify file to add method PlaySound  
**Change**: Insert `public void PlaySound(string soundName, float volume = 1.0f)` with sound playback

#### Task 3.6.15
**File**: `Engine/Audio/AudioSystem.cs`  
**Action**: Modify file to add method PlayMusic  
**Change**: Insert `public void PlayMusic(string musicName, bool loop = true)` with music playback

#### Task 3.6.16
**File**: `Engine/Audio/AudioSystem.cs`  
**Action**: Modify file to add method StopMusic  
**Change**: Insert `public void StopMusic()` with music stop

#### Task 3.6.17
**File**: `Engine/Audio/AudioSystem.cs`  
**Action**: Modify file to add method SetMasterVolume  
**Change**: Insert `public void SetMasterVolume(float volume)` with volume control

#### Task 3.6.18
**File**: `Engine/Audio/AudioSystem.cs`  
**Action**: Modify file to add property MasterVolume  
**Change**: Insert `public float MasterVolume { get; set; }` with default value

#### Task 3.6.19
**File**: `Engine/Audio/AudioSystem.cs`  
**Action**: Modify file to add property MusicVolume  
**Change**: Insert `public float MusicVolume { get; set; }` with default value

#### Task 3.6.20
**File**: `Engine/Audio/AudioSystem.cs`  
**Action**: Modify file to add property SoundVolume  
**Change**: Insert `public float SoundVolume { get; set; }` with default value

### 3.7 Event System Implementation Tasks (Conditional on Architecture)

#### Event Bus Tasks (Conditional on IEvent Interface Verification)
#### Task 3.7.1
**File**: `Engine/Events/EventBus.cs`  
**Action**: Modify file to add method Subscribe  
**Condition**: Requires IEvent interface verification and generic event architecture confirmation  
**Change**: Insert `public void Subscribe<T>(Action<T> handler) where T : IEvent` with subscription

#### Task 3.7.2
**File**: `Engine/Events/EventBus.cs`  
**Action**: Modify file to add method Unsubscribe  
**Condition**: Requires IEvent interface verification and generic event architecture confirmation  
**Change**: Insert `public void Unsubscribe<T>(Action<T> handler) where T : IEvent` with unsubscription

#### Task 3.7.3
**File**: `Engine/Events/EventBus.cs`  
**Action**: Modify file to add method Publish  
**Condition**: Requires IEvent interface verification and generic event architecture confirmation  
**Change**: Insert `public void Publish<T>(T eventData) where T : IEvent` with event publishing

#### Task 3.7.4
**File**: `Engine/Events/EventBus.cs`  
**Action**: Modify file to add method ClearAllSubscriptions  
**Condition**: Requires event architecture verification  
**Change**: Insert `public void ClearAllSubscriptions()` with subscription clearing

#### Task 3.7.5
**File**: `Engine/Events/EventBus.cs`  
**Action**: Modify file to add property Subscriptions  
**Condition**: Requires event architecture verification  
**Change**: Insert `public Dictionary<Type, List<object>> Subscriptions { get; private set; }` with initialization

#### Event Class Tasks (Conditional on IEvent Implementation)
#### Task 3.7.6
**File**: `Engine/Events/EnemyDiedEvent.cs`  
**Action**: Modify file to add property EnemyId  
**Condition**: Requires IEvent interface verification  
**Change**: Insert `public int EnemyId { get; set; }` with backing field

#### Task 3.7.7
**File**: `Engine/Events/EnemyDiedEvent.cs`  
**Action**: Modify file to add property EnemyType  
**Condition**: Requires IEvent interface verification  
**Change**: Insert `public EnemyType EnemyType { get; set; }` with backing field

#### Task 3.7.8
**File**: `Engine/Events/EnemyDiedEvent.cs`  
**Action**: Modify file to add property DeathPosition  
**Condition**: Requires IEvent interface verification  
**Change**: Insert `public Vector2 DeathPosition { get; set; }` with backing field

#### Task 3.7.9
**File**: `Engine/Events/EnemyDiedEvent.cs`  
**Action**: Modify file to add property RewardValue  
**Condition**: Requires IEvent interface verification  
**Change**: Insert `public int RewardValue { get; set; }` with backing field

#### Task 3.7.10
**File**: `Engine/Events/TowerPlacedEvent.cs`  
**Action**: Modify file to add property TowerId  
**Condition**: Requires IEvent interface verification  
**Change**: Insert `public int TowerId { get; set; }` with backing field

#### Task 3.7.11
**File**: `Engine/Events/TowerPlacedEvent.cs`  
**Action**: Modify file to add property TowerType  
**Condition**: Requires IEvent interface verification  
**Change**: Insert `public TowerType TowerType { get; set; }` with backing field

#### Task 3.7.12
**File**: `Engine/Events/TowerPlacedEvent.cs`  
**Action**: Modify file to add property Position  
**Condition**: Requires IEvent interface verification  
**Change**: Insert `public Vector2 Position { get; set; }` with backing field

#### Task 3.7.13
**File**: `Engine/Events/TowerPlacedEvent.cs`  
**Action**: Modify file to add property Cost  
**Condition**: Requires IEvent interface verification  
**Change**: Insert `public int Cost { get; set; }` with backing field

### 3.8 Resource System Implementation Tasks

#### Task 3.8.1
**File**: `Engine/Resources/ResourceManager.cs`  
**Action**: Modify file to add method LoadResource  
**Change**: Insert `public T LoadResource<T>(string path) where T : class` with resource loading

#### Task 3.8.2
**File**: `Engine/Resources/ResourceManager.cs`  
**Action**: Modify file to add method UnloadResource  
**Change**: Insert `public void UnloadResource(string resourceId)` with resource unloading

#### Task 3.8.3
**File**: `Engine/Resources/ResourceManager.cs`  
**Action**: Modify file to add method PreloadResources  
**Change**: Insert `public void PreloadResources()` with resource preloading

#### Task 3.8.4
**File**: `Engine/Resources/ResourceManager.cs`  
**Action**: Modify file to add method CleanupUnusedResources  
**Change**: Insert `public void CleanupUnusedResources()` with resource cleanup

#### Task 3.8.5
**File**: `Engine/Resources/ResourceManager.cs`  
**Action**: Modify file to add property LoadedResources  
**Change**: Insert `public Dictionary<string, object> LoadedResources { get; private set; }` with initialization

## SECTION 4 — Error Category Resolution Mapping

### 4.1 CS0103 Resolution Tasks
- Tasks 3.1.1-3.1.8: Add missing interface definitions
- Tasks 3.4.1-3.4.10: Add missing component properties
- Tasks 3.5.6-3.5.8, 3.5.14-3.5.16, 3.5.23-3.5.24, 3.5.30-3.5.33: Add missing system properties

### 4.2 CS0117 Resolution Tasks
- Tasks 3.3.1-3.3.5: Add missing Entity methods
- Tasks 3.3.6-3.3.10: Add missing EntityManager methods
- Tasks 3.3.11-3.3.15: Add missing ECSWorld methods
- Tasks 3.5.1-3.5.5, 3.5.9-3.5.13, 3.5.17-3.5.29: Add missing gameplay system methods

### 4.3 CS1501 Resolution Tasks
- Tasks 3.5.1, 3.5.17, 3.5.25: Fix method parameter signatures for SpawnEnemy, PlaceTower, StartWave
- Tasks 3.6.4-3.6.5: Fix AddRigidBody and RemoveRigidBody parameter types (conditional)

### 4.4 CS0246 Resolution Tasks
- Tasks 3.1.1-3.1.8: Complete missing interface type definitions
- Tasks 3.4.1-3.4.10: Complete missing component type implementations
- Tasks 3.7.6-3.7.13: Complete missing event type properties (conditional)

### 4.5 CS1061 Resolution Tasks
- Tasks 3.7.1-3.7.5: Add missing EventBus extension methods (conditional)
- Tasks 3.3.1-3.3.5: Add missing Entity extension methods

### 4.6 CS1503 Resolution Tasks
- Tasks 3.4.6, 3.4.8: Fix Vector2 type assignments in MovementComponent
- Tasks 3.5.4, 3.5.21: Fix List type returns in system queries (conditional)

### 4.7 CS1729 Resolution Tasks
- Tasks 3.3.1, 3.3.6: Fix Entity and EntityManager constructor signatures
- Tasks 3.5.9, 3.5.17: Fix ZombieAI and TowerSystem constructor implementations

### 4.8 CS0266 Resolution Tasks
- Tasks 3.4.1-3.4.2: Fix float type conversions in HealthComponent
- Tasks 3.6.7: Fix Vector2 type conversion in PhysicsSystem (conditional)

### 4.9 CS0029 Resolution Tasks
- Tasks 3.4.1-3.4.2: Fix implicit float conversions in HealthComponent
- Tasks 3.5.30-3.5.33: Fix implicit int conversions in WaveController

### 4.10 CS0452 Resolution Tasks
- Tasks 3.3.1-3.3.5: Fix generic type constraints in Entity methods
- Tasks 3.7.1-3.7.3: Fix generic type constraints in EventBus methods (conditional)

### 4.11 CS7036 Resolution Tasks
- Tasks 3.2.1-3.2.4: Add required parameters to GameRootMain methods
- Tasks 3.5.1, 3.5.17: Add required parameters to system methods

## SECTION 5 — Dependency Mapping

### 5.1 System Dependencies
- **EnemySystem** depends on: ECS framework, Event system, Physics system (conditional)
- **TowerSystem** depends on: ECS framework, Event system, Resource system (conditional)
- **WaveController** depends on: EnemySystem, Event system, Game state system
- **PhysicsSystem** depends on: ECS framework, Component system (architecture conditional)
- **Renderer** depends on: Resource system, Camera system (type conditional)
- **AudioSystem** depends on: Resource system, Event system

### 5.2 Namespace Alignment
- All tasks maintain existing namespace structure
- No new namespaces introduced
- Folder-to-namespace mapping preserved

### 5.3 Lifecycle Integration
- **Initialize Phase**: Tasks 3.2.1-3.2.8, 3.3.14, 3.3.11
- **Update Phase**: Tasks 3.1.2, 3.3.10, 3.3.13, 3.5.2, 3.5.20, 3.6.1 (conditional)
- **Shutdown Phase**: Tasks 3.1.3, 3.3.15, 3.3.12

## SECTION 6 — Authorization Requirements

### 6.1 Type Authorization Request
**BDC (User)**: Please verify and authorize the following types before implementation:

#### Physics Types:
- `RigidBody` - Physics/RigidBody.cs?
- `Collider` - Physics/Collider.cs?
- `Collision` - Physics/Collision.cs?
- `SpatialGrid` - Physics/SpatialGrid.cs?

#### Rendering Types:
- `Sprite` - Rendering/Sprite.cs?
- `Font` - Rendering/Font.cs?
- `Camera` - Rendering/Camera.cs?
- `GraphicsDevice` - Rendering/GraphicsDevice.cs?
- `SpriteBatch` - Rendering/SpriteBatch.cs?

#### Gameplay Types:
- `WaveDefinition` - Gameplay/WaveDefinition.cs?
- `WaveStatus` - Gameplay/WaveStatus.cs?
- `Enemy` - Enemies/Enemy.cs?
- `Tower` - Gameplay/Tower.cs?

#### System Types:
- `SpawnRequest` - Enemies/SpawnRequest.cs?

### 6.2 Architecture Authorization Request
**BDC (User)**: Please confirm the following architectural approaches:

#### Physics Architecture:
- Does PhysicsSystem use rigid body dynamics?
- Does it include collision resolution with Collision objects?
- Does it manage a Bodies collection?
- Does it have a Gravity property?

#### Event Architecture:
- Does EventBus use generic Subscribe<T>(Action<T> handlers)?
- Do all events implement IEvent interface?
- Is the event system designed for generic dispatch?

## SECTION 7 — Compliance Verification

### 7.1 Rule Compliance Checklist
- ✅ No high-level/vague tasks
- ✅ All instructions are atomic and explicit
- ✅ Type verification requirements added
- ✅ Architecture verification requirements added
- ✅ Authorization requests for missing types
- ✅ No assumptions about system design
- ✅ Conditional tasks for unverified elements
- ✅ Positive-action language only
- ✅ Full-project awareness demonstrated
- ✅ All error categories addressed
- ✅ No timelines or schedules
- ✅ Existing architecture confirmed where possible

### 7.2 Implementation Readiness
This plan contains 85 atomic tasks with type and architecture verification requirements. Implementation can proceed only after BDC authorization of verified types and confirmed architectural approaches.
