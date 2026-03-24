using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.VectorMath;
using System;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.UI
{
    /// <summary>
    /// Displays the player's current score on screen with optional smooth animations.
    /// </summary>
    public class ScoreDisplaySystem
    {
        private readonly EntityManager _entityManager;
        private readonly EventRouter _eventBus;
        private readonly List<ScorePopup> _activePopups = new();

        private int _currentScore;
        private int _displayedScore;
        private bool _isAnimating;

        /// <summary>
        /// Gets or sets the position of the score display on screen.
        /// </summary>
        public Vector3 Position { get; set; } = new(10, 10, 0);

        /// <summary>
        /// Gets or sets the font name for score display.
        /// </summary>
        public string FontName { get; set; } = "Arial";

        /// <summary>
        /// Gets or sets the font size for score display.
        /// </summary>
        public int FontSize { get; set; } = 24;

        /// <summary>
        /// Gets or sets the color for score text.
        /// </summary>
        public Color TextColor { get; set; } = Color.White;

        /// <summary>
        /// Gets or sets the format string for score display.
        /// </summary>
        public string ScoreFormat { get; set; } = "Score: {0}";

        /// <summary>
        /// Gets or sets whether to enable smooth score animations.
        /// </summary>
        public bool EnableAnimations { get; set; } = true;

        /// <summary>
        /// Gets or sets the animation speed for score changes (points per second).
        /// </summary>
        public float AnimationSpeed { get; set; } = 1000f;

        /// <summary>
        /// Gets or sets the duration for score popup animations in seconds.
        /// </summary>
        public float PopupDuration { get; set; } = 2.0f;

        /// <summary>
        /// Gets or sets the upward movement speed for score popups.
        /// </summary>
        public float PopupSpeed { get; set; } = 50f;

        /// <summary>
        /// Initializes a new instance of the ScoreDisplaySystem class.
        /// </summary>
        /// <param name="entityManager">Entity manager for component access</param>
        /// <param name="eventBus">Event router for score change notifications</param>
        public ScoreDisplaySystem(EntityManager entityManager, EventRouter eventBus)
        {
            _entityManager = entityManager ?? throw new ArgumentNullException(nameof(entityManager));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));

            _eventBus.Subscribe<KillAttributedEvent>(OnKillAttributed);
        }

        private void OnKillAttributed(KillAttributedEvent killEvent)
        {
            if (killEvent.ScoreAwarded <= 0) return;

            UpdateScore(killEvent.ScoreAwarded);
            if (EnableAnimations) CreateScorePopup(killEvent.ScoreAwarded);
        }

        private void UpdateScore(int scoreIncrease)
        {
            _currentScore += scoreIncrease;
            if (!EnableAnimations)
            {
                _displayedScore = _currentScore;
                return;
            }
            _isAnimating = true;
        }

        private void CreateScorePopup(int scoreAmount)
        {
            _activePopups.Add(new ScorePopup
            {
                Score = scoreAmount,
                Position = new Vector3(Position.X + 100, Position.Y, 0),
                Alpha = 1.0f,
                TimeRemaining = PopupDuration
            });
        }

        /// <summary>
        /// Updates the score display system state and animations.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since last update in seconds</param>
        public void Update(float deltaTime)
        {
            if (EnableAnimations)
            {
                UpdateScoreAnimation(deltaTime);
                UpdateScorePopups(deltaTime);
            }
        }

        private void UpdateScoreAnimation(float deltaTime)
        {
            if (!_isAnimating) return;

            _displayedScore = System.Math.Min(_displayedScore + (int)(AnimationSpeed * deltaTime), _currentScore);
            _isAnimating = _displayedScore < _currentScore;
        }

        private void UpdateScorePopups(float deltaTime)
        {
            for (int i = _activePopups.Count - 1; i >= 0; i--)
            {
                var popup = _activePopups[i];
                popup.TimeRemaining -= deltaTime;
                popup.Position = new Vector3(popup.Position.X, popup.Position.Y - PopupSpeed * deltaTime, 0);
                popup.Alpha = System.Math.Max(0f, popup.TimeRemaining / PopupDuration);

                if (popup.TimeRemaining <= 0f) _activePopups.RemoveAt(i);
            }
        }

        /// <summary>
        /// Renders the score display and active popups.
        /// </summary>
        /// <param name="context">Render context for drawing operations</param>
        public void Render(IRenderContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            RenderScoreText(context);
            RenderScorePopups(context);
        }

        private void RenderScoreText(IRenderContext context)
        {
            context.DrawText(string.Format(ScoreFormat, _displayedScore), Position.X, Position.Y, FontSize, TextColor);
        }

        private void RenderScorePopups(IRenderContext context)
        {
            foreach (var popup in _activePopups)
            {
                var popupColor = Color.FromArgb((int)(popup.Alpha * 255), (byte)TextColor.R, (byte)TextColor.G, (byte)TextColor.B);
                // TODO: Fix DrawText method signature
                // context.DrawText($"+{popup.Score}", popup.Position.X, popup.Position.Y, popupColor, FontSize - 4);
            }
        }

        /// <summary>
        /// Shuts down the score display system and unsubscribes from events.
        /// </summary>
        public void Shutdown()
        {
            _eventBus.Unsubscribe<KillAttributedEvent>(OnKillAttributed);
            _activePopups.Clear();
        }
    }

    /// <summary>
    /// Represents a score popup animation.
    /// </summary>
    internal class ScorePopup
    {
        public int Score { get; set; }
        public Vector3 Position { get; set; }
        public float Alpha { get; set; }
        public float TimeRemaining { get; set; }
    }
}
