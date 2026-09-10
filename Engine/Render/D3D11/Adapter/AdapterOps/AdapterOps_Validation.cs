//==========================================================================================
// FILE: AdapterOps_Validation.cs
// PATH: Engine/Render/Adapter/AdapterOps_Validation.cs
// SUBSYSTEM: Rendering / D3D11 Pipeline & Device Validation
//
// ROLE:
// Provides deterministic validation of D3D11 device, adapter, and pipeline
// configurations. Ensures that all rendering subsystems operate within stable,
// supported, and engine‑approved constraints. Supplies structured error reporting
// and corrective guidance to higher‑level systems.
//
// RESPONSIBILITIES:
//  - Validate D3D11 device correctness and stability.
//  - Validate adapter selection and feature level compatibility.
//  - Validate swap-chain configuration and output compatibility.
//  - Provide structured validation reports and error diagnostics.
//  - Support fallback logic for invalid or unsupported configurations.
//
// NON-RESPONSIBILITIES:
//  - Creating the D3D11 device.
//  - Performing any rendering or GPU operations.
//  - Managing pipelines, frame lifecycle, or swap-chain presentation.
//==========================================================================================

using System;
using SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore;

namespace SASZombieAssaultTD.Engine.Render.D3D11.Adapter.AdapterOps
{
    /// <summary>
    /// Deterministic D3D11 validation subsystem.
    /// </summary>
    public sealed class AdapterOps_Validation
    {
        private readonly D3D11RenderContext _context;
        private object _presentation;
        private object _presenter;
        private float _clearR;
        private float _clearG;
        private float _clearB;
        private float _clearA;
        private object _disposed;

        public object AspectRatio { get; private set; }
        public object Height { get; private set; }
        public object Width { get; private set; }

        public AdapterOps_Validation(D3D11RenderContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Validate the constructed D3D11 device.
        public bool ValidateDevice(D3D11DeviceCore deviceCore)
        {
            // Basic validation of the constructed D3D11 device core.
            if (deviceCore == null)
            {
                return false;
            }

            // Ensure the underlying D3D11 device and immediate context are present.
            if (deviceCore.Device == null || deviceCore.Context == null)
            {
                return false;
            }

            // Validate reported dimensions when available. Width/Height are expected to be positive.
            try
            {
                int w = deviceCore.Width;
                int h = deviceCore.Height;
                if (w <= 0 || h <= 0)
                {
                    return false;
                }
            }
            catch
            {
                // If properties are not accessible for some reason, fail validation conservatively.
                return false;
            }

            // Optionally ensure swap chain/backbuffer render target view exists when applicable.
            // If these properties do not exist or are null, treat as invalid configuration.
            try
            {
                var swap = deviceCore.SwapChain;
                var rtv = deviceCore.BackbufferRtv;
                if (swap == null || rtv == null)
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }

            // All quick checks passed.
            return true;
        }

        // Validate adapter selection and feature level compatibility.
        public bool ValidateAdapterSelection(int adapterIndex, object featureLevel)
        {
            // Basic parameter validation: adapter index must be non-negative.
            if (adapterIndex < 0)
            {
                return false;
            }

            // Ensure we have a render context to validate against.
            if (_context == null)
            {
                return false;
            }

            try
            {
                // If a feature level was provided, perform a conservative sanity check.
                // Accept enums and primitive/value types (common representations for feature levels).
                if (featureLevel != null)
                {
                    var ftType = featureLevel.GetType();
                    if (!(ftType.IsEnum || ftType.IsValueType))
                    {
                        // Unknown complex feature level representations are not considered valid here.
                        return false;
                    }
                }

                // Validate the swap chain / presentation configuration as part of adapter selection.
                // _presentation is a member available in this class and ValidateSwapChainConfig accepts an object.
                bool swapChainOk = ValidateSwapChainConfig(_presentation);

                // Additional device-specific validation would require a device core; defer to ValidateDevice when available.
                return swapChainOk;
            }
            catch
            {
                // Treat unexpected errors as validation failure; callers can request a fallback configuration.
                return false;
            }
        }

        // Validate swap-chain configuration for correctness and stability.
        public bool ValidateSwapChainConfig(object swapChainConfig)
        {
            // Basic null check
            if (swapChainConfig == null)
            {
                return false;
            }

            // Local helpers to extract numeric values from unknown objects
            static bool TryGetInt(object value, out int result)
            {
                result = 0;
                if (value == null) return false;
                try
                {
                    switch (value)
                    {
                        case int i:
                            result = i;
                            return true;
                        case short s:
                            result = s;
                            return true;
                        case long l when l >= int.MinValue && l <= int.MaxValue:
                            result = (int)l;
                            return true;
                        case uint ui when ui <= int.MaxValue:
                            result = (int)ui;
                            return true;
                        case string str when int.TryParse(str, out var parsed):
                            result = parsed;
                            return true;
                        case IConvertible conv:
                            result = Convert.ToInt32(conv);
                            return true;
                        default:
                            return false;
                    }
                }
                catch
                {
                    return false;
                }
            }

            static bool TryGetDouble(object value, out double result)
            {
                result = 0.0;
                if (value == null) return false;
                try
                {
                    switch (value)
                    {
                        case double d:
                            result = d;
                            return true;
                        case float f:
                            result = f;
                            return true;
                        case int i:
                            result = i;
                            return true;
                        case long l:
                            result = l;
                            return true;
                        case string s when double.TryParse(s, out var parsed):
                            result = parsed;
                            return true;
                        case IConvertible conv:
                            result = Convert.ToDouble(conv);
                            return true;
                        default:
                            return false;
                    }
                }
                catch
                {
                    return false;
                }
            }

            // Use reflection to read common swap-chain configuration properties if present
            var type = swapChainConfig.GetType();

            object widthObj = null;
            object heightObj = null;

            var widthProp = type.GetProperty("Width");
            if (widthProp != null)
            {
                widthObj = widthProp.GetValue(swapChainConfig);
            }
            else
            {
                // try common alternate name
                var wProp = type.GetProperty("BufferWidth") ?? type.GetProperty("ClientWidth");
                if (wProp != null) widthObj = wProp.GetValue(swapChainConfig);
            }

            var heightProp = type.GetProperty("Height");
            if (heightProp != null)
            {
                heightObj = heightProp.GetValue(swapChainConfig);
            }
            else
            {
                var hProp = type.GetProperty("BufferHeight") ?? type.GetProperty("ClientHeight");
                if (hProp != null) heightObj = hProp.GetValue(swapChainConfig);
            }

            // Validate width/height exist and are sensible
            if (!TryGetInt(widthObj, out var width) || !TryGetInt(heightObj, out var height))
            {
                return false;
            }

            if (width <= 0 || height <= 0) return false;

            // Arbitrary reasonable upper bound for swap chain dimensions to avoid invalid configs
            const int MaxDimension = 16384; // large but practical
            if (width > MaxDimension || height > MaxDimension) return false;

            // If this instance has Width/Height/AspectRatio fields set, try to ensure compatibility
            try
            {
                if (Width != null && TryGetInt(Width, out var ownerWidth))
                {
                    // allow mismatch but prefer same size
                    if (ownerWidth > 0 && System.Math.Abs(ownerWidth - width) > 2)
                    {
                        // small tolerance for off-by-one/2, otherwise consider invalid
                        return false;
                    }
                }

                if (Height != null && TryGetInt(Height, out var ownerHeight))
                {
                    if (ownerHeight > 0 && System.Math.Abs(ownerHeight - height) > 2)
                    {
                        return false;
                    }
                }

                if (AspectRatio != null && TryGetDouble(AspectRatio, out var ownerAspect))
                {
                    if (ownerAspect > 0.0)
                    {
                        var actualAspect = (double)width / System.Math.Max(1.0, height);
                        if (System.Math.Abs(actualAspect - ownerAspect) > 0.02) // small tolerance
                        {
                            return false;
                        }
                    }
                }
            }
            catch
            {
                // Any unexpected introspection error -> treat as invalid
                return false;
            }

            // Additional quick sanity checks: buffer count (if present) and format presence
            var bufferCountProp = type.GetProperty("BufferCount");
            if (bufferCountProp != null)
            {
                if (TryGetInt(bufferCountProp.GetValue(swapChainConfig), out var bufferCount))
                {
                    if (bufferCount < 1 || bufferCount > 16) return false;
                }
            }

            var formatProp = type.GetProperty("Format");
            if (formatProp != null)
            {
                var formatVal = formatProp.GetValue(swapChainConfig);
                // if format is a string or enum name, ensure it is not empty
                if (formatVal is string s && string.IsNullOrWhiteSpace(s)) return false;
            }

            // If all checks pass, consider the configuration valid
            return true;
        }

        // Produce a structured validation report.
        public object GenerateValidationReport()
        {
            // Construct a simple structured validation report using only known fields/properties
            var report = new System.Collections.Generic.Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            // Basic surface configuration
            report["Width"] = Width;
            report["Height"] = Height;
            report["AspectRatio"] = AspectRatio;

            // Presentation / presenter presence
            report["HasPresentation"] = _presentation != null;
            report["HasPresenter"] = _presenter != null;

            // Clear color components
            var clearColor = new System.Collections.Generic.Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                ["R"] = _clearR,
                ["G"] = _clearG,
                ["B"] = _clearB,
                ["A"] = _clearA
            };
            report["ClearColor"] = clearColor;

            // Disposal state
            report["Disposed"] = this._disposed;

            // Minimal context diagnostics (type name only to avoid touching unknown members)
            try
            {
                report["ContextType"] = _context?.GetType()?.FullName;
            }
            catch
            {
                report["ContextType"] = null;
            }

            // A short human-readable summary
            report["Summary"] = $"Presentation: {report["HasPresentation"]}, Presenter: {report["HasPresenter"]}, Disposed: {report["Disposed"]}";

            return report;
        }

        // Provide fallback logic for invalid configurations.
        public object GetFallbackConfiguration()
        {
            // Build a best-effort, descriptive fallback configuration as a dictionary.
            // This does not apply any changes to the context; it only returns a snapshot
            // that callers can examine or apply elsewhere.
            var fallback = new System.Collections.Generic.Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                ["AdapterIndex"] = 0,
                ["FeatureLevel"] = null,
                ["Width"] = this.Width,
                ["Height"] = this.Height,
                ["AspectRatio"] = this.AspectRatio,
                ["ClearColor"] = new float[] { this._clearR, this._clearG, this._clearB, this._clearA },
                ["ContextSummary"] = _context != null ? _context.ToString() : null,
                ["Presenter"] = this._presenter != null ? this._presenter.ToString() : null,
                ["Presentation"] = this._presentation != null ? this._presentation.ToString() : null
            };

            return fallback;
        }

        internal void ValidateAdapterSelection()
        {
            // Perform a best-effort validation using the available overloads and helpers.
            // Use a default adapter index (0) and no explicit feature level when none is provided.
            try
            {
                bool isValid = ValidateAdapterSelection(0, null);

                if (!isValid)
                {
                    // If the primary validation fails, attempt to obtain a fallback configuration
                    // and generate a validation report to aid diagnostics. We don't assume how
                    // the context should be modified here because that logic belongs elsewhere
                    // in the class; this method coordinates the validation flow.
                    var fallback = GetFallbackConfiguration();
                    var report = GenerateValidationReport();

                    // If further action is required (apply fallback or surface report), other
                    // members of this class should perform it. For now, surface a descriptive
                    // exception so callers are aware validation did not succeed.
                    throw new InvalidOperationException("Adapter selection validation failed. See validation report and fallback configuration for details.");
                }
            }
            catch (Exception ex) when (!(ex is InvalidOperationException))
            {
                // Wrap unexpected exceptions to provide clearer context for callers.
                throw new InvalidOperationException("An error occurred while validating adapter selection.", ex);
            }
        }
    }
}
