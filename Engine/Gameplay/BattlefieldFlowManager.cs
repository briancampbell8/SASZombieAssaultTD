// ====================================================================================================
//  FILE: BattlefieldFlowManager.cs
//  PATH: Engine/Gameplay/
//  MODULE: Gameplay (Battlefield Flow)
//
//  ROLE:
//      Coordinates battlefield selection, loading, progress tracking, and unlock progression.
//
//  RESPONSIBILITIES:
//      - Provide APIs for selecting and loading battlefields with progress callbacks.
//      - Track unlocked and completed battlefields and report completion state.
//      - Integrate with SceneManager for level transitions.
//
//  NON-RESPONSIBILITIES:
//      - Rendering or UI presentation of selection screens (UI systems handle visuals).
//
//  ARCHITECTURAL NOTES:
//      - Keep loading operations asynchronous-friendly and report deterministic progress values.
// ====================================================================================================

using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Diagnostics;

//
using SASZombieAssaultTD.Engine.Scenes;
namespace SASZombieAssaultTD.Engine.Gameplay
{
    ///<summary>
    ///Manages battlefield selection, loading, and completion tracking.
    ///P120-11: Provides centralized battlefield flow management with unlock system and progress tracking.
    ///</summary>
    public class BattlefieldFlowManager
    {
        private SceneManager? _sceneManager;
        private BattlefieldType? _selectedBattlefield;
        private Dictionary<BattlefieldType, bool> _unlockedBattlefields;
        private Dictionary<BattlefieldType, bool> _completedBattlefields;
        private bool _isLoading;
        private float _loadingProgress;
        private Action<float>? _loadingProgressCallback;

        ///<summary>
        ///Gets the currently selected battlefield.
        ///</summary>
        public BattlefieldType? SelectedBattlefield => _selectedBattlefield;

        ///<summary>
        ///Gets whether a battlefield is currently loading.
        ///</summary>
        public bool IsLoading => _isLoading;

        ///<summary>
        ///Gets the current loading progress (0.0 to 1.0).
        ///</summary>
        public float LoadingProgress => _loadingProgress;

        ///<summary>
        ///Event fired when battlefield selection changes.
        ///</summary>
        public event Action<BattlefieldType?>? OnBattlefieldSelected;

        ///<summary>
        ///Event fired when battlefield loading starts.
        ///</summary>
        public event Action<BattlefieldType>? OnBattlefieldLoadingStarted;

        ///<summary>
        ///Event fired when battlefield loading completes.
        ///</summary>
        public event Action<BattlefieldType>? OnBattlefieldLoadingCompleted;

        ///<summary>
        ///Event fired when a battlefield is unlocked.
        ///</summary>
        public event Action<BattlefieldType>? OnBattlefieldUnlocked;

        ///<summary>
        ///Event fired when a battlefield is completed.
        ///</summary>
        public event Action<BattlefieldType>? OnBattlefieldCompleted;

        ///<summary>
        ///Initializes a new battlefield flow manager.
        ///</summary>
        public BattlefieldFlowManager()
        {
            _unlockedBattlefields = new Dictionary<BattlefieldType, bool>();
            _completedBattlefields = new Dictionary<BattlefieldType, bool>();
            _isLoading = false;
            _loadingProgress = 0f;

            //Unlock Mean Street by default (first battlefield)
            _unlockedBattlefields[BattlefieldType.MeanStreet] = true;

            DLogger.Log(LogSubsystems.Gameplay, LogLevel.Info, "INFO", "BattlefieldFlowManager: Initialized");
        }

        ///<summary>
        ///Sets the scene manager for scene operations.
        ///</summary>
        ///<param name="sceneManager">The scene manager instance.</param>
        public void SetSceneManager(SceneManager sceneManager)
        {
            _sceneManager = sceneManager;
            DLogger.Log(LogSubsystems.Gameplay, LogLevel.Info, "INFO", "BattlefieldFlowManager: SceneManager set");
        }

        ///<summary>
        ///Selects a battlefield for loading.
        ///</summary>
        ///<param name="battlefield">The battlefield to select.</param>
        ///<returns>True if the battlefield was selected successfully.</returns>
        public bool SelectBattlefield(BattlefieldType battlefield)
        {
            if (!IsBattlefieldUnlocked(battlefield))
            {
                DLogger.Log(LogSubsystems.Gameplay,
                    LogLevel.Info,
                    "Warning",
                    $"BattlefieldFlowManager: Cannot select {battlefield.GetDisplayName()} - not unlocked");
                return false;
            }

            _selectedBattlefield = battlefield;
            DLogger.Log(LogSubsystems.Gameplay, LogLevel.Info, "INFO", $"BattlefieldFlowManager: Selected {battlefield.GetDisplayName()}");
            OnBattlefieldSelected?.Invoke(battlefield);

            return true;
        }

        ///<summary>
        ///Loads the selected battlefield.
        ///</summary>
        ///<param name="progressCallback">Optional callback for loading progress.</param>
        ///<returns>True if loading was initiated successfully.</returns>
        public bool LoadBattlefield(Action<float>? progressCallback = null)
        {
            if (_selectedBattlefield == null)
            {
                DLogger.Log(LogSubsystems.Gameplay, LogLevel.Info, "ERROR", "BattlefieldFlowManager: Cannot load - no battlefield selected");
                return false;
            }

            if (_isLoading)
            {
                DLogger.Log(LogSubsystems.Gameplay, LogLevel.Info, "Warning", "BattlefieldFlowManager: Cannot load - already loading");
                return false;
            }

            if (_sceneManager == null)
            {
                DLogger.Log(LogSubsystems.Gameplay, LogLevel.Info, "ERROR", "BattlefieldFlowManager: Cannot load - SceneManager not set");
                return false;
            }

            _loadingProgressCallback = progressCallback;
            _isLoading = true;
            _loadingProgress = 0f;

            DLogger.Log(LogSubsystems.Gameplay, LogLevel.Info, "INFO", $"BattlefieldFlowManager: Starting load for {_selectedBattlefield.Value.GetDisplayName()}");
            OnBattlefieldLoadingStarted?.Invoke(_selectedBattlefield.Value);

            //Simulate loading progress (in real implementation, this would load assets)
            SimulateLoading();

            return true;
        }

        ///<summary>
        ///Simulates loading progress.
        ///In a real implementation, this would load actual assets.
        ///</summary>
        private void SimulateLoading()
        {
            //TODO: Implement actual asset loading
            //For now, simulate immediate completion
            CompleteLoading();
        }

        ///<summary>
        ///Completes the battlefield loading.
        ///</summary>
        private void CompleteLoading()
        {
            if (_selectedBattlefield == null || _sceneManager == null)
            {
                _isLoading = false;
                return;
            }

            _loadingProgress = 1.0f;
            _loadingProgressCallback?.Invoke(_loadingProgress);

            //Switch to the battlefield scene
            var sceneName = _selectedBattlefield.Value.ToString();
            var success = _sceneManager.SwitchToScene(sceneName);

            if (success)
            {
                DLogger.Log(LogSubsystems.Gameplay, LogLevel.Info, "INFO", $"BattlefieldFlowManager: Successfully loaded {_selectedBattlefield.Value.GetDisplayName()}");
                OnBattlefieldLoadingCompleted?.Invoke(_selectedBattlefield.Value);
            }
            else
            {
                DLogger.Log(LogSubsystems.Gameplay, LogLevel.Info,
                                       "ERROR", $"BattlefieldFlowManager: Failed to load {_selectedBattlefield.Value.GetDisplayName()}");
            }

            _isLoading = false;
        }

        ///<summary>
        ///Checks if a battlefield is unlocked.
        ///</summary>
        ///<param name="battlefield">The battlefield to check.</param>
        ///<returns>True if the battlefield is unlocked.</returns>
        public bool IsBattlefieldUnlocked(BattlefieldType battlefield)
        {
            return _unlockedBattlefields.TryGetValue(battlefield, out var unlocked) && unlocked;
        }

        ///<summary>
        ///Unlocks a battlefield.
        ///</summary>
        ///<param name="battlefield">The battlefield to unlock.</param>
        public void UnlockBattlefield(BattlefieldType battlefield)
        {
            if (IsBattlefieldUnlocked(battlefield))
            {
                return;
            }

            _unlockedBattlefields[battlefield] = true;
            DLogger.Log(LogSubsystems.Gameplay, LogLevel.Info, "INFO", $"BattlefieldFlowManager: Unlocked {battlefield.GetDisplayName()}");
            OnBattlefieldUnlocked?.Invoke(battlefield);
        }

        ///<summary>
        ///Checks if a battlefield has been completed.
        ///</summary>
        ///<param name="battlefield">The battlefield to check.</param>
        ///<returns>True if the battlefield has been completed.</returns>
        public bool IsBattlefieldCompleted(BattlefieldType battlefield)
        {
            return _completedBattlefields.TryGetValue(battlefield, out var completed) && completed;
        }

        ///<summary>
        ///Marks a battlefield as completed.
        ///</summary>
        ///<param name="battlefield">The battlefield to mark as completed.</param>
        public void CompleteBattlefield(BattlefieldType battlefield)
        {
            if (IsBattlefieldCompleted(battlefield))
            {
                return;
            }

            _completedBattlefields[battlefield] = true;
            DLogger.Log(LogSubsystems.Gameplay, LogLevel.Info, "INFO", $"BattlefieldFlowManager: Completed {battlefield.GetDisplayName()}");

            //Unlock next battlefield if not the last one
            var nextBattlefield = GetNextBattlefield(battlefield);
            if (nextBattlefield.HasValue && !IsBattlefieldUnlocked(nextBattlefield.Value))
            {
                UnlockBattlefield(nextBattlefield.Value);
            }

            OnBattlefieldCompleted?.Invoke(battlefield);
        }

        ///<summary>
        ///Gets the next battlefield in the progression.
        ///</summary>
        ///<param name="currentBattlefield">The current battlefield.</param>
        ///<returns>The next battlefield, or null if current is the last one.</returns>
        private BattlefieldType? GetNextBattlefield(BattlefieldType currentBattlefield)
        {
            return currentBattlefield switch
            {
                BattlefieldType.MeanStreet => BattlefieldType.SubZero,
                BattlefieldType.SubZero => BattlefieldType.DeadWarehouse,
                BattlefieldType.DeadWarehouse => BattlefieldType.ShopTilYouDrop,
                BattlefieldType.ShopTilYouDrop => BattlefieldType.Killtop,
                BattlefieldType.Killtop => BattlefieldType.Touchdown,
                BattlefieldType.Touchdown => BattlefieldType.Cleanup,
                BattlefieldType.Cleanup => BattlefieldType.OutbreakMansion,
                BattlefieldType.OutbreakMansion => null, //Last battlefield
                _ => null
            };
        }

        ///<summary>
        ///Gets all unlocked battlefields.
        ///</summary>
        ///<returns>List of unlocked battlefield types.</returns>
        public List<BattlefieldType> GetUnlockedBattlefields()
        {
            var unlocked = new List<BattlefieldType>();
            foreach (var kvp in _unlockedBattlefields)
            {
                if (kvp.Value)
                {
                    unlocked.Add(kvp.Key);
                }
            }
            return unlocked;
        }

        ///<summary>
        ///Gets all completed battlefields.
        ///</summary>
        ///<returns>List of completed battlefield types.</returns>
        public List<BattlefieldType> GetCompletedBattlefields()
        {
            var completed = new List<BattlefieldType>();
            foreach (var kvp in _completedBattlefields)
            {
                if (kvp.Value)
                {
                    completed.Add(kvp.Key);
                }
            }
            return completed;
        }

        ///<summary>
        ///Gets the total number of battlefields.
        ///</summary>
        ///<returns>Total battlefield count.</returns>
        public int GetTotalBattlefieldCount()
        {
            return Enum.GetValues(typeof(BattlefieldType)).Length;
        }

        ///<summary>
        ///Gets the number of completed battlefields.
        ///</summary>
        ///<returns>Completed battlefield count.</returns>
        public int GetCompletedBattlefieldCount()
        {
            return _completedBattlefields.Count;
        }

        ///<summary>
        ///Gets the overall completion progress (0.0 to 1.0).
        ///</summary>
        ///<returns>Completion progress.</returns>
        public float GetOverallProgress()
        {
            var total = GetTotalBattlefieldCount();
            var completed = GetCompletedBattlefieldCount();
            return total > 0 ? (float)completed / total : 0f;
        }

        ///<summary>
        ///Resets all battlefield progress.
        ///</summary>
        public void ResetProgress()
        {
            _unlockedBattlefields.Clear();
            _completedBattlefields.Clear();

            //Unlock Mean Street by default
            _unlockedBattlefields[BattlefieldType.MeanStreet] = true;

            DLogger.Log(LogSubsystems.Gameplay, LogLevel.Info, "INFO", "BattlefieldFlowManager: Reset all progress");
        }

        ///<summary>
        ///Gets battlefield flow manager information as a string.
        ///</summary>
        public override string ToString()
        {
            return $"BattlefieldFlowManager: Selected={_selectedBattlefield?.GetDisplayName() ?? "None"}, " +
                   $"Loading={_isLoading}, Progress={_loadingProgress:P2}, " +
                   $"Unlocked={_unlockedBattlefields.Count}/{GetTotalBattlefieldCount()}, " +
                   $"Completed={_completedBattlefields.Count}/{GetTotalBattlefieldCount()}";
        }
    }
}
