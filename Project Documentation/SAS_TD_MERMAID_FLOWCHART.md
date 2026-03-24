flowchart TD
    Start([Program.cs Start]) --> EngineCore[EngineCore.Program.Main]
    EngineCore --> MainMenu[MainMenuController]
    
    MainMenu --> NewGame[NewGameController]
    MainMenu --> LoadGame[SASGameSaveManager]
    MainMenu --> Settings[SettingsController]
    
    NewGame --> Difficulty[DifficultySelectionController]
    LoadGame --> MainMenu
    Settings --> MainMenu
    
    Difficulty --> MapSelection[MapSelectionController]
    MapSelection --> GameStart[GameStateMachine.Start]
    
    GameStart --> WaveDirector[WaveDirector]
    GameStart --> EnemyManager[EnemyManager]
    GameStart --> EconomyManager[EconomyManager]
    GameStart --> TowerController[TowerController]
    GameStart --> HUDController[HUDController]
    
    HUDController --> CashDisplay[CashDisplay]
    HUDController --> WaveDisplay[WaveDisplay]
    HUDController --> LivesDisplay[LivesDisplay]
    HUDController --> TowerInfoPanel[TowerInfoPanel]
    HUDController --> UpgradePanel[UpgradePanel]
    HUDController --> PlacementInfo[PlacementInfoDisplay]
    
    TowerController --> PlacementValidator[PlacementValidator]
    TowerController --> NeuralManager[NeuralManager]
    TowerController --> TowerRegistry[TowerRegistry]
    
    PlacementValidator --> TowerPlaced[Tower Placed]
    NeuralManager --> TowerUpgraded[Tower Upgraded]
    TowerRegistry --> TowerSold[Tower Sold]
    
    WaveDirector --> EnemySpawn[Enemy Spawning]
    WaveDirector --> WaveComplete[Wave Complete]
    
    EnemySpawn --> TowerController
    WaveComplete --> NextWave[Next Wave]
    
    TowerPlaced --> HUDController
    TowerUpgraded --> HUDController
    TowerSold --> HUDController
    NextWave --> WaveDirector
    
    classDef gameState fill:#e1f5fe
    classDef process fill:#e3f2fd
    classDef endState fill:#ffebee
    
    class Start,MainMenu,GameStart,HUDController gameState
    class EngineCore,NewGame,LoadGame,Settings,Diculty,MapSelection,WaveDirector,EnemyManager,EconomyManager,TowerController,CashDisplay,WaveDisplay,LivesDisplay,TowerInfoPanel,UpgradePanel,PlacementInfo,PlacementValidator,NeuralManager,TowerRegistry,TowerPlaced,TowerUpgraded,TowerSold,EnemySpawn,WaveComplete,NextWave process
```

## Alternative Decision Flow Version

```mermaid
flowchart TD
    Start([Program.cs Start]) --> EngineDecision{Delegate to Engine?}
    
    EngineDecision -->|Yes| EngineCore[EngineCore.Program.Main]
    EngineDecision -->|No| DirectExit[Direct Exit]
    
    EngineCore --> EngineInit{Engine Initialize?}
    EngineInit -->|Success| MainMenu[MainMenuController]
    EngineInit -->|Failure| ErrorExit[Error Shutdown]
    
    MainMenu --> MenuDecision{Player Action}
    MenuDecision -->|New Game| NewGame[NewGameController]
    MenuDecision -->|Load Game| LoadGame[SASGameSaveManager]
    MenuDecision -->|Settings| Settings[SettingsController]
    MenuDecision -->|Exit| CleanShutdown[Clean Shutdown]
    
    NewGame --> Difficulty[DifficultySelectionController]
    Difficulty --> MapSelection[MapSelectionController]
    MapSelection --> GameStart[GameStateMachine.Start]
    
    GameStart --> SystemInit{Initialize Systems}
    SystemInit -->|Waves| WaveDirector[WaveDirector]
    SystemInit -->|Enemies| EnemyManager[EnemyManager]
    SystemInit -->|Economy| EconomyManager[EconomyManager]
    SystemInit -->|Towers| TowerController[TowerController]
    SystemInit -->|HUD| HUDController[HUDController]
    
    HUDController --> HUDInit{Initialize HUD}
    HUDInit -->|Cash| CashDisplay[CashDisplay]
    HUDInit -->|Wave| WaveDisplay[WaveDisplay]
    HUDInit -->|Lives| LivesDisplay[LivesDisplay]
    HUDInit -->|Tower| TowerInfo[TowerInfoPanel]
    HUDInit -->|Upgrade| UpgradePanel[UpgradePanel]
    HUDInit -->|Placement| PlacementInfo[PlacementInfoDisplay]
    
    TowerController --> TowerAction{Player Action}
    TowerAction -->|Place| Placement[PlacementValidator]
    TowerAction -->|Upgrade| NeuralMgr[NeuralManager]
    TowerAction -->|Sell| TowerReg[TowerRegistry]
    TowerAction -->|Select| TowerInfo[TowerInfoPanel]
    
    Placement --> PlaceDecision{Valid Position?}
    PlaceDecision -->|Yes| TowerPlaced[Tower Placed]
    PlaceDecision -->|No| ShowError[Show Error]
    
    NeuralMgr --> UpgradeDecision{Can Afford?}
    UpgradeDecision -->|Yes| TowerUpgraded[Tower Upgraded]
    UpgradeDecision -->|No| InsufficientFunds[Insufficient Funds]
    
    WaveDirector --> WaveStatus{Wave Status}
    WaveStatus -->|Spawning| EnemySpawn[Enemy Spawning]
    WaveStatus -->|Complete| WaveComplete[Wave Complete]
    WaveStatus -->|GameOver| GameOver[Game Over]
    WaveStatus -->|Paused| GamePaused[Game Paused]
    
    GamePaused --> PauseAction{Player Action}
    PauseAction -->|Resume| WaveStatus
    PauseAction -->|Save| SaveGame[SASGameSaveManager]
    PauseAction -->|Load| LoadGame
    PauseAction -->|Quit| MainMenu
    
    SaveGame --> SaveResult{Save Success?}
    SaveResult -->|Yes| GamePaused
    SaveResult -->|No| SaveError[Save Error]
    
    WaveComplete --> NextWave{Continue?}
    NextWave -->|Yes| WaveDirector
    NextWave -->|No| MainMenu
    
    GameOver --> EndAction{Player Action}
    EndAction -->|Retry| MapSelection
    EndAction -->|Menu| MainMenu
    EndAction -->|Exit| End
    
    DirectExit --> End([Application Exit])
    ErrorExit --> End
    CleanShutdown --> End
    
    classDef gameState fill:#e1f5fe
    classDef decision fill:#fff3e0
    classDef process fill:#e3f2fd
    classDef endState fill:#ffebee
    
    class Start,MainMenu,GameStart,SystemInit,HUDController,WaveStatus,GamePaused gameState
    class EngineDecision,EngineInit,MenuDecision,SystemInit,HUDInit,TowerAction,PlaceDecision,UpgradeDecision,WaveStatus,PauseAction,SaveResult,NextWave,EndAction decision
    class EngineCore,NewGame,Diculty,MapSelection,WaveDirector,EnemyManager,EconomyManager,TowerController,CashDisplay,WaveDisplay,LivesDisplay,TowerInfo,UpgradePanel,PlacementInfo,Placement,NeuralMgr,TowerReg,TowerPlaced,TowerUpgraded,EnemySpawn,WaveComplete,GameOver,SaveGame,LoadGame,SaveError,ShowError,InsufficientFunds,MapSelection process
    class End,DirectExit,ErrorExit,CleanShutdown endState
```

## Simple Vertical Stack Version

```mermaid
flowchart TD
    Start([Program.cs]) --> EngineCore[EngineCore]
    EngineCore --> MainMenu[MainMenuController]
    MainMenu --> NewGame[NewGameController]
    NewGame --> Difficulty[DifficultySelection]
    Difficulty --> MapSelection[MapSelectionController]
    MapSelection --> GameState[GameStateMachine]
    GameState --> WaveDirector[WaveDirector]
    WaveDirector --> EnemyManager[EnemyManager]
    EnemyManager --> TowerController[TowerController]
    TowerController --> HUDController[HUDController]
    HUDController --> CashDisplay[CashDisplay]
    CashDisplay --> WaveDisplay[WaveDisplay]
    WaveDisplay --> LivesDisplay[LivesDisplay]
    LivesDisplay --> TowerInfo[TowerInfoPanel]
    TowerInfo --> UpgradePanel[UpgradePanel]
    UpgradePanel --> PlacementInfo[PlacementInfoDisplay]
    PlacementInfo --> End([Game Complete])
```

## Usage Instructions

### How to Use These Flowcharts

1. **Copy the entire code block** including the ```mermaid tags
2. **Paste into your Mermaid tool** (like mermaid.live, draw.io, etc.)
3. **Choose which version** you want:
   - Simple vertical stack for clean flow
   - Decision flow for detailed logic
   - Complete version for full architecture

### Customization Tips

- **Change colors**: Modify the `classDef` values
- **Add nodes**: Follow the `NodeName[Label]` pattern
- **Create decisions**: Use `DecisionName{Question}` syntax
- **Add labels**: Use `-->|Label|` for branch labels
- **Group nodes**: Use `class` statements to style groups

### Folder Structure Mapping

Each node represents:
- **C# Class**: The actual class name
- **Folder Location**: Based on engine structure
- **Process Flow**: How execution moves between systems
- **Decision Points**: Where logic branches occur

This file provides ready-to-use Mermaid code for SAS TD flowchart creation.
