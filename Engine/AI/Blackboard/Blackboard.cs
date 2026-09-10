// ====================================================================================================
//  FILE: Blackboard.cs
//  PATH: ./Engine/AI/Blackboard/
//  MODULE: AI
//
//  ROLE:
//      Provide deterministic AI behavior, decision logic, or state evaluation.
//
//  RESPONSIBILITIES:
//      - Provide Set() behavior for the AI subsystem.
//      - Provide Get() behavior for the AI subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.AI.Blackboard
{
    ///<summary>
    ///Simple key/value blackboard for AI systems.
    ///</summary>
    public sealed class Blackboard
    {
        private readonly Dictionary<string, object> _data = new();

        ///<summary>
        ///Stores a value under the given key.
        ///</summary>
        public void Set(string key, object value)
        {
            _data[key] = value;
        }

        ///<summary>
        ///Retrieves a value from the blackboard.
        ///Returns null if the key does not exist.
        ///</summary>
        public object? Get(string key)
        {
            if (_data.TryGetValue(key, out var value))
                return value;

            return null;
        }
    }
}




