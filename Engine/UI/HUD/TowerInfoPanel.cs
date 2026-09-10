// ====================================================================================================
//  FILE: TowerInfoPanel.cs
//  PATH: ./Engine/UI/HUD/
//  MODULE: UI
//
//  ROLE:
//      Provide UI layout, interaction logic, or HUD rendering for Tower Information displays.
// ====================================================================================================
using System;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Diagnostics;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Render;
using SASZombieAssaultTD.Engine.Towers;
using SASZombieAssaultTD.Engine.UI.Elements;
using SASZombieAssaultTD.Engine.UI.Styles;
using SASZombieAssaultTD.Engine.VectorMath;
using SASZombieAssaultTD.Engine.TextRendering;

namespace SASZombieAssaultTD.Engine.UI.HUD
{
    public class TowerInfoPanel : ModernHUDComponent
    {
        // Core Fields
        private Tower _currentTower;
        private Tower _previousTower;

        private float _displayTimer = 0f;
        private bool _isTransitioning = false;
        private float _transitionTimer = 0f;
        private float _transitionDuration = 0.3f;

        // Visual Colors (System.Drawing.Color)
        private System.Drawing.Color _backgroundColor = System.Drawing.Color.FromArgb(180, 0, 0, 0);
        private System.Drawing.Color _borderColor = System.Drawing.Color.FromArgb(255, 200, 200, 200);
        private System.Drawing.Color _sectionColor = System.Drawing.Color.White;
        private System.Drawing.Color _sectionHoverColor = System.Drawing.Color.LightGray;

        // Fonts - Explicitly qualified as System.Drawing.Font to resolve CS0104 ambiguity
        private System.Drawing.Font _titleFont;
        private System.Drawing.Font _textFont;
        private System.Drawing.Font _smallFont;
        private System.Drawing.Font _iconFont;

        // Subsections
        private TowerInfoSection _basicInfo;
        private TowerInfoSection _statsSection;
        private TowerInfoSection _upgradeSection;
        private TowerInfoSection _targetingSection;
        private TowerInfoSection _specialAbilitiesSection;

        // Constructors
        public TowerInfoPanel()
        {
            _position = new Vector3(300f, 50f, 0);
            _size = new Vector3(300f, 400f, 0);
            LoadFonts();
            InitializeSections();
        }

        public TowerInfoPanel(System.Drawing.Color backgroundColor)
        {
            _position = new Vector3(300f, 50f, 0);
            _size = new Vector3(300f, 400f, 0);

            LoadFonts();
            InitializeSections();
            _backgroundColor = backgroundColor;
        }

        internal TowerInfoPanel(Tower currentTower,
            Tower previousTower,
            float displayTimer,
            bool isTransitioning,
            float transitionTimer,
            float transitionDuration,
            System.Drawing.Color backgroundColor,
            System.Drawing.Color borderColor,
            System.Drawing.Color sectionColor,
            System.Drawing.Color sectionHoverColor,
            System.Drawing.Font titleFont,
            System.Drawing.Font textFont,
            System.Drawing.Font smallFont,
            System.Drawing.Font iconFont,
            TowerInfoSection basicInfo,
            TowerInfoSection statsSection,
            TowerInfoSection upgradeSection,
            TowerInfoSection targetingSection,
            TowerInfoSection specialAbilitiesSection)
        {
            _currentTower = currentTower;
            _previousTower = previousTower;
            _displayTimer = displayTimer;
            _isTransitioning = isTransitioning;
            _transitionTimer = transitionTimer;
            _transitionDuration = transitionDuration;
            _backgroundColor = backgroundColor;
            _borderColor = borderColor;
            _sectionColor = sectionColor;
            _sectionHoverColor = sectionHoverColor;
            _titleFont = titleFont;
            _textFont = textFont;
            _smallFont = smallFont;
            _iconFont = iconFont;
            _basicInfo = basicInfo;
            _statsSection = statsSection;
            _upgradeSection = upgradeSection;
            _targetingSection = targetingSection;
            _specialAbilitiesSection = specialAbilitiesSection;
        }

        // Subsystem Initializers
        private void InitializeSections()
        {
            _basicInfo = new TowerInfoSection("Tower Information", 80f);
            _statsSection = new TowerInfoSection("Statistics", 100f);
            _upgradeSection = new TowerInfoSection("Upgrades", 120f);
            _targetingSection = new TowerInfoSection("Targeting", 100f);
            _specialAbilitiesSection = new TowerInfoSection("Special Abilities", 140f);
        }

        private void LoadFonts()
        {
            var cachedTitleFont = FontCache.GetFont("title");
            _titleFont = new System.Drawing.Font(cachedTitleFont.Name ?? "Arial", cachedTitleFont.Size > 0 ? cachedTitleFont.Size : 14);

            var cachedTextFont = FontCache.GetFont("default");
            _textFont = new System.Drawing.Font(cachedTextFont.Name ?? "Arial", cachedTextFont.Size > 0 ? cachedTextFont.Size : 12);

            var cachedSmallFont = FontCache.GetFont("small");
            _smallFont = new System.Drawing.Font(cachedSmallFont.Name ?? "Arial", cachedSmallFont.Size > 0 ? cachedSmallFont.Size : 10);

            var cachedIconFont = FontCache.GetFont("icon");
            _iconFont = new System.Drawing.Font(cachedIconFont.Name ?? "Arial", cachedIconFont.Size > 0 ? cachedIconFont.Size : 16);
        }

        // Core Overrides
        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            if (_isTransitioning)
            {
                _transitionTimer += deltaTime;
                if (_transitionTimer >= _transitionDuration)
                {
                    _transitionTimer = _transitionDuration;
                    _isTransitioning = false;
                }
            }

            if (_displayTimer > 0)
                _displayTimer -= deltaTime;
        }

        public override void Render()
        {
            if (!_isVisible)
                return;

            try
            {
                float progress = !_isTransitioning ? 1.0f : (_transitionDuration > 0f ? System.Math.Clamp(_transitionTimer / _transitionDuration, 0f, 1f) : 1.0f);
                float alpha = 255f * progress;

                System.Drawing.Color bg = System.Drawing.Color.FromArgb((int)alpha, _backgroundColor.R, _backgroundColor.G, _backgroundColor.B);
                System.Drawing.Color border = System.Drawing.Color.FromArgb((int)alpha, _borderColor.R, _borderColor.G, _borderColor.B);

                Renderer.DrawRectangle(_position.X, _position.Y, _size.X, _size.Y, bg);
                Renderer.DrawRectangle(_position.X, _position.Y, _size.X, _size.Y, border, 2f);
            }
            catch (Exception ex)
            {
                DLogger.Log($"Error rendering TowerInfoPanel: {ex.Message}");
            }
        }

        // Fixed CS0507: Changed modifier from public to protected to match base declaration
        protected override void RenderLegacy()
        {
            // Satisfies abstract contract safely
        }

        // UI Styling Routines
        protected override void InitializeP80Widgets()
        {
            base.InitializeP80Widgets();
        }

        protected override void ApplyStyle()
        {
            base.ApplyStyle();
        }

        //protected override void ApplyStyleFromSheet()
        //{
        //    base.ApplyStyleFromSheet(); // not applicable
        //}

        protected override void ApplyStyleToWidget(UIElement widget, UIStyle style)
        {
            base.ApplyStyleToWidget(widget, style);
        }

        // Boilerplate System Overrides
        public override bool Equals(object obj) => base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();
        public override string ToString() => base.ToString();
    }
}
