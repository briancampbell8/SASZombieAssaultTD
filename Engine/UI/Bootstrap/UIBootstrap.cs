// =====================================================================================================
//  FILE: UIBootstrap.cs
//  PATH: Engine/UI/Bootstrap/UIBootstrap.cs
//  SUBSYSTEM: UI Bootstrap
//
//  ROLE:
//      Provides a deterministic, centralized bootstrap for the UI subsystem.
//      Orchestrates initialization of UI font systems, element factories, HUD components,
//      and UI event routing in a strict, synchronous sequence.
//
//  RESPONSIBILITIES:
//      - Initialize all UI-related systems in a deterministic, non-async order.
//      - Register core HUD and UI components with the UI component registry.
//      - Expose initialization status and statistics for diagnostics and engine hosts.
//
//  NON-RESPONSIBILITIES:
//      - Implementing deep frame-level update calculation rules or rendering commands directly.
//      - Managing active game state transitions or scene graph orchestration.
//      - Handling discrete hardware device allocation boundaries.
//
//  ARCHITECTURAL NOTES:
//      - This class replaces the legacy UISystemInitializer static async pipeline.
//      - Engine hosts (e.g., GameRootMain) invoke UIBootstrap.Initialize() during startup.
//      - All UI subsystem initialization must flow through this bootstrap to preserve determinism.
// =====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Towers;
using SASZombieAssaultTD.Engine.UI.HUD;
using SASZombieAssaultTD.Engine.UI.Managers;
using SASZombieAssaultTD.Engine.UI.Systems;

namespace SASZombieAssaultTD.Engine.UI.Bootstrap
{
    internal sealed class UIBootstrap
    {
        private bool _initialized;

        internal bool IsInitialized => _initialized;

        internal bool Initialize()
        {
            if (_initialized)
            {
                DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Info, "UIBootstrap: Already initialized");
                return true;
            }

            try
            {
                DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Info, "UIBootstrap: Starting UI system initialization");

                var fontInitSuccess = InitializeFontSystem();
                if (!fontInitSuccess)
                {
                    DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Error, "UIBootstrap: Font system initialization failed");
                    return false;
                }

                var elementFactorySuccess = InitializeElementFactories();
                if (!elementFactorySuccess)
                {
                    DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Error, "UIBootstrap: Element factory initialization failed");
                    return false;
                }

                var componentRegistrationSuccess = RegisterUIComponents();
                if (!componentRegistrationSuccess)
                {
                    DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Error, "UIBootstrap: Component registration failed");
                    return false;
                }

                var eventSystemSuccess = InitializeEventSystems();
                if (!eventSystemSuccess)
                {
                    DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Error, "UIBootstrap: Event system initialization failed");
                    return false;
                }

                _initialized = true;

                DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Info, "UIBootstrap: UI system initialization completed successfully");
                return true;
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Error, $"UIBootstrap: Critical initialization failure - {ex.Message}");
                return false;
            }
        }

        private bool InitializeFontSystem()
        {
            try
            {
                DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Debug, "UIBootstrap: Initializing font system");

                // Deterministic synchronous font initialization
                FontManager.InitializeAsync().GetAwaiter().GetResult();

                var titleFont = FontManager.LoadTitleFont();
                var textFont = FontManager.LoadTextFont();
                var iconFont = FontManager.LoadIconFont();
                var smallFont = FontManager.LoadSmallFont();

                var allFontsLoaded =
                    titleFont != null &&
                    textFont != null &&
                    iconFont != null &&
                    smallFont != null;

                if (allFontsLoaded)
                {
                    DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Debug, "UIBootstrap: All fonts loaded successfully");
                    return true;
                }

                DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Warning, "UIBootstrap: Some fonts failed to load");
                return false;
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Error, $"UIBootstrap: Font system initialization failed - {ex.Message}");
                return false;
            }
        }

        private bool InitializeElementFactories()
        {
            try
            {
                DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Debug, "UIBootstrap: Initializing UI element factories");

                UIElementFactory.Initialize();

                DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Debug, "UIBootstrap: UI element factories initialized");
                return true;
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Error, $"UIBootstrap: Element factory initialization failed - {ex.Message}");
                return false;
            }
        }

        private bool RegisterUIComponents()
        {
            try
            {
                DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Debug, "UIBootstrap: Registering UI components");

                UIComponentRegistry.RegisterComponent<HUDController>();
                UIComponentRegistry.RegisterComponent<TowerInfoPanel>();
                UIComponentRegistry.RegisterComponent<UpgradePanel>();
                UIComponentRegistry.RegisterComponent<PlacementInfoDisplay>();

                DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Debug, "UIBootstrap: UI components registered successfully");
                return true;
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Error, $"UIBootstrap: Component registration failed - {ex.Message}");
                return false;
            }
        }

        private bool InitializeEventSystems()
        {
            try
            {
                DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Debug, "UIBootstrap: Initializing UI event systems");

                UIEventSystem.Initialize();

                DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Debug, "UIBootstrap: UI event systems initialized");
                return true;
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Error, $"UIBootstrap: Event system initialization failed - {ex.Message}");
                return false;
            }
        }

        internal UIBootstrapStatistics GetStatistics()
        {
            return new UIBootstrapStatistics
            {
                Initialized = _initialized,
                FontStatistics = FontManager.GetStatistics(),
                InitializationTime = DateTime.Now
            };
        }
    }

    internal sealed class UIBootstrapStatistics
    {
        internal bool Initialized { get; set; }
        internal FontCacheStatistics FontStatistics { get; set; }
        internal DateTime InitializationTime { get; set; }

        public override string ToString()
        {
            return $"UI Bootstrap: Initialized={Initialized}, {FontStatistics}";
        }
    }
}
