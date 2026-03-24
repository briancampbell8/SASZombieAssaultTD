/*
File:    EconomyEvents.cs
Purpose: Event system for economy-related notifications.
Features: Cash change events, purchase events, economy state changes.
*/

using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Core;

namespace SASZombieAssaultTD.Engine.Economy
{
    /// <summary>
    /// Event arguments for cash change events.
    /// </summary>
    public class CashChangedEventArgs : EventArgs
    {
        public int OldAmount { get; }
        public int NewAmount { get; }
        public int ChangeAmount => NewAmount - OldAmount;
        public string Source { get; }

        public CashChangedEventArgs(int oldAmount, int newAmount, string source)
        {
            OldAmount = oldAmount;
            NewAmount = newAmount;
            Source = source;
        }
    }

    /// <summary>
    /// Event arguments for purchase events.
    /// </summary>
    public class PurchaseEventArgs : EventArgs
    {
        public string ItemId { get; }
        public string ItemName { get; }
        public int Cost { get; }
        public bool IsSuccessful { get; }
        public string ErrorMessage { get; }

        public PurchaseEventArgs(string itemId, string itemName, int cost, bool isSuccessful, string errorMessage = "")
        {
            ItemId = itemId;
            ItemName = itemName;
            Cost = cost;
            IsSuccessful = isSuccessful;
            ErrorMessage = errorMessage;
        }
    }

    /// <summary>
    /// Event arguments for economy state change events.
    /// </summary>
    public class EconomyStateEventArgs : EventArgs
    {
        public string StateName { get; }
        public Dictionary<string, object> StateData { get; }

        public EconomyStateEventArgs(string stateName, Dictionary<string, object> stateData = null)
        {
            StateName = stateName;
            StateData = stateData ?? new Dictionary<string, object>();
        }
    }

    /// <summary>
    /// Central event manager for economy-related events.
    /// Provides loose coupling between economy systems and UI/other systems.
    /// </summary>
    public static class EconomyEvents
    {
        /// <summary>
        /// Fired when cash amount changes.
        /// </summary>
        public static event EventHandler<CashChangedEventArgs> OnCashChanged;

        /// <summary>
        /// Fired when a purchase is attempted.
        /// </summary>
        public static event EventHandler<PurchaseEventArgs> OnPurchaseAttempted;

        /// <summary>
        /// Fired when a purchase is completed.
        /// </summary>
        public static event EventHandler<PurchaseEventArgs> OnPurchaseCompleted;

        /// <summary>
        /// Fired when player has insufficient funds.
        /// </summary>
        public static event EventHandler<PurchaseEventArgs> OnInsufficientFunds;

        /// <summary>
        /// Fired when economy state changes.
        /// </summary>
        public static event EventHandler<EconomyStateEventArgs> OnEconomyStateChanged;

        /// <summary>
        /// Fired when a transaction is completed.
        /// </summary>
        public static event EventHandler<Transaction> OnTransactionCompleted;

        /// <summary>
        /// Triggers cash changed event.
        /// </summary>
        public static void TriggerCashChanged(int oldAmount, int newAmount, string source)
        {
            var args = new CashChangedEventArgs(oldAmount, newAmount, source);
            OnCashChanged?.Invoke(null, args);
            ModernLoggingSystem.Log("INFO", $"EconomyEvents: Cash changed from {oldAmount} to {newAmount} via {source}");
        }

        /// <summary>
        /// Triggers purchase attempted event.
        /// </summary>
        public static void TriggerPurchaseAttempted(string itemId, string itemName, int cost)
        {
            var args = new PurchaseEventArgs(itemId, itemName, cost, false);
            OnPurchaseAttempted?.Invoke(null, args);
            ModernLoggingSystem.Log("INFO", $"EconomyEvents: Purchase attempted - {itemName} ({cost})");
        }

        /// <summary>
        /// Triggers purchase completed event.
        /// </summary>
        public static void TriggerPurchaseCompleted(string itemId, string itemName, int cost, bool isSuccessful, string errorMessage = "")
        {
            var args = new PurchaseEventArgs(itemId, itemName, cost, isSuccessful, errorMessage);
            OnPurchaseCompleted?.Invoke(null, args);

            if (!isSuccessful)
            {
                OnInsufficientFunds?.Invoke(null, args);
            }

            ModernLoggingSystem.Log("INFO", $"EconomyEvents: Purchase {(isSuccessful ? "completed" : "failed")} - {itemName} ({cost})");
        }

        /// <summary>
        /// Triggers economy state changed event.
        /// </summary>
        public static void TriggerEconomyStateChanged(string stateName, Dictionary<string, object> stateData = null)
        {
            var args = new EconomyStateEventArgs(stateName, stateData);
            OnEconomyStateChanged?.Invoke(null, args);
            ModernLoggingSystem.Log("INFO", $"EconomyEvents: State changed to {stateName}");
        }

        /// <summary>
        /// Triggers transaction completed event.
        /// </summary>
        public static void TriggerTransactionCompleted(Transaction transaction)
        {
            OnTransactionCompleted?.Invoke(null, transaction);
            ModernLoggingSystem.Log("INFO", $"EconomyEvents: Transaction completed - {transaction}");
        }

        /// <summary>
        /// Clears all event subscriptions.
        /// </summary>
        public static void ClearAllEvents()
        {
            OnCashChanged = null;
            OnPurchaseAttempted = null;
            OnPurchaseCompleted = null;
            OnInsufficientFunds = null;
            OnEconomyStateChanged = null;
            OnTransactionCompleted = null;
            ModernLoggingSystem.Log("INFO", "EconomyEvents: All events cleared");
        }
    }
}
