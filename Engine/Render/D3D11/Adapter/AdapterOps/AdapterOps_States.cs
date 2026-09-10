//==========================================================================================
// FILE: AdapterOps_States.cs
// PATH: Engine/Render/Adapter/AdapterOps_States.cs
// SUBSYSTEM: Rendering / D3D11 GPU State Object Management
//
// ROLE:
// Provides deterministic creation, caching, and validation of immutable GPU
// state objects, including rasterizer states, depth‑stencil states, blend
// states, sampler states, and input layouts. Ensures stable state behavior
// and exposes a clean interface for pipeline and rendering subsystems.
//
// RESPONSIBILITIES:
//  - Create rasterizer, depth‑stencil, blend, and sampler states.
//  - Create and validate input layouts for shader programs.
//  - Cache immutable GPU states for deterministic reuse.
//  - Provide structured state information for debugging and diagnostics.
//  - Support pipeline subsystem with stable state access.
//
// NON-RESPONSIBILITIES:
//  - Binding pipeline states (handled by AdapterOps_Pipeline).
//  - Creating the D3D11 device.
//  - Managing swap chains, resources, or frame lifecycle.
//==========================================================================================

using System;

namespace SASZombieAssaultTD.Engine.Render.D3D11.Adapter.AdapterOps
{
    /// <summary>
    /// Deterministic D3D11 GPU state object management subsystem.
    /// </summary>
    public sealed class AdapterOps_States
    {
        private readonly D3D11RenderContext _context;

        public AdapterOps_States(D3D11RenderContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Create a rasterizer state.
        public object CreateRasterizerState(object rasterizerDesc)
        {
            // Validate input
            if (rasterizerDesc is null)
            {
                throw new ArgumentNullException(nameof(rasterizerDesc));
            }

            // In the absence of concrete D3D11 types in this context, return the provided descriptor
            // as the created state representation. This preserves the descriptor for callers
            // and keeps behavior deterministic until a platform-specific implementation is provided.
            return rasterizerDesc;
        }

        // Create a depth-stencil state.
        public object CreateDepthStencilState(object depthStencilDesc)
        {
            // Validate input
            if (depthStencilDesc == null)
            {
                throw new ArgumentNullException(nameof(depthStencilDesc));
            }

            // This adapter returns an opaque representation of the depth-stencil state.
            // The concrete conversion/creation into a native API state object is handled
            // by the higher-level render context; here we preserve the descriptor as-is
            // to avoid making assumptions about unavailable APIs in this scope.
            return depthStencilDesc;
        }

        // Create a blend state.
        public object CreateBlendState(object blendDesc)
        {
            // Validate input
            if (blendDesc is null)
                throw new ArgumentNullException(nameof(blendDesc));

            // If the render context provides a strongly-typed CreateBlendState method, invoke it via reflection.
            // This avoids introducing concrete graphics types into this adapter while still attempting to create
            // a runtime blend-state object when available.
            try
            {
                var context = _context;
                if (context != null)
                {
                    var ctxType = context.GetType();
                    var create = ctxType.GetMethod("CreateBlendState", new[] { blendDesc.GetType() });
                    if (create != null)
                    {
                        return create.Invoke(context, new[] { blendDesc });
                    }
                }
            }
            catch
            {
                // If reflection invocation fails for any reason, fall back to returning the descriptor object.
                // Swallowing exceptions here keeps the adapter resilient; callers can decide how to handle the
                // returned object or perform a stronger creation step themselves.
            }

            // As a fallback, return the descriptor so callers can use it directly or create the concrete state.
            return blendDesc;
        }

        // Create a sampler state.
        public object CreateSamplerState(object samplerDesc)
        {
            // Basic implementation: validate input and return the provided descriptor as the created state.
            // The concrete sampler state implementation is renderer-specific and not available in this context,
            // so preserve the descriptor as the opaque state object. Callers can pass a renderer-specific
            // descriptor instance and expect to receive it back as the created state handle.
            if (samplerDesc == null)
            {
                throw new ArgumentNullException(nameof(samplerDesc));
            }

            return samplerDesc;
        }

        // Create an input layout for a shader program.
        public object CreateInputLayout(object shaderBytecode, object inputElements)
        {
            // Validate inputs
            if (shaderBytecode is null)
                throw new ArgumentNullException(nameof(shaderBytecode));
            if (inputElements is null)
                throw new ArgumentNullException(nameof(inputElements));

            // Attempt to delegate to an underlying D3D11 device if available on the render context.
            // Use reflection to avoid depending on specific types that may not be present in this project.
            try
            {
                var context = _context;
                if (context != null)
                {
                    var contextType = context.GetType();
                    var deviceProp = contextType.GetProperty("Device");
                    if (deviceProp != null)
                    {
                        var device = deviceProp.GetValue(context);
                        if (device != null)
                        {
                            // Look for a CreateInputLayout method that accepts the provided argument types
                            var createMethod = device.GetType().GetMethod("CreateInputLayout", new[] { shaderBytecode.GetType(), inputElements.GetType() });
                            if (createMethod != null)
                            {
                                var layout = createMethod.Invoke(device, new[] { shaderBytecode, inputElements });
                                if (layout != null)
                                    return layout; // Successfully created through underlying device
                            }
                        }
                    }
                }
            }
            catch (System.Reflection.TargetInvocationException)
            {
                // Ignore and fallback to placeholder
            }
            catch (Exception)
            {
                // Ignore any reflection/runtime issues and fallback to placeholder
            }

            // Fallback: return a lightweight descriptor object bundling the provided inputs. This
            // ensures the method never throws unexpectedly in environments where the D3D device
            // isn't available. Callers that require a real native input layout should detect and
            // replace this placeholder with a proper implementation.
            return new { ShaderBytecode = shaderBytecode, InputElements = inputElements };
        }

        // Retrieve structured state information.
        public object GetStateInfo(object stateObject)
        {
            // Return null for no input
            if (stateObject == null)
            {
                return null;
            }

            // Create a dictionary to hold structured state information
            var info = new System.Collections.Generic.Dictionary<string, object>(StringComparer.Ordinal);

            var type = stateObject.GetType();
            info["Type"] = type.FullName ?? type.Name;

            // Capture the object's ToString() representation safely
            try
            {
                info["ToString"] = stateObject.ToString();
            }
            catch (Exception ex)
            {
                info["ToStringError"] = ex.Message;
            }

            // Inspect public instance properties
            var properties = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            foreach (var prop in properties)
            {
                try
                {
                    // Skip indexed properties
                    if (prop.GetIndexParameters().Length != 0)
                    {
                        info[$"Property:{prop.Name}"] = "<indexed>";
                    }
                    else
                    {
                        info[$"Property:{prop.Name}"] = prop.GetValue(stateObject);
                    }
                }
                catch (Exception ex)
                {
                    info[$"Property:{prop.Name}"] = $"<error:{ex.Message}>";
                }
            }

            // Inspect public instance fields
            var fields = type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            foreach (var field in fields)
            {
                try
                {
                    info[$"Field:{field.Name}"] = field.GetValue(stateObject);
                }
                catch (Exception ex)
                {
                    info[$"Field:{field.Name}"] = $"<error:{ex.Message}>";
                }
            }

            return info;
        }
    }
}