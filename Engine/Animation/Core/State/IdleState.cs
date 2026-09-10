// ====================================================================================================
//  FILE: IdleState.cs
//  PATH: Engine/Animation/Core/State/IdleState.cs
//  SUBSYSTEM: Animation Core State
//
//  ROLE:
//      Encapsulate core engine behavior for the IdleState module.
//
//  RESPONSIBILITIES:
//      - Provide Enter() behavior for the Core subsystem.
//      - Provide Exit() behavior for the Core subsystem.
//      - Provide Update() behavior for the Core subsystem.
//      - Provide CheckTransitions() behavior for the Core subsystem.
//      - Provide IsValid() behavior for the Core subsystem.
//      - Provide GetDebugInfo() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
/*
File:    IdleState.cs
Purpose: P11-17-03 - Implement IdleState using IAnimationState with deterministic transitions and no placeholder logic.
Provides deterministic idle state with explicit transition conditions and no placeholder behavior.
*/
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Animation.Core.Clips;
using SASZombieAssaultTD.Engine.Diagnostics;
namespace SASZombieAssaultTD.Engine.Animation.Core.State
//
{
    ///<summary>
    ///P11-17-03: Idle state implementation with deterministic transitions.
    ///Implements IAnimationState with explicit transition conditions and no placeholder logic.
    ///</summary>
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
            DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Debug, "IdleState: Entering idle state");
        }

        public void Exit()
        {
            DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Debug, $"IdleState: Exiting idle state after {IdleTime:F2}s");
        }

        public void Update(float deltaTime, float timeInState)
        {
            IdleTime = timeInState;
        }

        public IAnimationState? CheckTransitions()
        {
            if (IsJumping)
            {
                DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Debug, "IdleState: Transition condition met for JumpState");
                return _jumpState;
            }

            if (IsAttacking)
            {
                DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Debug, "IdleState: Transition condition met for AttackState");
                return _attackState;
            }

            if (IsMoving)
            {
                DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Debug, "IdleState: Transition condition met for MoveState");
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





