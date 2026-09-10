// =====================================================================================================
//  FILE: AnimationParameterStore.cs
//  PATH: Engine/Animation/Core/AnimationParameterStore.cs
//  SUBSYSTEM: Animation Core
//
//  ROLE:
//      Provides deterministic storage and retrieval of animation parameters used by controllers,
//      transitions, and blend engines.
//
//  RESPONSIBILITIES:
//      - Store float parameters by name.
//      - Provide deterministic retrieval.
//      - Support updating and clearing parameters.
//
//  NON-RESPONSIBILITIES:
//      - Evaluating transition logic.
//      - Managing animation playback.
//      - Handling blending or timing.
//
//  ARCHITECTURAL NOTES:
//      - Lives at the root of Animation/Core as a foundational primitive.
//      - Used by AnimationTransitionRule and AnimationController.
// =====================================================================================================

using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Animation.Core
{
    internal sealed class AnimationParameterStore
    {
        private readonly Dictionary<string, float> _parameters = new();

        /// <summary>
        /// Sets a parameter value.
        /// </summary>
        public void SetValue(string name, float value)
        {
            if (string.IsNullOrEmpty(name))
                return;

            _parameters[name] = value;
        }

        /// <summary>
        /// Gets a parameter value, or 0 if not present.
        /// </summary>
        public float GetValue(string name)
        {
            if (string.IsNullOrEmpty(name))
                return 0f;

            return _parameters.TryGetValue(name, out var v) ? v : 0f;
        }

        /// <summary>
        /// Clears all parameters.
        /// </summary>
        public void Clear()
        {
            _parameters.Clear();
        }

        /// <summary>
        /// Returns a snapshot of all parameters.
        /// </summary>
        public IReadOnlyDictionary<string, float> GetAll()
        {
            return _parameters;
        }
    }
}
