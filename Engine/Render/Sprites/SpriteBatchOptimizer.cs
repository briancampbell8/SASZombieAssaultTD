// ====================================================================================================
//  FILE: SpriteBatchOptimizer.cs
//  PATH: ./Engine/Render/Sprites/
//  MODULE: Render / Sprites
//
//  ROLE:
//      Provides deterministic batching and sorting of Sprite draw commands for the GPU pipeline.
//      Groups sprites by texture, sorts by layer depth, and produces optimized batches for rendering.
//
//  RESPONSIBILITIES:
//      - Accept Sprite commands from SpriteBatch.
//      - Group commands by Texture2D.
//      - Sort commands deterministically by layer depth.
//      - Produce optimized batches for GPU submission.
//      - Remain pure: no logging, no diagnostics, no resource management.
//
//  NON-RESPONSIBILITIES:
//      - Performing GPU draw calls (handled by SpriteBatch).
//      - Managing textures, atlases, or resource loading.
//      - Performing frustum culling or viewport management.
//      - Handling gameplay logic or UI layout.
//      - Emitting diagnostics or debug output.
//
//  NOTES:
//      - Pure batching subsystem for Option‑B deterministic rendering.
//      - Uses immutable Sprite structs.
//      - No legacy CPU-framebuffer abstractions.
// ====================================================================================================

using System.Collections.Generic;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.TextureRendering;

namespace SASZombieAssaultTD.Engine.Render.Sprites
{
    /// <summary>
    /// Deterministic batching optimizer for SpriteBatch. Groups and sorts Sprite commands for efficient GPU submission.
    /// </summary>
    internal sealed class SpriteBatchOptimizer
    {
        private readonly Dictionary<Texture2D, List<Sprite>> _groups;

        public SpriteBatchOptimizer() => _groups = new Dictionary<Texture2D, List<Sprite>>();

        /// <summary>
        /// Adds a sprite to the batching system.
        /// </summary>
        public void Add(Sprite sprite)
        {
            if (!_groups.TryGetValue(sprite.Texture, out var list))
            {
                list = new List<Sprite>();
                _groups[sprite.Texture] = list;
            }

            list.Add(sprite);
        }

        /// <summary>
        /// Produces optimized batches grouped by texture and sorted by layer depth.
        /// </summary>
        public IEnumerable<(Texture2D Texture, List<Sprite> Commands)> GetBatches()
        {
            foreach (var kvp in _groups)
            {
                var texture = kvp.Key;
                var commands = kvp.Value;

                // Sort by layer depth (front-to-back)
                commands.Sort((a, b) => a.LayerDepth.CompareTo(b.LayerDepth));

                yield return (texture, commands);
            }
        }

        /// <summary>
        /// Clears all batched commands.
        /// </summary>
        public void Clear()
        {
            _groups.Clear();
        }
    }
}
