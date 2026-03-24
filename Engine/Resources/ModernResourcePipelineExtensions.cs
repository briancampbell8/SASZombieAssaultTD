using System;
using System.Threading.Tasks;
using SASZombieAssaultTD.Engine.Audio;

namespace SASZombieAssaultTD.Engine.Resources
{
    /// <summary>
    /// Extension methods for ModernResourcePipeline to provide synchronous access.
    /// </summary>
    public static class ModernResourcePipelineExtensions
    {
        /// <summary>
        /// Gets a sound effect synchronously (blocking call).
        /// </summary>
        /// <param name="pipeline">The resource pipeline.</param>
        /// <param name="soundPath">The path to the sound file.</param>
        /// <returns>The loaded sound effect or null if failed.</returns>
        public static CoreSoundEffect? GetSound(this ModernResourcePipeline pipeline, string soundPath)
        {
            try
            {
                // For now, create a simple sound effect
                // In a real implementation, this would load from disk
                return new CoreSoundEffect(soundPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ModernResourcePipeline: Failed to load sound '{soundPath}': {ex.Message}");
                return null;
            }
        }
    }
}
