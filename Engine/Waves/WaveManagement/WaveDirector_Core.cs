/* ====================================================================================================
 *  FILE: WaveDirector_Core.cs
 *  PATH: Engine/Waves/WaveManagement/WaveDirector_Core.cs
 *  SUBSYSTEM: Waves
 *  ROLE: Internal engine and shared state container for the WaveDirector subsystem.
 *
 *  RESPONSIBILITIES:
 *      - Maintain all private fields, timers, and internal collections.
 *      - Provide shared state access for all WaveDirector partials.
 *      - Enforce deterministic initialization and singleton pattern.
 *      - Contain no gameplay logic, no spawning logic, no notifications, no flow control.
 *
 *  NON-RESPONSIBILITIES:
 *      - Public API exposure (handled by WaveDirector.cs façade).
 *      - Wave logic, spawning, notifications, or stats (handled by respective partials).
 *      - Serialization or external integration.
 *
 *  DEPENDENCIES:
 *      - WaveScript, WaveSpawnGroup, IWaveSpawnGroup
 *      - EnemyManager, DifficultyManager
 *
 *  CALLED BY:
 *      - WaveDirector façade and all partials
 *
 *  CALLS INTO:
 *      - None (pure internal state)
 *
 *  ARCHITECTURAL NOTES:
 *      - Must remain deterministic and free of external dependencies.
 *      - All partials rely on this file for shared state and initialization.
 *      - No public methods beyond singleton accessors and constructor.
 * ==================================================================================================== */

using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Enemies;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Waves.WaveManagement
{
    public partial class WaveDirector
    {
        //===============================================================================================
        // SINGLETON
        //===============================================================================================

        private static WaveDirector _instance;
        public static WaveDirector Instance => _instance ??= new WaveDirector();

        //===============================================================================================
        // CORE INTERNAL STATE
        //===============================================================================================

        //Wave scripts and queue
        internal readonly Dictionary<int, WaveScript> _waveScripts;
        internal readonly Queue<WaveScript> _upcomingWaves;

        //Current wave
        internal WaveScript _currentWave;
        internal WaveState _currentState;
        internal int _currentWaveNumber;
        internal int _totalWaves;

        //Flags
        internal bool _isInitialized;
        internal bool _isPaused;
        internal bool _isGameComplete;

        //Timers
        internal float _waveTimer;
        internal float _interWaveTimer;
        internal float _spawnTimer;
        internal float _currentSpawnDelay;

        //===============================================================================================
        // CONSTRUCTOR
        //===============================================================================================

        private WaveDirector()
        {
            _waveScripts = new Dictionary<int, WaveScript>();
            _upcomingWaves = new Queue<WaveScript>();

            _currentState = WaveState.NotStarted;
            _currentWaveNumber = 0;
            _totalWaves = 0;

            _isInitialized = false;
            _isPaused = false;
            _isGameComplete = false;

            _waveTimer = 0f;
            _interWaveTimer = 0f;
            _spawnTimer = 0f;
            _currentSpawnDelay = 0f;
        }

        //===============================================================================================
        // INTERNAL SHARED HELPERS (NO LOGIC)
        //===============================================================================================

        ///<summary>
        ///Internal helper for resetting timers.
        ///</summary>
        internal void ResetTimers()
        {
            _waveTimer = 0f;
            _interWaveTimer = 0f;
            _spawnTimer = 0f;
            _currentSpawnDelay = 0f;
        }

        ///<summary>
        ///Internal helper for resetting flags.
        ///</summary>
        internal void ResetFlags()
        {
            _isPaused = false;
            _isGameComplete = false;
        }
    }
}
