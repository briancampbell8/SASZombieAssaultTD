// ====================================================================================================
//  FILE: AttackState.cs
//  PATH: Engine\Animation\Core\State\AttackState.cs
//  SUBSYSTEM: Animation Core State
//
//  ROLE:
//      Encapsulate core engine behavior for the AttackState module.
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
File:    AttackState.cs
Purpose: P11-17-05 - Implement AttackState using IAnimationState with explicit transition conditions and deterministic Update behavior.
Provides deterministic attack state with explicit transition conditions and no placeholder behavior.
*/
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Animation.Core.Clips;
using SASZombieAssaultTD.Engine.Diagnostics;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
namespace SASZombieAssaultTD.Engine.Animation.Core.State
//
{
    ///<summary>
    ///P11-17-05: Attack state implementation with deterministic transitions.
    ///Implements IAnimationState with explicit transition conditions and deterministic Update behavior.
    ///</summary>
    public class AttackState : IAnimationState
    {
        public string Name => "Attack";
        public AnimationClip? Clip { get; set; }
        public float AttackDuration { get; set; } = 1.0f;
        public float AttackDamage { get; private set; }
        public bool IsAttacking { get; private set; }
        public float AttackTime { get; private set; }

        private readonly IdleState _idleState;
        private readonly MoveState _moveState;
        private readonly JumpState _jumpState;

        private const float ATTACK_COMPLETION_THRESHOLD = 0.8f;

        public AttackState(IdleState idleState, MoveState moveState, JumpState jumpState)
        {
            _idleState = idleState ?? throw new System.ArgumentNullException(nameof(idleState));
            _moveState = moveState ?? throw new System.ArgumentNullException(nameof(moveState));
            _jumpState = jumpState ?? throw new System.ArgumentNullException(nameof(jumpState));
        }

        public void Enter()
        {
            AttackTime = 0f;
            IsAttacking = true;
            DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Debug, $"AttackState: Entering attack state with duration {AttackDuration:F2}s");
        }

        public void Exit()
        {
            IsAttacking = false;
            AttackDamage = 0f;
            DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Debug, $"AttackState: Exiting attack state after {AttackTime:F2}s");
        }

        public void Update(float deltaTime, float timeInState)
        {
            AttackTime = timeInState;

            if (AttackTime < AttackDuration * ATTACK_COMPLETION_THRESHOLD)
            {
                AttackDamage = 0f; //Winding up
            }
            else
            {
                AttackDamage = 10f; //Fixed damage for deterministic behavior
            }
        }

        public IAnimationState? CheckTransitions()
        {
            if (_jumpState.IsJumping)
            {
                DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Debug, "AttackState: Transition condition met for JumpState");
                return _jumpState;
            }

            if (_moveState.IsMoving)
            {
                DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Debug, "AttackState: Transition condition met for MoveState");
                return _moveState;
            }

            if (AttackTime >= AttackDuration)
            {
                DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Debug, "AttackState: Transition condition met for IdleState (attack completed)");
                return _idleState;
            }

            return null;
        }

        public Dictionary<string, object> GetParameters()
        {
            return new()
            {
                ["AttackDuration"] = AttackDuration,
                ["AttackDamage"] = AttackDamage,
                ["IsAttacking"] = IsAttacking,
                ["AttackTime"] = AttackTime
            };
        }

        public bool IsValid()
        {
            return _idleState != null &&
                   _moveState != null &&
                   _jumpState != null &&
                   !string.IsNullOrEmpty(Name) &&
                   AttackDuration > 0f &&
                   ATTACK_COMPLETION_THRESHOLD > 0f &&
                   ATTACK_COMPLETION_THRESHOLD < 1f;
        }

        public string GetDebugInfo()
        {
            return $"AttackState: Time={AttackTime:F2}s, Duration={AttackDuration:F2}s, Damage={AttackDamage:F2}, Attacking={IsAttacking}";
        }
    }
}





