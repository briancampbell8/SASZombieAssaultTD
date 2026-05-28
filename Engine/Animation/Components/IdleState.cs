/*
File:    IdleState.cs
Purpose: P11-17-03 - Implement IdleState using IAnimationState with deterministic transitions and no placeholder logic.
Provides deterministic idle state with explicit transition conditions and no placeholder behavior.
*/
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.Animation.Core;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Animation.Components
{
    /// <summary>
    /// P11-17-03: Idle state implementation with deterministic transitions.
    /// Implements IAnimationState with explicit transition conditions and no placeholder logic.
    /// </summary>
    public class IdleState : IAnimationState
    {
        public string Name => "Idle";
        public AnimationClip? Clip { get; set; }
        public bool IsMoving { get; set; }
        public bool IsJumping { get; set; }
        public bool IsAttacking { get; set; }
        public float IdleTime { get; private set; }

        private readonly MoveState _moveState;
        private readonly JumpState _jumpState;
        private readonly AttackState _attackState;

        public IdleState(MoveState moveState, JumpState jumpState, AttackState attackState)
        {
            _moveState = moveState ?? throw new System.ArgumentNullException(nameof(moveState));
            _jumpState = jumpState ?? throw new System.ArgumentNullException(nameof(jumpState));
            _attackState = attackState ?? throw new System.ArgumentNullException(nameof(attackState));
        }

        public void Enter()
        {
            IdleTime = 0f;
            Engine.Diagnostics.DebugLogger.LogDebug("DEBUG", "IdleState: Entering idle state");
        }

        public void Exit()
        {
            Engine.Diagnostics.DebugLogger.LogDebug("DEBUG", $"IdleState: Exiting idle state after {IdleTime:F2}s");
        }

        public void Update(float deltaTime, float timeInState)
        {
            IdleTime = timeInState;
        }

        public IAnimationState? CheckTransitions()
        {
            if (IsJumping)
            {
                Engine.Diagnostics.DebugLogger.LogDebug("DEBUG", "IdleState: Transition condition met for JumpState");
                return _jumpState;
            }

            if (IsAttacking)
            {
                Engine.Diagnostics.DebugLogger.LogDebug("DEBUG", "IdleState: Transition condition met for AttackState");
                return _attackState;
            }

            if (IsMoving)
            {
                Engine.Diagnostics.DebugLogger.LogDebug("DEBUG", "IdleState: Transition condition met for MoveState");
                return _moveState;
            }

            return null;
        }

        public Dictionary<string, object> GetParameters()
        {
            return new()
            {
                ["IsMoving"] = IsMoving,
                ["IsJumping"] = IsJumping,
                ["IsAttacking"] = IsAttacking,
                ["IdleTime"] = IdleTime
            };
        }

        public bool IsValid()
        {
            return _moveState != null &&
                   _jumpState != null &&
                   _attackState != null &&
                   !string.IsNullOrEmpty(Name);
        }

        public string GetDebugInfo()
        {
            return $"IdleState: Time={IdleTime:F2}s, Moving={IsMoving}, Jumping={IsJumping}, Attacking={IsAttacking}";
        }
    }
}




