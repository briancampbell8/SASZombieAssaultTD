// ====================================================================================================
//  FILE: Transaction.cs
//  PATH: ./Engine/Economy/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the Transaction module.
//
//  RESPONSIBILITIES:
//      - Provide IsValid() behavior for the Core subsystem.
//      - Provide ToString() behavior for the Core subsystem.
//      - Provide Clone() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
/*
File:    Transaction.cs
Purpose: Represents individual economic transactions in the game.
Features: Transaction tracking, validation, and logging.
*/

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Core;

using SASZombieAssaultTD.Engine.Diagnostics;
namespace SASZombieAssaultTD.Engine.Economy
//
{
    ///<summary>
    ///Represents the type of economic transaction.
    ///</summary>
    public enum TransactionType
    {
        ///<summary>
        ///Money earned from kills, waves, or bonuses.
        ///</summary>
        Income,

        ///<summary>
        ///Money spent on towers, upgrades, or items.
        ///</summary>
        Expense,

        ///<summary>
        ///Money refunded from sales or cancellations.
        ///</summary>
        Refund,

        ///<summary>
        ///Bonus money from achievements or events.
        ///</summary>
        Bonus,

        ///<summary>
        ///Penalty for losing lives or failed objectives.
        ///</summary>
        Penalty
    }

    ///<summary>
    ///Represents a single economic transaction in the game.
    ///Tracks all financial movements for debugging and analytics.
    ///</summary>
    public class Transaction
    {
        ///<summary>
        ///Unique identifier for the transaction.
        ///</summary>
        public string Id { get; }

        ///<summary>
        ///Type of transaction (income, expense, etc.).
        ///</summary>
        public TransactionType Type { get; }

        ///<summary>
        ///Resource type being transacted.
        ///</summary>
        public ResourceType ResourceType { get; }

        ///<summary>
        ///Amount of the transaction (can be negative for expenses).
        ///</summary>
        public int Amount { get; }

        ///<summary>
        ///Description of what the transaction was for.
        ///</summary>
        public string Description { get; }

        ///<summary>
        ///Source of the transaction (e.g., "Enemy Kill", "Tower Purchase").
        ///</summary>
        public string Source { get; }

        ///<summary>
        ///Destination of the transaction (e.g., "Player Cash", "Tower Cost").
        ///</summary>
        public string Destination { get; }

        ///<summary>
        ///When the transaction occurred.
        ///</summary>
        public DateTime Timestamp { get; }

        ///<summary>
        ///Whether the transaction was successful.
        ///</summary>
        public bool IsSuccessful { get; }

        ///<summary>
        ///Error message if transaction failed.
        ///</summary>
        public string ErrorMessage { get; }

        ///<summary>
        ///Creates a new successful transaction.
        ///</summary>
        public Transaction(TransactionType type, ResourceType resourceType, int amount,
                     string description, string source, string destination)
        {
            Id = Guid.NewGuid().ToString("N")[..8];
            Type = type;
            ResourceType = resourceType;
            Amount = amount;
            Description = description;
            Source = source;
            Destination = destination;
            Timestamp = DateTime.Now;
            IsSuccessful = true;
            ErrorMessage = string.Empty;

            DLogger.Log(LogSubsystems.Unknown, LogEnums.LogLevel.Info, "DEBUG", $"Transaction: Created {type} transaction - {resourceType} {amount} from {source} to {destination}");
        }

        ///<summary>
        ///Creates a new failed transaction.
        ///</summary>
        public Transaction(TransactionType type, ResourceType resourceType, int amount,
                     string description, string source, string destination, string errorMessage)
        {
            Id = Guid.NewGuid().ToString("N")[..8];
            Type = type;
            ResourceType = resourceType;
            Amount = amount;
            Description = description;
            Source = source;
            Destination = destination;
            Timestamp = DateTime.Now;
            IsSuccessful = false;
            ErrorMessage = errorMessage;

            DLogger.Log(LogSubsystems.Unknown, LogEnums.LogLevel.Info, "WARNING", $"Transaction: Failed {type} transaction - {errorMessage}");
        }

        ///<summary>
        ///Validates the transaction data.
        ///</summary>
        ///<returns>True if valid</returns>
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Id) &&
                   !string.IsNullOrEmpty(Description) &&
                   !string.IsNullOrEmpty(Source) &&
                   !string.IsNullOrEmpty(Destination) &&
                   Amount != 0;
        }

        ///<summary>
        ///Gets a formatted string representation of the transaction.
        ///</summary>
        ///<returns>Formatted string</returns>
        public override string ToString()
        {
            var status = IsSuccessful ? "SUCCESS" : "FAILED";
            var sign = Amount >= 0 ? "+" : "";
            return $"[{Timestamp:HH:mm:ss}] {status} {Type}: {sign}{Amount} {ResourceType} - {Description} ({Source} → {Destination})";
        }

        ///<summary>
        ///Creates a copy of this transaction.
        ///</summary>
        ///<returns>Transaction copy</returns>
        public Transaction Clone()
        {
            return IsSuccessful
                ? new Transaction(Type, ResourceType, Amount, Description, Source, Destination)
                : new Transaction(Type, ResourceType, Amount, Description, Source, Destination, ErrorMessage);
        }
    }
}

