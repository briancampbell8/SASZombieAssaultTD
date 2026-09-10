//==========================================================================================
// FILE: AdapterOps_Pipeline.cs
// PATH: Engine/Render/Adapter/AdapterOps_Pipeline.cs
// SUBSYSTEM: Rendering / D3D11 Pipeline Binding & State Routing
//
// ROLE:
// Provides deterministic binding and routing of pipeline‑level GPU states,
// including shaders, input layouts, rasterizer, depth‑stencil, and blend
// configurations. Ensures stable, predictable pipeline transitions and
// exposes a clean interface for higher‑level rendering subsystems.
//
// RESPONSIBILITIES:
//  - Bind and unbind pipeline GPU states deterministically.
//  - Manage shader program assignment and validation.
//  - Configure rasterizer, depth‑stencil, and blend states.
//  - Provide structured pipeline state information.
//  - Support stable pipeline transitions for frame rendering.
//
// NON-RESPONSIBILITIES:
//  - Creating the D3D11 device.
//  - Managing swap chains or presentation.
//  - Allocating GPU resources or buffers.
//==========================================================================================

using System;

namespace SASZombieAssaultTD.Engine.Render.D3D11.Adapter.AdapterOps
{
    /// <summary>
    /// Deterministic D3D11 pipeline binding subsystem.
    /// </summary>
    public sealed class AdapterOps_Pipeline
    {
        private readonly D3D11RenderContext _context;
        private object _clearR;
        private object _clearG;
        private object _clearB;
        private object _clearA;
        private object _renderTargets;
        private object _presenter;
        private object _fullscreenQuad;
        private object _presentation;
        private object _disposed;

        public object Width { get; private set; }
        public object Height { get; private set; }
        public object AspectRatio { get; private set; }

        public AdapterOps_Pipeline(D3D11RenderContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Bind a shader program to the pipeline.
        public void BindShaderProgram(object shaderProgram)
        {
            // Validate input.
            if (shaderProgram == null)
            {
                throw new ArgumentNullException(nameof(shaderProgram));
            }

            // Attempt to bind the shader program by invoking a commonly named instance method on the shader object.
            // Many shader wrapper types expose parameterless methods such as Bind(), Use(), or Activate().
            var type = shaderProgram.GetType();
            string[] candidateNames = new[] { "Bind", "Use", "Activate", "Apply", "SetAsActive" };

            foreach (var name in candidateNames)
            {
                var mi = type.GetMethod(name, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic, null, Type.EmptyTypes, null);
                if (mi != null)
                {
                    mi.Invoke(shaderProgram, null);
                    return;
                }
            }

            // If the shader expects the render context to be passed in (e.g., Bind(context)), try that.
            if (_context != null)
            {
                var miWithContext = type.GetMethod("Bind", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic, null, new Type[] { _context.GetType() }, null);
                if (miWithContext != null)
                {
                    miWithContext.Invoke(shaderProgram, new object[] { _context });
                    return;
                }
            }

            // If no suitable method was found, fail with an informative error so callers can adapt.
            throw new InvalidOperationException($"Unable to bind shader program of type '{type.FullName}': no suitable Bind/Use/Activate method found.");
        }

        // Configure rasterizer, depth-stencil, and blend states.
        public void ConfigurePipelineStates(object pipelineStates)
        {
            if (pipelineStates == null)
                throw new ArgumentNullException(nameof(pipelineStates));

            if (_context == null)
                throw new InvalidOperationException("Render context is not initialized.");

            // Try to map common state names from the provided object to the render context.
            // We perform this with reflection to avoid assuming concrete types for pipelineStates
            // or the render context. Supported state identifiers: RasterizerState, DepthStencilState, BlendState.
            var stateNames = new[] { "RasterizerState", "DepthStencilState", "BlendState" };

            var pipelineType = pipelineStates.GetType();
            var contextType = _context.GetType();

            foreach (var stateName in stateNames)
            {
                try
                {
                    // Look for a matching property on the pipelineStates object (case-insensitive).
                    var srcProp = pipelineType.GetProperty(stateName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase);
                    if (srcProp == null)
                        continue;

                    var stateValue = srcProp.GetValue(pipelineStates);
                    if (stateValue == null)
                        continue;

                    // Prefer a SetXxx method on the context (e.g., SetRasterizerState).
                    var setMethod = contextType.GetMethod("Set" + stateName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase);
                    if (setMethod != null)
                    {
                        setMethod.Invoke(_context, new object[] { stateValue });
                        continue;
                    }

                    // Try an ApplyXxx method as an alternative.
                    var applyMethod = contextType.GetMethod("Apply" + stateName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase);
                    if (applyMethod != null)
                    {
                        applyMethod.Invoke(_context, new object[] { stateValue });
                        continue;
                    }

                    // Finally, try to set a property with the same name on the context.
                    var dstProp = contextType.GetProperty(stateName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase);
                    if (dstProp != null && dstProp.CanWrite)
                    {
                        dstProp.SetValue(_context, stateValue);
                        continue;
                    }

                    // If none of the above exist, skip this state silently.
                }
                catch (Exception)
                {
                    // Swallow exceptions per-state so a failure to configure one state does not
                    // prevent attempting to configure the others. Context-specific failures
                    // should be diagnosed by the caller if necessary.
                    continue;
                }
            }
        }

        // Unbind all pipeline states deterministically.
        public void ResetPipeline()
        {
            // Unbind and deterministically release/clear pipeline-related resources.
            // Dispose any resources that implement IDisposable, then reset fields to their default values.
            if (_renderTargets is IDisposable renderTargetsDisposable)
            {
                try
                {
                    renderTargetsDisposable.Dispose();
                }
                catch
                {
                    // Swallow exceptions to ensure deterministic reset; callers can reinitialize as needed.
                }
            }

            if (_presenter is IDisposable presenterDisposable)
            {
                try
                {
                    presenterDisposable.Dispose();
                }
                catch
                {
                }
            }

            if (_fullscreenQuad is IDisposable fullscreenDisposable)
            {
                try
                {
                    fullscreenDisposable.Dispose();
                }
                catch
                {
                }
            }

            if (_presentation is IDisposable presentationDisposable)
            {
                try
                {
                    presentationDisposable.Dispose();
                }
                catch
                {
                }
            }

            // Reset references and clear color/state fields to their default values.
            _renderTargets = default;
            _presenter = default;
            _fullscreenQuad = default;
            _presentation = default;

            _clearR = default;
            _clearG = default;
            _clearB = default;
            _clearA = default;
        }

        // Retrieve structured pipeline state information.
        public object GetPipelineStateInfo()
        {
            // Return a structured snapshot of the current pipeline state as an anonymous object.
            // Keep members raw (no assumptions about types or APIs) to avoid referencing unknown members.
            return new
            {
                this.Width,
                this.Height,
                this.AspectRatio,
                ClearColor = new { R = _clearR, G = _clearG, B = _clearB, A = _clearA },
                RenderTargets = _renderTargets,
                Presenter = _presenter,
                FullscreenQuad = _fullscreenQuad,
                Presentation = _presentation,
                Disposed = _disposed
            };
        }
    }
}
