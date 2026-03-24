using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Components
{
    /// <summary>
    /// Represents an item instance in the inventory system.
    /// </summary>
    public class ItemInstance
    {
        /// <summary>
        /// Unique identifier for the item.
        /// </summary>
        public string ItemId { get; set; } = string.Empty;

        /// <summary>
        /// Quantity of the item.
        /// </summary>
        public int Quantity { get; set; } = 1;

        /// <summary>
        /// Weight of a single unit of the item.
        /// </summary>
        public float Weight { get; set; } = 1f;

        /// <summary>
        /// Calculates the total weight of the item based on its quantity.
        /// </summary>
        public float TotalWeight => Quantity * Weight;
    }

    /// <summary>
    /// Core ECS component for inventory management.
    /// </summary>
    public class InventoryComponent
    {
        /// <summary>
        /// Dictionary of items in the inventory, keyed by their unique item ID.
        /// </summary>
        public Dictionary<string, ItemInstance> Items { get; private set; } = new();

        /// <summary>
        /// Maximum number of item slots in the inventory.
        /// </summary>
        public int MaxSlots { get; set; } = 20;

        /// <summary>
        /// Maximum weight capacity of the inventory.
        /// </summary>
        public float MaxWeight { get; set; } = 100f;

        /// <summary>
        /// Current total weight of all items in the inventory.
        /// </summary>
        public float CurrentWeight { get; private set; }

        /// <summary>
        /// Adds an item to the inventory.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>True if the item was added successfully, false if the inventory is full or exceeds weight capacity.</returns>
        public bool AddItem(ItemInstance item)
        {
            if (Items.Count >= MaxSlots)
                return false;

            float newWeight = CurrentWeight + item.TotalWeight;
            if (newWeight > MaxWeight)
                return false;

            if (Items.ContainsKey(item.ItemId))
            {
                Items[item.ItemId].Quantity += item.Quantity;
            }
            else
            {
                Items[item.ItemId] = item;
            }

            CurrentWeight = newWeight;
            return true;
        }

        /// <summary>
        /// Removes an item or a specific quantity of an item from the inventory.
        /// </summary>
        /// <param name="itemId">The unique ID of the item to remove.</param>
        /// <param name="quantity">The quantity to remove. Defaults to 1.</param>
        /// <returns>True if the item was removed successfully, false if the item does not exist.</returns>
        public bool RemoveItem(string itemId, int quantity = 1)
        {
            if (!Items.TryGetValue(itemId, out var item))
                return false;

            if (item.Quantity <= quantity)
            {
                CurrentWeight -= item.TotalWeight;
                Items.Remove(itemId);
            }
            else
            {
                CurrentWeight -= item.Weight * quantity;
                item.Quantity -= quantity;
            }

            return true;
        }

        /// <summary>
        /// Retrieves an item from the inventory by its unique ID.
        /// </summary>
        /// <param name="itemId">The unique ID of the item to retrieve.</param>
        /// <returns>The item instance if found, otherwise null.</returns>
        public ItemInstance GetItem(string itemId)
        {
            return Items.TryGetValue(itemId, out var item) ? item : null;
        }

        /// <summary>
        /// Checks if the inventory contains a specific item.
        /// </summary>
        /// <param name="itemId">The unique ID of the item to check.</param>
        /// <returns>True if the item exists in the inventory, otherwise false.</returns>
        public bool ContainsItem(string itemId)
        {
            return Items.ContainsKey(itemId);
        }

        /// <summary>
        /// Clears all items from the inventory.
        /// </summary>
        public void Clear()
        {
            Items.Clear();
            CurrentWeight = 0f;
        }

        /// <summary>
        /// Provides a summary of the inventory's current state.
        /// </summary>
        /// <returns>A formatted string summarizing the inventory.</returns>
        public override string ToString()
        {
            return $"Inventory: {Items.Count}/{MaxSlots} slots, {CurrentWeight:F2}/{MaxWeight:F2} weight";
        }
    }
}
