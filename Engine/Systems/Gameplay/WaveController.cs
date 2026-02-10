using System;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Systems.Gameplay
{
    /// <summary>
    /// Controls enemy wave progression, timing, and spawning logic.
    /// </summary>
    public sealed class WaveController
    {
        private readonly List<WaveDefinition> _waves = new();
        private int _currentWaveIndex;
        private bool _active;

        public bool IsActive => _active;
        public int CurrentWave => _currentWaveIndex + 1;
        public int TotalWaves => _waves.Count;

        public void AddWave(WaveDefinition wave)
        {
            if (wave is null)
                throw new ArgumentNullException(nameof(wave));

            _waves.Add(wave);
        }

        public void Start()
        {
            if (_waves.Count == 0)
                throw new InvalidOperationException("No waves defined.");

            _currentWaveIndex = 0;
            _active = true;
        }

        public void Update(float deltaSeconds)
        {
            if (!_active || _currentWaveIndex >= _waves.Count)
                return;

            var wave = _waves[_currentWaveIndex];
            wave.Update(deltaSeconds);

            if (wave.IsComplete)
            {
                _currentWaveIndex++;

                if (_currentWaveIndex >= _waves.Count)
                {
                    _active = false;
                }
                else
                {
                    _waves[_currentWaveIndex].Begin();
                }
            }
        }
    }

    public sealed class WaveDefinition
    {
        public bool IsComplete { get; private set; }

        public void Begin()
        {
            IsComplete = false;
        }

        public void Update(float deltaSeconds)
        {
            // Placeholder logic for wave progression.
            IsComplete = true;
        }
    }
}