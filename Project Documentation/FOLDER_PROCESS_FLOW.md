# SAS TD Folder Structure & Program Flow Mapping

## Overview
This document maps the complete folder structure and C# programs with step-by-step process flow showing how execution moves from folder to folder.

## Folder Structure with Programs

### Root Directory
```
Root/
└── Program.cs
```

### Engine Core Structure
```
Engine/
├── Core/
│   └── EngineCore.cs
├── GameState/
│   ├── GameStateMachine.cs
│   ├── GameState.cs
│   └── GameStateManager.cs
├── UI/
│   ├── MainMenu/
│   │   └── MainMenuController.cs
│   ├── NewGame/
│   │   ├── NewGameController.cs
│   │   └── DifficultySelectionController.cs
│   ├── Maps/
│   │   └── MapSelectionController.cs
│   ├── Settings/
│   │   ├── SettingsController.cs
│   │   ├── GraphicsSettingsController.cs
│   │   ├── AudioSettingsController.cs
│   │   └── GameplaySettingsController.cs
│   ├── HUD/
│   │   ├── HUDController.cs
│   │   ├── CashDisplay.cs
│   │   ├── WaveDisplay.cs
│   │   ├── LivesDisplay.cs
│   │   ├── TowerInfoPanel.cs
│   │   ├── UpgradePanel.cs
│   │   └── PlacementInfoDisplay.cs
│   ├── Pause/
│   │   └── PauseController.cs
│   ├── GameOver/
│   │   └── GameOverController.cs
│   └── Victory/
│       └── VictoryController.cs
├── Towers/
│   ├── TowerController.cs
│   ├── TowerRegistry.cs
│   ├── PlacementValidator.cs
│   ├── PlacementRenderer.cs
│   ├── TowerPlacementPreview.cs
│   └── TowerControl/
│       ├── NeuralManager.cs
│       ├── NeuralNet.cs
│       └── NeuralDatabase.cs
├── Enemies/
│   ├── EnemyManager.cs
│   └── Enemy.cs
├── Waves/
│   ├── WaveDirector.cs
│   └── Wave.cs
├── Maps/
│   └── MapController.cs
├── Economy/
│   └── EconomyManager.cs
├── Save/
│   └── SAS/
│       ├── SASGameSaveManager.cs
│       ├── SASGameSave.cs
│       ├── TowerSaveData.cs
│       └── EnemySaveData.cs
└── Utility/
    └── ErrorController.cs
```

## Step-by-Step Process Flow

### 1. Application Startup
```
Root/Program.cs 
--> Engine/Core/EngineCore.cs 
--> Engine/UI/MainMenu/MainMenuController.cs
```

### 2. Main Menu Navigation
```
Engine/UI/MainMenu/MainMenuController.cs
--> Player Action Decision:
    ├── New Game --> Engine/UI/NewGame/NewGameController.cs
    ├── Load Game --> Engine/Save/SAS/SASGameSaveManager.cs
    ├── Settings --> Engine/UI/Settings/SettingsController.cs
    └── Exit --> Engine/Core/EngineCore.cs (Shutdown)
```

### 3. New Game Flow
```
Engine/UI/NewGame/NewGameController.cs
--> Engine/UI/NewGame/DifficultySelectionController.cs
--> Engine/UI/Maps/MapSelectionController.cs
--> Engine/Maps/MapController.cs
--> Engine/GameState/GameStateMachine.cs
```

### 4. Game Initialization
```
Engine/GameState/GameStateMachine.cs
--> Engine/Waves/WaveDirector.cs
--> Engine/Enemies/EnemyManager.cs
--> Engine/Economy/EconomyManager.cs
--> Engine/Towers/TowerController.cs
--> Engine/UI/HUD/HUDController.cs
```

### 5. HUD System Initialization
```
Engine/UI/HUD/HUDController.cs
--> Engine/UI/HUD/CashDisplay.cs
--> Engine/UI/HUD/WaveDisplay.cs
--> Engine/UI/HUD/LivesDisplay.cs
--> Engine/UI/HUD/TowerInfoPanel.cs
--> Engine/UI/HUD/UpgradePanel.cs
--> Engine/UI/HUD/PlacementInfoDisplay.cs
```

### 6. Tower Management Flow
```
Engine/Towers/TowerController.cs
--> Engine/Towers/PlacementValidator.cs
--> Engine/Towers/PlacementRenderer.cs
--> Engine/Towers/TowerPlacementPreview.cs
--> Engine/Towers/TowerRegistry.cs
--> Engine/Towers/TowerControl/NeuralManager.cs
--> Engine/Towers/TowerControl/NeuralNet.cs
--> Engine/Towers/TowerControl/NeuralDatabase.cs
```

### 7. Wave Progression Flow
```
Engine/Waves/WaveDirector.cs
--> Engine/Enemies/EnemyManager.cs
--> Engine/GameState/GameStateMachine.cs
--> Engine/UI/HUD/WaveDisplay.cs
--> Engine/UI/HUD/LivesDisplay.cs
```

### 8. Enemy Management Flow
```
Engine/Enemies/EnemyManager.cs
--> Engine/Enemies/Enemy.cs
--> Engine/Waves/WaveDirector.cs
--> Engine/Towers/TowerController.cs
```

### 9. Save/Load Flow
```
Engine/Save/SAS/SASGameSaveManager.cs
--> Engine/Save/SAS/SASGameSave.cs
--> Engine/Save/SAS/TowerSaveData.cs
--> Engine/Save/SAS/EnemySaveData.cs
--> Engine/Towers/TowerController.cs (Restore)
--> Engine/Enemies/EnemyManager.cs (Restore)
--> Engine/GameState/GameStateManager.cs (Restore)
```

### 10. Settings Flow
```
Engine/UI/Settings/SettingsController.cs
--> Engine/UI/Settings/GraphicsSettingsController.cs
--> Engine/UI/Settings/AudioSettingsController.cs
--> Engine/UI/Settings/GameplaySettingsController.cs
--> Engine/Core/EngineCore.cs (Apply Settings)
```

### 11. Pause/Resume Flow
```
Engine/UI/Pause/PauseController.cs
--> Engine/GameState/GameStateMachine.cs (Pause State)
--> Engine/Save/SAS/SASGameSaveManager.cs (Save Option)
--> Engine/UI/Pause/PauseController.cs (Resume)
--> Engine/GameState/GameStateMachine.cs (Resume State)
```

### 12. Game Over Flow
```
Engine/GameState/GameStateMachine.cs (Game Over State)
--> Engine/UI/GameOver/GameOverController.cs
--> Engine/UI/Maps/MapSelectionController.cs (Retry)
--> Engine/UI/MainMenu/MainMenuController.cs (Main Menu)
```

### 13. Victory Flow
```
Engine/GameState/GameStateMachine.cs (Victory State)
--> Engine/UI/Victory/VictoryController.cs
--> Engine/UI/Maps/MapSelectionController.cs (Next Map)
--> Engine/UI/MainMenu/MainMenuController.cs (Main Menu)
```

### 14. Error Handling Flow
```
Any System Error --> Engine/Utility/ErrorController.cs
--> Engine/UI/MainMenu/MainMenuController.cs (Recovery)
```

## Complete Process Chain Example

### Full New Game Session
```
Root/Program.cs
--> Engine/Core/EngineCore.cs
--> Engine/UI/MainMenu/MainMenuController.cs
--> Engine/UI/NewGame/NewGameController.cs
--> Engine/UI/NewGame/DifficultySelectionController.cs
--> Engine/UI/Maps/MapSelectionController.cs
--> Engine/Maps/MapController.cs
--> Engine/GameState/GameStateMachine.cs
--> Engine/Waves/WaveDirector.cs
--> Engine/Enemies/EnemyManager.cs
--> Engine/Economy/EconomyManager.cs
--> Engine/Towers/TowerController.cs
--> Engine/UI/HUD/HUDController.cs
--> Engine/UI/HUD/CashDisplay.cs
--> Engine/UI/HUD/WaveDisplay.cs
--> Engine/UI/HUD/LivesDisplay.cs
--> Engine/Towers/PlacementValidator.cs
--> Engine/Towers/TowerControl/NeuralManager.cs
--> Engine/GameState/GameStateMachine.cs (Wave Complete)
--> Engine/Waves/WaveDirector.cs (Next Wave)
[Loop continues until Game Over/Victory]
```

### Save/Load Session
```
Engine/UI/MainMenu/MainMenuController.cs
--> Engine/Save/SAS/SASGameSaveManager.cs
--> Engine/Save/SAS/SASGameSave.cs
--> Engine/Save/SAS/TowerSaveData.cs
--> Engine/Save/SAS/EnemySaveData.cs
--> Engine/GameState/GameStateManager.cs
--> Engine/Towers/TowerController.cs
--> Engine/Enemies/EnemyManager.cs
--> Engine/Waves/WaveDirector.cs
--> Engine/Economy/EconomyManager.cs
--> Engine/UI/HUD/HUDController.cs
```

## Key Integration Points

### Core System Connections
1. **GameStateMachine** connects to all major systems
2. **HUDController** coordinates all UI displays
3. **TowerController** manages all tower-related operations
4. **WaveDirector** controls enemy spawning and wave progression
5. **SASGameSaveManager** handles all persistence operations

### Cross-Folder Dependencies
- **UI Controllers** depend on **Engine Systems**
- **Game Systems** depend on **GameStateMachine**
- **Save System** depends on **All Game Systems**
- **HUD System** depends on **All Game Systems**

This mapping provides the complete folder structure and step-by-step process flow for SAS TD development and flowchart creation.
