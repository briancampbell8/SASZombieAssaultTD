# Asset System Flow Map - SAS Zombie Assault TD

## Overview
This document provides a comprehensive flow map of all Asset C# programs in the SAS Zombie Assault TD project, showing the architecture, data flow, and integration points between components.

---

## System Architecture

```
┌─────────────────────────────────────────────────────────────────────────┐
│                         ASSET SYSTEM ENTRY POINT                          │
│                              AssetSystem                                  │
│                    (Resources/Integration/)                               │
└──────────────────────────────┬────────────────────────────────────────────┘
                               │
                               │ Initialize()
                               ▼
        ┌──────────────────────────────────────────────────────────────┐
        │                    AssetManager                              │
        │              (Resources/Manager/)                            │
        │  • AssetManager_Core.cs - Initialization, disposal           │
        │  • AssetManager_Loader.cs - Loading, caching, async ops      │
        │  • AssetManager_Registry.cs - Registration, metadata         │
        │  • AssetManager_Validation.cs - Health checks                │
        │  • AssetManager_Types.cs - Supporting types                 │
        │  • AssetLoader.cs - Core loading logic                       │
        └────────────┬─────────────────────────────────┬────────────────┘
                     │                                 │
                     │ LoadAsset()                     │ ProcessAsset()
                     ▼                                 ▼
        ┌──────────────────────────┐      ┌──────────────────────────┐
        │     AssetBundle          │      │     AssetPipeline        │
        │   (Resources/Bundles/)   │      │  (Resources/Pipeline/)   │
        │  • AssetBundle_Core     │      │  • AssetPipeline_Core    │
        │  • AssetBundle_Loader   │      │  • AssetPipeline_Import  │
        │  • AssetBundle_Metadata │      │  • AssetPipeline_Process │
        │  • AssetBundle_Process  │      │  • AssetPipeline_Output  │
        │  • AssetBundle_Validate │      │  • AssetPipeline_Discover│
        └──────────┬───────────────┘      └──────────┬───────────────┘
                   │                                 │
                   │ GetAsset()                     │ Processors
                   ▼                                 ▼
        ┌──────────────────────────┐      ┌──────────────────────────┐
        │   Specialized Loaders   │      │   Asset Processors      │
        │                          │      │                          │
        │ • TextureLoader          │      │ • TextureProcessor      │
        │ • CompositeImageLoader   │      │ • AudioProcessor        │
        │ • UIAssetLoader         │      │ • ModelProcessor        │
        │ • WaveLoader            │      │ • FontProcessor         │
        │ • DataLoader            │      │ • DataProcessor         │
        └──────────┬───────────────┘      └──────────┬───────────────┘
                   │                                 │
                   └──────────────┬──────────────────┘
                                  ▼
                    ┌──────────────────────────┐
                    │   Consuming Systems     │
                    │                          │
                    │ • Rendering System      │
                    │ • UI System             │
                    │ • Audio System          │
                    │ • Gameplay System       │
                    └──────────────────────────┘
```

---

## Component Details

### 1. AssetSystem (Integration Layer)
**Location:** `Engine/Resources/Integration/`

**Files:**
- `AssetIntegration_Core.cs` - Main orchestration, initialization, public API
- `AssetIntegration_Bundles.cs` - Bundle loading integration
- `AssetIntegration_Pipeline.cs` - Pipeline processing integration

**Responsibilities:**
- Facade entry point for all asset operations
- Coordinates AssetManager, AssetPipeline, and AssetBundle subsystems
- Thread-safe initialization and shutdown
- Provides system status and memory statistics

**Key Methods:**
- `Initialize(assetRootPath, bundlePath)` - System initialization
- `LoadAsset<T>(path)` - Unified asset loading
- `GetMemoryStats()` - Memory usage statistics
- `Shutdown()` - System shutdown

---

### 2. AssetManager (Core Management)
**Location:** `Engine/Resources/Manager/`

**Files:**
- `AssetManager_Core.cs` - Initialization, disposal, core logic
- `AssetManager_Loader.cs` - Loading, caching, async operations
- `AssetManager_Registry.cs` - Registration and metadata
- `AssetManager_Validation.cs` - Health checks and optimization
- `AssetManager_Types.cs` - Supporting types and configuration
- `AssetManager_Config.cs` - Configuration management
- `AssetManager_Interfaces.cs` - Public interfaces
- `AssetManager_Enums.cs` - Enumerations
- `AssetLoader.cs` - Core loading implementation
- `AssetRegistryRuntime.cs` - Runtime registry
- `AssetType.cs` - Asset type definitions

**Responsibilities:**
- Asset lifecycle management
- Path resolution and root directory management
- Memory usage tracking
- Caching and lazy loading
- Type-safe asset loading

**Key Methods:**
- `LoadAsync<T>(path)` - Async asset loading
- `Load<T>(path)` - Synchronous asset loading
- `Unload(path)` - Asset unloading
- `GetMemoryStats()` - Memory statistics

---

### 3. AssetBundle (Packaged Assets)
**Location:** `Engine/Resources/Bundles/`

**Files:**
- `AssetBundle_Core.cs` - Core fields, constructors, disposal
- `AssetBundle_Loader.cs` - Bundle loading/unloading logic
- `AssetBundle_Metadata.cs` - Metadata structures
- `AssetBundle_Processors.cs` - Compression/encryption
- `AssetBundle_Validation.cs` - Integrity verification
- `AssetBundle_Interfaces.cs` - Public interfaces
- `AssetBundle_Accessors.cs` - Asset access helpers
- `AssetBundle_Types.cs` - Supporting types
- `AssetBundle_Enums.cs` - Enumerations
- `AssetBundle_Defaults.cs` - Default configurations

**Responsibilities:**
- Packaged asset distribution
- Bundle loading and streaming
- Metadata lookup and indexing
- Compression and decompression
- Asset retrieval from bundles

**Key Methods:**
- `LoadAsync()` - Async bundle loading
- `GetAsset<T>(path)` - Asset retrieval
- `Unload()` - Bundle unloading
- `GetMetadata()` - Metadata access

---

### 4. AssetPipeline (Processing Pipeline)
**Location:** `Engine/Resources/Pipeline/`

**Files:**
- `AssetPipeline_Core.cs` - Initialization, config, disposal
- `AssetPipeline_Import.cs` - Asset import logic
- `AssetPipeline_Processors.cs` - Processing operations
- `AssetPipeline_Output.cs` - Output generation
- `AssetPipeline_Discovery.cs` - Asset discovery
- `AssetPipeline_Validation.cs` - Validation and reporting
- `AssetPipeline_Types.cs` - Supporting types
- `AssetPipeline_Interfaces.cs` - Processor interfaces

**Responsibilities:**
- Raw asset processing and optimization
- Type-specific transformations
- Hash-based caching for change detection
- Batch processing operations
- Quality vs performance trade-offs

**Key Methods:**
- `ProcessAsset(path)` - Single asset processing
- `ProcessDirectory(path)` - Batch processing
- `UpdateConfig(type, config)` - Configuration updates
- `GetCacheStats()` - Cache statistics

**Asset Processors:**
- `TextureProcessor` - Texture compression and optimization
- `AudioProcessor` - Audio format conversion
- `ModelProcessor` - 3D model optimization
- `FontProcessor` - Font character set optimization
- `DataProcessor` - Data serialization

---

### 5. Specialized Loaders
**Location:** Various directories

#### Rendering Loaders
- `Engine/Rendering/CompositeImageLoader.cs` - Composite image loading
- `Engine/Rendering/Interfaces/ICompositeImageLoader.cs` - Interface
- `Engine/Resources/TextureLoader.cs` - Basic texture byte loading

#### UI Loaders
- `Engine/UI/Assets/UIAssetLoader.cs` - UI-specific asset loading
  - UIFont loading and caching
  - UISprite loading and caching
  - Texture byte array management
- `Engine/UI/StaticLayoutLoader.cs` - UI layout loading

#### Game Loaders
- `Engine/Waves/WaveLoader.cs` - Wave data loading
- `Engine/Resources/Types/DataLoader.cs` - Generic data loading
- `Engine/Resources/Types/TextureLoader.cs` - Texture type loading

---

### 6. Legacy Resource System
**Location:** `Engine/Resources/` (Root level)

**Files:**
- `RSInitializer.cs` - Resource initialization
- `RSRegistry.cs` - Resource registry
- `RSMetadata.cs` - Resource metadata
- `RSLoadResult.cs` - Load result tracking
- `RSBatchLoadResult.cs` - Batch load results
- `RSLoadContext.cs` - Load context
- `RSSource.cs` - Source tracking
- `RSKey.cs` - Resource keys
- `RSType.cs` - Resource types
- `RSUtils.cs` - Utility functions
- `RSValidation.cs` - Validation helpers
- `RSPipeline.cs` - Processing pipeline
- `RSDiscovery.cs` - Asset discovery
- `RSHandle.cs` - Resource handles
- `RSBundle.cs` - Bundle handling
- `TextureLoader.cs` - Texture loading
- `AssetMetadata.cs` - Asset metadata
- `AssetRegistry.cs` - Asset registry
- `ModernResourcePipeline.cs` - Modern pipeline implementation
- `ModernResourcePipelineExtensionsRuntime.cs` - Runtime extensions

**Responsibilities:**
- Legacy resource management
- Backward compatibility
- Migration path to new system

---

## Data Flow Diagrams

### Asset Loading Flow

```
User Request
    │
    ▼
AssetSystem.LoadAsset<T>(path)
    │
    ├─→ Check Bundle Cache
    │   │   Found? ──→ AssetBundle.GetAsset<T>()
    │   │                   │
    │   │                   ▼
    │   │              Return Asset
    │   │
    └─→ Check File System
        │   Found? ──→ AssetManager.Load<T>()
        │                   │
        │                   ├─→ Check Asset Cache
        │                   │   Found? ──→ Return Cached
        │                   │
        │                   └─→ Load from Disk
        │                           │
        │                           ├─→ Specialized Loader
        │                           │   (TextureLoader, UIAssetLoader, etc.)
        │                           │
        │                           └─→ Cache Asset
        │
        └─→ Asset Not Found ──→ Throw Exception
```

### Asset Processing Flow

```
Raw Asset (Assets/Raw/)
    │
    ▼
AssetPipeline.ProcessAsset(path)
    │
    ├─→ Calculate Hash
    │   │
    │   └─→ Check Cache
    │       Cached? ──→ Skip Processing
    │
    ├─→ Select Processor
    │   (TextureProcessor, AudioProcessor, etc.)
    │
    ├─→ Process Asset
    │   │
    │   ├─→ Transform
    │   ├─→ Compress
    │   ├─→ Optimize
    │   └─→ Validate
    │
    ├─→ Write to Output (Assets/Processed/)
    │
    └─→ Update Cache
```

### Bundle Creation Flow

```
Processed Assets (Assets/Processed/)
    │
    ▼
AssetBundle.Create(bundlePath)
    │
    ├─→ Collect Assets
    │
    ├─→ Generate Metadata
    │   • Asset names
    │   • File sizes
    │   • Offsets
    │   • Hashes
    │
    ├─→ Compress Assets
    │
    ├─→ Write Bundle File
    │   • Header
    │   • Metadata Table
    │   • Asset Data
    │
    └─→ Validate Bundle
```

---

## Integration Points

### Rendering System Integration
```
Rendering System
    │
    ├─→ AssetSystem.LoadTexture()
    │       │
    │       └─→ AssetManager → TextureLoader → CompositeImageLoader
    │
    └─→ AssetSystem.LoadShader()
            │
            └─→ AssetManager → ShaderLoader
```

### UI System Integration
```
UI System
    │
    ├─→ UIAssetLoader.LoadFont()
    │       │
    │       └─→ AssetSystem → AssetManager → FontLoader
    │
    ├─→ UIAssetLoader.LoadSprite()
    │       │
    │       └─→ AssetSystem → AssetManager → TextureLoader
    │
    └─→ StaticLayoutLoader.LoadLayout()
            │
            └─→ AssetSystem → AssetManager → DataLoader
```

### Audio System Integration
```
Audio System
    │
    └─→ AssetSystem.LoadAudio()
            │
            └─→ AssetManager → AudioLoader → AudioProcessor
```

---

## File Structure Summary

```
Engine/Resources/
├── Integration/              # System orchestration
│   ├── AssetIntegration_Core.cs
│   ├── AssetIntegration_Bundles.cs
│   └── AssetIntegration_Pipeline.cs
│
├── Manager/                 # Core asset management
│   ├── AssetManager_Core.cs
│   ├── AssetManager_Loader.cs
│   ├── AssetManager_Registry.cs
│   ├── AssetManager_Validation.cs
│   ├── AssetManager_Types.cs
│   ├── AssetManager_Config.cs
│   ├── AssetManager_Interfaces.cs
│   ├── AssetManager_Enums.cs
│   ├── AssetLoader.cs
│   ├── AssetRegistryRuntime.cs
│   └── AssetType.cs
│
├── Bundles/                 # Bundle system
│   ├── AssetBundle_Core.cs
│   ├── AssetBundle_Loader.cs
│   ├── AssetBundle_Metadata.cs
│   ├── AssetBundle_Processors.cs
│   ├── AssetBundle_Validation.cs
│   ├── AssetBundle_Interfaces.cs
│   ├── AssetBundle_Accessors.cs
│   ├── AssetBundle_Types.cs
│   ├── AssetBundle_Enums.cs
│   └── AssetBundle_Defaults.cs
│
├── Pipeline/                # Processing pipeline
│   ├── AssetPipeline_Core.cs
│   ├── AssetPipeline_Import.cs
│   ├── AssetPipeline_Processors.cs
│   ├── AssetPipeline_Output.cs
│   ├── AssetPipeline_Discovery.cs
│   ├── AssetPipeline_Validation.cs
│   ├── AssetPipeline_Types.cs
│   └── AssetPipeline_Interfaces.cs
│
├── Types/                   # Specialized loaders
│   ├── DataLoader.cs
│   └── TextureLoader.cs
│
└── [Legacy RS* files]       # Legacy resource system

Engine/Rendering/
├── CompositeImageLoader.cs
└── Interfaces/
    └── ICompositeImageLoader.cs

Engine/UI/Assets/
├── UIAssetLoader.cs
└── StaticLayoutLoader.cs

Engine/Waves/
└── WaveLoader.cs
```

---

## Key Relationships

### AssetSystem → AssetManager
- AssetSystem uses AssetManager for all asset operations
- AssetManager provides the core loading, caching, and registry functionality
- AssetSystem coordinates initialization and shutdown

### AssetSystem → AssetBundle
- AssetSystem uses AssetBundle for packaged asset loading
- AssetBundle provides efficient asset retrieval from bundle files
- Integration handled in AssetIntegration_Bundles.cs

### AssetSystem → AssetPipeline
- AssetSystem uses AssetPipeline for asset processing
- AssetPipeline provides raw asset transformation and optimization
- Integration handled in AssetIntegration_Pipeline.cs

### AssetManager → Specialized Loaders
- AssetManager delegates to specialized loaders based on asset type
- Loaders provide type-specific loading logic
- Examples: TextureLoader, UIAssetLoader, WaveLoader

### AssetPipeline → Asset Processors
- AssetPipeline uses processors for type-specific transformations
- Processors implement IAssetProcessor interface
- Examples: TextureProcessor, AudioProcessor, ModelProcessor

---

## Performance Characteristics

### Caching Strategy
- **AssetManager**: Weak reference caching for automatic memory management
- **AssetPipeline**: Hash-based caching to prevent redundant processing
- **AssetBundle**: Metadata caching for fast asset lookup

### Async Operations
- AssetManager supports async loading with progress tracking
- AssetBundle supports async loading/unloading
- AssetPipeline supports parallel batch processing

### Memory Management
- Automatic garbage collection integration
- Memory statistics tracking
- Configurable cache limits
- Efficient streaming for large assets

---

## Usage Examples

### Basic Asset Loading
```csharp
// Initialize system
AssetSystem.Initialize("Assets");

// Load texture
var texture = AssetSystem.LoadAsset<Texture2D>("textures/player.png");

// Load audio
var audio = AssetSystem.LoadAsset<AudioClip>("sounds/explosion.wav");

// Shutdown
AssetSystem.Shutdown();
```

### Bundle Loading
```csharp
// Initialize with bundle
AssetSystem.Initialize("Assets", "Assets/Bundles/game.bundle");

// Load from bundle
var texture = AssetSystem.LoadAsset<Texture2D>("ui/icon.png");
```

### Asset Processing
```csharp
// Initialize pipeline
var pipeline = new AssetPipeline("Assets/Raw", "Assets/Processed");

// Process single asset
pipeline.ProcessAsset("textures/player.png");

// Process directory
pipeline.ProcessDirectory("textures/");

// Update configuration
pipeline.UpdateConfig(AssetType.Texture, 
    new AssetProcessorConfig { Quality = 0.9f });
```

---

## Summary

The SAS Zombie Assault TD asset system is a comprehensive, multi-layered architecture designed for efficient asset management, processing, and distribution. The system is organized into three main subsystems:

1. **AssetManager** - Core asset lifecycle management
2. **AssetBundle** - Packaged asset distribution
3. **AssetPipeline** - Raw asset processing and optimization

These subsystems are coordinated by the **AssetSystem** integration layer, which provides a unified public API. Specialized loaders handle type-specific asset loading, while asset processors handle type-specific transformations.

The system supports both file-based and bundle-based asset loading, with comprehensive caching, async operations, and memory management features.
