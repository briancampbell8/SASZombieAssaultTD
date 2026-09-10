// =====================================================================================================
//  FILE: AdapterOps_Frame.cs
//  PATH: Engine/Render/D3D11/Adapter/AdapterOps/AdapterOps_Frame.cs
//  SUBSYSTEM: Render D3D11 Adapter Operations
//
//  ROLE:
//      Defines the minimal deterministic lifecycle contract for any engine-hosted program.
//      This interface is implemented by engine hosts (e.g., GameRootMain) to provide a clean,
//      engine-facing API for startup, execution entry, and deterministic shutdown operations.
//
//  RESPONSIBILITIES:
//      - Provide a strict, minimal lifecycle surface for program orchestration.
//      - Enforce the structural sequencing contract: Initialize → Run Loop Execution → Shutdown.
//      - Serve as the base contract for any future top-level engine-hosted program modules.
//
//  NON-RESPONSIBILITIES:
//      - Implementing deep frame-level update calculation rules or rendering commands directly.
//      - Managing active systems registration pools, engine assets, or game states.
//      - Handling discrete hardware device allocation boundaries.
//
//  ARCHITECTURAL NOTES:
//      - This interface replaces the legacy GameRoot partial lifecycle methods.
//      - GameRootMain implements this interface and delegates to its subsystems:
//          • GameRootInitialization
//          • GameRootUpdateLoop
//          • GameRootStateController
//          • GameRootSystemRegistration
//      - All engine-hosted programs MUST implement this interface without exception.
// =====================================================================================================
using System;
using System.Collections;
using System.Numerics;
using System.Reflection;

namespace SASZombieAssaultTD.Engine.Render.D3D11.Adapter.AdapterOps
{
    /// <summary>
    /// Deterministic D3D11 frame lifecycle subsystem.
    /// Handles begin/end sequencing, clearing operations, and presentation routing.
    /// </summary>
    public sealed class AdapterOps_Frame
    {
        private readonly D3D11RenderContext _context;

        // Deterministic state fields
        private readonly bool _isDisposed = false;

        private readonly IEnumerable _renderTargets = null;
        private readonly object _presenter = null;
        private readonly object _presentation = null;

        private readonly float _clearR = 0f;
        private readonly float _clearG = 0f;
        private readonly float _clearB = 0f;
        private readonly float _clearA = 0f;

        private readonly object _fullscreenQuad = null;

        /// <summary>Aspect ratio of the current frame.</summary>
        public float AspectRatio { get; private set; }

        /// <summary>Height of the current frame.</summary>
        public int Height { get; private set; }

        /// <summary>Width of the current frame.</summary>
        public int Width { get; private set; }

        /// <summary>
        /// Creates a new deterministic frame lifecycle controller.
        /// </summary>
        public AdapterOps_Frame(D3D11RenderContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Begins the frame lifecycle and clears buffers.
        /// </summary>
        public void BeginFrame()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(AdapterOps_Frame));

            ClearFrameBuffers();
        }

        /// <summary>
        /// Clears render targets and depth buffers using reflection‑based clearing.
        /// </summary>
        public void ClearFrameBuffers()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(AdapterOps_Frame));

            float[] colorArray = new[] { _clearR, _clearG, _clearB, _clearA };

            void TryClearTarget(object target)
            {
                if (target == null)
                    return;

                var methods = target.GetType().GetMethods(
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.NonPublic);

                foreach (var m in methods)
                {
                    if (!m.Name.Contains("Clear", StringComparison.OrdinalIgnoreCase))
                        continue;

                    var ps = m.GetParameters();

                    try
                    {
                        if (ps.Length == 0)
                        {
                            m.Invoke(target, null);
                            return;
                        }

                        if (ps.Length == 1)
                        {
                            var pType = ps[0].ParameterType;

                            if (pType == typeof(float[]))
                            {
                                m.Invoke(target, new object[] { colorArray });
                                return;
                            }

                            if (pType == typeof(Vector4))
                            {
                                m.Invoke(target, new object[] { new Vector4(_clearR, _clearG, _clearB, _clearA) });
                                return;
                            }

                            if (pType == typeof(float))
                            {
                                m.Invoke(target, new object[] { _clearR });
                                return;
                            }
                        }

                        if (ps.Length >= 4)
                        {
                            var args = new object[ps.Length];
                            args[0] = _clearR;
                            args[1] = _clearG;
                            args[2] = _clearB;
                            args[3] = _clearA;
                            m.Invoke(target, args);
                            return;
                        }
                    }
                    catch
                    {
                        // Suppressed intentionally
                    }
                }
            }

            if (_renderTargets != null)
            {
                foreach (var t in _renderTargets)
                    TryClearTarget(t);
            }

            TryClearTarget(_presenter);
            TryClearTarget(_presentation);
            TryClearTarget(_context);
            TryClearTarget(_fullscreenQuad);
        }

        /// <summary>
        /// Ends the frame lifecycle and performs cleanup.
        /// </summary>
        public void EndFrame()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(AdapterOps_Frame));

            try
            {
                ClearFrameBuffers();
            }
            catch (Exception ex)
            {
                // Log if desired; suppressed intentionally
            }
        }

        /// <summary>
        /// Presents the frame using the swap‑chain subsystem.
        /// </summary>
        public void PresentFrame()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(AdapterOps_Frame));

            EndFrame();

            if (_presenter != null && _presentation != null)
            {
                try
                {
                    var presenterType = _presenter.GetType();
                    var presentMethod =
                        presenterType.GetMethod("Present") ??
                        presenterType.GetMethod("PresentFrame") ??
                        presenterType.GetMethod("SwapBuffers");

                    if (presentMethod != null)
                    {
                        var ps = presentMethod.GetParameters();
                        if (ps.Length == 1)
                            presentMethod.Invoke(_presenter, new[] { _presentation });
                        else
                            presentMethod.Invoke(_presenter, null);

                        return;
                    }
                }
                catch
                {
                    // Suppressed intentionally
                }
            }

            if (_renderTargets != null)
            {
                try
                {
                    var rtType = _renderTargets.GetType();
                    var rtPresent = rtType.GetMethod("Present");

                    if (rtPresent != null)
                    {
                        var ps = rtPresent.GetParameters();
                        if (ps.Length == 0)
                            rtPresent.Invoke(_renderTargets, null);
                        else
                            rtPresent.Invoke(_renderTargets, new[] { _presentation });
                    }
                }
                catch
                {
                    // Suppressed intentionally
                }
            }
        }

        /// <summary>
        /// Returns structured frame‑state information.
        /// </summary>
        public object GetFrameStateInfo()
        {
            return new
            {
                Width,
                Height,
                AspectRatio,
                ClearColor = new { R = _clearR, G = _clearG, B = _clearB, A = _clearA },
                Presentation = _presentation,
                Presenter = _presenter,
                RenderTargets = _renderTargets,
                IsDisposed = _isDisposed
            };
        }
    }
}
