/*
File:    AnimationStateInspector.cs
Path:    Engine/Animation/AnimationStateInspector.cs
Purpose: P11-16-05 - Animation state inspection and analysis tools.
Provides detailed animation state information and debugging capabilities.
*/

using SASZombieAssaultTD.Engine.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SASZombieAssaultTD.Engine.Animation.Components
{
    /// <summary>
    /// Animation state inspection and analysis tools.
    /// Implements P11-16-05: Animation state inspection and debugging capabilities.
    /// </summary>
    public class AnimationStateInspector
    {
        private readonly AnimationControllerComponent _animationController;
        private readonly object _inspectorLock = new();

        private readonly Dictionary<int, AnimationStateSnapshot> _stateSnapshots = new();
        private readonly Dictionary<int, List<AnimationTransitionRecord>> _transitionHistories = new();

        private bool _isEnabled;
        private DateTime _startTime;

        public bool IsEnabled => _isEnabled;
        public int InspectedEntityCount => _stateSnapshots.Count;

        public AnimationStateInspector(AnimationControllerComponent animationController)
        {
            _animationController = animationController ?? throw new ArgumentNullException(nameof(animationController));
        }

        public void Initialize()
        {
            lock (_inspectorLock)
            {
                Engine.Diagnostics.DebugLogger.LogDebug("INFO", "Initializing AnimationStateInspector...");
                _stateSnapshots.Clear();
                _transitionHistories.Clear();
                _isEnabled = true;
                _startTime = DateTime.Now;
                Engine.Diagnostics.DebugLogger.LogDebug("INFO", "AnimationStateInspector initialized successfully");
            }
        }

        public void Shutdown()
        {
            lock (_inspectorLock)
            {
                Engine.Diagnostics.DebugLogger.LogDebug("INFO", "Shutting down AnimationStateInspector...");
                GenerateFinalReport();
                _stateSnapshots.Clear();
                _transitionHistories.Clear();
                _isEnabled = false;
                Engine.Diagnostics.DebugLogger.LogDebug("INFO", "AnimationStateInspector shutdown completed");
            }
        }

        public AnimationStateInfo? GetStateInfo(int entityId)
        {
            lock (_inspectorLock)
            {
                if (!_isEnabled) return null;

                try
                {
                    var currentState = _animationController.GetCurrentState((uint)entityId);
                    var previousState = _animationController.GetPreviousState((uint)entityId);
                    var transitionProgress = _animationController.GetTransitionProgress((uint)entityId);
                    var animationTime = _animationController.GetAnimationTime((uint)entityId);
                    var parameters = _animationController.GetAnimationParameters((uint)entityId);
                    var blendWeights = _animationController.GetBlendWeights((uint)entityId);

                    var stateInfo = new AnimationStateInfo
                    {
                        EntityId = entityId,
                        CurrentState = currentState?.Name ?? "None",
                        PreviousState = previousState?.Name ?? "None",
                        TransitionProgress = transitionProgress,
                        AnimationTime = animationTime,
                        Parameters = new Dictionary<string, float>(parameters),
                        BlendWeights = new Dictionary<string, float>(blendWeights),
                        IsTransitioning = transitionProgress > 0f && transitionProgress < 1f,
                        Timestamp = DateTime.Now
                    };

                    _stateSnapshots[entityId] = new AnimationStateSnapshot
                    {
                        EntityId = entityId,
                        StateInfo = stateInfo,
                        Timestamp = DateTime.Now
                    };

                    return stateInfo;
                }
                catch (Exception ex)
                {
                    Engine.Diagnostics.DebugLogger.LogDebug("ERROR", $"Failed to get state info for entity {entityId}: {ex.Message}");
                    return null;
                }
            }
        }

        public List<AnimationTransitionRecord> GetTransitionHistory(int entityId, int maxTransitions = 50)
        {
            lock (_inspectorLock)
            {
                if (!_isEnabled) return new List<AnimationTransitionRecord>();
                return _transitionHistories.TryGetValue(entityId, out var history)
                    ? history.TakeLast(maxTransitions).ToList()
                    : new List<AnimationTransitionRecord>();
            }
        }

        public List<int> GetInspectedEntities()
        {
            lock (_inspectorLock)
            {
                return _stateSnapshots.Keys.ToList();
            }
        }

        public Dictionary<int, AnimationStateSnapshot> GetAllStateSnapshots()
        {
            lock (_inspectorLock)
            {
                return new Dictionary<int, AnimationStateSnapshot>(_stateSnapshots);
            }
        }

        public void RecordStateTransition(int entityId, string fromState, string toState, float duration)
        {
            lock (_inspectorLock)
            {
                if (!_isEnabled) return;

                if (!_transitionHistories.TryGetValue(entityId, out var history))
                {
                    history = new List<AnimationTransitionRecord>();
                    _transitionHistories[entityId] = history;
                }

                history.Add(new AnimationTransitionRecord
                {
                    EntityId = entityId,
                    FromState = fromState,
                    ToState = toState,
                    Duration = duration,
                    Timestamp = DateTime.Now
                });

                if (history.Count > 100) history.RemoveAt(0);
            }
        }

        public AnimationParameterInfo? GetParameterInfo(int entityId)
        {
            lock (_inspectorLock)
            {
                if (!_isEnabled) return null;

                try
                {
                    var parameters = _animationController.GetAnimationParameters((uint)entityId);
                    return new AnimationParameterInfo
                    {
                        EntityId = entityId,
                        Parameters = new Dictionary<string, float>(parameters),
                        ParameterCount = parameters.Count,
                        ActiveParameters = parameters.Where(p => p.Value > 0f).ToList(),
                        MaxParameterValue = parameters.Values.DefaultIfEmpty(0f).Max(),
                        MinParameterValue = parameters.Values.DefaultIfEmpty(0f).Min(),
                        Timestamp = DateTime.Now
                    };
                }
                catch (Exception ex)
                {
                    Engine.Diagnostics.DebugLogger.LogDebug("ERROR", $"Failed to get parameter info for entity {entityId}: {ex.Message}");
                    return null;
                }
            }
        }

        public AnimationBlendWeightInfo? GetBlendWeightInfo(int entityId)
        {
            lock (_inspectorLock)
            {
                if (!_isEnabled) return null;

                try
                {
                    var blendWeights = _animationController.GetBlendWeights((uint)entityId);
                    return new AnimationBlendWeightInfo
                    {
                        EntityId = entityId,
                        BlendWeights = new Dictionary<string, float>(blendWeights),
                        BlendCount = blendWeights.Count,
                        ActiveBlends = blendWeights.Where(b => b.Value > 0f).ToList(),
                        MaxBlendWeight = blendWeights.Values.DefaultIfEmpty(0f).Max(),
                        MinBlendWeight = blendWeights.Values.DefaultIfEmpty(0f).Min(),
                        TotalBlendWeight = blendWeights.Values.Sum(),
                        Timestamp = DateTime.Now
                    };
                }
                catch (Exception ex)
                {
                    Engine.Diagnostics.DebugLogger.LogDebug("ERROR", $"Failed to get blend weight info for entity {entityId}: {ex.Message}");
                    return null;
                }
            }
        }

        public void SetEnabled(bool enabled)
        {
            lock (_inspectorLock)
            {
                _isEnabled = enabled;
                Engine.Diagnostics.DebugLogger.LogInfo($"Animation state inspection {(enabled ? "enabled" : "disabled")}");
            }
        }

        public void Clear()
        {
            lock (_inspectorLock)
            {
                _stateSnapshots.Clear();
                _transitionHistories.Clear();
                _startTime = DateTime.Now;
                Engine.Diagnostics.DebugLogger.LogInfo("Animation state inspection data cleared");
            }
        }

        public AnimationStateInspectorStats GetStats()
        {
            lock (_inspectorLock)
            {
                return new AnimationStateInspectorStats
                {
                    IsEnabled = _isEnabled,
                    StartTime = _startTime,
                    InspectedEntityCount = _stateSnapshots.Count,
                    TotalTransitionRecords = _transitionHistories.Values.Sum(h => h.Count),
                    AverageTransitionsPerEntity = _transitionHistories.Count > 0
                        ? (float)_transitionHistories.Values.Sum(h => h.Count) / _transitionHistories.Count
                        : 0f,
                    TotalRunTime = (float)(DateTime.Now - _startTime).TotalSeconds
                };
            }
        }

        private void GenerateFinalReport()
        {
            try
            {
                var stats = GetStats();
                Engine.Diagnostics.DebugLogger.LogDebug("INFO", "Animation state inspector final report:");
                Engine.Diagnostics.DebugLogger.LogDebug("INFO", $"  Total run time: {stats.TotalRunTime:F2}s");
                Engine.Diagnostics.DebugLogger.LogDebug("INFO", $"  Inspected entities: {stats.InspectedEntityCount}");
                Engine.Diagnostics.DebugLogger.LogDebug("INFO", $"  Total transition records: {stats.TotalTransitionRecords}");
                Engine.Diagnostics.DebugLogger.LogDebug("INFO", $"  Average transitions per entity: {stats.AverageTransitionsPerEntity:F2}");
            }
            catch (Exception ex)
            {
                Engine.Diagnostics.DebugLogger.LogDebug("ERROR", $"Failed to generate final report: {ex.Message}");
            }
        }
    }
}

