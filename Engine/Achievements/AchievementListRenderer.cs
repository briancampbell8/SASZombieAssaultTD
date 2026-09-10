// ====================================================================================================
//  FILE: AchievementListRenderer.cs
//  PATH: ./Engine/Achievements/
//  MODULE: Core
//
//  ROLE:
//      Deterministic GPU‑only achievement list renderer.
//
//  RESPONSIBILITIES:
//      - Initialize()
//      - Render()
//      - AddAchievement()
//      - RemoveAchievement()
//      - ClearAchievements()
//      - Update()
//      - Cleanup()
//
//  NOTES:
//      Modernized for GPU pipeline (D3D11Adapter_Core).
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Numerics;
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using SASZombieAssaultTD.Engine.TextureRendering.UI;

namespace SASZombieAssaultTD.Engine.Achievements
{
    public sealed class AchievementListRenderer
    {
        private bool _isVisible = true;
        private Vector2 _position;
        private float _width = 400f;
        private float _height = 300f;

        private readonly List<AchievementItem> _achievements = new();

        public bool IsVisible
        {
            get => _isVisible;
            set => _isVisible = value;
        }

        public Vector2 Position
        {
            get => _position;
            set => _position = value;
        }

        public float Width
        {
            get => _width;
            set => _width = MathF.Max(100f, value);
        }

        public float Height
        {
            get => _height;
            set => _height = MathF.Max(100f, value);
        }

        // -------------------------------------------------------------------------------------------------
        // Initialization
        // -------------------------------------------------------------------------------------------------

        public void Initialize()
        {
            _position = new Vector2(50f, 50f);
            LoadAchievements();
        }

        // -------------------------------------------------------------------------------------------------
        // Render (GPU‑only)
        // -------------------------------------------------------------------------------------------------

        public void Render(D3D11Adapter_Core adapter)
        {
            if (!_isVisible)
                return;

            var backgroundRect = Rectangle.FromPositionAndSize(
                (int)_position.X,
                (int)_position.Y,
                (int)_width,
                (int)_height);

            // Background
            adapter.FillRectangle(backgroundRect, new ColorRGBA(0, 0, 0, 180));

            // Border
            adapter.DrawRectangle(
                backgroundRect,
                new ColorRGBA(255, 255, 255, 255),
                2f);

            // Header
            adapter.DrawText(
                "Achievements",
                new Vector2(_position.X + 10f, _position.Y + 10f),
                16f,
                (ColorRGBA)ColorRGBA.White);

            // Items
            float yOffset = 40f;
            foreach (var achievement in _achievements)
            {
                RenderAchievement(adapter, achievement, _position.X + 10f, _position.Y + yOffset);
                yOffset += 30f;
            }
        }

        // -------------------------------------------------------------------------------------------------
        // Render Single Achievement
        // -------------------------------------------------------------------------------------------------

        private void RenderAchievement(D3D11Adapter_Core adapter, AchievementItem achievement, float x, float y)
        {
            var nameColor = achievement.IsCompleted
                ? (object)new ColorRGBA(0, 255, 0, 255)
                : ColorRGBA.White;

            // Name
            adapter.DrawText(
                achievement.Name,
                new Vector2(x, y),
                12f,
                (ColorRGBA)nameColor);

            // Description
            adapter.DrawText(
                achievement.Description,
                new Vector2(x + 10f, y + 15f),
                10f,
                new ColorRGBA(200, 200, 200, 255));

            if (!achievement.IsCompleted)
            {
                float barWidth = 100f;
                float barHeight = 4f;
                float progress = achievement.Progress / achievement.MaxProgress;

                var backgroundBar = Rectangle.FromPositionAndSize(
                    (int)(x + 200f),
                    (int)(y + 10f),
                    (int)barWidth,
                    (int)barHeight);

                adapter.FillRectangle(backgroundBar, new ColorRGBA(100, 100, 100, 255));

                var progressBar = Rectangle.FromPositionAndSize(
                    (int)(x + 200f),
                    (int)(y + 10f),
                    (int)(barWidth * progress),
                    (int)barHeight);

                adapter.FillRectangle(progressBar, new ColorRGBA(255, 255, 0, 255));
            }
        }

        // -------------------------------------------------------------------------------------------------
        // Achievement Management
        // -------------------------------------------------------------------------------------------------

        public void AddAchievement(AchievementItem achievement)
        {
            if (achievement != null && !_achievements.Contains(achievement))
                _achievements.Add(achievement);
        }

        public void RemoveAchievement(AchievementItem achievement)
        {
            _achievements.Remove(achievement);
        }

        public void ClearAchievements()
        {
            _achievements.Clear();
        }

        private void LoadAchievements()
        {
            _achievements.Add(new AchievementItem
            {
                Name = "First Blood",
                Description = "Kill your first zombie",
                IsCompleted = true,
                Progress = 1f,
                MaxProgress = 1f
            });
        }

        public void Update(float deltaTime)
        {
            // Future animations or tooltip logic
        }

        public void Cleanup()
        {
            _achievements.Clear();
        }
    }

    public sealed class AchievementItem
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public float Progress { get; set; }
        public float MaxProgress { get; set; }
        public DateTime UnlockDate { get; set; }
    }
}
