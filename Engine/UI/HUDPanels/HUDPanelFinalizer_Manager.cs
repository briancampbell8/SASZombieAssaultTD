// ====================================================================================================
//  FILE: HUDPanelFinalizer_Manager.cs
//  PATH: Engine/UI/HUDPanels/
//  MODULE: HUD Panel Finalizer Manager (Controlling Program)
//
//  ROLE:
//      Central orchestrator for the CASH HUD panel finalization pipeline. Coordinates manual override
//      control, color parsing, geometry/crosshair resolution, and UIState generation. Acts as the
//      controlling program that wires all smaller Finalizer modules together.
//
//  RESPONSIBILITIES:
//      - Own the lifecycle of HUDPanelFinalizer_Control, _ColorParser, _Resolver, and _UIStateBuilder.
//      - Load persisted configuration via HUDConfigManager and distribute it to submodules.
//      - Invoke manual override processing and color parsing in deterministic order.
//      - Resolve final geometry, colors, and crosshair configuration for HUDPanel_Cash.
//      - Produce UIState objects for the GPU UI pipeline via the UIStateBuilder.
//      - Emit DiagnosticEntry messages for all orchestration steps.
//
//  NON-RESPONSIBILITIES:
//      - Direct rendering of the HUD panel (handled by HUDPanel_Cash).
//      - Low-level color parsing logic (delegated to HUDPanelFinalizer_ColorParser).
//      - Manual override field management (delegated to HUDPanelFinalizer_Control).
//      - Geometry/crosshair normalization (delegated to HUDPanelFinalizer_Resolver).
//      - UIState element construction (delegated to HUDPanelFinalizer_UIStateBuilder).
//
//  ARCHITECTURAL NOTES:
//      - This manager is the single entry point for the CASH HUD finalization pipeline.
//      - All submodules are composed here to maintain deterministic ordering and traceability.
//      - Designed to be extensible for additional HUD panels and future Finalizer modules.
//      - Integrates with the diagnostics pipeline for full traceability across UI subsystems.
// ====================================================================================================
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.UI.HUDPanels;
using IDrawingContext = SASZombieAssaultTD.Engine.Rendering.IDrawingContext;

namespace SASZombieAssaultTD.Engine.UI
{
    public class HUDPanelFinalizer_Manager : IHUDElement
    {
        public HUDManager _hudManager;
        public HUDPanel_CashUpdate _cashPanel;

        private readonly HUDPanelFinalizer_Control _control;
        private readonly HUDPanelFinalizer_ColorParser _colorParser;
        private readonly HUDPanelFinalizer_Resolver _resolver;
        private readonly HUDPanelFinalizer_UIStateBuilder _uiStateBuilder;

        // Use the HUDPanels.UIState type, not Engine.UI.UIState
        private HUDPanels.UIState _cachedState;

        public string Id => "cash_panel_manager";
        public int Layer => 998;

        string IHUDElement.Id => Id;
        int IHUDElement.Layer => Layer;

        public HUDPanelFinalizer_Manager(
            HUDManager hudManager,
            HUDPanel_CashUpdate cashPanel,
            HUDPanelFinalizer_Control control,
            HUDPanelFinalizer_ColorParser colorParser,
            HUDPanelFinalizer_Resolver resolver,
            HUDPanelFinalizer_UIStateBuilder uiStateBuilder)
        {
            _hudManager = hudManager;
            _cashPanel = cashPanel;
            _control = control;
            _colorParser = colorParser;
            _resolver = resolver;
            _uiStateBuilder = uiStateBuilder;

            DLogger.Log("UI", "FinalizerManager",
                "HUDPanelFinalizer_Manager constructed and wired to submodules.",
                "FinalizerManager_Created", 0);
        }

        public void Initialize()
        {
            HUDConfigManager.LoadPanel("cash_panel", _control);

            ProcessManualOverrides();
            ResolveFinalState();
            BuildUIState();
        }

        private void ProcessManualOverrides()
        {
            var controlData = _control.GetManualEntryData();

            var fillResult = _colorParser.TryParse(controlData.FillColorToken);
            var textResult = _colorParser.TryParse(controlData.TextColorToken);
            var priceResult = _colorParser.TryParse(controlData.ItemPriceColorToken);

            if (!fillResult.Success || !textResult.Success || !priceResult.Success)
            {
                _control.SetErrorMessage(
                    fillResult.Error ?? textResult.Error ?? priceResult.Error
                );
                _control.SetManualOverrideEnabled(false);
                return;
            }

            _resolver.SetManualColors(fillResult.Color, textResult.Color);
            _resolver.SetItemPriceColor(priceResult.Color);
            _control.SetManualOverrideEnabled(true);
        }

        private void ResolveFinalState()
        {
            var controlData = _control.GetManualEntryData();

            _resolver.SetGeometry(controlData.X, controlData.Y, controlData.Width, controlData.Height);

            _resolver.SetCrosshair(
                controlData.CrosshairCenterX,
                controlData.CrosshairCenterY,
                controlData.CrosshairArmLength,
                controlData.CrosshairThickness,
                controlData.CrosshairColor,
                controlData.UseFlashColor,
                controlData.FlashColor
            );

            _resolver.Resolve();
        }

        private void BuildUIState()
        {
            var resolved = _resolver.GetResolvedState();

            _cachedState = _uiStateBuilder.Build(
                x: resolved.X,
                y: resolved.Y,
                width: resolved.Width,
                height: resolved.Height,
                fillColor: resolved.FillColor,
                textColor: resolved.TextColor,
                itemPriceColor: resolved.ItemPriceColor,
                crosshairCenterX: resolved.CrosshairCenterX,
                crosshairCenterY: resolved.CrosshairCenterY,
                crosshairArmLength: resolved.CrosshairArmLength,
                crosshairThickness: resolved.CrosshairThickness,
                crosshairColor: resolved.CrosshairColor,
                useFlashColor: resolved.UseFlashColor,
                flashColor: resolved.FlashColor
            );
        }

        // HUDManager uses this to get the resolved state
        public HUDPanelFinalizerResolvedState GetResolved()
        {
            return _resolver.GetResolvedState();
        }

        public void ApplyToPanel(HUDPanel_CashUpdate cashPanel, HUDPanelFinalizerResolvedState resolved)
        {
            if (resolved.ManualOverrideEnabled)
            {
                cashPanel.ManualItemPriceColor = resolved.ItemPriceColor;

                cashPanel.ManualFillColor = resolved.FillColor;
                cashPanel.ManualTextColor = resolved.TextColor;
                cashPanel.UseManualColors = true;
                cashPanel.DisplayText = "Zero";
            }
            else
            {
                cashPanel.UseManualColors = false;
            }
        }

        public void Update(float deltaTime)
        {
            _cashPanel.Update(deltaTime);
        }

        // Draw uses IDrawingContext, matching HUDPanel_CashUpdate.Draw
        public void Draw(IDrawingContext context)
        {
            _cashPanel.Draw(context);
        }

        // Return the HUDPanels.UIState type
        public HUDPanels.UIState GetUIState()
        {
            return _cachedState;
        }

        public void SaveConfig()
        {
            var resolved = _resolver.GetResolvedState();

            HUDConfigManager.SetPanel(
                "cash_panel",
                resolved.X,
                resolved.Y,
                resolved.Width,
                resolved.Height,
                Layer
            );

            HUDConfigManager.SetCrosshair(
                "cash_panel",
                resolved.CrosshairCenterX,
                resolved.CrosshairCenterY,
                resolved.CrosshairArmLength,
                resolved.CrosshairThickness,
                resolved.CrosshairColor,
                resolved.UseFlashColor,
                resolved.FlashColor
            );
        }
    }

    public struct HUDPanelFinalizerControlData
    {
        public int X, Y, Width, Height;
        public string FillColorToken, TextColorToken;
        public string ItemPriceColorToken;
        public int CrosshairCenterX, CrosshairCenterY, CrosshairArmLength, CrosshairThickness;
        public System.Drawing.Color CrosshairColor, FlashColor;
        public bool UseFlashColor;
    }

    public struct HUDPanelFinalizerResolvedState
    {
        public int X, Y, Width, Height;
        public Color FillColor, TextColor, ItemPriceColor;
        public int CrosshairCenterX, CrosshairCenterY, CrosshairArmLength, CrosshairThickness;
        public System.Drawing.Color CrosshairColor, FlashColor;
        public bool UseFlashColor;
        public bool ManualOverrideEnabled;
    }
}
