using System;
using System.Collections.Generic;
//
using SASZombieAssaultTD.Engine.Core;

using SASZombieAssaultTD.Engine.Diagnostics;
namespace SASZombieAssaultTD.Engine.State
{
    ///<summary>
    ///Factory for creating and configuring game states.
    ///P20-02-Enhancement: Centralized state creation with dependency injection.
    ///</summary>
    public class StateFactory
    {
        private readonly Dictionary<GameStateType, Func<StateMachine, IGameState>> _stateCreators;
        private readonly Dictionary<GameStateType, object> _stateConfigurations;
        
        ///<summary>
        ///Initializes a new state factory.
        ///</summary>
        public StateFactory()
        {
            _stateCreators = new Dictionary<GameStateType, Func<StateMachine, IGameState>>();
            _stateConfigurations = new Dictionary<GameStateType, object>();
            
            RegisterDefaultStates();
        }
        
        ///<summary>
        ///Creates a state instance of the specified type.
        ///</summary>
        ///<param name="stateType">The type of state to create.</param>
        ///<param name="stateMachine">The state machine instance.</param>
        ///<returns>A new state instance.</returns>
        public IGameState CreateState(GameStateType stateType, StateMachine stateMachine)
        {
            if (!_stateCreators.TryGetValue(stateType, out var creator))
            {
DLogger.Log(LogSubsystems.State,LogLevel.Info,"ERROR",$"StateFactory: No creator registered for state type {stateType}");
                throw new ArgumentException($"No creator registered for state type {stateType}", nameof(stateType));
            }
            
            try
            {
                var state = creator(stateMachine);
                DLogger.Log(LogSubsystems.State,LogLevel.Info, $"StateFactory: Created state {stateType}");
                return state;
            }
            catch (Exception ex)
            {
DLogger.Log(LogSubsystems.State,LogLevel.Info,"ERROR",$"StateFactory: Failed to create state {stateType} - {ex.Message}");
                throw;
            }
        }
        
        ///<summary>
        ///Registers a custom state creator.
        ///</summary>
        ///<param name="stateType">The state type.</param>
        ///<param name="creator">The creator function.</param>
        public void RegisterStateCreator(GameStateType stateType, Func<StateMachine, IGameState> creator)
        {
            if (creator == null)
            throw new ArgumentNullException(nameof(creator));
            
            _stateCreators[stateType] = creator;
            DLogger.Log(LogSubsystems.State,LogLevel.Info, $"StateFactory: Registered custom creator for {stateType}");
        }
        
        ///<summary>
        ///Registers a state configuration.
        ///</summary>
        ///<typeparam name="T">The configuration type.</typeparam>
        ///<param name="stateType">The state type.</param>
        ///<param name="configuration">The configuration object.</param>
        public void RegisterStateConfiguration<T>(GameStateType stateType, T configuration)
        {
            if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));
            
            _stateConfigurations[stateType] = configuration;
            DLogger.Log(LogSubsystems.State,LogLevel.Info, $"StateFactory: Registered configuration for {stateType}");
        }
        
        ///<summary>
        ///Gets the configuration for a state type.
        ///</summary>
        ///<typeparam name="T">The configuration type.</typeparam>
        ///<param name="stateType">The state type.</param>
        ///<returns>The configuration object, or default if not found.</returns>
        public T GetStateConfiguration<T>(GameStateType stateType)
        {
            if (_stateConfigurations.TryGetValue(stateType, out var config) && config is T typedConfig)
            {
                return typedConfig;
            }
            
            return default(T);
        }
        
        ///<summary>
        ///Creates and configures all states for a state machine.
        ///</summary>
        ///<param name="stateMachine">The state machine to configure.</param>
        public void ConfigureStateMachine(StateMachine stateMachine)
        {
            if (stateMachine == null)
            throw new ArgumentNullException(nameof(stateMachine));
            
            DLogger.Log(LogSubsystems.State,LogLevel.Info, "StateFactory: Configuring StateMachine with all registered states");
            
            foreach (var stateType in _stateCreators.Keys)
            {
                try
                {
                    var state = CreateState(stateType, stateMachine);
                    stateMachine.RegisterState(stateType, state);
                }
                catch (Exception ex)
                {
 DLogger.Log(LogSubsystems.State,LogLevel.Info,"ERROR",$"StateFactory: Failed to configure state {stateType} - {ex.Message}");
                    //Continue with other states even if one fails
                }
            }
            
            DLogger.Log(LogSubsystems.State,LogLevel.Info, $"StateFactory: Configured {stateMachine.GetRegisteredStates().Length} states");
        }
        
        ///<summary>
        ///Gets all registered state types.
        ///</summary>
        ///<returns>Array of registered state types.</returns>
        public GameStateType[] GetRegisteredStateTypes()
        {
            var stateTypes = new GameStateType[_stateCreators.Count];
            _stateCreators.Keys.CopyTo(stateTypes, 0);
            return stateTypes;
        }
        
        ///<summary>
        ///Checks if a state type is registered.
        ///</summary>
        ///<param name="stateType">The state type to check.</param>
        ///<returns>True if registered, false otherwise.</returns>
        public bool IsStateRegistered(GameStateType stateType)
        {
            return _stateCreators.ContainsKey(stateType);
        }
        
        ///<summary>
        ///Registers the default state creators.
        ///</summary>
        private void RegisterDefaultStates()
        {
            DLogger.Log(LogSubsystems.State,LogLevel.Info, "StateFactory: Registering default state creators");
            
            _stateCreators[GameStateType.Boot] = (sm) => new BootState(sm);
            _stateCreators[GameStateType.MainMenu] = (sm) => new MainMenuState(sm);
            _stateCreators[GameStateType.Gameplay] = (sm) => new GameplayState(sm);
            _stateCreators[GameStateType.Paused] = (sm) => new PausedState(sm);
            
            DLogger.Log(LogSubsystems.State,LogLevel.Info, $"StateFactory: Registered {_stateCreators.Count} default state creators");
        }
    }
    
    ///<summary>
    ///Configuration for BootState.
    ///</summary>
    public class BootStateConfiguration
    {
        ///<summary>
        ///Time to simulate resource loading in milliseconds.
        ///</summary>
        public int ResourceLoadingTimeMs { get; set; } = 1000;
        
        ///<summary>
        ///Whether to auto-transition to MainMenu after loading.
        ///</summary>
        public bool AutoTransitionToMainMenu { get; set; } = true;
        
        ///<summary>
        ///Resources to load during boot.
        ///</summary>
        public List<string> ResourcesToLoad { get; set; } = new List<string>();
    }
    
    ///<summary>
    ///Configuration for MainMenuState.
    ///</summary>
    public class MainMenuStateConfiguration
    {
        ///<summary>
        ///Whether to show background animations.
        ///</summary>
        public bool ShowBackgroundAnimations { get; set; } = true;
        
        ///<summary>
        ///Background music track to play.
        ///</summary>
        public string BackgroundMusicTrack { get; set; } = "menu_theme";
        
        ///<summary>
        ///Menu navigation sensitivity.
        ///</summary>
        public float NavigationSensitivity { get; set; } = 1.0f;
    }
    
    ///<summary>
    ///Configuration for GameplayState.
    ///</summary>
    public class GameplayStateConfiguration
    {
        ///<summary>
        ///Whether to enable auto-pause on focus loss.
        ///</summary>
        public bool AutoPauseOnFocusLoss { get; set; } = true;
        
        ///<summary>
        ///Maximum gameplay time before forced pause (in minutes).
        ///</summary>
        public int MaxGameplayTimeMinutes { get; set; } = 0; //0 = no limit
        
        ///<summary>
        ///Whether to enable performance monitoring.
        ///</summary>
        public bool EnablePerformanceMonitoring { get; set; } = false;
    }
    
    ///<summary>
    ///Configuration for PausedState.
    ///</summary>
    public class PausedStateConfiguration
    {
        ///<summary>
        ///Whether to dim the screen when paused.
        ///</summary>
        public bool DimScreenWhenPaused { get; set; } = true;
        
        ///<summary>
        ///Dim level (0.0 to 1.0, where 1.0 is fully dimmed).
        ///</summary>
        public float DimLevel { get; set; } = 0.7f;
        
        ///<summary>
        ///Whether to show pause menu immediately.
        ///</summary>
        public bool ShowPauseMenuImmediately { get; set; } = true;
        
        ///<summary>
        ///Time before auto-resume (in seconds, 0 = no auto-resume).
        ///</summary>
        public int AutoResumeTimeSeconds { get; set; } = 0;
    }
}




