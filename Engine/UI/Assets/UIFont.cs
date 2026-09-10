// ====================================================================================================
//  FILE: UIFont.cs
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

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.UI.Assets
{
    ///<summary>
    ///Font metadata used by UI elements
    ///P80-07-02: UIFont defining font metadata used by UIText
    ///</summary>
    public class UIFont
    {
        private string _name;
        private float _size;
        private string _path;
        private bool _isLoaded;
        private System.Drawing.FontStyle _style;
        private bool _isPreloaded;

        ///<summary>
        ///Gets the font name
        ///</summary>
        public string Name => _name;

        ///<summary>
        ///Gets the font size
        ///</summary>
        public float Size => _size;

        ///<summary>
        ///Gets the font path
        ///</summary>
        public string Path => _path;

        ///<summary>
        ///Gets the font style
        ///</summary>
        public System.Drawing.FontStyle Style => _style;

        ///<summary>
        ///Gets whether the font is loaded
        ///</summary>
        public bool IsLoaded => _isLoaded;

        ///<summary>
        ///Gets whether the font is preloaded
        ///</summary>
        public bool IsPreloaded => _isPreloaded;

        ///<summary>
        ///Initializes a new UIFont
        ///</summary>
        public UIFont() => DLogger.Log(LogSubsystems.ResourcesPipeline, "UIFont: Created new font");

        ///<summary>
        ///Initializes a new UIFont with parameters
        ///</summary>
        ///<param name="name">Font name</param>
        ///<param name="size">Font size</param>
        ///<param name="path">Font file path</param>
        ///<param name="style">Font style</param>
        public UIFont(string name, float size, string path, System.Drawing.FontStyle style)
        {
            try
            {
                _name = name ?? string.Empty;
                _size = System.Math.Max(1.0f, size);
                _path = path ?? string.Empty;
                _style = style;

                DLogger.Log($"UIFont: Created font '{name}' with size {_size} from path '{path}'");
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIFont: Error creating font - {ex.Message}");
            }
        }

        ///<summary>
        ///Marks the font as loaded
        ///</summary>
        public void MarkAsLoaded()
        {
            try
            {
                _isLoaded = true;
                DLogger.Log($"UIFont: Marked font '{_name}' as loaded");
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIFont: Error marking font as loaded - {ex.Message}");
            }
        }

        ///<summary>
        ///Marks the font as preloaded
        ///</summary>
        public void MarkAsPreloaded()
        {
            try
            {
                _isPreloaded = true;
                DLogger.Log($"UIFont: Marked font '{_name}' as preloaded");
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIFont: Error marking font as preloaded - {ex.Message}");
            }
        }

        ///<summary>
        ///Unloads the font
        ///</summary>
        public void Unload()
        {
            try
            {
                _isLoaded = false;
                _isPreloaded = false;

                DLogger.Log($"UIFont: Unloaded font '{_name}'");
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIFont: Error unloading font - {ex.Message}");
            }
        }

        ///<summary>
        ///Gets a string representation of the font
        ///</summary>
        ///<returns>String representation</returns>
        public override string ToString()
        {
            try
            {
                return $"UIFont: Name='{_name}', Size={_size}, Path='{_path}', Style={_style}, Loaded={_isLoaded}";
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIFont: Error creating string representation - {ex.Message}");
                return "UIFont: Error";
            }
        }
    }
}





