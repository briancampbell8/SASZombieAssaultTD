// ====================================================================================================
//  FILE: LogEnums.cs
//  PATH: Engine/Diagnostics/
//  PROGRAM: LogEnums.cs
//  MODULE: Diagnostics Pipeline (Core Types)
//
//  ROLE:
//      Defines the enumeration types used throughout the diagnostics pipeline,
//      including log levels, subsystems, and high-level classification categories.
//
//  ARCHITECTURAL NOTES:
//      - Enums must remain stable to preserve log consistency.
//      - No duplicates, no legacy noise, no ambiguous names.
// ====================================================================================================

namespace SASZombieAssaultTD.Engine.Diagnostics
{
    public class LogEnums
    {
        /// <summary>
        /// Defines the priority of a log entry.
        /// </summary>
        public enum LogPriority
        {
            High,
            Low,
            Normal,
            None
        }

        /// <summary>
        /// Defines the subsystem or engine domain that produced a log entry.
        /// </summary>
        public enum LogSubsystems
        {
            // Core Systems
            Audio = 0,

            Core,

            CoreColorize,
            CoreMath,
            CoreRandom,
            CoreTime,

            // Animation Systems
            Animation,

            AnimationBlendTree,
            AnimationComponents,
            AnimationCore,
            AnimationCoreTime,
            AnimationEvents,
            AnimationIntegration,
            AnimationSystems,

            // AI Systems
            AI,

            AIBehaviors,
            AIBlackboard,

            // Resource Management
            Resources,

            ResourcesAssets,
            ResourcesAssetsManager,
            ResourcesAssetsPipeline,
            ResourcesAssetsBundle,
            ResourcesAssetsIntegration,

            // Achievements
            Achievements,

            AchievementsUI,

            // Waves
            Waves,

            WavesWaveManagement,

            // Reporting & Diagnostics
            Reporting,

            ReportingDiagnostics,
            ReportingLogs,
            ReportingPatterns,
            Diagnostics,
            DiagnosticsLogs,
            DiagnosticsPatterns,

            // Systems
            Systems,

            Scenes,
            ScenesUI,
            Economy,
            EconomyUI,
            Towers,
            TowersUI,
            Window,
            WindowEvents,
            WindowEventsUI,
            SystemsGameplay,
            SystemsHazards,
            SystemsTiming,
            SystemsTowers,
            SystemsTowersTowerControl,
            SystemsTowersUpgrades,

            // Gameplay & UI
            Gameplay,

            Enemies,
            GameplayItems,
            GameplayTowers,
            HUD,
            UI,
            Layout,
            Managers,
            Menus,
            Statistics,
            Styles,
            Widgets,
            State,
            GameRoot,
            Root,

            // Engine Systems
            Engine,

            Rendering,
            Input,
            Physics,
            PhysicsComponents,
            Networking,
            Content,
            Scripting,
            ECS,
            ECSComponents,
            Navigation,

            // Fallback / Misc
            General,

            Unknown,
            Save,
            Platform,
            D3D11,
            Difficulty,
            Performance,
            GameLoop,
            Memory,
            Player,
            Timing,
            System,
            UIHUDPanels,
            UI_HUD,
            Events,
            Render,
            HUDOverlay,
            RenderSprites,
            ResourcesManager,
            Snapshot,
            SoftwareGraphics,
            UIRenderingModern,
            PhysicsCollision,
            ECSRuntime,
            GamePlayFactory,
            ResourcesPipeline,
            UISystems,
            TowersTowerPlacement,
            ECSECSRuntime,
            ECSECSDebug,
            GraphicsSoftware,
            UIRenderer
        }

        /// <summary>
        /// Defines the severity level of a log entry. Clean, normalized, and deterministic.
        /// </summary>
        public enum LogLevel
        {
            Trace,
            Debug,
            Info,
            Warn,
            Error,
            Fatal,
            Critical,
            Warning,
            Exception,
            Recovery
        }

        /// <summary>
        /// Defines the high-level category of a log entry. Represents the "job title" of the program producing the log.
        /// </summary>
        public enum LogCategory
        {
            General,

            // Engine Domains
            Animation,

            Rendering,
            Gameplay,
            Audio,
            Networking,
            Physics,
            AI,
            Scripting,
            Engine,
            Content,
            Input,
            Diagnostics,

            // Error Domains
            Error,

            Exception,
            Debug,

            // New Architecture Categories (Job Titles)
            EntityCreation,

            ResourcesLoader,
            ResourcesCaching,
            ResourcesValidation,
            DeveloperTools,
            NavigationMigration,
            FramePresentation,
            TextureManagement,
            InputProcessing,
            AudioPlayback,
            Monitoring,
            System,
            Video,
            Serializing,
            Unknown,
            Transition,
            RenderSprites
        }

        /// <summary>
        /// Defines the severity of a log entry.
        /// </summary>
        public enum LogSeverity
        {
            None,
            Minor,
            Moderate,
            Severe,
            Critical
        }
    }
}