// ====================================================================================================
//  FILE: EnemyBehaviorModifier.cs
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
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Waves
{
    /// <summary>
    /// Enemy behavior modifier.
    /// </summary>
    public class EnemyBehaviorModifier
    {
        public string ModifierType { get; set; }
        public float Value { get; set; }
        public float Duration { get; set; }
        public Dictionary<string, object> Parameters { get; set; }

        public EnemyBehaviorModifier() => Parameters = new Dictionary<string, object>();

        public void Apply(Enemy enemy)
        {
            var enemyModifier = new Enemies.EnemyBehaviorModifier
            {
                ModifierType = this.ModifierType,
                Value = this.Value,
                Duration = this.Duration,
                Parameters = this.Parameters ?? new Dictionary<string, object>()
            };
            enemy.ApplyBehaviorModifier(enemyModifier);
        }

        public EnemyBehaviorModifier Clone()
        {
            return new EnemyBehaviorModifier
            {
                ModifierType = this.ModifierType,
                Value = this.Value,
                Duration = this.Duration,
                Parameters = new Dictionary<string, object>(this.Parameters)
            };
        }
    }
}
