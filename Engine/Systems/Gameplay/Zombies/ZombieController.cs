using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Systems.Gameplay.Zombies
{
    /// <summary>
    /// Manages a collection of zombies and updates them each frame.
    /// </summary>
    public sealed class ZombieController
    {
        private readonly List<ZombieBase> _zombies = new();

        public void Add(ZombieBase zombie)
        {
            if (zombie is null)
                return;

            _zombies.Add(zombie);
        }

        public void Update(float deltaSeconds)
        {
            for (int i = _zombies.Count - 1; i >= 0; i--)
            {
                var z = _zombies[i];
                z.Update(deltaSeconds);

                if (z.IsDead)
                {
                    _zombies.RemoveAt(i);
                }
            }
        }
    }
}