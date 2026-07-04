/*
File:    JumpState.cs
Purpose: P11-17-05 - Implement JumpState using IAnimationState with explicit transition conditions and deterministic Update behavior.
Provides deterministic jump state with explicit transition conditions and no placeholder behavior.
*/
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Animation.Core;
using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Animation.Components

{
    ///<summary>
    ///P11-17-05: Jump state implementation with deterministic transitions.
    ///Implements IAnimationState with explicit transition conditions and deterministic Update behavior.
    ///</summary>
    public class JumpState : IAnimationState
    {
        public string Name => "Jump";
        public AnimationClip? Clip { get; set; }
        public float JumpHeight { get; set; }
        public float JumpVelocity { get; private set; }
        public bool IsGrounded { get; private set; }
        public bool IsAttacking { get; set; }
        public float JumpTime { get; private set; }
        public bool IsJumping { get; private set; }

        private readonly IdleState _idleState;
        private readonly MoveState _moveState;
        private readonly AttackState _attackState;

        private const float JUMP_DURATION_THRESHOLD = 1.0f;
        private const float GRAVITY = 9.81f;

        public JumpState(IdleState idleState, MoveState moveState, AttackState attackState)
        {
            _idleState = idleState ?? throw new System.ArgumentNullException(nameof(idleState));
            _moveState = moveState ?? throw new System.ArgumentNullException(nameof(moveState));
            _attackState = attackState ?? throw new System.ArgumentNullException(nameof(attackState));
        }

        public void Enter()
        {
            JumpTime = 0f;
            IsGrounded = false;
            IsJumping = true;
            JumpVelocity = System.MathF.Sqrt(2f * GRAVITY * JumpHeight);
            DLogger.Log(LogSubsystems.Animation, LogLevel.Debug, $"JumpState: Entering jump state with height {JumpHeight:F2}");
        }

        public void Exit()
        {
            JumpVelocity = 0f;
            IsJumping = false;
            DLogger.Log(LogSubsystems.Animation, LogLevel.Debug, $"JumpState: Exiting jump state after {JumpTime:F2}s");
        }

        public void Update(float deltaTime, float timeInState)
        {
            JumpTime = timeInState;

            //Deterministic jump physics calculation
            JumpVelocity -= GRAVITY * deltaTime;
            JumpHeight += JumpVelocity * deltaTime;

            //Ground detection
            if (JumpHeight <= 0f)
            {
                JumpHeight = 0f;
                IsGrounded = true;
                IsJumping = false;
            }
        }

        public IAnimationState? CheckTransitions()
        {
            if (IsAttacking)
            {
                DLogger.Log(LogSubsystems.Animation, LogLevel.Debug, "JumpState: Transition condition met for AttackState");
                return _attackState;
            }

            if (IsGrounded)
            {
                if (_moveState.IsMoving)
                {
                    DLogger.Log(LogSubsystems.Animation, LogLevel.Debug, "JumpState: Transition condition met for MoveState (landing while moving)");
                    return _moveState;
                }

                DLogger.Log(LogSubsystems.Animation, LogLevel.Debug, "JumpState: Transition condition met for IdleState (landing while idle)");
                return _idleState;
            }

            if (JumpTime >= JUMP_DURATION_THRESHOLD)
            {
                DLogger.Log(LogSubsystems.Animation, LogLevel.Debug, $"JumpState: Transition condition met for IdleState (jump duration exceeded {JUMP_DURATION_THRESHOLD}s)");
                return _idleState;
            }

            return null;
        }

        public Dictionary<string, object> GetParameters()
        {
            return new()
            {
                ["JumpHeight"] = JumpHeight,
                ["JumpVelocity"] = JumpVelocity,
                ["IsGrounded"] = IsGrounded,
                ["IsAttacking"] = IsAttacking,
                ["JumpTime"] = JumpTime
            };
        }

        public bool IsValid()
        {
            return _idleState != null &&
                   _moveState != null &&
                   _attackState != null &&
                   !string.IsNullOrEmpty(Name) &&
                   JumpHeight >= 0f &&
                   JUMP_DURATION_THRESHOLD > 0f &&
                   GRAVITY > 0f;
        }

        public string GetDebugInfo()
        {
            return $"JumpState: Time={JumpTime:F2}s, Height={JumpHeight:F2}, Velocity={JumpVelocity:F2}, Grounded={IsGrounded}, Attacking={IsAttacking}";
        }
    }
}




