// ====================================================================================================
//  FILE: UIAssetLoader.cs
//  PATH: ./Engine/UI/Assets/
//  MODULE: UI
//
//  ROLE:
//      Provide UI layout, interaction logic, or HUD rendering.
//
//  RESPONSIBILITIES:
//      - Provide LoadFont() behavior for the UI subsystem.
//      - Provide LoadSprite() behavior for the UI subsystem.
//      - Provide LoadTexture() behavior for the UI subsystem.
//      - Provide GetFont() behavior for the UI subsystem.
//      - Provide GetSprite() behavior for the UI subsystem.
//      - Provide GetTexture() behavior for the UI subsystem.
//      - Provide GetLoadedFontNames() behavior for the UI subsystem.
//      - Provide GetLoadedSpriteNames() behavior for the UI subsystem.
//      - Provide GetLoadedTextureNames() behavior for the UI subsystem.
//      - Provide UnloadFont() behavior for the UI subsystem.
//      - Provide UnloadSprite() behavior for the UI subsystem.
//      - Provide UnloadTexture() behavior for the UI subsystem.
//      - Provide Clear() behavior for the UI subsystem.
//      - Provide ToString() behavior for the UI subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.UI.Assets
{
    /// <summary>
    /// Loading utilities for fonts, textures, and UI resources P80-07-01: UIAssetLoader providing loading utilities for
    /// fonts, textures, and UI resources
    /// </summary>
    public class UIAssetLoader
    {
        private readonly Dictionary<string, UIFont> _loadedFonts;
        private readonly Dictionary<string, UISprite> _loadedSprites;
        private readonly Dictionary<string, byte[]> _loadedTextures;
        private bool _isInitialized = false;

        /// <summary>
        /// Gets the number of loaded fonts
        /// </summary>
        public int LoadedFontCount => _loadedFonts.Count;

        /// <summary>
        /// Gets the number of loaded sprites
        /// </summary>
        public int LoadedSpriteCount => _loadedSprites.Count;

        /// <summary>
        /// Gets the number of loaded textures
        /// </summary>
        public int LoadedTextureCount => _loadedTextures.Count;

        /// <summary>
        /// Gets whether the loader is initialized
        /// </summary>
        public bool IsInitialized => _isInitialized;

        /// <summary>
        /// Initializes a new UIAssetLoader
        /// </summary>
        public UIAssetLoader()
        {
            _loadedFonts = new Dictionary<string, UIFont>();
            _loadedSprites = new Dictionary<string, UISprite>();
            _loadedTextures = new Dictionary<string, byte[]>();

            DLogger.Log(LogSubsystems.ResourcesPipeline, "UIAssetLoader: Initialized");
        }

        /// <summary>
        /// Loads a font from file
        /// </summary>
        /// <param name="path">Font file path</param>
        /// <param name="name">Font name</param>
        /// <param name="size">Font size</param>
        /// <param name="style">Font style</param>
        /// <returns>Loaded font, or null if failed</returns>
        public UIFont LoadFont(string path, string name, float size, System.Drawing.FontStyle style)
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                {
                    DLogger.Log($"UIAssetLoader: Cannot load font from null or empty path");
                    return null;
                }

                if (_loadedFonts.ContainsKey(name))
                {
                    DLogger.Log($"UIAssetLoader: Font '{name}' already loaded");
                    return _loadedFonts[name];
                }

                var font = new UIFont(name, size, path, style);

                //In a real implementation, this would load the font file
                font.MarkAsLoaded();

                _loadedFonts[name] = font;

                DLogger.Log($"UIAssetLoader: Loaded font '{name}' from '{path}'");
                return font;
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIAssetLoader: Error loading font '{name}' - {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Loads a sprite from file
        /// </summary>
        /// <param name="path">Sprite file path</param>
        /// <param name="name">Sprite name</param>
        /// <param name="size">Sprite size</param>
        /// <param name="sourceRect">Source rectangle in texture</param>
        /// <returns>Loaded sprite, or null if failed</returns>
        public UISprite LoadSprite(string path, string name, System.Drawing.SizeF size, System.Drawing.RectangleF sourceRect)
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                {
                    DLogger.Log(LogSubsystems.ResourcesPipeline, "UIAssetLoader: Cannot load sprite from null or empty path");
                    return null;
                }

                if (_loadedSprites.ContainsKey(name))
                {
                    DLogger.Log($"UIAssetLoader: Sprite '{name}' already loaded");
                    return _loadedSprites[name];
                }

                var sprite = new UISprite(name, path, size, sourceRect);

                //In a real implementation, this would load the sprite file
                sprite.MarkAsLoaded();

                _loadedSprites[name] = sprite;

                DLogger.Log($"UIAssetLoader: Loaded sprite '{name}' from '{path}'");
                return sprite;
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIAssetLoader: Error loading sprite '{name}' - {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Loads a texture from file
        /// </summary>
        /// <param name="path">Texture file path</param>
        /// <param name="name">Texture name</param>
        /// <returns>Loaded texture data, or null if failed</returns>
        public byte[] LoadTexture(string path, string name)
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                {
                    DLogger.Log($"UIAssetLoader: Cannot load texture from null or empty path");
                    return null;
                }

                if (_loadedTextures.ContainsKey(name))
                {
                    DLogger.Log($"UIAssetLoader: Texture '{name}' already loaded");
                    return _loadedTextures[name];
                }

                //In a real implementation, this would load the texture file
                var textureData = new byte[0]; //Placeholder
                _loadedTextures[name] = textureData;

                DLogger.Log($"UIAssetLoader: Loaded texture '{name}' from '{path}'");
                return textureData;
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIAssetLoader: Error loading texture '{name}' - {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Gets a loaded font by name
        /// </summary>
        /// <param name="name">Font name</param>
        /// <returns>Loaded font, or null if not found</returns>
        public UIFont GetFont(string name)
        {
            try
            {
                _loadedFonts.TryGetValue(name, out var font);
                return font;
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIAssetLoader: Error getting font '{name}' - {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Gets a loaded sprite by name
        /// </summary>
        /// <param name="name">Sprite name</param>
        /// <returns>Loaded sprite, or null if not found</returns>
        public UISprite GetSprite(string name)
        {
            try
            {
                _loadedSprites.TryGetValue(name, out var sprite);
                return sprite;
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIAssetLoader: Error getting sprite '{name}' - {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Gets loaded texture data by name
        /// </summary>
        /// <param name="name">Texture name</param>
        /// <returns>Loaded texture data, or null if not found</returns>
        public byte[] GetTexture(string name)
        {
            try
            {
                _loadedTextures.TryGetValue(name, out var texture);
                return texture;
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIAssetLoader: Error getting texture '{name}' - {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Gets all loaded font names
        /// </summary>
        /// <returns>Collection of font names</returns>
        public IEnumerable<string> GetLoadedFontNames()
        {
            try
            {
                return _loadedFonts.Keys;
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIAssetLoader: Error getting loaded font names - {ex.Message}");
                return new List<string>();
            }
        }

        /// <summary>
        /// Gets all loaded sprite names
        /// </summary>
        /// <returns>Collection of sprite names</returns>
        public IEnumerable<string> GetLoadedSpriteNames()
        {
            try
            {
                return _loadedSprites.Keys;
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIAssetLoader: Error getting loaded sprite names - {ex.Message}");
                return new List<string>();
            }
        }

        /// <summary>
        /// Gets all loaded texture names
        /// </summary>
        /// <returns>Collection of texture names</returns>
        public IEnumerable<string> GetLoadedTextureNames()
        {
            try
            {
                return _loadedTextures.Keys;
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIAssetLoader: Error getting loaded texture names - {ex.Message}");
                return new List<string>();
            }
        }

        /// <summary>
        /// Unloads a font
        /// </summary>
        /// <param name="name">Font name to unload</param>
        /// <returns>True if font was unloaded</returns>
        public bool UnloadFont(string name)
        {
            try
            {
                if (_loadedFonts.Remove(name))
                {
                    DLogger.Log($"UIAssetLoader: Unloaded font '{name}'");
                    return true;
                }
                else
                {
                    DLogger.Log($"UIAssetLoader: Font '{name}' not found in loaded fonts");
                    return false;
                }
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIAssetLoader: Error unloading font '{name}' - {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Unloads a sprite
        /// </summary>
        /// <param name="name">Sprite name to unload</param>
        /// <returns>True if sprite was unloaded</returns>
        public bool UnloadSprite(string name)
        {
            try
            {
                if (_loadedSprites.Remove(name))
                {
                    DLogger.Log($"UIAssetLoader: Unloaded sprite '{name}'");
                    return true;
                }
                else
                {
                    DLogger.Log($"UIAssetLoader: Sprite '{name}' not found in loaded sprites");
                    return false;
                }
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIAssetLoader: Error unloading sprite '{name}' - {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Unloads a texture
        /// </summary>
        /// <param name="name">Texture name to unload</param>
        /// <returns>True if texture was unloaded</returns>
        public bool UnloadTexture(string name)
        {
            try
            {
                if (_loadedTextures.Remove(name))
                {
                    DLogger.Log($"UIAssetLoader: Unloaded texture '{name}'");
                    return true;
                }
                else
                {
                    DLogger.Log($"UIAssetLoader: Texture '{name}' not found in loaded textures");
                    return false;
                }
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIAssetLoader: Error unloading texture '{name}' - {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Clears all loaded assets
        /// </summary>
        public void Clear()
        {
            try
            {
                _loadedFonts.Clear();
                _loadedSprites.Clear();
                _loadedTextures.Clear();

                DLogger.Log(LogSubsystems.ResourcesPipeline, "UIAssetLoader: Cleared all loaded assets");
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIAssetLoader: Error clearing assets - {ex.Message}");
            }
        }

        /// <summary>
        /// Gets a string representation of the asset loader state
        /// </summary>
        /// <returns>String representation</returns>
        public override string ToString()
        {
            try
            {
                return $"UIAssetLoader: {_loadedFonts.Count} fonts, {_loadedSprites.Count} sprites, {_loadedTextures.Count} textures";
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIAssetLoader: Error creating string representation - {ex.Message}");
                return "UIAssetLoader: Error";
            }
        }
    }
}
