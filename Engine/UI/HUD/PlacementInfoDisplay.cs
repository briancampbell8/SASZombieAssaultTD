using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.VectorMath;
using SASZombieAssaultTD.Engine.Audio;
using SASZombieAssaultTD.Engine.Towers;
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.Navigation;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.UI.HUD
{
    ///<summary>
    ///Placement info display for SAS Zombie Assault TD HUD.
    ///Shows tower placement information and validation feedback.
    ///</summary>
    public class PlacementInfoDisplay : HUDComponent
    {
        private PlacementInfo _placementInfo;
        private float _displayTimer = 0f;
        private new bool _isVisible = false;
        private bool _isTransitioning = false;
        private float _transitionTimer = 0f;
        private float _transitionDuration = 0.2f;

        //Visual properties
        private new Vector3 _position;
        private new Vector3 _size;
        private SASZombieAssaultTD.Engine.Core.Color _validColor = new SASZombieAssaultTD.Engine.Core.Color(0, 255, 0, 180);
        private SASZombieAssaultTD.Engine.Core.Color _invalidColor = new SASZombieAssaultTD.Engine.Core.Color(255, 0, 0, 180);
        private SASZombieAssaultTD.Engine.Core.Color _warningColor = new SASZombieAssaultTD.Engine.Core.Color(255, 255, 0, 180);
        private SASZombieAssaultTD.Engine.Core.Color _normalColor = new SASZombieAssaultTD.Engine.Core.Color(255, 255, 255, 180);
        private SASZombieAssaultTD.Engine.Core.Color _borderColor = new SASZombieAssaultTD.Engine.Core.Color(200, 200, 200, 255);

        //Text properties
        private Font _titleFont;
        private Font _textFont;
        private Font _iconFont;
        private Font _smallFont;

        //Animation properties
        private float _pulseSpeed = 2f;
        private float _pulseAmount = 0.1f;
        private float _pulseTimer = 0f;
        private bool _isPulsing = false;
        private Vector3 basePosition;

        //Events
        public event Action<PlacementInfo> OnPlacementAttempted;
        public event Action<PlacementInfo> OnPlacementConfirmed;
        public event Action<PlacementInfo> OnPlacementCancelled;

        public PlacementInfoDisplay(Vector3 position, Vector3 size)
        {
            _position = position;
            _size = size;
            _normalColor = SASZombieAssaultTD.Engine.Core.Color.Green;
            _validColor = new SASZombieAssaultTD.Engine.Core.Color(0, 255, 0, 180);
            _invalidColor = new SASZombieAssaultTD.Engine.Core.Color(255, 0, 0, 180);
            _warningColor = new SASZombieAssaultTD.Engine.Core.Color(255, 255, 0, 180);
            _normalColor = new SASZombieAssaultTD.Engine.Core.Color(255, 255, 255, 255);

            Initialize();
        }

        ///<summary>
        ///Set placement information.
        ///</summary>
        ///<param name="info">Placement information.</param>
        public void SetPlacementInfo(PlacementInfo info)
        {
            _placementInfo = info;
            _displayTimer = 2f;
            _isVisible = true;
            _isTransitioning = true;
            _transitionTimer = 0f;
            _transitionDuration = 0.2f;

            //Start transition animation
            StartTransition();
        }

        ///<summary>
        ///Hide placement info display.
        ///</summary>
        public void HidePlacementInfo()
        {
            _isVisible = false;
            _isTransitioning = false;
            _displayTimer = 0f;
            _placementInfo = null;
        }

        ///<summary>
        ///Set display position.
        ///</summary>
        ///<param name="position">New position.</param>
        public void SetPosition(Vector3 position)
        {
            _position = position;
        }

        ///<summary>
        ///Set display size.
        ///</summary>
        ///<param name="size">New size.</param>
        public void SetSize(Vector3 size)
        {
            _size = size;
            UpdateHeartPositions();
        }

        ///<summary>
        ///Set valid color.
        ///</summary>
        ///<param name="color">Valid placement color.</param>
        public void SetValidColor(SASZombieAssaultTD.Engine.Core.Color color)
        {
            _validColor = new SASZombieAssaultTD.Engine.Core.Color(color.R, color.G, color.B, color.A);
        }

        ///<summary>
        ///Set invalid color.
        ///</summary>
        ///<param name="color">Invalid placement color.</param>
        public void SetInvalidColor(SASZombieAssaultTD.Engine.Core.Color color)
        {
            _invalidColor = new SASZombieAssaultTD.Engine.Core.Color(color.R, color.G, color.B, color.A);
        }

        ///<summary>
        ///Set warning color.
        ///</summary>
        ///<param name="color">Warning placement color.</param>
        public void SetWarningColor(SASZombieAssaultTD.Engine.Core.Color color)
        {
            _warningColor = new SASZombieAssaultTD.Engine.Core.Color(color.R, color.G, color.B, color.A);
        }

        ///<summary>
        ///Set normal color.
        ///</summary>
        ///<param name="color">Normal placement color.</param>
        public void SetNormalColor(SASZombieAssaultTD.Engine.Core.Color color)
        {
            _normalColor = new SASZombieAssaultTD.Engine.Core.Color(color.R, color.G, color.B, color.A);
        }

        ///<summary>
        ///Set pulse animation properties.
        ///</summary>
        ///<param name="speed">Pulse animation speed.</param>
        ///<param name="amount">Pulse amount.</param>
        public void SetPulseProperties(float speed, float amount)
        {
            _pulseSpeed = System.Math.Max(0.1f, speed);
            _pulseAmount = (float)System.Math.Clamp(amount, 0f, 0.5f);
        }

        ///<summary>
        ///Set text prefix.
        ///</summary>
        ///<param name="prefix">Text prefix.</param>
        public void SetTextPrefix(string prefix)
        {
            //Text prefix would be set here
        }

        ///<summary>
        ///Set text format.
        ///</summary>
        ///<param name="format">Text format.</param>
        public void SetTextFormat(string format)
        {
            //Text format would be set here
        }

        ///<summary>
        ///Set display timer.
        ///</summary>
        ///<param name="seconds">Display duration in seconds.</param>
        public void SetDisplayTimer(float seconds)
        {
            _displayTimer = seconds;
        }

        ///<summary>
        ///Start transition animation.
        ///</summary>
        private void StartTransition()
        {
            _transitionTimer = 0f;
            _isTransitioning = true;
        }

        ///<summary>
        ///Update transition animation.
        ///</summary>
        private void UpdateTransition(float deltaTime)
        {
            _transitionTimer += deltaTime;
            if (_transitionTimer >= _transitionDuration)
            {
                _isTransitioning = false;
                _transitionTimer = 0f;
            }
        }

        ///<summary>
        ///Render the placement info display.
        ///</summary>
        private void RenderPlacementInfo()
        {
            if (!_isVisible || _placementInfo == null) return;

            try
            {
                //Render background
                RenderBackground();

                //Render tower preview
                RenderTowerPreview();

                //Render status
                RenderStatus();

                //Text
                RenderText();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error rendering placement info: {ex.Message}");
            }
        }

        ///<summary>
        ///Render background.
        ///</summary>
        private void RenderBackground()
        {
            var backgroundColor = Color.FromArgb((byte)(255 * GetTransitionProgress()), (byte)_backgroundColor.R, (byte)_backgroundColor.G, (byte)_backgroundColor.B);
            var borderColor = Color.FromArgb((byte)(255 * GetTransitionProgress()), (byte)_borderColor.R, (byte)_borderColor.G, (byte)_borderColor.B);

            //Use Renderer API (explicit float args used intentionally to avoid operator overload assumptions)
            Renderer.DrawRectangle((int)_position.X, (int)_position.Y, (int)_size.X, (int)_size.Y, backgroundColor);
            Renderer.DrawRectangle((int)_position.X, (int)_position.Y, (int)_size.X, (int)_size.Y, borderColor, 2f);
        }

        ///<summary>
        ///Render tower preview.
        ///</summary>
        private void RenderTowerPreview()
        {
            if (_placementInfo == null) return;

            var towerData = _placementInfo.TowerData;
            var towerSize = towerData.Size;
            var towerPosition = _placementInfo.GridPosition;
            var navigationGrid = new NavigationGrid();
            var worldPosition = navigationGrid.GridToWorld(towerPosition);
            var towerColor = _placementInfo.CanPlace ? _validColor : _invalidColor;

            //Calculate tower size for preview (avoid Vector3 operator overloads)
            var previewScale = 0.8f;
            var previewSize = new Vector3(towerSize.X * previewScale, towerSize.Y * previewScale, towerSize.Z * previewScale);
            var previewPosition = new Vector3(worldPosition.X - (previewSize.X / 2f), worldPosition.Y - (previewSize.Y / 2f), worldPosition.Z - (previewSize.Z / 2f));

            //Render tower preview
            if (towerData.Sprite != null)
            {
                //Render placeholder tower using renderer (signature kept as-is)
                Renderer.DrawRectangle(previewPosition, previewPosition, previewSize, towerColor, 1f);
            }
            else
            {
                //Render actual tower sprite
                Renderer.DrawSprite(towerData.Sprite, previewPosition, previewSize, towerColor, 1f);
            }

            //Render tower base
            var baseSize = new Vector3(towerSize.X * 0.8f, towerSize.Y * 0.8f, towerSize.Z * 0.8f);
            var baseColor = Color.FromArgb((byte)100, (byte)towerColor.R, (byte)towerColor.G, (byte)towerColor.B);
            Renderer.DrawRectangle(basePosition, baseSize, baseSize, baseColor, 1f);
        }

        ///<summary>
        ///Render status indicator.
        ///</summary>
        private void RenderStatus()
        {
            var statusText = GetStatusText();
            var statusColor = GetStatusColor();
            var statusTextColor = Color.FromArgb((int)statusColor.R, (int)statusColor.G, (int)statusColor.B, 255);
            var statusPosition = new Vector3(_position.X + 10f, _position.Y + _size.Y - 25f, 0);
            var statusFont = FontCache.GetFont("small") ?? FontCache.GetFont("default");

            Renderer.DrawString(statusText, statusPosition, statusTextColor, statusFont);
        }

        ///<summary>
        ///Get status text based on current state.
        ///</summary>
        private string GetStatusText()
        {
            if (_placementInfo == null) return "No tower selected";

            if (_placementInfo.CanPlace)
            {
                return $"CAN PLACE";
            }
            else if (_placementInfo.CanAfford)
            {
                return $"INSUFFICIENT FUNDS";
            }
            else
            {
                return $"INSUFFICIENT FUNDS";
            }
        }

        ///<summary>
        ///Get status color based on state.
        ///</summary>
        private SASZombieAssaultTD.Engine.Core.Color GetStatusColor()
        {
            if (_placementInfo == null) return _normalColor;

            if (_placementInfo.CanPlace)
            {
                return _validColor;
            }
            else if (_placementInfo.CanAfford)
            {
                return _warningColor;
            }
            else
            {
                return _invalidColor;
            }
        }

        ///<summary>
        ///Get transition progress (0-1).
        ///</summary>
        private float GetTransitionProgress()
        {
            if (!_isTransitioning) return 1f;
            return (float)System.Math.Clamp(_transitionTimer / _transitionDuration, 0f, 1f);
        }

        //Placeholder: kept to satisfy compilation until real implementation exists
        private void UpdateHeartPositions() { }
        private void RenderText() { }
    }
}
