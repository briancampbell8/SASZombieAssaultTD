using System;

namespace SASZombieAssaultTD.Engine.Systems.Gameplay
{
    /// <summary>
    /// Base class for all enemy types.
    /// </summary>
    public abstract class EnemyBase
    {
        public int MaxHealth { get; }
        public int CurrentHealth { get; private set; }
        public float X { get; protected set; }
        public float Y { get; protected set; }

        protected EnemyBase(int maxHealth, float startX, float startY)
        {
            if (maxHealth <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxHealth));

            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
            X = startX;
            Y = startY;
        }

        public bool IsDead => CurrentHealth <= 0;

        public void ApplyDamage(int amount)
        {
            if (amount < 0)
                amount = 0;

            CurrentHealth -= amount;

            if (CurrentHealth < 0)
                CurrentHealth = 0;
        }

        public abstract void Update(float deltaSeconds);
    }
}