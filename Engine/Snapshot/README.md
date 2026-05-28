# Snapshot System Documentation

## Overview

The SAS Zombie Assault TD snapshot system provides deterministic, audit-friendly save/load functionality for game states. It captures complete game state snapshots with validation, integrity checking, and drift-proof storage.

## Features

- **Deterministic ID Generation**: SHA256-based unique identifiers
- **Integrity Validation**: Checksum verification and data validation
- **Memory & Disk Storage**: Hybrid caching with persistent storage
- **Audit Trail**: Complete metadata and validation tracking
- **Command Interface**: Console commands for snapshot management
- **Error Handling**: Comprehensive error reporting and recovery

## Architecture

### Core Components

1. **SnapshotData**: Immutable snapshot data structure
2. **SnapshotManager**: Core management and operations
3. **SnapshotCommands**: Console command interface
4. **SnapshotIntegration**: Engine integration point

### Data Flow

```
Game State → CreateSnapshot → Validate → Save → Load → ApplySnapshot → Game State
```

## Usage

### Initialization

```csharp
// During engine startup
SnapshotIntegration.Initialize("E:\\Projects\\SASZombieAssaultTD\\Snapshots");
```

### Creating Snapshots

```csharp
// Manual snapshot
var snapshot = SnapshotIntegration.Instance.CreateSnapshot(
    PlayerSystem.Instance.State,
    waveNumber: 5,
    gameTime: 120.5f
);

// Quick snapshot with auto-generated ID
string snapshotId = SnapshotIntegration.QuickSnapshot("before_boss");
```

### Loading Snapshots

```csharp
var snapshot = SnapshotIntegration.Instance.LoadSnapshot("snap_abc123");
if (snapshot != null)
{
    bool success = SnapshotIntegration.Instance.ApplySnapshot(snapshot, out string error);
    if (success)
    {
        Console.WriteLine("Snapshot applied successfully");
    }
}
```

### Console Commands

```
snapshot_create [name]    - Create snapshot of current state
snapshot_list            - List all snapshots
snapshot_load <id>       - Load and apply snapshot
snapshot_delete <id>     - Delete snapshot
snapshot_validate <id>   - Validate snapshot integrity
snapshot_help            - Show help
```

## File Structure

Snapshots are stored as JSON files in the configured directory:

```
Snapshots/
├── snap_abc123def456.json
├── snap_789ghi012jkl.json
└── snap_345mno678pqr.json
```

### Snapshot File Format

```json
{
  "id": "snap_abc123def456",
  "timestamp": "2024-04-24T18:30:00.000Z",
  "gameVersion": "1.0.0",
  "playerState": {
    "cash": 1500,
    "lives": 20,
    "score": 5000,
    "waveNumber": 5,
    "isGameOver": false,
    "isPaused": false
  },
  "waveNumber": 5,
  "gameTime": 120.5,
  "customData": {
    "context": "before_boss",
    "auto": true
  },
  "metadata": {
    "checksum": "ABC123...",
    "formatVersion": 1,
    "isValid": true,
    "validationErrors": [],
    "creationContext": "Manual"
  }
}
```

## Validation Rules

### Required Fields
- ID must be non-null and non-empty
- Timestamp must be valid
- PlayerState must be non-null
- Cash, Lives, WaveNumber, GameTime must be non-negative

### Integrity Checks
- Checksum verification for data corruption detection
- Format version compatibility
- Data type validation

### Error Handling
- Graceful degradation for corrupted files
- Detailed error reporting
- Automatic cleanup of invalid snapshots

## Performance Considerations

### Memory Management
- Snapshots are cached in memory for fast access
- Deep cloning prevents state corruption
- Automatic cleanup of old snapshots

### Disk I/O
- Asynchronous save operations (future enhancement)
- Compression for large snapshots (future enhancement)
- Atomic file operations for data safety

## Security Considerations

### Data Integrity
- SHA256 checksums prevent tampering
- Validation on load prevents corruption
- Read-only snapshots prevent accidental modification

### File Access
- Snapshots stored in isolated directory
- No external dependencies or network access
- Local file system only

## Testing

### Unit Tests
- Snapshot creation and validation
- Checksum calculation and verification
- File save/load operations
- Error handling scenarios

### Integration Tests
- Full snapshot lifecycle
- Console command interface
- Engine integration points
- Performance under load

## Future Enhancements

### Planned Features
- Compression for storage efficiency
- Differential snapshots for smaller files
- Auto-save at key game events
- Snapshot comparison and diff tools
- Export/import functionality

### Performance Improvements
- Async I/O operations
- Memory pool for frequent operations
- Background cleanup tasks
- Lazy loading for large snapshot lists

## Troubleshooting

### Common Issues

1. **"Snapshot system not initialized"**
   - Call `SnapshotIntegration.Initialize()` during startup

2. **"Snapshot validation failed"**
   - Check validation error messages
   - Verify file integrity with checksum

3. **"Player system not available"**
   - Ensure PlayerSystem is initialized before snapshots

4. **"Failed to save snapshot"**
   - Check directory permissions
   - Verify disk space availability

### Debug Information

Enable debug logging to troubleshoot issues:

```csharp
ModernLog.SetLevel(LogLevel.Debug);
```

Debug logs include:
- Snapshot creation details
- Validation results
- File operation status
- Error stack traces

## API Reference

### SnapshotManager

#### Methods
- `CreateSnapshot()` - Create new snapshot
- `SaveSnapshot()` - Save to disk
- `LoadSnapshot()` - Load from disk
- `ApplySnapshot()` - Apply to game state
- `ListSnapshots()` - List available snapshots
- `DeleteSnapshot()` - Remove snapshot
- `ValidateSnapshot()` - Check integrity

### SnapshotData

#### Properties
- `Id` - Unique identifier
- `Timestamp` - Creation time
- `GameVersion` - Engine version
- `PlayerState` - Player game state
- `WaveNumber` - Current wave
- `GameTime` - Elapsed time
- `CustomData` - Additional data
- `Metadata` - Validation metadata

### SnapshotCommands

#### Commands
- `CreateSnapshot()` - Console create command
- `ListSnapshots()` - Console list command
- `LoadSnapshot()` - Console load command
- `DeleteSnapshot()` - Console delete command
- `ValidateSnapshot()` - Console validate command
- `ShowHelp()` - Console help command
