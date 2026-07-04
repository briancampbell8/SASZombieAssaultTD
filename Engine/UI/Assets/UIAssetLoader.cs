using System;
using System.Collections.Generic;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.UI.Assets
{
    ///<summary>
    ///Loading utilities for fonts, textures, and UI resources
    ///P80-07-01: UIAssetLoader providing loading utilities for fonts, textures, and UI resources
    ///</summary>
    public class UIAssetLoader
    {
        private readonly Dictionary<string, UIFont> _loadedFonts;
        private readonly Dictionary<string, UISprite> _loadedSprites;
        private readonly Dictionary<string, byte[]> _loadedTextures;
        private bool _isInitialized = false;

        ///<summary>
        ///Gets the number of loaded fonts
        ///</summary>
        public int LoadedFontCount => _loadedFonts.Count;

        ///<summary>
        ///Gets the number of loaded sprites
        ///</summary>
        public int LoadedSpriteCount => _loadedSprites.Count;

        ///<summary>
        ///Gets the number of loaded textures
        ///</summary>
        public int LoadedTextureCount => _loadedTextures.Count;

        ///<summary>
        ///Gets whether the loader is initialized
        ///</summary>
        public bool IsInitialized => _isInitialized;

        ///<summary>
        ///Initializes a new UIAssetLoader
        ///</summary>
        public UIAssetLoader()
        {
            _loadedFonts = new Dictionary<string, UIFont>();
            _loadedSprites = new Dictionary<string, UISprite>();
            _loadedTextures = new Dictionary<string, byte[]>();

            System.Diagnostics.Debug.WriteLine("UIAssetLoader: Initialized");
        }

        ///<summary>
        ///Loads a font from file
        ///</summary>
        ///<param name="path">Font file path</param>
        ///<param name="name">Font name</param>
        ///<param name="size">Font size</param>
        ///<param name="style">Font style</param>
        ///<returns>Loaded font, or null if failed</returns>
        public UIFont LoadFont(string path, string name, float size, System.Drawing.FontStyle style)
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                {
                    System.Diagnostics.Debug.WriteLine($"UIAssetLoader: Cannot load font from null or empty path");
                    return null;
                }

                if (_loadedFonts.ContainsKey(name))
                {
                    System.Diagnostics.Debug.WriteLine($"UIAssetLoader: Font '{name}' already loaded");
                    return _loadedFonts[name];
                }

                var font = new UIFont(name, size, path, style);

                //In a real implementation, this would load the font file
                font.MarkAsLoaded();

                _loadedFonts[name] = font;

                System.Diagnostics.Debug.WriteLine($"UIAssetLoader: Loaded font '{name}' from '{path}'");
                return font;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIAssetLoader: Error loading font '{name}' - {ex.Message}");
                return null;
            }
        }

        ///<summary>
        ///Loads a sprite from file
        ///</summary>
        ///<param name="path">Sprite file path</param>
        ///<param name="name">Sprite name</param>
        ///<param name="size">Sprite size</param>
        ///<param name="sourceRect">Source rectangle in texture</param>
        ///<returns>Loaded sprite, or null if failed</returns>
        public UISprite LoadSprite(string path, string name, System.Drawing.SizeF size, System.Drawing.RectangleF sourceRect)
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                {
                    System.Diagnostics.Debug.WriteLine("UIAssetLoader: Cannot load sprite from null or empty path");
                    return null;
                }

                if (_loadedSprites.ContainsKey(name))
                {
                    System.Diagnostics.Debug.WriteLine($"UIAssetLoader: Sprite '{name}' already loaded");
                    return _loadedSprites[name];
                }

                var sprite = new UISprite(name, path, size, sourceRect);

                //In a real implementation, this would load the sprite file
                sprite.MarkAsLoaded();

                _loadedSprites[name] = sprite;

                System.Diagnostics.Debug.WriteLine($"UIAssetLoader: Loaded sprite '{name}' from '{path}'");
                return sprite;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIAssetLoader: Error loading sprite '{name}' - {ex.Message}");
                return null;
            }
        }

        ///<summary>
        ///Loads a texture from file
        ///</summary>
        ///<param name="path">Texture file path</param>
        ///<param name="name">Texture name</param>
        ///<returns>Loaded texture data, or null if failed</returns>
        public byte[] LoadTexture(string path, string name)
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                {
                    System.Diagnostics.Debug.WriteLine($"UIAssetLoader: Cannot load texture from null or empty path");
                    return null;
                }

                if (_loadedTextures.ContainsKey(name))
                {
                    System.Diagnostics.Debug.WriteLine($"UIAssetLoader: Texture '{name}' already loaded");
                    return _loadedTextures[name];
                }

                //In a real implementation, this would load the texture file
                var textureData = new byte[0]; //Placeholder
                _loadedTextures[name] = textureData;

                System.Diagnostics.Debug.WriteLine($"UIAssetLoader: Loaded texture '{name}' from '{path}'");
                return textureData;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIAssetLoader: Error loading texture '{name}' - {ex.Message}");
                return null;
            }
        }

        ///<summary>
        ///Gets a loaded font by name
        ///</summary>
        ///<param name="name">Font name</param>
        ///<returns>Loaded font, or null if not found</returns>
        public UIFont GetFont(string name)
        {
            try
            {
                _loadedFonts.TryGetValue(name, out var font);
                return font;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIAssetLoader: Error getting font '{name}' - {ex.Message}");
                return null;
            }
        }

        ///<summary>
        ///Gets a loaded sprite by name
        ///</summary>
        ///<param name="name">Sprite name</param>
        ///<returns>Loaded sprite, or null if not found</returns>
        public UISprite GetSprite(string name)
        {
            try
            {
                _loadedSprites.TryGetValue(name, out var sprite);
                return sprite;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIAssetLoader: Error getting sprite '{name}' - {ex.Message}");
                return null;
            }
        }

        ///<summary>
        ///Gets loaded texture data by name
        ///</summary>
        ///<param name="name">Texture name</param>
        ///<returns>Loaded texture data, or null if not found</returns>
        public byte[] GetTexture(string name)
        {
            try
            {
                _loadedTextures.TryGetValue(name, out var texture);
                return texture;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIAssetLoader: Error getting texture '{name}' - {ex.Message}");
                return null;
            }
        }

        ///<summary>
        ///Gets all loaded font names
        ///</summary>
        ///<returns>Collection of font names</returns>
        public IEnumerable<string> GetLoadedFontNames()
        {
            try
            {
                return _loadedFonts.Keys;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIAssetLoader: Error getting loaded font names - {ex.Message}");
                return new List<string>();
            }
        }

        ///<summary>
        ///Gets all loaded sprite names
        ///</summary>
        ///<returns>Collection of sprite names</returns>
        public IEnumerable<string> GetLoadedSpriteNames()
        {
            try
            {
                return _loadedSprites.Keys;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIAssetLoader: Error getting loaded sprite names - {ex.Message}");
                return new List<string>();
            }
        }

        ///<summary>
        ///Gets all loaded texture names
        ///</summary>
        ///<returns>Collection of texture names</returns>
        public IEnumerable<string> GetLoadedTextureNames()
        {
            try
            {
                return _loadedTextures.Keys;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIAssetLoader: Error getting loaded texture names - {ex.Message}");
                return new List<string>();
            }
        }

        ///<summary>
        ///Unloads a font
        ///</summary>
        ///<param name="name">Font name to unload</param>
        ///<returns>True if font was unloaded</returns>
        public bool UnloadFont(string name)
        {
            try
            {
                if (_loadedFonts.Remove(name))
                {
                    System.Diagnostics.Debug.WriteLine($"UIAssetLoader: Unloaded font '{name}'");
                    return true;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"UIAssetLoader: Font '{name}' not found in loaded fonts");
                    return false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIAssetLoader: Error unloading font '{name}' - {ex.Message}");
                return false;
            }
        }

        ///<summary>
        ///Unloads a sprite
        ///</summary>
        ///<param name="name">Sprite name to unload</param>
        ///<returns>True if sprite was unloaded</returns>
        public bool UnloadSprite(string name)
        {
            try
            {
                if (_loadedSprites.Remove(name))
                {
                    System.Diagnostics.Debug.WriteLine($"UIAssetLoader: Unloaded sprite '{name}'");
                    return true;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"UIAssetLoader: Sprite '{name}' not found in loaded sprites");
                    return false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIAssetLoader: Error unloading sprite '{name}' - {ex.Message}");
                return false;
            }
        }

        ///<summary>
        ///Unloads a texture
        ///</summary>
        ///<param name="name">Texture name to unload</param>
        ///<returns>True if texture was unloaded</returns>
        public bool UnloadTexture(string name)
        {
            try
            {
                if (_loadedTextures.Remove(name))
                {
                    System.Diagnostics.Debug.WriteLine($"UIAssetLoader: Unloaded texture '{name}'");
                    return true;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"UIAssetLoader: Texture '{name}' not found in loaded textures");
                    return false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIAssetLoader: Error unloading texture '{name}' - {ex.Message}");
                return false;
            }
        }

        ///<summary>
        ///Clears all loaded assets
        ///</summary>
        public void Clear()
        {
            try
            {
                _loadedFonts.Clear();
                _loadedSprites.Clear();
                _loadedTextures.Clear();

                System.Diagnostics.Debug.WriteLine("UIAssetLoader: Cleared all loaded assets");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIAssetLoader: Error clearing assets - {ex.Message}");
            }
        }

        ///<summary>
        ///Gets a string representation of the asset loader state
        ///</summary>
        ///<returns>String representation</returns>
        public override string ToString()
        {
            try
            {
                return $"UIAssetLoader: {_loadedFonts.Count} fonts, {_loadedSprites.Count} sprites, {_loadedTextures.Count} textures";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIAssetLoader: Error creating string representation - {ex.Message}");
                return "UIAssetLoader: Error";
            }
        }
    }
}




