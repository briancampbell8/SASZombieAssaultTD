// =====================================================================================================
//  FILE: GSSerial.cs
//  PATH: Engine/Save/GameSave/GSSerial.cs
//  SUBSYSTEM: GameSave Serialization
//
//  ROLE:
//      GSSerial provides deterministic JSON serialization and deserialization routines for GSCore.
//      It isolates all JSON formatting, parsing, and size‑calculation logic from GSCore, ensuring
//      that orchestration and data modeling remain strictly separated.
//
//  RESPONSIBILITIES:
//      - Convert GSCore instances into JSON strings.
//      - Convert JSON strings back into GSCore instances.
//      - Provide accurate save‑size calculations.
//      - Enforce consistent JSON formatting rules across the entire save subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Validating save data (handled by GSValidation).
//      - Capturing game state (handled by GSCapture).
//      - Applying save data (handled by GSApply).
//      - Managing GSCore lifecycle or metadata.
//
//  ARCHITECTURAL NOTES:
//      - All methods are static and operate on GSCore instances.
//      - JSON formatting uses camelCase and indented output for readability.
//      - GSSerial replaces the serialization logic previously embedded in SASGameSave.
// =====================================================================================================
using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Text.Json;
using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Save.GameSave
{
    /// <summary>
    /// Static JSON serialization and deserialization routines for GSCore.
    /// </summary>
    public static class GSSerial
    {
        private static readonly JsonSerializerOptions WriteOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private static readonly JsonSerializerOptions ReadOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        /// <summary>
        /// Serializes a GSCore instance to a JSON string.
        /// </summary>
        public static string Serialize(GSCore save)
        {
            try
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline, "GameSave.GSSerial", 1, "Serialize",
                    "Serializing GSCore to JSON.");

                return JsonSerializer.Serialize(save, WriteOptions);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error serializing save data: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Deserializes a JSON string into a GSCore instance.
        /// </summary>
        public static GSCore Deserialize(string json)
        {
            try
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline, "GameSave.GSSerial", 2, "Deserialize",
                    "Deserializing GSCore from JSON.");

                var save = JsonSerializer.Deserialize<GSCore>(json, ReadOptions);
                return save;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deserializing save data: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Returns the approximate size of the save in bytes.
        /// </summary>
        public static long GetSaveSize(GSCore save)
        {
            try
            {
                var json = Serialize(save);
                return json?.Length ?? 0;
            }
            catch
            {
                return 0;
            }
        }
    }
}
