//============================================================================
// File Path: Engine/UI/StaticLayoutLoader.cs
// File: StaticLayoutLoader.cs
// Program: StaticLayoutLoader
// Subsystem: UI / Legacy Static Layout Loading
//
// Purpose:
//     Loads StaticLayout definitions from JSON and resolves all referenced
//     image assets through AssetManager. Provides deterministic handle
//     tracking, lifecycle‑safe disposal, and a stable bridge between static
//     UI layout data and runtime rendering.
//
// Architectural Role:
//     - Converts JSON → StaticLayout model
//     - Resolves image paths → RSHandle instances via AssetManager
//     - Maintains a handle registry keyed by layout image IDs
//     - Supplies StaticLayoutRenderer with resolved texture handles
//     - Performs no rendering, no mutation of layout data
//
// Diagnostics:
//     - Emits deterministic, grep‑friendly trace messages for JSON load,
//       deserialization, and asset resolution
//     - System.Diagnostics.Debug.WriteLine permitted ONLY for forensic output
//     - No silent failures; missing images or invalid JSON are surfaced
//
// Integration Points:
//     - AssetManager.LoadAsync<T>() for handle‑based asset resolution
//     - RSHandle (non‑generic) for resolved RS assets
//     - StaticLayout (data model) for static UI definitions
//     - StaticLayoutRenderer for runtime draw submission
//
// Notes:
//     - Loader does NOT decode textures; it only resolves handles
//     - JSON schema must match StaticLayout model
//     - All loaded handles are released on Dispose()
//     - This class is part of the legacy UI/Rendering subsystem and is
//       scheduled for migration into the unified Engine.Rendering pipeline
//============================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using SASZombieAssaultTD.Engine.Resources;

namespace SASZombieAssaultTD.Engine.UI
{
    ///<summary>
    ///Loads StaticLayout definitions and resolves all referenced image assets.
    ///Maintains handle registry for use by StaticLayoutRenderer.
    ///</summary>
    public class StaticLayoutLoader : IDisposable
    {
        //--------------------------------------------------------------------
        // INTERNAL STATE
        //--------------------------------------------------------------------

        private readonly AssetManager _assetManager;

        ///<summary>
        ///Handle registry keyed by image ID from the StaticLayout JSON.
        ///</summary>
        private readonly Dictionary<string, RSHandle> _handles;

        //--------------------------------------------------------------------
        // CONSTRUCTION
        //--------------------------------------------------------------------

        public StaticLayoutLoader(AssetManager assetManager)
        {
            _assetManager = assetManager;
            _handles = new Dictionary<string, RSHandle>();
        }

        //--------------------------------------------------------------------
        // LAYOUT LOADING
        //--------------------------------------------------------------------

        public async Task<object> LoadAsync(string path)
        {
            //Load JSON text
            var json = await File.ReadAllTextAsync(path);
            System.Diagnostics.Debug.WriteLine(
                $"[StaticLayoutLoader] JSON loaded: {json.Length} chars");

            //Deserialize layout
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var layout = JsonSerializer.Deserialize<StaticLayout>(json, options);

            System.Diagnostics.Debug.WriteLine(
                $"[StaticLayoutLoader] Deserialized Images count: {layout?.Images?.Count ?? -1}");

            //Resolve all image references
            if (layout?.Images != null)
            {
                foreach (var image in layout.Images)
                {
                    if (!_handles.ContainsKey(image.Id))
                    {
                        //Load asset via AssetManager (handle‑based)
                        RSHandle handle = await _assetManager.LoadAsync<object>(
                            image.Path,
                            AssetPriority.Normal);

                        _handles[image.Id] = handle;

                        //RSHandle has no AddReference() — correct for RS subsystem
                    }
                }
            }

            return layout ?? new StaticLayout();
        }

        //--------------------------------------------------------------------
        // HANDLE RETRIEVAL
        //--------------------------------------------------------------------

        public RSHandle? TryGetHandle(string id)
        {
            _handles.TryGetValue(id, out var handle);
            return handle;
        }

        //--------------------------------------------------------------------
        // DISPOSAL
        //--------------------------------------------------------------------

        public void Dispose()
        {
            foreach (var kvp in _handles)
            {
                _assetManager?.Release(kvp.Value);
            }

            _handles.Clear();
        }
    }
}
