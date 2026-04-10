using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.VectorMath;
using System;

namespace SASZombieAssaultTD.Engine.UI.HUD
{
    /// <summary>
    /// Wave display component for SAS Zombie Assault TD HUD.
    /// Shows current wave number, progress, and wave information.
    /// </summary>
    public class WaveDisplay : HUDComponent
    {
        int _currentWave = 1;
        int _totalWaves = 10;
        float _progress;
        bool _isWaveActive;
        bool _showInterWaveTimer;
        float _interWaveTimer;
        float _interWaveDelay = 10f;

        // Visual properties
        new Vector3 _position;
        new Vector3 _size;
        Color _normalColor = Color.White;
        Color _warningColor = Color.Yellow;
        Color _dangerColor = Color.Red;
        Color _currentColor;

        // Text properties
        Font _titleFont;
        Font _progressFont;
        Font _timerFont;
        string _wavePrefix = "WAVE";
        string _progressPrefix = "PROGRESS";

        // Animation properties
        float _pulseSpeed = 2f;
        float _pulseAmount = 0.1f;
        float _pulseTimer;
        bool _isPulsing;

        // Events
        public event Action<int> OnWaveStarted;
        public event Action<int> OnWaveCompleted;
        public event Action<float> OnWaveProgress;

        public WaveDisplay()
        {
            _position = new Vector3(50f, 100f, 0);
            _size = new Vector3(250f, 60f, 0);
            _currentColor = _normalColor;

            // Initialize fonts
            var cachedTitleFont = FontCache.GetFont("large");
            _titleFont = new Font(cachedTitleFont?.Name ?? "Arial", cachedTitleFont?.Size ?? 16);
            var cachedProgressFont = FontCache.GetFont("medium");
            _progressFont = new Font(cachedProgressFont?.Name ?? "Arial", cachedProgressFont?.Size ?? 12);
            var cachedTimerFont = FontCache.GetFont("small");
            _timerFont = new Font(cachedTimerFont?.Name ?? "Arial", cachedTimerFont?.Size ?? 10);
        }

        /// <summary>
        /// Set wave information.
        /// </summary>
        /// <param name="waveNumber">Current wave number.</param>
        /// <param name="totalWaves">Total number of waves.</param>
        /// <param name="progress">Wave progress (0-1).</param>
        public void SetWaveInfo(int waveNumber, int totalWaves, float progress)
        {
            _currentWave = waveNumber;
            _totalWaves = totalWaves;
            _progress = progress;
            _isWaveActive = progress < 1f;

            // Update color based on wave progress
            UpdateWaveColor();

            // Start pulsing if wave is active
            if (_isWaveActive)
            {
                StartPulsing();
            }

            // Trigger events
            if (progress >= 1f)
            {
                OnWaveCompleted?.Invoke(waveNumber);
                StopPulsing();
            }
        }

        /// <summary>
        /// Set wave progress.
        /// </summary>
        /// <param name="progress">Wave progress (0-1).</param>
        public void SetProgress(float progress)
        {
            _progress = System.Math.Clamp(progress, 0f, 1f);
            _isWaveActive = _progress < 1f;

            UpdateWaveColor();

            if (_isWaveActive)
            {
                OnWaveProgress?.Invoke(progress);
            }
        }

        /// <summary>
        /// Set inter-wave timer display.
        /// </summary>
        /// <param name="timer">Time remaining until next wave.</param>
        public void SetInterWaveTimer(float timer)
        {
            _interWaveTimer = timer;
            _showInterWaveTimer = timer > 0;
        }

        /// <summary>
        /// Set display position.
        /// </summary>
        /// <param name="position">New position.</param>
        public void SetPosition(Vector3 position) => _position = position;

        /// <summary>
        /// Set display size.
        /// </summary>
        /// <param name="size">New size.</param>
        public void SetSize(Vector3 size) => _size = size;

        /// <summary>
        /// Set normal color.
        /// </summary>
        /// <param name="color">Normal color.</param>
        public void SetNormalColor(Color color)
        {
            _normalColor = color;
            UpdateWaveColor();
        }

        /// <summary>
        /// Set warning color.
        /// </summary>
        /// <param name="color">Warning color.</param>
        public void SetWarningColor(Color color)
        {
            _warningColor = color;
            UpdateWaveColor();
        }

        /// <summary>
        /// Set danger color.
        /// </summary>
        /// <param name="color">Danger color.</param>
        public void SetDangerColor(Color color)
        {
            _dangerColor = color;
            UpdateWaveColor();
        }

        /// <summary>
        /// Enable or disable pulsing effect.
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

        /// <summary>
        /// Set pulse speed.
        /// </summary>
        /// <param name="speed">Pulse speed multiplier.</param>
        public void SetPulseSpeed(float speed) => _pulseSpeed = System.Math.Max(0.1f, speed);

        /// <summary>
        /// Set wave prefix text.
        /// </summary>
        /// <param name="prefix">Wave prefix text.</param>
        public void SetWavePrefix(string prefix) => _wavePrefix = prefix;

        /// <summary>
        /// Set progress prefix text.
        /// </summary>
        /// <param name="prefix">Progress prefix text.</param>
        public void SetProgressPrefix(string prefix) => _progressPrefix = prefix;

        public override void Initialize()
        {
            base.Initialize();

            // Load fonts
            var cachedLargeFont = FontCache.GetFont("large");
            _titleFont = new Font(cachedLargeFont?.Name ?? "Arial", cachedLargeFont?.Size ?? 16);
            var cachedMediumFont = FontCache.GetFont("medium");
            _progressFont = new Font(cachedMediumFont?.Name ?? "Arial", cachedMediumFont?.Size ?? 12);
            var cachedSmallFont = FontCache.GetFont("small");
            _timerFont = new Font(cachedSmallFont?.Name ?? "Arial", cachedSmallFont?.Size ?? 10);

            // Set initial values
            UpdateWaveColor();
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            // Update pulse animation
            if (_isPulsing)
            {
                UpdatePulseAnimation(deltaTime);
            }

            // Update inter-wave timer
            if (_showInterWaveTimer && _interWaveTimer > 0)
            {
                _interWaveTimer -= deltaTime;
            }
        }

        public override void Render()
        {
            base.Render();

            // Render background
            RenderBackground();

            // Render wave info
            RenderWaveInfo();

            // Render progress bar
            RenderProgressBar();

            // Render inter-wave timer
            if (_showInterWaveTimer)
            {
                RenderInterWaveTimer();
            }

            // Render pulse effect
            if (_isPulsing)
            {
                RenderPulseEffect();
            }
        }

        /// <summary>
        /// Start pulsing animation.
        /// </summary>
        void StartPulsing()
        {
            _isPulsing = true;
            _pulseTimer = 0f;
        }

        /// <summary>
        /// Stop pulsing animation.
        /// </summary>
        void StopPulsing()
        {
            _isPulsing = false;
            _pulseTimer = 0f;
        }

        /// <summary>
        /// Update pulse animation.
        /// </summary>
        void UpdatePulseAnimation(float deltaTime) => _pulseTimer += deltaTime * _pulseSpeed;

        /// <summary>
        /// Update wave color based on state.
        /// </summary>
        void UpdateWaveColor()
        {
            if (!_isWaveActive)
            {
                _currentColor = _normalColor;
            }
            else if (_progress < 0.25f)
            {
                _currentColor = _dangerColor;
            }
            else if (_progress < 0.75f)
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
        void RenderBackground()
        {
            var backgroundColor = new Color(0, 0, 0, 150);
            var borderColor = new Color(_currentColor.R, _currentColor.G, _currentColor.B, 255);

            RenderSystem.DrawRectangle(_position.X, _position.Y, _size.X, _size.Y, backgroundColor);
            RenderSystem.DrawRectangle(_position.X, _position.Y, _size.X, _size.Y, borderColor, 2f);
        }

        /// <summary>
        /// Render wave information.
        /// </summary>
        void RenderWaveInfo()
        {
            var titleText = $"{_wavePrefix} {_currentWave}/{_totalWaves}";
            var titleColor = new Color(_currentColor.R, _currentColor.G, _currentColor.B, 255);

            var titlePosition = new Vector3(_position.X + 10f, _position.Y + 10f, 0f);
            RenderSystem.DrawString(titleText, titlePosition, titleColor, _titleFont, new Vector3(12f, 12f, 0f));

            // Add wave status indicator
            var statusText = _isWaveActive ? "IN PROGRESS" : "WAITING";
            var statusColor = _isWaveActive ? Color.Green : Color.Gray;
            var statusPosition = new Vector3(_position.X + 10f, _position.Y + 35f, 0f);
            RenderSystem.DrawString(statusText, statusPosition, statusColor, _timerFont, new Vector3(10f, 10f, 0f));
        }

        /// <summary>
        /// Render progress bar.
        /// </summary>
        void RenderProgressBar()
        {
            var barWidth = _size.X - 20f;
            var barHeight = 8f;
            var barX = _position.X + 10f;
            var barY = _position.Y + _size.Y - 20f;

            // Background
            var backgroundColor = new Color(50, 50, 50, 200);
            RenderSystem.DrawRectangle(barX, barY, barWidth, barHeight, backgroundColor);

            // Progress fill
            var progressWidth = barWidth * _progress;
            var progressColor = _currentColor;
            RenderSystem.DrawRectangle(barX, barY, progressWidth, barHeight, progressColor);

            // Progress text
            var progressText = $"{_progressPrefix}: {(int)(_progress * 100)}%";
            var progressColorBytes = new Color((byte)_currentColor.R, (byte)_currentColor.G, (byte)_currentColor.B, 255);
            RenderSystem.DrawString(progressText, new Vector3(barX + 5f, barY - 2f, 0), progressColorBytes, new Font("Arial", 10f), new Vector3(10f, 10f, 0f));
        }

        /// <summary>
        /// Render inter-wave timer.
        /// </summary>
        void RenderInterWaveTimer()
        {
            var timerText = $"Next wave in: {_interWaveTimer:F0}s";
            var timerColor = Color.Yellow;

            RenderSystem.DrawString(timerText, new Vector3(_position.X + 10f, _position.Y + _size.Y + 5f, 0), timerColor, new Font("Arial", 10f), new Vector3(10f, 10f, 0f));
        }

        /// <summary>
        /// Render pulse effect.
        /// </summary>
        void RenderPulseEffect()
        {
            var pulse = 1f + (MathF.Sin(_pulseTimer) * _pulseAmount);
            var pulseScale = pulse;

            var scaledSize = _size * pulseScale;
            var scaledPosition = _position + (_size - scaledSize) * 0.5f;

            var pulseColor = new Color(_currentColor.R, _currentColor.G, _currentColor.B, (byte)(255 * (0.3f + (MathF.Sin(_pulseTimer * 2f) * 0.3f))));

            RenderSystem.DrawRectangle(scaledPosition, scaledSize, pulseColor, 1f);
        }
    }
}