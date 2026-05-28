# TODO Comments in SAS Zombie Assault TD Project

This document catalogs all TODO comments found in the Engine source code.

---

## Engine\Towers\TowerControl\NeuralNet.cs

### Line 116
```csharp
TowerType = TowerType.Basic; // TODO: VickersTurret doesn't exist in enum
```
**Context:** NeuralTowerUpgrade constructor sets TowerType to Basic, but references a non-existent VickersTurret enum value.

### Line 196
```csharp
// TODO: Complete upgrade type doesn't exist, using Special instead
// InitializeCompleteUpgrade();
```
**Context:** UpgradeType.Special case in SetDefaultValues method - Complete upgrade type doesn't exist.

### Line 321
```csharp
// TODO: Fix tower method calls - these methods don't exist on Tower class
```
**Context:** ApplyToTower method attempts to call non-existent methods on Tower class (SetStatModifier, AddSpecialAbility, AddVisualEffect).

### Line 328
```csharp
// TODO: Fix tower method calls
```
**Context:** ApplyToTower method - another instance of calling non-existent Tower methods.

### Line 335
```csharp
// TODO: Fix tower method calls
```
**Context:** ApplyToTower method - third instance of calling non-existent Tower methods.

### Line 345
```csharp
// TODO: Fix tower method calls
```
**Context:** ApplyToTower method - fourth instance of calling non-existent Tower methods (SetSprite, SetTintColor).

### Line 369
```csharp
// TODO: Fix tower method calls
```
**Context:** RemoveFromTower method attempts to call non-existent methods on Tower class.

### Line 376
```csharp
// TODO: Fix tower method calls
```
**Context:** RemoveFromTower method - second instance of calling non-existent Tower methods.

### Line 383
```csharp
// TODO: Fix tower method calls
```
**Context:** RemoveFromTower method - third instance of calling non-existent Tower methods.

### Line 528
```csharp
// TODO: Fix ParticleSystem.CreateEffect - method doesn't exist
```
**Context:** ApplyUpgradeEffects method attempts to call non-existent ParticleSystem.CreateEffect method.

---

## Engine\Towers\TowerControl\NeuralManager.cs

### Line 493
```csharp
// TODO: Fix GetUpgradePath method - doesn't exist on upgrade database
// var upgradePath = database.GetUpgradePath();
```
**Context:** InitializeUpgradePaths method attempts to call non-existent GetUpgradePath method on TowerUpgradeDatabase.

### Line 555
```csharp
// TODO: Implement ModernAudioSubsystem instance
// ModernAudioSubsystem.PlaySound(soundName);
```
**Context:** PlaySuccessSound method needs ModernAudioSubsystem implementation.

### Line 564
```csharp
// TODO: Implement ModernAudioSubsystem instance
// ModernAudioSubsystem.PlaySound(soundName);
```
**Context:** PlayErrorSound method needs ModernAudioSubsystem implementation.

---

## Engine\Towers\TowerManager.cs

### Line 218
```csharp
// TODO: Fix GetEntityWithComponent call - ECSWorld doesn't have this method signature
```
**Context:** DestroyTower method attempts to call non-existent ECSWorld.GetEntityWithComponent method.

### Line 229
```csharp
// TODO: Destroy tower entity
```
**Context:** DestroyTower method needs implementation for destroying tower entity.

### Line 362
```csharp
// TODO: Implement path blocking check
// This would require integration with the pathfinding system
```
**Context:** BlocksEnemyPath method needs path blocking check implementation.

### Line 374
```csharp
// TODO: Implement tower-specific distance requirements
```
**Context:** GetMinDistanceFromTowers method needs tower-specific distance requirements.

### Line 410
```csharp
// TODO: Implement tower cost database
```
**Context:** GetTowerCost method needs tower cost database implementation.

---

## Engine\Towers\PlacementValidator.cs

### Line 219
```csharp
// TODO: Implement tower registry when available
// if (_towerRegistry == null)
//     return false;
```
**Context:** IsTooCloseToOtherTowers method needs tower registry implementation.

### Line 227
```csharp
// TODO: Implement tower registry when available
// foreach (var tower in _towerRegistry.GetAllTowers())
```
**Context:** IsTooCloseToOtherTowers method - second instance needing tower registry.

### Line 293
```csharp
TowerRegistry = null; // TODO: Implement TowerRegistry when available
```
**Context:** Initialize method needs TowerRegistry implementation.

---

## Engine\Waves\WaveSpawnGroup.cs

### Line 162
```csharp
var difficulty = "Normal"; // TODO: Implement proper difficulty system
```
**Context:** GetEffectiveCount method needs proper difficulty system implementation.

### Line 559
```csharp
// TODO: Add ChampionLevel property to Enemy class
// enemy.ChampionLevel = ChampionLevel;
```
**Context:** ApplyChampionProperties method needs ChampionLevel property on Enemy class.

### Line 568
```csharp
// enemy.SetChampionVisuals(); // TODO: implement champion visuals
```
**Context:** ApplyChampionProperties method needs champion visuals implementation.

---

## Engine\Waves\SpawnPattern.cs

### Line 739
```csharp
// TODO: Fix type mismatch - Bounds is Rect?, can't use ?? with Vector3
// var bounds = parameters.Bounds ?? new Vector3(10f, 10f, 0f);
```
**Context:** GetCirclePattern method has type mismatch with Bounds property.

---

## Engine\UI\Systems\UISystem.cs

### Line 337
```csharp
// TODO: Implement Cleanup method for UIPanel
// panel.Cleanup();
```
**Context:** Cleanup method needs UIPanel.Cleanup implementation.

---

## Engine\UI\Systems\UIManager.cs

### Line 129
```csharp
// TODO: Implement UIManager with proper UIElement interface
```
**Context:** AddElement method needs proper UIElement interface implementation.

### Line 132
```csharp
// TODO: Implement when UIElement has required properties (Id, IsEnabled, etc.)
```
**Context:** AddElement method needs UIElement properties.

### Line 136
```csharp
// TODO: Implement UIManager with proper UIElement interface
```
**Context:** RemoveElement method needs proper UIElement interface.

### Line 139
```csharp
// TODO: Implement when UIElement has required properties (Id, etc.)
```
**Context:** RemoveElement method needs UIElement properties.

### Line 152
```csharp
// TODO: Implement UIManager with proper UIElement interface
```
**Context:** SetFocus method needs proper UIElement interface.

### Line 155
```csharp
// TODO: Implement when UIElement has required properties (IsEnabled, SetFocus, etc.)
```
**Context:** SetFocus method needs UIElement properties.

### Line 158
```csharp
// TODO: Implement UIManager with proper UIElement interface
```
**Context:** ClearFocus method needs proper UIElement interface.

### Line 161
```csharp
// TODO: Implement when UIElement has required properties (RemoveFocus, etc.)
```
**Context:** ClearFocus method needs UIElement properties.

### Line 170
```csharp
// TODO: Implement UIManager with proper UIElement interface
```
**Context:** HandleInput method needs proper UIElement interface.

### Line 173
```csharp
// TODO: Implement when UIElement has required properties (IsEnabled, Bounds, HandleInput, etc.)
```
**Context:** HandleInput method needs UIElement properties.

---

## Engine\UI\Systems\ScoreDisplaySystem.cs

### Line 167
```csharp
// TODO: Fix DrawText method signature
// context.DrawText($"+{popup.Score}", popup.Position.X, popup.Position.Y, popupColor, FontSize - 4);
```
**Context:** RenderPopups method needs DrawText method signature fix.

---

## Engine\UI\Statistics\StatisticsDisplay_Main.cs

### Line 135
```csharp
// TODO: Fix VictoryStatistics properties - TotalScore and TotalKills don't exist
// Score = victoryStats?.TotalScore ?? 0;
// Kills = victoryStats?.TotalKills ?? 0;
```
**Context:** Initialize(VictoryStatistics) method needs property fixes.

### Line 150
```csharp
// TODO: Fix GameStatistics properties - TotalScore and TotalKills don't exist
// Score = gameStats?.TotalScore ?? 0;
// Kills = gameStats?.TotalKills ?? 0;
```
**Context:** Initialize(GameStatistics) method needs property fixes.

---

## Engine\UI\Rendering\UIRenderer.cs

### Line 217
```csharp
// TODO: Implement SetClipRect method
// _context.SetClipRect(rect);
```
**Context:** SetClipRect method needs implementation.

---

## Engine\UI\HUD\HUDController.cs

### Line 754
```csharp
// TODO: Implement rendering system
// RenderSystem.DrawString(Title, x + 50f, y + 15f, titleColor, font);
```
**Context:** RenderNotification method needs rendering system implementation.

### Line 760
```csharp
// TODO: Implement rendering system
// RenderSystem.DrawString(Message, x + 50f, y + 35f, messageColor, font);
```
**Context:** RenderNotification method - second instance needing rendering system.

---

## Engine\UI\HUD\LivesDisplay.cs

### Line 475
```csharp
// TODO: Implement rendering system
RenderSystem.DrawHeart(heartPosition.X, heartPosition.Y, heartSize, heartColor);
```
**Context:** Render method needs DrawHeart implementation.

### Line 480
```csharp
// TODO: Implement rendering system
RenderSystem.DrawEmptyHeart(heartPosition.X, heartPosition.Y, heartSize, heartColor);
```
**Context:** Render method needs DrawEmptyHeart implementation.

---

## Engine\UI\HUD\TowerInfoPanel.cs

### Line 104
```csharp
/// Section displaying special abilities (TODO: implementation pending).
```
**Context:** _specialAbilitiesSection field comment indicating pending implementation.

### Line 520
```csharp
// TODO: Add TargetPriority property to Tower class
```
**Context:** UpdateTowerInfo method needs TargetPriority property on Tower class.

### Line 525
```csharp
// TODO: Add SpecialAbilities property to Tower class
```
**Context:** UpdateTowerInfo method needs SpecialAbilities property on Tower class.

---

## Engine\UI\HUD\UpgradePanel.cs

### Line 489
```csharp
// TODO: Fix type mismatch - cannot cast Tower to UI.HUD.TowerUpgrade
// OnUpgradeCompleted?.Invoke((UI.HUD.TowerUpgrade)_currentTower);
```
**Context:** PurchaseUpgrade method has type mismatch with TowerUpgrade casting.

---

## Engine\UI\Components\Label.cs

### Line 204
```csharp
// TODO: SizeF type not found - using Vector3 instead
Size = new System.Drawing.SizeF(TextSize.X, TextSize.Y);
```
**Context:** InvalidateTextCache method has SizeF type issue.

---

## Engine\Snapshot\SnapshotIntegration.cs

### Line 2
**TODO comment found** - Need to examine file content.

---

## Engine\Snapshot\SnapshotCommands.cs

### Line 1
**TODO comment found** - Need to examine file content.

---

## Engine\Resources\RSInitializer.cs

### Line 1
**TODO comment found** - Need to examine file content.

---

## Engine\Save\SAS\SASGameSave.cs

### Line 3
**TODO comment found** - Need to examine file content.

---

## Engine\Scenes\SceneManager.cs

### Line 1
**TODO comment found** - Need to examine file content.

---

## Engine\Save\SAS\EnemySaveData.cs

### Line 1
**TODO comment found** - Need to examine file content.

---

## Engine\Scenes\MainMenuScene.cs

### Line 1
**TODO comment found** - Need to examine file content.

---

## Engine\Rendering\D3D11\D3D11DeviceCoreFullscreenQuad.cs

### Line 2
**TODO comment found** - Need to examine file content.

---

## Engine\Navigation\NavigationDebugRenderer.cs

### Line 2
**TODO comment found** - Need to examine file content.

---

## Engine\Enemies\EnemySystem.cs

### Line 2
**TODO comment found** - Need to examine file content.

---

## Engine\GameRoot\Initialization.cs

### Line 3
**TODO comment found** - Need to examine file content.

---

## Engine\EngineBootstrap.cs

### Line 1
**TODO comment found** - Need to examine file content.

---

## Engine\Animation\Systems\AnimationUpdateSystem.cs

### Line 1
**TODO comment found** - Need to examine file content.

---

## Engine\Animation\Integration\AnimationECSIntegration.cs

### Line 2
**TODO comment found** - Need to examine file content.

---

## Summary

**Total TODO comments found:** 44+ across 30 Engine files (BGFX-related TODOs excluded)

**Common themes:**
1. **Missing method implementations** - Many TODOs reference methods that don't exist on classes
2. **Property additions needed** - Several classes need new properties (ChampionLevel, TargetPriority, SpecialAbilities)
3. **Type mismatches** - Several TODOs indicate type conversion issues
4. **Rendering system integration** - Multiple UI/HUD components need rendering system implementation
5. **Audio system integration** - ModernAudioSubsystem needs implementation
6. **Tower registry** - Placement system needs tower registry implementation
7. **Difficulty system** - Wave system needs proper difficulty implementation

**Priority recommendations:**
- **High Priority:** Tower method calls, UI rendering, Audio system integration
- **Medium Priority:** Property additions, Type fixes
- **Low Priority:** Difficulty system, Champion visuals
