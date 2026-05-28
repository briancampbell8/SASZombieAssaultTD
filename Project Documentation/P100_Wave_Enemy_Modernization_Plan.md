# P100 Wave and Enemy System Modernization - Implementation Plan

**Date:** May 24, 2026
**Status:** 🔄 **IN PROGRESS**
**Objective:** Modernize wave and enemy systems with deterministic timing, difficulty scaling, champion properties, and audio integration.

---

## Executive Summary

P100 focuses on behavioral modernization of the wave and enemy systems, building upon the completed P90 audio subsystem. The modernization addresses legacy TODOs, implements proper difficulty scaling, adds champion properties, and integrates with the new audio system.

**Current Status:** Research complete, architecture design in progress.

---

## TODO Audit Results

### WaveSpawnGroup.cs TODOs
1. **Line 162:** `// TODO: Implement proper difficulty system` - Hardcoded to "Normal"
2. **Line 559:** `// TODO: Add ChampionLevel property to Enemy class` - Commented out code
3. **Line 568:** `// TODO: implement champion visuals` - Commented method call

### SpawnPattern.cs TODOs
1. **Line 739:** `// TODO: Fix type mismatch - Bounds is Rect?, can't use ?? with Vector3` - Type mismatch

### Enemy.cs Analysis
- Has `IsChampion` property but lacks `ChampionLevel` property
- Has comprehensive behavior modifier system
- Has visual effects system
- Has custom properties system

### WaveDirector.cs Analysis
- Has event system for wave lifecycle
- Has async wave spawning (placeholder implementation)
- Has progress tracking
- Needs integration with WaveAudioIntegration

---

## Modernization Architecture

### Phase 1: Difficulty Scaling System

**File:** `Engine/Waves/DifficultyScaling.cs` (NEW)

**Features:**
- Difficulty levels: Easy, Normal, Hard, Elite, Nightmare
- Wave-based difficulty progression
- Enemy stat multipliers per difficulty
- Champion spawn rates per difficulty
- Configurable difficulty curves

**Properties:**
```csharp
public class DifficultyScaling
{
    public enum DifficultyLevel { Easy, Normal, Hard, Elite, Nightmare }
    
    public float HealthMultiplier { get; }
    public float SpeedMultiplier { get; }
    public float DamageMultiplier { get; }
    public float CountMultiplier { get; }
    public float ChampionSpawnRate { get; }
    
    public static DifficultyScaling GetForWave(int waveNumber, DifficultyLevel baseLevel);
    public static DifficultyScaling GetCurrent();
}
```

---

### Phase 2: Champion Properties System

**File:** `Engine/Enemies/ChampionProperties.cs` (NEW)

**Features:**
- Champion level system (1-10)
- Champion stat bonuses per level
- Champion visual effects
- Champion special abilities
- Champion rarity tiers

**Properties:**
```csharp
public class ChampionProperties
{
    public int Level { get; }
    public float HealthBonus { get; }
    public float DamageBonus { get; }
    public float SpeedBonus { get; }
    public List<string> SpecialAbilities { get; }
    public ChampionVisuals Visuals { get; }
    
    public static ChampionProperties Generate(int level);
    public void ApplyTo(Enemy enemy);
}
```

**Enemy.cs Addition:**
```csharp
public int ChampionLevel { get; set; } = 0;
public ChampionProperties ChampionData { get; set; }
```

---

### Phase 3: SpawnPattern Type Fix

**File:** `Engine/Waves/SpawnPattern.cs` (MODIFY)

**Fix:**
- Change `Bounds` parameter from `Rectangle?` to `Vector3?`
- Or add separate `BoundsVector3` parameter
- Update RandomSpawnStrategy to use correct type

---

### Phase 4: Wave Audio Integration

**File:** `Engine/Waves/WaveAudioIntegration.cs` (ALREADY EXISTS - ENHANCE)

**Enhancements:**
- Integrate with WaveDirector events
- Add wave start audio callback
- Add wave complete audio callback
- Add enemy spawn audio callback
- Add wave music management

**Integration Points:**
```csharp
WaveDirector.Instance.OnWaveStarted += (waveNum) => 
    WaveAudioIntegration.PlayWaveStart();
    
WaveDirector.Instance.OnWaveCompleted += (waveNum) => 
    WaveAudioIntegration.PlayWaveComplete();
    
WaveDirector.Instance.OnEnemySpawned += (enemy) => 
    EnemyAudioIntegration.PlayEnemySpawn(enemy.Position);
```

---

### Phase 5: Champion Visuals System

**File:** `Engine/Enemies/ChampionVisuals.cs` (NEW)

**Features:**
- Champion aura effects
- Champion color tinting
- Champion particle effects
- Champion scale modifications
- Champion glow effects

**Properties:**
```csharp
public class ChampionVisuals
{
    public Color AuraColor { get; }
    public float AuraIntensity { get; }
    public float ScaleMultiplier { get; }
    public List<ParticleEffect> Particles { get; }
    
    public void Apply(Enemy enemy);
    public void Update(float deltaTime);
}
```

---

## Implementation Sequence

### Step 1: Create Difficulty Scaling System
- Create `DifficultyScaling.cs`
- Implement difficulty level enums
- Implement wave-based progression
- Implement stat multipliers
- Replace hardcoded "Normal" in WaveSpawnGroup.cs

### Step 2: Add ChampionLevel to Enemy
- Add `ChampionLevel` property to Enemy.cs
- Add `ChampionData` property to Enemy.cs
- Update WaveSpawnGroup.cs to use ChampionLevel
- Uncomment champion level assignment

### Step 3: Create Champion Properties System
- Create `ChampionProperties.cs`
- Implement champion level generation
- Implement stat bonuses per level
- Implement champion visual effects
- Update WaveSpawnGroup.cs champion logic

### Step 4: Fix SpawnPattern Type Mismatch
- Fix Bounds parameter type in SpawnPatternParameterss
- Update RandomSpawnStrategy
- Test spawn pattern generation

### Step 5: Integrate Wave Audio
- Enhance WaveAudioIntegration.cs
- Add WaveDirector event hooks
- Add wave music management
- Test audio synchronization

### Step 6: Create Champion Visuals System
- Create `ChampionVisuals.cs`
- Implement aura effects
- Implement particle effects
- Apply visuals to champion enemies
- Uncomment champion visuals call in WaveSpawnGroup.cs

### Step 7: Validate Synchronization
- Test wave progression
- Test HUD updates
- Test audio cues
- Test champion spawning
- Test difficulty scaling

---

## Files to Create

1. `Engine/Waves/DifficultyScaling.cs` - Difficulty scaling system
2. `Engine/Enemies/ChampionProperties.cs` - Champion properties system
3. `Engine/Enemies/ChampionVisuals.cs` - Champion visual effects
4. `Project Documentation/P100_Wave_Enemy_Modernization_Summary.md` - Final summary

---

## Files to Modify

1. `Engine/Waves/WaveSpawnGroup.cs` - Replace difficulty TODO, uncomment champion code
2. `Engine/Waves/SpawnPattern.cs` - Fix type mismatch
3. `Engine/Enemies/Enemy.cs` - Add ChampionLevel property
4. `Engine/Waves/WaveDirector.cs` - Add audio integration hooks
5. `Engine/Waves/WaveAudioIntegration.cs` - Enhance with director integration

---

## Integration Points

### WaveDirector Events
- `OnWaveStarted` → WaveAudioIntegration.PlayWaveStart()
- `OnWaveCompleted` → WaveAudioIntegration.PlayWaveComplete()
- `OnEnemySpawned` → EnemyAudioIntegration.PlayEnemySpawn()
- `OnAllWavesCompleted` → WaveAudioIntegration.PlayWaveMusic("music_victory")

### HUD Integration
- Wave progress updates
- Champion spawn notifications
- Difficulty level display

### Audio Integration
- Wave start/complete sounds
- Enemy spawn/death sounds
- Champion spawn sounds
- Wave music transitions

---

## Success Criteria

- ✅ Difficulty system fully implemented and configurable
- ✅ ChampionLevel property added to Enemy class
- ✅ Champion properties system with visual effects
- ✅ SpawnPattern type mismatch resolved
- ✅ Wave audio integration complete
- ✅ Wave/HUD/audio synchronization validated
- ✅ All TODOs in wave/enemy systems resolved
- ✅ Documentation complete

---

## Next Steps

1. Implement DifficultyScaling.cs
2. Add ChampionLevel to Enemy.cs
3. Create ChampionProperties.cs
4. Fix SpawnPattern type mismatch
5. Enhance WaveAudioIntegration
6. Create ChampionVisuals.cs
7. Update WaveSpawnGroup.cs
8. Validate integration
9. Document completion

---

*This modernization embodies the principle: "Fix legacy TODOs, implement missing systems, integrate with modern infrastructure."*
