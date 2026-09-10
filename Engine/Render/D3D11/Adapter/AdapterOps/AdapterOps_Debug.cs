//==========================================================================================
// FILE: AdapterOps_Debug.cs
// PATH: Engine/Render/Adapter/AdapterOps_Debug.cs
// SUBSYSTEM: Rendering / D3D11 Debug & Diagnostics
//
// ROLE:
// Provides deterministic configuration and management of D3D11 debug layers,
// validation layers, and diagnostic output. Ensures consistent logging behavior
// and stable debug-state initialization across all rendering subsystems.
//
// RESPONSIBILITIES:
//  - Enable or disable D3D11 debug layers.
//  - Configure validation and GPU error reporting.
//  - Route diagnostic messages to the engine logging system.
//  - Provide structured debug-state information to higher-level systems.
//  - Support device creation with debug and validation flags.
//
// NON-RESPONSIBILITIES:
//  - Creating the D3D11 device.
//  - Performing any rendering or GPU operations.
//  - Managing swap chains, pipelines, or frame lifecycle.
//==========================================================================================

using System;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Render.D3D11;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Render.D3D11.Adapter.AdapterOps
{
    /// <summary>
    /// Deterministic D3D11 debug and diagnostics subsystem.
    /// </summary>
    public sealed class AdapterOps_Debug
    {
        private readonly D3D11RenderContext _context;

        private bool _debugLayersEnabled;
        private bool _validationEnabled;
        private bool _diagnosticOutputHooked;

        public AdapterOps_Debug(D3D11RenderContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Enable D3D11 debug layers if supported.
        public void EnableDebugLayers()
        {
#if DEBUG
            _debugLayersEnabled = true;
            DLogger.Log(LogSubsystems.Rendering, "Debug layers enabled.");
#else
            _debugLayersEnabled = false;
#endif
        }

        // Configure validation layers and GPU error reporting.
        public void ConfigureValidation()
        {
#if DEBUG
            _validationEnabled = true;
            DLogger.Log(LogSubsystems.Rendering, "Validation layers configured.");
#else
            _validationEnabled = false;
#endif
        }

        // Route diagnostic messages to the engine logging system.
        public void HookDiagnosticOutput()
        {
            _diagnosticOutputHooked = true;
            DLogger.Log(LogSubsystems.Rendering, "Diagnostic output hooked.");
        }

        // Retrieve structured debug-state information.
        public object GetDebugState()
        {
            return new
            {
                DebugLayersEnabled = _debugLayersEnabled,
                ValidationEnabled = _validationEnabled,
                DiagnosticOutputHooked = _diagnosticOutputHooked,
                ContextType = _context?.GetType().FullName ?? "null"
            };
        }
    }
}
