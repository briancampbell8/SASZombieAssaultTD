# SAS Zombie Assault TD - Full Project Directory Map

## Root Directory Structure

```
SASZombieAssaultTD/
├── .config/                          # Configuration files
├── .git/                             # Git repository data
├── .vs/                              # Visual Studio configuration
├── .vscode/                          # VS Code configuration
├── Archive/                          # Archived files
├── Assets/                           # Game assets directory
├── Automation/                       # Automation tools and scripts
├── bin/                              # Compiled binaries
├── blobs/                            # Binary large objects
├── Core/                             # Core game logic
├── Docs/                             # Documentation
├── Engine/                           # Game engine source code
├── Entities/                         # Entity definitions
├── GameData/                         # Game data files
├── LogAnalysisReports/               # Analysis reports
├── Logs/                             # Log files
├── Maps/                             # Game maps
├── OriginalSWF/                      # Original Flash files
├── Project Documents/                # Project documentation
├── SASZombieAssaultTD/               # Main game project
├── Scenes/                           # Game scenes
├── Systems/                          # Game systems
├── Tools/                            # Development tools
├── UI/                               # User interface
├── obj/                              # Build objects
├── manifests/                        # Build manifests
├── Program.cs                        # Entry point
├── SASZombieAssaultTD.csproj         # Project file
├── SASZombieAssaultTD.slnx          # Solution file
├── app.manifest                      # Application manifest
├── .gitattributes                    # Git attributes
├── .gitignore                        # Git ignore file
├── CS0246_Error_Report.xlsx          # Error report
├── CompleteErrorAnalysis.csv         # Error analysis
├── error_fix_plan.xlsx               # Error fixing plan
├── remaining_errors.xlsx             # Remaining errors
└── remaining_errors_after_wrap.xlsx  # Post-fix errors
```

## Assets Directory Structure

```
Assets/
├── AtlasSource/                      # Texture atlas sources
│   └── PremiumItems/                 # Premium item assets
├── Audio/                            # Audio files
├── Battlefields/                     # Battlefield assets
├── Bullets/                          # Projectile assets
│   ├── Assault/                      # Assault rifle bullets
│   ├── Flamethrower/                 # Flamethrower effects
│   ├── Rocket/                       # Rocket projectiles
│   └── Sniper/                       # Sniper bullets
├── Effects/                          # Visual effects
│   ├── Blood/                        # Blood effects
│   ├── Explosions/                   # Explosion effects
│   ├── HitEffects/                   # Hit impact effects
│   └── MuzzleFlashes/                # Muzzle flash effects
├── FlashExtract/                     # Extracted Flash assets
│   └── PremiumItems/                 # Premium item Flash assets
├── Fonts/                            # Font files
├── Sprites/                          # Sprite sheets
│   ├── PremiumItems/                 # Premium item sprites
│   ├── Soldiers/                     # Soldier sprites
│   └── Zombies/                      # Zombie sprites
├── Textures/                         # Texture files
├── Tiles/                            # Tile sets
├── Towers/                           # Tower assets
│   ├── Assault/                      # Assault tower assets
│   ├── Flamethrower/                 # Flamethrower tower assets
│   ├── Rocket/                       # Rocket tower assets
│   ├── Sniper/                       # Sniper tower assets
│   └── Support/                      # Support tower assets
├── UI/                               # User interface assets
│   ├── AbilitySymbols/               # Ability icons
│   ├── Buttons/                      # Button graphics
│   ├── HUD/                          # HUD elements
│   └── Icons/                        # UI icons
└── Zombies/                          # Zombie assets
    ├── Runner/                       # Runner zombie assets
    │   ├── Attack/                   # Attack animations
    │   ├── Death/                    # Death animations
    │   └── Walk/                     # Walk animations
    ├── Special/                      # Special zombie assets
    │   ├── Attack/                   # Attack animations
    │   ├── Death/                    # Death animations
    │   └── Walk/                     # Walk animations
    ├── Tank/                         # Tank zombie assets
    │   ├── Attack/                   # Attack animations
    │   ├── Death/                    # Death animations
    │   └── Walk/                     # Walk animations
    └── Walker/                       # Walker zombie assets
        ├── Attack/                   # Attack animations
        ├── Death/                    # Death animations
        └── Walk/                     # Walk animations
```

## Engine Directory Structure

```
Engine/
├── Animation/                        # Animation system
│   ├── BlendTrees/                   # Animation blend trees
│   │   └── Nodes/                    # Blend tree nodes
│   ├── Diagnostics/                  # Animation diagnostics
│   ├── Events/                       # Animation events
│   └── States/                       # Animation states
├── Compatibility/                    # Compatibility layer
│   └── UnityStubs/                   # Unity compatibility stubs
├── Components/                       # Engine components
├── Core/                             # Core engine systems
│   ├── Input/                        # Input handling
│   ├── Logging/                      # Logging system
│   └── Timing/                       # Timing utilities
├── Debug/                            # Debug utilities
├── ECS/                              # Entity Component System
│   ├── Components/                   # ECS components
│   └── Systems/                      # ECS systems
├── Entities/                         # Entity management
├── Managers/                         # Engine managers
├── Navigation/                       # Pathfinding and navigation
├── Physics/                          # Physics system
├── Platform/                         # Platform-specific code
├── Rendering/                        # Rendering system
│   ├── Debug/                        # Debug rendering
│   └── Zombies/                      # Zombie-specific rendering
├── Scenes/                           # Scene management
│   └── TestScenes/                   # Test scenes
└── Systems/                          # Engine systems
    ├── AI/                           # AI systems
    │   ├── Behaviors/                # AI behaviors
    │   └── Blackboard/               # AI blackboard
    ├── Achievements/                 # Achievement system
    │   └── UI/                       # Achievement UI
    ├── Assets/                       # Asset management
    ├── Audio/                        # Audio system
    ├── Challenges/                   # Challenge system
    ├── Combat/                       # Combat system
    │   └── Statistics/               # Combat statistics
    ├── Diagnostics/                  # System diagnostics
    ├── Enemies/                      # Enemy systems
    │   └── Tests/                    # Enemy system tests
    ├── Events/                       # Event system
    ├── Gameplay/                     # Gameplay systems
    │   ├── Animation/                # Gameplay animations
    │   └── Zombies/                  # Zombie gameplay
    ├── Hazards/                      # Hazard system
    │   └── Analytics/                # Hazard analytics
    ├── Input/                        # Input system
    ├── Match/                        # Match management
    ├── Multiplayer/                  # Multiplayer systems
    ├── Particles/                    # Particle system
    ├── Performance/                  # Performance monitoring
    ├── Physics/                      # Physics systems
    ├── Rendering/                    # Rendering systems
    ├── Resources/                    # Resource management
    ├── SaveLoad/                     # Save/Load system
    ├── Scoring/                      # Scoring system
    ├── Serialization/                # Serialization utilities
    ├── Spawning/                     # Entity spawning
    ├── UI/                           # UI systems
    └── Waves/                        # Wave management
```

## Systems Directory Structure

```
Systems/
├── AI/                               # AI systems
├── Collision/                        # Collision detection
├── Combat/                           # Combat mechanics
├── Economy/                          # Economic systems
├── Placement/                        # Tower placement
├── Survival/                         # Survival mechanics
├── Upgrades/                         # Upgrade systems
└── Waves/                            # Wave management
```

## Documentation Structure

```
Docs/
└── Decompiled/                       # Decompiled documentation
```

## Project Documents Structure

```
Project Documents/
├── CSV/                              # CSV data files
├── Excel/                            # Excel spreadsheets
├── Images/                           # Project images
├── Markdown/                         # Markdown documentation
├── SAS_PowerShell_Scripts/           # PowerShell automation scripts
│   ├── Automation/                   # Automation scripts
│   │   ├── Automation/               # Core automation
│   │   ├── Changes/                  # Change tracking
│   │   ├── Docs/                     # Automation documentation
│   │   ├── Engine/                   # Engine automation
│   │   ├── Logs/                     # Automation logs
│   │   ├── Modules/                  # PowerShell modules
│   │   └── Templates/                # Script templates
│   └── Tools/                        # PowerShell tools
│       └── Harry/                    # Harry's tools
└── Word/                             # Word documents
```

## Automation Directory Structure

```
Automation/
├── Automation/                       # Main automation scripts
│   └── Logs/                         # Automation logs
├── Changes/                          # Change tracking
├── Docs/                             # Automation documentation
└── Engine/                           # Engine automation
    └── Systems/                      # System automation
```

## Core Directory Structure

```
Core/
├── Config/                           # Configuration files
└── Managers/                         # Core managers
```

## Tools Directory Structure

```
Tools/
├── Harry/                            # Harry's tools
├── LogAnalysis/                      # Log analysis tools
├── Performance/                      # Performance tools
├── Testing/                          # Testing tools
└── Utilities/                        # Utility tools
```

## Key File Types by Extension

- **.cs** - C# source code files
- **.csproj** - C# project files
- **.slnx** - Solution files
- **.xlsx/.xls** - Excel spreadsheets
- **.csv** - Comma-separated values
- **.md** - Markdown documentation
- **.ps1** - PowerShell scripts
- **.png/.jpg/.gif** - Image files
- **.wav/.mp3** - Audio files
- **.json/.xml** - Configuration and data files
- **.html** - HTML files (like ProjectFileTree.html)

## Development Areas

1. **Game Engine** - Located in `Engine/` directory
2. **Game Assets** - Located in `Assets/` directory
3. **Game Systems** - Located in `Systems/` directory
4. **Core Logic** - Located in `Core/` directory
5. **Documentation** - Located in `Docs/` and `Project Documents/`
6. **Automation** - Located in `Automation/` directory
7. **Development Tools** - Located in `Tools/` directory

## Build Artifacts

- **bin/** - Compiled binaries
- **obj/** - Build objects and intermediate files
- **manifests/** - Build manifests

## Configuration and Version Control

- **.git/** - Git repository
- **.config/** - Configuration files
- **.vs/.vscode/** - IDE configurations
- **app.manifest** - Application manifest

This directory map provides a comprehensive overview of the SAS Zombie Assault TD project structure, showing all major directories and their subdirectories up to several levels deep.
