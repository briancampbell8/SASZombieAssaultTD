# P11-12 — Entity & Component System Migration Documentation

## Overview

P11-12 implements a modern Entity Component System (ECS) to replace legacy entity-like classes. This document outlines what has been migrated and what remains as legacy code.

## Legacy Entity System Analysis

### Existing Entity-Like Classes

#### 1. `Engine/Entities/Entity.cs` (Legacy Base)
- **Purpose**: Abstract base class with Position, Rotation, Scale
- **Methods**: `Update(float deltaTime)`, `Render(IRenderContext context)`
- **Status**: **CONFLICTS WITH NEW ECS ENTITY** - Namespace collision
- **Migration**: **REPLACED** by new ECS Entity and components

#### 2. `Engine/Entities/Enemy.cs` (Legacy)
- **Purpose**: Enemy entity with health, movement, scoring
- **Key Properties**: Health, MaxHealth, ScoreValue, EnemyType, Speed, IsDead
- **Methods**: Health management, death handling
- **Migration**: **PARTIALLY MIGRATED** to ECS components:
  - `HealthComponent` - Health management (CurrentHealth, MaxHealth, damage/heal events)
  - `MovementComponent` - Movement functionality (Speed, Velocity, acceleration)
  - Remaining: EnemyType, ScoreValue need dedicated components

#### 3. `Engine/Entities/Projectile.cs` (Legacy)
- **Purpose**: Projectile with position, velocity, damage
- **Key Properties**: Id, Velocity, Damage, Active
- **Methods**: Movement update, simple rendering
- **Migration**: **PARTIALLY MIGRATED** to ECS components:
  - `TransformComponent` - Position (replaces PointF Position)
  - `MovementComponent` - Velocity (replaces PointF Velocity)
  - Remaining: Damage, Active need dedicated components

## ECS Components Created

### Core Components
- **`TransformComponent`** - Position, Rotation, Scale (replaces legacy Entity base)
- **`RenderableComponent`** - Rendering data, visibility, layers

### Migrated Components
- **`HealthComponent`** - Health management (from Enemy.cs)
  - CurrentHealth, MaxHealth
  - Damage/heal events
  - Death/revive callbacks
- **`MovementComponent`** - Movement functionality (from Enemy.cs + Projectile.cs)
  - Velocity, Speed, Acceleration
  - Friction, MaxSpeed
  - Movement events

### Components Still Needed
- **`ScoreComponent`** - Score value, scoring events (from Enemy.cs)
- **`DamageComponent`** - Damage amount, damage type (from Projectile.cs)
- **`EnemyTypeComponent`** - Enemy classification (from Enemy.cs)
- **`ActiveComponent`** - Active state management (from Projectile.cs)

## Migration Strategy

### Phase 1: Foundation (✅ Complete)
- [x] Create ECS Entity, Component interfaces
- [x] Implement ECSWorld for entity management
- [x] Create core components (Transform, Renderable)
- [x] Migrate basic health and movement functionality

### Phase 2: Component Completion (🔄 In Progress)
- [ ] Create ScoreComponent for scoring
- [ ] Create DamageComponent for damage dealing
- [ ] Create EnemyTypeComponent for enemy classification
- [ ] Create ActiveComponent for state management

### Phase 3: System Integration (⏳ Pending)
- [ ] Create CombatSystem for damage dealing
- [ ] Create ScoringSystem for score management
- [ ] Create AISystem for enemy behavior
- [ ] Integrate with existing EnemySystem and ProjectileSystem

### Phase 4: Legacy Replacement (⏳ Pending)
- [ ] Replace Enemy.cs with ECS-based enemies
- [ ] Replace Projectile.cs with ECS-based projectiles
- [ ] Update EnemyManager to use ECSWorld
- [ ] Update ProjectileManager to use ECSWorld
- [ ] Resolve namespace conflicts (legacy Entity vs ECS Entity)

## Usage Examples

### Creating an ECS Enemy
```csharp
// Create entity
var enemy = ecsWorld.CreateEntity();

// Add components
enemy.AddComponent(new TransformComponent(new Vector2(100, 100)));
enemy.AddComponent(new HealthComponent(100f));
enemy.AddComponent(new MovementComponent(2.5f));
enemy.AddComponent(new RenderableComponent("enemy_sprite", true, 1));
enemy.AddComponent(new EnemyTypeComponent(EnemyType.Zombie));
enemy.AddComponent(new ScoreComponent(10));

// Configure
var health = enemy.GetComponent<HealthComponent>();
health.OnDeath += () => {
    DebugLogger.Log(LogSubsystems.ResourcesPipeline, "INFO", $"Enemy {enemy.Id} died!");
    // Handle enemy death (spawn loot, play sound, etc.)
};
```

### Creating an ECS Projectile
```csharp
// Create entity
var projectile = ecsWorld.CreateEntity();

// Add components
projectile.AddComponent(new TransformComponent(new Vector2(0, 0)));
projectile.AddComponent(new MovementComponent());
projectile.AddComponent(new RenderableComponent("bullet_sprite", true, 2));
projectile.AddComponent(new DamageComponent(25f));
projectile.AddComponent(new ActiveComponent(true));

// Configure movement
var movement = projectile.GetComponent<MovementComponent>();
movement.SetVelocity(new Vector2(10, 0)); // Move right at 10 units/sec
```

### Querying Entities
```csharp
// Get all enemies
var enemies = ecsWorld.GetEntitiesWith<EnemyTypeComponent, HealthComponent>();

// Get all active projectiles
var projectiles = ecsWorld.GetEntitiesWith<DamageComponent, ActiveComponent>()
    .Where(e => e.GetComponent<ActiveComponent>()!.IsActive);

// Get all entities that need rendering
var renderables = ecsWorld.GetEntitiesWith<TransformComponent, RenderableComponent>();
```

## Integration Points

### With Existing Systems
- **EnemySystem**: Can query ECS entities instead of managing Enemy objects
- **ProjectileSystem**: Can query ECS entities instead of managing Projectile objects
- **WaveSystem**: Can spawn ECS entities instead of legacy entities
- **CombatSystem**: Can use HealthComponent and DamageComponent for damage dealing

### With Rendering
- **RenderSystem**: Already implemented to render TransformComponent + RenderableComponent entities
- **Legacy Render**: Existing render methods can be gradually replaced

### With Events
- **HealthComponent**: Events for damage, healing, death, revive
- **MovementComponent**: Events for movement start/stop
- **Future**: Score events, collision events, etc.

## Benefits of Migration

### 1. Performance
- Cache-friendly component storage
- Efficient queries for specific component combinations
- Better memory locality

### 2. Flexibility
- Easy to add new component types
- Mix and match components freely
- No deep inheritance hierarchies

### 3. Maintainability
- Clear separation of concerns
- Component-based architecture
- Easier testing and debugging

### 4. Scalability
- Handles thousands of entities efficiently
- System-based processing
- Parallel processing potential

## Remaining Challenges

### 1. Namespace Conflicts
- Legacy `Engine.Entities.Entity` conflicts with ECS `Engine.ECS.Entity`
- **Solution**: Rename legacy namespace or fully qualify types

### 2. Gradual Migration
- Need to support both systems during transition
- **Solution**: Adapter pattern or dual-system approach

### 3. Existing Code Dependencies
- Many systems depend on legacy Entity classes
- **Solution**: Incremental refactoring with compatibility layers

### 4. Asset Integration
- Need to connect ECS with existing asset system
- **Solution**: AssetId in RenderableComponent

## Next Steps

1. **Complete Component Set**: Create remaining components (Score, Damage, EnemyType, Active)
2. **System Development**: Build combat, scoring, and AI systems
3. **Integration Testing**: Test ECS with existing game systems
4. **Performance Testing**: Verify ECS performance vs legacy system
5. **Legacy Phase-out**: Gradually replace legacy entity usage
6. **Documentation**: Update all system documentation

## Conclusion

The ECS foundation is solid and ready for expansion. The core components provide a good base for migrating existing entity functionality while maintaining performance and flexibility. The migration should be done incrementally to ensure stability and allow for thorough testing at each stage.
