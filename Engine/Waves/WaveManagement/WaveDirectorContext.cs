// =====================================================================================================
//  FILE: WaveDirectorContext.cs
//  PATH: Engine/Waves/WaveManagement/WaveDirectorContext.cs
//  SUBSYSTEM: Waves - Wave Management
// =====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Threading.Tasks;

namespace SASZombieAssaultTD.Engine.Waves.WaveManagement
{
    public class WaveDirectorContext
    {
        // ---------------------------------------------------------------------------------------------
        // RUNTIME STATE FLAGS & TIMERS
        // ---------------------------------------------------------------------------------------------

        public bool IsInitialized { get; set; }
        public bool IsPaused { get; set; }
        public bool IsGameComplete { get; set; }
        public WaveState CurrentState { get; set; } = WaveState.NotStarted;
        public int CurrentWaveNumber { get; set; }
        public int TotalWaves { get; set; }
        public float WaveTimer { get; set; }
        public float SpawnTimer { get; set; }
        public float InterWaveTimer { get; set; }

        // ---------------------------------------------------------------------------------------------
        // DATA COLLECTIONS & CONTAINERS
        // ---------------------------------------------------------------------------------------------

        public object CurrentWave { get; set; }
        public Dictionary<int, object> WaveScripts { get; set; } = new Dictionary<int, object>();
        public List<object> UpcomingWaves { get; set; } = new List<object>();

        // ---------------------------------------------------------------------------------------------
        // FAÇADE ACTION ROUTING ENDPOINTS & CALLBACKS
        // ---------------------------------------------------------------------------------------------

        public Action OnInitializeInternal { get; set; }

        // FIX: Removed "= Invoke" initialization to avoid the float conversion mismatch.
        // It can now be safely invoked via OnInvokeEnemySpawned?.Invoke();
        public Action OnInvokeEnemySpawned { get; set; }

        public Action<int> OnInvokeWaveStarted { get; set; }
        public Action<int> OnInvokeWaveCompleted { get; set; }
        public Action OnInvokeWaveProgressUpdated { get; set; }
        public Action OnInvokeAllWavesCompleted { get; set; }
        public Action OnInvokeGameComplete { get; set; }
        public Func<object, Task> OnStartWaveSpawningInternal { get; set; }
        public Action OnResetFlags { get; set; }
        public Action OnResetTimers { get; set; }
        public Action OnInitializeWaveQueueInternal { get; set; }
        public Action OnUpdateWaitingToStartInternal { get; set; }
        public Action OnUpdateWaveStartingInternal { get; set; }
        public Action OnUpdateCompleteGameInternal { get; set; }

        // ---------------------------------------------------------------------------------------------
        // LIGHTWEIGHT HELPER PROPERTIES
        // ---------------------------------------------------------------------------------------------

        public float WaveProgress => TotalWaves > 0 ? (float)CurrentWaveNumber / TotalWaves : 0f;

        public float GetInterWaveDelay()
        {
            return 10.0f; // Safe deterministic fallback delay constant
        }
    }
}
