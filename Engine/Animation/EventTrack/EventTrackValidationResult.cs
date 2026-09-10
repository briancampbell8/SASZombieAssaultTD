// ====================================================================================================
//  FILE: EventTrackValidationResult.cs
//  PATH: Engine/Animation/EventTrack/EventTrackValidationResult.cs
//  MODULE: Amination EventTrack
//
//  ROLE:
//      Validation result data container for AnimationEventTrack.
//
//  RESPONSIBILITIES:
//      - Hold validation success flag.
//      - Hold validation error messages.
//      - Hold validation warning messages.
//
//  NON-RESPONSIBILITIES:
//      - Performing validation (handled by AnimationEventTrackValidator).
//      - Event triggering or evaluation.
//      - Diagnostics or statistics.
// ====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Animation.EventTrack
{
    public sealed class EventTrackValidationResult
    {
        public float Validate { get; set; }
        public bool IsValid { get; }
        public IReadOnlyList<string> Errors { get; }
        public IReadOnlyList<string> Warnings { get; }

        public EventTrackValidationResult(bool isValid, string[]? errors = null, string[]? warnings = null)
        {
            IsValid = isValid;
            Errors = errors ?? Array.Empty<string>();
            Warnings = warnings ?? Array.Empty<string>();
        }
    }
}
