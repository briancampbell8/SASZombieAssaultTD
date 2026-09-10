//==========================================================================================
//  FILE: AdapterOps_Enumerate.cs
//  PATH: Engine/Render/Adapter/AdapterOps_Enumerate.cs
//  SUBSYSTEM: Rendering / GPU Adapter Enumeration
//
//  ROLE:
//  Provides deterministic enumeration of all available D3D11 adapters,
//  including vendor, device, memory, and output capabilities. Supplies
//  the manager with a clean, structured list of GPU candidates.
//
//  RESPONSIBILITIES:
//  - Enumerate all physical and virtual D3D11 adapters.
//  - Query adapter properties (name, vendor, memory, feature support).
//  - Enumerate adapter outputs (monitors, display modes).
//  - Provide deterministic, stable ordering of adapters.
//  - Expose enumeration results to higher-level systems.
//
//  NON-RESPONSIBILITIES:
//  - Creating or selecting the active device.
//  - Performing any GPU calls or context creation.
//  - Managing swap chains, pipelines, or rendering operations.
//==========================================================================================

using System;
using System.Collections.Generic;
using System.Drawing;
using SASZombieAssaultTD.Engine.Render.D3D11;
using Vortice.DXGI;

namespace SASZombieAssaultTD.Engine.Render.D3D11.Adapter.AdapterOps
{
    /// <summary>
    /// Deterministic GPU adapter enumeration subsystem.
    /// </summary>
    public sealed class AdapterOps_Enumerate
    {
        private readonly D3D11RenderContext _context;

        public AdapterOps_Enumerate(D3D11RenderContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Enumerate all adapters available on the system.
        public IReadOnlyList<IDXGIAdapter1> EnumerateAdapters()
        {
            var factory = _context.Factory as IDXGIFactory1;
            if (factory == null)
                return Array.Empty<IDXGIAdapter1>();

            var list = new List<IDXGIAdapter1>();

            for (uint i = 0; ; i++)
            {
                factory.EnumAdapters1(i, out IDXGIAdapter1 adapter);
                if (adapter == null)
                    break;

                list.Add(adapter);
            }

            return list;
        }

        // Retrieve detailed information about a specific adapter.
        // Retrieve detailed information about a specific adapter.
        public AdapterInfo GetAdapterInfo(int index)
        {
            var adapters = EnumerateAdapters();
            if (index < 0 || index >= adapters.Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            var adapter = adapters[index];
            var desc = adapter.Description1;

            return new AdapterInfo
            {
                Index = index,
                Description = desc.Description,
                VendorId = desc.VendorId,
                DeviceId = desc.DeviceId,
                SubSysId = desc.SubsystemId,
                Revision = desc.Revision,
                DedicatedVideoMemory = (ulong)desc.DedicatedVideoMemory.Value,
                DedicatedSystemMemory = (ulong)desc.DedicatedSystemMemory.Value,
                SharedSystemMemory = (ulong)desc.SharedSystemMemory.Value
            };
        }

        // Enumerate all outputs (monitors) for a given adapter.
        public IReadOnlyList<IDXGIOutput> EnumerateOutputs(int adapterIndex)
        {
            var adapters = EnumerateAdapters();
            if (adapterIndex < 0 || adapterIndex >= adapters.Count)
                throw new ArgumentOutOfRangeException(nameof(adapterIndex));

            var adapter = adapters[adapterIndex];
            var list = new List<IDXGIOutput>();

            for (uint i = 0; ; i++)
            {
                adapter.EnumOutputs(i, out IDXGIOutput output);
                if (output == null)
                    break;

                list.Add(output);
            }

            return list;
        }

        // Retrieve detailed information about a specific output.
        public OutputInfo GetOutputInfo(int adapterIndex, int outputIndex)
        {
            var outputs = EnumerateOutputs(adapterIndex);
            if (outputIndex < 0 || outputIndex >= outputs.Count)
                throw new ArgumentOutOfRangeException(nameof(outputIndex));

            var output = outputs[outputIndex];
            var desc = output.Description;

            return new OutputInfo
            {
                AdapterIndex = adapterIndex,
                OutputIndex = outputIndex,
                DeviceName = desc.DeviceName,
                DesktopCoordinates = desc.DesktopCoordinates,
                AttachedToDesktop = desc.AttachedToDesktop,
                Rotation = desc.Rotation
            };
        }
    }

    // Structured adapter info
    public sealed class AdapterInfo
    {
        public int Index { get; set; }
        public string Description { get; set; }
        public uint VendorId { get; set; }
        public uint DeviceId { get; set; }
        public uint SubSysId { get; set; }
        public uint Revision { get; set; }
        public ulong DedicatedVideoMemory { get; set; }
        public ulong DedicatedSystemMemory { get; set; }
        public ulong SharedSystemMemory { get; set; }
    }

    // Structured output info
    public sealed class OutputInfo
    {
        public int AdapterIndex { get; set; }
        public int OutputIndex { get; set; }
        public string DeviceName { get; set; }
        public Rectangle DesktopCoordinates { get; set; }
        public bool AttachedToDesktop { get; set; }
        public ModeRotation Rotation { get; set; }
    }
}
