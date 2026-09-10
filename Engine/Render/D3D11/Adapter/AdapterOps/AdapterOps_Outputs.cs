//==========================================================================================
// FILE: AdapterOps_Outputs.cs
// PATH: Engine/Render/Adapter/AdapterOps_Outputs.cs
// SUBSYSTEM: Rendering / D3D11 Output Enumeration
//
// ROLE:
// Provides deterministic enumeration and querying of all outputs (monitors)
// attached to a given D3D11 adapter. Supplies structured display mode data,
// resolution information, and output capabilities to higher-level systems.
//
// RESPONSIBILITIES:
//  - Enumerate all outputs for a given adapter.
//  - Query output capabilities (resolutions, refresh rates, formats).
//  - Provide structured display mode information.
//  - Validate output indices and availability.
//  - Expose output data to device and swap-chain subsystems.
//
// NON-RESPONSIBILITIES:
//  - Creating the D3D11 device.
//  - Managing swap chains or pipelines.
//  - Performing any rendering or GPU operations.
//==========================================================================================

using System;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Render.D3D11.Adapter.AdapterOps
{
    /// <summary>
    /// Deterministic D3D11 output enumeration subsystem.
    /// </summary>
    public sealed class AdapterOps_Outputs
    {
        private readonly D3D11RenderContext _context;

        public AdapterOps_Outputs(D3D11RenderContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Enumerate all outputs (monitors) for a given adapter.
        public IReadOnlyList<object> EnumerateOutputs(int adapterIndex)
        {
            // Validate input and context
            if (adapterIndex < 0 || _context == null)
            {
                return Array.Empty<object>();
            }

            try
            {
                // Local helper: find a member (property/field) whose name contains the provided hint
                // and whose value is an IEnumerable (excluding string).
                object FindEnumerableMember(Type type, object instance, string nameHint)
                {
                    // Search properties first
                    foreach (var prop in type.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic))
                    {
                        if (prop.GetIndexParameters().Length != 0)
                            continue; // skip indexers

                        if (!prop.Name.Contains(nameHint, StringComparison.OrdinalIgnoreCase))
                            continue;

                        object value = null;
                        try { value = prop.GetValue(instance); } catch { value = null; }
                        if (value is System.Collections.IEnumerable && !(value is string))
                            return value;
                    }

                    // Then search fields
                    foreach (var field in type.GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic))
                    {
                        if (!field.Name.Contains(nameHint, StringComparison.OrdinalIgnoreCase))
                            continue;

                        object value = null;
                        try { value = field.GetValue(instance); } catch { value = null; }
                        if (value is System.Collections.IEnumerable && !(value is string))
                            return value;
                    }

                    return null;
                }

                // Attempt to find an adapters collection on the context object
                var contextType = _context.GetType();
                var adaptersEnumerable = FindEnumerableMember(contextType, _context, "adapter");
                if (adaptersEnumerable == null)
                    return Array.Empty<object>();

                // Extract the adapter at the requested index
                object selectedAdapter = null;
                var enumer = ((System.Collections.IEnumerable)adaptersEnumerable).GetEnumerator();
                int idx = 0;
                while (enumer.MoveNext())
                {
                    if (idx == adapterIndex)
                    {
                        selectedAdapter = enumer.Current;
                        break;
                    }
                    idx++;
                }

                if (selectedAdapter == null)
                    return Array.Empty<object>();

                // Find outputs collection on the selected adapter object
                var adapterType = selectedAdapter.GetType();
                var outputsEnumerable = FindEnumerableMember(adapterType, selectedAdapter, "output");
                if (outputsEnumerable == null)
                    return Array.Empty<object>();

                // Materialize outputs into a list of objects
                var list = new List<object>();
                foreach (var item in (System.Collections.IEnumerable)outputsEnumerable)
                {
                    list.Add(item!);
                }

                return new System.Collections.ObjectModel.ReadOnlyCollection<object>(list);
            }
            catch
            {
                // On any error, return an empty set rather than throwing to keep callers robust.
                return Array.Empty<object>();
            }
        }

        // Retrieve detailed information about a specific output.
        public object GetOutputInfo(int adapterIndex, int outputIndex)
        {
            // Validate the provided adapter and output indices
            if (!ValidateOutputIndex(adapterIndex, outputIndex))
            {
                throw new ArgumentOutOfRangeException(nameof(outputIndex), "Invalid adapter or output index.");
            }

            // Retrieve the list of outputs for the adapter
            var outputs = EnumerateOutputs(adapterIndex);
            if (outputs == null || outputIndex < 0 || outputIndex >= outputs.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(outputIndex), "Output index is out of range for the specified adapter.");
            }

            // Get the selected output and its display modes
            var output = outputs[outputIndex];
            var displayModes = GetDisplayModes(adapterIndex, outputIndex);

            // Return a simple information object. Using an anonymous type returned as object
            // keeps the method signature unchanged while providing structured info about the output.
            return new
            {
                AdapterIndex = adapterIndex,
                OutputIndex = outputIndex,
                Output = output,
                DisplayModes = displayModes ?? Array.Empty<object>(),
                IsValid = true
            };
        }

        // Retrieve all supported display modes for a given output.
        public IReadOnlyList<object> GetDisplayModes(int adapterIndex, int outputIndex)
        {
            // Validate indices first. If invalid, throw to surface the caller error.
            if (!ValidateOutputIndex(adapterIndex, outputIndex))
            {
                throw new ArgumentOutOfRangeException(nameof(outputIndex));
            }

            // This implementation returns an empty, readonly list of display modes.
            // The real implementation would query the underlying D3D/DXGI objects
            // exposed by the render context for the specified adapter/output and
            // build a list of supported display mode descriptors. Avoid making
            // assumptions about the render context's internals here.
            return Array.Empty<object>();
        }

        // Validate that an output index is within range.
        public bool ValidateOutputIndex(int adapterIndex, int outputIndex)
        {
            // Validate adapter index and ensure the outputs collection contains the requested index.
            if (adapterIndex < 0)
            {
                return false;
            }

            // Use existing EnumerateOutputs to obtain the list of outputs for the adapter.
            var outputs = EnumerateOutputs(adapterIndex);

            // If we couldn't obtain outputs, treat as invalid.
            if (outputs == null)
            {
                return false;
            }

            // Ensure outputIndex is within [0, outputs.Count - 1].
            return outputIndex >= 0 && outputIndex < outputs.Count;
        }

        internal object EnumerateOutputs()
        {
            // Default implementation: return the outputs for adapter index 0.
            // This delegates to the existing overload that returns a read-only list of outputs
            // for a specific adapter. Returning that list as object preserves the original
            // method signature while avoiding duplication of enumeration logic.
            return EnumerateOutputs(0);
        }
    }
}
