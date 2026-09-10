// ====================================================================================================
//  FILE: SpriteBatchRenderer.cs
//  PATH: ./Engine/Render/Sprites/
//  MODULE: Render / Sprites
//
//  ROLE:
//      Executes optimized Sprite batches produced by SpriteBatchOptimizer.
//      Submits grouped and sorted Sprite commands to the GPU using the RenderDevice.
//
//  RESPONSIBILITIES:
//      - Accept optimized batches of Sprite commands.
//      - Bind textures and pipeline state.
//      - Issue GPU draw calls for each batch.
//      - Remain deterministic and side‑effect free.
//
//  NON-RESPONSIBILITIES:
//      - Sorting or grouping Sprite commands (handled by SpriteBatchOptimizer).
//      - Managing textures or resource loading.
//      - Performing gameplay logic or UI layout.
//      - Logging or diagnostics.
//
//  NOTES:
//      - Pure GPU submission layer.
//      - Option‑B deterministic rendering compliant.
//      - Uses immutable Sprite structs.
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Drawing;
using SASZombieAssaultTD.Engine.TextureRendering;

namespace SASZombieAssaultTD.Engine.Render.Sprites
{
    /// <summary>
    /// Submits optimized Sprite batches to the GPU.
    /// </summary>
    internal sealed class SpriteBatchRenderer
    {
        private readonly RenderDevice _device;

        public SpriteBatchRenderer(RenderDevice device) => _device = device;

        /// <summary>
        /// Renders all sprite batches produced by the optimizer.
        /// </summary>
        public void RenderBatches(IEnumerable<(Texture2D Texture, List<Sprite> Commands)> batches)
        {
            foreach (var batch in batches)
            {
                var texture = batch.Texture;
                var commands = batch.Commands;

                if (commands.Count == 0)
                    continue;

                // Bind texture
                _device.BindTexture(texture);

                // Submit each sprite in the batch
                foreach (var sprite in commands)
                {
                    SubmitSprite(sprite);
                }
            }
        }

        internal void Draw(Texture2D pixel, Rectangle dest, Color color)
        {
            // Validate input
            if (pixel is null)
            {
                throw new ArgumentNullException(nameof(pixel));
            }

            // Create a sprite instance and submit it for batching.
            // The Sprite type lives in the Render.Sprites namespace; construct it using the texture, destination rectangle and tint color.
            var sprite = new Sprite(pixel, dest, color);
            SubmitSprite(sprite);
        }

        /// <summary>
        /// Submits a single sprite draw command to the GPU.
        /// </summary>
        private void SubmitSprite(Sprite sprite)
        {
            // Convert RectangleF to engine Rectangle
            var dest = new Rectangle(
                (int)sprite.Destination.X,
                (int)sprite.Destination.Y,
                (int)sprite.Destination.Width,
                (int)sprite.Destination.Height
            );

            Rectangle? src = null;
            if (sprite.Source.HasValue)
            {
                var s = sprite.Source.Value;
                src = new Rectangle(
                    (int)s.X,
                    (int)s.Y,
                    (int)s.Width,
                    (int)s.Height);
            }

            _device.DrawSprite(
                sprite.Texture,
                dest,
                src,
                sprite.Color,
                sprite.Rotation,
                sprite.Origin,
                sprite.LayerDepth);
        }
    }
}
