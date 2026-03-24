using System;

namespace SASZombieAssaultTD.Engine.UI.Assets
{
    /// <summary>
    /// Sprite metadata used by UI elements
    /// P80-07-03: UISprite defining sprite metadata used by UI elements
    /// </summary>
    public class UISprite
    {
        private string _name;
        private string _path;
        private System.Drawing.SizeF _size;
        private System.Drawing.RectangleF _sourceRect;
        private bool _isLoaded;
        private bool _isPreloaded;

        /// <summary>
        /// Gets the sprite name
        /// </summary>
        public string Name => _name;

        /// <summary>
        /// Gets the sprite path
        /// </summary>
        public string Path => _path;

        /// <summary>
        /// Gets the sprite size
        /// </summary>
        public System.Drawing.SizeF Size => _size;

        /// <summary>
        /// Gets the source rectangle
        /// </summary>
        public System.Drawing.RectangleF SourceRect => _sourceRect;

        /// <summary>
        /// Gets whether the sprite is loaded
        /// </summary>
        public bool IsLoaded => _isLoaded;

        /// <summary>
        /// Gets whether the sprite is preloaded
        /// </summary>
        public bool IsPreloaded => _isPreloaded;

        /// <summary>
        /// Initializes a new UISprite
        /// </summary>
        public UISprite()
        {
            Console.WriteLine("UISprite: Created new sprite");
        }

        /// <summary>
        /// Initializes a new UISprite with parameters
        /// </summary>
        /// <param name="name">Sprite name</param>
        /// <param name="path">Sprite file path</param>
        /// <param name="size">Sprite size</param>
        /// <param name="sourceRect">Source rectangle in texture</param>
        public UISprite(string name, string path, System.Drawing.SizeF size, System.Drawing.RectangleF sourceRect)
        {
            try
            {
                _name = name ?? string.Empty;
                _path = path ?? string.Empty;
                _size = size;
                _sourceRect = sourceRect;

                Console.WriteLine($"UISprite: Created sprite '{name}' with size {size} from path '{path}'");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UISprite: Error creating sprite - {ex.Message}");
            }
        }

        /// <summary>
        /// Marks the sprite as loaded
        /// </summary>
        public void MarkAsLoaded()
        {
            try
            {
                _isLoaded = true;
                Console.WriteLine($"UISprite: Marked sprite '{_name}' as loaded");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UISprite: Error marking sprite as loaded - {ex.Message}");
            }
        }

        /// <summary>
        /// Marks the sprite as preloaded
        /// </summary>
        public void MarkAsPreloaded()
        {
            try
            {
                _isPreloaded = true;
                Console.WriteLine($"UISprite: Marked sprite '{_name}' as preloaded");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UISprite: Error marking sprite as preloaded - {ex.Message}");
            }
        }

        /// <summary>
        /// Unloads the sprite
        /// </summary>
        public void Unload()
        {
            try
            {
                _isLoaded = false;
                _isPreloaded = false;

                Console.WriteLine($"UISprite: Unloaded sprite '{_name}'");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UISprite: Error unloading sprite - {ex.Message}");
            }
        }

        /// <summary>
        /// Gets a string representation of the sprite
        /// </summary>
        /// <returns>String representation</returns>
        public override string ToString()
        {
            try
            {
                return $"UISprite: Name='{_name}', Path='{_path}', Size={_size}, SourceRect={_sourceRect}, Loaded={_isLoaded}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UISprite: Error creating string representation - {ex.Message}");
                return "UISprite: Error";
            }
        }
    }
}




