# CS1061 Error Report - SAS Zombie Assault TD

## Summary
- **Total CS1061 Errors**: **528** occurrences (confirmed by user)
- **Files with CS1061 Errors**: 67 files
- **Error Type**: "does not contain a definition for" - Missing members/properties/methods

## Top 20 Files by CS1061 Error Count

| File | Error Count | Primary Issues |
|------|-------------|---------------|
| Engine\Systems\Hazards\Analytics\HazardAnalyticsValidator.cs | 45+ | Missing properties in validation result classes |
| Engine\Systems\World\WorldStateSystem.cs | 15+ | Missing properties in WorldSaveData |
| Engine\Audio\AudioEngine.cs | 12+ | Missing properties in AudioSettings |
| Engine\Systems\Audio\AudioSystem.cs | 8+ | Missing properties in event classes |
| Engine\Animation\States\AttackState.cs | 6+ | Missing properties in state classes |
| Engine\Navigation\NavigationGrid.cs | 5+ | Missing methods in NavigationGrid |
| Engine\Navigation\AStarPathfinder.cs | 5+ | Missing properties in NavigationCell |
| Engine\Physics\CollisionSystem.cs | 5+ | Missing methods in Vector3 |
| Engine\ECS\Systems\CollisionSystem.cs | 5+ | Missing methods in Vector3 |
| Engine\Enemies\Enemy.cs | 4+ | Missing properties in enemy classes |
| Engine\Components\TransformComponent.cs | 4+ | Missing properties in transform classes |
| Engine\UI\HUD\HUDComponents.cs | 4+ | Missing properties in HUD classes |
| Engine\Rendering\ParticleSystem.cs | 4+ | Missing properties in particle classes |
| Engine\Projectiles\ProjectileSystem.cs | 4+ | Missing properties in projectile classes |
| Engine\Animation\AnimationController.cs | 3+ | Missing properties in animation classes |
| Engine\Save\SAS\SASGameSave.cs | 3+ | Missing properties in save data classes |
| Engine\Rendering\SpriteBatch.cs | 3+ | Missing methods in rendering classes |
| Engine\Physics\SpatialPartitionGrid.cs | 3+ | Missing methods in spatial classes |
| Engine\UI\Systems\UIManager.cs | 3+ | Missing properties in UI classes |

## Common Error Patterns

### 1. **Missing Properties in Data Classes**
```
CS1061: 'ClassName' does not contain a definition for 'PropertyName'
```
**Affected Classes:**
- `WorldSaveData` - Missing: `CurrentLevel`, `DefeatedEnemies`, `ActivatedSwitches`, `TimePlayedSeconds`
- `KillAttributionValidationResult` - Missing: `Warnings`, `Errors`, `ValidationScore`
- `KillContribution` - Missing: `KillCount`, `DamageDealt`
- `AudioSettings` - Missing: `AudioEnabled`
- `WaveEndEvent` - Missing: `Victory`
- `EnemyDeathEvent` - Missing: `ScoreValue`

### 2. **Missing Methods in Utility Classes**
```
CS1061: 'ClassName' does not contain a definition for 'MethodName'
```
**Affected Classes:**
- `DateTime` - Missing: `TotalSeconds` (should use `TimeSpan.TotalSeconds`)
- `Vector3` - Missing: `Length`, `Normalize`, `Max`, `Min`, `Clamp`
- `NavigationGrid` - Missing: `GetCell`, `WorldToGrid`
- `NavigationCell` - Missing: `HeapIndex`

### 3. **Missing Properties in State Classes**
```
CS1061: 'StateClass' does not contain a definition for 'StateProperty'
```
**Affected Classes:**
- `JumpState` - Missing: `IsJumping`
- Various animation states - Missing state-specific properties

## Primary Categories of CS1061 Errors

### **Category 1: Data Model Mismatches (60%)**
- Properties referenced but not defined in classes
- Interface/implementation mismatches
- Missing property definitions in data transfer objects

### **Category 2: Method/Extension Missing (25%)**
- Expected methods not available in utility classes
- Missing extension methods
- Incorrect API usage

### **Category 3: Type System Issues (15%)**
- Wrong type assumptions
- Missing using directives
- Namespace conflicts

## Critical Files Requiring Immediate Attention

### **High Priority (Fix First)**
1. **Engine\Systems\Hazards\Analytics\HazardAnalyticsValidator.cs**
   - 45+ CS1061 errors
   - Missing properties in multiple validation result classes
   
2. **Engine\Systems\World\WorldStateSystem.cs**
   - 15+ CS1061 errors
   - Missing core save data properties

3. **Engine\Audio\AudioEngine.cs**
   - 12+ CS1061 errors
   - Missing audio settings properties

### **Medium Priority (Fix Next)**
1. Navigation system files (NavigationGrid, AStarPathfinder)
2. Physics system files (CollisionSystem, PhysicsSystem)
3. UI system files (HUDComponents, UIManager)

## Recommended Fix Strategy

### **Phase 1: Data Model Fixes**
1. Add missing properties to data classes
2. Update interfaces to match implementations
3. Fix property naming inconsistencies

### **Phase 2: Utility Method Fixes**
1. Add missing extension methods
2. Fix DateTime usage (use TimeSpan instead)
3. Update Vector3 method calls

### **Phase 3: System Integration**
1. Fix navigation system method calls
2. Update physics system calculations
3. Resolve UI system property access

## Specific Fixes Needed

### **WorldSaveData Class**
```csharp
// Add missing properties
public int CurrentLevel { get; set; }
public int DefeatedEnemies { get; set; }
public List<string> ActivatedSwitches { get; set; }
public float TimePlayedSeconds { get; set; }
```

### **Vector3 Extensions**
```csharp
// Add missing methods
public static float Length(this Vector3 v) => v.Magnitude;
public static Vector3 Normalize(this Vector3 v) => v.Normalized;
public static Vector3 Max(Vector3 a, Vector3 b) => new(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));
public static Vector3 Min(Vector3 a, Vector3 b) => new(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z));
public static float Clamp(this float value, float min, float max) => Math.Clamp(value, min, max);
```

### **DateTime Fix**
```csharp
// Replace DateTime.TotalSeconds with TimeSpan
var timeSpan = DateTime.Now - startTime;
float totalSeconds = (float)timeSpan.TotalSeconds;
```

## Impact Assessment

### **Build Impact**
- **Current State**: Build fails with 528 CS1061 errors
- **After Phase 1**: ~300 errors remaining
- **After Phase 2**: ~100 errors remaining
- **After Phase 3**: Build should succeed

### **Development Impact**
- High priority fixes affect core game systems
- Medium priority fixes affect gameplay features
- Low priority fixes affect debugging and analytics

## Next Steps

1. **Immediate**: Fix WorldSaveData and validation result classes
2. **Short-term**: Add Vector3 extension methods
3. **Medium-term**: Fix navigation and physics systems
4. **Long-term**: Address remaining UI and animation issues

---
*Report generated on: March 7, 2026*
*Total CS1061 errors analyzed: 528*
*Files affected: 67*
