# SAS TD Decision Mapping for Top-Down Flowchart

## Overview
This document maps all decision points and game states for SAS TD to create a comprehensive top-down flowchart, including actual C# class names and folder locations.

## Main Game Flow

### 1. Application Start
```
Start → Program.cs → EngineCore.Program.Main() → MainMenuController.Initialize()
```

### 2. Main Menu Branches
```
MainMenuController (UI/MainMenu/) → Player Action Decision:
├── New Game → NewGameController.StartGame()
├── Load Game → SASGameSaveManager.LoadSave()
├── Settings → SettingsController.OpenSettings()
└── Exit → EngineCore.Shutdown()
```

### 3. New Game Flow
```
NewGameController (UI/NewGame/) → DifficultySelectionController.ShowDifficulty():
├── Normal → DifficultyMode.Normal
├── Hard → DifficultyMode.Hard
└── Elite → DifficultyMode.Elite

DifficultySelectionController → MapSelectionController.ShowMaps():
├── Map 1 → MapController.LoadMap("Map1")
├── Map 2 → MapController.LoadMap("Map2")
├── Map 3 → MapController.LoadMap("Map3")
└── Back → MainMenuController.ShowMenu()
```

### 4. Core Game Loop
```
MapController (Maps/) → GameStateMachine.StartGame() → WaveDirector.StartWave()

GameStateMachine (GameState/) → Status Check:
├── Enemies Spawning → EnemyManager.SpawnWave()
├── All Enemies Defeated → WaveDirector.CompleteWave()
├── Player Lives = 0 → GameStateMachine.SetState(GameState.GameOver)
└── Player Paused → GameStateMachine.SetState(GameState.Paused)
```

### 5. Tower Management Decisions
```
Wave In Progress → TowerController (Towers/) → Player Action:
├── Place Tower → PlacementValidator.ValidatePosition()
├── Upgrade Tower → NeuralManager.PurchaseUpgrade()
├── Sell Tower → TowerRegistry.SellTower()
└── Select Tower → TowerInfoPanel.ShowTowerInfo()

PlacementValidator (Towers/) → Position Validation:
├── Valid → TowerRegistry.AddTower()
└── Invalid → PlacementRenderer.ShowError()

NeuralManager (Towers/TowerControl/) → Affordability Check:
├── Can Afford → TowerBrain.ApplyToTower()
└── Cannot Afford → HUDController.ShowError()
```

### 6. Wave Progression
```
WaveDirector (Waves/) → Wave Complete → RewardDistribution.GiveRewards() → Continue Decision:
├── Yes → WaveDirector.StartNextWave()
└── No → GameStateMachine.SetState(GameState.MainMenu)

WaveDirector.StartNextWave() → Wave Start (Loop)
```

### 7. Game States
```
GameStateMachine (GameState/) → Game Paused → PauseController.HandlePause():
├── Resume → GameStateMachine.SetState(GameState.Playing)
├── Save Game → SASGameSaveManager.CreateSave()
├── Load Game → SASGameSaveManager.LoadSave()
└── Quit to Menu → GameStateMachine.SetState(GameState.MainMenu)

SASGameSaveManager (Save/SAS/) → Save Result:
├── Success → PauseController.ReturnToPaused()
└── Failure → ErrorController.ShowSaveError()

SASGameSaveManager → Load Result:
├── Success → GameStateManager.RestoreGameState()
└── Failure → PauseController.ReturnToPaused()
```

### 8. End Game States
```
GameStateMachine → Game Over → GameOverController.ShowGameOver():
├── Retry → MapSelectionController.ShowMaps()
├── Main Menu → MainMenuController.ShowMenu()
└── Exit → EngineCore.Shutdown()

GameStateMachine → Victory → VictoryController.ShowVictory():
├── Next Map → MapSelectionController.ShowMaps()
├── Main Menu → MainMenuController.ShowMenu()
└── Exit → EngineCore.Shutdown()
```

### 9. Load Game Flow
```
MainMenuController → Load Game → SASGameSaveManager (Save/SAS/) → Save File Check:
├── File Exists → SASGameSave.DeserializeFromJson()
└── No File → ErrorController.ShowNoSaveError() → MainMenuController.ShowMenu()

SASGameSave.DeserializeFromJson() → Load Result:
├── Success → GameStateManager.RestoreGameState()
└── Failure → ErrorController.ShowLoadError()
```

### 10. Settings Flow
```
SettingsController (UI/Settings/) → Settings Category:
├── Graphics → GraphicsSettingsController.ShowOptions()
├── Audio → AudioSettingsController.ShowOptions()
├── Gameplay → GameplaySettingsController.ShowOptions()
└── Back → MainMenuController.ShowMenu()

Each Settings Controller → Settings.ApplyChanges() → Return to Settings
```

## HUD Integration Flow

### 11. HUD System Integration
```
HUDController (UI/HUD/) → Initialize():
├── CashDisplay.Initialize()
├── WaveDisplay.Initialize()
├── LivesDisplay.Initialize()
├── TowerInfoPanel.Initialize()
├── UpgradePanel.Initialize()
└── PlacementInfoDisplay.Initialize()

EconomyManager (Economy/) → CashDisplay.UpdateCash()
WaveDirector (Waves/) → WaveDisplay.UpdateWave()
PlayerLives (GameState/) → LivesDisplay.UpdateLives()
TowerRegistry (Towers/) → TowerInfoPanel.UpdateTowerInfo()
NeuralManager (Towers/TowerControl/) → UpgradePanel.UpdateUpgrades()
PlacementValidator (Towers/) → PlacementInfoDisplay.UpdatePlacement()
```

## Decision Points Summary

### Critical Decision Nodes with C# Classes
1. **MainMenuController.PlayerAction()** - Game entry point
2. **DifficultySelectionController.SelectDifficulty()** - Game difficulty modifier
3. **MapSelectionController.SelectMap()** - Level selection
4. **GameStateMachine.CheckWaveStatus()** - Game progression state
5. **TowerController.HandlePlayerAction()** - Tower management
6. **PlacementValidator.ValidatePosition()** - Tower placement rules
7. **NeuralManager.CanPurchaseUpgrade()** - Resource management
8. **PauseController.HandlePause()** - Game state management
9. **SASGameSaveManager.Save/Load()** - Data persistence
10. **GameOverController/VictoryController.HandleEndGame()** - Completion handling

### Validation Points with Classes
- **PlacementValidator.IsValidPosition()** - Tower placement validity
- **NeuralManager.CanAffordUpgrade()** - Upgrade affordability
- **SASGameSaveManager.SaveExists()** - Save file existence
- **SASGameSaveManager.LoadSave()** - Load operation success
- **SASGameSaveManager.CreateSave()** - Save operation success

### State Transitions with Classes
- **GameStateMachine.TransitionTo()** - Main state changes
- **PauseController.Resume/Pause()** - Pause state management
- **WaveDirector.NextWave()** - Wave progression loop
- **TowerController.Place/Upgrade/Sell()** - Tower management loop
- **SettingsController.NavigateSettings()** - Settings navigation

## Class Locations Reference

### Core Engine Classes
- `Program.cs` - Root/
- `EngineCore.Program` - Engine/Core/
- `GameStateMachine` - Engine/GameState/
- `GameState` - Engine/GameState/

### UI Controllers
- `MainMenuController` - Engine/UI/MainMenu/
- `NewGameController` - Engine/UI/NewGame/
- `DifficultySelectionController` - Engine/UI/NewGame/
- `MapSelectionController` - Engine/UI/Maps/
- `SettingsController` - Engine/UI/Settings/
- `PauseController` - Engine/UI/Pause/
- `GameOverController` - Engine/UI/GameOver/
- `VictoryController` - Engine/UI/Victory/
- `HUDController` - Engine/UI/HUD/

### Game Systems
- `TowerController` - Engine/Towers/
- `TowerRegistry` - Engine/Towers/
- `PlacementValidator` - Engine/Towers/
- `PlacementRenderer` - Engine/Towers/
- `NeuralManager` - Engine/Towers/TowerControl/
- `TowerBrain` - Engine/Towers/TowerControl/
- `NeuralDatabase` - Engine/Towers/TowerControl/

- `EnemyManager` - Engine/Enemies/
- `WaveDirector` - Engine/Waves/
- `MapController` - Engine/Maps/
- `EconomyManager` - Engine/Economy/
- `PlayerLives` - Engine/GameState/

### Save/Load System
- `SASGameSaveManager` - Engine/Save/SAS/
- `SASGameSave` - Engine/Save/SAS/
- `TowerSaveData` - Engine/Save/SAS/
- `EnemySaveData` - Engine/Save/SAS/

### HUD Components
- `CashDisplay` - Engine/UI/HUD/
- `WaveDisplay` - Engine/UI/HUD/
- `LivesDisplay` - Engine/UI/HUD/
- `TowerInfoPanel` - Engine/UI/HUD/
- `UpgradePanel` - Engine/UI/HUD/
- `PlacementInfoDisplay` - Engine/UI/HUD/

## Flowchart Construction Notes

### Node Types with Classes
- **Start/End**: `Program.cs`, `EngineCore.Shutdown()`
- **Process**: `GameStateMachine`, `WaveDirector`, `TowerController`
- **Decision**: `PlacementValidator`, `NeuralManager`, `SASGameSaveManager`
- **Data**: `SASGameSave`, `TowerSaveData`, `EnemySaveData`

### Connection Rules
- All decisions must have at least 2 outcomes
- Loops should be clearly marked with class methods
- Error states should return to previous controller
- Victory/defeat should have multiple exit options

### Styling Recommendations
- Use different colors for:
  - Game states (blue) - GameStateMachine
  - Decisions (orange) - Validator classes
  - Actions (green) - Controller classes
  - Error states (red) - ErrorController
  - End states (gray) - EngineCore.Shutdown()

This mapping provides complete decision structure with actual C# class names and folder locations for building a comprehensive SAS TD flowchart.
