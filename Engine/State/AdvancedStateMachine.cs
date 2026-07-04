using System;
using System.Collections.Generic;
using System.Linq;
using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.State

{
    ///<summary>
    ///Advanced state machine with history, validation, and enhanced features.
    ///P20-02-Enhancement: Extended StateMachine with additional capabilities.
    ///</summary>
    public class AdvancedStateMachine : StateMachine
    {
        private readonly List<StateTransition> _transitionHistory;
        private readonly int _maxHistorySize;
        private readonly Dictionary<GameStateType, DateTime> _stateEnterTimes;
        private StateFactory _stateFactory;

        ///<summary>
        ///Gets the transition history.
        ///</summary>
        public IReadOnlyList<StateTransition> TransitionHistory => _transitionHistory.AsReadOnly();

        ///<summary>
        ///Gets the time the current state was entered.
        ///</summary>
        public DateTime CurrentStateEnterTime =>
        _stateEnterTimes.TryGetValue(CurrentStateType, out var time) ? time : DateTime.MinValue;

        ///<summary>
        ///Gets the duration the current state has been active.
        ///</summary>
        public TimeSpan CurrentStateDuration => DateTime.UtcNow - CurrentStateEnterTime;

        ///<summary>
        ///Event fired when a state transition is about to occur.
        ///</summary>
        public event Action<StateTransition> OnTransitionStarted;

        ///<summary>
        ///Event fired when a state transition has completed.
        ///</summary>
        public event Action<StateTransition> OnTransitionCompleted;

        ///<summary>
        ///Event fired when a state transition fails.
        ///</summary>
        public event Action<StateTransition, Exception> OnTransitionFailed;

        ///<summary>
        ///Initializes a new advanced state machine.
        ///</summary>
        ///<param name="maxHistorySize">Maximum number of transitions to keep in history.</param>
        public AdvancedStateMachine(int maxHistorySize = 100) : base()
        {
            _maxHistorySize = maxHistorySize;
            _transitionHistory = new List<StateTransition>();
            _stateEnterTimes = new Dictionary<GameStateType, DateTime>();
        }

        ///<summary>
        ///Sets the state factory for advanced state creation.
        ///</summary>
        ///<param name="stateFactory">The state factory to use.</param>
        public void SetStateFactory(StateFactory stateFactory, object ex)
        {
            _stateFactory = stateFactory ?? throw new ArgumentNullException(nameof(stateFactory));
            DLogger.Log(
                LogSubsystems.State,
                LogLevel.Info,
                $"AdvancedStateMachine: StateFactory set to {stateFactory.GetType().Name}");



        }

        ///<summary>
        ///Changes state with enhanced tracking and validation.
        ///</summary>
        ///<param name="type">The target state type.</param>
        ///<param name="triggerEvent">The event that triggered the transition.</param>
        public override void ChangeState(GameStateType type, GameEvent? triggerEvent = null)
        {
            var transition = new StateTransition(CurrentStateType, type, triggerEvent);

            try
            {
                //Validate transition
                if (!StateTransitionRules.IsTransitionAllowed(CurrentStateType, type))
                {
                    var message = StateTransitionRules.GetValidationMessage(CurrentStateType, type);
                    throw new InvalidOperationException(message);
                }

                //Fire transition started event
                OnTransitionStarted?.Invoke(transition);

                //Record state enter time
                _stateEnterTimes[type] = DateTime.UtcNow;

                //Perform the actual transition
                base.ChangeState(type);

                //Complete the transition
                transition.Complete();
                AddToHistory(transition);

                //Fire transition completed event
                OnTransitionCompleted?.Invoke(transition);

                DLogger.Log(
                    LogSubsystems.State,
                    LogLevel.Info,
                     $"AdvancedStateMachine: Transition completed - {transition}");
            }
            catch (Exception ex)
            {
                DLogger.Log(
                LogSubsystems.Unknown, LogLevel.Info,
                    $"AdvancedStateMachine: Transition failed - {ex.Message}");
                OnTransitionFailed?.Invoke(transition, ex);
                throw;
            }
        }

        ///<summary>
        ///Gets statistics about state machine usage.
        ///</summary>
        ///<returns>Advanced state machine statistics.</returns>
        public AdvancedStateMachineStatistics GetAdvancedStatistics()
        {
            var baseStats = GetStatistics();
            var stateDurations = GetStateDurations();
            var mostFrequentTransitions = GetMostFrequentTransitions();

            return new AdvancedStateMachineStatistics
            {
                CurrentState = baseStats.CurrentState,
                RegisteredStates = baseStats.RegisteredStates,
                ValidTransitions = baseStats.ValidTransitions,
                HasCurrentState = baseStats.HasCurrentState,
                CurrentStateDuration = CurrentStateDuration,
                TotalTransitions = _transitionHistory.Count,
                AverageStateDuration = stateDurations.Values.DefaultIfEmpty(TimeSpan.Zero).Average(ts => ts.TotalMilliseconds),
                MostFrequentTransition = mostFrequentTransitions.FirstOrDefault(),
                StateEnterTimes = new Dictionary<GameStateType, DateTime>(_stateEnterTimes)
            };
        }

        ///<summary>
        ///Gets the duration each state has been active.
        ///</summary>
        ///<returns>Dictionary of state types and their total durations.</returns>
        public Dictionary<GameStateType, TimeSpan> GetStateDurations()
        {
            var durations = new Dictionary<GameStateType, TimeSpan>();

            //Calculate durations from transition history
            for (int i = 0; i < _transitionHistory.Count; i++)
            {
                var transition = _transitionHistory[i];

                if (!durations.ContainsKey(transition.FromState))
                    durations[transition.FromState] = TimeSpan.Zero;

                durations[transition.FromState] = durations[transition.FromState].Add(TimeSpan.FromMilliseconds(transition.DurationMs));
            }

            //Add current state duration
            if (CurrentState != null)
            {
                if (!durations.ContainsKey(CurrentStateType))
                    durations[CurrentStateType] = TimeSpan.Zero;

                durations[CurrentStateType] = durations[CurrentStateType].Add(CurrentStateDuration);
            }

            return durations;
        }

        ///<summary>
        ///Gets the most frequent state transitions.
        ///</summary>
        ///<param name="topCount">Number of top transitions to return.</param>
        ///<returns>List of most frequent transitions with their counts.</returns>
        public List<(StateTransition Transition, int Count)> GetMostFrequentTransitions(int topCount = 5)
        {
            var transitionCounts = new Dictionary<string, int>();

            foreach (var transition in _transitionHistory)
            {
                var key = $"{transition.FromState}→{transition.ToState}";
                if (!transitionCounts.TryAdd(key, 1))
                    transitionCounts[key]++;
            }

            return transitionCounts
            .OrderByDescending(kvp => kvp.Value)
            .Take(topCount)
            .Select(kvp =>
            {
                var parts = kvp.Key.Split('→');
                var fromState = Enum.Parse<GameStateType>(parts[0]);
                var toState = Enum.Parse<GameStateType>(parts[1]);
                var transition = new StateTransition(fromState, toState);
                return (transition, kvp.Value);
            })
            .ToList();
        }

        ///<summary>
        ///Clears the transition history.
        ///</summary>
        public void ClearHistory()
        {
            _transitionHistory.Clear();
            _stateEnterTimes.Clear();
            System.Diagnostics.Debug.WriteLine("AdvancedStateMachine: Transition history cleared");
        }

        ///<summary>
        ///Exports the transition history to a string format.
        ///</summary>
        ///<returns>String representation of transition history.</returns>
        public string ExportHistory()
        {
            var lines = new List<string>
            {
                "State Transition History",
                $"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}",
                $"Total Transitions: {_transitionHistory.Count}",
                ""
            };

            foreach (var transition in _transitionHistory)
            {
                lines.Add($"{transition.Timestamp:HH:mm:ss.fff} | {transition.FromState} → {transition.ToState} | {transition.DurationMs}ms | {transition.TriggerEvent?.GetType().Name ?? "None"}");
            }

            return string.Join(Environment.NewLine, lines);
        }

        ///<summary>
        ///Configures the state machine using a state factory.
        ///</summary>
        public void ConfigureWithFactory()
        {
            if (_stateFactory == null)
            {
                System.Diagnostics.Debug.WriteLine("AdvancedStateMachine: No StateFactory set for configuration");
                return;
            }

            _stateFactory.ConfigureStateMachine(this);
            System.Diagnostics.Debug.WriteLine("AdvancedStateMachine: Configured with StateFactory");
        }

        ///<summary>
        ///Adds a transition to the history.
        ///</summary>
        ///<param name="transition">The transition to add.</param>
        private void AddToHistory(StateTransition transition)
        {
            _transitionHistory.Add(transition);

            //Maintain maximum history size
            while (_transitionHistory.Count > _maxHistorySize)
            {
                _transitionHistory.RemoveAt(0);
            }
        }
    }

    ///<summary>
    ///Advanced statistics for the StateMachine.
    ///</summary>
    public class AdvancedStateMachineStatistics : StateMachineStatistics
    {
        ///<summary>
        ///Duration the current state has been active.
        ///</summary>
        public TimeSpan CurrentStateDuration { get; set; }

        ///<summary>
        ///Total number of transitions performed.
        ///</summary>
        public int TotalTransitions { get; set; }

        ///<summary>
        ///Average state duration in milliseconds.
        ///</summary>
        public double AverageStateDuration { get; set; }

        ///<summary>
        ///The most frequent transition.
        ///</summary>
        public (StateTransition Transition, int Count) MostFrequentTransition { get; set; }

        ///<summary>
        ///Dictionary of state enter times.
        ///</summary>
        public Dictionary<GameStateType, DateTime> StateEnterTimes { get; set; }

        ///<summary>
        ///Returns a string representation of the advanced statistics.
        ///</summary>
        public override string ToString()
        {
            return $"Advanced StateMachine Stats: Current={CurrentState} ({CurrentStateDuration.TotalSeconds:F1}s), " +
            $"Transitions={TotalTransitions}, AvgDuration={AverageStateDuration:F1}ms, " +
            $"MostFrequent={MostFrequentTransition.Transition.FromState}→{MostFrequentTransition.Transition.ToState} ({MostFrequentTransition.Count}x)";
        }
    }
}




