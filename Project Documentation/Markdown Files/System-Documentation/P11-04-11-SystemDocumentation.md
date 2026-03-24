# P11-04-11 Persistence System Documentation

## Overview

P11-04-11 implements a comprehensive save/load persistence system for SAS Zombie Assault TD. The system provides complete game state serialization, multiple save slots, versioning support, and robust error handling with audit-friendly logging throughout.

## Architecture

### Core Components

1. **SaveManager.cs** - Main entry point for save/load operations
2. **SaveGameData.cs** - Complete game state data container
3. **SaveSerializer.cs** - JSON and binary serialization with version validation
4. **LoadSystem.cs** - World state restoration and entity recreation
5. **GameLoadedEvent.cs** - Event published after successful load operations

### Enhanced Components

1. **StatsComponent.cs** - Added ToSaveData() and FromSaveData() methods
2. **HealthComponent.cs** - Added ToSaveData() and FromSaveData() methods
3. **TransformComponent.cs** - Added ToSaveData() and FromSaveData() methods
4. **RoundResetSystem.cs** - Added ToSaveData() and FromSaveData() methods
5. **UISystem.cs** - Added save/load dialog and notification methods

## System Details

### SaveManager.cs

**Purpose**: Main save system entry point for game persistence operations.

**Key Features**:
- Multi-slot save support (10 slots)
- JSON and binary serialization
- File integrity verification with checksums
- Atomic file operations
- Comprehensive error handling
- Save file metadata management

**Public API**:
```csharp
bool SaveGame(int saveSlotId)
bool LoadGame(int saveSlotId)
bool HasSaveFile(int saveSlotId)
SaveFileInfo GetSaveFileInfo(int saveSlotId)
List<SaveFileInfo> GetAllSaveFilesInfo()
bool DeleteSaveFile(int saveSlotId)
```

**Serialization Logic**:
1. Collect player entity data (position, health, stats, inventory)
2. Collect system state (round, score, resources, difficulty)
3. Collect persistent world entities (enemies, spawners)
4. Validate all collected data
5. Serialize to JSON with version information
6. Write atomically with checksum verification

**Versioning Strategy**:
- Current version: "1.0.0"
- Major version must match for compatibility
- Minor version differences generate warnings
- Version validation during load operations

**Event Flow**:
- Save operations: Direct file I/O with logging
- Load operations: Publishes GameLoadedEvent on success
- Error handling: Comprehensive logging with context

### SaveGameData.cs

**Purpose**: Complete game save data container with full serializability.

**Data Structure**:
```csharp
public class SaveGameData
{
    public string SaveVersion { get; set; }
    public DateTime Timestamp { get; set; }
    public PlayerStatsData PlayerStats { get; set; }
    public PointF PlayerPosition { get; set; }
    public PointF PlayerRespawnPosition { get; set; }
    public float PlayerCurrentHealth { get; set; }
    public float PlayerMaxHealth { get; set; }
    public int CurrentRound { get; set; }
    public int CurrentWave { get; set; }
    public float DifficultyMultiplier { get; set; }
    public long TotalScore { get; set; }
    public int PlayerResources { get; set; }
    public List<InventoryItemData> Inventory { get; set; }
    public List<PersistentEntityData> PersistentEntities { get; set; }
    public Dictionary<string, object> CustomData { get; set; }
}
```

**Serialization Logic**:
- All properties marked with [Serializable] attribute
- JSON serialization with camelCase naming
- Binary serialization for compact storage
- Validation methods for data integrity

**Versioning Strategy**:
- SaveVersion field tracks file format version
- Backward compatibility through selective field restoration
- Forward compatibility through CustomData dictionary

### SaveSerializer.cs

**Purpose**: Handles JSON and binary serialization with version compatibility.

**Key Features**:
- JSON serialization with proper formatting
- Binary serialization for compact storage
- Version compatibility validation
- Atomic file operations
- Checksum calculation for integrity verification
- Comprehensive error handling

**Public API**:
```csharp
string SerializeToJson(SaveGameData saveData)
SaveGameData DeserializeFromJson(string json)
byte[] SerializeToBinary(SaveGameData saveData)
SaveGameData DeserializeFromBinary(byte[] binaryData)
void WriteToJsonFile(SaveGameData saveData, string filePath)
SaveGameData ReadFromJsonFile(string filePath)
void WriteToBinaryFile(SaveGameData saveData, string filePath)
SaveGameData ReadFromBinaryFile(string filePath)
string CalculateChecksum(SaveGameData saveData)
```

**Serialization Logic**:
- JSON: System.Text.Json with custom options
- Binary: BinaryFormatter for maximum compatibility
- Files: Atomic operations with temporary files
- Checksums: SHA256 for integrity verification

**Versioning Strategy**:
- Major version must match exactly
- Minor version differences allowed with warnings
- Validation before deserialization
- Clear error messages for incompatibility

### LoadSystem.cs

**Purpose**: Loads SaveGameData and applies it to the ECS world with comprehensive state restoration.

**Key Features**:
- World state preparation and cleanup
- Player state restoration
- System state restoration
- Persistent entity recreation
- Progress tracking and error handling
- GameLoadedEvent publishing

**Public API**:
```csharp
bool LoadGame(SaveGameData saveData, int saveSlotId)
bool IsLoading { get; }
float LoadProgress { get; }
```

**Restoration Logic**:
1. **Phase 1**: Prepare world (pause systems, clear entities)
2. **Phase 2**: Load player state (position, health, stats, inventory)
3. **Phase 3**: Load system state (round, score, resources)
4. **Phase 4**: Load persistent entities (enemies, spawners)
5. **Phase 5**: Finalize and publish GameLoadedEvent

**State Restoration**:
- Player: Transform, Health, Stats components
- Systems: RoundReset, Score, Economy systems
- Entities: Enemies, spawners with full state
- World: Proper entity relationships and references

### GameLoadedEvent.cs

**Purpose**: Event published when a game is successfully loaded from save data.

**Data Structure**:
```csharp
public class GameLoadedEvent
{
    public int SaveSlotId { get; set; }
    public DateTime LoadTimestamp { get; set; }
    public string SaveVersion { get; set; }
    public DateTime SaveTimestamp { get; set; }
    public int RestoredRound { get; set; }
    public int RestoredWave { get; set; }
    public long RestoredScore { get; set; }
    public int RestoredResources { get; set; }
    public int RestoredKills { get; set; }
    public int RestoredDeaths { get; set; }
    public int RestoredEntityCount { get; set; }
    public int RestoredItemCount { get; set; }
    public string LoadContext { get; set; }
    public List<string> LoadWarnings { get; set; }
    public Dictionary<string, object> CustomLoadData { get; set; }
    public bool LoadSuccessful { get; set; }
    public long LoadDurationMs { get; set; }
}
```

**Event Flow**:
- Published by LoadSystem after successful load
- Contains comprehensive load summary
- Includes warnings and custom data
- Systems can subscribe to react to load events

## Component Enhancements

### StatsComponent.cs

**New Methods**:
```csharp
public PlayerStatsData ToSaveData()
public void FromSaveData(PlayerStatsData saveData)
```

**Save Logic**:
- Exports all statistics to PlayerStatsData
- Converts enum keys to strings for serialization
- Preserves timestamps and counters

**Load Logic**:
- Restores statistics with validation
- Converts string keys back to enums
- Partial restoration support (only updates present fields)
- Preserves existing data for missing fields

### HealthComponent.cs

**New Methods**:
```csharp
public object ToSaveData()
public void FromSaveData(object saveData)
```

**Save Logic**:
- Exports current health, max health, invincibility state
- Includes auto-heal settings
- Uses anonymous object for flexibility

**Load Logic**:
- Reflection-based property restoration
- Validates health ranges and constraints
- Resets damage tracking on load
- Partial restoration support

### TransformComponent.cs

**New Methods**:
```csharp
public object ToSaveData()
public void FromSaveData(object saveData)
```

**Save Logic**:
- Exports position, rotation, scale, origin
- Includes respawn position data
- Uses anonymous object for flexibility

**Load Logic**:
- Reflection-based property restoration
- Validates scale values (> 0)
- Preserves coordinate precision
- Partial restoration support

### RoundResetSystem.cs

**New Methods**:
```csharp
public object ToSaveData()
public void FromSaveData(object saveData)
```

**Save Logic**:
- Exports current round and system settings
- Includes current round state with progress
- Preserves transition and reset settings
- Uses nested anonymous objects

**Load Logic**:
- Reflection-based property restoration
- Complex round state restoration
- Preserves system configuration
- Partial restoration support

### UISystem.cs

**New Methods**:
```csharp
public void ShowSaveConfirmationDialog(int saveSlotId, string saveSummary)
public void ShowLoadConfirmationDialog(int saveSlotId, string saveSummary)
public void ShowSaveSlotSelectionDialog(List<SaveFileInfo> availableSaves, bool isSaving)
public void ShowIncompatibleSaveVersionDialog(string saveVersion, string currentVersion, string errorMessage)
public void ShowSaveLoadProgressDialog(string operation, float progress, string message)
public void HideSaveLoadDialogs()
public void ShowSaveCompletedNotification(int saveSlotId, string saveSummary)
public void ShowLoadCompletedNotification(int saveSlotId, string loadSummary)
```

**UI Features**:
- Save/load confirmation dialogs
- Save slot selection with file information
- Version compatibility error dialogs
- Progress dialogs for long operations
- Completion notifications
- Proper UI layering with UIElementBase

## Error Handling

### Save Operations
- File I/O errors with detailed logging
- Serialization error handling
- Atomic operations to prevent corruption
- Checksum verification for integrity
- Graceful degradation on partial failures

### Load Operations
- Version compatibility validation
- Data validation before restoration
- Entity creation error handling
- Rollback on partial failures
- Comprehensive error reporting

### UI Operations
- Dialog creation error handling
- User input validation
- Progress tracking with cancellation
- Notification system integration
- Accessibility considerations

## Performance Considerations

### Save Performance
- Efficient data collection algorithms
- Minimal memory allocations
- Asynchronous file operations support
- Compression for binary saves
- Incremental save optimization potential

### Load Performance
- Progress tracking for large saves
- Entity pooling for recreation
- Lazy loading for optional data
- Background loading support
- Memory usage optimization

## Security Considerations

### Save File Security
- Checksum verification for integrity
- Version validation for compatibility
- Path traversal prevention
- File size limits
- Corruption detection

### Data Validation
- Range validation for all numeric values
- Null reference protection
- Type safety enforcement
- Buffer overflow prevention
- Malformed data handling

## Testing Considerations

### Unit Testing
- Individual component save/load methods
- Serialization/deserialization accuracy
- Version compatibility scenarios
- Error condition handling
- Edge case validation

### Integration Testing
- End-to-end save/load workflows
- Multi-slot operations
- Concurrent access scenarios
- File system error simulation
- UI interaction testing

### Performance Testing
- Large save file handling
- Memory usage profiling
- Load time benchmarks
- Save time optimization
- Concurrent operation testing

## Future Enhancements

### Potential Features
- Cloud save integration
- Auto-save functionality
- Save file compression
- Differential saves
- Save file encryption
- Save file sharing
- Achievement integration
- Statistics tracking

### Architecture Improvements
- Plugin-based save system
- Custom serialization providers
- Save file migration tools
- Backup and recovery system
- Save file analysis tools

## Audit Trail

All systems include comprehensive audit-friendly logging:
- Timestamped operation logs
- Detailed error reporting
- Performance metrics
- Data validation results
- User action tracking
- System state changes

This documentation provides complete coverage of the P11-04-11 persistence system implementation, including all serialization logic, versioning strategies, event flows, and XML documentation requirements.
