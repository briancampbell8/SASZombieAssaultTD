# Code Size Assessment Report

## Top 20 Largest Files (by line count)

| Rank | File | Lines | Category |
|------|------|-------|----------|
| 1 | RSManager.cs | 1,212 | Resources |
| 2 | PlayerProgression.cs | 1,124 | Player |
| 3 | SaveLoadManager.cs | 1,022 | Player |
| 4 | SpawnPattern.cs | 991 | Waves |
| 5 | PlayerSystem.cs | 938 | Player |
| 6 | AssetPipeline.cs | 912 | Resources |
| 7 | ModernUIRenderer.cs | 907 | UI Rendering |
| 8 | PlayerActions.cs | 896 | Player |
| 9 | AssetManager.cs | 889 | Resources |
| 10 | LevelProgression.cs | 884 | LevelUp |
| 11 | ModernUIComponents.cs | 875 | UI Components |
| 12 | PlayerDataTypes.cs | 873 | Player |
| 13 | AssetSystemIntegration.cs | 827 | Resources |
| 14 | SASGameSave.cs | 806 | Save |
| 15 | AssetBundle.cs | 803 | Resources |
| 16 | WaveLoader.cs | 792 | Waves |
| 17 | ModernUIAssetLoader.cs | 770 | UI Assets |
| 18 | LevelUpController.cs | 770 | LevelUp |
| 19 | MathExtensions.cs | 741 | Core Math |
| 20 | WaveDirector.cs | 741 | Waves |

---

## Files Requiring Refactoring

### 🔴 **CRITICAL (>1000 lines)**

#### 1. **RSManager.cs** (1,212 lines)
**Issues:**
- Single monolithic class handling resource loading, caching, validation, analytics, and priority management
- Contains nested classes: `ResourceRequest`, `ResourceValidationResult`, `ResourceAnalytics`
- Mixes concerns: asset management, texture caching, validation, analytics

**Recommendation:**
```
Split into:
├── RSManager.cs (~300 lines) - Core orchestration
├── ResourceCache.cs (~200 lines) - Caching logic
├── ResourceValidator.cs (~200 lines) - Validation
├── ResourceAnalytics.cs (~150 lines) - Analytics
├── ResourceLoader.cs (~200 lines) - Loading logic
└── RSManager.Types.cs (~150 lines) - Nested types
```

#### 2. **PlayerProgression.cs** (1,124 lines)
**Issues:**
- Single class handling experience, levels, tower unlocks, achievements, events
- Heavy XML documentation (lines 1-78)
- Mixes progression logic with event management

**Recommendation:**
```
Split into:
├── PlayerProgression.cs (~400 lines) - Core progression
├── ExperienceCalculator.cs (~200 lines) - XP logic
├── TowerUnlockManager.cs (~200 lines) - Unlock logic
├── ProgressionEvents.cs (~150 lines) - Event handling
└── ProgressionData.cs (~175 lines) - Data structures
```

#### 3. **SaveLoadManager.cs** (1,022 lines)
**Issues:**
- Static class handling serialization, file I/O, validation, versioning
- Heavy XML documentation (lines 1-99)
- Mixes save/load logic with validation and error handling

**Recommendation:**
```
Split into:
├── SaveLoadManager.cs (~300 lines) - Core save/load
├── SaveSerializer.cs (~250 lines) - JSON serialization
├── SaveValidator.cs (~200 lines) - Validation
├── SaveFileManager.cs (~200 lines) - File operations
└── SaveDataMigration.cs (~150 lines) - Version migration
```

---

### 🟡 **HIGH PRIORITY (800-1000 lines)**

#### 4. **SpawnPattern.cs** (991 lines)
**Issues:**
- Already has good structure with strategy pattern
- Contains 13 strategy classes in one file
- Base class and interface mixed with implementations

**Recommendation:**
```
Split into:
├── SpawnPattern.cs (~100 lines) - Main class + interface
├── Strategies/BaseSpawnStrategy.cs (~50 lines) - Base class
├── Strategies/SingleSpawnStrategy.cs (~50 lines)
├── Strategies/LineSpawnStrategy.cs (~50 lines)
├── Strategies/ClusterSpawnStrategy.cs (~50 lines)
├── Strategies/SpreadSpawnStrategy.cs (~50 lines)
├── Strategies/WaveSpawnStrategy.cs (~50 lines)
├── Strategies/CircleSpawnStrategy.cs (~50 lines)
├── Strategies/RandomSpawnStrategy.cs (~50 lines)
├── Strategies/FlankingSpawnStrategy.cs (~50 lines)
├── Strategies/PincerSpawnStrategy.cs (~50 lines)
├── Strategies/SpiralSpawnStrategy.cs (~50 lines)
├── Strategies/GridSpawnStrategy.cs (~50 lines)
└── Strategies/VFormationSpawnStrategy.cs (~50 lines)
```

#### 5. **PlayerSystem.cs** (938 lines)
**Issues:**
- Central player management system
- Likely mixes multiple concerns

**Recommendation:**
```
Split into:
├── PlayerSystem.cs (~300 lines) - Core orchestration
├── PlayerStateManager.cs (~250 lines) - State management
├── PlayerInventoryManager.cs (~200 lines) - Inventory
└── PlayerStatsManager.cs (~200 lines) - Statistics
```

#### 6. **AssetPipeline.cs** (912 lines)
**Issues:**
- Asset processing pipeline
- Likely contains multiple pipeline stages

**Recommendation:**
```
Split into:
├── AssetPipeline.cs (~200 lines) - Core pipeline
├── AssetProcessor.cs (~250 lines) - Processing logic
├── AssetOptimizer.cs (~200 lines) - Optimization
└── AssetValidator.cs (~250 lines) - Validation
```

#### 7. **ModernUIRenderer.cs** (907 lines)
**Issues:**
- UI rendering system
- Likely handles multiple UI element types

**Recommendation:**
```
Split into:
├── ModernUIRenderer.cs (~300 lines) - Core renderer
├── UIButtonRenderer.cs (~200 lines) - Button rendering
├── UIPanelRenderer.cs (~200 lines) - Panel rendering
└── UITextRenderer.cs (~200 lines) - Text rendering
```

#### 8. **PlayerActions.cs** (896 lines)
**Issues:**
- Player action handling
- Likely contains many action types

**Recommendation:**
```
Split into:
├── PlayerActions.cs (~200 lines) - Core action system
├── CombatActions.cs (~250 lines) - Combat actions
├── MovementActions.cs (~200 lines) - Movement actions
└── InteractionActions.cs (~250 lines) - Interaction actions
```

---

### 🟢 **MEDIUM PRIORITY (700-800 lines)**

#### 9. **AssetManager.cs** (889 lines)
**Recommendation:** Split into AssetManager.cs + AssetLoader.cs + AssetCache.cs

#### 10. **LevelProgression.cs** (884 lines)
**Recommendation:** Split into LevelProgression.cs + LevelCalculator.cs + LevelRewards.cs

#### 11. **ModernUIComponents.cs** (875 lines)
**Recommendation:** Already has multiple classes - extract to separate files:
- ModernButton.cs
- ComponentStyle.cs
- ComponentAnimationController.cs

#### 12. **PlayerDataTypes.cs** (873 lines)
**Recommendation:** Already has multiple classes - extract to separate files:
- Transaction.cs
- ActionResult.cs
- LevelUp.cs
- PlayerData.cs

---

## General Refactoring Guidelines

### When to Split a File:
- **>500 lines** - Consider splitting
- **>750 lines** - Should split
- **>1000 lines** - Must split immediately

### Splitting Strategies:
1. **By Concern** - Separate different responsibilities
2. **By Feature** - Group related functionality
3. **By Abstraction** - Extract interfaces/base classes
4. **By Data** - Separate data structures from logic

### Benefits of Smaller Files:
- **Easier Navigation** - Faster to find code
- **Better Compilation** - Incremental builds faster
- **Easier Testing** - Smaller units to test
- **Reduced Merge Conflicts** - Smaller change sets
- **Better Code Review** - Focused reviews

### What NOT to Break:
- Keep related classes together if they form a cohesive unit
- Don't split just for the sake of it
- Maintain logical grouping
- Consider the team's workflow

---

## Recommended Action Plan

### Phase 1 (Immediate - Critical Files):
1. Split RSManager.cs (6 files)
2. Split PlayerProgression.cs (5 files)
3. Split SaveLoadManager.cs (5 files)

### Phase 2 (High Priority):
4. Split SpawnPattern.cs (14 files)
5. Split PlayerSystem.cs (4 files)
6. Split AssetPipeline.cs (4 files)
7. Split ModernUIRenderer.cs (4 files)
8. Split PlayerActions.cs (4 files)

### Phase 3 (Medium Priority):
9-12. Split remaining 700-800 line files
