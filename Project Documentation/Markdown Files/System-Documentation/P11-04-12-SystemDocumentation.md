# P11-04-12 Inventory and Resource Systems Documentation

## Overview

P11-04-12 implements a comprehensive inventory and resource management system for SAS Zombie Assault TD. This system provides item storage, resource tracking, UI integration, and event-driven architecture for all inventory and economy-related operations.

## System Architecture

### Core Components

#### 1. ItemDefinition.cs
**Location:** `Engine/Systems/Inventory/ItemDefinition.cs`

**Purpose:** Defines static metadata for item types including properties, rarity, and behavior.

**Key Features:**
- **Item Metadata:** ID, Name, Description, Rarity, MaxStack
- **Economic Properties:** BaseValue, IsUsable, IsSellable, IsTradable
- **Serialization Support:** ToSaveData() and FromSaveData() methods
- **JSON Serialization:** Full support for save/load operations

**Public API:**
```csharp
public class ItemDefinition
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public ItemRarity Rarity { get; set; }
    public int MaxStack { get; set; }
    public int BaseValue { get; set; }
    public bool IsUsable { get; set; }
    public bool IsSellable { get; set; }
    public bool IsTradable { get; set; }
    public string IconPath { get; set; }
    
    public ItemDefinitionSaveData ToSaveData();
    public static ItemDefinition FromSaveData(ItemDefinitionSaveData saveData);
}
```

**Serialization Logic:**
- Uses `ItemDefinitionSaveData` for JSON serialization
- Preserves all item properties for save/load operations
- Supports both embedded defaults and external JSON loading

#### 2. ItemInstance.cs
**Location:** `Engine/Systems/Inventory/ItemInstance.cs`

**Purpose:** Represents individual item stacks with quantity, durability, and metadata.

**Key Features:**
- **Stack Management:** Quantity tracking with max stack limits
- **Durability System:** Optional durability tracking (0-100 scale)
- **Custom Metadata:** Dictionary for enchantments, properties, etc.
- **Stack Operations:** Merge, split, and transfer functionality
- **Unique Instance IDs:** GUID-based identification

**Public API:**
```csharp
public class ItemInstance
{
    public string InstanceId { get; }
    public string ItemDefinitionId { get; }
    public int Quantity { get; }
    public float Durability { get; }
    public bool HasDurability { get; }
    public Dictionary<string, object> CustomMetadata { get; }
    public DateTime CreationTime { get; }
    
    public void SetQuantity(int newQuantity);
    public int AddQuantity(int amount);
    public int RemoveQuantity(int amount);
    public void SetDurability(float newDurability);
    public bool ReduceDurability(float amount);
    public void Repair();
    public bool CanMergeWith(ItemInstance other);
    public int MergeFrom(ItemInstance other, int maxStack);
    public ItemInstance Split(int splitAmount);
    public void SetCustomMetadata(string key, object value);
    public object GetCustomMetadata(string key);
    
    public ItemInstanceSaveData ToSaveData();
    public static ItemInstance FromSaveData(ItemInstanceSaveData saveData);
}
```

**Stack Operations:**
- **Merge:** Combines compatible stacks respecting max stack limits
- **Split:** Creates new stack from existing quantity
- **Transfer:** Moves items between inventories with validation

#### 3. InventoryComponent.cs
**Location:** `Engine/Components/InventoryComponent.cs`

**Purpose:** ECS component that stores inventory data for entities.

**Key Features:**
- **Slot Management:** Configurable maximum slots
- **Stack Handling:** Automatic merging and overflow management
- **Query Operations:** HasItem(), GetItemCount(), GetItems()
- **Capacity Enforcement:** Prevents overfull inventories
- **ECS Integration:** Compatible with entity system

**Public API:**
```csharp
public class InventoryComponent
{
    public List<ItemInstance> Items { get; }
    public int MaxSlots { get; set; }
    public int UsedSlots { get; }
    public bool IsEmpty { get; }
    public bool IsFull { get; }
    
    public InventoryAddResult AddItem(ItemInstance itemInstance, ItemDatabase itemDatabase);
    public InventoryRemoveResult RemoveItem(string itemDefinitionId, int quantity);
    public bool HasItem(string itemDefinitionId, int minimumQuantity = 1);
    public int GetItemCount(string itemDefinitionId);
    public List<ItemInstance> GetItems(string itemDefinitionId);
    public ItemInstance GetItemByInstanceId(string instanceId);
    public void Clear();
    public Dictionary<string, int> GetItemSummary();
    
    public InventoryComponentSaveData ToSaveData();
    public static InventoryComponent FromSaveData(InventoryComponentSaveData saveData);
}
```

**Inventory Operations:**
- **AddItem:** Handles stack merging and overflow with detailed results
- **RemoveItem:** Removes specific quantities with validation
- **Query Methods:** Multiple ways to check and retrieve item information

#### 4. InventorySystem.cs
**Location:** `Engine/Systems/Gameplay/InventorySystem.cs`

**Purpose:** Manages inventory operations and coordinates with other systems.

**Key Features:**
- **Event-Driven:** Subscribes to ItemPickupEvent and ItemUseEvent
- **Operation Management:** Pickup, drop, use, and transfer operations
- **Validation:** Enforces inventory rules and capacity limits
- **Audit Logging:** Comprehensive logging for all operations

**Public API:**
```csharp
public class InventorySystem
{
    public InventorySystem(EntityManager entityManager, EventManager eventManager, ItemDatabase itemDatabase);
    
    public InventoryComponent GetInventory(Entity entity);
    public InventoryComponent AddInventoryToEntity(Entity entity, int maxSlots = 20);
    public bool DropItem(int entityId, string itemDefinitionId, int quantity, string instanceId = null);
    public bool TransferItem(int fromEntityId, int toEntityId, string itemDefinitionId, int quantity);
    public void Dispose();
}
```

**Event Handling:**
- **ItemPickupEvent:** Adds items to inventory with stack management
- **ItemUseEvent:** Consumes usable items and publishes removal events

#### 5. ResourceSystem.cs
**Location:** `Engine/Systems/Gameplay/ResourceSystem.cs`

**Purpose:** Tracks and manages player resources (gold, credits, etc.).

**Key Features:**
- **Resource Types:** Gold, Credits, Scrap, Energy, Experience, Reputation
- **Transaction Support:** Add, spend, transfer operations
- **Validation:** Ensures sufficient resources for operations
- **Event Publishing:** ResourceChangedEvent for all modifications

**Public API:**
```csharp
public class ResourceSystem
{
    public ResourceSystem(EntityManager entityManager, EventManager eventManager);
    
    public float GetResource(int entityId, ResourceType resourceType);
    public Dictionary<ResourceType, float> GetAllResources(int entityId);
    public bool AddResource(int entityId, ResourceType resourceType, float amount, string source = null);
    public bool SpendResource(int entityId, ResourceType resourceType, float amount, string reason = null);
    public bool HasEnough(int entityId, ResourceType resourceType, float requiredAmount);
    public bool SetResource(int entityId, ResourceType resourceType, float amount, string source = null);
    public bool TransferResource(int fromEntityId, int toEntityId, ResourceType resourceType, float amount);
    public void ClearResources(int entityId);
    
    public ResourceSystemSaveData ToSaveData();
    public static ResourceSystem FromSaveData(ResourceSystemSaveData saveData, EntityManager entityManager, EventManager eventManager);
}
```

**Resource Management:**
- **Type Safety:** Enum-based resource types prevent errors
- **Transaction Safety:** All operations are validated before execution
- **Event Integration:** Automatic event publishing for UI updates

#### 6. ItemDatabase.cs
**Location:** `Engine/Systems/Inventory/ItemDatabase.cs`

**Purpose:** Central registry of all item definitions with loading and management capabilities.

**Key Features:**
- **Item Registry:** Dictionary-based fast lookup
- **Search Capabilities:** Name, description, and rarity filtering
- **Persistence:** JSON file loading and saving
- **Default Items:** Built-in fallback item definitions

**Public API:**
```csharp
public class ItemDatabase
{
    public ItemDatabase(string dataFilePath = null);
    public int Count { get; }
    
    public ItemDefinition GetItemDefinition(string itemId);
    public IReadOnlyCollection<ItemDefinition> GetAllItems();
    public IReadOnlyCollection<ItemDefinition> GetItemsByRarity(ItemRarity rarity);
    public IReadOnlyCollection<ItemDefinition> GetUsableItems();
    public bool RegisterItem(ItemDefinition itemDefinition);
    public bool UpdateItem(ItemDefinition itemDefinition);
    public bool RemoveItem(string itemId);
    public bool HasItem(string itemId);
    public IReadOnlyCollection<ItemDefinition> SearchItemsByName(string searchTerm);
    public IReadOnlyCollection<ItemDefinition> SearchItemsByDescription(string searchTerm);
    public bool LoadFromFile(string filePath = null);
    public bool SaveToFile(string filePath = null);
    public void Clear();
}
```

**Database Operations:**
- **CRUD Operations:** Complete create, read, update, delete support
- **Search Functions:** Multiple search methods for item discovery
- **File I/O:** JSON-based persistence with error handling

## Event System

### Event Types

#### 1. ItemPickupEvent
**Location:** `Engine/Systems/Events/ItemPickupEvent.cs`

**Purpose:** Fired when an entity picks up an item.

**Properties:**
- EntityId, ItemDefinitionId, Quantity
- Position, SourceEntityId, PickupReason
- Durability, CustomMetadata, Timestamp

#### 2. ItemUseEvent
**Location:** `Engine/Systems/Events/ItemUseEvent.cs`

**Purpose:** Fired when an entity uses an item.

**Properties:**
- EntityId, ItemDefinitionId, Quantity
- TargetEntityId, TargetPosition
- WasSuccessful, UseResult, FailureReason

#### 3. ItemAddedEvent
**Location:** `Engine/Systems/Events/ItemAddedEvent.cs`

**Purpose:** Fired when an item is successfully added to inventory.

**Properties:**
- EntityId, ItemDefinitionId, Quantity
- Source, SourceEntityId, WasPartial
- RemainingQuantity, TotalQuantity, SlotsOccupied

#### 4. ItemRemovedEvent
**Location:** `Engine/Systems/Events/ItemRemovedEvent.cs`

**Purpose:** Fired when an item is removed from inventory.

**Properties:**
- EntityId, ItemDefinitionId, Quantity
- Reason, TargetEntityId, WasVoluntary
- ValueReceived, ValueType, RemainingQuantity

#### 5. ResourceChangedEvent
**Location:** `Engine/Systems/Events/ResourceChangedEvent.cs`

**Purpose:** Fired when an entity's resources change.

**Properties:**
- EntityId, ResourceType, OldAmount, NewAmount
- ChangeAmount, Source, SourceEntityId
- IsGain, IsLoss, IsDepleted, Context

## UI Integration

### UI Components

#### 1. InventoryPanelRenderer
**Location:** `Engine/Systems/UI/InventoryPanelRenderer.cs`

**Purpose:** Renders grid-based inventory panel with item icons and stack counts.

**Features:**
- Grid-based inventory display
- Item icons and stack counts
- Drag/drop support (framework)
- UIElementBase integration
- Proper layering above HUD

#### 2. ResourceDisplayRenderer
**Location:** `Engine/Systems/UI/ResourceDisplayRenderer.cs`

**Purpose:** Displays player resources with icons and animated counters.

**Features:**
- Multiple resource type support
- Animated counter transitions
- Resource change notifications
- Configurable visibility
- HUD integration

#### 3. ItemTooltipRenderer
**Location:** `Engine/Systems/UI/ItemTooltipRenderer.cs`

**Purpose:** Shows detailed item information on hover.

**Features:**
- Dynamic positioning
- Rarity-based coloring
- Item stat display
- Screen edge avoidance
- Top-layer rendering

### UISystem Integration

**Updated Methods:**
```csharp
// Inventory Management
public void ShowInventoryPanel(int entityId, object inventoryData);
public void HideInventoryPanel();
public void UpdateInventoryPanel(object inventoryData);

// Resource Display
public void UpdateResourceDisplay(object resourceData);
public void SetResourceDisplayVisibility(bool isVisible);

// Tooltips
public void ShowItemTooltip(object itemData, Vector2 position);
public void HideItemTooltip();
public void UpdateItemTooltip(object itemData, Vector2? position = null);
```

**Rendering Order:**
1. Health bars (background)
2. Score display, Kill feed
3. Resource display
4. Inventory panel
5. Game lifecycle UI
6. Tooltips (foreground)

## Serialization

### Save Data Structures

#### ItemDefinitionSaveData
```csharp
public class ItemDefinitionSaveData
{
    public string Id;
    public string Name;
    public string Description;
    public ItemRarity Rarity;
    public int MaxStack;
    public int BaseValue;
    public bool IsUsable;
    public bool IsSellable;
    public bool IsTradable;
    public string IconPath;
}
```

#### ItemInstanceSaveData
```csharp
public class ItemInstanceSaveData
{
    public string InstanceId;
    public string ItemDefinitionId;
    public int Quantity;
    public float Durability;
    public bool HasDurability;
    public Dictionary<string, object> CustomMetadata;
    public DateTime CreationTime;
}
```

#### InventoryComponentSaveData
```csharp
public class InventoryComponentSaveData
{
    public int MaxSlots;
    public List<ItemInstanceSaveData> Items;
}
```

#### ResourceSystemSaveData
```csharp
public class ResourceSystemSaveData
{
    public List<EntityResourceData> EntityResources;
}

public class EntityResourceData
{
    public int EntityId;
    public List<ResourceData> Resources;
}

public class ResourceData
{
    public ResourceType ResourceType;
    public float Amount;
}
```

## Usage Examples

### Basic Inventory Operations
```csharp
// Create inventory system
var itemDatabase = new ItemDatabase();
var inventorySystem = new InventorySystem(entityManager, eventManager, itemDatabase);

// Add inventory to player
var playerInventory = inventorySystem.AddInventoryToEntity(playerEntity, 20);

// Add item to inventory
var item = new ItemInstance("medkit_small", 3);
var result = playerInventory.AddItem(item, itemDatabase);

// Check for items
if (playerInventory.HasItem("ammo_pistol", 50))
{
    // Use pistol ammo
}
```

### Resource Management
```csharp
// Create resource system
var resourceSystem = new ResourceSystem(entityManager, eventManager);

// Add resources
resourceSystem.AddResource(playerId, ResourceType.Gold, 100, "Starting gold");
resourceSystem.AddResource(playerId, ResourceType.Credits, 50, "Bonus");

// Check and spend resources
if (resourceSystem.HasEnough(playerId, ResourceType.Gold, 25))
{
    resourceSystem.SpendResource(playerId, ResourceType.Gold, 25, "Purchased item");
}
```

### Event Handling
```csharp
// Subscribe to item events
eventManager.Subscribe<ItemAddedEvent>(OnItemAdded);
eventManager.Subscribe<ResourceChangedEvent>(OnResourceChanged);

private void OnItemAdded(ItemAddedEvent evt)
{
    Console.WriteLine($"Added {evt.Quantity}x {evt.ItemDefinitionId} to entity {evt.EntityId}");
}

private void OnResourceChanged(ResourceChangedEvent evt)
{
    if (evt.IsGain)
    {
        Console.WriteLine($"Gained {evt.ChangeAmount} {evt.ResourceType}");
    }
}
```

## Integration Points

### System Dependencies
- **EntityManager:** For component access and entity management
- **EventManager:** For event publishing and subscription
- **AssetManager:** For UI assets and item icons
- **TextRenderer:** For UI text rendering

### Required Initialization
```csharp
// Initialize systems in order
var itemDatabase = new ItemDatabase();
var resourceSystem = new ResourceSystem(entityManager, eventManager);
var inventorySystem = new InventorySystem(entityManager, eventManager, itemDatabase);

// UI system will automatically handle inventory/resource UI
```

### Save/Load Integration
```csharp
// Save game state
var inventoryData = playerInventory.ToSaveData();
var resourceData = resourceSystem.ToSaveData();

// Load game state
var loadedInventory = InventoryComponent.FromSaveData(inventoryData);
var loadedResources = ResourceSystem.FromSaveData(resourceData, entityManager, eventManager);
```

## Performance Considerations

### Optimization Strategies
1. **Item Database Caching:** Item definitions loaded once and cached
2. **Event Batching:** Multiple resource changes can be batched
3. **UI Update Throttling:** UI updates limited to reasonable intervals
4. **Memory Management:** Proper cleanup of item instances and metadata

### Memory Usage
- **Item Instances:** Lightweight with GUID-based identification
- **Metadata Dictionaries:** Only created when needed
- **Event Objects:** Reusable where possible

## Security and Validation

### Input Validation
- All public methods validate input parameters
- Stack limits enforced at multiple levels
- Resource operations check for sufficient funds
- Item IDs validated against database

### Error Handling
- Comprehensive exception handling in all systems
- Graceful degradation for missing assets
- Audit logging for all operations
- Rollback support for failed transactions

## Future Enhancements

### Planned Features
1. **Crafting System:** Recipe-based item creation
2. **Trading System:** Player-to-player item exchange
3. **Equipment System:** Wearable items with slots
4. **Market System:** NPC vendors and dynamic pricing
5. **Achievement Integration:** Item collection milestones

### Extension Points
- Custom item types through inheritance
- Additional resource types via enum extension
- Custom UI renderers for specialized inventories
- Plugin architecture for item behaviors

## Conclusion

P11-04-12 provides a robust, extensible foundation for inventory and resource management in SAS Zombie Assault TD. The system features:

- **Complete Functionality:** Full CRUD operations for items and resources
- **Event-Driven Architecture:** Loose coupling and easy integration
- **UI Integration:** Comprehensive UI support with proper layering
- **Serialization Support:** Complete save/load functionality
- **Performance Optimized:** Efficient data structures and caching
- **Extensible Design:** Easy to add new features and item types

The system is production-ready and provides a solid foundation for future gameplay enhancements.
