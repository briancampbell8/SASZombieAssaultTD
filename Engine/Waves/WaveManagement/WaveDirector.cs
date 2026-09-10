// ====================================================================================================
// FILE: WaveDirector.cs
// PATH: Engine/Waves/WaveManagement/WaveDirector.cs
// SUBSYSTEM: Waves
//
// ROLE:
//     Public façade for the WaveDirector subsystem.
//     Provides the unified external API for wave control, state access, and event dispatch.
//
// RESPONSIBILITIES:
//     - Expose public API for wave lifecycle control (initialize, start, pause, resume, stop, reset).
//     - Expose read-only properties for wave state, progress, and wave counts.
//     - Declare all wave-related events consumed by external systems.
//     - Provide safe internal event-invoker methods.
//     - Coordinate state tracking via independent standalone context and workflow programs.
//
// NON-RESPONSIBILITIES:
//     - Storing local instance properties or mutating values implicitly without parameters.
//     - Low-level pathfinding, difficulty parameters, or file serialization.
// ====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Waves.WaveManagement
{
    /// <summary>
    /// Public façade for the WaveDirector subsystem. Provides wave control, state access, and event dispatch.
    /// </summary>
    public class WaveDirector : IWaveDirector
    {
        private static WaveDirector _instance;
        public static WaveDirector Instance => _instance ??= new WaveDirector();

        // Standalone state data context and stateless pipeline calculation programs
        private readonly WaveDirectorContext _context;
        private readonly WaveDirectorWorkflow _workflow;

        //===============================================================================================
        // EVENTS
        //===============================================================================================

        public event Action<int> OnWaveStarted;

        public event Action<int> OnWaveCompleted;

        public event Action<Enemy> OnEnemySpawned;

        public event Action OnAllWavesCompleted;

        public event Action OnGameComplete;

        public event Action<float> OnWaveProgressUpdated;

        //===============================================================================================
        // CONSTRUCTOR & EVENT PIPELINE BINDING
        //===============================================================================================

        public WaveDirector()
        {
            _context = new WaveDirectorContext();
            _workflow = new WaveDirectorWorkflow();

            // Wire back internal context invoke callbacks directly to facade events
            _context.OnInitializeInternal = Initialize_Internal;
            _context.OnInvokeWaveStarted = InvokeWaveStarted;
            _context.OnInvokeWaveCompleted = InvokeWaveCompleted;
            _context.OnInvokeAllWavesCompleted = InvokeAllWavesCompleted;
            _context.OnInvokeGameComplete = InvokeGameComplete;
            _context.OnInvokeWaveProgressUpdated = () => InvokeWaveProgressUpdated(WaveProgress);

            // Wire fallback lifecycle endpoints safely
            _context.OnResetFlags = () => { };
            _context.OnResetTimers = () => { _context.WaveTimer = 0f; _context.SpawnTimer = 0f; _context.InterWaveTimer = 0f; };
            _context.OnInitializeWaveQueueInternal = () => { };
            _context.OnUpdateWaitingToStartInternal = () => { };
            _context.OnUpdateWaveStartingInternal = () => { };
            _context.OnUpdateCompleteGameInternal = () => { };
            _context.OnStartWaveSpawningInternal = async (wave) => await System.Threading.Tasks.Task.CompletedTask;
        }

        private void Initialize_Internal()
        {
            _context.IsInitialized = true;
            _context.TotalWaves = 10; // Explicit configuration fallback threshold constant
        }

        //===============================================================================================
        // INTERNAL EVENT INVOKERS
        //===============================================================================================

        internal void InvokeWaveStarted(int waveNumber)
            => OnWaveStarted?.Invoke(waveNumber);

        internal void InvokeWaveCompleted(int waveNumber)
            => OnWaveCompleted?.Invoke(waveNumber);

        internal void InvokeEnemySpawned(Enemy enemy)
            => OnEnemySpawned?.Invoke(enemy);

        internal void InvokeAllWavesCompleted()
            => OnAllWavesCompleted?.Invoke();

        internal void InvokeGameComplete()
            => OnGameComplete?.Invoke();

        internal void InvokeWaveProgressUpdated(float progress)
            => OnWaveProgressUpdated?.Invoke(progress);

        //===============================================================================================
        // PUBLIC PROPERTIES (EXPLICIT DATA OVERLAYS)
        //===============================================================================================

        public int CurrentWave => _context.CurrentWaveNumber;
        public int TotalWaves => _context.TotalWaves;
        public WaveState CurrentState => _context.CurrentState;

        public bool IsWaveActive => _context.CurrentState == WaveState.InProgress;
        public bool IsBetweenWaves => _context.CurrentState == WaveState.InterWave;
        public bool IsGameComplete => _context.IsGameComplete;
        public bool IsPaused => _context.IsPaused;

        public float WaveProgress => GetWaveProgress();
        public float OverallProgress => GetOverallProgress();

        private float GetWaveProgress()
        {
            return _context.WaveProgress;
        }

        private float GetOverallProgress()
        {
            return _context.TotalWaves > 0 ? (float)_context.CurrentWaveNumber / _context.TotalWaves : 0f;
        }

        //===============================================================================================
        // PUBLIC API — PIPELINE DELEGATION
        //===============================================================================================

        public void Initialize() => _context.OnInitializeInternal?.Invoke();

        public void StartGame() => _workflow.StartGame_Internal(_context);

        public System.Threading.Tasks.Task<bool> StartWave(int waveNumber)
            => _workflow.StartWave_Internal(_context, waveNumber);

        public void StartWaveSync(int waveNumber)
            => _workflow.StartWave_Internal(_context, waveNumber).GetAwaiter().GetResult();

        public void StartNextWave() => _workflow.StartNextWave_Internal(_context);

        public void StartNextWaveEarly() => _workflow.StartNextWaveEarly_Internal(_context);

        public void Update(float deltaTime) => _workflow.Update_Internal(_context, deltaTime);

        public void Pause() => _workflow.Pause_Internal(_context);

        public void Resume() => _workflow.Resume_Internal(_context);

        public void Stop() => _workflow.Stop_Internal(_context);

        public void Reset() => _workflow.Reset_Internal(_context);
    }

    public interface IWaveDirector
    {
        public event Action<int> OnWaveStarted;
        public event Action<int> OnWaveCompleted;
        public event Action<Enemy> OnEnemySpawned;
        public event Action OnAllWavesCompleted;
        public event Action OnGameComplete;
        public event Action<float> OnWaveProgressUpdated;

        public int CurrentWave { get; }
        public int TotalWaves { get; }
        public WaveState CurrentState { get; }

        public bool IsWaveActive { get; }
        public bool IsBetweenWaves { get; }
        public bool IsGameComplete { get; }
        public bool IsPaused { get; }
    }
}
