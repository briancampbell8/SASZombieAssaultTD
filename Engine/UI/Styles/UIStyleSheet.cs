using System;
using System.Collections.Generic;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.UI.Styles
{
    ///<summary>
    ///A collection of UIStyle objects and lookup utilities
    ///P80-06-02: UIStyleSheet providing a collection of UIStyle objects and lookup utilities
    ///</summary>
    public class UIStyleSheet
    {
        private readonly Dictionary<string, UIStyle> _styles;
        private readonly Dictionary<Type, UIStyle> _typeStyles;

        ///<summary>
        ///Gets the number of styles in the sheet
        ///</summary>
        public int Count => _styles.Count;

        ///<summary>
        ///Gets the number of type styles in the sheet
        ///</summary>
        public int TypeStyleCount => _typeStyles.Count;

        ///<summary>
        ///Initializes a new UIStyleSheet
        ///</summary>
        public UIStyleSheet()
        {
            _styles = new Dictionary<string, UIStyle>();
            _typeStyles = new Dictionary<Type, UIStyle>();

            System.Diagnostics.Debug.WriteLine("UIStyleSheet: Created new style sheet");
        }

        ///<summary>
        ///Adds a style to the sheet
        ///</summary>
        ///<param name="name">Style name</param>
        ///<param name="style">Style to add</param>
        public void AddStyle(string name, UIStyle style)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    System.Diagnostics.Debug.WriteLine("UIStyleSheet: Cannot add style with null or empty name");
                    return;
                }

                if (style == null)
                {
                    System.Diagnostics.Debug.WriteLine("UIStyleSheet: Cannot add null style");
                    return;
                }

                _styles[name] = style;
                System.Diagnostics.Debug.WriteLine($"UIStyleSheet: Added style '{name}'");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIStyleSheet: Error adding style '{name}' - {ex.Message}");
            }
        }

        ///<summary>
        ///Adds a style for a specific type
        ///</summary>
        ///<param name="type">Type to associate with style</param>
        ///<param name="style">Style to add</param>
        public void AddTypeStyle(Type type, UIStyle style)
        {
            try
            {
                if (type == null)
                {
                    System.Diagnostics.Debug.WriteLine("UIStyleSheet: Cannot add type style with null type");
                    return;
                }

                if (style == null)
                {
                    System.Diagnostics.Debug.WriteLine("UIStyleSheet: Cannot add null type style");
                    return;
                }

                _typeStyles[type] = style;
                System.Diagnostics.Debug.WriteLine($"UIStyleSheet: Added type style for '{type.Name}'");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIStyleSheet: Error adding type style for '{type?.Name}' - {ex.Message}");
            }
        }

        ///<summary>
        ///Gets a style by name
        ///</summary>
        ///<param name="name">Style name</param>
        ///<returns>Style, or null if not found</returns>
        public UIStyle GetStyle(string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    System.Diagnostics.Debug.WriteLine("UIStyleSheet: Cannot get style with null or empty name");
                    return null;
                }

                _styles.TryGetValue(name, out var style);
                return style;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIStyleSheet: Error getting style '{name}' - {ex.Message}");
                return null;
            }
        }

        ///<summary>
        ///Gets a style by type
        ///</summary>
        ///<param name="type">Type to get style for</param>
        ///<returns>Style, or null if not found</returns>
        public UIStyle GetStyle(Type type)
        {
            try
            {
                if (type == null)
                {
                    System.Diagnostics.Debug.WriteLine("UIStyleSheet: Cannot get style for null type");
                    return null;
                }

                _typeStyles.TryGetValue(type, out var style);
                return style;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIStyleSheet: Error getting style for type '{type.Name}' - {ex.Message}");
                return null;
            }
        }

        ///<summary>
        ///Gets a style by type (generic version)
        ///</summary>
        ///<typeparam name="T">Type to get style for</typeparam>
        ///<returns>Style, or null if not found</returns>
        public UIStyle GetStyle<T>()
        {
            return GetStyle(typeof(T));
        }

        ///<summary>
        ///Removes a style by name
        ///</summary>
        ///<param name="name">Style name to remove</param>
        ///<returns>True if style was removed</returns>
        public bool RemoveStyle(string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    System.Diagnostics.Debug.WriteLine("UIStyleSheet: Cannot remove style with null or empty name");
                    return false;
                }

                bool removed = _styles.Remove(name);
                if (removed)
                {
                    System.Diagnostics.Debug.WriteLine($"UIStyleSheet: Removed style '{name}'");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"UIStyleSheet: Style '{name}' not found");
                }

                return removed;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIStyleSheet: Error removing style '{name}' - {ex.Message}");
                return false;
            }
        }

        ///<summary>
        ///Removes a style by type
        ///</summary>
        ///<param name="type">Type to remove style for</param>
        ///<returns>True if style was removed</returns>
        public bool RemoveTypeStyle(Type type)
        {
            try
            {
                if (type == null)
                {
                    System.Diagnostics.Debug.WriteLine("UIStyleSheet: Cannot remove type style for null type");
                    return false;
                }

                bool removed = _typeStyles.Remove(type);
                if (removed)
                {
                    System.Diagnostics.Debug.WriteLine($"UIStyleSheet: Removed type style for '{type.Name}'");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"UIStyleSheet: Type style for '{type.Name}' not found");
                }

                return removed;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIStyleSheet: Error removing type style for '{type?.Name}' - {ex.Message}");
                return false;
            }
        }

        ///<summary>
        ///Checks if a style exists by name
        ///</summary>
        ///<param name="name">Style name to check</param>
        ///<returns>True if style exists</returns>
        public bool HasStyle(string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                    return false;

                return _styles.ContainsKey(name);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIStyleSheet: Error checking style '{name}' - {ex.Message}");
                return false;
            }
        }

        ///<summary>
        ///Checks if a style exists by type
        ///</summary>
        ///<param name="type">Type to check</param>
        ///<returns>True if style exists</returns>
        public bool HasStyle(Type type)
        {
            try
            {
                if (type == null)
                    return false;

                return _typeStyles.ContainsKey(type);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIStyleSheet: Error checking type style for '{type?.Name}' - {ex.Message}");
                return false;
            }
        }

        ///<summary>
        ///Gets all style names
        ///</summary>
        ///<returns>Collection of style names</returns>
        public IEnumerable<string> GetStyleNames()
        {
            try
            {
                return _styles.Keys;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIStyleSheet: Error getting style names - {ex.Message}");
                return new List<string>();
            }
        }

        ///<summary>
        ///Clears all styles
        ///</summary>
        public void Clear()
        {
            try
            {
                _styles.Clear();
                _typeStyles.Clear();

                System.Diagnostics.Debug.WriteLine("UIStyleSheet: Cleared all styles");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIStyleSheet: Error clearing styles - {ex.Message}");
            }
        }

        ///<summary>
        ///Creates a copy of this style sheet
        ///</summary>
        ///<returns>Copy of the style sheet</returns>
        public UIStyleSheet Copy()
        {
            try
            {
                var copy = new UIStyleSheet();

                //Copy named styles
                foreach (var kvp in _styles)
                {
                    copy.AddStyle(kvp.Key, kvp.Value.Copy());
                }

                //Copy type styles
                foreach (var kvp in _typeStyles)
                {
                    copy.AddTypeStyle(kvp.Key, kvp.Value.Copy());
                }

                System.Diagnostics.Debug.WriteLine("UIStyleSheet: Created style sheet copy");
                return copy;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIStyleSheet: Error creating copy - {ex.Message}");
                return new UIStyleSheet();
            }
        }

        ///<summary>
        ///Gets a string representation of the style sheet
        ///</summary>
        ///<returns>String representation</returns>
        public override string ToString()
        {
            try
            {
                return $"UIStyleSheet: {_styles.Count} styles, {_typeStyles.Count} type styles";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIStyleSheet: Error creating string representation - {ex.Message}");
                return "UIStyleSheet: Error";
            }
        }
    }
}




