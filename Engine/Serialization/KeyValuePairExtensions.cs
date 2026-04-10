/*
File:    KeyValuePairExtensions.cs
Purpose: Extension methods for KeyValuePair serialization in SAS Zombie Assault TD.
Features: Complete serialization support for KeyValuePair used in save system and level progression.
Used by: LevelProgression system, save/load functionality, data persistence.
*/

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SASZombieAssaultTD.Engine.Serialization
{
    /// <summary>
    /// Extension methods providing serialization capabilities for KeyValuePair structures.
    /// Enables proper JSON serialization and deserialization of KeyValuePair instances
    /// used throughout the SAS Zombie Assault TD save system and level progression tracking.
    /// </summary>
    public static class KeyValuePairExtensions
    {
        #region JSON Serialization Extensions

        /// <summary>
        /// Serializes a KeyValuePair to JSON string format.
        /// Handles both value types and reference types with proper JSON formatting.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="keyValuePair">The KeyValuePair to serialize.</param>
        /// <returns>JSON string representation of the KeyValuePair.</returns>
        /// <exception cref="ArgumentNullException">Thrown when keyValuePair is null.</exception>
        public static string Serialize<TKey, TValue>(this KeyValuePair<TKey, TValue> keyValuePair)
        {
            if (keyValuePair.Equals(default(KeyValuePair<TKey, TValue>)))
                throw new ArgumentNullException(nameof(keyValuePair));

            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = false,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };

                var serializablePair = new SerializableKeyValuePair<TKey, TValue>(keyValuePair);
                return JsonSerializer.Serialize(serializablePair, options);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Failed to serialize KeyValuePair: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Deserializes a JSON string back to a KeyValuePair.
        /// Handles both value types and reference types with proper JSON parsing.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <returns>Deserialized KeyValuePair.</returns>
        /// <exception cref="ArgumentNullException">Thrown when json is null or empty.</exception>
        /// <exception cref="JsonException">Thrown when JSON is invalid.</exception>
        public static KeyValuePair<TKey, TValue> DeserializeKeyValuePair<TKey, TValue>(this string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                throw new ArgumentNullException(nameof(json));

            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var serializablePair = JsonSerializer.Deserialize<SerializableKeyValuePair<TKey, TValue>>(json, options);
                return serializablePair.ToKeyValuePair();
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Failed to deserialize KeyValuePair: {ex.Message}", ex);
            }
        }

        #endregion

        #region Binary Serialization Extensions

        /// <summary>
        /// Serializes a KeyValuePair to a byte array for binary storage.
        /// Provides efficient binary serialization for performance-critical scenarios.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="keyValuePair">The KeyValuePair to serialize.</param>
        /// <returns>Byte array containing the serialized data.</returns>
        /// <exception cref="ArgumentNullException">Thrown when keyValuePair is null.</exception>
        public static byte[] SerializeToBinary<TKey, TValue>(this KeyValuePair<TKey, TValue> keyValuePair)
        {
            if (keyValuePair.Equals(default(KeyValuePair<TKey, TValue>)))
                throw new ArgumentNullException(nameof(keyValuePair));

            using var stream = new System.IO.MemoryStream();
            using var writer = new System.IO.BinaryWriter(stream);

            try
            {
                // Serialize key
                SerializeObject(writer, keyValuePair.Key);
                
                // Serialize value
                SerializeObject(writer, keyValuePair.Value);
                
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to serialize KeyValuePair to binary: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Deserializes a byte array back to a KeyValuePair.
        /// Provides efficient binary deserialization for performance-critical scenarios.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="data">The byte array to deserialize.</param>
        /// <returns>Deserialized KeyValuePair.</returns>
        /// <exception cref="ArgumentNullException">Thrown when data is null or empty.</exception>
        /// <exception cref="InvalidOperationException">Thrown when deserialization fails.</exception>
        public static KeyValuePair<TKey, TValue> DeserializeFromBinary<TKey, TValue>(this byte[] data)
        {
            if (data == null || data.Length == 0)
                throw new ArgumentNullException(nameof(data));

            using var stream = new System.IO.MemoryStream(data);
            using var reader = new System.IO.BinaryReader(stream);

            try
            {
                // Deserialize key
                var key = DeserializeObject<TKey>(reader);
                
                // Deserialize value
                var value = DeserializeObject<TValue>(reader);
                
                return new KeyValuePair<TKey, TValue>(key, value);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to deserialize KeyValuePair from binary: {ex.Message}", ex);
            }
        }

        #endregion

        #region Utility Extensions

        /// <summary>
        /// Creates a deep copy of a KeyValuePair with serialized values.
        /// Useful for creating independent copies that won't share references.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="keyValuePair">The KeyValuePair to copy.</param>
        /// <returns>A deep copy of the KeyValuePair.</returns>
        public static KeyValuePair<TKey, TValue> DeepCopy<TKey, TValue>(this KeyValuePair<TKey, TValue> keyValuePair)
        {
            if (keyValuePair.Equals(default(KeyValuePair<TKey, TValue>)))
                return default(KeyValuePair<TKey, TValue>);

            try
            {
                var json = keyValuePair.Serialize();
                return json.DeserializeKeyValuePair<TKey, TValue>();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create deep copy of KeyValuePair: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets a hash code for a KeyValuePair suitable for use in hash-based collections.
        /// Provides consistent hashing for KeyValuePair instances.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="keyValuePair">The KeyValuePair to hash.</param>
        /// <returns>Hash code for the KeyValuePair.</returns>
        public static int GetPairHashCode<TKey, TValue>(this KeyValuePair<TKey, TValue> keyValuePair)
        {
            if (keyValuePair.Equals(default(KeyValuePair<TKey, TValue>)))
                return 0;

            int keyHash = keyValuePair.Key?.GetHashCode() ?? 0;
            int valueHash = keyValuePair.Value?.GetHashCode() ?? 0;
            
            return HashCode.Combine(keyHash, valueHash);
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Serializes an object to binary format using pattern matching.
        /// </summary>
        private static void SerializeObject<T>(System.IO.BinaryWriter writer, T obj)
        {
            if (obj == null)
            {
                writer.Write(false); // Null flag
                return;
            }

            writer.Write(true); // Not null flag

            switch (obj)
            {
                case string s:
                    writer.Write(s);
                    break;
                case int i:
                    writer.Write(i);
                    break;
                case float f:
                    writer.Write(f);
                    break;
                case bool b:
                    writer.Write(b);
                    break;
                default:
                    // Fallback to JSON serialization for complex types
                    var json = JsonSerializer.Serialize(obj);
                    writer.Write(json);
                    break;
            }
        }

        /// <summary>
        /// Deserializes an object from binary format using pattern matching.
        /// </summary>
        private static T DeserializeObject<T>(System.IO.BinaryReader reader)
        {
            var isNotNull = reader.ReadBoolean();
            if (!isNotNull)
                return default(T);

            return typeof(T) switch
            {
                var t when t == typeof(string) => (T)(object)reader.ReadString(),
                var t when t == typeof(int) => (T)(object)reader.ReadInt32(),
                var t when t == typeof(float) => (T)(object)reader.ReadSingle(),
                var t when t == typeof(bool) => (T)(object)reader.ReadBoolean(),
                _ => (T)(object)JsonSerializer.Deserialize<T>(reader.ReadString())
            };
        }

        #endregion
    }

    /// <summary>
    /// Serializable wrapper for KeyValuePair to enable JSON serialization.
    /// Provides a serializable representation of KeyValuePair that can be properly
    /// handled by the System.Text.Json serializer.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    internal sealed class SerializableKeyValuePair<TKey, TValue>
    {
        /// <summary>
        /// Gets or sets the key value.
        /// </summary>
        public TKey Key { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public TValue Value { get; set; }

        /// <summary>
        /// Creates a new SerializableKeyValuePair from an existing KeyValuePair.
        /// </summary>
        /// <param name="keyValuePair">The source KeyValuePair.</param>
        public SerializableKeyValuePair(KeyValuePair<TKey, TValue> keyValuePair)
        {
            Key = keyValuePair.Key;
            Value = keyValuePair.Value;
        }

        /// <summary>
        /// Parameterless constructor for JSON deserialization.
        /// </summary>
        public SerializableKeyValuePair()
        {
        }

        /// <summary>
        /// Converts this SerializableKeyValuePair back to a KeyValuePair.
        /// </summary>
        /// <returns>The equivalent KeyValuePair.</returns>
        public KeyValuePair<TKey, TValue> ToKeyValuePair()
        {
            return new KeyValuePair<TKey, TValue>(Key, Value);
        }
    }
}
