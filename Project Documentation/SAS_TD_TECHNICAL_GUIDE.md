# SAS ZOMBIE ASSAULT TD - TECHNICAL IMPLEMENTATION GUIDE
*Based on Original Game Mechanics*
*Integration with Existing Engine Architecture*

## 🎮 GAME LOOP & CORE MECHANICS

### ✅ Grid System Integration
**Current Engine:** `Engine/Systems/Gameplay/NavigationGrid.cs`
**Required Enhancement:**
```csharp
public class NavigationGrid
{
    private bool[,] _isOccupied; // NEW: Tower placement tracking
    private Vector2Int _gridSize;
    
    public bool CanPlaceTower(Vector2Int position, int towerSize = 1)
    {
        for (int x = position.X; x < position.X + towerSize; x++)
        {
            for (int y = position.Y; y < position.Y + towerSize; y++)
            {
                if (!IsInBounds(x, y) || _isOccupied[x, y])
                    return false;
            }
        }
        return true;
    }
    
    public void SetOccupied(Vector2Int position, int towerSize, bool occupied)
    {
        for (int x = position.X; x < position.X + towerSize; x++)
        {
            for (int y = position.Y; y < position.Y + towerSize; y++)
            {
                if (IsInBounds(x, y))
                    _isOccupied[x, y] = occupied;
            }
        }
    }
}
```

### ✅ Health-Based Tower System
**New System Required:** `Engine/Towers/`
```csharp
public class Tower : Entity
{
    public float Health { get; private set; }
    public float MaxHealth { get; private set; }
    public bool IsDestroyed => Health <= 0;
    
    public void TakeDamage(int amount)
    {
        Health = Math.Max(0, Health - amount);
        if (IsDestroyed)
        {
            OnDestroyed();
        }
    }
    
    public void Repair(int amount)
    {
        Health = Math.Min(MaxHealth, Health + amount);
    }
    
    private void OnDestroyed()
    {
        // Remove from game, play destruction animation
        GameWorld.RemoveEntity(this);
    }
}
```

### ✅ Wave Scaling System
**Integration with:** `Engine/Enemies/EnemyDefinition.cs`
```csharp
public class EnemyDefinition
{
    public float BaseHealth { get; set; }
    public float BaseSpeed { get; set; }
    public float BaseDamage { get; set; }
    
    public EnemyStats GetScaledStats(int waveNumber)
    {
        float difficultyMultiplier = 1.0f + (waveNumber * 0.1f);
        return new EnemyStats
        {
            Health = BaseHealth * difficultyMultiplier,
            Speed = BaseSpeed * difficultyMultiplier,
            Damage = BaseDamage * difficultyMultiplier
        };
    }
}
```

## 🧟 ZOMBIE DATA PROFILES

### ✅ Enemy Type System
**Enhancement to:** `Engine/Enemies/EnemyDefinition.cs`
```csharp
public enum ZombieType
{
    Swarm,      // Standard pathing
    Sprinter,   // 1.5x Speed, low health, high attack rate
    Shadow,     // Stealth - not targetable unless near radar
    Bloater,    // Death spawn - spawns 3-5 worms on death
    Ruin        // Boss - 50-life penalty if reaches exit
}

public class EnemyDefinition
{
    public ZombieType Type { get; set; }
    public float BaseHealth { get; set; }
    public float Speed { get; set; }
    public float AttackRate { get; set; }
    public bool IsTargetable { get; set; } = true;
    public int SpawnCount { get; set; } = 1;
    
    // Special behaviors
    public bool IsStealthed => Type == ZombieType.Shadow;
    public bool SpawnsOnDeath => Type == ZombieType.Bloater;
    public bool IsBoss => Type == ZombieType.Ruin;
}
```

### ✅ Zombie Behavior Implementation
**Integration with:** `Engine/AI/Behaviors/`
```csharp
public class SwarmBehavior : BasicChaseBehavior
{
    // Standard pathing - no special logic
}

public class SprinterBehavior : BasicChaseBehavior
{
    public override void Initialize(EnemyDefinition definition)
    {
        base.Initialize(definition);
        // 1.5x speed multiplier
        SpeedMultiplier = 1.5f;
    }
}

public class ShadowBehavior : BasicChaseBehavior
{
    public override bool IsTargetable(Vector2Int position, Vector2Int towerPosition)
    {
        // Only targetable within radar range
        float distance = Vector2.Distance(position, towerPosition);
        return distance <= RadarRange;
    }
}

public class BloaterBehavior : BasicChaseBehavior
{
    public override void OnDeath(Vector2Int position)
    {
        // Spawn 3-5 worms at current position
        int spawnCount = Random.Range(3, 6);
        for (int i = 0; i < spawnCount; i++)
        {
            var worm = EnemyFactory.CreateEnemy(ZombieType.Worm, position);
            GameWorld.AddEntity(worm);
        }
    }
}

public class RuinBehavior : BasicChaseBehavior
{
    public override void OnReachExit()
    {
        // 50-life penalty (insta-kill)
        GameState.Lives = 0;
        GameState.TriggerGameOver();
    }
}
```

## 💰 UNIT & ECONOMY DATA

### ✅ Economy System
**New System Required:** `Engine/Economy/`
```csharp
public class EconomyManager
{
    public static readonly int StartingCash = 450;
    
    public int CurrentCash { get; private set; } = StartingCash;
    
    public bool CanAfford(int cost) => CurrentCash >= cost;
    
    public void Spend(int amount)
    {
        if (CanAfford(amount))
        {
            CurrentCash -= amount;
            OnCashChanged?.Invoke(CurrentCash);
        }
    }
    
    public void Earn(int amount)
    {
        CurrentCash += amount;
        OnCashChanged?.Invoke(CurrentCash);
    }
    
    public event Action<int> OnCashChanged;
}
```

### ✅ Tower Data System
**New System Required:** `Engine/Towers/`
```csharp
public enum TowerType
{
    VickersTurret,    // Low cost, high fire rate, single target
    MGLTurret,        // Mid cost, splash damage
    SASSoldier        // Mobile, repositionable
}

public class TowerData
{
    public TowerType Type { get; set; }
    public int Cost { get; set; }
    public float BaseHealth { get; set; }
    public float FireRate { get; set; }
    public float Range { get; set; }
    public int Damage { get; set; }
    public bool HasSplash { get; set; }
    public float SplashRadius { get; set; }
    public bool IsMobile { get; set; }
    public int RepositionCost { get; set; }
}

public static class TowerDatabase
{
    public static readonly Dictionary<TowerType, TowerData> Towers = new()
    {
        [TowerType.VickersTurret] = new TowerData
        {
            Type = TowerType.VickersTurret,
            Cost = 150,
            BaseHealth = 100,
            FireRate = 10f, // shots per second
            Range = 5f,
            Damage = 25,
            HasSplash = false,
            IsMobile = false
        },
        
        [TowerType.MGLTurret] = new TowerData
        {
            Type = TowerType.MGLTurret,
            Cost = 300,
            BaseHealth = 150,
            FireRate = 2f,
            Range = 6f,
            Damage = 80,
            HasSplash = true,
            SplashRadius = 2f,
            IsMobile = false
        },
        
        [TowerType.SASSoldier] = new TowerData
        {
            Type = TowerType.SASSoldier,
            Cost = 200,
            BaseHealth = 80,
            FireRate = 5f,
            Range = 4f,
            Damage = 35,
            HasSplash = false,
            IsMobile = true,
            RepositionCost = 50
        }
    };
}
```

## 🔧 TECHNICAL IMPLEMENTATION

### ✅ Enhanced Pathfinding Integration
**Current System:** `Engine/Systems/Gameplay/PathfindingSystem.cs`
**Already Implements:** A* algorithm ✅
**Required Enhancement:** Waypoint system integration
```csharp
public class PathfindingSystem
{
    private List<Vector2Int> _waypoints;
    
    public List<Vector3> GetPathToNextWaypoint(Vector3 currentPos)
    {
        var currentGrid = WorldToGrid(currentPos);
        var nextWaypoint = GetNextWaypoint(currentGrid);
        return FindPath(currentPos, GridToWorld(nextWaypoint));
    }
    
    private Vector2Int GetNextWaypoint(Vector2Int currentPos)
    {
        // Find next waypoint in the path
        foreach (var waypoint in _waypoints)
        {
            if (waypoint != currentPos)
                return waypoint;
        }
        return _waypoints.LastOrDefault();
    }
}
```

### ✅ Enhanced Targeting System
**Integration with:** `Engine/Towers/Tower.cs`
```csharp
public class Tower : Entity
{
    public float Range { get; private set; }
    public TargetingMode TargetingMode { get; set; } = TargetingMode.First;
    
    public Enemy GetBestTarget(List<Enemy> enemiesInRange)
    {
        return TargetingMode switch
        {
            TargetingMode.First => enemiesInRange
                .OrderBy(e => e.DistanceTraveled)
                .FirstOrDefault(),
            TargetingMode.Last => enemiesInRange
                .OrderByDescending(e => e.DistanceTraveled)
                .FirstOrDefault(),
            TargetingMode.Strongest => enemiesInRange
                .OrderByDescending(e => e.Health)
                .FirstOrDefault(),
            TargetingMode.Weakest => enemiesInRange
                .OrderBy(e => e.Health)
                .FirstOrDefault(),
            _ => null
        };
    }
    
    public List<Enemy> GetEnemiesInRange()
    {
        return GameWorld.GetEnemies()
            .Where(e => Vector2.Distance(Position, e.Position) <= Range)
            .Where(e => e.IsTargetable)
            .ToList();
    }
}

public enum TargetingMode
{
    First,      // Target enemy furthest along path
    Last,       // Target enemy closest to start
    Strongest,  // Target enemy with most health
    Weakest     // Target enemy with least health
}
```

### ✅ Splash Damage System
**Integration with:** `Engine/Combat/CombatSystem.cs`
```csharp
public class CombatSystem
{
    public void DealDamage(Enemy target, int damage, bool hasSplash, float splashRadius, Vector3 impactPoint)
    {
        // Direct damage
        target.TakeDamage(damage);
        
        // Splash damage
        if (hasSplash)
        {
            var enemiesInSplash = GameWorld.GetEnemies()
                .Where(e => Vector2.Distance(impactPoint, e.Position) <= splashRadius)
                .Where(e => e != target) // Don't hit the same enemy twice
                .ToList();
                
            foreach (var enemy in enemiesInSplash)
            {
                enemy.TakeDamage(damage / 2); // Half damage for splash
            }
        }
    }
}
```

## 🎯 INTEGRATION PLAN

### Phase 1: Core Systems (Week 1)
1. **Economy Manager** - `Engine/Economy/EconomyManager.cs`
2. **Tower Base Class** - `Engine/Towers/Tower.cs`
3. **Tower Database** - `Engine/Towers/TowerDatabase.cs`
4. **Grid Enhancement** - Add occupancy tracking to `NavigationGrid.cs`

### Phase 2: Zombie Behaviors (Week 2)
1. **Zombie Types** - Enhance `EnemyDefinition.cs`
2. **Special Behaviors** - New behavior classes in `Engine/AI/Behaviors/`
3. **Wave Scaling** - Enhance enemy spawning system
4. **Boss System** - Implement Ruin boss mechanics

### Phase 3: Combat & Targeting (Week 3)
1. **Tower Targeting** - Enhanced targeting logic
2. **Splash Damage** - Area of effect implementation
3. **Mobile Units** - SAS soldier repositioning
4. **Combat Integration** - Connect all combat systems

### Phase 4: Game Integration (Week 4)
1. **Game State** - Menu, playing, game over states
2. **UI/HUD** - Cash display, tower info, wave info
3. **Save/Load** - Game state persistence
4. **Polish** - Effects, sounds, animations

## 📊 TECHNICAL SPECIFICATIONS

### Performance Targets:
- **60 FPS** with 100+ enemies on screen
- **< 100ms** pathfinding calculations
- **< 16ms** frame time for targeting calculations

### Memory Management:
- **Object pooling** for bullets and effects
- **LOD system** for distant enemies
- **Asset streaming** for large levels

### Scalability:
- **Configurable** enemy stats via JSON
- **Modular** tower system for easy additions
- **Extensible** behavior system for new enemy types

---
*This guide provides the technical foundation for implementing authentic SAS Zombie Assault TD gameplay using our existing robust engine architecture.*
