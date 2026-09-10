//==========================================================================================
// FILE: D3D11Adapter_Manager.cs
// PATH: Engine/Render/Adapter/D3D11Adapter_Manager.cs
// SUBSYSTEM: Rendering / D3D11 Adapter Manager (Orchestration Layer)
//
// ROLE:
// Acts as the deterministic root orchestrator for all AdapterOps_* subsystems.
// Delegates initialization, frame lifecycle, and info reporting to specialized
// manager components. Provides a unified, stable interface for all rendering
// subsystems requiring GPU access.
//
// RESPONSIBILITIES:
//  - Own and expose all AdapterOps_* subsystem instances.
//  - Delegate initialization workflow to AdapterManager_Initialization.
//  - Delegate frame lifecycle to AdapterManager_FrameLifecycle.
//  - Delegate info reporting to AdapterManager_Info.
//  - Provide stable access to device, context, swap-chain, and pipeline subsystems.
//
// NON-RESPONSIBILITIES:
//  - Implementing GPU operations directly.
//  - Managing game-level rendering logic or scene composition.
//  - Performing initialization or frame sequencing internally.
//==========================================================================================

using System;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter.AdapterOps;

namespace SASZombieAssaultTD.Engine.Render.D3D11.Adapter
{
    public sealed class D3D11Adapter_Manager
    {
        private readonly D3D11RenderContext _context;

        // Subsystems
        public AdapterOps_Enumerate EnumerateOps { get; }
        public AdapterOps_CreateDevice CreateDeviceOps { get; }
        public AdapterOps_FeatureLevels FeatureLevelOps { get; }
        public AdapterOps_Debug DebugOps { get; }
        public AdapterOps_Outputs OutputOps { get; }
        public AdapterOps_Validation ValidationOps { get; }
        public AdapterOps_Info InfoOps { get; }
        public AdapterOps_SwapChain SwapChainOps { get; }
        public AdapterOps_Pipeline PipelineOps { get; }
        public AdapterOps_Frame FrameOps { get; }
        public AdapterOps_Resources ResourceOps { get; }
        public AdapterOps_States StateOps { get; }
        public AdapterOps_Device DeviceOps { get; }

        // Specialized managers
        public AdapterManager_Initialization InitManager { get; }
        public AdapterManager_FrameLifecycle FrameManager { get; }
        public AdapterManager_Info InfoManager { get; }
        public object BackendContext { get; internal set; }
        public object Width { get; internal set; }
        public object Height { get; internal set; }
        public object AspectRatio { get; internal set; }
        public object Initialized { get; internal set; }

        public D3D11Adapter_Manager(D3D11RenderContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            // Construct subsystems
            EnumerateOps = new AdapterOps_Enumerate(context);
            CreateDeviceOps = new AdapterOps_CreateDevice(context);
            FeatureLevelOps = new AdapterOps_FeatureLevels(context);
            DebugOps = new AdapterOps_Debug(context);
            OutputOps = new AdapterOps_Outputs(context);
            ValidationOps = new AdapterOps_Validation(context);
            InfoOps = new AdapterOps_Info(context);
            SwapChainOps = new AdapterOps_SwapChain(context);
            PipelineOps = new AdapterOps_Pipeline(context);
            FrameOps = new AdapterOps_Frame(context);
            ResourceOps = new AdapterOps_Resources(context);
            StateOps = new AdapterOps_States(context);
            DeviceOps = new AdapterOps_Device(context);

            // Construct specialized managers
            InitManager = new AdapterManager_Initialization(this);
            FrameManager = new AdapterManager_FrameLifecycle(this);
            InfoManager = new AdapterManager_Info(this);
        }

        public void Initialize() => InitManager.Initialize();
        public void BeginFrame() => FrameManager.BeginFrame();
        public void EndFrame() => FrameManager.EndFrame();
        public void Present() => FrameManager.Present();
        public object GetManagerInfo() => InfoManager.GetManagerInfo();

        internal object GetBackendContext()
        {
            // Return the stored backend context for consumers.
            // The backend context is an object that typically exposes Device and DeviceContext members.
            return BackendContext;
        }
    }
}
