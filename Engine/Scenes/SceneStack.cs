/*
Program Name: SASZombieAssaultTD
File Path: Engine\Scenes\SceneStack.cs
Purpose: Scene stack management for push/pop/replace navigation semantics.
Features: Scene hierarchy management, navigation flow, scene lifecycle coordination.
*/

using System;
using System.Collections.Generic;
//
using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.Scenes.Battlefields;
namespace SASZombieAssaultTD.Engine.Scenes
{
    ///<summary>
    ///Scene stack for managing scene hierarchy and navigation.
    ///P120-05: Implements push/pop/replace semantics for scene navigation.
    ///</summary>
    public class SceneStack
    {
        private readonly Stack<BaseScene> _sceneStack;
        private SceneManager? _sceneManager;

        ///<summary>
        ///Gets the number of scenes in the stack.
        ///</summary>
        public int Count => _sceneStack.Count;

        ///<summary>
        ///Gets whether the stack is empty.
        ///</summary>
        public bool IsEmpty => _sceneStack.Count == 0;

        ///<summary>
        ///Event fired when a scene is pushed onto the stack.
        ///</summary>
        public event Action<BaseScene>? OnScenePushed;

        ///<summary>
        ///Event fired when a scene is popped from the stack.
        ///</summary>
        public event Action<BaseScene>? OnScenePopped;

        ///<summary>
        ///Event fired when a scene is replaced on the stack.
        ///</summary>
        public event Action<BaseScene, BaseScene>? OnSceneReplaced;

        ///<summary>
        ///Initializes a new scene stack.
        ///</summary>
        public SceneStack()
        {
            _sceneStack = new Stack<BaseScene>();
            Dlogger.Log("Info", "SceneStack: Initialized");
        }

        ///<summary>
        ///Sets the scene manager for scene operations.
        ///</summary>
        ///<param name="sceneManager">The scene manager instance.</param>
        public void SetSceneManager(SceneManager sceneManager)
        {
            _sceneManager = sceneManager;
            Dlogger.Log("Info", "SceneStack: SceneManager set");
        }

        ///<summary>
        ///Pushes a scene onto the stack.
        ///</summary>
        ///<param name="scene">The scene to push.</param>
        ///<returns>True if the scene was pushed successfully.</returns>
        public bool Push(BaseScene scene)
        {
            if (scene == null)
            {
                Dlogger.Log("Error", "SceneStack: Cannot push null scene");
                return false;
            }

            try
            {
                //Deactivate current top scene if exists
                if (_sceneStack.Count > 0)
                {
                    var currentTop = _sceneStack.Peek();
                    currentTop.OnExit();
                }

                //Push new scene
                _sceneStack.Push(scene);
                scene.OnEnter();

                Dlogger.Log("Info", $"SceneStack: Pushed scene '{scene.GetType().Name}' (stack size: {_sceneStack.Count})");
                OnScenePushed?.Invoke(scene);

                return true;
            }
            catch (Exception ex)
            {
                Dlogger.Log("Error", $"SceneStack: Failed to push scene - {ex.Message}");
                return false;
            }
        }

        ///<summary>
        ///Pushes a scene by name (loads through SceneManager).
        ///</summary>
        ///<param name="sceneName">The name of the scene to push.</param>
        ///<returns>True if the scene was pushed successfully.</returns>
        public bool Push(string sceneName)
        {
            if (_sceneManager == null)
            {
                Dlogger.Log("Error", "SceneStack: Cannot push by name - SceneManager not set");
                return false;
            }

            var scene = _sceneManager.LoadScene(sceneName);
            if (scene == null)
            {
                Dlogger.Log("Error", $"SceneStack: Failed to load scene '{sceneName}'");
                return false;
            }

            return Push(scene);
        }

        ///<summary>
        ///Pops the top scene from the stack.
        ///</summary>
        ///<returns>The popped scene, or null if the stack is empty.</returns>
        public BaseScene? Pop()
        {
            if (_sceneStack.Count == 0)
            {
                Dlogger.Log("Warning", "SceneStack: Cannot pop from empty stack");
                return null;
            }

            try
            {
                var poppedScene = _sceneStack.Pop();
                poppedScene.OnExit();
                poppedScene.Cleanup();

                Dlogger.Log("Info", $"SceneStack: Popped scene '{poppedScene.GetType().Name}' (stack size: {_sceneStack.Count})");
                OnScenePopped?.Invoke(poppedScene);

                //Activate new top scene if exists
                if (_sceneStack.Count > 0)
                {
                    var newTop = _sceneStack.Peek();
                    newTop.OnEnter();
                }

                return poppedScene;
            }
            catch (Exception ex)
            {
                Dlogger.Log("Error", $"SceneStack: Failed to pop scene - {ex.Message}");
                return null;
            }
        }

        ///<summary>
        ///Replaces the top scene with a new scene.
        ///</summary>
        ///<param name="scene">The new scene to replace with.</param>
        ///<returns>The replaced scene, or null if the stack is empty.</returns>
        public BaseScene? Replace(BaseScene scene)
        {
            if (scene == null)
            {
                Dlogger.Log("Error", "SceneStack: Cannot replace with null scene");
                return null;
            }

            if (_sceneStack.Count == 0)
            {
                Dlogger.Log("Warning", "SceneStack: Cannot replace on empty stack, pushing instead");
                Push(scene);
                return null;
            }

            try
            {
                var replacedScene = _sceneStack.Pop();
                replacedScene.OnExit();
                replacedScene.Cleanup();

                _sceneStack.Push(scene);
                scene.OnEnter();

                Dlogger.Log("Info", $"SceneStack: Replaced '{replacedScene.GetType().Name}' with '{scene.GetType().Name}'");
                OnSceneReplaced?.Invoke(replacedScene, scene);

                return replacedScene;
            }
            catch (Exception ex)
            {
                Dlogger.Log("Error", $"SceneStack: Failed to replace scene - {ex.Message}");
                return null;
            }
        }

        ///<summary>
        ///Replaces the top scene with a scene by name.
        ///</summary>
        ///<param name="sceneName">The name of the scene to replace with.</param>
        ///<returns>The replaced scene, or null if the stack is empty.</returns>
        public BaseScene? Replace(string sceneName)
        {
            if (_sceneManager == null)
            {
                Dlogger.Log("Error", "SceneStack: Cannot replace by name - SceneManager not set");
                return null;
            }

            var scene = _sceneManager.LoadScene(sceneName);
            if (scene == null)
            {
                Dlogger.Log("Error", $"SceneStack: Failed to load scene '{sceneName}'");
                return null;
            }

            return Replace(scene);
        }

        ///<summary>
        ///Peeks at the top scene without removing it.
        ///</summary>
        ///<returns>The top scene, or null if the stack is empty.</returns>
        public BaseScene? Peek()
        {
            if (_sceneStack.Count == 0)
            {
                return null;
            }

            return _sceneStack.Peek();
        }

        ///<summary>
        ///Clears all scenes from the stack.
        ///</summary>
        public void Clear()
        {
            if (_sceneStack.Count == 0)
            {
                Dlogger.Log("Warning", "SceneStack: Stack is already empty");
                return;
            }

            try
            {
                var count = _sceneStack.Count;
                while (_sceneStack.Count > 0)
                {
                    var scene = _sceneStack.Pop();
                    scene.OnExit();
                    scene.Cleanup();
                }

                Dlogger.Log("Info", $"SceneStack: Cleared {count} scenes from stack");
            }
            catch (Exception ex)
            {
                Dlogger.Log("Error", $"SceneStack: Failed to clear stack - {ex.Message}");
            }
        }

        ///<summary>
        ///Gets all scenes in the stack (from bottom to top).
        ///</summary>
        ///<returns>Array of scenes in the stack.</returns>
        public BaseScene[] GetAllScenes()
        {
            return _sceneStack.ToArray();
        }

        ///<summary>
        ///Updates the top scene in the stack.
        ///</summary>
        ///<param name="deltaTime">Time elapsed since last update.</param>
        public void Update(float deltaTime)
        {
            var topScene = Peek();
            if (topScene != null)
            {
                topScene.Update(deltaTime);
            }
        }

        ///<summary>
        ///Renders the top scene in the stack.
        ///</summary>
        ///<param name="context">The render context.</param>
        public void Render(IDrawingContext context)
        {
            var topScene = Peek();
            if (topScene != null)
            {
                topScene.Render(context);
            }
        }

        ///<summary>
        ///Gets scene stack information as a string.
        ///</summary>
        public override string ToString()
        {
            var sceneNames = new List<string>();
            foreach (var scene in _sceneStack)
            {
                sceneNames.Add(scene.GetType().Name);
            }
            sceneNames.Reverse(); //Show from bottom to top

            return $"SceneStack: Count={_sceneStack.Count}, Scenes=[{string.Join(" -> ", sceneNames)}]";
        }
    }
}
