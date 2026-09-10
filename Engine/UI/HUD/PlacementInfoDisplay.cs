// ====================================================================================================
//  FILE: PlacementInfoDisplay.cs
//  PATH: ./Engine/UI/HUD/
//  MODULE: UI
//
//  ROLE:
//      Provide UI layout, interaction logic, or HUD rendering.
//
//  RESPONSIBILITIES:
//      - Provide SetPlacementInfo() behavior for the UI subsystem.
//      - Provide HidePlacementInfo() behavior for the UI subsystem.
//      - Provide SetPosition() behavior for the UI subsystem.
//      - Provide SetSize() behavior for the UI subsystem.
//      - Provide SetValidColor() behavior for the UI subsystem.
//      - Provide SetInvalidColor() behavior for the UI subsystem.
//      - Provide SetWarningColor() behavior for the UI subsystem.
//      - Provide SetNormalColor() behavior for the UI subsystem.
//      - Provide SetPulseProperties() behavior for the UI subsystem.
//      - Provide SetTextPrefix() behavior for the UI subsystem.
//      - Provide SetTextFormat() behavior for the UI subsystem.
//      - Provide SetDisplayTimer() behavior for the UI subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
using System;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Navigation;
using SASZombieAssaultTD.Engine.Towers.Placement;
using SASZombieAssaultTD.Engine.UI.HUD;
using SASZombieAssaultTD.Engine.VectorMath;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine
{
    /// <summary>
    /// Modern placement info display for SAS Zombie Assault TD HUD. Shows tower placement information and validation
    /// feedback.
    /// </summary>
    public class PlacementInfoDisplay : ModernHUDComponent
    {
        private PlacementInfo _placementInfo;

        private float _displayTimer = 0f;
        private bool _isTransitioning = false;
        private float _transitionTimer = 0f;
        private float _transitionDuration = 0.2f;

        // Visual colors (System.Drawing.Color)
        private Color _validColor = Color.FromArgb(180, 0, 255, 0);

        private Color _invalidColor = Color.FromArgb(180, 255, 0, 0);
        private Color _warningColor = Color.FromArgb(180, 255, 255, 0);
        private Color _normalColor = Color.FromArgb(255, 255, 255, 255);
        private Color _borderColor = Color.FromArgb(255, 200, 200, 200);

        // Animation
        private float _pulseSpeed = 2f;

        private float _pulseAmount = 0.1f;
        private float _pulseTimer = 0f;
        private bool _isPulsing = false;
        private object FontCache;

        // Events
        public event Action<PlacementInfo> OnPlacementAttempted;

        public event Action<PlacementInfo> OnPlacementConfirmed;

        public event Action<PlacementInfo> OnPlacementCancelled;

        public PlacementInfoDisplay(Vector3 position, Vector3 size)
        {
            _position = position;
            _size = size;
        }

        // ---------------------------------------------------------------------------------------------
        // Public API
        // ---------------------------------------------------------------------------------------------

        public void SetPlacementInfo(PlacementInfo info)
        {
            _placementInfo = info;
            _displayTimer = 2f;
            _isVisible = true;

            _isTransitioning = true;
            _transitionTimer = 0f;

            StartTransition();
        }

        public void HidePlacementInfo()
        {
            _isVisible = false;
            _isTransitioning = false;
            _displayTimer = 0f;
            _placementInfo = null;
        }

        public void SetPosition(Vector3 position)
        {
            _position = position;
        }

        public void SetSize(Vector3 size)
        {
            _size = size;
        }

        public void SetValidColor(Color color) => _validColor = color;

        public void SetInvalidColor(Color color) => _invalidColor = color;

        public void SetWarningColor(Color color) => _warningColor = color;

        public void SetNormalColor(Color color) => _normalColor = color;

        public void SetPulseProperties(float speed, float amount)
        {
            _pulseSpeed = System.Math.Max(0.1f, speed);
            _pulseAmount = System.Math.Clamp(amount, 0f, 0.5f);
        }

        public void SetDisplayTimer(float seconds)
        {
            _displayTimer = seconds;
        }

        // ---------------------------------------------------------------------------------------------
        // Transition Animation
        // ---------------------------------------------------------------------------------------------

        private void StartTransition()
        {
            _transitionTimer = 0f;
            _isTransitioning = true;
        }

        private void UpdateTransition(float deltaTime)
        {
            if (!_isTransitioning)
                return;

            _transitionTimer += deltaTime;

            if (_transitionTimer >= _transitionDuration)
            {
                _isTransitioning = false;
                _transitionTimer = 0f;
            }
        }

        private float GetTransitionProgress()
        {
            if (!_isTransitioning)
                return 1f;

            return System.Math.Clamp(_transitionTimer / _transitionDuration, 0f, 1f);
        }

        // ---------------------------------------------------------------------------------------------
        // Rendering
        // ---------------------------------------------------------------------------------------------

        public override void Render()
        {
            if (!_isVisible || _placementInfo == null)
                return;

            RenderBackground();
            RenderTowerPreview();
            RenderStatus();
        }

        private void RenderBackground()
        {
            float alpha = 255f * GetTransitionProgress();

            Color bg = Color.FromArgb((int)alpha, _normalColor.R, _normalColor.G, _normalColor.B);
            Color border = Color.FromArgb((int)alpha, _borderColor.R, _borderColor.G, _borderColor.B);

            SASZombieAssaultTD.Engine.Render.Renderer.DrawRectangle(
                (int)_position.X,
                (int)_position.Y,
                (int)_size.X,
                (int)_size.Y,
                bg);

            SASZombieAssaultTD.Engine.Render.Renderer.DrawRectangle(
                (int)_position.X,
                (int)_position.Y,
                (int)_size.X,
                (int)_size.Y,
                border,
                2f);
        }

        private void RenderTowerPreview()
        {
            if (_placementInfo == null)
                return;

            var towerData = _placementInfo.TowerData;
            var towerSize = towerData.Size;
            var gridPos = _placementInfo.GridPosition;

            var nav = new NavigationGrid();
            var worldPos = nav.GridToWorld(gridPos);

            var color = _placementInfo.CanPlace ? _validColor : _invalidColor;

            var previewScale = 0.8f;
            var previewSize = new Vector3(
                towerSize.X * previewScale,
                towerSize.Y * previewScale,
                towerSize.Z * previewScale);

            var previewPos = new Vector3(
                worldPos.X - previewSize.X / 2f,
                worldPos.Y - previewSize.Y / 2f,
                worldPos.Z);

            if (towerData.Sprite != null)
            {
                SASZombieAssaultTD.Engine.Render.Renderer renderer = new Engine.Render.Renderer();
                renderer.DrawSprite(
                    towerData.Sprite, previewPos, previewSize, color);
            }
        }

        private void RenderStatus()
        {
            string text = GetStatusText();
            Color color = GetStatusColor();

            Color textColor =
                (Color)Color.FromArgb(
                    (byte)
                    (
                    (color.R << 24) |
                    (color.G << 16) |
                    (color.B << 8) |
                    255
                    ));

            var pos = new Vector3(
                _position.X + 10f,
                _position.Y + _size.Y - 25f,
                0);

            // Line 251 - Replace 'ActualFontCacheType' with your true font cache class name
            var cache = (ActualFontCacheType)FontCache;
            var font = cache.GetFont("small") ?? cache.GetFont("default");

            SASZombieAssaultTD.Engine.Render.Renderer.DrawString(
                text,
                pos,
                textColor,
                font);
        }

        // ---------------------------------------------------------------------------------------------
        // Status Logic
        // ---------------------------------------------------------------------------------------------

        private string GetStatusText()
        {
            if (_placementInfo == null)
                return "No tower selected";

            if (_placementInfo.CanPlace)
                return "CAN PLACE";

            if (_placementInfo.CanAfford)
                return "INSUFFICIENT FUNDS";

            return "INSUFFICIENT FUNDS";
        }

        private Color GetStatusColor()
        {
            if (_placementInfo.CanPlace)
                return _normalColor;

            if (_placementInfo.CanAfford)
                return _warningColor;

            return _invalidColor;
        }

        protected override void RenderLegacy()
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "NotImplementedException: RenderLegacy() is not implemented in PlacementInfoDisplay.");
        }
    }

    internal class ActualFontCacheType
    {
        internal object GetFont(string v)
        {
            // Return null for null/empty input
            if (string.IsNullOrWhiteSpace(v))
                return null;

            var type = GetType();
            var comparison = StringComparison.OrdinalIgnoreCase;

            // Search properties for a matching font property or a dictionary containing the font
            foreach (var prop in type.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic))
            {
                if (string.Equals(prop.Name, v, comparison) ||
                    string.Equals(prop.Name, v + "Font", comparison) ||
                    string.Equals(prop.Name, "Font" + v, comparison))
                {
                    try
                    {
                        return prop.GetValue(this);
                    }
                    catch
                    {
                        // ignore and continue searching
                    }
                }

                // If property is a non-generic IDictionary, try a lookup
                if (typeof(System.Collections.IDictionary).IsAssignableFrom(prop.PropertyType))
                {
                    var dict = prop.GetValue(this) as System.Collections.IDictionary;
                    if (dict != null && dict.Contains(v))
                        return dict[v];
                }
                else
                {
                    // Look for generic IDictionary<string, T>
                    var genericDictInterface = (Type)null;
                    foreach (var iface in prop.PropertyType.GetInterfaces())
                    {
                        if (iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(System.Collections.Generic.IDictionary<,>))
                        {
                            genericDictInterface = iface;
                            break;
                        }
                    }

                    if (genericDictInterface != null)
                    {
                        var keyType = genericDictInterface.GetGenericArguments()[0];
                        if (keyType == typeof(string))
                        {
                            var dictObj = prop.GetValue(this);
                            if (dictObj != null)
                            {
                                var tryGet = genericDictInterface.GetMethod("TryGetValue");
                                if (tryGet != null)
                                {
                                    var args = new object[] { v, null };
                                    var ok = (bool)tryGet.Invoke(dictObj, args);
                                    if (ok)
                                        return args[1];
                                }
                            }
                        }
                    }
                }
            }

            // Search fields for a matching font field or a dictionary containing the font
            foreach (var field in type.GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic))
            {
                if (string.Equals(field.Name, v, comparison) ||
                    string.Equals(field.Name, v + "Font", comparison) ||
                    string.Equals(field.Name, "Font" + v, comparison))
                {
                    try
                    {
                        return field.GetValue(this);
                    }
                    catch
                    {
                        // ignore and continue searching
                    }
                }

                if (typeof(System.Collections.IDictionary).IsAssignableFrom(field.FieldType))
                {
                    var dict = field.GetValue(this) as System.Collections.IDictionary;
                    if (dict != null && dict.Contains(v))
                        return dict[v];
                }
                else
                {
                    var genericDictInterface = (Type)null;
                    foreach (var iface in field.FieldType.GetInterfaces())
                    {
                        if (iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(System.Collections.Generic.IDictionary<,>))
                        {
                            genericDictInterface = iface;
                            break;
                        }
                    }

                    if (genericDictInterface != null)
                    {
                        var keyType = genericDictInterface.GetGenericArguments()[0];
                        if (keyType == typeof(string))
                        {
                            var dictObj = field.GetValue(this);
                            if (dictObj != null)
                            {
                                var tryGet = genericDictInterface.GetMethod("TryGetValue");
                                if (tryGet != null)
                                {
                                    var args = new object[] { v, null };
                                    var ok = (bool)tryGet.Invoke(dictObj, args);
                                    if (ok)
                                        return args[1];
                                }
                            }
                        }
                    }
                }
            }

            // If nothing found, return null to allow callers to fallback as needed
            return null;
        }
    }
}
