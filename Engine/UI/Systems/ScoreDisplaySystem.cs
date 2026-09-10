// ====================================================================================================
//  FILE: ScoreDisplaySystem.cs
//  PATH: ./Engine/UI/Systems/
//  MODULE: UI
//
//  ROLE:
//      Provide UI layout, interaction logic, or HUD rendering.
//
//  RESPONSIBILITIES:
//      - Provide Update() behavior for the UI subsystem.
//      - Provide Render() behavior for the UI subsystem.
//      - Provide Shutdown() behavior for the UI subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using SASZombieAssaultTD.Engine.Systems;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.UI
{
    /// <summary>
    /// Displays the player's current score on screen with optional smooth animations.
    /// </summary>
    public class ScoreDisplaySystem
    {
        private readonly ECSRuntimeEvents _events;

        private readonly List<ScorePopup> _activePopups = new();

        private int _currentScore;
        private int _displayedScore;
        private bool _isAnimating;

        public System.Numerics.Vector3 Position { get; set; } = new(10, 10, 0);
        public string FontName { get; set; } = "Arial";
        public int FontSize { get; set; } = 24;
        public Color TextColor { get; set; } = Color.White;
        public string ScoreFormat { get; set; } = "Score: {0}";
        public bool EnableAnimations { get; set; } = true;
        public float AnimationSpeed { get; set; } = 1000f;
        public float PopupDuration { get; set; } = 2.0f;
        public float PopupSpeed { get; set; } = 50f;

        public ScoreDisplaySystem(ECSRuntimeCore runtime, ECSRuntimeEvents eventBus)
        {
            _events = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _events.Subscribe<KillAttributedEvent>(OnKillAttributed);

            DLogger.Log(LogSubsystems.UI, LogLevel.Info, "ScoreDisplaySystem: Initialized");
        }

        private void OnKillAttributed(KillAttributedEvent killEvent)
        {
            if (killEvent.ScoreAwarded <= 0)
                return;

            UpdateScore(killEvent.ScoreAwarded);

            if (EnableAnimations)
                CreateScorePopup(killEvent.ScoreAwarded);
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
                Position = new System.Numerics.Vector3(Position.X + 100, Position.Y, 0),
                Alpha = 1.0f,
                TimeRemaining = PopupDuration
            });
        }

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
            if (!_isAnimating)
                return;

            _displayedScore = System.Math.Min(
                _displayedScore + (int)(AnimationSpeed * deltaTime),
                _currentScore);

            _isAnimating = _displayedScore < _currentScore;
        }

        private void UpdateScorePopups(float deltaTime)
        {
            for (int i = _activePopups.Count - 1; i >= 0; i--)
            {
                var popup = _activePopups[i];

                popup.TimeRemaining -= deltaTime;
                popup.Position = new System.Numerics.Vector3(
                    popup.Position.X,
                    popup.Position.Y - PopupSpeed * deltaTime,
                    0);

                popup.Alpha = System.Math.Max(0f, popup.TimeRemaining / PopupDuration);

                if (popup.TimeRemaining <= 0f)
                    _activePopups.RemoveAt(i);
            }
        }

        public void Render(D3D11Adapter_Core adapter)
        {
            if (adapter == null)
                throw new ArgumentNullException(nameof(adapter));

            RenderScoreText(adapter);
            RenderScorePopups(adapter);
        }

        private void RenderScoreText(D3D11Adapter_Core adapter)
        {
            adapter.DrawText(
                string.Format(ScoreFormat, _displayedScore),
                Position.X,
                Position.Y,
                FontSize,
                TextColor);
        }

        private void RenderScorePopups(D3D11Adapter_Core adapter)
        {
            if (_activePopups.Count == 0)
                return;

            for (int i = 0; i < _activePopups.Count; i++)
            {
                var popup = _activePopups[i];
                if (popup == null)
                    continue;

                adapter.DrawText(
                    $"+{popup.Score}",
                    popup.Position.X,
                    popup.Position.Y,
                    FontSize - 4,
                    Color.FromArgb((int)(popup.Alpha * 255), TextColor));
            }
        }

        public void Shutdown()
        {
            _events.Unsubscribe<KillAttributedEvent>(OnKillAttributed);
            _activePopups.Clear();

            DLogger.Log(LogSubsystems.UI, LogLevel.Info, "ScoreDisplaySystem: Shutdown complete");
        }
    }

    internal class ScorePopup
    {
        public int Score { get; set; }
        public System.Numerics.Vector3 Position { get; set; }
        public float Alpha { get; set; }
        public float TimeRemaining { get; set; }
    }
}
