using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.VectorMath;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Achievements
{
    ///<summary>
    ///Achievement list renderer for SAS Zombie Assault TD.
    ///Handles rendering of achievement lists and progress indicators.
    ///</summary>
    public class AchievementListRenderer
    {
        private bool _isVisible = true;
        private Vector3 _position;
        private float _width = 400f;
        private float _height = 300f;
        private List<AchievementItem> _achievements = new();

        ///<summary>
        ///Whether the achievement list is visible.
        ///</summary>
        public bool IsVisible
        {
            get => _isVisible;
            set => _isVisible = value;
        }

        ///<summary>
        ///Position of the achievement list.
        ///</summary>
        public Vector3 Position
        {
            get => _position;
            set => _position = value;
        }

        ///<summary>
        ///Width of the achievement list.
        ///</summary>
        public float Width
        {
            get => _width;
            set => _width = Math.Math.Max(100f, value);
        }

        ///<summary>
        ///Height of the achievement list.
        ///</summary>
        public float Height
        {
            get => _height;
            set => _height = Math.Math.Max(100f, value);
        }

        ///<summary>
        ///Initialize the achievement list renderer.
        ///</summary>
        public void Initialize()
        {
            _position = new Vector3(50f, 50f, 0f);
            LoadAchievements();
        }

        ///<summary>
        ///Render the achievement list.
        ///</summary>
        ///<param name="renderContext">Render context.</param>
        public void Render(IRenderContext renderContext)
        {
            if (!_isVisible) return;

            //Render background
            var backgroundRect = Rectangle.FromPositionAndSize(_position.X, _position.Y, _width, _height);
            renderContext.FillRectangle(backgroundRect, new Color(0, 0, 0, 180));

            //Render border
            renderContext.DrawRectangle(backgroundRect, new Color(255, 255, 255, 255), 2f);

            //Render header
            renderContext.DrawText("Achievements", new Vector3(_position.X + 10f, _position.Y + 10f, 0),
                new Color(255, 255, 255, 255), 16f);

            //Render achievement items
            float yOffset = 40f;
            foreach (var achievement in _achievements)
            {
                RenderAchievement(renderContext, achievement, _position.X + 10f,
                    _position.Y + yOffset);
                yOffset += 30f;
            }
        }

        ///<summary>
        ///Add an achievement to the list.
        ///</summary>
        ///<param name="achievement">Achievement to add.</param>
        public void AddAchievement(AchievementItem achievement)
        {
            if (achievement != null && !_achievements.Contains(achievement))
            {
                _achievements.Add(achievement);
            }
        }

        ///<summary>
        ///Remove an achievement from the list.
        ///</summary>
        ///<param name="achievement">Achievement to remove.</param>
        public void RemoveAchievement(AchievementItem achievement)
        {
            _achievements.Remove(achievement);
        }

        ///<summary>
        ///Clear all achievements.
        ///</summary>
        public void ClearAchievements()
        {
            _achievements.Clear();
        }

        ///<summary>
        ///Load achievements from data source.
        ///</summary>
        private void LoadAchievements()
        {
            //Placeholder - would load from achievement system
            _achievements.Add(new AchievementItem
            {
                Name = "First Blood",
                Description = "Kill your first zombie",
                IsCompleted = true,
                Progress = 1f,
                MaxProgress = 1f
            });
        }

        ///<summary>
        ///Render a single achievement item.
        ///</summary>
        private void RenderAchievement(IRenderContext context, AchievementItem achievement,
            float x, float y)
        {
            var color = achievement.IsCompleted ?
                new Color(0, 255, 0, 255) : new Color(255, 255, 255, 255);

            context.DrawText(achievement.Name, new Vector3(x, y, 0), color, 12f);
            context.DrawText(achievement.Description, new Vector3(x + 10f, y + 15f, 0),
                new Color(200, 200, 200, 255), 10f);

            if (!achievement.IsCompleted)
            {
                //Render progress bar
                var barWidth = 100f;
                var barHeight = 4f;
                var progress = achievement.Progress / achievement.MaxProgress;

                var backgroundBar = Rectangle.FromPositionAndSize(x + 200f, y + 10f, barWidth, barHeight);
                context.DrawRectangle(backgroundBar, new Color(100, 100, 100, 255));

                var progressBar = Rectangle.FromPositionAndSize(x + 200f, y + 10f, barWidth * progress, barHeight);
                context.FillRectangle(progressBar, new Color(255, 255, 0, 255));
            }
        }

        ///<summary>
        ///Update the achievement list.
        ///</summary>
        ///<param name="deltaTime">Time since last frame.</param>
        public void Update(float deltaTime)
        {
            //Update animations, tooltips, etc.
        }

        ///<summary>
        ///Cleanup resources.
        ///</summary>
        public void Cleanup()
        {
            _achievements.Clear();
        }
    }

    ///<summary>
    ///Achievement item data.
    ///</summary>
    public class AchievementItem
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public float Progress { get; set; }
        public float MaxProgress { get; set; }
        public DateTime UnlockDate { get; set; }
    }
}




