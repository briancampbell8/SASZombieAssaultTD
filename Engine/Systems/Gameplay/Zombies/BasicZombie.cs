using SASZombieAssaultTD.Engine.Systems.Gameplay;

namespace SASZombieAssaultTD.Engine.Systems.Gameplay.Zombies
{
    /// <summary>
    /// A simple zombie enemy with basic movement and health.
    /// </summary>
    public sealed class BasicZombie : EnemyBase
    {
        private float _speed;

        public BasicZombie(int maxHealth, float startX, float startY, float speed)
            : base(maxHealth, startX, startY)
        {
            _speed = speed;
        }

        /// <summary>
        /// Updates zombie movement and behavior.
        /// </summary>
        public override void Update(float deltaSeconds)
        {
            // Simple placeholder movement: move right at constant speed.
            X += _speed * deltaSeconds;

            // Additional behavior can be added here later.
        }

        /// <summary>
        /// Applies damage to the zombie.
        /// </summary>
        public void TakeDamage(int amount)
        {
            ApplyDamage(amount);
        }
    }
}