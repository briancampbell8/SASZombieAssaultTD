//==========================================================================================
// FILE: AdapterOps_FeatureLevels.cs
// PATH: Engine/Render/Adapter/AdapterOps_FeatureLevels.cs
// SUBSYSTEM: Rendering / D3D11 Feature Level Negotiation
//
// ROLE:
// Provides deterministic negotiation and validation of supported D3D11 feature levels.
// Ensures the device creation process selects the highest stable feature level available.
// Supplies fallback logic and structured reporting for unsupported configurations.
//
// RESPONSIBILITIES:
//  - Query supported feature levels from the adapter.
//  - Determine the highest stable feature level available.
//  - Validate feature level compatibility with engine requirements.
//  - Provide fallback logic when preferred levels are unavailable.
//  - Expose negotiated feature level to device creation subsystems.
//
// NON-RESPONSIBILITIES:
//  - Creating the D3D11 device.
//  - Managing swap chains, pipelines, or rendering operations.
//  - Performing any GPU calls or context creation.
//==========================================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using Vortice.Direct3D;
using Vortice.Direct3D11;

namespace SASZombieAssaultTD.Engine.Render.D3D11.Adapter.AdapterOps
{
    /// <summary>
    /// Deterministic D3D11 feature level negotiation subsystem.
    /// </summary>
    public sealed class AdapterOps_FeatureLevels
    {
        private readonly D3D11RenderContext _context;

        // Ordered highest → lowest
        private static readonly FeatureLevel[] PreferredLevels =
        {
            FeatureLevel.Level_11_1,
            FeatureLevel.Level_11_0,
            FeatureLevel.Level_10_1,
            FeatureLevel.Level_10_0,
            FeatureLevel.Level_9_3,
            FeatureLevel.Level_9_2,
            FeatureLevel.Level_9_1
        };

        public AdapterOps_FeatureLevels(D3D11RenderContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Lightweight trace helper
        private void Trace(string msg)
        {
            System.Diagnostics.Debug.WriteLine("[FeatureLevels] " + msg);
        }

        private object D3D11CreateDevice(
            object value,
            DriverType driverType,
            DeviceCreationFlags flags,
            FeatureLevel[] featureLevels,
            out ID3D11Device device,
            out ID3D11DeviceContext context)
        {
            device = null;
            context = null;

            var adapter = value as Vortice.DXGI.IDXGIAdapter;

            Trace($"D3D11CreateDevice: Attempting. Adapter={(adapter != null ? "IDXGIAdapter" : "null")}, Levels=[{string.Join(",", featureLevels)}]");

            var result = Vortice.Direct3D11.D3D11.D3D11CreateDevice(
                adapter,
                driverType,
                flags,
                featureLevels,
                out device,
                out context);

            Trace($"D3D11CreateDevice: Result={result.Code}, Device={(device != null)}, Context={(context != null)}");

            return result;
        }

        // Retrieve all supported feature levels for the current adapter.
        public IReadOnlyList<FeatureLevel> GetSupportedFeatureLevels()
        {
            var supported = new List<FeatureLevel>();

            foreach (var level in PreferredLevels)
            {
                Trace($"Probing feature level {level}");

                var result = D3D11CreateDevice(
                    null,
                    DriverType.Hardware,
                    DeviceCreationFlags.None,
                    new[] { level },
                    out ID3D11Device device,
                    out ID3D11DeviceContext context);

                // FIXED: Your original code never added supported levels!
                if (device != null)
                {
                    supported.Add(level);
                    Trace($"Feature level {level} supported.");
                }
                else
                {
                    Trace($"Feature level {level} NOT supported.");
                }
            }

            Trace($"Supported feature levels: {string.Join(",", supported)}");

            return supported;
        }

        // Select the highest stable feature level available.
        public FeatureLevel NegotiateBestFeatureLevel()
        {
            var supported = GetSupportedFeatureLevels();

            if (supported.Count == 0)
                throw new InvalidOperationException("No supported D3D11 feature levels found on this adapter.");

            foreach (var level in PreferredLevels)
            {
                if (supported.Contains(level))
                {
                    Trace($"Negotiated best feature level: {level}");
                    return level;
                }
            }

            var fallback = supported[supported.Count - 1];
            Trace($"Negotiated fallback feature level: {fallback}");
            return fallback;
        }

        // Validate that the negotiated feature level meets engine requirements.
        public void ValidateFeatureLevel(FeatureLevel level)
        {
            Trace($"Validating feature level {level}");

            var supported = GetSupportedFeatureLevels();

            if (!supported.Contains(level))
                throw new NotSupportedException($"Feature level {level} is not supported by this adapter.");

            Trace($"Feature level {level} validated.");
        }

        // Provide fallback logic when preferred feature levels are unavailable.
        public FeatureLevel GetFallbackFeatureLevel()
        {
            var supported = GetSupportedFeatureLevels();

            if (supported.Count == 0)
                throw new InvalidOperationException("No fallback feature level available.");

            var fallback = supported[supported.Count - 1];
            Trace($"Fallback feature level selected: {fallback}");
            return fallback;
        }

        internal void ValidateFeatureLevel()
        {
            var best = NegotiateBestFeatureLevel();
            ValidateFeatureLevel(best);
        }
    }
}
