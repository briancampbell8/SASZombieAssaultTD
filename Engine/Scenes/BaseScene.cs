// =====================================================================================================
//  FILE: BaseScene.cs
//  PATH: Engine/Scenes/BaseScene.cs
//  SUBSYSTEM: Scene System / Deterministic Scene Lifecycle Stage
//
//  ROLE:
//      Authoritative base class for all engine-hosted scenes. Provides the deterministic lifecycle
//      contract used by SceneManager and GameRootMain. Represents a single pipeline stage in the
//      scene processing system.
//
//  RESPONSIBILITIES:
//      - Provide canonical lifecycle surface:
//          • SetGameRoot(GameRootMain root)
//          • SetSceneManager(SceneManager manager)
//          • SetInputRouter(UIInputRouter router)
//          • OnLoad()
//          • OnStart()
//          • OnUpdate(float deltaTime)
//          • OnRender(D3D11Adapter_Core adapter)
//          • OnUnload()
//      - Maintain initialization and start flags.
//      - Serve as the foundation for all gameplay and UI scenes.
//
//  NON-RESPONSIBILITIES:
//      - Scene-specific logic (implemented in derived scenes).
//      - Resource loading, GPU management, or asset lifecycles.
//      - Emitting SceneProcessingFlags (SceneManager handles pipeline confirmation).
//
//  ARCHITECTURAL NOTES:
//      - All scenes MUST inherit from BaseScene.
//      - SceneManager invokes lifecycle methods deterministically.
//      - This file is permanently correct for the new deterministic pipeline.
// =====================================================================================================

using SASZombieAssaultTD.Engine.Input;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;

namespace SASZombieAssaultTD.Engine.Scenes
{
    public abstract class BaseScene
    {
        protected GameRootMain GameRoot;
        protected SceneManager SceneManager;
        protected UIInputRouter InputRouter;

        protected bool _isInitialized;
        protected bool _isStarted;

        // ---------------------------------------------------------------------------------------------
        // ENGINE INJECTION
        // ---------------------------------------------------------------------------------------------

        public virtual void SetGameRoot(GameRootMain root)
        {
            GameRoot = root;
        }

        public virtual void SetSceneManager(SceneManager manager)
        {
            SceneManager = manager;
        }

        public virtual void SetInputRouter(UIInputRouter router)
        {
            InputRouter = router;
        }

        // ---------------------------------------------------------------------------------------------
        // LIFECYCLE (INTERNAL – CALLED BY SceneManager)
        // ---------------------------------------------------------------------------------------------

        internal virtual void OnLoad()
        {
            _isInitialized = true;
        }

        internal virtual void OnStart()
        {
            _isStarted = true;
        }

        internal virtual void OnUpdate(float deltaTime)
        {
        }

        internal virtual void OnRender(D3D11Adapter_Core adapter)
        {
        }

        internal virtual void OnUnload()
        {
            _isInitialized = false;
            _isStarted = false;
        }

        // ---------------------------------------------------------------------------------------------
        // PUBLIC OVERRIDABLE SURFACE (FOR DERIVED SCENES)
        // ---------------------------------------------------------------------------------------------

        public virtual void Initialize()
        {
            _isInitialized = true;
        }

        public virtual void Update(float deltaTime)
        {
        }

        public virtual void Cleanup()
        {
            _isInitialized = false;
            _isStarted = false;
        }

        // ---------------------------------------------------------------------------------------------
        // RENDER DISPATCH
        // ---------------------------------------------------------------------------------------------

        internal void Render(D3D11Adapter_Core uiContext)
        {
            if (uiContext == null)
                return;

            if (!_isInitialized || !_isStarted)
                return;

            OnRender(uiContext);
        }

        internal void Render(D3D11Adapter_Core adapter, float v)
        {
            Render(adapter);
        }
    }
}
