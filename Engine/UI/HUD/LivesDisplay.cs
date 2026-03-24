using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.VectorMath;
using SASZombieAssaultTD.Engine.Math;

namespace SASZombieAssaultTD.Engine.UI.HUD
{
    /// <summary>
    /// Lives display component for SAS Zombie Assault TD HUD.
    /// Shows current player lives with visual feedback for low life situations.
    /// </summary>
    public class LivesDisplay : HUDComponent
    {
        private int _currentLives = 20;
        private int _maxLives = 20;
        private int _previousLives = 20;
        private float _damageFlashTimer = 0f;
        private bool _isLowLives = false;
        private bool _isCriticalLives = false;
        private bool _showDamageFlash = true;
        private float _deltaTime = 0f;

        // Visual properties
        // Removed duplicate _position field - using inherited field from HUDComponent

        public LivesDisplay(Vector3 position)
        {
            _position = position;
        }

        private Color _normalColor = Color.Green;
        private Color _warningColor = Color.Orange;
        private Color _dangerColor = Color.Red;
        private Color _currentColor;

        // Text properties
        private Font _font;
        private string _prefix = "LIVES";
        private string _format = "{0}/{1}";
        private List<HeartIcon> _hearts;

        // Animation properties
        private float _pulseSpeed = 3f;
        private float _pulseAmount = 0.15f;
        private float _pulseTimer = 0f;
        private bool _isPulsing = false;
        private float _shakeAmount = 0f;

        // Events
        public event Action<int> OnLivesChanged;
        public event Action<int> OnLivesWarning;
        public event Action<int> OnLivesDanger;
        public event Action OnLivesLost;
        public event Action OnLivesRestored;

        public LivesDisplay()
        {
            _position = new Vector3(50f, 150f, 0);
            _size = new Vector3(200f, 40f, 0);
            _currentColor = _normalColor;
            _hearts = new List<HeartIcon>();

            // Initialize heart icons
            InitializeHearts();
        }

        /// <summary>
        /// Set lives information.
        /// </summary>
        /// <param name="current">Current lives.</param>
        /// <param name="max">Maximum lives.</param>
        public void SetLives(int current, int max)
        {
            _currentLives = System.Math.Max(0, current);
            _maxLives = System.Math.Max(1, max);

            // Update life status
            _isLowLives = _currentLives <= _maxLives * 0.25f;
            _isCriticalLives = _currentLives <= _maxLives * 0.1f;

            // Update color and effects
            UpdateLivesColor();

            // Trigger damage flash if lives decreased
            if (_showDamageFlash && _damageFlashTimer > 0)
            {
                StartDamageFlash();
            }

            // Trigger events
            OnLivesChanged?.Invoke(_currentLives);

            if (_isLowLives)
            {
                OnLivesWarning?.Invoke(_currentLives);
            }

            if (_isCriticalLives)
            {
                OnLivesDanger?.Invoke(_currentLives);
            }

            if (current < _previousLives)
            {
                OnLivesLost?.Invoke();
            }

            _previousLives = current;

            Console.WriteLine($"Lives updated: {current}/{max}");
        }

        /// <summary>
        /// Add lives.
        /// </summary>
        /// <param name="amount">Amount of lives to add.</param>
        public void AddLives(int amount)
        {
            SetLives(_currentLives + amount, _maxLives);
        }

        /// <summary>
        /// Remove lives.
        /// </summary>
        /// <param name="amount">Amount of lives to remove.</param>
        public void RemoveLives(int amount)
        {
            SetLives(_currentLives - amount, _maxLives);
        }

        /// <summary>
        /// Restore all lives.
        /// </summary>
        public void RestoreAllLives()
        {
            SetLives(_maxLives, _maxLives);
            OnLivesRestored?.Invoke();
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
            UpdateHeartPositions(0f);
        }

        /// <summary>
        /// Set normal color.
        /// </summary>
        /// <param name="color">Normal color.</param>
        public void SetNormalColor(Color color)
        {
            _normalColor = color;
            UpdateLivesColor();
        }

        /// <summary>
        /// Set warning color.
        /// </summary>
        /// <param name="color">Warning color.</param>
        public void SetWarningColor(Color color)
        {
            _warningColor = color;
            UpdateLivesColor();
        }

        /// <summary>
        /// Set danger color.
        /// </summary>
        /// <param name="color">Danger color.</param>
        public void SetDangerColor(Color color)
        {
            _dangerColor = color;
            UpdateLivesColor();
        }

        /// <summary>
        /// Enable or disable damage flash effect.
        /// </summary>
        /// <param name="enabled">Whether to show damage flash.</param>
        public void SetDamageFlashEnabled(bool enabled)
        {
            _showDamageFlash = enabled;
        }

        /// <summary>
        /// Set pulse animation speed.
        /// </summary>
        /// <param name="speed">Pulse speed multiplier.</param>
        public void SetPulseSpeed(float speed)
        {
            _pulseSpeed = System.Math.Max(0.1f, speed);
        }

        /// <summary>
        /// Set pulse animation amount.
        /// </summary>
        /// <param name="amount">Pulse amount.</param>
        public void SetPulseAmount(float amount)
        {
            _pulseAmount = System.Math.Clamp(0f, 0.5f, amount);
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
        /// Enable or disable pulsing animation.
        /// </summary>
        /// <param name="enabled">Whether to enable pulsing.</param>
        public void SetPulsingEnabled(bool enabled)
        {
            _isPulsing = enabled;
            if (!enabled)
            {
                _pulseTimer = 0f;
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            // Load font
            var cachedFont = FontCache.GetFont("large");
            _font = cachedFont != null ? new Font(cachedFont.Name, cachedFont.Size) : new Font("Arial", 12);

            // Set initial values
            UpdateLivesColor();
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            // Update pulse animation
            if (_isPulsing)
            {
                UpdatePulseAnimation(deltaTime);
            }

            // Update damage flash
            if (_damageFlashTimer > 0)
            {
                UpdateDamageFlash(deltaTime);
            }
        }

        public override void Render()
        {
            base.Render();

            // Render background
            RenderBackground();

            // Render hearts
            RenderHearts();

            // Render text
            RenderText();

            // Render effects
            RenderEffects();
        }

        /// <summary>
        /// Initialize heart icons.
        /// </summary>
        private void InitializeHearts()
        {
            _hearts.Clear();

            var heartCount = _maxLives;
            for (int i = 0; i < heartCount; i++)
            {
                var heart = new HeartIcon
                {
                    Index = i,
                    IsFull = i < _currentLives,
                    TargetPosition = Vector3.Zero,
                    CurrentPosition = Vector3.Zero,
                    Scale = 1f,
                    Alpha = 1f
                };

                _hearts.Add(heart);
            }

            UpdateHeartPositions(0f);
        }

        /// <summary>
        /// Update heart positions.
        /// </summary>
        private void UpdateHeartPositions(float deltaTime)
        {
            var heartSize = new Vector3(20f, 20f, 0);
            var spacing = 25f;
            var heartCount = _hearts.Count; // Define heartCount as the number of hearts in _hearts
            var startX = _position.X + (_size.X - (heartCount * spacing + heartSize.X) / 2f);
            var startY = _position.Y + (_size.Y - heartSize.Y) / 2f;

            for (int i = 0; i < _hearts.Count; i++)
            {
                var heart = _hearts[i];
                var targetPosition = new Vector3(startX + (i * spacing), startY, 0);

                heart.CurrentPosition = heart.CurrentPosition;
                heart.TargetPosition = targetPosition;
                heart.Scale = heart.IsFull ? 1f : 0.5f;

                // Animate heart position
                if (heart.CurrentPosition != heart.TargetPosition)
                {
                    var direction = (heart.TargetPosition - heart.CurrentPosition).Normalized;
                    var moveSpeed = 2f;
                    var moveDistance = direction.Length;

                    if (moveDistance > 0.01f)
                    {
                        heart.CurrentPosition += direction * System.MathF.Min(moveSpeed * deltaTime, moveDistance);
                    }
                }
            }
        }

        /// <summary>
        /// Update lives color based on current state.
        /// </summary>
        private void UpdateLivesColor()
        {
            if (_isCriticalLives)
            {
                _currentColor = _dangerColor;
                _pulseAmount = 0.3f; // Stronger pulse for critical lives
                _pulseSpeed = 4f; // Faster pulse for critical lives
            }
            else if (_isLowLives)
            {
                _currentColor = _warningColor;
                _pulseAmount = 0.2f; // Moderate pulse for low lives
                _pulseSpeed = 3f;
            }
            else
            {
                _currentColor = _normalColor;
                _pulseAmount = 0.1f; // Subtle pulse for normal lives
                _pulseSpeed = 2f;
            }
        }

        /// <summary>
        /// Update damage flash animation.
        /// </summary>
        private void UpdateDamageFlash(float deltaTime)
        {
            _damageFlashTimer -= deltaTime;

            if (_damageFlashTimer <= 0)
            {
                _damageFlashTimer = 0f;
            }
        }

        /// <summary>
        /// Start damage flash effect.
        /// </summary>
        private void StartDamageFlash()
        {
            _damageFlashTimer = 0.5f;
        }

        /// <summary>
        /// Update pulse animation.
        /// </summary>
        private void UpdatePulseAnimation(float deltaTime)
        {
            _pulseTimer += deltaTime * _pulseSpeed;
        }

        /// <summary>
        /// Render background.
        /// </summary>
        private void RenderBackground()
        {
            var backgroundColor = new Color(0, 0, 0, 150);
            var borderColor = new Color(_currentColor.R, _currentColor.G, _currentColor.B, 255);

            RenderSystem.DrawRectangle(_position.X, _position.Y, _size.X, _size.Y, backgroundColor);
            RenderSystem.DrawRectangle(_position.X, _position.Y, _size.X, _size.Y, borderColor, 2f);
        }

        /// <summary>
        /// Render heart icons.
        /// </summary>
        private void RenderHearts()
        {
            foreach (var heart in _hearts)
            {
                var heartColor = heart.IsFull ? _currentColor : new Color(_currentColor.R, _currentColor.G, _currentColor.B, (byte)(255 * heart.Alpha));
                var heartSize = new Vector3(20f, 20f, 0) * heart.Scale;
                var heartPosition = heart.CurrentPosition;

                // Render heart
                if (heart.IsFull)
                {
                    // TODO: Implement rendering system
                    // RenderSystem.DrawHeart(heartPosition.X, heartPosition.Y, heartSize, heartColor);
                }
                else
                {
                    // TODO: Implement rendering system
                    // RenderSystem.DrawEmptyHeart(heartPosition.X, heartPosition.Y, heartSize, heartColor);
                }
            }
        }

        /// <summary>
        /// Render text.
        /// </summary>
        private void RenderText()
        {
            var text = $"{_prefix} {string.Format(_format, _currentLives, _maxLives)}";
            var textColor = new Color(_currentColor.R, _currentColor.G, _currentColor.B, 255);

            // TODO: Replace with proper renderContext parameter
            // RenderSystem.DrawString(text, _position.X + 10f, _position.Y + 10f, textColor, _font);
        }

        /// <summary>
        /// Render visual effects.
        /// </summary>
        private void RenderEffects()
        {
            // Render damage flash overlay
            if (_damageFlashTimer > 0)
            {
                var flashAlpha = (_damageFlashTimer / 0.5f) * 0.5f;
                var flashColor = new Color(255, 0, 0, (byte)(255 * flashAlpha));
                // TODO: Replace with proper renderContext parameter
                // RenderSystem.DrawRectangle(_position.X, _position.Y, _size.X, _size.Y, flashColor);
            }

            // Render pulse effect
            if (_isPulsing)
            {
                var pulse = 1f + (MathF.Sin(_pulseTimer * _pulseSpeed) * _pulseAmount);
                var pulseColor = new Color(_currentColor.R, _currentColor.G, _currentColor.B, (byte)(255 * (0.3f + (MathF.Sin(_pulseTimer * _pulseSpeed * 2f) * 0.3f))));
                // TODO: Replace with proper renderContext parameter
                // RenderSystem.DrawRectangle(_position.X, _position.Y, _size.X, _size.Y, pulseColor, 1f);
            }
        }
    }

    /// <summary>
    /// Heart icon for lives display.
    /// </summary>
    public class HeartIcon
    {
        public int Index { get; set; }
        public bool IsFull { get; set; }
        public Vector3 TargetPosition { get; set; }
        public Vector3 CurrentPosition { get; set; }
        public float Scale { get; set; }
        public float Alpha { get; set; }

        public HeartIcon()
        {
            TargetPosition = Vector3.Zero;
            CurrentPosition = Vector3.Zero;
            Scale = 1f;
            Alpha = 1f;
        }
    }
}
