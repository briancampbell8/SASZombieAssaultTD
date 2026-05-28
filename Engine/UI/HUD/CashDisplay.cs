using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.Resources;
using SASZombieAssaultTD.Engine.VectorMath;
using System;

namespace SASZombieAssaultTD.Engine.UI.HUD
{
    /// <summary>
    /// Static audio system for playing sounds.
    /// </summary>
    public static class AudioSystem
    {
        public static void PlaySoundEffect(string soundName)
        {
            // Placeholder for audio playback
            System.Diagnostics.Debug.WriteLine($"Playing sound: {soundName}");
        }
    }

    /// <summary>
    /// Static rendering system for drawing operations.
    /// </summary>
    public static class RenderSystem
    {
        private static object TheContainingType;
        private static object TheContainingMember;

        public static void DrawRectangle(float x, float y, float width, float height, Color color)
        {
            // Placeholder for rectangle drawing
        }

        public static void DrawRectangle(float x, float y, float width, float height, Color color, float borderWidth)
        {
            // Placeholder for bordered rectangle drawing
        }

        public static void DrawString(string text, Vector3 position, Color color, SASZombieAssaultTD.Engine.Rendering.Font font, Vector3 size)
        {
            // Placeholder for text drawing
        }

        public static void DrawArrow(float x1, float y1, float x2, float y2, Color color, float width)
        {
            // Placeholder for arrow drawing
        }

        internal static void DrawSprite(object iconSprite, Vector3 vector3, float v, Color white)
        {
            NotImplementedGuard.Hit("NOT_IMPLEMENTED");

            throw new NotImplementedException();
        }

        internal static void DrawRectangle(Vector3 scaledPosition, Vector3 scaledSize, Color pulseColor, float v)
        {
            NotImplementedGuard.Hit("NOT_IMPLEMENTED");
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// Cash display component for SAS Zombie Assault TD HUD.
    /// Shows current player cash with animations and effects.
    /// </summary>
    public class CashDisplay : HUDComponent
    {
        private int _currentCash = 0;
        private int _previousCash = 0;
        private int _targetCash = 0;
        private float _animationTimer = 0f;
        private float _displayTimer = 0f;
        private bool _isAnimating = false;
        private bool _showChangeEffect = true;

        // Animation properties
        private float _animationSpeed = 2f;
        private float _pulseSpeed = 3f;
        private float _pulseAmount = 0.2f;
        private float _changeEffectDuration = 1f;

        // Visual properties
        private Color _normalColor = Color.Yellow;
        private Color _warningColor = Color.Orange;
        private Color _dangerColor = Color.Red;
        private Color _currentColor;
        private float _baseScale = 1f;

        // Text properties
        private SASZombieAssaultTD.Engine.Rendering.Font _font;
        private string _prefix = "$";
        private string _format = "{0:N0}";

        // Events
        public event Action<int> OnCashChanged;
        public event Action<int> OnCashWarning;
        public event Action<int> OnCashDanger;

        /// <summary>
        /// Set the cash amount.
        /// </summary>
        /// <param name="amount">New cash amount.</param>
        public void SetAmount(int amount)
        {
            if (amount < 0)
            {
                amount = 0;
            }

            _previousCash = _currentCash;
            _currentCash = amount;
            _targetCash = amount;

            // Trigger change effect
            if (_showChangeEffect && System.Math.Abs(amount - _previousCash) > 0)
            {
                StartChangeAnimation();
            }

            // Update color based on cash level
            UpdateCashColor();

            // Trigger events
            OnCashChanged?.Invoke(amount);

            if (amount < 100)
            {
                OnCashDanger?.Invoke(amount);
            }
            else if (amount < 300)
            {
                OnCashWarning?.Invoke(amount);
            }

            System.Diagnostics.Debug.WriteLine($"Cash updated: ${amount}");
        }

        /// <summary>
        /// Set display position.
        /// </summary>
        /// <param name="position">New position.</param>
        public void SetPosition(Vector3 position)
        {
            _position = position;
        }

        /// <summary>
        /// Set display size.
        /// </summary>
        /// <param name="size">New size.</param>
        public void SetSize(Vector3 size)
        {
            _size = size;
        }

        /// <summary>
        /// Set text prefix.
        /// </summary>
        /// <param name="prefix">Text prefix.</param>
        public void SetPrefix(string prefix)
        {
            _prefix = prefix;
        }

        /// <summary>
        /// Set text format.
        /// </summary>
        /// <param name="format">Text format string.</param>
        public void SetFormat(string format)
        {
            _format = format;
        }

        /// <summary>
        /// Set normal color.
        /// </summary>
        /// <param name="color">Normal color.</param>
        public void SetNormalColor(Color color)
        {
            _normalColor = color;
            UpdateCashColor();
        }

        /// <summary>
        /// Set warning color.
        /// </summary>
        /// <param name="color">Warning color.</param>
        public void SetWarningColor(Color color)
        {
            _warningColor = color;
            UpdateCashColor();
        }

        /// <summary>
        /// Set danger color.
        /// </summary>
        /// <param name="color">Danger color.</param>
        public void SetDangerColor(Color color)
        {
            _dangerColor = color;
            UpdateCashColor();
        }

        /// <summary>
        /// Enable or disable change effects.
        /// </summary>
        /// <param name="enabled">Whether to show change effects.</param>
        public void SetChangeEffectsEnabled(bool enabled)
        {
            _showChangeEffect = enabled;
        }

        /// <summary>
        /// Set animation speed.
        /// </summary>
        /// <param name="speed">Animation speed multiplier.</param>
        public void SetAnimationSpeed(float speed)
        {
            _animationSpeed = System.Math.Max(0.1f, speed);
        }

        /// <summary>
        /// Set visibility of the cash display.
        /// </summary>
        /// <param name="visible">Whether the display should be visible.</param>
        public void SetVisibility(bool visible)
        {
            IsVisible = visible;
        }

        public override void Initialize()
        {
            base.Initialize();

            // Set initial position and size
            _position = new Vector3(50f, 50f, 0);
            _size = new Vector3(200f, 40f, 0);

            // Load font
            var cachedFont = SASZombieAssaultTD.Engine.Rendering.FontCache.GetFont("large")
                ?? SASZombieAssaultTD.Engine.Rendering.FontCache.GetFont("default");
            // TODO: Cannot cast CachedFont to Font
            // _font = (SASZombieAssaultTD.Engine.Rendering.Font)cachedFont;
            _font = default(SASZombieAssaultTD.Engine.Rendering.Font);

            // Set initial values
            UpdateCashColor();
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            // Update animations
            if (_isAnimating)
            {
                UpdateChangeAnimation(deltaTime);
            }

            // Update display timer
            if (_displayTimer > 0)
            {
                _displayTimer -= deltaTime;
            }
        }

        public override void Render()
        {
            base.Render();

            // Render background
            RenderBackground();

            // Render text
            RenderText();

            // Render effects
            if (_isAnimating)
            {
                RenderChangeEffect();
            }
        }

        /// <summary>
        /// Start cash change animation.
        /// </summary>
        private void StartChangeAnimation()
        {
            _isAnimating = true;
            _animationTimer = 0f;
            _displayTimer = _changeEffectDuration;
            _targetCash = _currentCash;

            // Play cash change sound
            if (_currentCash > _previousCash)
            {
                AudioSystem.PlaySoundEffect("cash_increase");
            }
            else if (_currentCash < _previousCash)
            {
                AudioSystem.PlaySoundEffect("cash_decrease");
            }
        }

        /// <summary>
        /// Update cash change animation.
        /// </summary>
        private void UpdateChangeAnimation(float deltaTime)
        {
            _animationTimer += deltaTime * _animationSpeed;

            // Smooth interpolation to target cash
            var progress = System.Math.Min(1f, _animationTimer / _changeEffectDuration);
            var animatedCash = (int)(_previousCash + (_targetCash - _previousCash) * progress);

            if (animatedCash != _currentCash)
            {
                _currentCash = animatedCash;
            }
            else
            {
                _isAnimating = false;
                _animationTimer = 0f;
            }
        }

        /// <summary>
        /// Update cash color based on amount.
        /// </summary>
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

        /// <summary>
        /// Render background.
        /// </summary>
        private void RenderBackground()
        {
            var backgroundColor = new Color(0, 0, 0, 180);
            var borderColor = new Color(_currentColor.R, _currentColor.G, _currentColor.B, 255);

            RenderSystem.DrawRectangle(_position.X, _position.Y, _size.X, _size.Y, backgroundColor);
            RenderSystem.DrawRectangle(_position.X, _position.Y, _size.X, _size.Y, borderColor, 2f);
        }

        /// <summary>
        /// Render cash text.
        /// </summary>
        private void RenderText()
        {
            var text = $"{_prefix}{string.Format(_format, _currentCash)}";
            var textColor = new Color(_currentColor.R, _currentColor.G, _currentColor.B, 255);

            // Add pulse effect for low cash
            if (_currentCash < 100)
            {
                var pulse = 1f + (MathF.Sin(_displayTimer * _pulseSpeed) * _pulseAmount);
                var scale = _baseScale * pulse;
                var scaledSize = _size * scale;
                var scaledPosition = _position + (_size - scaledSize) * 0.5f;

                RenderSystem.DrawString(text, scaledPosition, textColor, _font, scaledSize);
            }
            else
            {
                RenderSystem.DrawString(text, _position, textColor, _font, _size);
            }
        }

        /// <summary>
        /// Render change effect.
        /// </summary>
        private void RenderChangeEffect()
        {
            if (_currentCash > _previousCash)
            {
                RenderIncreaseEffect();
            }
            else if (_currentCash < _previousCash)
            {
                RenderDecreaseEffect();
            }
        }

        /// <summary>
        /// Render cash increase effect.
        /// </summary>
        private void RenderIncreaseEffect()
        {
            var progress = _animationTimer / _changeEffectDuration;
            var effectAlpha = (1f - progress) * 0.5f;
            var effectColor = new Color(0, 255, 0, (byte)(255 * effectAlpha));

            // Render upward arrows
            var arrowCount = 3;
            for (int i = 0; i < arrowCount; i++)
            {
                var arrowX = _position.X + _size.X * (0.3f + (i * 0.2f));
                var arrowY = _position.Y + _size.Y - (progress * 20f) - (i * 10f);
                var arrowSize = new Vector3(10f, 15f, 0);

                RenderSystem.DrawArrow(arrowX, arrowY, arrowX, arrowY - 15f, effectColor, 2f);
            }
        }

        /// <summary>
        /// Render cash decrease effect.
        /// </summary>
        private void RenderDecreaseEffect()
        {
            var progress = _animationTimer / _changeEffectDuration;
            var effectAlpha = (1f - progress) * 0.5f;
            var effectColor = new Color(255, 0, 0, (byte)(255 * effectAlpha));

            // Render downward arrows
            var arrowCount = 3;
            for (int i = 0; i < arrowCount; i++)
            {
                var arrowX = _position.X + _size.X * (0.3f + (i * 0.2f));
                var arrowY = _position.Y + (progress * 20f) + (i * 10f);
                var arrowSize = new Vector3(10f, 15f, 0);

                RenderSystem.DrawArrow(arrowX, arrowY, arrowX, arrowY + 15f, effectColor, 2f);
            }
        }
    }
}
