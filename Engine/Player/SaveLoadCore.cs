// File:    SaveLoadCore.cs
// Purpose: Pure serialization and deserialization logic.
//          No state, no side effects, no logging, no file I/O.
//

using SASZombieAssaultTD.Engine.Diagnostics;

using System;

using System.Text.Json;

namespace SASZombieAssaultTD.Engine.Player

{
    /// <summary>

    /// Sealed, static, deterministic serialization engine for save/load operations.

    /// Contains pure serialization/deserialization logic with no state or side effects.

    /// </summary>

    public sealed class SaveLoadCore

    {
        /// <summary>

        /// JSON serializer options for save operations.

        /// </summary>

        public static readonly JsonSerializerOptions SaveOptions = new()

        {
            WriteIndented = true,

            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        /// <summary>

        /// JSON serializer options for load operations.

        /// </summary>

        public static readonly JsonSerializerOptions LoadOptions = new()

        {
            PropertyNameCaseInsensitive = true
        };

        /// <summary>

        /// Serializes player data to JSON format.

        /// </summary>

        /// <param name="playerData">The player data to serialize.</param>

        /// <returns>The JSON string representation of the player data.</returns>

        /// <exception cref="ArgumentNullException">Thrown when playerData is null.</exception>

        public static string Serialize(PlayerData playerData)

        {
            if (playerData == null)

                throw new ArgumentNullException(nameof(playerData));

            return JsonSerializer.Serialize(playerData, SaveOptions);
        }

        /// <summary>

        /// Deserializes JSON data to player data structure.

        /// </summary>

        /// <param name="json">The JSON string to deserialize.</param>

        /// <returns>The deserialized PlayerData structure, or null if deserialization fails.</returns>

        public static PlayerData Deserialize(string json)

        {
            if (string.IsNullOrWhiteSpace(json))

                return null;

            //             try

            {
                return JsonSerializer.Deserialize<PlayerData>(json, LoadOptions);
            }

            //             catch

            {
                return null;
            }
        }

        /// <summary>

        /// Validates player data structure for integrity.

        /// </summary>

        /// <param name="playerData">The PlayerData structure to validate.</param>

        /// <returns>True if the data is valid, false otherwise.</returns>

        public static bool ValidatePlayerData(PlayerData playerData)

        {
            if (playerData == null)

                return false;

            if (playerData.State == null)

                return false;

            if (playerData.Progression == null)

                return false;

            // Validate state

            //             try

            {
                playerData.Validate();
            }

            //             catch

            {
                return false;
            }

            // Validate progression

            if (playerData.Progression.CurrentLevel < 1 || playerData.Progression.CurrentLevel > 100)

                return false;

            if (playerData.Progression.CurrentExperience < 0)

                return false;

            if (playerData.Progression.UnlockedTowers == null)

                return false;

            return true;
        }

        /// <summary>

        /// Validates that a player system is in a valid state for saving.

        /// </summary>

        /// <param name="playerSystem">The PlayerSystem instance to validate.</param>

        /// <returns>True if the system is valid for saving, false otherwise.</returns>

        public static bool ValidatePlayerSystemForSave(PlayerSystem playerSystem)

        {
            if (playerSystem == null)

                return false;

            if (playerSystem.State == null || playerSystem.Economy == null || playerSystem == null)

                return false;

            //             try

            {
                playerSystem.Validate();

                return true;
            }

            //             catch

            {
                return false;
            }
        }

        /// <summary>

        /// Checks if a string appears to be valid JSON.

        /// </summary>

        /// <param name="json">The string to check.</param>

        /// <returns>True if the string appears to be valid JSON, false otherwise.</returns>

        public static bool IsValidJson(string json)

        {
            if (string.IsNullOrWhiteSpace(json))

                return false;

            //             try

            {
                JsonDocument.Parse(json);

                return true;
            }

            //             catch

            {
                return false;
            }
        }

        /// <summary>

        /// Gets the maximum allowed save file size in bytes.

        /// </summary>

        public const int MaxFileSize = 1_000_000;

        /// <summary>

        /// Gets the minimum allowed save file size in bytes.

        /// </summary>

        public const int MinFileSize = 1;

        /// <summary>

        /// Validates file size is within acceptable range.

        /// </summary>

        /// <param name="fileSize">The file size in bytes.</param>

        /// <returns>True if the file size is valid, false otherwise.</returns>

        public static bool IsValidFileSize(long fileSize)

        {
            return fileSize >= MinFileSize && fileSize <= MaxFileSize;
        }

        /// <summary>

        /// Gets the current save data version.

        /// </summary>

        public const int CurrentVersion = 1;

        /// <summary>

        /// Checks if a save data version is compatible with the current version.

        /// </summary>

        /// <param name="version">The version to check.</param>

        /// <returns>True if the version is compatible, false otherwise.</returns>

        public static bool IsCompatibleVersion(int version)

        {
            return version == CurrentVersion;
        }
    }

    //public class PlayerData : already defined in PlayerStateData.cs
    //{
    //}
}
