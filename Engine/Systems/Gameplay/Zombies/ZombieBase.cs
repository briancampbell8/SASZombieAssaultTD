using SASZombieAssaultTD.Engine.Systems.Gameplay;

namespace SASZombieAssaultTD.Engine.Systems.Gameplay.Zombies
{
    public abstract class ZombieBase : EnemyBase
    {
        protected ZombieBase(int maxHealth, float startX, float startY)
            : base(maxHealth, startX, startY)
        {
        }
    }
}