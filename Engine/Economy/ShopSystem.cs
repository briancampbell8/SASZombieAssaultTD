/*
File:    ShopSystem.cs
Purpose: Tower and upgrade purchasing system for SAS Zombie Assault TD.
Features: Tower catalog, pricing, purchase validation, special offers.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using SASZombieAssaultTD.Engine.Core;

using SASZombieAssaultTD.Engine.Diagnostics;
namespace SASZombieAssaultTD.Engine.Economy
//
{
    ///<summary>
    ///Represents an item available for purchase in the shop.
    ///</summary>
    public class ShopItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int BaseCost { get; set; }
        public string IconPath { get; set; }
        public string Category { get; set; }
        public bool IsAvailable { get; set; } = true;
        public Dictionary<string, object> Metadata { get; set; } = new();

        public int GetFinalCost(bool isUpgrade = false)
        {
            return EconomyManager.GetFinalCost(BaseCost);
        }
    }

    ///<summary>
    ///Manages the in-game shop for purchasing towers and upgrades.
    ///Handles pricing, availability, and special offers.
    ///</summary>
    public class ShopSystem
    {
        private static ShopSystem _instance;
        public static ShopSystem Instance => _instance ??= new ShopSystem();

        private readonly Dictionary<string, ShopItem> _shopItems;
        private readonly List<string> _specialOffers;
        private float _globalDiscount = 1.0f;

        private ShopSystem()
        {
            _shopItems = new Dictionary<string, ShopItem>();
            _specialOffers = new List<string>();
            InitializeShopItems();
            DLogger.Log(LogSubsystems.Economy,LogLevel.Info, "ShopSystem: Initialized with shop catalog");
        }

        ///<summary>
        ///Gets all available shop items.
        ///</summary>
        public IReadOnlyDictionary<string, ShopItem> ShopItems => _shopItems;

        ///<summary>
        ///Gets current special offers.
        ///</summary>
        public IReadOnlyList<string> SpecialOffers => _specialOffers;

        ///<summary>
        ///Event fired when shop items change.
        ///</summary>
        public event Action OnShopUpdated;

        ///<summary>
        ///Event fired when special offers change.
        ///</summary>
        public event Action<List<string>> OnSpecialOffersChanged;

        ///<summary>
        ///Initializes the default shop items.
        ///</summary>
        private void InitializeShopItems()
        {
            //Basic Towers
            AddShopItem(new ShopItem
            {
                Id = "tower_basic",
                Name = "Basic Tower",
                Description = "Standard defensive tower with moderate damage and range",
                BaseCost = 100,
                IconPath = "towers/basic.png",
                Category = "Towers"
            });

            AddShopItem(new ShopItem
            {
                Id = "tower_sniper",
                Name = "Sniper Tower",
                Description = "Long-range tower with high damage but slow fire rate",
                BaseCost = 250,
                IconPath = "towers/sniper.png",
                Category = "Towers"
            });

            AddShopItem(new ShopItem
            {
                Id = "tower_shotgun",
                Name = "Shotgun Tower",
                Description = "Short-range tower with spread damage",
                BaseCost = 150,
                IconPath = "towers/shotgun.png",
                Category = "Towers"
            });

            //Upgrades
            AddShopItem(new ShopItem
            {
                Id = "upgrade_damage",
                Name = "Damage Upgrade",
                Description = "Increases tower damage by 25%",
                BaseCost = 75,
                IconPath = "upgrades/damage.png",
                Category = "Upgrades"
            });

            AddShopItem(new ShopItem
            {
                Id = "upgrade_range",
                Name = "Range Upgrade",
                Description = "Increases tower range by 20%",
                BaseCost = 50,
                IconPath = "upgrades/range.png",
                Category = "Upgrades"
            });

            AddShopItem(new ShopItem
            {
                Id = "upgrade_speed",
                Name = "Fire Rate Upgrade",
                Description = "Increases tower fire rate by 30%",
                BaseCost = 60,
                IconPath = "upgrades/speed.png",
                Category = "Upgrades"
            });

            DLogger.Log(LogSubsystems.Economy,LogLevel.Info, $"ShopSystem: Initialized {_shopItems.Count} shop items");
        }

        ///<summary>
        ///Adds a new shop item.
        ///</summary>
        public void AddShopItem(ShopItem item)
        {
            if (string.IsNullOrEmpty(item.Id))
            {
                DLogger.Log(LogSubsystems.Unknown, LogLevel.Info, "WARNING", "ShopSystem: Cannot add shop item without ID");
                return;
            }

            _shopItems[item.Id] = item;
            OnShopUpdated?.Invoke();
            DLogger.Log(LogSubsystems.Economy,LogLevel.Info, $"ShopSystem: Added shop item {item.Name} ({item.Id})");
        }

        ///<summary>
        ///Removes a shop item.
        ///</summary>
        public bool RemoveShopItem(string itemId)
        {
            if (_shopItems.Remove(itemId))
            {
                OnShopUpdated?.Invoke();
                DLogger.Log(LogSubsystems.Economy,LogLevel.Info, $"ShopSystem: Removed shop item {itemId}");
                return true;
            }

            DLogger.Log(LogSubsystems.Unknown, LogLevel.Info, "WARNING", $"ShopSystem: Shop item {itemId} not found");
            return false;
        }

        ///<summary>
        ///Gets a shop item by ID.
        ///</summary>
        public ShopItem GetShopItem(string itemId)
        {
            return _shopItems.TryGetValue(itemId, out var item) ? item : null;
        }

        ///<summary>
        ///Purchases an item from the shop.
        ///</summary>
        public bool PurchaseItem(string itemId)
        {
            var item = GetShopItem(itemId);
            if (item == null)
            {
                DLogger.Log(LogSubsystems.Unknown, LogLevel.Info, "WARNING", $"ShopSystem: Shop item {itemId} not found");
                return false;
            }

            if (!item.IsAvailable)
            {
                DLogger.Log(LogSubsystems.Unknown, LogLevel.Info, "WARNING", $"ShopSystem: Shop item {itemId} is not available");
                return false;
            }

            var finalCost = item.GetFinalCost();

            //Trigger purchase attempt event
            EconomyEvents.TriggerPurchaseAttempted(itemId, item.Name, finalCost);

            if (!EconomyManager.HasEnoughCash(finalCost))
            {
                EconomyEvents.TriggerPurchaseCompleted(itemId, item.Name, finalCost, false, "Insufficient funds");
                return false;
            }

            EconomyManager.Spend(finalCost);

            EconomyEvents.TriggerPurchaseCompleted(itemId, item.Name, finalCost, true);
            DLogger.Log(LogSubsystems.Economy,LogLevel.Info, $"ShopSystem: Purchased {item.Name} for {finalCost} cash");
            return true;
        }

        ///<summary>
        ///Gets shop items by category.
        ///</summary>
        public List<ShopItem> GetItemsByCategory(string category)
        {
            return _shopItems.Values
                .Where(item => item.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        ///<summary>
        ///Gets available shop items.
        ///</summary>
        public List<ShopItem> GetAvailableItems()
        {
            return _shopItems.Values
                .Where(item => item.IsAvailable)
                .ToList();
        }

        ///<summary>
        ///Sets a global discount for all items.
        ///</summary>
        public void SetGlobalDiscount(float discountMultiplier)
        {
            _globalDiscount = System.Math.Max(0.1f, System.Math.Min(1.0f, discountMultiplier));
            OnShopUpdated?.Invoke();
            DLogger.Log(LogSubsystems.Economy,LogLevel.Info, $"ShopSystem: Set global discount to {_globalDiscount:F2}");
        }

        ///<summary>
        ///Adds a special offer.
        ///</summary>
        public void AddSpecialOffer(string itemId)
        {
            if (!_specialOffers.Contains(itemId))
            {
                _specialOffers.Add(itemId);
                OnSpecialOffersChanged?.Invoke(new List<string>(_specialOffers));
                DLogger.Log(LogSubsystems.Economy,LogLevel.Info, $"ShopSystem: Added special offer {itemId}");
            }
        }

        ///<summary>
        ///Removes a special offer.
        ///</summary>
        public void RemoveSpecialOffer(string itemId)
        {
            if (_specialOffers.Remove(itemId))
            {
                OnSpecialOffersChanged?.Invoke(new List<string>(_specialOffers));
                DLogger.Log(LogSubsystems.Economy,LogLevel.Info, $"ShopSystem: Removed special offer {itemId}");
            }
        }

        ///<summary>
        ///Checks if an item is on special offer.
        ///</summary>
        public bool IsSpecialOffer(string itemId)
        {
            return _specialOffers.Contains(itemId);
        }
    }
}
