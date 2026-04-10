namespace SASZombieAssaultTD.Engine.UI.Rendering
{
    /// <summary>
    /// Texture wrapper for UI image rendering.
    /// Provides texture information for image drawing operations.
    /// </summary>
    public class Texture
    {
        /// <summary>
        /// Gets the texture width.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Gets the texture height.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Gets the texture identifier or path.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Creates a texture with specified dimensions.
        /// </summary>
        /// <param name="id">Texture identifier</param>
        /// <param name="width">Texture width</param>
        /// <param name="height">Texture height</param>
        public Texture(string id, int width, int height)
        {
            Id = id;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Creates a texture from an image file.
        /// </summary>
        /// <param name="filePath">Path to the image file</param>
        public Texture(string filePath)
        {
            Id = filePath;
            // In a real implementation, this would load the texture
            // and set Width/Height based on the loaded image
            Width = 64; // Placeholder
            Height = 64; // Placeholder
        }

        /// <summary>
        /// Gets a default placeholder texture.
        /// </summary>
        public static Texture Default => new Texture("default", 64, 64);
    }
}
