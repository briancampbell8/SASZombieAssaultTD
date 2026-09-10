//==========================================================================================
// FILE: AdapterManager_Initialization.cs
// PATH: Engine/Render/Adapter/AdapterManager_Initialization.cs
// SUBSYSTEM: Rendering / D3D11 Adapter Initialization Workflow
//
// ROLE:
// Provides deterministic initialization workflow for the adapter manager,
// including enumeration, feature negotiation, device creation, swap-chain
// configuration, and validation.
//
// RESPONSIBILITIES:
//  - Enumerate adapters and outputs.
//  - Negotiate and validate feature levels.
//  - Create and validate the D3D11 device.
//  - Create and configure the swap chain.
//  - Register the active device with DeviceOps.
//  - Produce validation reports and fallback logic.
//
// NON-RESPONSIBILITIES:
//  - Frame lifecycle management.
//  - Info aggregation.
//  - GPU operations or rendering logic.
//==========================================================================================

using System;
using SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore;

namespace SASZombieAssaultTD.Engine.Render.D3D11.Adapter.AdapterOps
{
    public sealed class AdapterManager_Initialization
    {
        private readonly D3D11Adapter_Manager _manager;

        public AdapterManager_Initialization(D3D11Adapter_Manager manager)
        {
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
        }

        public void Initialize()
        {
            // Already initialized?
            if ((bool)_manager.Initialized)
                return;

            var enumerate = _manager.EnumerateOps;
            var outputs = _manager.OutputOps;
            var validate = _manager.ValidationOps;
            var features = _manager.FeatureLevelOps;
            var create = _manager.CreateDeviceOps;
            var swap = _manager.SwapChainOps;
            var deviceOps = _manager.DeviceOps;

            // Enumerate adapters and outputs
            enumerate?.EnumerateAdapters();
            outputs?.EnumerateOutputs(0);

            // Feature negotiation
            var negotiatedLevel = features?.NegotiateBestFeatureLevel();
            features?.ValidateFeatureLevel((Vortice.Direct3D.FeatureLevel)negotiatedLevel);

            // Device creation
            var deviceObj = create?.CreateDevice();
            if (deviceObj is D3D11DeviceCore deviceCore)
            {
                create?.ValidateDevice(deviceCore);
                deviceOps?.SetActiveDevice(deviceCore);
            }

            // Swap chain creation
            var swapChain = swap?.CreateSwapChain(null);
            if (swapChain != null)
            {
                swap?.ConfigureSwapChain(swapChain);
                swap?.ValidateSwapChain(swapChain);
            }

            // Validation report (non-fatal)
            validate?.GenerateValidationReport();

            // Mark manager initialized
            _manager.Initialized = true;
        }
    }
}
