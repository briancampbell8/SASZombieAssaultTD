using System;
//
using SASZombieAssaultTD.Engine.Core;

using SASZombieAssaultTD.Engine.Diagnostics;
namespace SASZombieAssaultTD.Engine.State
{
    ///<summary>
    ///Simple test class to verify state machine functionality.
    ///P20-02-10: Verification test for state transitions and event handling.
    ///</summary>
    public static class StateMachineTest
    {
        ///<summary>
        ///Runs basic state machine verification tests.
        ///</summary>
        public static void RunVerificationTests()
        {
            DLogger.Log(LogSubsystems.State,LogLevel.Info, "StateMachineTest: Starting verification tests");
            
            try
            {
                //Test 1: State machine creation and initialization
                TestStateMachineCreation();
                
                //Test 2: State registration
                TestStateRegistration();
                
                //Test 3: State transitions
                TestStateTransitions();
                
                //Test 4: Event handling
                TestEventHandling();
                
                DLogger.Log(LogSubsystems.State,LogLevel.Info, "StateMachineTest: All verification tests passed");
            }
            catch (Exception ex)
            {
DLogger.Log(LogSubsystems.State,LogLevel.Info,"ERROR",$"StateMachineTest: Verification failed - {ex.Message}");
                throw;
            }
        }
        
        private static void TestStateMachineCreation()
        {
            DLogger.Log(LogSubsystems.State,LogLevel.Info, "StateMachineTest: Testing state machine creation");
            
            var stateMachine = new StateMachine();
            
            //Verify initial state
            if (stateMachine.CurrentStateType != GameStateType.Boot)
            {
                throw new Exception($"Expected initial state Boot, got {stateMachine.CurrentStateType}");
            }
            
            if (stateMachine.CurrentState != null)
            {
                throw new Exception("Expected null current state before initialization");
            }
            
            DLogger.Log(LogSubsystems.State,LogLevel.Info, "StateMachineTest: State machine creation test passed");
        }
        
        private static void TestStateRegistration()
        {
            DLogger.Log(LogSubsystems.State,LogLevel.Info, "StateMachineTest: Testing state registration");
            
            var stateMachine = new StateMachine();
            
            //Register test states
            var bootState = new BootState(stateMachine);
            var mainMenuState = new MainMenuState(stateMachine);
            
            stateMachine.RegisterState(GameStateType.Boot, bootState);
            stateMachine.RegisterState(GameStateType.MainMenu, mainMenuState);
            
            //Verify registration
            if (!stateMachine.HasState(GameStateType.Boot))
            {
                throw new Exception("Boot state not registered");
            }
            
            if (!stateMachine.HasState(GameStateType.MainMenu))
            {
                throw new Exception("MainMenu state not registered");
            }
            
            var registeredStates = stateMachine.GetRegisteredStates();
            if (registeredStates.Length < 2)
            {
                throw new Exception($"Expected at least 2 registered states, got {registeredStates.Length}");
            }
            
            DLogger.Log(LogSubsystems.State,LogLevel.Info, "StateMachineTest: State registration test passed");
        }
        
        private static void TestStateTransitions()
        {
            DLogger.Log(LogSubsystems.State,LogLevel.Info, "StateMachineTest: Testing state transitions");
            
            var stateMachine = new StateMachine();
            
            //Register states
            stateMachine.RegisterState(GameStateType.Boot, new BootState(stateMachine));
            stateMachine.RegisterState(GameStateType.MainMenu, new MainMenuState(stateMachine));
            stateMachine.RegisterState(GameStateType.Gameplay, new GameplayState(stateMachine));
            stateMachine.RegisterState(GameStateType.Paused, new PausedState(stateMachine));
            
            //Test Boot -> MainMenu transition
            stateMachine.ChangeState(GameStateType.Boot);
            if (stateMachine.CurrentStateType != GameStateType.Boot)
            {
                throw new Exception($"Expected Boot state, got {stateMachine.CurrentStateType}");
            }
            
            //Test MainMenu transition
            stateMachine.ChangeState(GameStateType.MainMenu);
            if (stateMachine.CurrentStateType != GameStateType.MainMenu)
            {
                throw new Exception($"Expected MainMenu state, got {stateMachine.CurrentStateType}");
            }
            
            //Test Gameplay transition
            stateMachine.ChangeState(GameStateType.Gameplay);
            if (stateMachine.CurrentStateType != GameStateType.Gameplay)
            {
                throw new Exception($"Expected Gameplay state, got {stateMachine.CurrentStateType}");
            }
            
            //Test Paused transition
            stateMachine.ChangeState(GameStateType.Paused);
            if (stateMachine.CurrentStateType != GameStateType.Paused)
            {
                throw new Exception($"Expected Paused state, got {stateMachine.CurrentStateType}");
            }
            
            DLogger.Log(LogSubsystems.State,LogLevel.Info, "StateMachineTest: State transitions test passed");
        }
        
        private static void TestEventHandling()
        {
            DLogger.Log(LogSubsystems.State,LogLevel.Info, "StateMachineTest: Testing event handling");
            
            var stateMachine = new StateMachine();
            
            //Register states
            stateMachine.RegisterState(GameStateType.MainMenu, new MainMenuState(stateMachine));
            stateMachine.RegisterState(GameStateType.Gameplay, new GameplayState(stateMachine));
            
            //Set to MainMenu state
            stateMachine.ChangeState(GameStateType.MainMenu);
            
            //Test menu input event
            var menuEvent = new MenuInputEvent(MenuAction.StartGame);
            stateMachine.HandleEvent(menuEvent);
            
            //Should have transitioned to Gameplay
            if (stateMachine.CurrentStateType != GameStateType.Gameplay)
            {
                throw new Exception($"Expected Gameplay state after StartGame event, got {stateMachine.CurrentStateType}");
            }
            
            //Test pause event
            var pauseEvent = new GameplayInputEvent(GameplayAction.Pause);
            stateMachine.HandleEvent(pauseEvent);
            
            //Should have transitioned to Paused
            if (stateMachine.CurrentStateType != GameStateType.Paused)
            {
                throw new Exception($"Expected Paused state after Pause event, got {stateMachine.CurrentStateType}");
            }
            
            //Test resume event
            var resumeEvent = new PauseInputEvent(PauseAction.Resume);
            stateMachine.HandleEvent(resumeEvent);
            
            //Should have transitioned back to Gameplay
            if (stateMachine.CurrentStateType != GameStateType.Gameplay)
            {
                throw new Exception($"Expected Gameplay state after Resume event, got {stateMachine.CurrentStateType}");
            }
            
            DLogger.Log(LogSubsystems.State,LogLevel.Info, "StateMachineTest: Event handling test passed");
        }
    }
}




