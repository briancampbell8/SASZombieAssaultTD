// =====================================================================================================
//  FILE: CashDisplayAnimation.cs
//  PATH: Engine/UI/HUD/CashDisplay/CashDisplayAnimation.cs
//  PROGRAM: HUD Cash Display Animation
//
//  ROLE:
//      Implements the deterministic animation program used by the modern Cash Display HUD module.
//      This program encapsulates all timing, interpolation, and pulse-effect logic triggered when
//      the player's cash value changes.
//
//  RESPONSIBILITIES:
//      - Maintain animation state (timers, previous/target values, active flags).
//      - Compute smooth interpolation between cash values over time.
//      - Generate pulse/attention effects for low-cash warning states.
//      - Expose deterministic update calls consumed by ModernCashDisplay.
//      - Report animation completion state.
//
//  NON-RESPONSIBILITIES:
//      - Rendering or widget manipulation (handled by ModernCashDisplay).
//      - Color threshold evaluation (handled by CashDisplayColor).
//      - Text formatting or prefix logic (handled by CashDisplayFormatter).
//      - Sound routing or audio playback (handled by CashDisplaySound).
//      - Layout, positioning, or sizing of widgets (handled by CashDisplayLayout).
//
//  ARCHITECTURAL NOTES:
//      - This is a NEW PROGRAM module: fully isolated, no legacy dependencies.
//      - ModernCashDisplay delegates all animation behavior to this program.
//      - Ensures deterministic, frame-consistent animation behavior across the HUD pipeline.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.UI.HUD.CashDisplay
{
    internal class CashDisplayAnimation
    {
        // Animation state
        private bool _isAnimating = false;
        private float _animationTimer = 0f;
        private float _changeEffectDuration = 0.25f;

        // Cash interpolation
        private int _previousCash = 0;
        private int _targetCash = 0;

        // Pulse effect (for low cash warning)
        private float _pulseTimer = 0f;
        private float _pulseSpeed = 6f;
        private float _pulseAmount = 0.15f;

        // Public properties
        public bool IsAnimating => _isAnimating;

        public float PulseScale { get; private set; } = 1f;

        public int DisplayCash { get; private set; }

        // Start a new animation
        public void StartChangeAnimation(int previousCash, int currentCash)
        {
            _previousCash = previousCash;
            _targetCash = currentCash;

            _animationTimer = 0f;
            _isAnimating = true;
        }

        // Update animation each frame
        public void Update(float deltaTime, bool lowCashWarning)
        {
            if (_isAnimating)
            {
                _animationTimer += deltaTime;

                float t = _animationTimer / _changeEffectDuration;
                if (t >= 1f)
                {
                    t = 1f;
                    _isAnimating = false;
                }

                // Smooth interpolation
                DisplayCash = (int)(_previousCash + ((_targetCash - _previousCash) * t));
            }
            else
            {
                // No animation active — display target cash directly
                DisplayCash = _targetCash;
            }

            // Pulse effect for low cash
            if (lowCashWarning)
            {
                _pulseTimer += deltaTime * _pulseSpeed;
                PulseScale = 1f + (System.MathF.Sin(_pulseTimer) * _pulseAmount);
            }
            else
            {
                PulseScale = 1f;
                _pulseTimer = 0f;
            }
        }

        // Force-set cash instantly (no animation)
        public void SetInstant(int cash)
        {
            _previousCash = cash;
            _targetCash = cash;
            DisplayCash = cash;

            _animationTimer = 0f;
            _isAnimating = false;
            PulseScale = 1f;
        }
    }
}
