using System;

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
        public UIFont()
        {
            System.Diagnostics.Debug.WriteLine("UIFont: Created new font");
        }

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

                System.Diagnostics.Debug.WriteLine($"UIFont: Created font '{name}' with size {_size} from path '{path}'");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIFont: Error creating font - {ex.Message}");
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
                System.Diagnostics.Debug.WriteLine($"UIFont: Marked font '{_name}' as loaded");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIFont: Error marking font as loaded - {ex.Message}");
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
                System.Diagnostics.Debug.WriteLine($"UIFont: Marked font '{_name}' as preloaded");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIFont: Error marking font as preloaded - {ex.Message}");
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

                System.Diagnostics.Debug.WriteLine($"UIFont: Unloaded font '{_name}'");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIFont: Error unloading font - {ex.Message}");
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
                System.Diagnostics.Debug.WriteLine($"UIFont: Error creating string representation - {ex.Message}");
                return "UIFont: Error";
            }
        }
    }
}




