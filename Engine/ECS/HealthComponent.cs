namespace SASZombieAssaultTD.Engine.ECS
{
    /// <summary>
    /// Health component for damage and lifecycle management.
    /// </summary>
    public class HealthComponent
    {
        public double CurrentHealth { get; set; }
        public double MaxHealth { get; set; }

        public double HealthPercentage => MaxHealth > 0 ? CurrentHealth / MaxHealth : 0;
        public bool IsDead => CurrentHealth <= 0;
        public bool IsDamaged => CurrentHealth < MaxHealth;
        public float DamageTaken => (float)(MaxHealth - CurrentHealth);

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