/*
// File: StaticLayoutRenderer.cs
// Purpose: Render static layout images using texture handles provided by StaticLayoutLoader.
// Notes:   This renderer draws only pre-defined static UI elements. No animation, no logic.
//          All visibility, ordering, and asset resolution is handled externally.
*/

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Drawing;
using SASZombieAssaultTD.Engine.Rendering;

namespace SASZombieAssaultTD.Engine.UI
{
    /*
    // Class: StaticLayoutRenderer
    // Purpose: Iterate through StaticLayout images and draw them using IDrawingContext.
    // Notes:   This class performs no state mutation. It only reads layout data and issues draw calls.
    */
    internal sealed class StaticLayoutRenderer
    {
        private readonly StaticLayout _layout;
        private readonly StaticLayoutLoader _loader;

        /*
        // Method: Constructor
        // Purpose: Initialize renderer with layout and loader references.
        // Parameters:
        //   layout - StaticLayout containing image definitions.
        //   loader - StaticLayoutLoader providing texture handles.
        */
        internal StaticLayoutRenderer(StaticLayout layout, StaticLayoutLoader loader)
        {
            _layout = layout ?? throw new ArgumentNullException(nameof(layout));
            _loader = loader ?? throw new ArgumentNullException(nameof(loader));
        }

        /*
        // Method: Render
        // Purpose: Draw all visible static layout images in the order they appear.
        // Parameters:
        //   context - Rendering context used to issue draw commands.
        */
        public void Render(IDrawingContext context)
        {
            System.Diagnostics.Debug.WriteLine($"[StaticLayoutRenderer] Render called, image count: {_layout.Images.Count}");

            foreach (var image in _layout.Images)
            {
                System.Diagnostics.Debug.WriteLine($"[StaticLayoutRenderer] Processing image: {image.Id}");
                // Visibility check
                if (!image.Visible)
                {
                    System.Diagnostics.Debug.WriteLine($"[StaticLayoutRenderer] Image {image.Id} skipped - not visible");
                    continue;
                }

                // Retrieve texture handle
                var handle = _loader.TryGetHandle(image.Id);
                if (handle == null)
                {
                    System.Diagnostics.Debug.WriteLine($"[StaticLayoutRenderer] Image {image.Id} skipped - handle is null");
                    continue;
                }

                // Validate handle state
                // TODO: RSHandle doesn't have IsLoaded/HasFailed properties
                // if (!handle.IsLoaded)
                // {
                //     System.Diagnostics.Debug.WriteLine($"[StaticLayoutRenderer] Image {image.Id} skipped - not loaded");
                //     continue;
                // }

                // if (handle.HasFailed)
                // {
                //     System.Diagnostics.Debug.WriteLine($"[StaticLayoutRenderer] Image {image.Id} skipped - load failed");
                //     continue;
                // }

                // Draw sprite
                System.Diagnostics.Debug.WriteLine($"[StaticLayoutRenderer] Drawing" +
                    $" {image.Id} at ({image.X},{image.Y}) size {image.Width}x{image.Height}");

                context.DrawSprite(
                    handle,
                    image.X,
                    image.Y,
                    image.Width,
                    image.Height,
                    Color.White // Static UI always draws full white tint
                );

                System.Diagnostics.Debug.WriteLine($"[StaticLayoutRenderer] DrawSprite completed for {image.Id}");
            }
        }
    }

    /* 
    // Class: StaticLayout
    // Purpose: Container for static UI image definitions.
    // Notes:   Ordering is preserved as inserted.
    */
    internal sealed class StaticLayout
    {
        internal List<StaticLayoutImage> Images { get; } = new();
    }

    /* 
    // Class: StaticLayoutImage
    // Purpose: Represents a single static UI image entry.
    // Notes:   No logic, no behavior. Pure data container.
    */
    internal sealed class StaticLayoutImage
    {
        internal string Id { get; set; }
        internal string Path { get; set; }
        internal int X { get; set; }
        internal int Y { get; set; }
        internal int Width { get; set; }
        internal int Height { get; set; }
        internal bool Visible { get; set; } = true;
    }
}
