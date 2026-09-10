// =====================================================================================================
//  FILE: AnimationEnums.cs
//  PATH: Engine/Animation/AnimationEnums.cs
//  SUBSYSTEM: Animation
//
//  ROLE:
//      Defines All enumerations used by the Animation Module
//
//  RESPONSIBILITIES:
//      - Provide a strict, minimal lifecycle surface for program orchestration.
//      - Enforce the structural sequencing contract: Initialize → Run Loop Execution → Shutdown.
//      - Serve as the base contract for any future top-level engine-hosted program modules.
//
//  NON-RESPONSIBILITIES:
//      - Implementing deep frame-level update calculation rules or rendering commands directly.
//      - Managing active systems registration pools, engine assets, or game states.
//      - Handling discrete hardware device allocation boundaries.
//
//  ARCHITECTURAL NOTES:
//      - This interface replaces the legacy GameRoot partial lifecycle methods.
//      - GameRootMain implements this interface and delegates to its subsystems:
//          • GameRootInitialization
//          • GameRootUpdateLoop
//          • GameRootStateController
//          • GameRootSystemRegistration
//      - All engine-hosted programs MUST implement this interface without exception.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.Animation
{
    public class AnimationEnums
    {
        //------------------------------------------------------------------------------------------------
        // ANIMATION PLAYBACK MODE
        //------------------------------------------------------------------------------------------------
        public enum AnimationPlaybackMode
        {
            Once,
            Loop
        }

        //------------------------------------------------------------------------------------------------
        // VALIDATION SEVERITY
        //------------------------------------------------------------------------------------------------
        public enum ValidationSeverity
        {
            /// <summary>
            /// Informational message that doesn't affect functionality.
            /// </summary>
            Info,

            /// <summary>
            /// Warning that might cause issues but won't break functionality.
            /// </summary>
            Warning,

            /// <summary>
            /// Error that will cause functional problems.
            /// </summary>
            Error,

            /// <summary>
            /// Critical error that will prevent the blend tree from working.
            /// </summary>
            Critical
        }

        //------------------------------------------------------------------------------------------------
        // ANIMATION CONDITION OPERATOR
        //------------------------------------------------------------------------------------------------
        public enum AnimationConditionOperator
        {
            Equals,
            NotEquals,
            GreaterThan,
            LessThan,
            GreaterThanOrEqual,
            LessThanOrEqual,
            Contains,
            StartsWith,
            EndsWith,
            WithinRange,
            OutsideRange
        }

        //------------------------------------------------------------------------------------------------
        // ANIMATION TRACK TYPE
        //------------------------------------------------------------------------------------------------
        public enum AnimationTrackType
        {
            /// <summary>Sprite index track for animation frames.</summary>
            SpriteIndex,

            /// <summary>Transform offset track for position/rotation/scale changes.</summary>
            TransformOffset,

            /// <summary>Color tint track for color changes.</summary>
            ColorTint,

            /// <summary>Custom parameter track for user-defined properties.</summary>
            Custom,

            /// <summary>Visibility track for show/hide states.</summary>
            Visibility
        }

        //------------------------------------------------------------------------------------------------
        // ANIMATION TRANSITION ACTION TYPE
        //------------------------------------------------------------------------------------------------
        public enum AnimationTransitionActionType
        {
            /// <summary>
            /// Sets a parameter value.
            /// </summary>
            SetParameter,

            /// <summary>
            /// Fires an event.
            /// </summary>
            FireEvent,

            /// <summary>
            /// Plays an animation clip.
            /// </summary>
            PlayClip
        }

        //------------------------------------------------------------------------------------------------
        // DEATH TYPE
        //------------------------------------------------------------------------------------------------
        public enum DeathType
        {
            Explosion,
            Electric,
            Fire,
            Freeze
        }
    }
}
