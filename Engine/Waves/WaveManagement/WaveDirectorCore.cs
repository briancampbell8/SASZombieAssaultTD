// ====================================================================================================
//  FILE: WaveDirectorCore.cs
//  PATH: Engine/Waves/WaveManagement/WaveDirectorCore.cs
//  SUBSYSTEM: Waves
// ====================================================================================================

using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Waves.WaveManagement
{
    public class WaveDirectorCore
    {
        //===============================================================================================
        // SINGLETON (Thread-Safe Static Initialization)
        //===============================================================================================

        // Static constructors in C# are guaranteed to execute deterministically and thread-safely
        private static readonly WaveDirectorCore _instance = new WaveDirectorCore();
        public static WaveDirectorCore Instance => _instance;

        public int CurrentWave { get; internal set; }
        public float WaveProgress { get; internal set; }
        public int TotalWaves { get; internal set; }
        public float OverallProgress { get; internal set; }
        public bool InProgress { get; internal set; }
        public object DifficultyManager { get; internal set; }

        //===============================================================================================
        // CORE INTERNAL STATE
        //===============================================================================================

        internal readonly Dictionary<int, WaveScript> _waveScripts;
        internal readonly Queue<WaveScript> _upcomingWaves;

        internal WaveScript _currentWave;
        internal WaveState _currentState;
        internal int _currentWaveNumber;
        internal int _totalWaves;

        internal bool _isInitialized;
        internal bool _isPaused;
        internal bool _isGameComplete;

        internal float _waveTimer;
        internal float _interWaveTimer;
        internal float _spawnTimer;
        internal float _currentSpawnDelay;

        //===============================================================================================
        // CONSTRUCTOR
        //===============================================================================================

        private WaveDirectorCore()
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
        // INTERNAL SHARED HELPERS (Pure State Mapping)
        //===============================================================================================

        internal void ResetTimers()
        {
            _waveTimer = 0f;
            _interWaveTimer = 0f;
            _spawnTimer = 0f;
            _currentSpawnDelay = 0f;
        }

        internal void ResetFlags()
        {
            _isPaused = false;
            _isGameComplete = false;
        }

        /// <summary>
        /// Syncs the internal concrete core state out to a decoupled pipeline context object.
        /// </summary>
        internal void SyncToContext(WaveDirectorContext context)
        {
            if (context == null) return;

            context.IsInitialized = _isInitialized;
            context.IsPaused = _isPaused;
            context.IsGameComplete = _isGameComplete;
            context.CurrentState = _currentState;
            context.CurrentWaveNumber = _currentWaveNumber;
            context.TotalWaves = _totalWaves;
            context.WaveTimer = _waveTimer;
            context.SpawnTimer = _spawnTimer;
            context.InterWaveTimer = _interWaveTimer;
            context.CurrentWave = _currentWave;
        }
    }
}
