using System.IO;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Assets
{
    public static class TextureLoader
    {
        ///<summary>
        ///Loads texture bytes from the specified file path.
        ///</summary>
        public static byte[] LoadTextureBytes(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"Texture file not found: {path}");

            return File.ReadAllBytes(path);
        }
    }
}



