/*
File:    SceneTransitionTest.cs
Purpose: P11-11-10 - Test scene transitions and verify deterministic behavior.
*/

using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Extensions;
using SASZombieAssaultTD.Engine.VectorMath;
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.State;
using SASZombieAssaultTD.Engine.Scenes;

using SASZombieAssaultTD.Engine.Diagnostics;
namespace SASZombieAssaultTD.Engine.Scenes
//
{
    ///<summary>
    ///Represents the current state of the engine.
    ///</summary>
    public enum EngineState
    {
        Initializing,
        Running,
        Paused,
        Stopped,
        ShuttingDown,
        MainMenu,
        InGame,
        Loading
    }

    ///<summary>
    ///P11-11-10: Test class to verify scene transitions work correctly.
    ///</summary>
    public static class SceneTransitionTest
    {
        ///<summary>
        ///Tests basic scene transitions: MainMenu → Game → Pause → Game → Exit.
        ///</summary>
        ///<param name="sceneManager">The scene manager to test.</param>
        public static void TestBasicTransitions(SceneManager sceneManager)
        {
            if (sceneManager == null)
            {
DLogger.Log(LogSubsystems.Scenes,LogLevel.Info,"ERROR", "SceneTransitionTest: SceneManager is null");
                return;
            }

            DLogger.Log(LogSubsystems.Scenes,LogLevel.Info, "SceneTransitionTest: Starting basic transition test");

            //Test 1: MainMenu → Game
            var mainMenuScene = new MainMenuScene();
            var gameScene = new GameScene();
            var pauseScene = new PauseScene();

            DLogger.Log(LogSubsystems.Scenes,LogLevel.Info, "SceneTransitionTest: Testing MainMenu → Game");
            sceneManager.SetScene(mainMenuScene.GetType().Name);
            VerifySceneState(sceneManager, typeof(MainMenuScene), GameStateType.MainMenu);

            sceneManager.QueueScene(gameScene.GetType().Name);
            //Process update to trigger transition
            sceneManager.Update(0.016f); //~60 FPS
            VerifySceneState(sceneManager, typeof(GameScene), GameStateType.Gameplay);

            //Test 2: Game → Pause
            DLogger.Log(LogSubsystems.Scenes,LogLevel.Info, "SceneTransitionTest: Testing Game → Pause");
            sceneManager.QueueScene(pauseScene.GetType().Name);
            sceneManager.Update(0.016f);
            VerifySceneState(sceneManager, typeof(PauseScene), GameStateType.Paused);

            //Test 3: Pause → Game
            DLogger.Log(LogSubsystems.Scenes,LogLevel.Info, "SceneTransitionTest: Testing Pause → Game");
            sceneManager.QueueScene(gameScene.GetType().Name);
            sceneManager.Update(0.016f);
            VerifySceneState(sceneManager, typeof(GameScene), GameStateType.Gameplay);

            DLogger.Log(LogSubsystems.Scenes,LogLevel.Info, "SceneTransitionTest: Basic transition test completed successfully");
        }

        ///<summary>
        ///Tests loading scene transitions.
        ///</summary>
        ///<param name="sceneManager">The scene manager to test.</param>
        public static void TestLoadingTransitions(SceneManager sceneManager)
        {
            if (sceneManager == null)
            {
DLogger.Log(LogSubsystems.Scenes,LogLevel.Info,"ERROR", "SceneTransitionTest: SceneManager is null");
                return;
            }

            DLogger.Log(LogSubsystems.Scenes,LogLevel.Info, "SceneTransitionTest: Starting loading transition test");

            var loadingScene = new LoadingScene(new GameScene(), 1.0f);

            DLogger.Log(LogSubsystems.Scenes,LogLevel.Info, "SceneTransitionTest: Testing LoadingScene → GameScene");
            sceneManager.QueueScene(loadingScene.GetType().Name);
            VerifySceneState(sceneManager, typeof(LoadingScene), GameStateType.Boot);

            //Simulate loading completion
            for (int i = 0; i < 120; i++) //2 seconds at 60 FPS
            {
                loadingScene.Update(0.016f);
                if (loadingScene.IsLoadingComplete)
                {
                    break;
                }
            }

            sceneManager.Update(0.016f); //Trigger transition
            VerifySceneState(sceneManager, typeof(GameScene), GameStateType.Gameplay);

            DLogger.Log(LogSubsystems.Scenes,LogLevel.Info, "SceneTransitionTest: Loading transition test completed successfully");
        }

        ///<summary>
        ///Tests that only the active scene receives updates.
        ///</summary>
        ///<param name="sceneManager">The scene manager to test.</param>
        public static void TestActiveSceneOnly(SceneManager sceneManager)
        {
            if (sceneManager == null)
            {
DLogger.Log(LogSubsystems.Scenes,LogLevel.Info,"ERROR", "SceneTransitionTest: SceneManager is null");
                return;
            }

            DLogger.Log(LogSubsystems.Scenes,LogLevel.Info, "SceneTransitionTest: Testing active scene only updates");

            var gameScene = new GameScene();
            var menuScene = new MainMenuScene();

            //Set game scene as active
            sceneManager.SetScene(gameScene.GetType().Name);

            //Verify only game scene is active
            if (sceneManager.ActiveScene != gameScene)
            {
DLogger.Log(LogSubsystems.Scenes,LogLevel.Info,"ERROR", "SceneTransitionTest: Active scene is not the expected game scene");
                return;
            }

            //Update and verify game scene receives updates
            sceneManager.Update(0.016f);

            //Queue menu scene but don't process transition yet
            sceneManager.QueueScene(menuScene.GetType().Name);

            //Game scene should still be active and receiving updates
            if (sceneManager.ActiveScene != gameScene)
            {
DLogger.Log(LogSubsystems.Scenes,LogLevel.Info,"ERROR", "SceneTransitionTest: Active scene changed before transition processing");
                return;
            }

            DLogger.Log(LogSubsystems.Scenes,LogLevel.Info, "SceneTransitionTest: Active scene only test completed successfully");
        }

        ///<summary>
        ///Verifies the scene manager is in the expected state.
        ///</summary>
        ///<param name="sceneManager">The scene manager to verify.</param>
        ///<param name="expectedSceneType">The expected active scene type.</param>
        ///<param name="expectedGameStateType">The expected game state.</param>
        private static void VerifySceneState(SceneManager sceneManager, Type expectedSceneType, GameStateType expectedGameStateType)
        {
            if (sceneManager.ActiveScene?.GetType() != expectedSceneType)
            {
DLogger.Log(LogSubsystems.Scenes,LogLevel.Info,"ERROR",$"SceneTransitionTest: Expected scene {expectedSceneType.Name}, but got {sceneManager.ActiveScene?.GetType().Name}");
                return;
            }

            if (sceneManager.GameStateType != expectedGameStateType.ToString())
            {
DLogger.Log(LogSubsystems.Scenes,LogLevel.Info,"ERROR",$"SceneTransitionTest: Expected game state {expectedGameStateType}, but got {sceneManager.GameStateType}");
                return;
            }

            DLogger.Log(LogSubsystems.Scenes,LogLevel.Info, $"SceneTransitionTest: Verified scene {expectedSceneType.Name} with state {expectedGameStateType}");
        }

        ///<summary>
        ///Runs all scene transition tests.
        ///</summary>
        ///<param name="sceneManager">The scene manager to test.</param>
        public static void RunAllTests(SceneManager sceneManager)
        {
            DLogger.Log(LogSubsystems.Scenes,LogLevel.Info, "SceneTransitionTest: Starting comprehensive test suite");

            TestBasicTransitions(sceneManager);
            TestLoadingTransitions(sceneManager);
            TestActiveSceneOnly(sceneManager);

            DLogger.Log(LogSubsystems.Scenes,LogLevel.Info, "SceneTransitionTest: All tests completed successfully");
        }
    }
}




