/*
File:    StringExtensions.cs
Purpose:  Extension methods for string operations.
Features:  MeasureString method for UI text rendering.
*/

using System;
using System.Drawing;

namespace SASZombieAssaultTD.Engine.Extensions
{
    /// <summary>
    /// Extension methods for string operations.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Measures a string for rendering purposes.
        /// </summary>
        /// <param name="text">The text to measure.</param>
        /// <param name="font">The font to use for measurement.</param>
        /// <returns>Size of the rendered text.</returns>
        public static SizeF MeasureString(this string text, Font font)
        {
            if (string.IsNullOrEmpty(text) || font == null)
                return SizeF.Empty;

            // Create a graphics context for measurement
            using (var graphics = Graphics.FromImage(new Bitmap(1, 1)))
            {
                var fontSize = font.Size;
                var measuredSize = graphics.MeasureString(text, font);
                return new SizeF(measuredSize.Width, measuredSize.Height);
            }
        }

        /// <summary>
        /// Measures a string for rendering purposes (simplified version).
        /// </summary>
        /// <param name="text">The text to measure.</param>
        /// <returns>Size of the rendered text.</returns>
        public static SizeF MeasureString(this object text)
        {
            if (text == null)
                return SizeF.Empty;

            return MeasureString(text.ToString(), new Font("Arial", 12));
        }

        /// <summary>
        /// Gets the width of a string.
        /// </summary>
        /// <param name="text">The text to measure.</param>
        /// <returns>Width of the text.</returns>
        public static float Width(this string text)
        {
            return MeasureString(text).Width;
        }

        /// <summary>
        /// Gets the height of a string.
        /// </summary>
        /// <param name="text">The text to measure.</param>
        /// <returns>Height of the text.</returns>
        public static float Height(this string text)
        {
            return MeasureString(text).Height;
        }

        /// <summary>
        /// Gets the total cells count from a string configuration.
        /// </summary>
        /// <param name="config">The configuration string.</param>
        /// <returns>Total cells count.</returns>
        public static int TotalCells(this string config)
        {
            if (string.IsNullOrEmpty(config))
                return 0;

            // Simple parsing - assume format like "10x10" 
            var parts = config.Split('x');
            if (parts.Length == 2 && int.TryParse(parts[0], out var width) && int.TryParse(parts[1], out var height))
            {
                return width * height;
            }
            return 0;
        }

        /// <summary>
        /// Gets the grid width from a string configuration.
        /// </summary>
        /// <param name="config">The configuration string.</param>
        /// <returns>Grid width.</returns>
        public static int GridWidth(this string config)
        {
            if (string.IsNullOrEmpty(config))
                return 0;

            var parts = config.Split('x');
            if (parts.Length == 2 && int.TryParse(parts[0], out var width))
            {
                return width;
            }
            return 0;
        }

        /// <summary>
        /// Gets the grid height from a string configuration.
        /// </summary>
        /// <param name="config">The configuration string.</param>
        /// <returns>Grid height.</returns>
        public static int GridHeight(this string config)
        {
            if (string.IsNullOrEmpty(config))
                return 0;

            var parts = config.Split('x');
            if (parts.Length == 2 && int.TryParse(parts[1], out var height))
            {
                return height;
            }
            return 0;
        }

        /// <summary>
        /// Gets the occupied cells count from a string configuration.
        /// </summary>
        /// <param name="config">The configuration string.</param>
        /// <returns>Occupied cells count.</returns>
        public static int OccupiedCells(this string config)
        {
            // For now, return a reasonable default
            return TotalCells(config) / 2;
        }

        /// <summary>
        /// Gets the total entities count from a string configuration.
        /// </summary>
        /// <param name="config">The configuration string.</param>
        /// <returns>Total entities count.</returns>
        public static int TotalEntities(this string config)
        {
            // For now, return a reasonable default
            return TotalCells(config) / 4;
        }

        /// <summary>
        /// Gets the average entities per cell from a string configuration.
        /// </summary>
        /// <param name="config">The configuration configuration.</param>
        /// <returns>Average entities per cell.</returns>
        public static float AverageEntitiesPerCell(this string config)
        {
            var total = TotalCells(config);
            var entities = TotalEntities(config);
            return total > 0 ? (float)entities / total : 0f;
        }

        /// <summary>
        /// Gets the max entities per cell from a string configuration.
        /// </summary>
        /// <param name="config">The configuration string.</param>
        /// <returns>Max entities per cell.</returns>
        public static int MaxEntitiesPerCell(this string config)
        {
            // For now, return a reasonable default
            return 2;
        }
    }

    }
