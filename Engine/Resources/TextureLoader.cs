// ====================================================================================================
//  FILE: TextureLoader.cs
//  PATH: ./Engine/Resources/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the TextureLoader module.
//
//  RESPONSIBILITIES:
//      - Provide LoadTextureBytes() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
using System.IO;

using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

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




