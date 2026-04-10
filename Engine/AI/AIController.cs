// ROLE: Central controller for enemy AI behavior management.
// RESPONSIBILITY: Maintain and execute a collection of AI behaviors that drive enemy entity 
//                  decision-making during gameplay.
// TRIGGERS: Invoked by UpdateManager during the Update phase.
// INPUTS: Receives delta time from UpdateManager and behavior state from AIContext.
// OUTPUTS: Executes behavior Tick methods to control enemy actions.
// DEPENDENCIES: Self-contained with IAIBehavior interface and AIContext data.
// CONTENTS: AIController class with AddBehavior, RemoveBehavior, Update methods, _behaviors list, 
//           _context, and _enemySystem reference.

#nullable enable

using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.AI
{
    /// <summary>
    /// Central controller for enemy AI behavior management.
    /// Manages a collection of behaviors that drive enemy entity decision-making.
    /// </summary>
    public class AIController
    {
        private readonly List<IAIBehavior> _behaviors = new();
        private readonly AIContext _context = new AIContext();
        private EnemySystem? _enemySystem;

        /// <summary>
        /// Sets the enemy system reference for this AI controller.
        /// Enables coordination with the global enemy management system.
        /// </summary>
        /// <param name="enemySystem">The enemy system to associate with this controller.</param>
        public void SetEnemySystem(EnemySystem enemySystem)
        {
            _enemySystem = enemySystem;
        }

        /// <summary>
        /// Adds a behavior to this AI controller's behavior stack.
        /// Behaviors are evaluated in order each update cycle.
        /// </summary>
        /// <param name="behavior">The AI behavior to add. Must implement IAIBehavior.</param>
        public void AddBehavior(IAIBehavior behavior)
        {
            // FILE PATH: Engine/AI/Behaviors/BasicChaseBehavior.cs
            // EXECUTION TRIGGER: Called by AIController during behavior Update cycle
            // PROGRAM PURPOSE: Simple chase behavior that moves an entity toward a target position using vector-based movement with configurable speed
            // PROGRAM CALLS: AIContext, IMovable, IPositionProvider
            // PROGRAM CONTENTS: BasicChaseBehavior class implementing IAIBehavior with Speed property and Tick method

            if (behavior != null && !_behaviors.Contains(behavior))
                _behaviors.Add(behavior);
        }

        /// <summary>
        /// Sets the target for this AI controller.
        /// Target is shared via AIContext to all behaviors.
        /// </summary>
        /// <param name="target">The target object (typically a tower or player base).</param>
        public void SetTarget(object? target)
        {
            _context.Target = target;
        }

        /// <summary>
        /// Clears the current target, forcing target reacquisition.
        /// Called when current target is destroyed or out of range.
        /// </summary>
        public void AcquireTargets()
        {
            _context.Target = null;
        }

        /// <summary>
        /// Updates the AI controller with the current delta time.
        /// Called every frame by the game loop or entity system.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since last frame in seconds.</param>
        public void Update(float deltaTime)
        {
            _context.DeltaTime = deltaTime;
            EvaluateBehaviors();
        }

        /// <summary>
        /// Evaluates all registered behaviors by ticking them sequentially.
        /// Each behavior receives the shared AIContext for state access.
        /// </summary>
        public void EvaluateBehaviors()
        {
            for (int i = 0; i < _behaviors.Count; i++)
                _behaviors[i].Tick(_context);
        }

        /// <summary>
        /// Updates the AI controller with a provided context.
        /// Alternative update path for context injection from external systems.
        /// </summary>
        /// <param name="context">The AI context containing state information.</param>
        public void Update(AIContext context)
        {
            foreach (var behavior in _behaviors)
                behavior.Tick(context);
        }
    }

    /// <summary>
    /// Shared context container for AI behaviors.
    /// Provides state communication between behaviors and the AI controller.
    /// </summary>
    public class AIContext
    {
        /// <summary>
        /// Time elapsed since last frame in seconds.
        /// Used for frame-rate independent calculations.
        /// </summary>
        public float DeltaTime { get; set; }

        /// <summary>
        /// The entity that owns this AI context (the enemy entity).
        /// </summary>
        public object? Owner { get; set; }

        /// <summary>
        /// The current target of the AI (tower, player base, etc.).
        /// Updated by AIController.SetTarget() or behavior logic.
        /// </summary>
        public object? Target { get; set; }
    }

    /// <summary>
    /// Interface for AI behaviors that can be registered with AIController.
    /// Implementations define specific enemy behaviors (chase, attack, patrol, etc.).
    /// </summary>
    public interface IAIBehavior
    {
        /// <summary>
        /// Executes one tick of this behavior.
        /// Called every frame by AIController during EvaluateBehaviors().
        /// </summary>
        /// <param name="context">Shared AI context containing state and timing information.</param>
        void Tick(AIContext context);
    }
}




