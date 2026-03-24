/*
File:    MoveState.cs
Purpose: P11-17-04 - Implement MoveState using IAnimationState with explicit transition conditions and deterministic Update behavior.
Provides deterministic movement state with explicit transition conditions and no placeholder behavior.
*/
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.Animation.Core;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Animation.Components
{
    /// <summary>
    /// P11-17-04: Movement state implementation with deterministic transitions.
    /// Implements IAnimationState with explicit transition conditions and deterministic Update behavior.
    /// </summary>
    public class MoveState : IAnimationState
    {
        /// <summary>
        /// P11-17-04: State name identifier.
        /// Deterministic state identification.
        /// </summary>
        public string Name => "Move";

        /// <summary>
        /// P11-17-02: Animation clip associated with this state.
        /// Provides deterministic clip association for state-based animation.
        /// </summary>
        public AnimationClip? Clip { get; set; }

        /// <summary>
        /// P11-17-04: Movement speed parameter.
        /// Deterministic parameter for movement speed tracking.
        /// </summary>
        public float MovementSpeed { get; set; }

        /// <summary>
        /// P11-17-04: Movement direction parameter.
        /// Deterministic parameter for movement direction tracking.
        /// </summary>
        public float MovementDirection { get; set; }

        /// <summary>
        /// P11-17-04: IsMoving flag for transition checking.
        /// Deterministic parameter for movement state validation.
        /// </summary>
        public bool IsMoving { get; private set; }

        /// <summary>
        /// P11-17-04: Jump flag for transition checking.
        /// Deterministic parameter for jump detection during movement.
        /// </summary>
        public bool IsJumping { get; set; }

        /// <summary>
        /// P11-17-04: Attack flag for transition checking.
        /// Deterministic parameter for attack detection during movement.
        /// </summary>
        public bool IsAttacking { get; set; }

        /// <summary>
        /// P11-17-04: Time spent in move state.
        /// Deterministic timing for state duration tracking.
        /// </summary>
        public float MoveTime { get; private set; }

        /// <summary>
        /// P11-17-04: Reference to IdleState for transitions.
        /// Explicit state reference for deterministic transitions.
        /// </summary>
        private readonly IdleState _idleState;

        /// <summary>
        /// P11-17-04: Reference to JumpState for transitions.
        /// Explicit state reference for deterministic transitions.
        /// </summary>
        private readonly JumpState _jumpState;

        /// <summary>
        /// P11-17-04: Reference to AttackState for transitions.
        /// Explicit state reference for deterministic transitions.
        /// </summary>
        private readonly AttackState _attackState;

        /// <summary>
        /// P11-17-04: Movement threshold for determining if entity is actually moving.
        /// Deterministic threshold for movement validation.
        /// </summary>
        private const float MOVEMENT_THRESHOLD = 0.1f;

        /// <summary>
        /// P11-17-04: Constructor with explicit state dependencies.
        /// Initializes move state with required transition targets.
        /// </summary>
        /// <param name="idleState">Target state for idle transitions.</param>
        /// <param name="jumpState">Target state for jump transitions.</param>
        /// <param name="attackState">Target state for attack transitions.</param>
        public MoveState(IdleState idleState, JumpState jumpState, AttackState attackState)
        {
            _idleState = idleState ?? throw new System.ArgumentNullException(nameof(idleState));
            _jumpState = jumpState ?? throw new System.ArgumentNullException(nameof(jumpState));
            _attackState = attackState ?? throw new System.ArgumentNullException(nameof(attackState));
        }

        /// <summary>
        /// P11-17-04: Called when entering the move state.
        /// Explicit initialization with no side effects beyond state setup.
        /// </summary>
        public void Enter()
        {
            MoveTime = 0f;
            ModernLoggingSystem.Log("DEBUG", $"MoveState: Entering move state with speed {MovementSpeed:F2}");
        }

        /// <summary>
        /// P11-17-04: Called when exiting the move state.
        /// Explicit cleanup with no side effects beyond state cleanup.
        /// </summary>
        public void Exit()
        {
            ModernLoggingSystem.Log("DEBUG", $"MoveState: Exiting move state after {MoveTime:F2}s");
        }

        /// <summary>
        /// P11-17-04: Updates move state logic with deterministic behavior.
        /// Updates state timing and movement parameters without external side effects.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since last update in seconds.</param>
        /// <param name="timeInState">Total time elapsed in current state in seconds.</param>
        public void Update(float deltaTime, float timeInState)
        {
            MoveTime = timeInState;

            // Deterministic movement validation based on speed threshold
            IsMoving = MovementSpeed > MOVEMENT_THRESHOLD;

            // No placeholder logic - only update timing and validate movement
            // State transitions are handled by CheckTransitions method
        }

        /// <summary>
        /// P11-17-04: Checks for state transitions with deterministic conditions.
        /// Returns next state if transition conditions are met, null otherwise.
        /// No side effects - only evaluates transition conditions.
        /// </summary>
        /// <returns>Next state to transition to, or null if no transition.</returns>
        public IAnimationState? CheckTransitions()
        {
            // Priority order: Jump > Attack > Idle
            // Deterministic transition checking with explicit conditions

            if (IsJumping)
            {
                ModernLoggingSystem.Log("DEBUG", "MoveState: Transition condition met for JumpState");
                return _jumpState;
            }

            if (IsAttacking)
            {
                ModernLoggingSystem.Log("DEBUG", "MoveState: Transition condition met for AttackState");
                return _attackState;
            }

            // Transition to idle if movement speed falls below threshold
            if (!IsMoving)
            {
                ModernLoggingSystem.Log("DEBUG", "MoveState: Transition condition met for IdleState (movement stopped)");
                return _idleState;
            }

            // No transition conditions met
            return null;
        }

        /// <summary>
        /// P11-17-04: Gets state-specific parameters for debugging and inspection.
        /// Returns deterministic parameter values without side effects.
        /// </summary>
        /// <returns>Dictionary of state parameters.</returns>
        public Dictionary<string, object> GetParameters()
        {
            return new()
            {
                ["MovementSpeed"] = MovementSpeed,
                ["MovementDirection"] = MovementDirection,
                ["IsMoving"] = IsMoving,
                ["IsJumping"] = IsJumping,
                ["IsAttacking"] = IsAttacking,
                ["MoveTime"] = MoveTime
            };
        }

        /// <summary>
        /// P11-17-04: Validates state configuration and dependencies.
        /// Returns validation result without side effects.
        /// </summary>
        /// <returns>True if state is valid, false otherwise.</returns>
        public bool IsValid()
        {
            return _idleState != null &&
                   _jumpState != null &&
                   _attackState != null &&
                   !string.IsNullOrEmpty(Name) &&
                   MovementSpeed >= 0f;
        }

        /// <summary>
        /// P11-17-04: Gets debug information about the move state.
        /// Returns deterministic debug information without side effects.
        /// </summary>
        /// <returns>Debug information string.</returns>
        public string GetDebugInfo()
        {
            return $"MoveState: Time={MoveTime:F2}s, Speed={MovementSpeed:F2}, Direction={MovementDirection:F2}, Moving={IsMoving}, Jumping={IsJumping}, Attacking={IsAttacking}";
        }
    }
}




