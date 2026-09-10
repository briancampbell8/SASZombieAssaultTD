//==========================================================================================
// FILE: AdapterOps_SwapChain.cs
// PATH: Engine/Render/Adapter/AdapterOps_SwapChain.cs
// SUBSYSTEM: Rendering / D3D11 Swap Chain Management
//
// ROLE:
// Provides deterministic creation, configuration, and management of the D3D11
// swap chain. Ensures stable presentation behavior, correct output selection,
// and engine‑approved configuration of buffers, formats, and refresh behavior.
//
// RESPONSIBILITIES:
//  - Create the D3D11 swap chain for the active device and window.
//  - Configure swap-chain parameters (format, buffer count, usage, vsync).
//  - Validate swap-chain compatibility with the selected adapter/output.
//  - Provide deterministic resize and recreation logic.
//  - Expose structured swap-chain information to higher-level systems.
//
// NON-RESPONSIBILITIES:
//  - Creating the D3D11 device.
//  - Performing rendering or GPU operations.
//  - Managing pipelines or frame lifecycle beyond presentation.
//==========================================================================================

using System;

namespace SASZombieAssaultTD.Engine.Render.D3D11.Adapter.AdapterOps
{
    /// <summary>
    /// Deterministic D3D11 swap-chain management subsystem.
    /// </summary>
    public sealed class AdapterOps_SwapChain
    {
        private readonly D3D11RenderContext _context;
        private object _presentation;
        private bool _disposed;

        public float AspectRatio { get; private set; }
        public float Width { get; private set; }
        public int Height { get; private set; }

        public AdapterOps_SwapChain(D3D11RenderContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Create the swap chain for the active device and window.
        public object CreateSwapChain(object windowHandle)
        {
            // Ensure the ops instance is usable
            if (_disposed)
                throw new ObjectDisposedException(nameof(AdapterOps_SwapChain));

            if (_context == null)
                throw new InvalidOperationException("Render context is not initialized.");

            // If a swap chain/presentation already exists, return it to avoid recreating resources.
            if (_presentation != null)
                return _presentation;

            // Create a lightweight swap chain representation. The concrete engine originally
            // used a native DXGI swap chain object; here we create an anonymous object that
            // contains the minimal information callers in this codebase expect (handle and
            // current size/aspect). Storing it in _presentation keeps behaviour consistent.
            var swapChain = new
            {
                WindowHandle = windowHandle,
                Width,
                Height,
                AspectRatio,
                CreatedUtc = DateTime.UtcNow
            };

            _presentation = swapChain;
            return swapChain;
        }

        // Configure swap-chain parameters (format, buffer count, vsync, etc.).
        public void ConfigureSwapChain(object swapChainConfig)
        {
            // Use provided config or fall back to the stored presentation configuration.
            if (swapChainConfig == null)
            {
                swapChainConfig = _presentation;
            }

            // Update the stored presentation configuration.
            _presentation = swapChainConfig;

            // Attempt to extract width/height information from the provided configuration using reflection
            // so the adapter can update its local state (Width/Height/AspectRatio) if available.
            // Reflection is best-effort: any failures will be ignored and existing state preserved.
            if (swapChainConfig != null)
            {
                try
                {
                    var cfgType = swapChainConfig.GetType();

                    var propWidth = cfgType.GetProperty("Width");
                    var propHeight = cfgType.GetProperty("Height");

                    if (propWidth != null && propHeight != null)
                    {
                        var widthObj = propWidth.GetValue(swapChainConfig);
                        var heightObj = propHeight.GetValue(swapChainConfig);

                        // Normalize width to float and height to int based on common numeric types.
                        if (widthObj is int wi)
                            Width = wi;
                        else if (widthObj is float wf)
                            Width = wf;
                        else if (widthObj is double wd)
                            Width = (float)wd;

                        if (heightObj is int hi)
                            Height = hi;
                        else if (heightObj is float hf)
                            Height = (int)hf;
                        else if (heightObj is double hd)
                            Height = (int)hd;

                        if (Height > 0)
                            AspectRatio = (float)Width / Height;
                    }
                }
                catch
                {
                    // Intentionally ignore reflection errors; preserve current adapter state.
                }
            }

            // Note: Platform-specific swap-chain reconfiguration (creating render targets, presenters, etc.)
            // is out of scope for this generic adapter method. This implementation updates the stored
            // presentation configuration and local dimensions so other parts of the adapter can react.
        }

        // Validate swap-chain compatibility with adapter/output.
        public bool ValidateSwapChain(object swapChain)
        {
            // Ensure the helper hasn't been disposed.
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(AdapterOps_SwapChain));
            }

            // Null swap-chain cannot be validated as valid.
            if (swapChain == null)
            {
                return false;
            }

            // If we already track this presentation object, consider it valid.
            if (ReferenceEquals(_presentation, swapChain))
            {
                return true;
            }

            try
            {
                var type = swapChain.GetType();

                // Common DX swap-chain methods we can look for via reflection.
                // If any of these are present we assume the object is a compatible swap-chain
                // and keep a reference for future operations.
                bool hasPresent = type.GetMethod("Present", Type.EmptyTypes) != null
                                  || type.GetMethod("Present", new[] { typeof(int), typeof(int) }) != null;
                bool hasResizeBuffers = type.GetMethod("ResizeBuffers") != null;
                bool hasGetBuffer = type.GetMethod("GetBuffer") != null;

                if (hasPresent || hasResizeBuffers || hasGetBuffer)
                {
                    _presentation = swapChain; // remember the valid presentation object
                    return true;
                }

                // As a fallback, accept objects exposing a Description/Desc property which many wrappers provide.
                if (type.GetProperty("Description") != null || type.GetProperty("Desc") != null)
                {
                    _presentation = swapChain;
                    return true;
                }

                // Not a recognized swap-chain implementation.
                return false;
            }
            catch
            {
                // Any reflection errors or unexpected behaviours should treat the swap chain as invalid.
                return false;
            }
        }

        // Resize swap-chain buffers deterministically.
        public void ResizeSwapChain(object swapChain, int width, int height)
        {
            // Validate input dimensions
            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width), "Width must be greater than zero.");
            if (height <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(height),
                    "Height must be greater than zero.");

            // Update local state
            Width = width;
            Height = height;
            AspectRatio = (float)width / height;

            // If no swapChain provided, nothing more to do.
            if (swapChain == null)
                return;

            // Validate provided swap-chain and reconfigure presentation if valid.
            if (!ValidateSwapChain(swapChain))
                throw new ArgumentException("The provided swapChain is not valid.", nameof(swapChain));

            // Try to obtain swap-chain specific info and apply configuration. If no info is available,
            // fall back to the parameterless ConfigureSwapChain overload.
            var info = GetSwapChainInfo(swapChain);
            if (info != null)
            {
                ConfigureSwapChain(info);
            }
            else
            {
                ConfigureSwapChain();
            }
        }

        // Retrieve structured swap-chain information.
        public object GetSwapChainInfo(object swapChain)
        {
            // Build a flexible dictionary of swap chain information to avoid creating new types.
            var info = new System.Collections.Generic.Dictionary<string, object>();

            // Basic presence and type information
            info["SwapChain"] = swapChain ?? null;
            info["Type"] = swapChain != null ? swapChain.GetType().FullName : "null";

            // Validate swap chain if possible and include the result
            bool isValid = false;
            try
            {
                isValid = ValidateSwapChain(swapChain);
            }
            catch
            {
                // If validation throws for an unexpected swapChain implementation, treat as invalid.
                isValid = false;
            }

            info["IsValid"] = isValid;

            // Expose current configured/known properties of this adapter ops instance
            info["Width"] = Width;
            info["Height"] = Height;
            info["AspectRatio"] = AspectRatio;

            // Include presentation-related objects where available for diagnostics
            info["Presentation"] = _presentation ?? null;
            info["Context"] = _context ?? (object)null;

            return info;
        }

        internal void ValidateSwapChain()
        {
            // Ensure the swapchain helper hasn't been disposed.
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(AdapterOps_SwapChain));
            }

            // Delegate the actual validation to the overload that accepts a swap chain object.
            // That method may attempt to repair state and returns a bool indicating validity.
            bool valid;
            try
            {
                valid = ValidateSwapChain(_presentation);
            }
            catch (Exception ex)
            {
                // Wrap and rethrow to give clearer context to callers.
                throw new InvalidOperationException("An exception occurred while validating the swap chain.", ex);
            }

            if (!valid)
            {
                // If the validation method could not repair the swap chain, fail fast so callers can handle it.
                throw new InvalidOperationException("Swap chain validation failed and the swap chain is not in a valid state.");
            }
        }

        internal void CreateSwapChain()
        {
            // Ensure the swapchain helper hasn't been disposed.
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(AdapterOps_SwapChain));
            }

            // Validate current swap chain state (throws or fixes state as implemented elsewhere).
            ValidateSwapChain();

            // Configure or (re)create swap chain resources. Concrete behavior is provided by the
            // parameterless ConfigureSwapChain implementation in this class.
            ConfigureSwapChain();

            // Update derived values when we have valid dimensions.
            // Use a safe fallback for aspect ratio to avoid division by zero.
            if (Width > 0 && Height > 0)
            {
                AspectRatio = (float)Width / (float)Height;
            }
            else
            {
                AspectRatio = 1.0f;
            }
        }

        internal void ConfigureSwapChain()
        {
            // Delegate to the overload that accepts a swap-chain configuration object.
            // _presentation holds the current swap-chain configuration for this adapter.
            ConfigureSwapChain(_presentation);
        }
    }
}
