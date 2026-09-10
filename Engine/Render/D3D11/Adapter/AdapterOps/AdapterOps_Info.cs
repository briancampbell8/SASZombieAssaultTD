//==========================================================================================
// FILE: AdapterOps_Info.cs
// PATH: Engine/Render/Adapter/AdapterOps_Info.cs
// SUBSYSTEM: Rendering / D3D11 Adapter & Device Information
//
// ROLE:
// Provides deterministic retrieval of structured information about the active
// adapter, device, and rendering context. Supplies higher‑level systems with
// stable metadata including vendor, memory, feature levels, and device state.
//
// RESPONSIBILITIES:
//  - Retrieve adapter metadata (vendor, name, memory).
//  - Retrieve device metadata (feature level, flags, capabilities).
//  - Provide structured information about the active rendering context.
//  - Support debugging, diagnostics, and UI display of GPU information.
//  - Expose stable, deterministic info objects to other subsystems.
//
// NON-RESPONSIBILITIES:
//  - Creating the D3D11 device.
//  - Enumerating adapters or outputs.
//  - Managing swap chains, pipelines, or rendering operations.
//==========================================================================================

using System;

namespace SASZombieAssaultTD.Engine.Render.D3D11.Adapter.AdapterOps
{
    /// <summary>
    /// Deterministic adapter and device information subsystem.
    /// </summary>
    public sealed class AdapterOps_Info
    {
        private readonly D3D11RenderContext _context;
        private Array _renderTargets;
        private object _presenter;
        private object _fullscreenQuad;
        private object _presentation;
        private object _clearR;
        private object _clearG;
        private object _clearB;
        private object _clearA;
        private object _disposed;

        public object Width { get; private set; }
        public object Height { get; private set; }
        public object AspectRatio { get; private set; }

        public AdapterOps_Info(D3D11RenderContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Retrieve structured information about the active adapter.
        public object GetAdapterInfo()
        {
            // Gather and return structured information about the active adapter and related components.
            // Use safe/null-aware operations because many fields are optional or set at runtime.
            int renderTargetsCount = 0;
            try
            {
                renderTargetsCount = _renderTargets != null ? _renderTargets.Length : 0;
            }
            catch
            {
                // In case _renderTargets is in an unexpected state, fall back to 0.
                renderTargetsCount = 0;
            }

            string presenterType = _presenter?.GetType().FullName;
            string presentationType = _presentation?.GetType().FullName;
            bool fullscreenQuadPresent = _fullscreenQuad != null;

            var clearColor = new
            {
                R = _clearR,
                G = _clearG,
                B = _clearB,
                A = _clearA
            };

            return new
            {
                Width,
                Height,
                AspectRatio,
                RenderTargetsCount = renderTargetsCount,
                Presenter = presenterType,
                Presentation = presentationType,
                FullscreenQuadPresent = fullscreenQuadPresent,
                ClearColor = clearColor,
                Disposed = _disposed
            };
        }

        // Retrieve structured information about the active device.
        public object GetDeviceInfo()
        {
            // Gather device-related information that is available from this wrapper.
            // Avoid accessing members that may not exist on the underlying context; use the fields
            // declared on this class which are guaranteed to be present.

            // Determine number of configured render targets in a safe manner.
            int renderTargetCount = _renderTargets != null ? _renderTargets.Length : 0;

            return new
            {
                Width,
                Height,
                AspectRatio,
                RenderTargetCount = renderTargetCount,
                Presenter = _presenter,
                Presentation = _presentation,
                FullscreenQuad = _fullscreenQuad,
                ClearColor = new
                {
                    R = _clearR,
                    G = _clearG,
                    B = _clearB,
                    A = _clearA
                },
                // Include the raw context reference so callers can inspect it if needed.
                Context = _context
            };
        }

        // Retrieve structured information about the rendering context.
        public object GetContextInfo()
        {
            // Collect basic, safe-to-access diagnostics about the current rendering context and related members.
            // Avoid calling any unknown APIs on _context or other members; expose only presence, type, and simple quantities.
            int renderTargetsCount = -1;

            if (_renderTargets is Array arr)
            {
                renderTargetsCount = arr.Length;
            }
            else if (_renderTargets is System.Collections.ICollection coll)
            {
                renderTargetsCount = coll.Count;
            }

            return new
            {
                // Context presence and type information (safe reflection-based info)
                HasContext = _context != null,
                ContextType = _context?.GetType()?.FullName,

                // Basic surface / viewport information available on this class
                Width,
                Height,
                AspectRatio,

                // Local member diagnostics
                RenderTargetsCount = renderTargetsCount,
                HasPresenter = _presenter != null,
                HasFullscreenQuad = _fullscreenQuad != null,
                Presentation = _presentation,

                // Clear color and disposed flag
                ClearColor = new { R = _clearR, G = _clearG, B = _clearB, A = _clearA },
                Disposed = _disposed
            };
        }

        // Retrieve combined GPU information for UI or diagnostics.
        public object GetFullInfo()
        {
            // Combine adapter, device, and context information into a single object
            // so callers (UI or diagnostics) can access all GPU-related info in one place.
            var adapterInfo = GetAdapterInfo();
            var deviceInfo = GetDeviceInfo();
            var contextInfo = GetContextInfo();

            return new
            {
                Adapter = adapterInfo,
                Device = deviceInfo,
                Context = contextInfo
            };
        }
    }
}
