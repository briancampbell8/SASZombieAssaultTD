using System;
using System.Collections.Generic;
using System.Drawing;

namespace SASZombieAssaultTD.Engine.Enemies
{
    /// <summary>
    /// Handles the spawning of zombies at a specified interval and spawn point.
    /// </summary>
    public class ZombieSpawner
    {
        /// <summary>
        /// Gets or sets the spawn point for zombies.
        /// </summary>
        public PointF SpawnPoint { get; set; }

        /// <summary>
        /// Gets or sets the interval (in seconds) between zombie spawns.
        /// </summary>
        public float SpawnInterval { get; set; } = 2.0f;

        private float _timer;
        private readonly List<ZombieMovement> _spawned = new();

        /// <summary>
        /// Gets a read-only list of all spawned zombies.
        /// </summary>
        public IReadOnlyList<ZombieMovement> Spawned => _spawned;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZombieSpawner"/> class.
        /// </summary>
        /// <param name="spawnPoint">The initial spawn point for zombies.</param>
        public ZombieSpawner(PointF spawnPoint)
        {
            SpawnPoint = spawnPoint;
        }

        /// <summary>
        /// Updates the spawner, spawning zombies at the specified interval.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last update.</param>
        public void Update(float deltaTime)
        {
            _timer += deltaTime;
            if (_timer >= SpawnInterval)
            {
                _timer -= SpawnInterval;
                SpawnZombie();
            }
        }

        /// <summary>
        /// Spawns a new zombie at the spawn point.
        /// </summary>
        private void SpawnZombie()
        {
            var zombie = new ZombieMovement(SpawnPoint);
            _spawned.Add(zombie);
            LogSpawn(zombie);
        }

        /// <summary>
        /// Logs the spawning of a zombie.
        /// </summary>
        /// <param name="zombie">The spawned zombie.</param>
        private void LogSpawn(ZombieMovement zombie)
        {
            System.Diagnostics.Debug.WriteLine($"[ZombieSpawner] Spawned zombie at {SpawnPoint} (total: {_spawned.Count})");
        }
    }
}




