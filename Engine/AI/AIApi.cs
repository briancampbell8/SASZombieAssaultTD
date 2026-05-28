// File:    AIApi.cs
// Purpose: Core AI interfaces and context definitions.
//          Defines the contract for AI behaviors and shared context data.
//
// Architecture:
// - IAIBehavior interface for all AI behavior implementations
// - AIContext class is canonical in Engine/AI/AIContext.cs (do not duplicate)
// - EnemySystem reference for global enemy management
//
// Usage:
//    public class ChaseBehavior : IAIBehavior
//    {
//        public float DeltaTime { get; set; }
//        public object Owner { get; set; }
//        public object GetTarget() => ...;
//        public void SetTarget(object value) { ...; }
//    }
//

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.AI
{
    /// <summary>
    /// Interface for all AI behavior implementations.
    /// Defines the contract for behavior execution in the AI system.
    /// </summary>
    // public interface IAIBehavior DUPLICATE - canonical definition lives in Engine/AI/IAIBehavior.cs
    //{
    //    /// <summary>
    //    /// Time elapsed since last frame in seconds.
    //    /// Used for frame-rate independent movement calculations.
    //    /// </summary>
    //    float DeltaTime { get; set; }

        /// <summary>
        /// The entity that owns this AI controller.
        /// Typically implements IMovable for movement behaviors.
        /// </summary>
    //    object Owner { get; set; }

        /// <summary>
        /// The current target for this AI.
        /// Typically implements IPositionProvider for targeting.
        /// Can be null when no target is available.
        /// </summary>
    //    object GetTarget();

        /// <summary>
        /// Sets the current target for this AI.
        /// </summary>
        /// <param name="value">Target object (may be null).</param>
    //    void SetTarget(object value);
    // }

    /// <summary>
    /// Reference to the global enemy management system.
    /// Used for AI coordination with enemy spawning and management.
    /// Placeholder — actual implementation lives in the game systems.
    /// </summary>
    public class EnemySystem
    {
        // Implementation lives in the game's enemy management system.
    }
}
