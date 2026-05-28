/*
 * File Path: Engine/UI/Systems/UISystemInitializer.cs
 * Program Name: UISystemInitializer
 * Date Created: 2026-03-05
 * 
 * Change Log:
 * ----------
 * 2026-03-05: Initial implementation
 * What: Created UI system initialization framework
 * Why: To provide centralized UI initialization and resolve unused font field warnings
 * 
 * Purpose: Initialize all UI systems and components in proper order
 * Features: 
 * - Async initialization pipeline
 * - Font manager integration
 * - UI element registration
 * - Performance monitoring
 * - Error handling and recovery
 */

using System;
using System.Threading.Tasks;
using SASZombieAssaultTD.Engine.UI.Managers;
using SASZombieAssaultTD.Engine.Dictionary;
using SASZombieAssaultTD.Engine.Core;
using ModernLoggingSystem = SASZombieAssaultTD.Engine.Core.ModernLoggingSystem;
using SASZombieAssaultTD.Engine.UI.HUD;
using SASZombieAssaultTD.Engine.UI.Components;

namespace SASZombieAssaultTD.Engine.UI.Systems
{
    /// <summary>
    /// Centralized UI system initialization manager.
    /// Coordinates the setup of all UI components and systems.
    /// </summary>
    public static class UISystemInitializer
    {
        private static bool _initialized = false;
        private static readonly object _initLock = new object();

        /// <summary>
        /// Initialize all UI systems in proper order.
        /// </summary>
        /// <returns>Task representing the initialization process</returns>
        public static async Task<bool> InitializeAsync()
        {
            lock (_initLock)
            {
                if (_initialized)
                {
                    Engine.Diagnostics.DebugLogger.LogDebug("INFO", "UISystemInitializer: Already initialized");
                    return true;
                }
            }

            try
            {
                Engine.Diagnostics.DebugLogger.LogDebug("INFO", "UISystemInitializer: Starting UI system initialization");
                
                // Phase 1: Initialize font management
                var fontInitSuccess = await InitializeFontSystemAsync();
                if (!fontInitSuccess)
                {
                    Engine.Diagnostics.DebugLogger.LogDebug("ERROR", "UISystemInitializer: Font system initialization failed");
                    return false;
                }

                // Phase 2: Initialize UI element factories
                var elementFactorySuccess = InitializeElementFactories();
                if (!elementFactorySuccess)
                {
                    Engine.Diagnostics.DebugLogger.LogDebug("ERROR", "UISystemInitializer: Element factory initialization failed");
                    return false;
                }

                // Phase 3: Register UI components
                var componentRegistrationSuccess = RegisterUIComponents();
                if (!componentRegistrationSuccess)
                {
                    Engine.Diagnostics.DebugLogger.LogDebug("ERROR", "UISystemInitializer: Component registration failed");
                    return false;
                }

                // Phase 4: Initialize UI event systems
                var eventSystemSuccess = InitializeEventSystems();
                if (!eventSystemSuccess)
                {
                    Engine.Diagnostics.DebugLogger.LogDebug("ERROR", "UISystemInitializer: Event system initialization failed");
                    return false;
                }

                lock (_initLock)
                {
                    _initialized = true;
                }

                Engine.Diagnostics.DebugLogger.LogDebug("INFO", "UISystemInitializer: UI system initialization completed successfully");
                return true;
            }
            catch (Exception ex)
            {
                Engine.Diagnostics.DebugLogger.LogDebug("ERROR", $"UISystemInitializer: Critical initialization failure - {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Initialize font management system.
        /// </summary>
        /// <returns>Task representing font system initialization</returns>
        private static async Task<bool> InitializeFontSystemAsync()
        {
            try
            {
                Engine.Diagnostics.DebugLogger.LogDebug("DEBUG", "UISystemInitializer: Initializing font system");
                
                await FontManager.InitializeAsync();
                
                // Test font loading to ensure system is working
                var titleFont = FontManager.LoadTitleFont();
                var textFont = FontManager.LoadTextFont();
                var iconFont = FontManager.LoadIconFont();
                var smallFont = FontManager.LoadSmallFont();

                var allFontsLoaded = titleFont != null && textFont != null && 
                                   iconFont != null && smallFont != null;

                if (allFontsLoaded)
                {
                    Engine.Diagnostics.DebugLogger.LogDebug("DEBUG", "UISystemInitializer: All fonts loaded successfully");
                    return true;
                }
                else
                {
                    Engine.Diagnostics.DebugLogger.LogDebug("WARNING", "UISystemInitializer: Some fonts failed to load");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Engine.Diagnostics.DebugLogger.LogDebug("ERROR", $"UISystemInitializer: Font system initialization failed - {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Initialize UI element factories.
        /// </summary>
        /// <returns>True if successful, false otherwise</returns>
        private static bool InitializeElementFactories()
        {
            try
            {
                Engine.Diagnostics.DebugLogger.LogDebug("DEBUG", "UISystemInitializer: Initializing UI element factories");
                
                // Initialize UI element factories
                UIElementFactory.Initialize();
                
                Engine.Diagnostics.DebugLogger.LogDebug("DEBUG", "UISystemInitializer: UI element factories initialized");
                return true;
            }
            catch (Exception ex)
            {
                Engine.Diagnostics.DebugLogger.LogDebug("ERROR", $"UISystemInitializer: Element factory initialization failed - {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Register UI components with the system.
        /// </summary>
        /// <returns>True if successful, false otherwise</returns>
        private static bool RegisterUIComponents()
        {
            try
            {
                Engine.Diagnostics.DebugLogger.LogDebug("DEBUG", "UISystemInitializer: Registering UI components");
                
                // Register core UI components
                UIComponentRegistry.RegisterComponent<HUDController>();
                UIComponentRegistry.RegisterComponent<TowerInfoPanel>();
                UIComponentRegistry.RegisterComponent<UpgradePanel>();
                UIComponentRegistry.RegisterComponent<PlacementInfoDisplay>();
                
                Engine.Diagnostics.DebugLogger.LogDebug("DEBUG", "UISystemInitializer: UI components registered successfully");
                return true;
            }
            catch (Exception ex)
            {
                Engine.Diagnostics.DebugLogger.LogDebug("ERROR", $"UISystemInitializer: Component registration failed - {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Initialize UI event systems.
        /// </summary>
        /// <returns>True if successful, false otherwise</returns>
        private static bool InitializeEventSystems()
        {
            try
            {
                Engine.Diagnostics.DebugLogger.LogDebug("DEBUG", "UISystemInitializer: Initializing UI event systems");
                
                // Initialize event routing and handling
                UIEventSystem.Initialize();
                
                Engine.Diagnostics.DebugLogger.LogDebug("DEBUG", "UISystemInitializer: UI event systems initialized");
                return true;
            }
            catch (Exception ex)
            {
                Engine.Diagnostics.DebugLogger.LogDebug("ERROR", $"UISystemInitializer: Event system initialization failed - {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Check if UI systems are initialized.
        /// </summary>
        public static bool IsInitialized
        {
            get
            {
                lock (_initLock)
                {
                    return _initialized;
                }
            }
        }

        /// <summary>
        /// Get initialization status and statistics.
        /// </summary>
        /// <returns>UI system initialization statistics</returns>
        public static UISystemStatistics GetStatistics()
        {
            return new UISystemStatistics
            {
                Initialized = IsInitialized,
                FontStatistics = FontManager.GetStatistics(),
                InitializationTime = DateTime.Now // Would track actual init time in real implementation
            };
        }
    }

    /// <summary>
    /// UI system initialization statistics.
    /// </summary>
    public sealed class UISystemStatistics
    {
        public bool Initialized { get; set; }
        public FontCacheStatistics FontStatistics { get; set; }
        public DateTime InitializationTime { get; set; }

        public override string ToString()
        {
            return $"UI System: Initialized={Initialized}, {FontStatistics}";
        }
    }
}
