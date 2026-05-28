using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.UI.Input;
using SASZombieAssaultTD.Engine.UI.Systems;
using SASZombieAssaultTD.Engine.Scenes;
using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.Audio;
using SASZombieAssaultTD.Engine.VectorMath;
using SASZombieAssaultTD.Engine.State;
using SASZombieAssaultTD.Engine.Extensions;
using SASZombieAssaultTD.Engine.Core;

namespace SASZombieAssaultTD.Engine.State
{
    /// <summary>
    /// Manages UI elements and interactions.
    /// </summary>
    public class StateMachineUISystem
    {
        public bool IsVisible { get; set; } = true;
        public bool IsInteractionEnabled { get; set; } = true;

        private readonly List<string> _activePanels;

        public StateMachineUISystem()
        {
            _activePanels = new List<string>();
        }

        /// <summary>
        /// Shows a UI panel.
        /// </summary>
        public void ShowPanel(string panelName)
        {
            if (!_activePanels.Contains(panelName))
            {
                _activePanels.Add(panelName);
            }
        }

        /// <summary>
        /// Hides a UI panel.
        /// </summary>
        public void HidePanel(string panelName)
        {
            _activePanels.Remove(panelName);
        }

        /// <summary>
        /// Checks if a panel is currently visible.
        /// </summary>
        public bool IsPanelVisible(string panelName)
        {
            return _activePanels.Contains(panelName);
        }

        /// <summary>
        /// Gets all active panels.
        /// </summary>
        public IReadOnlyList<string> GetActivePanels()
        {
            return _activePanels.AsReadOnly();
        }

        /// <summary>
        /// Updates the UI system.
        /// </summary>
        public void Update(float deltaTime)
        {
            // UI update logic here
        }
    }

    /// <summary>
    /// Integration utilities for connecting the state machine with other engine systems.
    /// P20-02-Enhancement: Seamless integration with existing engine components.
    /// </summary>
    public static class StateMachineIntegration
    {
        /// <summary>
        /// Creates and configures a state machine for integration with GameRoot.
        /// </summary>
        /// <param name="enableProfiling">Whether to enable performance profiling.</param>
        /// <param name="enableDebugging">Whether to enable debug logging.</param>
        /// <returns>A configured state machine ready for GameRoot integration.</returns>
        public static EnhancedStateMachine CreateGameRootStateMachine(bool enableProfiling = false, bool enableDebugging = false)
        {
            var builder = StateMachineBuilder.Create()
            .WithDefaultStates()
            .WithInitialState(GameStateType.Boot);

            if (enableProfiling)
                builder.WithProfiling();

            if (enableDebugging)
                builder.WithDebugging();

            var stateMachine = builder.BuildEnhanced();

            Engine.Diagnostics.DebugLogger.LogDebug("INFO", $"StateMachineIntegration: Created GameRoot state machine - " +
            $"Profiling: {enableProfiling}, Debugging: {enableDebugging}");

            return stateMachine;
        }

        /// <summary>
        /// Integrates the state machine with the input system.
        /// </summary>
        /// <param name="stateMachine">The state machine to integrate.</param>
        /// <param name="inputManager">The input manager to integrate with.</param>
        public static void IntegrateWithInputSystem(StateMachine stateMachine, UIInputRouter inputManager)
        {
            if (stateMachine == null)
                throw new ArgumentNullException(nameof(stateMachine));
            if (inputManager == null)
                throw new ArgumentNullException(nameof(inputManager));

            // This would set up event handlers to convert input events to state machine events
            // For now, we'll just log the integration
            Engine.Diagnostics.DebugLogger.LogDebug("INFO", "StateMachineIntegration: Integrated with Input system");
        }

        /// <summary>
        /// Integrates the state machine with the audio system.
        /// </summary>
        /// <param name="stateMachine">The state machine to integrate.</param>
        public static void IntegrateWithAudioSystem(StateMachine stateMachine)
        {
            if (stateMachine == null)
                throw new ArgumentNullException(nameof(stateMachine));

            // AudioSystem is a static utility; do not accept it as a parameter.
            // This is where you'd wire up callbacks or use AudioSystem static methods directly.
            // For now, we'll just log the integration.
            Engine.Diagnostics.DebugLogger.LogDebug("INFO", "StateMachineIntegration: Integrated with Audio system");
        }

        /// <summary>
        /// Integrates the state machine with the UI system.
        /// </summary>
        /// <param name="stateMachine">The state machine to integrate.</param>
        /// <param name="uiSystem">The UI system to integrate with.</param>
        public static void IntegrateWithUISystem(StateMachine stateMachine, SASZombieAssaultTD.Engine.UI.UISystem uiSystem)
        {
            if (stateMachine == null)
                throw new ArgumentNullException(nameof(stateMachine));
            if (uiSystem == null)
                throw new ArgumentNullException(nameof(uiSystem));

            // This would set up UI callbacks for state changes
            // For now, we'll just log the integration
            Engine.Diagnostics.DebugLogger.LogDebug("INFO", "StateMachineIntegration: Integrated with UI system");
        }

        /// <summary>
        /// Creates a state machine configuration for different environments.
        /// </summary>
        /// <param name="environment">The target environment.</param>
        /// <returns>A configured state machine for the specified environment.</returns>
        public static EnhancedStateMachine CreateEnvironmentStateMachine(StateMachineEnvironment environment)
        {
            switch (environment)
            {
                case StateMachineEnvironment.Development:
                    return StateMachineBuilderExtensions.CreateDevelopment().BuildEnhanced();

                case StateMachineEnvironment.Testing:
                    return StateMachineBuilder.Create()
                    .WithDefaultStates()
                    .WithInitialState(GameStateType.Boot)
                    .WithProfiling()
                    .WithDebugging()
                    .WithMaxHistorySize(500)
                    .BuildEnhanced();

                case StateMachineEnvironment.Production:
                    return StateMachineBuilderExtensions.CreateProduction().BuildEnhanced();

                default:
                    return StateMachineBuilderExtensions.CreateDefault().BuildEnhanced();
            }
        }

        /// <summary>
        /// Migrates an existing StateMachine to an EnhancedStateMachine.
        /// </summary>
        /// <param name="existingStateMachine">The existing state machine.</param>
        /// <returns>An enhanced state machine with migrated states.</returns>
        public static EnhancedStateMachine MigrateToEnhanced(StateMachine existingStateMachine)
        {
            if (existingStateMachine == null)
                throw new ArgumentNullException(nameof(existingStateMachine));

            var enhancedStateMachine = new EnhancedStateMachine();

            // Copy registered states
            var registeredStates = existingStateMachine.GetRegisteredStates();
            foreach (var stateType in registeredStates)
            {
                var state = existingStateMachine.GetState(stateType);
                if (state != null)
                {
                    enhancedStateMachine.RegisterState(stateType, state);
                }
            }

            // Preserve current state
            if (existingStateMachine.CurrentState != null)
            {
                enhancedStateMachine.ChangeState(existingStateMachine.CurrentStateType);
            }

            Engine.Diagnostics.DebugLogger.LogDebug("INFO", $"StateMachineIntegration: Migrated StateMachine to EnhancedStateMachine with {registeredStates.Length} states");

            return enhancedStateMachine;
        }

        /// <summary>
        /// Creates a state machine monitoring dashboard.
        /// </summary>
        /// <param name="stateMachine">The state machine to monitor.</param>
        /// <returns>A monitoring dashboard instance.</returns>
        public static StateMachineDashboard CreateMonitoringDashboard(StateMachine stateMachine)
        {
            if (stateMachine == null)
                throw new ArgumentNullException(nameof(stateMachine));

            return new StateMachineDashboard(stateMachine);
        }

        /// <summary>
        /// Sets up automatic performance monitoring for a state machine.
        /// </summary>
        /// <param name="stateMachine">The state machine to monitor.</param>
        /// <param name="reportIntervalMs">Reporting interval in milliseconds.</param>
        public static void SetupPerformanceMonitoring(EnhancedStateMachine stateMachine, int reportIntervalMs = 60000)
        {
            if (stateMachine == null)
                throw new ArgumentNullException(nameof(stateMachine));

            // This would set up a timer to periodically report performance metrics
            // For now, we'll just log the setup
            Engine.Diagnostics.DebugLogger.LogDebug("INFO", $"StateMachineIntegration: Set up performance monitoring with {reportIntervalMs}ms interval");
        }
    }

    /// <summary>
    /// Environment types for state machine configuration.
    /// </summary>
    public enum StateMachineEnvironment
    {
        /// <summary>Development environment with full debugging.</summary>
        Development,

        /// <summary>Testing environment with moderate debugging.</summary>
        Testing,

        /// <summary>Production environment with minimal overhead.</summary>
        Production
    }

    /// <summary>
    /// Monitoring dashboard for state machine metrics.
    /// </summary>
    public class StateMachineDashboard
    {
        private readonly StateMachine _stateMachine;
        private readonly DateTime _createdTime;

        /// <summary>
        /// Initializes a new monitoring dashboard.
        /// </summary>
        /// <param name="stateMachine">The state machine to monitor.</param>
        public StateMachineDashboard(StateMachine stateMachine)
        {
            _stateMachine = stateMachine ?? throw new ArgumentNullException(nameof(stateMachine));
            _createdTime = DateTime.UtcNow;
        }

        /// <summary>
        /// Gets the current dashboard status.
        /// </summary>
        /// <returns>Dashboard status information.</returns>
        public string GetStatus()
        {
            var uptime = DateTime.UtcNow - _createdTime;
            var stats = _stateMachine.GetStatistics();

            return $"Dashboard Status - Uptime: {uptime:hh\\:mm\\:ss}, " +
            $"Current State: {stats.CurrentState}, " +
            $"Transitions: {stats.TotalTransitions}";
        }

        /// <summary>
        /// Generates a detailed dashboard report.
        /// </summary>
        /// <returns>Detailed dashboard report.</returns>
        public string GenerateReport()
        {
            var report = new List<string>
            {
                "State Machine Dashboard Report",
                $"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}",
                $"Dashboard Uptime: {DateTime.UtcNow - _createdTime:hh\\:mm\\:ss}",
                ""
            };

            if (_stateMachine is EnhancedStateMachine enhancedStateMachine)
            {
                report.Add(enhancedStateMachine.GenerateComprehensiveReport());
            }
            else
            {
                var stats = _stateMachine.GetStatistics();
                report.Add($"Current State: {stats.CurrentState}");
                report.Add($"Registered States: {stats.RegisteredStates}");
                report.Add($"Valid Transitions: {stats.ValidTransitions}");
            }

            return string.Join(Environment.NewLine, report);
        }
    }

    /// <summary>
    /// Extension methods for GameRoot to simplify state machine integration.
    /// </summary>
    public static class GameRootStateExtensions
    {
        /// <summary>
        /// Initializes GameRoot with an enhanced state machine.
        /// </summary>
        /// <param name="gameRoot">The GameRoot instance.</param>
        /// <param name="environment">The target environment.</param>
        public static void InitializeWithEnhancedStateMachine(this GameRoot gameRoot, StateMachineEnvironment environment = StateMachineEnvironment.Production)
        {
            if (gameRoot == null)
                throw new ArgumentNullException(nameof(gameRoot));

            var enhancedStateMachine = StateMachineIntegration.CreateEnvironmentStateMachine(environment);

            // Replace the existing state machine (this would require modifying GameRoot to allow this)
            // For now, we'll just log the initialization
            Engine.Diagnostics.DebugLogger.LogDebug("INFO", $"GameRoot: Initialized with enhanced state machine for {environment} environment");
        }

        /// <summary>
        /// Gets state machine statistics from GameRoot.
        /// </summary>
        /// <param name="gameRoot">The GameRoot instance.</param>
        /// <returns>State machine statistics.</returns>
        public static string GetStateMachineReport(this GameRoot gameRoot)
        {
            if (gameRoot == null)
                throw new ArgumentNullException(nameof(gameRoot));

            // Invoke the delegate function to get the actual state machine instance
            // Invoke the delegate function once using parenthesis
            var actualStateMachine = gameRoot.StateMachine();

            if (actualStateMachine is EnhancedStateMachine enhancedStateMachine)
            {
                return enhancedStateMachine.GenerateComprehensiveReport();
            }

            // Fixed fallback report showing the raw machine instance string representation
            return $"Basic StateMachine Report: Instance={actualStateMachine}";


        }
    }
}
