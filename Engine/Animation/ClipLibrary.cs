// =====================================================================================================
//  FILE: ClipLibrary.cs
//  PATH: Engine/Animation/ClipLibrary.cs
//  SUBSYSTEM: Animation
//
//  ROLE:
//      Provides deterministic lookup, caching, and retrieval of AnimationClip instances.
//      This module acts as a centralized clip repository used by animation controllers,
//      state machines, and event processors.
//
//  RESPONSIBILITIES:
//      - Store and retrieve AnimationClip instances by name.
//      - Provide deterministic clip access for animation systems.
//      - Maintain strict subsystem boundaries: no sequencing, no rendering,
//        no ECS world ownership, no state machine logic.
//      - Log clip registration and lookup activity for debugging and profiling.
//
//  NON-RESPONSIBILITIES:
//      - Animation controller playback logic.
//      - State machine evaluation or transitions.
//      - Gameplay callbacks triggered by animation events.
//      - ECS ECSEntityCore lifecycle management.
//      - Deterministic sequencing (handled by ECSRuntimeCore).
//
//  ARCHITECTURAL NOTES:
//      - This module is intentionally thin and stateless except for its clip cache.
//      - All clip retrieval must route through this library to ensure consistency.
//      - Ensures animation clip access remains isolated and deterministic.
//
//  CHANGE LOG:
//      • 08-15-2026 — File created during Animation subsystem modernization (BDC).
// =====================================================================================================

using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Animation.Core.Clips;
using SASZombieAssaultTD.Engine.Diagnostics;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Animation
{
    /// <summary>
    /// Provides deterministic lookup and caching for animation clips.
    /// </summary>
    internal sealed class ClipLibrary
    {
        private readonly Dictionary<string, AnimationClip> _clipCache = new();

        // =====================================================================================================
        //  PUBLIC API
        // =====================================================================================================

        /// <summary>
        /// Registers a clip into the library. If a clip with the same name already exists,
        /// it will be overwritten.
        /// </summary>
        public void RegisterClip(AnimationClip clip)
        {
            if (clip == null || string.IsNullOrWhiteSpace(clip.Name))
            {
                DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Warning,
                    "ClipLibrary: Attempted to register a clip with invalid name or null reference");
                return;
            }

            _clipCache[clip.Name] = clip;

            DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Info,
                $"ClipLibrary: Registered clip '{clip.Name}' (Duration={clip.Duration:F3})");
        }

        /// <summary>
        /// Attempts to retrieve a clip by name. Returns null if not found.
        /// </summary>
        public AnimationClip? GetClip(string clipName)
        {
            if (string.IsNullOrWhiteSpace(clipName))
            {
                DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Warning,
                    "ClipLibrary: Requested clip with invalid or empty name");
                return null;
            }

            if (_clipCache.TryGetValue(clipName, out var clip))
            {
                DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Debug,
                    $"ClipLibrary: Retrieved clip '{clipName}'");
                return clip;
            }

            DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Warning,
                $"ClipLibrary: Clip '{clipName}' not found in library");

            return null;
        }

        /// <summary>
        /// Returns true if the clip exists in the library.
        /// </summary>
        public bool HasClip(string clipName)
        {
            return _clipCache.ContainsKey(clipName);
        }
    }
}
