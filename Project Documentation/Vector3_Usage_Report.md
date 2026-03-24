# Vector3 Usage Report - SAS Zombie Assault TD

## Summary
- **Total Vector3 References**: 1,033 occurrences across all C# files
- **Files with Vector3**: 89 files
- **Primary Definition**: `Engine\VectorMath\Vector3Types.cs` (Canonical Vector3 type)

## Top 20 Files by Vector3 Usage Count

| File | Usage Count |
|------|-------------|
| Engine\Physics\CollisionSystem.cs | High |
| Engine\Navigation\NavigationGrid.cs | High |
| Engine\Navigation\AStarPathfinder.cs | High |
| Engine\Rendering\ParticleSystem.cs | High |
| Engine\UI\HUD\HUDComponents.cs | High |
| Engine\ECS\Systems\CollisionSystem.cs | High |
| Engine\Physics\PhysicsSystem.cs | High |
| Engine\Rendering\RenderCommandQueue.cs | Medium |
| Engine\Rendering\Renderer.cs | Medium |
| Engine\UI\Systems\UIManager.cs | Medium |
| Engine\Towers\TowerPlacementPreview.cs | Medium |
| Engine\Projectiles\ProjectileSystem.cs | Medium |
| Engine\Enemies\Enemy.cs | Medium |
| Engine\Components\TransformComponent.cs | Medium |
| Engine\ECS\EntityManager.cs | Medium |
| Engine\Animation\AnimationController.cs | Medium |
| Engine\Audio\ModernAudioSubsystem.cs | Medium |
| Engine\Save\SAS\SASGameSave.cs | Medium |
| Engine\Rendering\SpriteBatch.cs | Medium |
| Engine\Physics\SpatialPartitionGrid.cs | Medium |

## Key Usage Patterns

### 1. **Using Directives**
```csharp
using Vector3 = SASZombieAssaultTD.Engine.VectorMath.Vector3;
using Vector3Int = SASZombieAssaultTD.Engine.VectorMath.Vector3Int;
using SASZombieAssaultTD.Engine.VectorMath;
```

### 2. **Common Operations**
- Position calculations
- Distance calculations (`Vector3.Distance`)
- Direction vectors (`Vector3.Forward`, `Vector3.Up`, etc.)
- Vector arithmetic (`+`, `-`, `*`, `/`)
- Normalization and magnitude

### 3. **Primary Namespaces**
- `SASZombieAssaultTD.Engine.VectorMath` (Canonical definition)
- `SASZombieAssaultTD.Engine.Navigation`
- `SASZombieAssaultTD.Engine.Physics`
- `SASZombieAssaultTD.Engine.Rendering`
- `SASZombieAssaultTD.Engine.UI`

## Definition Files

### Main Vector3 Definition
**File**: `Engine\VectorMath\Vector3Types.cs`
- **Purpose**: Canonical Vector3 type for the entire engine
- **Properties**: X, Y, Z (float)
- **Static Values**: Zero, One, UnitX, UnitY, UnitZ, Forward, Backward, Left, Right, Up, Down
- **Methods**: Distance, Dot, Cross, Lerp, Parse
- **Operators**: +, -, *, /, ==, !=

### Vector3Int Definition
**File**: `Engine\VectorMath\Vector3Int.cs`
- **Purpose**: Integer-based 3D vector for grid coordinates
- **Compatibility**: Provides both lowercase (x, y, z) and uppercase (X, Y, Z) properties
- **Constructors**: 2-argument (x, y) and 3-argument (x, y, z) versions

## Issues Identified

### 1. **Namespace Conflicts**
- Multiple files use alias directives to resolve conflicts
- Some files still reference old Vector3 implementations

### 2. **Constructor Mismatches**
- Some code expects 2-argument constructors
- Vector3Int now supports both 2D and 3D constructors

### 3. **Property Case Inconsistency**
- Vector3Int provides both case variants for compatibility
- Mixed usage of `.X` vs `.x` throughout codebase

## Recommendations

1. **Standardize on Vector3Types.cs** as the single authoritative source
2. **Remove conflicting Vector3 definitions** from other namespaces
3. **Update all using directives** to use the canonical Vector3
4. **Consider deprecating Vector3Int 2-argument constructor** in favor of explicit 3D usage

## Files Requiring Updates

### High Priority (Fix Immediately)
- Files with namespace conflicts
- Files using old Vector3 implementations
- Files with constructor mismatches

### Medium Priority (Clean Up)
- Standardize property access patterns
- Update using directives
- Remove redundant aliases

### Low Priority (Future)
- Performance optimizations
- Code documentation improvements
- Unit test coverage

---
*Report generated on: March 7, 2026*
*Total files analyzed: 89 C# files*
*Total Vector3 references: 1,033*
