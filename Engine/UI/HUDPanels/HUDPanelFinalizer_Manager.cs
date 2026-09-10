// =====================================================================================================
//  FILE: HUDPanelFinalizer_Manager.cs
//  PATH: Engine/UI/HUDPanels/HUDPanelFinalizer_Manager.cs
//  MODULE: HUD Finalizer Pipeline Coordinator
//  LAYER: UI → HUDPanels → Finalizer
//
//  ROLE:
//      Manager of the HUDPanelFinalizer subsystem. Coordinates the deterministic pipeline:
//      Control → ColorParser → Resolver → UIStateBuilder. Produces a resolved UIState and applies
//      it to the CASH panel. Exposes resolved state and panel metrics for HUDOverlay diagnostics.
//
//  RESPONSIBILITIES:
//      - Own and sequence the Finalizer pipeline.
//      - Process manual color overrides.
//      - Resolve geometry and normalized color state.
//      - Build the final UIState consumed by HUDPanel_CashUpdate.
//      - Apply resolved state to the CASH panel.
//      - Expose resolved state and panel metrics for HUDOverlay diagnostics.
//      - Emit full tracing for all pipeline phases.
//
//  NON-RESPONSIBILITIES:
//      - Rendering (delegated to HUDPanel_CashUpdate).
//      - UI tree participation (no UIElement inheritance).
//      - HUDManager integration (removed).
//      - HUDConfigManager integration (removed).
// =====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.UI.HUDPanels;

namespace SASZombieAssaultTD.Engine.UI
{
    /// <summary>
    /// Manager-only coordinator for the HUDPanel Finalizer pipeline.
    /// </summary>
    public sealed class HUDPanelFinalizer_Manager
    {
        private readonly HUDPanel_CashUpdate _cashPanel;
        private readonly HUDPanelFinalizer_Control _control;
        private readonly HUDPanelFinalizer_ColorParser _colorParser;
        private readonly HUDPanelFinalizer_Resolver _resolver;
        private readonly HUDPanelFinalizer_UIStateBuilder _uiStateBuilder;

        private HUDPanels.UIState? _cachedState;

        // Manual override fields
        public Color ManualFillColor { get; set; } = Color.FromArgb(32, 32, 32);
        public Color ManualTextColor { get; set; } = Color.FromArgb(0, 224, 255);
        public bool ManualColorOverrideEnabled { get; set; } = false;

        /// <summary>
        /// Logical layer used only for diagnostics (HUDOverlay).
        /// </summary>
        public int Layer { get; } = 998;

        public HUDPanelFinalizer_Manager(
            HUDPanel_CashUpdate cashPanel,
            HUDPanelFinalizer_Control control,
            HUDPanelFinalizer_ColorParser colorParser,
            HUDPanelFinalizer_Resolver resolver,
            HUDPanelFinalizer_UIStateBuilder uiStateBuilder)
        {
            _cashPanel = cashPanel ?? throw new ArgumentNullException(nameof(cashPanel));
            _control = control ?? throw new ArgumentNullException(nameof(control));
            _colorParser = colorParser ?? throw new ArgumentNullException(nameof(colorParser));
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _uiStateBuilder = uiStateBuilder ?? throw new ArgumentNullException(nameof(uiStateBuilder));

            DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Info,
                "HUDPanelFinalizer_Manager: Constructed and wired to submodules.");
        }

        // -------------------------------------------------------------------------------------------------
        //  PIPELINE SEQUENCING
        // -------------------------------------------------------------------------------------------------

        public void Initialize()
        {
            DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Info,
                "HUDPanelFinalizer_Manager: Initialize ENTRY.");

            ProcessManualOverrides();
            ResolveFinalState();
            BuildUIState();

            DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Info,
                "HUDPanelFinalizer_Manager: Initialize EXIT.");
        }

        public void ProcessManualOverrides()
        {
            DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Trace,
                "HUDPanelFinalizer_Manager: Processing manual overrides.");

            _colorParser.ParseColors(
                ManualFillColor,
                ManualTextColor,
                ManualColorOverrideEnabled
            );
        }

        public void ResolveFinalState()
        {
            DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Trace,
                "HUDPanelFinalizer_Manager: Resolving final geometry and color state.");

            _resolver.ResolveGeometry(
                _control.X,
                _control.Y,
                _control.Width,
                _control.Height,
                _colorParser
            );
        }

        public void BuildUIState()
        {
            DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Trace,
                "HUDPanelFinalizer_Manager: Building UIState.");

            _cachedState = _uiStateBuilder.ConstructState(_resolver);
            ApplyToPanel(_cashPanel, _cachedState);
        }

        // -------------------------------------------------------------------------------------------------
        //  STATE ACCESSORS (HUDOverlay Diagnostics)
        // -------------------------------------------------------------------------------------------------

        public HUDPanels.UIState GetResolvedState()
        {
            if (_cachedState == null)
            {
                DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Warn,
                    "HUDPanelFinalizer_Manager: Cached state missing; rebuilding.");
                BuildUIState();
            }

            return _cachedState!;
        }

        public HUDPanel_CashUpdate GetCashPanel()
        {
            return _cashPanel;
        }

        public bool UseManualPanelColors => ManualColorOverrideEnabled;

        public FinalizerElement GetActiveElement()
        {
            var state = GetResolvedState();

            return new FinalizerElement
            {
                Id = "cash_panel",
                AnchorPanelId = "cash_panel",
                PanelX = state.PanelX,
                PanelY = state.PanelY,
                PanelWidth = state.PanelWidth,
                PanelHeight = state.PanelHeight
            };
        }

        // -------------------------------------------------------------------------------------------------
        //  STATE APPLICATION
        // -------------------------------------------------------------------------------------------------

        public void ApplyToPanel(HUDPanel_CashUpdate panel, HUDPanels.UIState state)
        {
            if (panel == null || state == null)
                return;

            panel.X = state.PanelX;
            panel.Y = state.PanelY;
            panel.Width = state.PanelWidth;
            panel.Height = state.PanelHeight;

            panel.ManualFillColor = state.PanelFillColor;
            panel.ManualTextColor = state.PanelTextColor;
            panel.ManualItemPriceColor = state.ItemPriceColor;
            panel.UseManualColors = state.UseManualColors;

            DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Trace,
                "HUDPanelFinalizer_Manager: Applied resolved state to CASH panel.");
        }
    }

    /// <summary>
    /// Lightweight descriptor used by HUDOverlayDiagnostics.
    /// </summary>
    public sealed class FinalizerElement
    {
        public string Id { get; set; } = "cash_panel";
        public string AnchorPanelId { get; set; } = "cash_panel";

        public float PanelX { get; set; }
        public float PanelY { get; set; }
        public float PanelWidth { get; set; }
        public float PanelHeight { get; set; }
    }
}
