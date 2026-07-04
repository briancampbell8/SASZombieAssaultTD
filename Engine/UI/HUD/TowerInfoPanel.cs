using System;
using System.Collections.Generic;
using System.Linq;
using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.Audio;
using SASZombieAssaultTD.Engine.Towers;
using SASZombieAssaultTD.Engine.VectorMath;
using SASZombieAssaultTD.Engine.UI.Rendering;
using TowerUpgrade = SASZombieAssaultTD.Engine.UI.HUD.TowerUpgrade;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.UI.HUD
{
    ///<summary>
    ///Tower info panel for SAS Zombie Assault TD HUD.
    ///Shows detailed information about selected tower.
    ///</summary>
    public class TowerInfoPanel : HUDComponent
    {
        private Tower _currentTower;
        private Tower _previousTower;
        private float _displayTimer = 0f;
        private new bool _isVisible = false;
        private bool _isTransitioning = false;
        private float _transitionTimer = 0f;
        private float _transitionDuration = 0.3f;

        //Panel sections
        private TowerInfoSection _basicInfo;
        private TowerInfoSection _statsSection;
        private TowerInfoSection _upgradeSection;
        private TowerInfoSection _targetingSection;
        private TowerInfoSection _specialAbilitiesSection;

        //Visual properties
        private new Vector3 _position;
        private new Vector3 _size;
        private new Color _backgroundColor = new Color(0, 0, 0, 180);
        private Color _borderColor = new Color(200, 200, 200, 255);
        private Color _sectionColor = Color.White;
        private Color _sectionHoverColor = Color.LightGray;

        //Text properties
        private Font _titleFont;
        private Font _textFont;
        private Font _smallFont;
        private Font _iconFont;

        //Events
        public event Action<Tower> OnTowerUpgraded;
        public event Action<Tower> OnTowerSold;
        public event Action<Tower> OnTowerTargetChanged;

        public TowerInfoPanel()
        {
            _position = new Vector3(300f, 50f, 0);
            _size = new Vector3(300f, 400f, 0);

            //Initialize fonts
            var cachedTitleFont = FontCache.GetFont("title");
            _titleFont = new Font(cachedTitleFont?.Name ?? "Arial", cachedTitleFont?.Size ?? 14);
            var cachedTextFont = FontCache.GetFont("default");
            _textFont = new Font(cachedTextFont?.Name ?? "Arial", cachedTextFont?.Size ?? 12);
            var cachedSmallFont = FontCache.GetFont("small");
            _smallFont = new Font(cachedSmallFont?.Name ?? "Arial", cachedSmallFont?.Size ?? 10);
            var cachedIconFont = FontCache.GetFont("icon");
            _iconFont = new Font(cachedIconFont?.Name ?? "Arial", cachedIconFont?.Size ?? 16);

            InitializeSections();
        }

        ///<summary>
        ///Set tower to display.
        ///</summary>
        ///<param name="tower">Tower to display.</param>
        public void SetTower(Tower tower)
        {
            if (tower == null)
            {
                HidePanel();
                return;
            }

            _previousTower = _currentTower;
            _currentTower = tower;
            _isVisible = true;
            _displayTimer = 0f;
            _isTransitioning = true;
            _transitionTimer = 0f;

            //Start transition animation
            StartTransition();
        }

        ///<summary>
        ///Hide the panel.
        ///</summary>
        public void HidePanel()
        {
            _isVisible = false;
            _isTransitioning = false;
            _transitionTimer = 0f;
            _currentTower = null;
            _previousTower = null;
        }

        ///<summary>
        ///Set panel position.
        ///</summary>
        ///<param name="position">New position.</param>
        public void SetPosition(Vector3 position)
        {
            _position = position;
        }

        ///<summary>
        ///Set panel size.
        ///</summary>
        ///<param name="size">New size.</param>
        public void SetSize(Vector3 size)
        {
            _size = size;
        }

        ///<summary>
        ///Set background color.
        ///</summary>
        ///<param name="color">Background color.</param>
        public void SetBackgroundColor(Color color)
        {
            _backgroundColor = color;
        }

        ///<summary>
        ///Set border color.
        ///</summary>
        ///<param name="color">Border color.</param>
        public void SetBorderColor(Color color)
        {
            _borderColor = color;
        }

        ///<summary>
        ///Set section colors.
        ///</summary>
        public void SetSectionColors(Color normal, Color hover)
        {
            _sectionColor = normal;
            _sectionHoverColor = hover;
        }

        public override void Initialize()
        {
            base.Initialize();

            //Load fonts
            var cachedLargeFont = FontCache.GetFont("large");
            _titleFont = new Font(cachedLargeFont?.Name ?? "Arial", cachedLargeFont?.Size ?? 14);
            var cachedMediumFont = FontCache.GetFont("medium");
            _textFont = new Font(cachedMediumFont?.Name ?? "Arial", cachedMediumFont?.Size ?? 12);
            var cachedSmallFont = FontCache.GetFont("small");
            _smallFont = new Font(cachedSmallFont?.Name ?? "Arial", cachedSmallFont?.Size ?? 10);
            var cachedIconFont = FontCache.GetFont("icon");
            _iconFont = new Font(cachedIconFont?.Name ?? "Arial", cachedIconFont?.Size ?? 16);

            //Initialize sections
            InitializeSections();
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            //Update transition animation
            if (_isTransitioning)
            {
                UpdateTransition(deltaTime);
            }

            //Update display timer
            if (_displayTimer > 0)
            {
                _displayTimer -= deltaTime;
            }

            //Update tower info if tower changed
            if (_currentTower != null || _currentTower != _previousTower)
            {
                UpdateTowerInfo();
            }
        }

        public override void Render()
        {
            if (!_isVisible) return;

            try
            {
                //Render background
                //TODO: RenderBackground is not a method
                //RenderBackground();

                //Render sections
                RenderSections();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error rendering tower info panel: {ex.Message}");
            }
        }

        ///<summary>
        ///Initialize all panel sections.
        ///</summary>
        private void InitializeSections()
        {
            _basicInfo = new TowerInfoSection("Tower Information", 80f);

            _statsSection = new TowerInfoSection("Statistics", 100f);

            _upgradeSection = new TowerInfoSection("Upgrades", 120f);

            _targetingSection = new TowerInfoSection("Targeting", 100f);

            _specialAbilitiesSection = new TowerInfoSection("Special Abilities", 140f);
        }

        private void StartTransition()
        {
            _transitionTimer = 0f;
            _isTransitioning = true;
        }

        ///<summary>
        ///Update transition animation.
        ///</summary>
        private void UpdateTransition(float deltaTime)
        {
            _transitionTimer += deltaTime;

            if (_transitionTimer >= _transitionDuration)
            {
                _isTransitioning = false;
                _transitionTimer = 0f;
            }
        }

        ///<summary>
        ///Update tower information display.
        ///</summary>
        private void UpdateTowerInfo()
        {
            if (_currentTower == null) return;

            //Update basic info
            _basicInfo.ClearContent();
            _basicInfo.AddContent($"Name: {_currentTower.Name}");
            _basicInfo.AddContent($"Type: {_currentTower.Type}");
            _basicInfo.AddContent($"Level: {_currentTower.Level}");
            _basicInfo.AddContent($"Damage: {_currentTower.Damage}");
            _basicInfo.AddContent($"Range: {_currentTower.Range:F1}");

            //Update stats
            _statsSection.ClearContent();
            _statsSection.AddContent($"Kills: {_currentTower.TotalKills}");
            _statsSection.AddContent($"Accuracy: {_currentTower.Accuracy:P1}");
            _statsSection.AddContent($"DPS: {_currentTower.DPS:F1}");
            _statsSection.AddContent($"Uptime: {_currentTower.Uptime:F1}s");

            //Update upgrade info
            _upgradeSection.ClearContent();
            var upgrades = _currentTower.AvailableUpgrades;
            foreach (var upgrade in upgrades)
            {
                _upgradeSection.AddContent($"{upgrade.Name} (${upgrade.Cost})");
            }

            //Update targeting info
            _targetingSection.ClearContent();
            _targetingSection.AddContent($"Mode: {_currentTower.TargetingMode}");
            //TODO: Add TargetPriority property to Tower class
            //_targetingSection.AddContent($"Priority: {_currentTower.TargetPriority}");
            _targetingSection.AddContent($"Range: {_currentTower.Range:F1}");

            //Update special abilities
            _specialAbilitiesSection.ClearContent();
            //TODO: Add SpecialAbilities property to Tower class
            //var abilities = _currentTower.SpecialAbilities;
            //foreach (var ability in abilities)
            //{
            //    _specialAbilitiesSection.AddContent(ability);
            //}
        }

        ///<summary>
        ///Render all sections.
        ///</summary>
        private void RenderSections()
        {
            var sectionY = _position.Y + 20f;

            //Render each section
            RenderSection(_basicInfo, sectionY);
            sectionY += _basicInfo.Height + 10f;

            RenderSection(_statsSection, sectionY);
            sectionY += _statsSection.Height + 10f;

            RenderSection(_upgradeSection, sectionY);
            sectionY += _upgradeSection.Height + 10f;

            RenderSection(_targetingSection, sectionY);
            sectionY += _targetingSection.Height + 10f;

            RenderSection(_specialAbilitiesSection, sectionY);
        }

        ///<summary>
        ///Render a section.
        ///</summary>
        ///<param name="section">Section to render.</param>
        ///<param name="y">Y position.</param>
        private void RenderSection(TowerInfoSection section, float y)
        {
            var sectionHeight = section.Height;
            var sectionY = _position.Y + y;
            var sectionWidth = _size.X - 40f; //Margin
            var sectionX = _position.X + 20f; //Margin

            //Render section background
            var backgroundColor = new Color(
                _backgroundColor.R, _backgroundColor.G, _backgroundColor.B,
                (byte)(200 * GetTransitionProgress())
            );
            //RenderSystem.DrawRectangle(sectionX, sectionY, sectionWidth, sectionHeight, backgroundColor); //TODO: implement

            //Render border
            var borderColor = new Color(
                _borderColor.R, _borderColor.G, _borderColor.B,
                (byte)(255 * GetTransitionProgress())
            );
            //RenderSystem.DrawRectangle(sectionX, sectionY, sectionWidth, sectionHeight, borderColor, 2f); //TODO: implement

            //Render section title
            var titleColor = new Color(_sectionColor.R, _sectionColor.G, _sectionColor.B, 255);
            //RenderSystem.DrawString(section.Title, sectionX + 10f, sectionY + 10f, titleColor, _titleFont); //TODO: implement

            //Render section content
            var contentColor = new Color(_sectionColor.R, _sectionColor.G, _sectionColor.B, 255);
            foreach (var content in section.GetContent())
            {
                //RenderSystem.DrawString(content, sectionX + 10f, sectionY + 30f + (section.GetContent().IndexOf(content) * 20f), contentColor, _textFont); //TODO: implement
            }
        }

        ///<summary>
        ///Get transition progress (0-1).
        ///</summary>
        ///<returns>Transition progress.</returns>
        private float GetTransitionProgress()
        {
            if (!_isTransitioning) return 1f;
            return System.Math.Clamp(_transitionTimer / _transitionDuration, 0f, 1f);
        }

        ///<summary>
        ///Clear all section content.
        ///</summary>
        public void ClearAllContent()
        {
            _basicInfo.ClearContent();
            _statsSection.ClearContent();
            _upgradeSection.ClearContent();
            _targetingSection.ClearContent();
            _specialAbilitiesSection.ClearContent();
        }
    }

    ///<summary>
    ///Tower info section container.
    ///</summary>
    public class TowerInfoSection
    {
        public string Title { get; set; }
        public float Height { get; set; }
        private List<string> _content = new List<string>();

        public TowerInfoSection(string title, float height)
        {
            Title = title;
            Height = height;
        }

        public void ClearContent()
        {
            _content.Clear();
        }

        public void AddContent(string content)
        {
            _content.Add(content);
        }

        public List<string> GetContent()
        {
            return new List<string>(_content);
        }
    }

    ///<summary>
    ///Extension methods for TowerInfoPanel.
    ///</summary>
    public static class TowerInfoPanelExtensions
    {
        ///<summary>
        ///Get tower upgrade cost.
        ///</summary>
        public static int GetUpgradeCost(Tower tower)
        {
            return tower.AvailableUpgrades?.FirstOrDefault()?.Cost ?? 0;
        }

        ///<summary>
        ///Get tower upgrade level.
        ///</summary>
        public static int GetUpgradeLevel(Tower tower)
        {
            return tower.Level;
        }

        ///<summary>
        ///Check if tower can be upgraded.
        ///</summary>
        public static bool CanUpgrade(Tower tower)
        {
            return tower.CanUpgrade;
        }

        ///<summary>
        ///Get upgrade price for next level.
        ///</summary>
        public static int GetNextUpgradeCost(Tower tower)
        {
            var currentLevel = GetUpgradeLevel(tower);
            var upgrades = tower.AvailableUpgrades;
            var nextLevel = currentLevel + 1;

            return upgrades.FirstOrDefault(u => u.Level == nextLevel)?.Cost ?? 0;
        }

        ///<summary>
        ///Get all available upgrades for tower.
        ///</summary>
        public static List<Towers.TowerUpgrade> GetAvailableUpgrades(Tower tower)
        {
            return tower.AvailableUpgrades.ToList();
        }

        ///<summary>
        ///Get tower damage per second.
        ///</summary>
        public static float GetDPS(Tower tower)
        {
            return (float)tower.DPS;
        }

        ///<summary>
        ///Get tower accuracy percentage.
        ///</summary>
        public static float GetAccuracy(Tower tower)
        {
            return (float)tower.Accuracy;
        }

        ///<summary>
        ///Get tower uptime.
        ///</summary>
        public static float GetUptime(Tower tower)
        {
            return tower.Uptime;
        }
    }
}
