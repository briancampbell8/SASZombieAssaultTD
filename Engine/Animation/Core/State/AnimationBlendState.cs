// =====================================================================================================
//  FILE: AnimationBlendState.cs
//  PATH: Engine/Animation/Core/State/AnimationBlendState.cs
//  SUBSYSTEM: Animation Core State
//
//  ROLE:
//      Represents the deterministic blend weights between animation clips during transitions.
//      Stores and updates normalized weights used by the animation controller and transition engine.
//
//  RESPONSIBILITIES:
//      - Maintain blend weights for one or more animation clips.
//      - Provide deterministic normalization of weights.
//      - Allow querying and updating of individual clip weights.
//      - Support clearing and rebuilding blend sets during transitions.
//
//  NON-RESPONSIBILITIES:
//      - Executing animation playback.
//      - Managing animation clips or tracks.
//      - Handling cross‑fade timing or transition rules.
//      - Dispatching animation events.
//
//  ARCHITECTURAL NOTES:
//      - Lives under Animation/Core/State as a foundational primitive.
//      - Used by AnimationTransitionState and AnimationController.
//      - Must remain lightweight and deterministic.
// =====================================================================================================

using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Linq;

namespace SASZombieAssaultTD.Engine.Animation.Core.State
{
    internal sealed class AnimationBlendState
    {
        private readonly Dictionary<string, float> _weights = new();

        /// <summary>
        /// Sets the blend weight for a clip.
        /// </summary>
        public void SetWeight(string clipName, float weight)
        {
            if (string.IsNullOrEmpty(clipName))
                return;

            _weights[clipName] = weight;
        }

        /// <summary>
        /// Gets the blend weight for a clip, or 0 if not present.
        /// </summary>
        public float GetWeight(string clipName)
        {
            if (string.IsNullOrEmpty(clipName))
                return 0f;

            return _weights.TryGetValue(clipName, out var w) ? w : 0f;
        }

        /// <summary>
        /// Normalizes all weights so the total equals 1.
        /// </summary>
        public void Normalize()
        {
            float sum = 0f;

            foreach (var kvp in _weights)
                sum += kvp.Value;

            if (sum <= 0f)
                return;

            var keys = _weights.Keys.ToList();
            foreach (var key in keys)
                _weights[key] = _weights[key] / sum;
        }

        /// <summary>
        /// Clears all blend weights.
        /// </summary>
        public void Clear()
        {
            _weights.Clear();
        }

        /// <summary>
        /// Returns a snapshot of all blend weights.
        /// </summary>
        public IReadOnlyDictionary<string, float> GetAllWeights()
        {
            return _weights;
        }
    }
}
