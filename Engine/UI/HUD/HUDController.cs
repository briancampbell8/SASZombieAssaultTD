using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.Waves;
using SASZombieAssaultTD.Engine.Towers;
using SASZombieAssaultTD.Engine.VectorMath;
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.Dictionary;
using SASZombieAssaultTD.Engine.Gameplay;
using Tower = SASZombieAssaultTD.Engine.Towers.Tower;
using PlacementInfo = SASZombieAssaultTD.Engine.Towers.PlacementInfo;
using SASZombieAssaultTD.Engine.Extensions;

namespace SASZombieAssaultTD.Engine.UI.HUD
{
    /// <summary>
    /// HUD controller for SAS Zombie Assault TD.
    /// Manages all HUD elements and player interface.
    /// </summary>
    public class HUDController
    {
        private readonly Dictionary<string, HUDComponent> _components;
        private readonly List<HUDNotification> _notifications;
        private bool _isInitialized;
        private bool _isVisible;
        private bool _isPaused;

        // Core HUD components
        private CashDisplay _cashDisplay;
        private WaveDisplay _waveDisplay;
        private LivesDisplay _livesDisplay;
        private TowerInfoPanel _towerInfoPanel;
        private UpgradePanel _upgradePanel;
        private PlacementInfoDisplay _placementInfoDisplay;

        // Events
        public event Action<int> OnCashChanged;
        public event Action<int> OnLivesChanged;
        public event Action<int> OnWaveStarted;
        public event Action<float> OnWaveProgress;
        public event Action<Tower> OnTowerSelected;
        public event Action OnPlacementStarted;
        public event Action OnPlacementCancelled;

        // Properties
        public bool IsVisible => _isVisible;
        public bool IsPaused => _isPaused;
        public bool IsInitialized => _isInitialized;

        // Singleton
        private static HUDController _instance;
        public static HUDController Instance => _instance ??= new HUDController();

        private HUDController()
        {
            _components = new Dictionary<string, HUDComponent>();
            _notifications = new List<HUDNotification>();
        }

        /// <summary>
        /// Initialize the HUD controller.
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized) return;

            System.Diagnostics.Debug.WriteLine("Initializing HUD Controller");

            try
            {
                // Initialize core components
                InitializeComponents();

                // Subscribe to game events
                SubscribeToEvents();

                // Set initial visibility
                SetVisibility(true);

                _isInitialized = true;
                System.Diagnostics.Debug.WriteLine("HUD Controller initialized successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to initialize HUD Controller: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Update all HUD components.
        /// </summary>
        /// <param name="deltaTime">Time since last frame.</param>
        public void Update(float deltaTime)
        {
            if (!_isInitialized || !_isVisible || _isPaused) return;

            try
            {
                // Update all components
                foreach (var component in _components.Values)
                {
                    component.Update(deltaTime);
                }

                // Update notifications
                UpdateNotifications(deltaTime);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating HUD: {ex.Message}");
            }
        }

        /// <summary>
        /// Render all HUD components.
        /// </summary>
        public void Render()
        {
            if (!_isInitialized || !_isVisible) return;

            try
            {
                // Render all components
                foreach (var component in _components.Values)
                {
                    if (component.IsVisible)
                        component.Render();
                }

                // Render notifications
                RenderNotifications();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error rendering HUD: {ex.Message}");
            }
        }

        /// <summary>
        /// Set HUD visibility.
        /// </summary>
        /// <param name="visible">Whether HUD should be visible.</param>
        public void SetVisibility(bool visible)
        {
            _isVisible = visible;

            foreach (var component in _components.Values)
            {
                component.SetVisibility(visible);
            }
        }

        /// <summary>
        /// Set HUD pause state.
        /// </summary>
        /// <param name="paused">Whether HUD should be paused.</param>
        public void SetPaused(bool paused)
        {
            _isPaused = paused;

            foreach (var component in _components.Values)
            {
                component.SetPaused(paused);
            }
        }

        /// <param name="name">Name of the component.</param>
        /// <returns>HUD component, or null if not found.</returns>
        public HUDComponent GetComponent(string name)
        {
            return _components.TryGetValue(name, out var component) ? component : null;
        }

        /// <summary>
        /// Add a custom HUD component.
        /// </summary>
        /// <param name="name">Name of the component.</param>
        /// <param name="component">Component to add.</param>
        public void AddComponent(string name, HUDComponent component)
        {
            if (_components.ContainsKey(name))
            {
                System.Diagnostics.Debug.WriteLine($"HUD component '{name}' already exists, replacing");
                _components[name].Cleanup();
            }

            _components[name] = component;
            component.Initialize();
            System.Diagnostics.Debug.WriteLine($"Added HUD component: {name}");
        }

        /// <summary>
        /// Remove a HUD component.
        /// </summary>
        /// <param name="name">Name of the component to remove.</param>
        /// <returns>True if component was removed.</returns>
        public bool RemoveComponent(string name)
        {
            if (_components.TryGetValue(name, out var component))
            {
                component.Cleanup();
                _components.Remove(name);
                System.Diagnostics.Debug.WriteLine($"Removed HUD component: {name}");
                return true;
            }

            return false;
        }

        /// <summary>
        /// Show a notification.
        /// </summary>
        /// <param name="notification">Notification to show.</param>
        public void ShowNotification(HUDNotification notification)
        {
            if (notification == null) return;

            _notifications.Add(notification);

            // Limit notifications to prevent overflow
            if (_notifications.Count > 5)
            {
                _notifications.RemoveAt(0);
            }

            // Play notification sound
            SASZombieAssaultTD.Engine.Audio.AudioSystem.PlaySound("notification");
        }

        /// <summary>
        /// Show cash display.
        /// </summary>
        /// <param name="amount">Cash amount to display.</param>
        public void ShowCashDisplay(int amount)
        {
            _cashDisplay?.SetAmount(amount);
            _cashDisplay?.SetVisibility(true);
        }

        /// <summary>
        /// Show wave display.
        /// </summary>
        /// <param name="waveNumber">Current wave number.</param>
        /// <param name="totalWaves">Total number of waves.</param>
        /// <param name="progress">Wave progress (0-1).</param>
        public void ShowWaveDisplay(int waveNumber, int totalWaves, float progress)
        {
            _waveDisplay?.SetWaveInfo(waveNumber, totalWaves, progress);
            _waveDisplay?.SetVisibility(true);
        }

        /// <summary>
        /// Show lives display.
        /// </summary>
        /// <param name="lives">Current lives.</param>
        /// <param name="maxLives">Maximum lives.</param>
        public void ShowLivesDisplay(int lives, int maxLives)
        {
            _livesDisplay?.SetLives(lives, maxLives);
            _livesDisplay?.SetVisibility(true);
        }

        /// <summary>
        /// Show tower info panel.
        /// </summary>
        /// <param name="tower">Tower to show info for.</param>
        public void ShowTowerInfoPanel(Tower tower)
        {
            _towerInfoPanel?.SetTower(tower);
            _towerInfoPanel?.SetVisibility(true);
        }

        /// <summary>
        /// Show upgrade panel.
        /// </summary>
        /// <param name="tower">Tower to show upgrades for.</param>
        public void ShowUpgradePanel(Tower tower)
        {
            _upgradePanel?.SetTower(tower);
            _upgradePanel?.SetVisibility(true);
        }

        /// <summary>
        /// Show placement info display.
        /// </summary>
        /// <param name="info">Placement information.</param>
        public void ShowPlacementInfo(PlacementInfo info)
        {
            _placementInfoDisplay?.SetPlacementInfo(info);
            _placementInfoDisplay?.SetVisibility(true);
        }

        /// <summary>
        /// Update placement info display.
        /// </summary>
        /// <param name="info">Updated placement information.</param>
        public void UpdatePlacementInfo(PlacementInfo info)
        {
            _placementInfoDisplay?.SetPlacementInfo(info);
        }

        /// <summary>
        /// Hide placement info display.
        /// </summary>
        public void HidePlacementInfo()
        {
            _placementInfoDisplay?.SetVisibility(false);
        }

        /// <summary>
        /// Show minimap.
        /// </summary>
        public void UpdateGameStates()
        {
            try
            {
                var waveDirector = WaveDirector.Instance;
                if (waveDirector != null)
                {
                    ShowWaveDisplay(waveDirector.CurrentWave, waveDirector.TotalWaves, waveDirector.WaveProgress);
                }

                var playerLives = ModernPlayerStateSystem.Instance;
                if (playerLives != null)
                {
                    ShowLivesDisplay(playerLives.CurrentLives, playerLives.MaxLives);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating game states: {ex.Message}");
            }
        }

        /// <summary>
        /// Show wave notification.
        /// </summary>
        /// <param name="notification">Wave notification to show.</param>
        public void ShowWaveNotification(WaveNotification notification)
        {
            var hudNotification = new HUDNotification
            {
                Title = notification.Title,
                Message = notification.Description,
                Type = NotificationType.Wave,
                Duration = notification.Duration,
                Icon = "wave"
            };

            ShowNotification(hudNotification);
        }

        /// <summary>
        /// Show error notification.
        /// </summary>
        /// <param name="message">Error message.</param>
        public void ShowError(string message)
        {
            var notification = new HUDNotification
            {
                Title = "Error",
                Message = message,
                Type = NotificationType.Error,
                Duration = 3f,
                Icon = "error"
            };

            ShowNotification(notification);
        }

        /// <summary>
        /// Show success notification.
        /// </summary>
        /// <param name="message">Success message.</param>
        public void ShowSuccess(string message)
        {
            var notification = new HUDNotification
            {
                Title = "Success",
                Message = message,
                Type = NotificationType.Success,
                Duration = 2f,
                Icon = "success"
            };

            ShowNotification(notification);
        }

        /// <summary>
        /// Show warning notification.
        /// </summary>
        /// <param name="message">Warning message.</param>
        public void ShowWarning(string message)
        {
            var notification = new HUDNotification
            {
                Title = "Warning",
                Message = message,
                Type = NotificationType.Warning,
                Duration = 2.5f,
                Icon = "warning"
            };

            ShowNotification(notification);
        }

        /// <summary>
        /// Show info notification.
        /// </summary>
        /// <param name="message">Info message.</param>
        public void ShowInfo(string message)
        {
            var notification = new HUDNotification
            {
                Title = "Info",
                Message = message,
                Type = NotificationType.Info,
                Duration = 2f,
                Icon = "info"
            };

            ShowNotification(notification);
        }

        /// <summary>
        /// Show message dialog.
        /// </summary>
        /// <param name="title">Dialog title.</param>
        /// <param name="message">Dialog message.</param>
        /// <param name="onConfirm">Action when confirmed.</param>
        /// <param name="onCancel">Action when cancelled.</param>
        public void ShowMessageDialog(string title, string message, Action onConfirm, Action onCancel = null)
        {
            var dialog = new MessageDialog
            {
                Title = title,
                Message = message,
                ConfirmText = "OK",
                CancelText = onCancel != null ? "Cancel" : null,
                OnConfirm = onConfirm,
                OnCancel = onCancel
            };

            // Show dialog implementation would go here
            System.Diagnostics.Debug.WriteLine($"Showing message dialog: {title} - {message}");
        }

        /// <summary>
        /// Show confirmation dialog.
        /// </summary>
        /// <param name="title">Dialog title.</param>
        /// <param name="message">Dialog message.</param>
        /// <param name="onConfirm">Action when confirmed.</param>
        /// <param name="onCancel">Action when cancelled.</param>
        public void ShowConfirmationDialog(string title, string message, Action onConfirm, Action onCancel = null)
        {
            var dialog = new MessageDialog
            {
                Title = title,
                Message = message,
                ConfirmText = "Confirm",
                CancelText = onCancel != null ? "Cancel" : null,
                OnConfirm = onConfirm,
                OnCancel = onCancel
            };

            // Show dialog implementation would go here
            System.Diagnostics.Debug.WriteLine($"Showing confirmation dialog: {title} - {message}");
        }

        /// <summary>
        /// Get HUD statistics.
        /// </summary>
        /// <returns>HUD statistics.</returns>
        public HUDStatistics GetStatistics()
        {
            return new HUDStatistics
            {
                VisibleComponents = GetVisibleComponentCount(),
                TotalComponents = _components.Count,
                ActiveNotifications = _notifications.Count,
                IsVisible = _isVisible,
                IsPaused = _isPaused,
                ComponentStates = GetComponentStates()
            };
        }

        /// <summary>
        /// Cleanup all HUD components.
        /// </summary>
        public void Cleanup()
        {
            System.Diagnostics.Debug.WriteLine("Cleaning up HUD Controller");

            // Cleanup all components
            foreach (var component in _components.Values)
            {
                component.Cleanup();
            }

            _components.Clear();
            _notifications.Clear();

            _isInitialized = false;
            _isVisible = false;
            _isPaused = false;
        }

        ///  Private Methods

        /// <summary>
        /// Initialize all HUD components.
        /// </summary>
        private void InitializeComponents()
        {
            // Create core HUD components
            _cashDisplay = new CashDisplay();
            _waveDisplay = new WaveDisplay();
            _livesDisplay = new LivesDisplay();
            _towerInfoPanel = new TowerInfoPanel();
            _upgradePanel = new UpgradePanel();
            _placementInfoDisplay = new PlacementInfoDisplay(new Vector3(0f, 0f, 0f), new Vector3(100f, 50f, 0f));

            // Add components to dictionary
            AddComponent("CashDisplay", _cashDisplay);
            AddComponent("WaveDisplay", _waveDisplay);
            AddComponent("LivesDisplay", _livesDisplay);
            AddComponent("TowerInfoPanel", _towerInfoPanel);
            AddComponent("UpgradePanel", _upgradePanel);
            AddComponent("PlacementInfoDisplay", _placementInfoDisplay);

            System.Diagnostics.Debug.WriteLine($"Initialized {_components.Count} HUD components");
        }

        /// <summary>
        /// Subscribe to game events.
        /// </summary>
        private void SubscribeToEvents()
        {
            // Wave events
            var waveDirector = WaveDirector.Instance;
            if (waveDirector != null)
            {
                waveDirector.OnWaveStarted += (wave) => ShowWaveDisplay(wave, waveDirector.TotalWaves, 0f);
                waveDirector.OnWaveCompleted += (wave) => ShowSuccess($"Wave {wave} completed!");
                // TODO: OnWaveProgress is a method group, not an event
                // waveDirector.OnWaveProgress += (progress) => UpdateWaveProgress(progress);
            }

            // Player lives events
            var playerLives = ModernPlayerStateSystem.Instance;
            if (playerLives != null)
            {
                playerLives.OnLivesChanged += (lives, maxLives) => ShowLivesDisplay(lives, maxLives);
            }

            // Tower events
            // TODO: Fix TowerRegistry event subscriptions
            // TowerRegistry.Instance.OnTowerSelected += (tower) => ShowTowerInfoPanel(tower);
            // TowerRegistry.Instance.OnTowerDeselected += () => HideTowerInfoPanel();
        }

        /// <summary>
        /// Update wave progress display.
        /// </summary>
        private void UpdateWaveProgress(float progress)
        {
            _waveDisplay?.SetProgress(progress);
        }

        /// <summary>
        /// Hide tower info panel.
        /// </summary>
        private void HideTowerInfoPanel()
        {
            _towerInfoPanel?.SetVisibility(false);
        }

        /// <summary>
        /// Update notifications.
        /// </summary>
        private void UpdateNotifications(float deltaTime)
        {
            for (int i = _notifications.Count - 1; i >= 0; i--)
            {
                var notification = _notifications[i];
                notification.Update(deltaTime);

                if (notification.IsExpired)
                {
                    _notifications.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Render notifications.
        /// </summary>
        private void RenderNotifications()
        {
            var yPosition = 100f; // Start from top

            foreach (var notification in _notifications)
            {
                notification.Render(50f, yPosition);
                yPosition += notification.Height + 10f;
            }
        }

        /// <summary>
        /// Get count of visible components.
        /// </summary>
        private int GetVisibleComponentCount()
        {
            var count = 0;
            foreach (var component in _components.Values)
            {
                if (component.IsVisible)
                    count++;
            }
            return count;
        }

        /// <summary>
        /// Get component states.
        /// </summary>
        private Dictionary<string, bool> GetComponentStates()
        {
            var states = new Dictionary<string, bool>();
            foreach (var kvp in _components)
            {
                states[kvp.Key] = kvp.Value.IsVisible;
            }
            return states;
        }

        /// 
    }

    /// <summary>
    /// HUD statistics container.
    /// </summary>
    public class HUDStatistics
    {
        public int VisibleComponents { get; set; }
        public int TotalComponents { get; set; }
        public int ActiveNotifications { get; set; }
        public bool IsVisible { get; set; }
        public bool IsPaused { get; set; }
        public Dictionary<string, bool> ComponentStates { get; set; }

        public override string ToString()
        {
            return $"HUD Statistics:\n" +
                   $"Visible Components: {VisibleComponents}/{TotalComponents}\n" +
                   $"Active Notifications: {ActiveNotifications}\n" +
                   $"Is Visible: {IsVisible}\n" +
                   $"Is Paused: {IsPaused}";
        }
    }

    /// <summary>
    /// HUD notification for displaying messages to the player.
    /// </summary>
    public class HUDNotification
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public NotificationType Type { get; set; }
        public float Duration { get; set; }
        public string Icon { get; set; }
        public float Height { get; set; }
        public bool IsExpired { get; private set; }
        private float _timer;

        public HUDNotification()
        {
            Height = 60f;
            IsExpired = false;
        }

        public void Update(float deltaTime)
        {
            _timer += deltaTime;
            IsExpired = _timer >= Duration;
        }

        public void Render(float x, float y)
        {
            // Render notification background
            var backgroundColor = Type switch
            {
                NotificationType.Error => new Color(200, 50, 50, 200),
                NotificationType.Warning => new Color(200, 150, 50, 200),
                NotificationType.Success => new Color(50, 200, 50, 200),
                NotificationType.Info => new Color(50, 100, 200, 200),
                NotificationType.Wave => new Color(50, 150, 200, 200),
                _ => new Color(100, 100, 100, 200)
            };

            RenderSystem.DrawRectangle(x, y, 300f, Height, backgroundColor);

            // Render icon
            if (!string.IsNullOrEmpty(Icon))
            {
                var iconSprite = SpriteCache.GetSprite(Icon);
                if (iconSprite != null)
                {
                    RenderSystem.DrawSprite(iconSprite, new Vector3(x + 10f, y + 10f, 0), 0.8f, Color.White);
                }
            }

            // Render text
            var font = FontCache.GetFont("medium");
            var titleColor = Type switch
            {
                NotificationType.Error => System.Drawing.Color.Red,
                NotificationType.Warning => System.Drawing.Color.Orange,
                NotificationType.Success => System.Drawing.Color.Green,
                NotificationType.Info => System.Drawing.Color.Cyan,
                NotificationType.Wave => System.Drawing.Color.Yellow,
                _ => System.Drawing.Color.White
            };

            // TODO: Implement rendering system
            // RenderSystem.DrawString(Title, x + 50f, y + 15f, titleColor, font);

            if (!string.IsNullOrEmpty(Message))
            {
                var messageColor = System.Drawing.Color.White;
                // TODO: Implement rendering system
                // RenderSystem.DrawString(Message, x + 50f, y + 35f, messageColor, font);
            }
        }
    }

    /// <summary>
    /// Notification types.
    /// </summary>
    public enum NotificationType
    {
        Info,
        Warning,
        Error,
        Success,
        Wave
    }

    /// <summary>
    /// Message dialog for showing dialogs to the player.
    /// </summary>
    public class MessageDialog
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string ConfirmText { get; set; }
        public string CancelText { get; set; }
        public Action OnConfirm { get; set; }
        public Action OnCancel { get; set; }
    }
}
