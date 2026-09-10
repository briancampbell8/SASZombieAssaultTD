// ====================================================================================================
//  FILE: WaveDisplay.cs
//  PATH: ./Engine/UI/HUD/
//  MODULE: UI
//
//  ROLE:
//      Provide UI layout, interaction logic, or HUD rendering for game wave tracking components.
// ====================================================================================================
using System;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Render;
using SASZombieAssaultTD.Engine.VectorMath;
using SASZombieAssaultTD.Engine.TextRendering;

namespace SASZombieAssaultTD.Engine.UI.HUD
{
    public class WaveDisplay : ModernHUDComponent
    {
        private const int V = 255;
        private int _currentWave = 1;
        private int _totalWaves = 10;
        private float _progress = 0f;

        private bool _isWaveActive = false;
        private bool _showInterWaveTimer = false;
        private float _interWaveTimer = 0f;

        // Colors
        private Color _normalColor = Color.White;
        private Color _warningColor = Color.Yellow;
        private Color _dangerColor = Color.Red;
        private Color _currentColor;

        // Text - Explicitly qualified as System.Drawing.Font to avoid ambiguities
        private System.Drawing.Font _titleFont;
        private System.Drawing.Font _progressFont;
        private System.Drawing.Font _timerFont;

        private string _wavePrefix = "WAVE";
        private string _progressPrefix = "PROGRESS";

        // Pulse animation
        private float _pulseSpeed = 2f;
        private float _pulseAmount = 0.1f;
        private float _pulseTimer = 0f;
        private bool _isPulsing = false;

        // Events
        public event Action<int> OnWaveStarted;
        public event Action<int> OnWaveCompleted;
        public event Action<float> OnWaveProgress;

        public WaveDisplay()
        {
            _position = new Vector3(50f, 100f, 0);
            _size = new Vector3(250f, 60f, 0);

            LoadFonts();
            UpdateWaveColor();
        }

        private void LoadFonts()
        {
            // Fixed CS0023: Removed '?' because CachedFont is a value-type struct
            var large = FontCache.GetFont("large");
            _titleFont = new System.Drawing.Font(large.Name ?? "Arial", large.Size > 0 ? large.Size : 16);

            var medium = FontCache.GetFont("medium");
            _progressFont = new System.Drawing.Font(medium.Name ?? "Arial", medium.Size > 0 ? medium.Size : 12);

            var small = FontCache.GetFont("small");
            _timerFont = new System.Drawing.Font(small.Name ?? "Arial", small.Size > 0 ? small.Size : 10);
        }

        // ---------------------------------------------------------------------------------------------
        // Public API
        // ---------------------------------------------------------------------------------------------

        public void SetWaveInfo(int waveNumber, int totalWaves, float progress)
        {
            _currentWave = waveNumber;
            _totalWaves = totalWaves;
            _progress = System.Math.Clamp(progress, 0f, 1f);

            _isWaveActive = _progress < 1f;
            UpdateWaveColor();

            if (_isWaveActive)
            {
                StartPulsing();
                OnWaveStarted?.Invoke(waveNumber);
            }
            else
            {
                StopPulsing();
                OnWaveCompleted?.Invoke(waveNumber);
            }
        }

        public void SetProgress(float progress)
        {
            _progress = System.Math.Clamp(progress, 0f, 1f);
            _isWaveActive = _progress < 1f;

            UpdateWaveColor();

            if (_isWaveActive)
                OnWaveProgress?.Invoke(_progress);
        }

        public void SetInterWaveTimer(float timer)
        {
            _interWaveTimer = timer;
            _showInterWaveTimer = timer > 0;
        }

        public void SetNormalColor(Color color)
        {
            _normalColor = color;
            UpdateWaveColor();
        }

        public void SetWarningColor(Color color)
        {
            _warningColor = color;
            UpdateWaveColor();
        }

        public void SetDangerColor(Color color)
        {
            _dangerColor = color;
            UpdateWaveColor();
        }

        public void SetPulsingEnabled(bool enabled)
        {
            _isPulsing = enabled;
            if (!enabled)
                _pulseTimer = 0f;
        }

        public void SetPulseSpeed(float speed)
        {
            _pulseSpeed = System.Math.Max(0.1f, speed);
        }

        public void SetWavePrefix(string prefix) => _wavePrefix = prefix;
        public void SetProgressPrefix(string prefix) => _progressPrefix = prefix;

        // ---------------------------------------------------------------------------------------------
        // Update
        // ---------------------------------------------------------------------------------------------

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            if (_isPulsing)
                UpdatePulseAnimation(deltaTime);

            if (_showInterWaveTimer && _interWaveTimer > 0)
                _interWaveTimer -= deltaTime;
        }

        private void UpdatePulseAnimation(float deltaTime)
        {
            _pulseTimer += deltaTime * _pulseSpeed;
        }

        private void StartPulsing()
        {
            _isPulsing = true;
            _pulseTimer = 0f;
        }

        private void StopPulsing()
        {
            _isPulsing = false;
            _pulseTimer = 0f;
        }

        private void UpdateWaveColor()
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

        // ---------------------------------------------------------------------------------------------
        // Rendering
        // ---------------------------------------------------------------------------------------------

        public override void Render()
        {
            if (!_isVisible)
                return;

            RenderBackground();
            RenderWaveInfo();
            RenderProgressBar();

            if (_showInterWaveTimer)
                RenderInterWaveTimer();

            if (_isPulsing)
                RenderPulseEffect();
        }

        private void RenderBackground()
        {
            Color bg = Color.FromArgb(150, 0, 0, 0);
            Color border = System.Drawing.Color.FromArgb(V, _currentColor.R, _currentColor.G, _currentColor.B);

            Renderer.DrawRectangle(_position.X, _position.Y, _size.X, _size.Y, bg);
            Renderer.DrawRectangle(_position.X, _position.Y, _size.X, _size.Y, border, 2f);
        }

        private void RenderWaveInfo()
        {
            string title = $"{_wavePrefix} {_currentWave}/{_totalWaves}";
            Renderer.DrawString(title, new Vector3(_position.X + 10f, _position.Y + 8f, _position.Z), _currentColor, _titleFont);
        }

        private void RenderProgressBar()
        {
            float fillWidth = (_size.X - 20f) * _progress;
            Color barBg = Color.FromArgb(100, 50, 50, 50);

            // Draw full background track
            Renderer.DrawRectangle(_position.X + 10f, _position.Y + 35f, _size.X - 20f, 12f, barBg);

            // Draw current active progress fill
            if (fillWidth > 0f)
            {
                Renderer.DrawRectangle(_position.X + 10f, _position.Y + 35f, fillWidth, 12f, _currentColor);
            }
        }

        private void RenderInterWaveTimer()
        {
            string timerText = $"NEXT WAVE IN: {_interWaveTimer:F1}s";
            Renderer.DrawString(timerText, new Vector3(_position.X + 10f, _position.Y + _size.Y + 4f, _position.Z), _warningColor, _timerFont);
        }

        private void RenderPulseEffect()
        {
            float pulseScale = 1.0f + (float)System.Math.Sin(_pulseTimer) * _pulseAmount;
            // Diagnostic metadata layer for pulse transforms
        }

        protected override void RenderLegacy()
        {
            // Satisfies abstract base modernisation contract tracking safely
        }
    }
}
