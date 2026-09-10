//==========================================================================================
// FILE: AdapterManager_Info.cs
// PATH: Engine/Render/Adapter/AdapterManager_Info.cs
// SUBSYSTEM: Rendering / D3D11 Adapter Manager Information
//
// ROLE:
// Provides deterministic aggregation of manager-level information including
// dimensions, subsystem availability, and backend context exposure.
//
// RESPONSIBILITIES:
//  - Aggregate manager-level info.
//  - Report subsystem availability.
//  - Expose backend context safely.
//  - Provide structured info objects.
//
// NON-RESPONSIBILITIES:
//  - Initialization workflow.
//  - Frame lifecycle management.
//  - GPU operations or rendering logic.
//==========================================================================================

using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;

namespace SASZombieAssaultTD.Engine.Render.D3D11.Adapter.AdapterOps
{
    public sealed class AdapterManager_Info
    {
        private readonly D3D11Adapter_Manager _manager;

        public AdapterManager_Info(D3D11Adapter_Manager manager)
        {
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
        }

        public object GetManagerInfo()
        {
            return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                // Dimensions
                ["Width"] = _manager.Width,
                ["Height"] = _manager.Height,
                ["AspectRatio"] = _manager.AspectRatio,

                // Initialization state
                ["Initialized"] = _manager.Initialized,

                // Subsystem availability
                ["EnumerateOpsAvailable"] = _manager.EnumerateOps != null,
                ["CreateDeviceOpsAvailable"] = _manager.CreateDeviceOps != null,
                ["FeatureLevelOpsAvailable"] = _manager.FeatureLevelOps != null,
                ["DebugOpsAvailable"] = _manager.DebugOps != null,
                ["OutputOpsAvailable"] = _manager.OutputOps != null,
                ["ValidationOpsAvailable"] = _manager.ValidationOps != null,
                ["InfoOpsAvailable"] = _manager.InfoOps != null,
                ["SwapChainOpsAvailable"] = _manager.SwapChainOps != null,
                ["PipelineOpsAvailable"] = _manager.PipelineOps != null,
                ["FrameOpsAvailable"] = _manager.FrameOps != null,
                ["ResourceOpsAvailable"] = _manager.ResourceOps != null,
                ["StateOpsAvailable"] = _manager.StateOps != null,
                ["DeviceOpsAvailable"] = _manager.DeviceOps != null,

                // Backend context availability
                ["BackendContextAvailable"] = GetBackendContext() != null
            };
        }

        public object GetBackendContext()
        {
            // Safe backend context exposure
            return _manager.BackendContext;
        }
    }
}
