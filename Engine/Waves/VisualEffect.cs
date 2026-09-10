// ====================================================================================================
//  FILE: VisualEffect.cs
//  PATH: ./Engine/Waves/
//  MODULE: WaveDirector
//
//  ROLE:
//      Load, validate, and construct wave definitions for the WaveDirector subsystem.
//
//  RESPONSIBILITIES:
//      - Provide GetEffectiveSpawnDelay() behavior for the WaveDirector subsystem.
//      - Provide GetEffectiveCount() behavior for the WaveDirector subsystem.
//      - Provide GetSpawnPosition() behavior for the WaveDirector subsystem.
//      - Provide ApplyEnemyModifications() behavior for the WaveDirector subsystem.
//      - Provide AreSpawnConditionsMet() behavior for the WaveDirector subsystem.
//      - Provide ShouldSpawnEnemy() behavior for the WaveDirector subsystem.
//      - Provide OnGroupCompletedCallback() behavior for the WaveDirector subsystem.
//      - Provide OnEnemySpawnedCallback() behavior for the WaveDirector subsystem.
//      - Provide GetDescription() behavior for the WaveDirector subsystem.
//      - Provide Validate() behavior for the WaveDirector subsystem.
//      - Provide Clone() behavior for the WaveDirector subsystem.
//      - Provide Apply() behavior for the WaveDirector subsystem.
//      - Provide Clone() behavior for the WaveDirector subsystem.
//      - Provide Clone() behavior for the WaveDirector subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Interfaces;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Waves
{
    /// <summary>
    /// Visual effect for enemies.
    /// </summary>
    public class VisualEffect
    {
        // Container to strongly type the debug colors and resolve CS1061
        public class DebugColorContainer
        {
            public object Highlight { get; set; }
            public object Info { get; set; }
        }

        public string EffectType { get; set; }
        public WaveSpawnGroup.Color Color { get; set; }
        public float Intensity { get; set; }
        public float Duration { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
        public object DebugColors { get; private set; }
        public float AnimationTime { get; private set; }
        public DebugColorContainer DebugColor { get; private set; }
        public bool IsExpired { get; internal set; }

        public VisualEffect() => Parameters = new Dictionary<string, object>();

        public VisualEffect Clone()
        {
            return new VisualEffect
            {
                EffectType = this.EffectType,
                Color = this.Color,
                Intensity = this.Intensity,
                Duration = this.Duration,
                Parameters = new Dictionary<string, object>(this.Parameters)
            };
        }

        internal void Render(IDebugRenderer debugRenderer, Vector3 position, float animationTime)
        {
            if (debugRenderer == null || DebugColor == null)
                return;

            debugRenderer.DrawPoint(position, DebugColor.Highlight);
            debugRenderer.DrawText(position, $"t={animationTime:F2}", DebugColor.Info);
        }

        internal void Update(float animationTime)
        {
            // Deterministic animation progression
            AnimationTime = animationTime;

            // Example: clamp or normalize if needed
            if (AnimationTime < 0f)
                AnimationTime = 0f;
        }

        /// <summary>
        /// Game state for wave system.
        /// </summary>
        public class GameState
        {
            public int PlayerLevel { get; set; }
            public int BuiltTowers { get; set; }
            public float GameSpeed { get; set; }
            public bool IsPaused { get; set; }

            public GameState()
            {
                PlayerLevel = 1;
                BuiltTowers = 0;
                GameSpeed = 1f;
                IsPaused = false;
            }
        }
    }
}
