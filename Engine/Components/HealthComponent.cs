// ====================================================================================================
//  FILE: HealthComponent.cs
//  PATH: Engine/Components/HealthComponent.cs
//  SUBSYSTEM: Engine Components
//
//  ROLE:
//      Core engine component for representing health state. Stores current/max health values and
//      provides deterministic mutator methods used by gameplay, combat, and hit‑resolution systems.
//
//  RESPONSIBILITIES:
//      - Store current and maximum health values
//      - Provide TakeDamage(), Heal(), and Revive() operations
//      - Expose health state (IsAlive, IsDead, IsDamaged, DamageTaken)
//      - Provide percentage-based health queries for UI and gameplay systems
//
//  NON-RESPONSIBILITIES:
//      - Executing combat logic or applying damage to other entities
//      - Managing world-level ECSEntityCore allocation or lifecycle sequencing
//      - Performing physics, collision, or movement calculations
//      - Acting as a system or orchestrator
//
//  ARCHITECTURAL NOTES:
//      - This is a pure engine component with minimal behavioral logic
//      - All continuous numeric values use double for precision
//      - Integrates with combat, damage, and hit‑resolution systems but does not implement them
// ====================================================================================================


using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Components
{
    /// <summary>
    /// Health component for damage and lifecycle management.
    /// </summary>
    public class HealthComponent
    {
        public double CurrentHealth { get; set; }

        public Action FullRestore { get; set; }

        public double MaxHealth { get; set; }

        public bool IsAlive => CurrentHealth > 0;
        public bool IsDead => CurrentHealth <= 0;
        public bool IsDamaged => CurrentHealth < MaxHealth;
        public float DamageTaken => (float)(MaxHealth - CurrentHealth);

        public double Health => CurrentHealth;

        public double HealthPercentage => MaxHealth > 0 ? CurrentHealth / MaxHealth : 0;

        /// <summary>
        /// Required so object initializers work.
        /// </summary>
        public HealthComponent()
        {
            CurrentHealth = 0;
            MaxHealth = 0;
        }

        /// <summary>
        /// Convenience constructor for setting max health.
        /// </summary>
        public HealthComponent(double maxHealth)
        {
            CurrentHealth = maxHealth;
            MaxHealth = maxHealth;
        }

        public void TakeDamage(float damage)
        {
            CurrentHealth = global::System.Math.Max(0, CurrentHealth - damage);
        }

        public void Heal(float amount)
        {
            CurrentHealth = global::System.Math.Min(MaxHealth, CurrentHealth + amount);
        }

        public void Revive(float healthPercentage = 1.0f)
        {
            CurrentHealth = MaxHealth * global::System.Math.Clamp(healthPercentage, 0f, 1f);
        }
    }
}
