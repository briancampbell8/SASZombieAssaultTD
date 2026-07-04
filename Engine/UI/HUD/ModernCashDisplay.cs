// ====================================================================================================
//  FILE: ModernCashDisplay.cs
//  PATH: Engine/UI/HUD/
//  MODULE: UI/HUD Components (Modern Cash Display)
//
//  ROLE:
//      Displays the player's cash using modern P80 UI widgets while preserving legacy behavior.
//
//  RESPONSIBILITIES:
//      - Update and animate cash value changes.
//      - Present visual feedback for cash changes using P80 UIText and UIPanel widgets.
//      - Maintain API compatibility with legacy CashDisplay where feasible.
//
//  NON-RESPONSIBILITIES:
//      - Low-level audio/timing systems (those are provided by other subsystems).
//
//  ARCHITECTURAL NOTES:
//      - Prefer composition and minimal allocations in per-frame code.
// ====================================================================================================

using System;
using SASZombieAssaultTD.Engine.VectorMath;
using SASZombieAssaultTD.Engine.UI.Widgets;
using System.Drawing;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.UI.HUD
{
    ///<summary>
    ///P80 UI/HUD Rendering Modernization - Modern cash display using P80 UI widgets.
    ///Demonstrates migration from legacy HUD rendering to P80 UI system.
    ///</summary>
    public class ModernCashDisplay : ModernHUDComponent
    {
        private int _currentCash = 0;
        private int _previousCash = 0;
        private int _targetCash = 0;
        private float _animationTimer = 0f;
        private float _displayTimer = 0f;
        private bool _isAnimating = false;
        private bool _showChangeEffect = true;

        //Animation properties
        private float _animationSpeed = 2f;
        private float _pulseSpeed = 3f;
        private float _pulseAmount = 0.2f;
        private float _changeEffectDuration = 1f;

        //Visual properties
        private Color _normalColor = Color.Yellow;
        private Color _warningColor = Color.Orange;
        private Color _dangerColor = Color.Red;
        private Color _currentColor;

        //Text properties
        private string _prefix = "$";
        private string _format = "{0:N0}";

        //P80 UI Widgets
        private UIText _cashTextWidget;
        private UIPanel _backgroundPanel;

        //Events
        public event Action<int> OnCashChanged;
        public event Action<int> OnCashWarning;
        public event Action<int> OnCashDanger;

        ///<summary>
        ///Set the cash amount.
        ///</summary>
        public void SetAmount(int amount)
        {
            if (amount < 0)
            {
                amount = 0;
            }

            _previousCash = _currentCash;
            _currentCash = amount;
            _targetCash = amount;

            //Trigger change effect
            if (_showChangeEffect && System.Math.Abs(amount - _previousCash) > 0)
            {
                StartChangeAnimation();
            }

            //Update color based on cash level
            UpdateCashColor();

            //Update P80 widgets
            UpdateCashTextWidget();

            //Trigger events
            OnCashChanged?.Invoke(amount);

            if (amount < 100)
            {
                OnCashDanger?.Invoke(amount);
            }
            else if (amount < 300)
            {
                OnCashWarning?.Invoke(amount);
            }

            System.Diagnostics.Debug.WriteLine($"ModernCashDisplay: Cash updated to ${amount}");
        }

        ///<summary>
        ///Set display position.
        ///</summary>
        public void SetPosition(Vector3 position)
        {
            Position = position;
            UpdateWidgetPositions();
        }

        ///<summary>
        ///Set display size.
        ///</summary>
        public void SetSize(Vector3 size)
        {
            Size = size;
            UpdateWidgetSizes();
        }

        ///<summary>
        ///Set text prefix.
        ///</summary>
        public void SetPrefix(string prefix)
        {
            _prefix = prefix;
            UpdateCashTextWidget();
        }

        ///<summary>
        ///Set text format.
        ///</summary>
        public void SetFormat(string format)
        {
            _format = format;
            UpdateCashTextWidget();
        }

        ///<summary>
        ///Set normal color.
        ///</summary>
        public void SetNormalColor(Color color)
        {
            _normalColor = color;
            UpdateCashColor();
            UpdateWidgetColors();
        }

        ///<summary>
        ///Set warning color.
        ///</summary>
        public void SetWarningColor(Color color)
        {
            _warningColor = color;
            UpdateCashColor();
            UpdateWidgetColors();
        }

        ///<summary>
        ///Set danger color.
        ///</summary>
        public void SetDangerColor(Color color)
        {
            _dangerColor = color;
            UpdateCashColor();
            UpdateWidgetColors();
        }

        ///<summary>
        ///Enable or disable change effects.
        ///</summary>
        public void SetChangeEffectsEnabled(bool enabled)
        {
            _showChangeEffect = enabled;
        }

        ///<summary>
        ///Set animation speed.
        ///</summary>
        public void SetAnimationSpeed(float speed)
        {
            _animationSpeed = System.Math.Max(0.1f, speed);
        }

        ///<summary>
        ///Initialize P80 UI widgets for cash display.
        ///</summary>
        protected override void InitializeP80Widgets()
        {
            base.InitializeP80Widgets();

            //Set initial position and size
            _position = new Vector3(50f, 50f, 0);
            _size = new Vector3(200f, 40f, 0);

            //Create background panel
            _backgroundPanel = CreatePanelWidget(
                "background",
                Color.FromArgb(180, 0, 0, 0),
                _position,
                _size
            );
            _backgroundPanel.SetBorder(_currentColor, 2f);

            //Create cash text widget
            _cashTextWidget = CreateTextWidget(
                "cashText",
                $"{_prefix}{string.Format(_format, _currentCash)}",
                _position,
                _size
            );
            _cashTextWidget.Color = _currentColor;
            _cashTextWidget.FontSize = 16f;
            _cashTextWidget.Alignment = ContentAlignment.MiddleCenter;

            //Set initial values
            UpdateCashColor();

            System.Diagnostics.Debug.WriteLine("ModernCashDisplay: P80 widgets initialized");
        }

        ///<summary>
        ///Update the modern cash display.
        ///</summary>
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            //Update animations
            if (_isAnimating)
            {
                UpdateChangeAnimation(deltaTime);
            }

            //Update display timer
            if (_displayTimer > 0)
            {
                _displayTimer -= deltaTime;
            }

            //Update pulse effect for low cash
            if (_currentCash < 100 && _cashTextWidget != null)
            {
                var pulse = 1f + (MathF.Sin(_displayTimer * _pulseSpeed) * _pulseAmount);
                _cashTextWidget.FontSize = 16f * pulse;
            }
        }

        ///<summary>
        ///Start cash change animation.
        ///</summary>
        private void StartChangeAnimation()
        {
            _isAnimating = true;
            _animationTimer = 0f;
            _displayTimer = _changeEffectDuration;
            _targetCash = _currentCash;

            //Play cash change sound (placeholder)
            if (_currentCash > _previousCash)
            {
                System.Diagnostics.Debug.WriteLine("ModernCashDisplay: Playing cash_increase sound");
            }
            else if (_currentCash < _previousCash)
            {
                System.Diagnostics.Debug.WriteLine("ModernCashDisplay: Playing cash_decrease sound");
            }
        }

        ///<summary>
        ///Update cash change animation.
        ///</summary>
        private void UpdateChangeAnimation(float deltaTime)
        {
            _animationTimer += deltaTime * _animationSpeed;

            //Smooth interpolation to target cash
            var progress = System.Math.Min(1f, _animationTimer / _changeEffectDuration);
            var animatedCash = (int)(_previousCash + (_targetCash - _previousCash) * progress);

            if (animatedCash != _currentCash)
            {
                _currentCash = animatedCash;
                UpdateCashTextWidget();
            }
            else
            {
                _isAnimating = false;
                _animationTimer = 0f;
            }
        }

        ///<summary>
        ///Update cash color based on amount.
        ///</summary>
        private void UpdateCashColor()
        {
            if (_currentCash < 100)
            {
                _currentColor = _dangerColor;
            }
            else if (_currentCash < 300)
            {
                _currentColor = _warningColor;
            }
            else
            {
                _currentColor = _normalColor;
            }
        }

        ///<summary>
        ///Update cash text widget content.
        ///</summary>
        private void UpdateCashTextWidget()
        {
            if (_cashTextWidget != null)
            {
                var text = $"{_prefix}{string.Format(_format, _currentCash)}";
                _cashTextWidget.Text = text;
                //TODO: Cannot assign Engine.Core.Color to System.Drawing.Color
                //_cashTextWidget.Color = _currentColor;
            }
        }

        ///<summary>
        ///Update widget positions based on component position.
        ///</summary>
        private void UpdateWidgetPositions()
        {
            if (_backgroundPanel != null)
            {
                //TODO: PointF doesn't have a 2-argument constructor
                //_backgroundPanel.Position = new PointF(_position.X, _position.Y);
            }
            if (_cashTextWidget != null)
            {
                //TODO: PointF doesn't have a 2-argument constructor
                //_cashTextWidget.Position = new PointF(_position.X, _position.Y);
            }
        }

        ///<summary>
        ///Update widget sizes based on component size.
        ///</summary>
        private void UpdateWidgetSizes()
        {
            if (_backgroundPanel != null)
            {
                _backgroundPanel.Size = new SizeF(_size.X, _size.Y);
            }
            if (_cashTextWidget != null)
            {
                _cashTextWidget.Size = new SizeF(_size.X, _size.Y);
            }
        }

        ///<summary>
        ///Update widget colors based on current cash color.
        ///</summary>
        private void UpdateWidgetColors()
        {
            if (_cashTextWidget != null)
            {
                _cashTextWidget.Color = _currentColor;
            }
            if (_backgroundPanel != null && _backgroundPanel.HasBorder)
            {
                _backgroundPanel.BorderColor = _currentColor;
            }
        }

        ///<summary>
        ///Cleanup the modern cash display.
        ///</summary>
        public new void Cleanup()
        {
            RemoveWidget("background");
            RemoveWidget("cashText");
            base.Cleanup();
            System.Diagnostics.Debug.WriteLine("ModernCashDisplay: Cleaned up");
        }
    }
}
