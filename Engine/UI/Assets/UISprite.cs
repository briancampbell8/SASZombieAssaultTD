// ====================================================================================================
//  FILE: UISprite.cs
//  PATH: ./Engine/UI/Assets/
//  MODULE: UI
//
//  ROLE:
//      Provide UI layout, interaction logic, or HUD rendering.
//
//  RESPONSIBILITIES:
//      - Provide MarkAsLoaded() behavior for the UI subsystem.
//      - Provide MarkAsPreloaded() behavior for the UI subsystem.
//      - Provide Unload() behavior for the UI subsystem.
//      - Provide ToString() behavior for the UI subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.UI.Assets
{
    ///<summary>
    ///Sprite metadata used by UI elements
    ///P80-07-03: UISprite defining sprite metadata used by UI elements
    ///</summary>
    public class UISprite
    {
        private string _name;
        private string _path;
        private System.Drawing.SizeF _size;
        private System.Drawing.RectangleF _sourceRect;
        private bool _isLoaded;
        private bool _isPreloaded;

        ///<summary>
        ///Gets the sprite name
        ///</summary>
        public string Name => _name;

        ///<summary>
        ///Gets the sprite path
        ///</summary>
        public string Path => _path;

        ///<summary>
        ///Gets the sprite size
        ///</summary>
        public System.Drawing.SizeF Size => _size;

        ///<summary>
        ///Gets the source rectangle
        ///</summary>
        public System.Drawing.RectangleF SourceRect => _sourceRect;

        ///<summary>
        ///Gets whether the sprite is loaded
        ///</summary>
        public bool IsLoaded => _isLoaded;

        ///<summary>
        ///Gets whether the sprite is preloaded
        ///</summary>
        public bool IsPreloaded => _isPreloaded;

        ///<summary>
        ///Initializes a new UISprite
        ///</summary>
        public UISprite() => DLogger.Log(LogSubsystems.ResourcesPipeline, "UISprite: Created new sprite");

        ///<summary>
        ///Initializes a new UISprite with parameters
        ///</summary>
        ///<param name="name">Sprite name</param>
        ///<param name="path">Sprite file path</param>
        ///<param name="size">Sprite size</param>
        ///<param name="sourceRect">Source rectangle in texture</param>
        public UISprite(string name, string path, System.Drawing.SizeF size, System.Drawing.RectangleF sourceRect)
        {
            try
            {
                _name = name ?? string.Empty;
                _path = path ?? string.Empty;
                _size = size;
                _sourceRect = sourceRect;

                DLogger.Log($"UISprite: Created sprite '{name}' with size {size} from path '{path}'");
            }
            catch (Exception ex)
            {
                DLogger.Log($"UISprite: Error creating sprite - {ex.Message}");
            }
        }

        ///<summary>
        ///Marks the sprite as loaded
        ///</summary>
        public void MarkAsLoaded()
        {
            try
            {
                _isLoaded = true;
                DLogger.Log($"UISprite: Marked sprite '{_name}' as loaded");
            }
            catch (Exception ex)
            {
                DLogger.Log($"UISprite: Error marking sprite as loaded - {ex.Message}");
            }
        }

        ///<summary>
        ///Marks the sprite as preloaded
        ///</summary>
        public void MarkAsPreloaded()
        {
            try
            {
                _isPreloaded = true;
                DLogger.Log($"UISprite: Marked sprite '{_name}' as preloaded");
            }
            catch (Exception ex)
            {
                DLogger.Log($"UISprite: Error marking sprite as preloaded - {ex.Message}");
            }
        }

        ///<summary>
        ///Unloads the sprite
        ///</summary>
        public void Unload()
        {
            try
            {
                _isLoaded = false;
                _isPreloaded = false;

                DLogger.Log($"UISprite: Unloaded sprite '{_name}'");
            }
            catch (Exception ex)
            {
                DLogger.Log($"UISprite: Error unloading sprite - {ex.Message}");
            }
        }

        ///<summary>
        ///Gets a string representation of the sprite
        ///</summary>
        ///<returns>String representation</returns>
        public override string ToString()
        {
            try
            {
                return $"UISprite: Name='{_name}', Path='{_path}', Size={_size}, SourceRect={_sourceRect}, Loaded={_isLoaded}";
            }
            catch (Exception ex)
            {
                DLogger.Log($"UISprite: Error creating string representation - {ex.Message}");
                return "UISprite: Error";
            }
        }
    }
}





