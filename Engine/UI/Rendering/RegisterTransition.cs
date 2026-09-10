// =====================================================================================================
//  FILE: RegisterTransition.cs
//  PATH: Engine/UI/Rendering/
//  SUBSYSTEM: UI Rendering Pipeline
//
//  ROLE:
//      Defines the deterministic registration packet and classification enums for UI transitions.
//      This struct is the canonical data carrier used by the UI rendering pipeline to accept,
//      classify, and trace transitions submitted by higher-level UI subsystems (e.g., MainMenu,
//      MapSelection, HUD panels).
//
//  RESPONSIBILITIES:
//      - Represent a single, fully-described UI transition registration request.
//      - Provide explicit classification via TransitionType, TransitionMode, and TransitionDiagnostic.
//      - Carry timing and behavioral metadata (delay, blocking mode) for deterministic execution.
//      - Serve as the stable intake format for UIRenderer’s transition queue and execution logic.
//
//  NON-RESPONSIBILITIES:
//      - Executing transition animations or frame-by-frame interpolation logic directly.
//      - Managing global UI state machines, menu navigation, or input handling.
//      - Owning renderer resources, GPU command buffers, or low-level device interactions.
//
//  ARCHITECTURAL NOTES:
//      - This struct is a value-type “packet” intended for fast, deterministic handoff into UIRenderer.
//      - Higher-level UI subsystems construct RegisterTransition instances and submit them via
//        UIRenderer.RegisterTransition(RegisterTransition registration).
//      - Enums defined here are tightly scoped to transition registration semantics and should not
//        be repurposed for unrelated subsystems.
// =====================================================================================================

using SASZombieAssaultTD.Engine.UI.MainMenu;

namespace SASZombieAssaultTD.Engine.UI.Rendering
{
    /// <summary>
    /// Classification of the visual or positional behavior of a UI transition.
    /// </summary>
    public enum TransitionType // Fixed: Changed from internal to public
    {
        FadeIn,
        FadeOut,
        SlideLeft,
        SlideRight,
        ScaleUp,
        ScaleDown,
        CrossFade,
        Reveal,
        Hide
    }

    /// <summary>
    /// Execution mode and scheduling behavior for a UI transition.
    /// </summary>
    public enum TransitionMode // Fixed: Changed from internal to public
    {
        /// <summary>
        /// Executes as soon as the renderer processes the registration.
        /// </summary>
        Immediate,

        /// <summary>
        /// Executes after a specified delay interval.
        /// </summary>
        Deferred,

        /// <summary>
        /// Blocks other transitions until completion.
        /// </summary>
        Blocking,

        /// <summary>
        /// Runs in parallel with other transitions and does not block.
        /// </summary>
        NonBlocking
    }

    /// <summary>
    /// Diagnostic verbosity level for a registered transition.
    /// </summary>
    public enum TransitionDiagnostic // Fixed: Changed from internal to public
    {
        /// <summary>
        /// No diagnostic output for this transition.
        /// </summary>
        None,

        /// <summary>
        /// Logs registration and completion events.
        /// </summary>
        Basic,

        /// <summary>
        /// Logs registration, execution start, periodic updates, and completion.
        /// </summary>
        Verbose,

        /// <summary>
        /// Emits full trace information including flags, conditions, timings, and state changes.
        /// </summary>
        Trace
    }

    /// <summary>
    /// Deterministic registration packet for a single UI transition.
    /// </summary>
    public struct RegisterTransition
    {
        /// <summary>
        /// Classification of the transition’s visual/positional behavior.
        /// </summary>
        public TransitionType Type { get; }

        /// <summary>
        /// Execution mode and scheduling behavior.
        /// </summary>
        public TransitionMode Mode { get; }

        /// <summary>
        /// Diagnostic verbosity level for this transition.
        /// </summary>
        public TransitionDiagnostic Diagnostic { get; }

        /// <summary>
        /// The underlying transition instance to be executed by the renderer.
        /// </summary>
        public UITransition Transition { get; }

        /// <summary>
        /// Delay (in seconds) before the transition begins execution.
        /// </summary>
        public float Delay { get; }

        /// <summary>
        /// Indicates whether this transition should block other transitions until completion.
        /// </summary>
        public bool IsBlocking => Mode == TransitionMode.Blocking;

        /// <summary>
        /// Constructs a fully-described transition registration packet.
        /// </summary>
        /// <param name="type">Classification of the transition behavior.</param>
        /// <param name="mode">Execution mode and scheduling behavior.</param>
        /// <param name="diagnostic">Diagnostic verbosity level.</param>
        /// <param name="transition">Underlying transition instance.</param>
        /// <param name="delay">Delay before execution, in seconds.</param>
        public RegisterTransition(
            TransitionType type,
            TransitionMode mode,
            TransitionDiagnostic diagnostic,
            UITransition transition,
            float delay)
        {
            Type = type;
            Mode = mode;
            Diagnostic = diagnostic;
            Transition = transition;
            Delay = delay;
        }
    }
}
